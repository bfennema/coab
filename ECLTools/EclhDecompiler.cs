using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eclh
{
    // ── Operand ───────────────────────────────────────────────────────────────

    public enum OperandKind { ByteImm, WordRef, Code02, WordImm, StringInline, StringPtr }

    public class Operand
    {
        public OperandKind Kind;
        public ushort      Word;          // address or 16-bit literal
        public byte        ByteVal;       // for ByteImm
        public string      StringVal;     // for StringInline / StringPtr

        // Bytes this operand occupies (for size calculations)
        public int Size;

        /// <summary>True when this operand carries an ECL address (code 0x01 or 0x03).</summary>
        public bool IsAddress => Kind == OperandKind.WordRef || Kind == OperandKind.WordImm;

        public override string ToString()
        {
            return Kind switch
            {
                OperandKind.ByteImm      => $"#{ByteVal}",
                OperandKind.WordRef      => $"[${Word:X4}]",
                OperandKind.Code02       => $"?[${Word:X4}]",
                OperandKind.WordImm      => $"##{Word:X4}",
                OperandKind.StringInline => $"\"{EscapeString(StringVal)}\"",
                OperandKind.StringPtr    => $"@[${Word:X4}]",
                _                        => "???"
            };
        }

        private static string EscapeString(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    // ── Instruction ───────────────────────────────────────────────────────────

    public class Instruction
    {
        public ushort           Address;
        public byte             Opcode;
        public string           Mnemonic;
        public List<Operand>    Operands = new();
        public int              ByteLength;   // total bytes including opcode byte

        /// <summary>
        /// True for terminal instructions that have no fall-through successor.
        /// </summary>
        public bool IsTerminal =>
            Opcode is 0x00 or 0x13 or 0x20 or 0x24;
            // EXIT, RETURN, NEWECL, COMBAT all stop sequential flow.
            // GOTO (0x01) is also terminal for fall-through but handled separately.

        public bool IsGoto    => Opcode == 0x01;
        public bool IsGosub   => Opcode == 0x02;
        public bool IsOnGoto  => Opcode == 0x25;
        public bool IsOnGosub => Opcode == 0x26;
        public bool IsIf      => Opcode >= 0x16 && Opcode <= 0x1B;
        public bool IsCompare => Opcode == 0x03 || Opcode == 0x14;

        public string IfOp => Opcode switch
        {
            0x16 => "==", 0x17 => "!=", 0x18 => "<",
            0x19 => ">",  0x1A => "<=", 0x1B => ">=",
            _    => "?"
        };

        public string IfOpNegated => Opcode switch
        {
            0x16 => "!=", 0x17 => "==", 0x18 => ">=",
            0x19 => "<=", 0x1A => ">",  0x1B => "<",
            _    => "?"
        };

        public override string ToString()
        {
            var ops = string.Join(", ", Operands.Select(o => o.ToString()));
            return $"${Address:X4}: {Opcode:X2} {Mnemonic,-22} {ops}".TrimEnd();
        }
    }

    // ── Opcode table entry ────────────────────────────────────────────────────

    internal class OpcodeInfo
    {
        public string Mnemonic;
        public int    FixedOperands;   // -1 = variable (see VariableArityRules)
    }

    // ── Decompiler ────────────────────────────────────────────────────────────

    /// <summary>
    /// Standalone ECL decompiler.  Works on a raw byte[] without the live game
    /// runtime.  Uses worklist-based CFG traversal so data intermixed with code
    /// is never misidentified as instructions.
    /// </summary>
    public class EclhDecompiler
    {
        // ── Configuration ─────────────────────────────────────────────────────

        /// <summary>
        /// The ECL base address for this block (initial_ecl_offset).
        /// File byte[2] corresponds to ECL address Base.
        /// </summary>
        public ushort Base { get; }

        /// <summary>
        /// Pool of Radiance engine function addresses.  Override for other games.
        /// </summary>
        public Dictionary<ushort, string> EngineFunctions { get; set; } = new()
        {
            [0x2C90] = "redraw",
            [0x8000] = "duel_player",
            [0x8001] = "duel_monster",
            [0xBA03] = "play_sound",
            [0xC01E] = "move_forward",
            [0xC018] = "get_wall",
            [0x6803] = "demo_frame",
        };

        /// <summary>
        /// Hardware register addresses (map position, wall type, engine params etc.)
        /// Named separately from mem_regions since they are engine state, not ECL variables.
        /// </summary>
        public Dictionary<ushort, string> HardwareRegisters { get; set; } = new()
        {
            // Map state (vm_GetMemoryValue case 4, offset from 0xC04B)
            [0xC04B] = "map_x",
            [0xC04C] = "map_y",
            [0xC04D] = "map_direction",
            [0xC04E] = "map_wall_type",
            [0xC04F] = "map_wall_roof",

            // Engine parameter registers (write before engine function call)
            [0x03DE] = "sound_param",    // 8 = sound_a, 10 = sound_b; written before play_sound()
            [0x00B8] = "engine_b8",      // word_1EE78 — internal engine state
            [0x00B9] = "engine_b9",      // word_1EE7A — internal engine state
        };

        /// <summary>
        /// Memory region classification for variable names.
        /// Key = (start, end inclusive), Value = region label.
        /// </summary>
        public List<(ushort Start, ushort End, string Label)> MemRegions { get; set; } = new()
        {
            (0x4900, 0x4CFF, "area"),
            (0x6B00, 0x6EFF, "player"),
            (0x9700, 0x98FF, "shared"),
        };

        // ── Private state ─────────────────────────────────────────────────────

        private readonly byte[]   _data;       // raw file bytes (includes 2-byte DAX prefix)
        private readonly int      _fileSize;

        // CFG traversal results
        private readonly SortedDictionary<ushort, Instruction> _instructions = new();
        private readonly HashSet<ushort> _codeBytes  = new();
        private readonly HashSet<ushort> _visited    = new();

        // Labelling
        private readonly HashSet<ushort> _gosubTargets = new();
        private readonly HashSet<ushort> _gotoTargets  = new();
        private readonly HashSet<ushort> _dataAddrs    = new();   // GETTABLE/SAVETABLE bases

        // Friendly names assigned in the naming pass
        private readonly Dictionary<ushort, string> _labelNames = new();
        private readonly Dictionary<ushort, string> _varNames   = new();

        // ── Opcode table ──────────────────────────────────────────────────────

        private static readonly Dictionary<byte, OpcodeInfo> Opcodes = new()
        {
            [0x00] = new() { Mnemonic = "EXIT",            FixedOperands = 0  },
            [0x01] = new() { Mnemonic = "GOTO",            FixedOperands = 1  },
            [0x02] = new() { Mnemonic = "GOSUB",           FixedOperands = 1  },
            [0x03] = new() { Mnemonic = "COMPARE",         FixedOperands = 2  },
            [0x04] = new() { Mnemonic = "ADD",             FixedOperands = 3  },
            [0x05] = new() { Mnemonic = "SUBTRACT",        FixedOperands = 3  },
            [0x06] = new() { Mnemonic = "DIVIDE",          FixedOperands = 3  },
            [0x07] = new() { Mnemonic = "MULTIPLY",        FixedOperands = 3  },
            [0x08] = new() { Mnemonic = "RANDOM",          FixedOperands = 2  },
            [0x09] = new() { Mnemonic = "SAVE",            FixedOperands = 2  },
            [0x0A] = new() { Mnemonic = "LOAD CHARACTER",  FixedOperands = 1  },
            [0x0B] = new() { Mnemonic = "LOAD MONSTER",    FixedOperands = 3  },
            [0x0C] = new() { Mnemonic = "SETUP MONSTER",   FixedOperands = 3  },
            [0x0D] = new() { Mnemonic = "APPROACH",        FixedOperands = 0  },
            [0x0E] = new() { Mnemonic = "PICTURE",         FixedOperands = 1  },
            [0x0F] = new() { Mnemonic = "INPUT NUMBER",    FixedOperands = 2  },
            [0x10] = new() { Mnemonic = "INPUT STRING",    FixedOperands = 2  },
            [0x11] = new() { Mnemonic = "PRINT",           FixedOperands = 1  },
            [0x12] = new() { Mnemonic = "PRINTCLEAR",      FixedOperands = 1  },
            [0x13] = new() { Mnemonic = "RETURN",          FixedOperands = 0  },
            [0x14] = new() { Mnemonic = "COMPARE AND",     FixedOperands = 4  },
            [0x15] = new() { Mnemonic = "VERTICAL MENU",   FixedOperands = -1 }, // var
            [0x16] = new() { Mnemonic = "IF =",            FixedOperands = 0  },
            [0x17] = new() { Mnemonic = "IF <>",           FixedOperands = 0  },
            [0x18] = new() { Mnemonic = "IF <",            FixedOperands = 0  },
            [0x19] = new() { Mnemonic = "IF >",            FixedOperands = 0  },
            [0x1A] = new() { Mnemonic = "IF <=",           FixedOperands = 0  },
            [0x1B] = new() { Mnemonic = "IF >=",           FixedOperands = 0  },
            [0x1C] = new() { Mnemonic = "CLEARMONSTERS",   FixedOperands = 0  },
            [0x1D] = new() { Mnemonic = "PARTYSTRENGTH",   FixedOperands = 1  },
            [0x1E] = new() { Mnemonic = "CHECKPARTY",      FixedOperands = 6  },
            [0x1F] = new() { Mnemonic = "NOTSURE_1F",      FixedOperands = 2  },
            [0x20] = new() { Mnemonic = "NEWECL",          FixedOperands = 1  },
            [0x21] = new() { Mnemonic = "LOAD FILES",      FixedOperands = 3  },
            [0x22] = new() { Mnemonic = "PARTY SURPRISE",  FixedOperands = 2  },
            [0x23] = new() { Mnemonic = "SURPRISE",        FixedOperands = 4  },
            [0x24] = new() { Mnemonic = "COMBAT",          FixedOperands = 0  },
            [0x25] = new() { Mnemonic = "ON GOTO",         FixedOperands = -1 }, // var
            [0x26] = new() { Mnemonic = "ON GOSUB",        FixedOperands = -1 }, // var
            [0x27] = new() { Mnemonic = "TREASURE",        FixedOperands = 8  },
            [0x28] = new() { Mnemonic = "ROB",             FixedOperands = 3  },
            [0x29] = new() { Mnemonic = "ENCOUNTER MENU",  FixedOperands = 14 },
            [0x2A] = new() { Mnemonic = "GETTABLE",        FixedOperands = 3  },
            [0x2B] = new() { Mnemonic = "HORIZONTAL MENU", FixedOperands = -1 }, // var
            [0x2C] = new() { Mnemonic = "PARLAY",          FixedOperands = 6  },
            [0x2D] = new() { Mnemonic = "CALL",            FixedOperands = 1  },
            [0x2E] = new() { Mnemonic = "DAMAGE",          FixedOperands = 5  },
            [0x2F] = new() { Mnemonic = "AND",             FixedOperands = 3  },
            [0x30] = new() { Mnemonic = "OR",              FixedOperands = 3  },
            [0x31] = new() { Mnemonic = "SPRITE OFF",      FixedOperands = 0  },
            [0x32] = new() { Mnemonic = "FIND ITEM",       FixedOperands = 1  },
            [0x33] = new() { Mnemonic = "PRINT RETURN",    FixedOperands = 0  },
            [0x34] = new() { Mnemonic = "ECL CLOCK",       FixedOperands = 1  },
            [0x35] = new() { Mnemonic = "SAVE TABLE",      FixedOperands = 3  },
            [0x36] = new() { Mnemonic = "ADD NPC",         FixedOperands = 2  },
            [0x37] = new() { Mnemonic = "LOAD PIECES",     FixedOperands = 3  },
            [0x38] = new() { Mnemonic = "PROGRAM",         FixedOperands = 1  },
            [0x39] = new() { Mnemonic = "WHO",             FixedOperands = 1  },
            [0x3A] = new() { Mnemonic = "DELAY",           FixedOperands = 0  },
            [0x3B] = new() { Mnemonic = "SPELL",           FixedOperands = 3  },
            [0x3C] = new() { Mnemonic = "PROTECTION",      FixedOperands = 1  },
            [0x3D] = new() { Mnemonic = "CLEAR BOX",       FixedOperands = 0  },
            [0x3E] = new() { Mnemonic = "DUMP",            FixedOperands = 0  },
            [0x3F] = new() { Mnemonic = "FIND SPECIAL",    FixedOperands = 1  },
            [0x40] = new() { Mnemonic = "DESTROY ITEMS",   FixedOperands = 1  },
        };

        // ── Constructor ───────────────────────────────────────────────────────

        public EclhDecompiler(byte[] rawFileBytes, ushort baseAddress)
        {
            _data     = rawFileBytes;
            _fileSize = rawFileBytes.Length;
            Base      = baseAddress;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Run all decompiler passes and return the ECLH source text.
        /// </summary>
        public string Decompile()
        {
            var entryPoints = ParseHeader();
            TraverseCfg(entryPoints);
            AnalyseReferences();
            AssignNames();
            return EmitSource(entryPoints);
        }

        /// <summary>
        /// Exposes decoded instructions for external analysis.
        /// Call after Decompile() or TraverseCfg() + AnalyseReferences().
        /// </summary>
        public IReadOnlyDictionary<ushort, Instruction> Instructions => _instructions;

        // ── Pass 0: Header ────────────────────────────────────────────────────

        private (ushort onMove, ushort onSearch, ushort onPreCamp,
                 ushort onCampInterrupted, ushort onEnter) ParseHeader()
        {
            // File layout:
            //   [0..1]  = 2-byte DAX prefix (skipped)
            //   [2..21] = 5 × GOTO instructions, each [0x01][0x01][lo][hi]
            //   [22..]  = executable code
            //
            // ECL address = (file_offset - 2) + Base
            // File offset = (ecl_addr - Base) + 2

            if (_fileSize < 22)
                throw new InvalidOperationException("File too small to contain ECL header");

            ushort ReadEntryPoint(int fileOffset)
            {
                byte op   = _data[fileOffset];
                byte code = _data[fileOffset + 1];
                byte lo   = _data[fileOffset + 2];
                byte hi   = _data[fileOffset + 3];

                if (op != 0x01)
                    throw new InvalidOperationException(
                        $"Expected GOTO (0x01) at file offset 0x{fileOffset:X4}, got 0x{op:X2}");

                return (ushort)(lo + (hi << 8));
            }

            return (
                onMove:             ReadEntryPoint(2),
                onSearch:           ReadEntryPoint(6),
                onPreCamp:          ReadEntryPoint(10),
                onCampInterrupted:  ReadEntryPoint(14),
                onEnter:            ReadEntryPoint(18)
            );
        }

        // ── Pass 1: CFG traversal ─────────────────────────────────────────────

        private void TraverseCfg(
            (ushort onMove, ushort onSearch, ushort onPreCamp,
             ushort onCampInterrupted, ushort onEnter) entryPoints)
        {
            var worklist = new Queue<ushort>();

            void Enqueue(ushort addr)
            {
                if (!_visited.Contains(addr) && IsInRange(addr))
                    worklist.Enqueue(addr);
            }

            Enqueue(entryPoints.onMove);
            Enqueue(entryPoints.onSearch);
            Enqueue(entryPoints.onPreCamp);
            Enqueue(entryPoints.onCampInterrupted);
            Enqueue(entryPoints.onEnter);

            while (worklist.Count > 0)
            {
                ushort addr = worklist.Dequeue();

                if (_visited.Contains(addr)) continue;
                if (!IsInRange(addr))        continue;

                _visited.Add(addr);

                Instruction? instr = TryDecodeAt(addr);
                if (instr == null)
                {
                    // Not a valid instruction — treat as data, stop this path
                    continue;
                }

                _instructions[addr] = instr;

                // Mark every byte of this instruction as code
                for (int b = 0; b < instr.ByteLength; b++)
                    _codeBytes.Add((ushort)(addr + b));

                // Compute successors
                ushort fallThrough = (ushort)(addr + instr.ByteLength);

                if (instr.IsGoto)
                {
                    // GOTO target only — no fall-through
                    if (instr.Operands.Count > 0 && instr.Operands[0].IsAddress)
                        Enqueue(instr.Operands[0].Word);
                }
                else if (instr.IsGosub)
                {
                    // Both the subroutine target AND fall-through after return
                    if (instr.Operands.Count > 0 && instr.Operands[0].IsAddress)
                        Enqueue(instr.Operands[0].Word);
                    Enqueue(fallThrough);
                }
                else if (instr.IsOnGoto || instr.IsOnGosub)
                {
                    // All address operands beyond op1 and op2 are jump targets.
                    // Op1 = index value, op2..opN = target addresses.
                    for (int i = 1; i < instr.Operands.Count; i++)
                        if (instr.Operands[i].IsAddress)
                            Enqueue(instr.Operands[i].Word);
                    // Fall-through for out-of-range index
                    Enqueue(fallThrough);
                }
                else if (instr.IsIf)
                {
                    // Two paths: condition-true skips next instruction,
                    // condition-false falls through to next instruction.
                    // We need the size of the next instruction to find the skip target.
                    // Add both fall-through and skip-over-next as successors.
                    Enqueue(fallThrough);   // condition false: next instruction

                    // Peek at the next instruction's size to find the skip target
                    Instruction? next = TryDecodeAt(fallThrough);
                    if (next != null)
                        Enqueue((ushort)(fallThrough + next.ByteLength));  // condition true: skip
                }
                else if (instr.IsTerminal)
                {
                    // No successors
                }
                else
                {
                    // All other instructions: fall through
                    Enqueue(fallThrough);
                }
            }
        }

        // ── Pass 2: Reference analysis ────────────────────────────────────────

        private void AnalyseReferences()
        {
            foreach (var instr in _instructions.Values)
            {
                // Classify GOTO/GOSUB targets
                if (instr.IsGoto && instr.Operands.Count > 0)
                    _gotoTargets.Add(instr.Operands[0].Word);

                if (instr.IsGosub && instr.Operands.Count > 0)
                    _gosubTargets.Add(instr.Operands[0].Word);

                if ((instr.IsOnGoto || instr.IsOnGosub) && instr.Operands.Count > 1)
                {
                    var set = instr.IsOnGoto ? _gotoTargets : _gosubTargets;
                    for (int i = 1; i < instr.Operands.Count; i++)
                        if (instr.Operands[i].IsAddress)
                            set.Add(instr.Operands[i].Word);
                }

                // Identify GETTABLE / SAVETABLE base addresses as data tables
                if (instr.Opcode == 0x2A && instr.Operands.Count > 0)   // GETTABLE
                    if (instr.Operands[0].IsAddress && !_codeBytes.Contains(instr.Operands[0].Word))
                        _dataAddrs.Add(instr.Operands[0].Word);

                if (instr.Opcode == 0x35 && instr.Operands.Count > 1)   // SAVE TABLE
                    if (instr.Operands[1].IsAddress && !_codeBytes.Contains(instr.Operands[1].Word))
                        _dataAddrs.Add(instr.Operands[1].Word);
            }
        }

        // ── Pass 3: Name assignment ────────────────────────────────────────────

        // Addresses of instructions whose immediately preceding decoded instruction
        // is NOT a COMPARE — these are flag-reuse IFs.
        private readonly HashSet<ushort> _flagReuseIfs = new();

        private void AssignNames()
        {
            // Labels
            foreach (ushort addr in _gosubTargets)
                if (!_labelNames.ContainsKey(addr))
                    _labelNames[addr] = $"sub_{addr:X4}";

            foreach (ushort addr in _gotoTargets)
                if (!_labelNames.ContainsKey(addr))
                    _labelNames[addr] = $"loc_{addr:X4}";

            // Walk instructions in order to detect flag-reuse IFs
            var ordered = _instructions.Values.OrderBy(i => i.Address).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                var instr = ordered[i];
                if (!instr.IsIf) continue;
                bool preceded_by_compare = i > 0 && ordered[i - 1].IsCompare;
                if (!preceded_by_compare)
                    _flagReuseIfs.Add(instr.Address);
            }

            // Variables: any 0x01 (WordRef), 0x03 (WordImm), or 0x81 (StringPtr)
            // operand whose address falls in a mem_region.
            // ##addr (WordImm) operands in mem regions are functionally variable refs.
            foreach (var instr in _instructions.Values)
            {
                foreach (var op in instr.Operands)
                {
                    if (op.Kind != OperandKind.WordRef  &&
                        op.Kind != OperandKind.WordImm  &&
                        op.Kind != OperandKind.StringPtr)
                        continue;

                    ushort addr = op.Word;

                    if (HardwareRegisters.ContainsKey(addr)) continue;
                    if (_varNames.ContainsKey(addr))          continue;

                    string? regionLabel = ClassifyMemAddr(addr);
                    if (regionLabel != null)
                        _varNames[addr] = $"{regionLabel}_{addr:X4}";
                }
            }
        }

        // ── Pass 4: Source emission ───────────────────────────────────────────

        private string EmitSource(
            (ushort onMove, ushort onSearch, ushort onPreCamp,
             ushort onCampInterrupted, ushort onEnter) ep)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"@base   0x{Base:X4}");
            sb.AppendLine($"@game   pool_of_radiance");
            sb.AppendLine();

            sb.AppendLine($"on_move              = {ResolveLabel(ep.onMove)}");
            sb.AppendLine($"on_search            = {ResolveLabel(ep.onSearch)}");
            sb.AppendLine($"on_pre_camp          = {ResolveLabel(ep.onPreCamp)}");
            sb.AppendLine($"on_camp_interrupted  = {ResolveLabel(ep.onCampInterrupted)}");
            sb.AppendLine($"on_enter             = {ResolveLabel(ep.onEnter)}");
            sb.AppendLine();

            // Var block
            var sortedVars = _varNames.OrderBy(kv => kv.Key).ToList();
            if (sortedVars.Count > 0)
            {
                sb.AppendLine("var {");
                foreach (var (addr, name) in sortedVars)
                    sb.AppendLine($"    word  {name,-20} @ 0x{addr:X4}");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // Hardware registers used in this module
            var usedHw = new SortedDictionary<ushort, string>();
            foreach (var instr in _instructions.Values)
                foreach (var op in instr.Operands)
                    if (op.IsAddress && HardwareRegisters.TryGetValue(op.Word, out string? hwn))
                        usedHw.TryAdd(op.Word, hwn);

            if (usedHw.Count > 0)
            {
                sb.AppendLine("// Hardware registers referenced in this module:");
                foreach (var (addr, name) in usedHw)
                    sb.AppendLine($"// hardware_reg {name,-20} = 0x{addr:X4}");
                sb.AppendLine();
            }

            // Data tables
            var sortedData = _dataAddrs.OrderBy(a => a).ToList();
            if (sortedData.Count > 0)
            {
                sb.AppendLine("data {");
                foreach (ushort tableBase in sortedData)
                {
                    string tname = $"tbl_{tableBase:X4}";
                    string bytes = ReadDataBytes(tableBase);
                    sb.AppendLine($"    byte  {tname,-20} @ 0x{tableBase:X4} = {{ {bytes} }}");
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }

            // Structured code emission
            var allAddrs = _instructions.Keys.OrderBy(a => a).ToList();
            EmitRange(sb, allAddrs, 0, allAddrs.Count, indent: "");

            return sb.ToString();
        }

        /// <summary>
        /// Emit instructions from allAddrs[start..end) with structured if/while
        /// reconstruction.  Recursively handles nested blocks.
        /// </summary>
        private void EmitRange(StringBuilder sb, List<ushort> allAddrs,
                               int start, int end, string indent)
        {
            int i = start;
            while (i < end)
            {
                ushort addr = allAddrs[i];
                var instr = _instructions[addr];

                // Emit any labels at this address
                EmitLabels(sb, addr, indent);

                // ── Try to reconstruct while loop ────────────────────────────
                // Pattern at loop top: COMPARE + IF<negated> + GOTO_forward (guard)
                // Pattern at loop bottom: some instr, then GOTO_backward (back-edge)
                if (TryEmitWhile(sb, allAddrs, i, end, indent, out int whileConsumed))
                {
                    i += whileConsumed;
                    continue;
                }

                // ── Try to reconstruct block-if / if-else ────────────────────
                if (TryEmitBlockIf(sb, allAddrs, i, end, indent, out int ifConsumed))
                {
                    i += ifConsumed;
                    continue;
                }

                // ── do/while: back-edge GOTO with fresh COMPARE just before ──
                // (handled by the while detector above looking backward from GOTO)

                // ── Plain instruction ─────────────────────────────────────────
                sb.AppendLine($"{indent}    {FormatInstruction(instr)}");
                i++;
            }
        }

        private void EmitLabels(StringBuilder sb, ushort addr, string indent)
        {
            if (!_labelNames.TryGetValue(addr, out string? lname)) return;

            // Blank line before labels for readability (except at very start)
            sb.AppendLine();
            sb.AppendLine($"{indent}{lname} @ 0x{addr:X4}:");
        }

        /// <summary>
        /// Attempt to emit a block-if or if/else starting at allAddrs[i].
        /// Returns true and sets consumed to the number of address-list slots consumed.
        /// Pattern: COMPARE + IF<op> + GOTO_fwd [+ body + GOTO_after]
        ///          where the guard GOTO target is a label reachable only from inside
        ///          the if structure (i.e. not targeted from outside).
        /// </summary>
        private bool TryEmitBlockIf(StringBuilder sb, List<ushort> allAddrs,
                                    int i, int end, string indent, out int consumed)
        {
            consumed = 0;
            if (i + 2 >= end) return false;

            var cmpInstr  = _instructions[allAddrs[i]];
            var ifInstr   = _instructions[allAddrs[i+1]];
            var gotoInstr = _instructions[allAddrs[i+2]];

            if (!cmpInstr.IsCompare)  return false;
            if (!ifInstr.IsIf)        return false;
            if (_flagReuseIfs.Contains(ifInstr.Address)) return false; // emit raw
            if (!gotoInstr.IsGoto)    return false;
            if (gotoInstr.Operands.Count == 0) return false;

            ushort guardTarget = gotoInstr.Operands[0].Word;
            if (guardTarget <= gotoInstr.Address) return false;

            int guardIdx = allAddrs.BinarySearch(guardTarget);
            if (guardIdx < 0 || guardIdx >= end) return false;

            int bodyStart = i + 3;
            int bodyEnd   = guardIdx;
            if (bodyStart > bodyEnd) return false;

            // If any address inside the body is targeted from outside the whole
            // if-structure, we cannot safely lift it — emit flat.
            if (HasExternalJumpsIntoRange(allAddrs, bodyStart, bodyEnd, i, guardIdx))
                return false;

            // If the body contains a flag-reuse IF (no COMPARE immediately before it),
            // this is a multi-way branch, not a simple block-if — emit flat.
            if (BodyContainsFlagReuseIf(allAddrs, bodyStart, bodyEnd))
                return false;

            // Check for else: body ends with GOTO forward past guardTarget
            bool hasElse  = false;
            int elseStart = guardIdx;
            int elseEnd   = guardIdx;

            if (bodyEnd > bodyStart)
            {
                var lastBodyInstr = _instructions[allAddrs[bodyEnd - 1]];
                if (lastBodyInstr.IsGoto && lastBodyInstr.Operands.Count > 0)
                {
                    ushort elseTgt = lastBodyInstr.Operands[0].Word;
                    if (elseTgt > guardTarget)
                    {
                        int afterIdx = allAddrs.BinarySearch(elseTgt);
                        if (afterIdx >= 0 && afterIdx <= end &&
                            !HasExternalJumpsIntoRange(allAddrs, guardIdx, afterIdx,
                                                       bodyStart - 3, afterIdx))
                        {
                            elseStart = guardIdx;
                            elseEnd   = afterIdx;
                            hasElse   = true;
                            bodyEnd--;  // exclude terminal GOTO from body
                        }
                    }
                }
            }

            string condition = FormatCondition(cmpInstr, UnNegateOp(ifInstr.IfOp));

            sb.AppendLine();
            sb.AppendLine($"{indent}    if ({condition}) {{");
            EmitRange(sb, allAddrs, bodyStart, bodyEnd, indent + "    ");
            sb.AppendLine($"{indent}    }}");

            if (hasElse)
            {
                sb.AppendLine($"{indent}    else {{");
                EmitRange(sb, allAddrs, elseStart, elseEnd, indent + "    ");
                sb.AppendLine($"{indent}    }}");
                consumed = elseEnd - i;
            }
            else
            {
                consumed = guardIdx - i;
            }

            return true;
        }

        private bool TryEmitWhile(StringBuilder sb, List<ushort> allAddrs,
                                  int i, int end, string indent, out int consumed)
        {
            consumed = 0;
            if (i + 2 >= end) return false;

            var cmpInstr  = _instructions[allAddrs[i]];
            var ifInstr   = _instructions[allAddrs[i+1]];
            var gotoInstr = _instructions[allAddrs[i+2]];

            if (!cmpInstr.IsCompare) return false;
            if (!ifInstr.IsIf)       return false;
            if (_flagReuseIfs.Contains(ifInstr.Address)) return false;
            if (!gotoInstr.IsGoto || gotoInstr.Operands.Count == 0) return false;

            ushort guardTarget = gotoInstr.Operands[0].Word;
            if (guardTarget <= gotoInstr.Address) return false;

            int guardIdx = allAddrs.BinarySearch(guardTarget);
            if (guardIdx < 0 || guardIdx >= end) return false;
            if (guardIdx - 1 < i + 3) return false;

            // Last instruction before guard must be GOTO back to compare address
            var lastInstr = _instructions[allAddrs[guardIdx - 1]];
            if (!lastInstr.IsGoto || lastInstr.Operands.Count == 0) return false;
            if (lastInstr.Operands[0].Word != cmpInstr.Address)     return false;

            // No external jumps into the loop body
            if (HasExternalJumpsIntoRange(allAddrs, i + 3, guardIdx - 1, i, guardIdx))
                return false;

            string condition = FormatCondition(cmpInstr, UnNegateOp(ifInstr.IfOp));

            sb.AppendLine();
            sb.AppendLine($"{indent}    while ({condition}) {{");
            EmitRange(sb, allAddrs, i + 3, guardIdx - 1, indent + "    ");
            sb.AppendLine($"{indent}    }}");

            consumed = guardIdx - i;
            return true;
        }

        /// <summary>
        /// Returns true if any instruction in the given range is a flag-reuse IF —
        /// i.e. an IF whose immediately preceding decoded instruction is not a COMPARE.
        /// Such bodies contain multi-way branches and cannot be cleanly lifted into
        /// a structured block-if.
        /// </summary>
        private bool BodyContainsFlagReuseIf(List<ushort> allAddrs, int bodyStart, int bodyEnd)
        {
            for (int k = bodyStart; k < bodyEnd; k++)
                if (_flagReuseIfs.Contains(allAddrs[k]))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if any address in allAddrs[bodyStart..bodyEnd) is targeted
        /// by a jump from outside allAddrs[outerStart..outerEnd).
        /// Prevents lifting code into structured blocks when external labels exist.
        /// </summary>
        private bool HasExternalJumpsIntoRange(List<ushort> allAddrs,
                                               int bodyStart, int bodyEnd,
                                               int outerStart, int outerEnd)
        {
            if (bodyStart >= bodyEnd) return false;

            var bodyAddrs = new HashSet<ushort>();
            for (int k = bodyStart; k < bodyEnd; k++)
                bodyAddrs.Add(allAddrs[k]);

            for (int k = 0; k < allAddrs.Count; k++)
            {
                if (k >= outerStart && k < outerEnd) continue; // inside — OK
                var instr = _instructions[allAddrs[k]];
                foreach (var op in instr.Operands)
                    if (op.IsAddress && bodyAddrs.Contains(op.Word))
                        return true;
            }
            return false;
        }

        /// <summary>Reverse the negation applied when emitting a block-if guard GOTO.</summary>
        private static string UnNegateOp(string negatedOp) => negatedOp switch
        {
            "!=" => "==",
            "==" => "!=",
            ">=" => "<",
            "<=" => ">",
            ">"  => "<=",
            "<"  => ">=",
            _    => negatedOp
        };

        private string FormatCondition(Instruction cmpInstr, string op)
        {
            if (cmpInstr.Opcode == 0x03 && cmpInstr.Operands.Count >= 2)
                return $"{FormatOp(cmpInstr.Operands[0])} {op} {FormatOp(cmpInstr.Operands[1])}";

            if (cmpInstr.Opcode == 0x14 && cmpInstr.Operands.Count >= 4)
            {
                // COMPARE AND tests flag[0]=both-equal or flag[1]=either-differs.
                // The op passed here is already un-negated back to the source condition.
                // op=="==" means both pairs must be equal (COMPARE AND + IF<> guard)
                // op=="!=" means either pair differs
                string pairA = $"{FormatOp(cmpInstr.Operands[0])} == {FormatOp(cmpInstr.Operands[1])}";
                string pairB = $"{FormatOp(cmpInstr.Operands[2])} == {FormatOp(cmpInstr.Operands[3])}";
                return op == "==" ? $"{pairA} && {pairB}"
                                  : $"!({pairA} && {pairB})";
            }
            return "?";
        }

        // ── Instruction formatting ────────────────────────────────────────────

        private string FormatInstruction(Instruction instr)
        {
            // CALL: check engine functions
            if (instr.Opcode == 0x2D && instr.Operands.Count == 1 &&
                instr.Operands[0].IsAddress &&
                EngineFunctions.TryGetValue(instr.Operands[0].Word, out string? fname))
                return $"{fname}()";

            // GOTO / GOSUB
            if (instr.IsGoto && instr.Operands.Count > 0)
                return $"goto {ResolveLabel(instr.Operands[0].Word)}";

            if (instr.IsGosub && instr.Operands.Count > 0)
            {
                string target = ResolveLabel(instr.Operands[0].Word);
                return $"{target}()";
            }

            // IF: flag-reuse form vs normal form
            if (instr.IsIf)
            {
                if (_flagReuseIfs.Contains(instr.Address))
                    return $"if ({instr.IfOp})   // flag-reuse — no COMPARE emitted";
                else
                    return $"[IF {instr.IfOp}]";
            }

            // COMPARE
            if (instr.Opcode == 0x03 && instr.Operands.Count >= 2)
                return $"compare {FormatOp(instr.Operands[0])} vs {FormatOp(instr.Operands[1])}";

            if (instr.Opcode == 0x14 && instr.Operands.Count >= 4)
                return $"compare_and ({FormatOp(instr.Operands[0])}=={FormatOp(instr.Operands[1])}) && " +
                       $"({FormatOp(instr.Operands[2])}=={FormatOp(instr.Operands[3])})";

            // SAVE: dest = source
            // Special case: string literal source with string-pointer destination
            if (instr.Opcode == 0x09 && instr.Operands.Count >= 2)
            {
                string src  = FormatOp(instr.Operands[0]);
                string dest = FormatOp(instr.Operands[1]);
                return $"{dest} = {src}";
            }

            // ADD / SUB / MUL / DIV
            if (instr.Opcode is >= 0x04 and <= 0x07 && instr.Operands.Count >= 3)
            {
                string op = instr.Opcode switch { 0x04 => "+", 0x05 => "-", 0x06 => "/", 0x07 => "*", _ => "?" };
                if (instr.Opcode == 0x05)   // SUBTRACT: ECL stores (rhs, lhs, dest)
                    return $"{FormatOp(instr.Operands[2])} = {FormatOp(instr.Operands[1])} {op} {FormatOp(instr.Operands[0])}";
                return $"{FormatOp(instr.Operands[2])} = {FormatOp(instr.Operands[0])} {op} {FormatOp(instr.Operands[1])}";
            }

            // RANDOM
            if (instr.Opcode == 0x08 && instr.Operands.Count >= 2)
                return $"{FormatOp(instr.Operands[1])} = random({FormatOp(instr.Operands[0])})";

            // GETTABLE
            if (instr.Opcode == 0x2A && instr.Operands.Count >= 3)
                return $"{FormatOp(instr.Operands[2])} = {FormatTableRef(instr.Operands[0])}[{FormatOp(instr.Operands[1])}]";

            // SAVE TABLE
            if (instr.Opcode == 0x35 && instr.Operands.Count >= 3)
                return $"{FormatTableRef(instr.Operands[1])}[{FormatOp(instr.Operands[2])}] = {FormatOp(instr.Operands[0])}";

            // AND / OR
            if (instr.Opcode is 0x2F or 0x30 && instr.Operands.Count >= 3)
            {
                string op = instr.Opcode == 0x2F ? "&" : "|";
                return $"{FormatOp(instr.Operands[2])} = {FormatOp(instr.Operands[0])} {op} {FormatOp(instr.Operands[1])}";
            }

            // ON GOTO / ON GOSUB
            if (instr.IsOnGoto || instr.IsOnGosub)
            {
                string verb = instr.IsOnGoto ? "goto" : "call";
                string idx  = FormatOp(instr.Operands[0]);
                var targets  = instr.Operands.Skip(1)
                                    .Where(o => o.IsAddress)
                                    .Select(o => ResolveLabel(o.Word));
                return $"{verb}({idx}) {{ {string.Join(", ", targets)} }}";
            }

            // Generic fallback: mnemonic + operands
            var ops = string.Join(", ", instr.Operands.Select(FormatOp));
            return $"{instr.Mnemonic.ToLower().Replace(' ', '_')} {ops}".TrimEnd();
        }

        private string FormatOp(Operand op)
        {
            switch (op.Kind)
            {
                case OperandKind.WordRef:
                case OperandKind.WordImm:
                    // Hardware registers take priority
                    if (HardwareRegisters.TryGetValue(op.Word, out string? hwname))
                        return hwname;
                    // Named variables (from mem_regions)
                    if (_varNames.TryGetValue(op.Word, out string? vname))
                        return vname;
                    // Code labels
                    if (_labelNames.TryGetValue(op.Word, out string? lname))
                        return lname;
                    // Engine functions
                    if (EngineFunctions.TryGetValue(op.Word, out string? ename))
                        return ename;
                    // String pointers (0x81) handled below; for 0x01/0x03 unclassified:
                    string prefix = op.Kind == OperandKind.WordImm ? "##" : "";
                    return $"{prefix}[${op.Word:X4}]";

                case OperandKind.StringPtr:
                    // Named if in a mem region
                    if (_varNames.TryGetValue(op.Word, out string? spname))
                        return spname;
                    return $"@[${op.Word:X4}]";

                default:
                    return op.ToString();
            }
        }

        private string FormatTableRef(Operand op)
        {
            if (op.Kind == OperandKind.WordRef && _dataAddrs.Contains(op.Word))
                return $"tbl_{op.Word:X4}";
            return FormatOp(op);
        }

        private string ResolveLabel(ushort addr)
        {
            if (_labelNames.TryGetValue(addr, out string? name)) return name;
            return $"loc_{addr:X4}";
        }

        // ── Instruction decoder ───────────────────────────────────────────────

        /// <summary>
        /// Attempt to decode one instruction at the given ECL address.
        /// Returns null if the opcode is unknown or operands run past end of file.
        /// Does NOT modify any state — safe to call as a peek.
        /// </summary>
        private Instruction? TryDecodeAt(ushort addr)
        {
            int fileOff = FileOffset(addr);
            if (fileOff < 0 || fileOff >= _fileSize) return null;

            byte opcode = _data[fileOff];
            if (!Opcodes.TryGetValue(opcode, out OpcodeInfo? info)) return null;

            var instr = new Instruction
            {
                Address  = addr,
                Opcode   = opcode,
                Mnemonic = info.Mnemonic,
            };

            // cursor tracks a FILE offset throughout — never an ECL address.
            int cursor = fileOff + 1;

            if (info.FixedOperands >= 0)
            {
                for (int i = 0; i < info.FixedOperands; i++)
                {
                    Operand? op = TryReadOperand(ref cursor);
                    if (op == null) return null;
                    instr.Operands.Add(op);
                }

                // Variable-arity: ON GOTO (0x25), ON GOSUB (0x26), HORIZONTAL MENU (0x2B)
                // After 2 fixed operands, read count-more operands where count = op2 value.
                if (opcode is 0x25 or 0x26 or 0x2B)
                {
                    if (instr.Operands.Count < 2) return null;
                    int count = GetOperandValue(instr.Operands[1]);
                    for (int i = 0; i < count; i++)
                    {
                        Operand? op = TryReadOperand(ref cursor);
                        if (op == null) return null;
                        instr.Operands.Add(op);
                    }
                }
            }
            else
            {
                // VERTICAL MENU (0x15): 3 fixed ops, then count (= op3 value) extra ops.
                for (int i = 0; i < 3; i++)
                {
                    Operand? op = TryReadOperand(ref cursor);
                    if (op == null) return null;
                    instr.Operands.Add(op);
                }
                int count = GetOperandValue(instr.Operands[2]);
                for (int i = 0; i < count; i++)
                {
                    Operand? op = TryReadOperand(ref cursor);
                    if (op == null) return null;
                    instr.Operands.Add(op);
                }
            }

            instr.ByteLength = cursor - fileOff;
            return instr;
        }

        /// <summary>
        /// Read one CmdSet operand from the file at the given FILE cursor.
        /// Advances cursor by the operand's byte size.  Returns null on overrun.
        /// cursor is always a FILE offset, never an ECL address.
        /// </summary>
        private Operand? TryReadOperand(ref int cursor)
        {
            if (cursor >= _fileSize) return null;

            byte code = _data[cursor];
            cursor++;

            if (code == 0x00)
            {
                if (cursor >= _fileSize) return null;
                byte val = _data[cursor++];
                return new Operand { Kind = OperandKind.ByteImm, ByteVal = val, Size = 2 };
            }
            else if (code is 0x01 or 0x02 or 0x03)
            {
                if (cursor + 1 >= _fileSize) return null;
                byte lo = _data[cursor++];
                byte hi = _data[cursor++];
                ushort word = (ushort)(lo + (hi << 8));
                var kind = code switch
                {
                    0x01 => OperandKind.WordRef,
                    0x02 => OperandKind.Code02,
                    0x03 => OperandKind.WordImm,
                    _    => OperandKind.WordRef
                };
                return new Operand { Kind = kind, Word = word, Size = 3 };
            }
            else if (code == 0x80)
            {
                if (cursor >= _fileSize) return null;
                byte len = _data[cursor++];
                if (cursor + len > _fileSize) return null;
                byte[] compressed = new byte[len];
                Array.Copy(_data, cursor, compressed, 0, len);
                cursor += len;
                string str = DecompressString(compressed);
                return new Operand { Kind = OperandKind.StringInline, StringVal = str, Size = 2 + len };
            }
            else if (code == 0x81)
            {
                if (cursor + 1 >= _fileSize) return null;
                byte lo = _data[cursor++];
                byte hi = _data[cursor++];
                ushort word = (ushort)(lo + (hi << 8));
                return new Operand { Kind = OperandKind.StringPtr, Word = word, Size = 3 };
            }
            else
            {
                return null;   // unknown operand code → not a valid instruction
            }
        }

        // ── String decompression (ported from ovr008) ─────────────────────────

        private static char InflateChar(uint v)
        {
            if (v <= 0x1F) v += 0x40;
            return (char)v;
        }

        private static string DecompressString(byte[] data)
        {
            var sb    = new StringBuilder();
            int state = 1;
            uint lastByte = 0;

            foreach (uint b in data)
            {
                uint curr;
                switch (state)
                {
                    case 1:
                        curr = (b >> 2) & 0x3F;
                        if (curr != 0) sb.Append(InflateChar(curr));
                        state = 2;
                        break;
                    case 2:
                        curr = ((lastByte << 4) | (b >> 4)) & 0x3F;
                        if (curr != 0) sb.Append(InflateChar(curr));
                        state = 3;
                        break;
                    case 3:
                        curr = ((lastByte << 2) | (b >> 6)) & 0x3F;
                        if (curr != 0) sb.Append(InflateChar(curr));
                        curr = b & 0x3F;
                        if (curr != 0) sb.Append(InflateChar(curr));
                        state = 1;
                        break;
                }
                lastByte = b;
            }

            return sb.ToString();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private int FileOffset(ushort eclAddr) => (eclAddr - Base) + 2;

        private bool IsInRange(ushort eclAddr)
        {
            int off = FileOffset(eclAddr);
            return off >= 2 && off < _fileSize;
        }

        private static int GetOperandValue(Operand op) => op.Kind switch
        {
            OperandKind.ByteImm => op.ByteVal,
            OperandKind.WordRef => op.Word,
            OperandKind.WordImm => op.Word,
            _                   => 0
        };

        private string? ClassifyMemAddr(ushort addr)
        {
            foreach (var (start, end, label) in MemRegions)
                if (addr >= start && addr <= end)
                    return label;
            return null;
        }

        private string ReadDataBytes(ushort tableBase)
        {
            // Read until we hit a code byte or end of file; max 64 bytes
            var vals = new List<string>();
            ushort cur = tableBase;
            for (int i = 0; i < 64; i++)
            {
                if (_codeBytes.Contains(cur) || !IsInRange(cur)) break;
                vals.Add($"0x{_data[FileOffset(cur)]:X2}");
                cur++;
            }
            return string.Join(", ", vals);
        }
    }

    // ── Entry point for standalone use ────────────────────────────────────────

    public static class EclhDecompilerProgram
    {
        /// <summary>
        /// Decompile a raw ECL file and print the ECLH source to stdout.
        /// Usage: EclhDecompilerProgram.Run(rawBytes, baseAddress)
        /// </summary>
        public static string Run(byte[] rawBytes, ushort baseAddress = 0x9900)
        {
            var decompiler = new EclhDecompiler(rawBytes, baseAddress);
            return decompiler.Decompile();
        }

        /// <summary>
        /// Dump a raw instruction listing (no ECLH formatting) for analysis.
        /// </summary>
        public static void DumpRaw(byte[] rawBytes, ushort baseAddress = 0x9900)
        {
            var d = new EclhDecompiler(rawBytes, baseAddress);
            d.Decompile();   // run passes to populate Instructions

            Console.WriteLine($"Base: 0x{baseAddress:X4}   Instructions: {d.Instructions.Count}");
            Console.WriteLine(new string('-', 80));

            foreach (var instr in d.Instructions.Values)
                Console.WriteLine(instr);
        }
    }
}
