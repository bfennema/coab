using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ECLH Decompiler — ECL bytecode to ECLH source
//
// Changelog:
//   1.7.4 — Fixed _flagReuseIfs look-back regression from v1.7.3: the new
//            backward walk was skipping block-if guard GOTOs (which jump forward
//            past the current IF) as if they were single-action-if actions. Added
//            the key rule: only skip an IF/GOTO pair when the GOTO's target is at
//            or before the current IF's address (meaning it branches away, not
//            over). A GOTO whose target is past the current IF is a block-if guard
//            — a genuine control-flow break — and the look-back stops there,
//            correctly classifying the IF as flag-reuse.
//   1.7.3 — Generalized the _flagReuseIfs look-back fix from v1.7.0: instead of
//            looking back exactly one GOTO/COMPARE pair, now walks backward past
//            any number of single-action-if GOTO/IF pairs (each pair is COMPARE +
//            IF + GOTO) to find the COMPARE that originally set the flags. The
//            v1.7.0 fix only handled one prior single-action-if; chains of two or
//            more (as in ECL1_18's COMPARE/IF>/GOTO, COMPARE/IF>/GOTO, COMPARE/
//            IF== sequence) still incorrectly classified the last IF as flag-reuse.
//   1.7.2 — NEWECL confirmed genuinely terminal (it replaces the entire ECL in
//            memory; old code cannot continue). The 0x00 byte after NEWECL in
//            ECL6_1 is assembler padding, not reachable code. The compiler now
//            handles such gaps via zero-padding (EclhCompiler v0.2.1) rather than
//            treating them as CFG bugs.
//   1.7.1 — Fixed TryEmitBlockIf false-positive else detection: a GOTO at the
//            end of a then-body that is the action of a single-action if (part of
//            a COMPARE+IF+GOTO triple) was mistakenly treated as an else-skip jump
//            because its target was forward of the guard target. Added a check:
//            if the two preceding body instructions are COMPARE and IF, the GOTO
//            is part of that triple and is not an else-skip. This fixes the pattern
//            where two consecutive single-action ifs appear in a block-if body and
//            the last one jumps forward past the guard target.
//   1.7.0 — Fixed _flagReuseIfs detection: an IF immediately following a GOTO
//            that is itself immediately following a COMPARE was incorrectly
//            classified as flag-reuse (no preceding COMPARE). This occurs in
//            sequences like: COMPARE/IF/GOTO, COMPARE/IF/GOTO where the second
//            triple's IF was adjacent to the first triple's GOTO (not to its own
//            COMPARE) in the address-ordered instruction list. The GOTO is the
//            action of the first IF+GOTO pair, not a control-flow break between
//            the second COMPARE and the second IF. Fixed by extending the look-back
//            one extra step past a GOTO when checking for a preceding COMPARE.
//   1.6.9 — Added @tail { 0xXX, ... } directive: any bytes past the last decoded
//            instruction or data table are emitted as a verbatim tail block so the
//            compiler can reproduce the file byte-exactly. Addresses the 0xF7 (and
//            any similar) alignment-padding byte found at the very end of ECL6_1.
//            Added ComputeContentEnd() to find the high-water mark of all decoded
//            content (instructions via Operand.Size + data tables via _codeBytes
//            scan) and emit anything beyond it.
//   1.6.8 — Corrected the += compound-assignment shorthand to fire for the
//            destIsRhs case (ADD n, [x], [x] — amount as op0, variable as op1
//            and dest) rather than destIsLhs (ADD [x], n, [x]). The original
//            ECL compiler always uses the amount-first form for compound addition
//            — destIsLhs never appears in practice. The previous convention
//            (v1.6.5) was based on the wrong assumption about which form is
//            common. The compiler's LayoutCompoundAssign is updated to match,
//            so "x += n" round-trips as ADD n, [x], [x] byte-exactly.
//            SUB compound-assign (x -= n) is unchanged since its raw encoding
//            always had the variable as lhs after unswap, making destIsLhs
//            correct for SUB regardless.
//   1.6.7 — Audit pass: same ambiguity class as v1.6.4/1.6.6, found in
//            ResolveLabel. GOTO, GOSUB, CALL-as-engine-function, and ON GOTO/ON
//            GOSUB target lists all resolved jump targets to a bare label name
//            with no marker, regardless of whether the underlying operand was
//            WordRef (0x01) or WordImm (0x03) — never yet triggered in test data,
//            but the same unrecoverable ambiguity if it occurred. Added an
//            Operand-aware ResolveLabel overload that applies the ## marker
//            consistently, matching FormatOp's WordImm handling. Audited every
//            other operand-formatting path (FormatOp, FormatTableRef,
//            FormatCondition, Operand.ToString(), and the rest of
//            FormatInstruction) — no further instances found; all other
//            named-lookup sites already route through the fixed FormatOp.
//   1.6.6 — CRITICAL FIX: FormatOp's StringPtr case now emits @[name] for named
//            addresses instead of the bare name, eliminating the same WordRef/
//            StringPtr ambiguity class as the v1.6.4 WordImm marker fix. The
//            bare name was indistinguishable from a WordRef (0x01) reference to
//            the same address, even though StringPtr (0x81) encodes to different
//            bytes. Affects every StringPtr operand used as a plain argument
//            (e.g. "print shared_985E", or both operands of a COMPARE that
//            checks string-pointer identity directly, such as
//            "compare player_6B00 vs shared_9890" inside sub_AF56) — not just
//            SAVE destinations, which is the only case the earlier ToStringPtr
//            compiler workaround (v0.1.5) covered.
//   1.6.5 — CRITICAL FIX: removed the ambiguous "x += n" shorthand for the
//            destIsRhs case (ADD n, [x], [x] — amount as op0, dest as op1).
//            It produced text identical to the much more common destIsLhs case
//            (ADD [x], n, [x] — dest as op0), an unrecoverable ambiguity for
//            round-trip compilation since the two encode to different bytes
//            (same issue class as the x++ / ++x distinction). The destIsRhs case
//            now falls through to the explicit "dest = lhs + rhs" form, which
//            fully preserves operand order.
//   1.6.4 — CRITICAL FIX: FormatOp now preserves the ## WordImm marker even when
//            the address resolves to a named mem-region variable, label, hardware
//            register, or engine function. Previously the named-lookup branches
//            (hardware reg / var / label / engine func) returned the bare name
//            with no prefix regardless of operand kind, making WordRef (0x01) and
//            WordImm (0x03) operands referencing the same named address produce
//            IDENTICAL source text — an unrecoverable ambiguity for round-trip
//            compilation, since the two kinds encode to different bytes. The ##
//            marker previously only ever appeared for unclassified addresses
//            (the [$XXXX] fallback), which in practice almost never triggered
//            since most WordImm-in-mem-region addresses are named variables.
//   1.6.3 — CRITICAL FIX: reverted the redundant-GOTO suppression added in
//            v1.5.5. It was a lossy transformation: a GOTO whose target is the
//            immediately next instruction is still a real, addressable
//            instruction occupying real file bytes. Suppressing it from the
//            source for readability left the round-trip compiler with no way
//            to reconstruct those bytes — every such GOTO now caused a pinned-
//            label address mismatch in EclhCompiler for any subsequent label.
//            Correctness for round-trip compilation takes priority over this
//            minor readability improvement.
//   1.6.2 — CRITICAL FIX: COMBAT (0x24) removed from IsTerminal. COMBAT falls
//            through — subsequent code checks combat-result flags, and the same
//            fall-through path is used for shop entry (PICTURE #255 / EXIT
//            immediately after COMBAT is the shop-closed path). Previously the
//            CFG traversal treated COMBAT as having no fall-through successor,
//            silently omitting all reachable code immediately after it from the
//            decompiled output across every affected ECL file.
//   1.6.1 — EngineFunctions and HardwareRegisters are now static, so EclhCompiler
//            can reference the same profile directly via
//            Eclh.EclhDecompiler.EngineFunctions / .HardwareRegisters instead of
//            duplicating the address tables, eliminating profile drift between
//            the decompiler and compiler.
//   1.6.0 — Added do/while loop detection (TryEmitDoWhile). Condition at bottom,
//            not negated — GOTO fires when loop continues.
//   1.5.5 — Redundant GOTO suppressed when target is the immediately next instruction.
//   1.5.4 — Block-if lifting now rejected when body contains subroutine entry
//            points (GOSUB targets), preventing dispatch-table subroutines from
//            being incorrectly folded into a block body.
//   1.5.3 — x++ vs ++x distinguishes ADD operand order for round-trip fidelity.
//            x-- for SUB #1, [x], [x]. x+=n / x-=n for compound assignment.
//   1.5.2 — ADD/SUB emit x++, x--, x+=n, x-=n shorthand where applicable.
//   1.5.1 — Removed extra blank line between a label and the if/while that
//            immediately follows it.
//   1.5.0 — Labels always emit at column 0 regardless of nesting depth.
//            Fixed overlapping data table declarations — each table now capped
//            at the next table's base address.
//   1.4.0 — Removed IsGoto exclusion from TryEmitSingleIf/TryEmitFlagReuseIf;
//            all COMPARE+IF+GOTO now emit as single-action if statements.
//            Fixed FormatCondition for COMPARE AND to emit cleaner || form.
//   1.3.0 — Added BodyContainsFlagReuseIf safety check in TryEmitBlockIf;
//            fixed unconditional-terminal skip to not fire after conditional GOTOs.
//   1.2.0 — Fixed HORIZONTAL/VERTICAL MENU and ON GOTO/GOSUB FixedOperands;
//            unified variable-arity decode; added unreachable-code skip after
//            terminal instructions.
//   1.1.0 — Added entry point label naming (_entryPointNames / RecordEntry);
//            added TryEmitSingleIf and TryEmitFlagReuseIf for single-action if;
//            added hardware register support and StringPtr variable naming.
//   1.0.0 — Initial release. CFG traversal, structured if/else/while emission,
//            flag-reuse IF detection, game profile support.

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
            Opcode is 0x00 or 0x13 or 0x20;
            // EXIT, RETURN, NEWECL stop sequential flow.
            // GOTO (0x01) is also terminal for fall-through but handled separately.
            // COMBAT (0x24) falls through — subsequent code checks combat-result
            // flags, and the same fall-through is used for shop entry (PICTURE
            // #255 / EXIT immediately after COMBAT is the shop-closed path).
            // NEWECL (0x20) is genuinely terminal — it replaces the entire current
            // ECL in memory so execution of the old code cannot continue. Any byte
            // immediately after NEWECL is unreachable dead code (assembler padding).

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
        /// Pool of Radiance engine function addresses. Static — shared across all
        /// decompiler instances and directly referenceable by EclhCompiler via
        /// Eclh.EclhDecompiler.EngineFunctions, so the compiler's game profile can
        /// never drift out of sync with the decompiler's.
        /// </summary>
        public static Dictionary<ushort, string> EngineFunctions { get; set; } = new()
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
        /// Named separately from mem_regions since they are engine state, not ECL
        /// variables. Static for the same reason as EngineFunctions above.
        /// </summary>
        public static Dictionary<ushort, string> HardwareRegisters { get; set; } = new()
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
            [0x15] = new() { Mnemonic = "VERTICAL MENU",   FixedOperands = 3  }, // 3 fixed + count from op3
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
            [0x25] = new() { Mnemonic = "ON GOTO",         FixedOperands = 2  }, // 2 fixed + count from op2
            [0x26] = new() { Mnemonic = "ON GOSUB",        FixedOperands = 2  }, // 2 fixed + count from op2
            [0x27] = new() { Mnemonic = "TREASURE",        FixedOperands = 8  },
            [0x28] = new() { Mnemonic = "ROB",             FixedOperands = 3  },
            [0x29] = new() { Mnemonic = "ENCOUNTER MENU",  FixedOperands = 14 },
            [0x2A] = new() { Mnemonic = "GETTABLE",        FixedOperands = 3  },
            [0x2B] = new() { Mnemonic = "HORIZONTAL MENU", FixedOperands = 2  }, // 2 fixed + count from op2
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

        // Entry-point address → slot name, populated during TraverseCfg
        private readonly Dictionary<ushort, string> _entryPointNames = new();

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

            // Record entry-point addresses as named labels before seeding the worklist.
            // Multiple slots may point to the same address — first wins for the name.
            void RecordEntry(ushort addr, string name)
            {
                _entryPointNames.TryAdd(addr, name);
                Enqueue(addr);
            }

            RecordEntry(entryPoints.onMove,            "on_move");
            RecordEntry(entryPoints.onSearch,          "on_search");
            RecordEntry(entryPoints.onPreCamp,         "on_pre_camp");
            RecordEntry(entryPoints.onCampInterrupted, "on_camp_interrupted");
            RecordEntry(entryPoints.onEnter,           "on_enter");

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
            // Entry-point labels take priority over everything else
            foreach (var (addr, name) in _entryPointNames)
                _labelNames[addr] = name;

            // GOSUB targets — skip if already named as an entry point
            foreach (ushort addr in _gosubTargets)
                if (!_labelNames.ContainsKey(addr))
                    _labelNames[addr] = $"sub_{addr:X4}";

            // GOTO targets — skip if already named
            foreach (ushort addr in _gotoTargets)
                if (!_labelNames.ContainsKey(addr))
                    _labelNames[addr] = $"loc_{addr:X4}";

            // Walk instructions in order to detect flag-reuse IFs.
            // An IF is flag-reuse if there is no COMPARE instruction immediately
            // before it in sequential code flow. "Immediately before" means
            // looking back past GOTO instructions that are actions of prior
            // single-action ifs (COMPARE+IF+GOTO triples), since those GOTOs
            // don't reset the comparison flags.
            //
            // Key rule: only skip an IF/GOTO pair when the GOTO's target is at
            // or before the address of the IF we're currently testing. If the
            // GOTO's target is past the current IF, that GOTO jumps OVER it —
            // it's a block-if guard, not a single-action-if action — which is
            // a genuine control-flow break that resets what "preceding COMPARE"
            // means for the current IF.
            var ordered = _instructions.Values.OrderBy(i => i.Address).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                var instr = ordered[i];
                if (!instr.IsIf) continue;

                bool preceded_by_compare = false;
                int k = i - 1;
                while (k >= 0)
                {
                    if (ordered[k].IsCompare) { preceded_by_compare = true; break; }
                    // Only skip an IF/GOTO pair if the GOTO does NOT jump past
                    // the current IF (i.e. its target <= current IF's address).
                    // A GOTO that jumps past the current IF is a block-if guard.
                    if (ordered[k].IsGoto && k >= 1 && ordered[k - 1].IsIf)
                    {
                        ushort gotoTarget = ordered[k].Operands.Count > 0
                            ? ordered[k].Operands[0].Word : (ushort)0;
                        if (gotoTarget <= instr.Address)
                        {
                            k -= 2; // single-action-if action; safe to skip
                            continue;
                        }
                    }
                    break; // real control-flow break; stop looking back
                }

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

        // ── Version ───────────────────────────────────────────────────────────

        public const string Version = "1.7.4";

        // ── Pass 4: Source emission ───────────────────────────────────────────

        private string EmitSource(
            (ushort onMove, ushort onSearch, ushort onPreCamp,
             ushort onCampInterrupted, ushort onEnter) ep)
        {
            var sb = new StringBuilder();

            // File header
            sb.AppendLine($"// Generated by ECLH Decompiler v{Version}");
            sb.AppendLine($"// Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"// Instructions decoded: {_instructions.Count}");
            sb.AppendLine();

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

            // Tail bytes — any bytes past the last known content (instruction or
            // data table) that are unreachable and not part of any named structure.
            // Stored verbatim so the compiler can reproduce the file byte-exactly.
            int tailStart = ComputeContentEnd();
            if (tailStart < _fileSize)
            {
                var tailBytes = Enumerable.Range(tailStart, _fileSize - tailStart)
                                          .Select(i => $"0x{_data[i]:X2}");
                sb.AppendLine();
                sb.AppendLine($"@tail {{ {string.Join(", ", tailBytes)} }}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Computes the file offset of the first byte past the last decoded
        /// instruction or data table — any bytes from here to end-of-file are
        /// unreachable tail padding.
        /// </summary>
        private int ComputeContentEnd()
        {
            int end = 2 + 20; // DAX prefix + header

            // High-water mark from decoded instructions
            foreach (var (addr, instr) in _instructions)
            {
                int instrEnd = FileOffset(addr) + 1; // opcode byte
                foreach (var op in instr.Operands)
                    instrEnd += op.Kind switch
                    {
                        OperandKind.ByteImm => 2,
                        OperandKind.WordRef => 3,
                        OperandKind.Code02 => 3,
                        OperandKind.WordImm => 3,
                        OperandKind.StringPtr => 3,
                        OperandKind.StringInline => op.Size,
                        _ => 0
                    };
                if (instrEnd > end) end = instrEnd;
            }

            // High-water mark from data tables
            foreach (ushort tableBase in _dataAddrs)
            {
                int tblOff = FileOffset(tableBase);
                int scan = tblOff;
                while (scan < _fileSize && !_codeBytes.Contains((ushort)(Base + scan - 2)))
                    scan++;
                if (scan > end) end = scan;
            }

            return end;
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

                // ── Try to reconstruct do/while loop ─────────────────────────
                // Looks at the entire remaining range for a back-edge to current addr.
                if (_labelNames.ContainsKey(addr) &&
                    TryEmitDoWhile(sb, allAddrs, i, end, indent, out int doWhileConsumed))
                {
                    i += doWhileConsumed;
                    continue;
                }

                // ── Try to reconstruct while loop ────────────────────────────
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

                // ── Single-action if: COMPARE + IF<op> + one instruction ──────
                if (TryEmitSingleIf(sb, allAddrs, i, end, indent, out int singleConsumed))
                {
                    i += singleConsumed;
                    continue;
                }

                // ── Flag-reuse single-action if: IF<op> + one instruction ─────
                if (TryEmitFlagReuseIf(sb, allAddrs, i, end, indent, out int frConsumed))
                {
                    i += frConsumed;
                    continue;
                }

                // ── Plain instruction ─────────────────────────────────────────
                // NOTE: a prior version (v1.5.5) suppressed GOTO instructions whose
                // target was the immediately next instruction, treating them as
                // redundant no-op jumps for readability. That suppression was
                // reverted in v1.6.3: every such GOTO is a real, addressable
                // instruction occupying real bytes in the file, and omitting it
                // from the source left the compiler with no way to reconstruct
                // those bytes, breaking byte-exact round-trip compilation. Some of
                // these "redundant" GOTOs may also exist because the original
                // compiler emitted them deliberately (e.g. as a stable jump target
                // for code shared across multiple paths) — they are not actually
                // meaningless even though they look like no-ops in isolation.
                sb.AppendLine($"{indent}    {FormatInstruction(instr)}");
                i++;

                // After an unconditional terminal (standalone GOTO, EXIT, RETURN,
                // NEWECL) skip any following instructions that have no label — they
                // are unreachable by sequential fall-through.
                // Do NOT skip after a GOTO that is the action of an IF opcode —
                // that GOTO is conditional and the next instruction is still reachable
                // when the condition is false.
                bool prevIsIf = i >= 2 && _instructions[allAddrs[i - 2]].IsIf;
                bool isUnconditionalTerminal =
                    (instr.IsGoto && !prevIsIf) || instr.IsTerminal;

                if (isUnconditionalTerminal)
                {
                    while (i < end && !_labelNames.ContainsKey(allAddrs[i]))
                        i++;
                }
            }
        }

        private void EmitLabels(StringBuilder sb, ushort addr, string indent)
        {
            if (!_labelNames.TryGetValue(addr, out string? lname)) return;

            // Labels always at column 0 regardless of nesting depth —
            // they are jump targets reachable from anywhere in the file.
            sb.AppendLine();
            sb.AppendLine($"{lname} @ 0x{addr:X4}:");
        }

        /// <summary>
        /// Attempt to emit a block-if or if/else starting at allAddrs[i].
        /// Returns true and sets consumed to the number of address-list slots consumed.
        /// Pattern: COMPARE + IF<op> + GOTO_fwd [+ body + GOTO_after]
        ///          where the guard GOTO target is a label reachable only from inside
        ///          the if structure (i.e. not targeted from outside).
        /// </summary>
        /// <summary>
        /// Fuse COMPARE + IF&lt;op&gt; + single_action into:
        ///     if (lhs op rhs) action
        ///
        /// The operator is used directly — no negation — because for single-action
        /// if, the action executes when the flag is true.
        ///
        /// Does NOT fuse when:
        ///   - the IF is a flag-reuse IF (no COMPARE immediately before it)
        ///   - the action is a GOTO (block-if handles those)
        ///   - the action is itself a COMPARE (would hide a chained compare)
        /// </summary>
        private bool TryEmitSingleIf(StringBuilder sb, List<ushort> allAddrs,
                                     int i, int end, string indent, out int consumed)
        {
            consumed = 0;
            if (i + 2 >= end) return false;

            var cmpInstr    = _instructions[allAddrs[i]];
            var ifInstr     = _instructions[allAddrs[i + 1]];
            var actionInstr = _instructions[allAddrs[i + 2]];

            if (!cmpInstr.IsCompare)  return false;
            if (!ifInstr.IsIf)        return false;
            if (_flagReuseIfs.Contains(ifInstr.Address)) return false;

            // Don't fuse if the action is itself a COMPARE — it would be confusing
            if (actionInstr.IsCompare) return false;

            // Use the IF operator directly (no negation for single-action form)
            string condition = FormatCondition(cmpInstr, ifInstr.IfOp);
            string action    = FormatInstruction(actionInstr);

            sb.AppendLine($"{indent}    if ({condition}) {action}");
            consumed = 3;
            return true;
        }

        /// <summary>
        /// Fuse a flag-reuse IF + single_action into:
        ///     if (op) action    // bare operator — no COMPARE emitted
        ///
        /// Used when the IF has no COMPARE immediately before it and instead
        /// relies on flags set by an earlier COMPARE.
        /// </summary>
        private bool TryEmitFlagReuseIf(StringBuilder sb, List<ushort> allAddrs,
                                        int i, int end, string indent, out int consumed)
        {
            consumed = 0;
            if (i + 1 >= end) return false;

            var ifInstr     = _instructions[allAddrs[i]];
            var actionInstr = _instructions[allAddrs[i + 1]];

            if (!ifInstr.IsIf)                              return false;
            if (!_flagReuseIfs.Contains(ifInstr.Address))   return false;
            if (actionInstr.IsCompare)                      return false;

            string action = FormatInstruction(actionInstr);
            sb.AppendLine($"{indent}    if ({ifInstr.IfOp}) {action}   // flag-reuse");
            consumed = 2;
            return true;
        }

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

            // If the body contains subroutine entry points (GOSUB targets), those are
            // separate functions that happen to live in this address range — lifting
            // them into the block body produces misleading structure.
            if (BodyContainsSubroutine(allAddrs, bodyStart, bodyEnd))
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
                        // Make sure this GOTO is a standalone unconditional jump,
                        // not the action of a single-action if (COMPARE+IF+GOTO).
                        // If the two preceding instructions are COMPARE and IF, this
                        // GOTO is part of that triple and is NOT an else-skip jump.
                        bool isElseSkip = true;
                        if (bodyEnd - bodyStart >= 3)
                        {
                            var prev1 = _instructions[allAddrs[bodyEnd - 2]];
                            var prev2 = _instructions[allAddrs[bodyEnd - 3]];
                            if (prev1.IsIf && prev2.IsCompare)
                                isElseSkip = false;
                        }

                        if (isElseSkip)
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
            }

            string condition = FormatCondition(cmpInstr, UnNegateOp(ifInstr.IfOp));

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

            sb.AppendLine($"{indent}    while ({condition}) {{");
            EmitRange(sb, allAddrs, i + 3, guardIdx - 1, indent + "    ");
            sb.AppendLine($"{indent}    }}");

            consumed = guardIdx - i;
            return true;
        }

        /// <summary>
        /// Attempt to emit a do/while loop starting at allAddrs[i].
        /// Pattern:
        ///   _loop:                        ← allAddrs[i] is the loop-top label
        ///   body...
        ///   COMPARE + IF&lt;op&gt; + GOTO _loop  ← last three instructions before end
        ///
        /// The condition is NOT negated — the GOTO fires when the loop continues.
        /// Only lifts when:
        ///   - allAddrs[i] is in _labelNames (has a label = potential loop target)
        ///   - the last three instructions form COMPARE + IF + GOTO_backward to allAddrs[i]
        ///   - no external jumps into the body except to the loop-top label itself
        ///   - body contains no subroutines or flag-reuse IFs
        /// </summary>
        private bool TryEmitDoWhile(StringBuilder sb, List<ushort> allAddrs,
                                    int i, int end, string indent, out int consumed)
        {
            consumed = 0;
            if (end - i < 4) return false;  // need at least: body + CMP + IF + GOTO

            ushort loopTop = allAddrs[i];

            // Last three slots must be COMPARE + IF + GOTO_back
            int gotoPos = end - 1;
            int ifPos   = end - 2;
            int cmpPos  = end - 3;
            if (cmpPos <= i) return false;

            var gotoInstr = _instructions[allAddrs[gotoPos]];
            var ifInstr   = _instructions[allAddrs[ifPos]];
            var cmpInstr  = _instructions[allAddrs[cmpPos]];

            if (!gotoInstr.IsGoto || gotoInstr.Operands.Count == 0) return false;
            if (!ifInstr.IsIf)                                       return false;
            if (_flagReuseIfs.Contains(ifInstr.Address))             return false;
            if (!cmpInstr.IsCompare)                                 return false;

            // GOTO must go backward to the loop top
            if (gotoInstr.Operands[0].Word != loopTop) return false;

            // Body: allAddrs[i .. cmpPos)
            int bodyStart = i;
            int bodyEnd   = cmpPos;

            // No external jumps into body (excluding the loop-top label itself)
            // The loop-top IS expected to be a label target — that's fine.
            // Check for external jumps to any body address OTHER than the loop top.
            var bodyAddrs = new HashSet<ushort>();
            for (int k = bodyStart + 1; k < bodyEnd; k++)  // skip loopTop itself
                bodyAddrs.Add(allAddrs[k]);

            foreach (var instr in _instructions.Values)
            {
                // Skip instructions inside the whole range
                int idx = allAddrs.BinarySearch(instr.Address);
                if (idx >= bodyStart && idx < end) continue;
                foreach (var op in instr.Operands)
                    if (op.IsAddress && bodyAddrs.Contains(op.Word))
                        return false;
            }

            if (BodyContainsFlagReuseIf(allAddrs, bodyStart, bodyEnd)) return false;
            if (BodyContainsSubroutine(allAddrs,  bodyStart + 1, bodyEnd)) return false;

            // Condition is used directly (not negated) — GOTO fires when loop continues
            string condition = FormatCondition(cmpInstr, ifInstr.IfOp);

            sb.AppendLine($"{indent}    do {{");
            EmitRange(sb, allAddrs, bodyStart, bodyEnd, indent + "    ");
            sb.AppendLine($"{indent}    }} while ({condition})");

            consumed = end - i;
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
        /// Returns true if any address in the body range is a GOSUB target —
        /// meaning it's a subroutine entry point, not just sequential block code.
        /// Prevents folding subroutine definitions into a block-if body.
        /// </summary>
        private bool BodyContainsSubroutine(List<ushort> allAddrs, int bodyStart, int bodyEnd)
        {
            for (int k = bodyStart; k < bodyEnd; k++)
                if (_gosubTargets.Contains(allAddrs[k]))
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
                // COMPARE AND sets flag[0] when both pairs equal, flag[1] when either differs.
                // op is the un-negated source condition:
                //   "==" → both pairs must be equal  → emit (a == b && c == d)
                //   "!=" → either pair must differ   → emit (a != b || c != d)
                string a = FormatOp(cmpInstr.Operands[0]);
                string b = FormatOp(cmpInstr.Operands[1]);
                string c = FormatOp(cmpInstr.Operands[2]);
                string d = FormatOp(cmpInstr.Operands[3]);
                return op == "==" ? $"{a} == {b} && {c} == {d}"
                                  : $"{a} != {b} || {c} != {d}";
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
            {
                string callPrefix = instr.Operands[0].Kind == OperandKind.WordImm ? "##" : "";
                return $"{callPrefix}{fname}()";
            }

            // GOTO / GOSUB
            if (instr.IsGoto && instr.Operands.Count > 0)
                return $"goto {ResolveLabel(instr.Operands[0])}";

            if (instr.IsGosub && instr.Operands.Count > 0)
            {
                string target = ResolveLabel(instr.Operands[0]);
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

                // For ADD and SUB, check for increment/decrement and compound assignment
                if (instr.Opcode is 0x04 or 0x05)
                {
                    // SUBTRACT operand order is (rhs, lhs, dest) — normalise to lhs/rhs
                    Operand lhs  = instr.Opcode == 0x05 ? instr.Operands[1] : instr.Operands[0];
                    Operand rhs  = instr.Opcode == 0x05 ? instr.Operands[0] : instr.Operands[1];
                    Operand dest = instr.Operands[2];

                    string destStr = FormatOp(dest);
                    string lhsStr  = FormatOp(lhs);
                    string rhsStr  = FormatOp(rhs);

                    bool rhsIsOne  = rhs.Kind == OperandKind.ByteImm && rhs.ByteVal == 1;
                    bool lhsIsOne  = lhs.Kind == OperandKind.ByteImm && lhs.ByteVal == 1;
                    bool destIsLhs = destStr == lhsStr;
                    bool destIsRhs = destStr == rhsStr;  // ADD: #1 + x -> x (commutative)

                    // Use post-increment (x++) when x is op0 (ADD [x], #1, [x])
                    // Use pre-increment (++x) when #1 is op0 (ADD #1, [x], [x])
                    // These produce different binary encodings and must be distinguishable.
                    // Similarly x-- vs --x for subtraction.
                    if (instr.Opcode == 0x04 && rhsIsOne && destIsLhs) return $"{destStr}++";
                    if (instr.Opcode == 0x04 && lhsIsOne && destIsRhs) return $"++{destStr}";
                    if (instr.Opcode == 0x05 && rhsIsOne && destIsLhs) return $"{destStr}--";

                    // x += n  — emit shorthand when dest is op1 (rhs): ADD n, [x], [x]
                    // (amount is op0, variable is op1 and also dest). This is the form
                    // the original ECL compiler always produces for compound addition —
                    // ADD [x], n, [x] (destIsLhs) never appears in practice.
                    // The compiler maps "x += n" back to ADD n, [x], [x] to match.
                    // SUB is unchanged: SUB n, [x], [x] normalises to lhs=x, rhs=n,
                    // so destIsLhs is always true for SUB compound-assign and correctly
                    // emits "x -= n".
                    if (instr.Opcode == 0x04 && destIsRhs) return $"{destStr} += {lhsStr}";

                    return $"{destStr} = {lhsStr} {op} {rhsStr}";
                }

                // MUL / DIV — no shorthand, but still normalise output
                if (instr.Opcode == 0x05)
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
                                    .Select(o => ResolveLabel(o));
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
                    // The ## marker for WordImm must be preserved regardless of whether
                    // the address also resolves to a named variable/label/engine func —
                    // WordRef (0x01) and WordImm (0x03) produce different bytes even when
                    // they reference the exact same address, so the text must always be
                    // able to distinguish them for round-trip compilation. Only the
                    // generic [$XXXX] fallback form omitted the marker for named cases;
                    // fixed by applying the prefix uniformly at the top.
                    string wPrefix = op.Kind == OperandKind.WordImm ? "##" : "";

                    // Hardware registers take priority
                    if (HardwareRegisters.TryGetValue(op.Word, out string? hwname))
                        return wPrefix + hwname;
                    // Named variables (from mem_regions)
                    if (_varNames.TryGetValue(op.Word, out string? vname))
                        return wPrefix + vname;
                    // Code labels
                    if (_labelNames.TryGetValue(op.Word, out string? lname))
                        return wPrefix + lname;
                    // Engine functions
                    if (EngineFunctions.TryGetValue(op.Word, out string? ename))
                        return wPrefix + ename;
                    // Unclassified — raw address form
                    return $"{wPrefix}[${op.Word:X4}]";

                case OperandKind.StringPtr:
                    // @[name] marker must be preserved even when the address resolves
                    // to a named variable — StringPtr (0x81) and WordRef (0x01)
                    // referencing the same address produce different bytes, so the
                    // text must always distinguish them (same issue class as the ##
                    // WordImm marker fix in v1.6.4). Reuses the existing @[symbol]
                    // syntax (already parsed by EclhCompiler) rather than the bare
                    // name, which would be indistinguishable from a WordRef.
                    if (_varNames.TryGetValue(op.Word, out string? spname))
                        return $"@[{spname}]";
                    if (_labelNames.TryGetValue(op.Word, out string? splname))
                        return $"@[{splname}]";
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

        /// <summary>
        /// Resolves a jump/call target address to its label name, with the same ##
        /// WordImm marker used elsewhere — GOTO/GOSUB/CALL/ON-GOTO/ON-GOSUB targets
        /// are not guaranteed to always be WordRef-encoded, and silently assuming so
        /// would be the same unrecoverable ambiguity class fixed in v1.6.4/1.6.6 for
        /// plain operands, just undetected so far because no test data has
        /// exercised a WordImm-encoded jump target yet.
        /// </summary>
        private string ResolveLabel(Operand op)
        {
            string prefix = op.Kind == OperandKind.WordImm ? "##" : "";
            if (_labelNames.TryGetValue(op.Word, out string? name)) return prefix + name;
            return $"{prefix}loc_{op.Word:X4}";
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

                // Variable-arity extras: ON GOTO/GOSUB/HORIZONTAL MENU use op2 as count;
                // VERTICAL MENU uses op3 as count.
                int extraCount = 0;
                if (opcode is 0x25 or 0x26 or 0x2B)          // count in op2 (index 1)
                {
                    if (instr.Operands.Count < 2) return null;
                    extraCount = GetOperandValue(instr.Operands[1]);
                }
                else if (opcode == 0x15)                       // count in op3 (index 2)
                {
                    if (instr.Operands.Count < 3) return null;
                    extraCount = GetOperandValue(instr.Operands[2]);
                }

                for (int i = 0; i < extraCount; i++)
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
            // Read until we hit a code byte, the next table base, or end of file.
            // Cap at the next known data table address to prevent overlap when
            // multiple GETTABLE instructions reference overlapping slices of the
            // same underlying data array.
            ushort nextTableBase = _dataAddrs
                .Where(a => a > tableBase)
                .OrderBy(a => a)
                .Select(a => (ushort?)a)
                .FirstOrDefault() ?? ushort.MaxValue;

            var vals = new List<string>();
            ushort cur = tableBase;
            for (int i = 0; i < 256; i++)
            {
                if (_codeBytes.Contains(cur))  break;
                if (cur >= nextTableBase)       break;
                if (!IsInRange(cur))            break;
                vals.Add($"0x{_data[FileOffset(cur)]:X2}");
                cur++;
            }
            return string.Join(", ", vals);
        }
    }

    // ── Entry point helpers ───────────────────────────────────────────────────

    public static class EclhDecompilerProgram
    {
        /// <summary>
        /// Decompile a raw ECL block (including 2-byte DAX prefix) and return
        /// the ECLH source text.
        /// </summary>
        public static string Run(byte[] rawBytes, ushort baseAddress = 0x9900)
        {
            var d = new EclhDecompiler(rawBytes, baseAddress);
            return d.Decompile();
        }

        /// <summary>
        /// Dump a flat instruction listing to stdout for quick analysis.
        /// Runs all CFG and analysis passes but skips structured emission.
        /// </summary>
        public static void DumpRaw(byte[] rawBytes, ushort baseAddress = 0x9900)
        {
            var d = new EclhDecompiler(rawBytes, baseAddress);
            d.Decompile();

            Console.WriteLine($"Base: 0x{baseAddress:X4}   Instructions: {d.Instructions.Count}");
            Console.WriteLine(new string('-', 80));

            foreach (var instr in d.Instructions.Values)
                Console.WriteLine(instr);
        }
    }
}
