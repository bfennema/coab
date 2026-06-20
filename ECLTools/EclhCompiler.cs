using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

// ECLH Compiler — ECLH source to ECL bytecode
//
// Companion to EclhDecompiler.cs. Targets exactly the syntax that decompiler
// emits (see EclhDecompiler EmitSource/FormatInstruction/TryEmit* methods),
// not the full aspirational grammar in the language spec. Phase-1 goal:
// round-trip — decompile(x) |> compile == x, byte for byte.
//
// Changelog:
//   0.1.0 — Initial implementation. Lexer, recursive-descent parser, AST,
//            two-pass code generator (size pass + address-resolving emit pass).
//            Supports: var{}, data{}, entry points, if/else, while, do/while,
//            single-action if, flag-reuse if, all instruction forms emitted by
//            EclhDecompiler v1.6.0, ++x/x++/x--/x+=n/x-=n shorthand,
//            engine_func/hardware_reg/mem_region resolution via game profile.

namespace Eclh
{
    // ════════════════════════════════════════════════════════════════════════
    // Lexer
    // ════════════════════════════════════════════════════════════════════════

    public enum TokenKind
    {
        Identifier, Number, HexNumber, String, Comment,
        // Punctuation
        LParen, RParen, LBrace, RBrace, LBracket, RBracket,
        Comma, Colon, Semicolon, At, Hash, HashHash, AtBracket,
        Equals, PlusEquals, MinusEquals, PlusPlus, MinusMinus, PlusPlusPrefix, MinusMinusPrefix,
        Plus, Minus, Star, Slash, Amp, Pipe,
        EqEq, NotEq, Lt, Gt, LtEq, GtEq, AndAnd, OrOr, Bang, Question,
        DotDot, Arrow,
        // Keywords
        KwVar, KwData, KwByte, KwWord, KwIf, KwElse, KwWhile, KwDo,
        KwGoto, KwCall, KwReturn, KwExit, KwRandom, KwBase, KwGame,
        KwOnMove, KwOnSearch, KwOnPreCamp, KwOnCampInterrupted, KwOnEnter,
        EOF
    }

    public struct Token
    {
        public TokenKind Kind;
        public string Text;
        public long IntValue;
        public int Line;
        public int Col;

        public override string ToString() => $"{Kind}:'{Text}' @{Line}:{Col}";
    }

    public class LexError : Exception
    {
        public int Line, Col;
        public LexError(string msg, int line, int col) : base(msg) { Line = line; Col = col; }
    }

    public class Lexer
    {
        private readonly string _src;
        private int _pos;
        private int _line = 1;
        private int _col = 1;

        private static readonly Dictionary<string, TokenKind> Keywords = new()
        {
            ["var"] = TokenKind.KwVar,
            ["data"] = TokenKind.KwData,
            ["byte"] = TokenKind.KwByte,
            ["word"] = TokenKind.KwWord,
            ["if"] = TokenKind.KwIf,
            ["else"] = TokenKind.KwElse,
            ["while"] = TokenKind.KwWhile,
            ["do"] = TokenKind.KwDo,
            ["goto"] = TokenKind.KwGoto,
            ["call"] = TokenKind.KwCall,
            ["return"] = TokenKind.KwReturn,
            ["exit"] = TokenKind.KwExit,
            ["random"] = TokenKind.KwRandom,
            ["on_move"] = TokenKind.KwOnMove,
            ["on_search"] = TokenKind.KwOnSearch,
            ["on_pre_camp"] = TokenKind.KwOnPreCamp,
            ["on_camp_interrupted"] = TokenKind.KwOnCampInterrupted,
            ["on_enter"] = TokenKind.KwOnEnter,
        };

        public Lexer(string source) { _src = source; }

        private char Cur => _pos < _src.Length ? _src[_pos] : '\0';
        private char Peek(int o = 1) => _pos + o < _src.Length ? _src[_pos + o] : '\0';

        private void Advance()
        {
            if (Cur == '\n') { _line++; _col = 1; } else { _col++; }
            _pos++;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (true)
            {
                SkipWhitespaceAndComments(tokens);
                if (_pos >= _src.Length)
                {
                    tokens.Add(new Token { Kind = TokenKind.EOF, Line = _line, Col = _col });
                    break;
                }

                int startLine = _line, startCol = _col;
                char c = Cur;

                if (char.IsLetter(c) || c == '_')
                {
                    tokens.Add(ReadIdentifierOrKeyword(startLine, startCol));
                }
                else if (char.IsDigit(c))
                {
                    tokens.Add(ReadNumber(startLine, startCol));
                }
                else if (c == '$')
                {
                    tokens.Add(ReadDollarHex(startLine, startCol));
                }
                else if (c == '"')
                {
                    tokens.Add(ReadString(startLine, startCol));
                }
                else
                {
                    tokens.Add(ReadPunctuation(startLine, startCol));
                }
            }
            return tokens;
        }

        private void SkipWhitespaceAndComments(List<Token> tokens)
        {
            while (_pos < _src.Length)
            {
                if (char.IsWhiteSpace(Cur)) { Advance(); continue; }
                if (Cur == '/' && Peek() == '/')
                {
                    while (_pos < _src.Length && Cur != '\n') Advance();
                    continue;
                }
                break;
            }
        }

        private Token ReadIdentifierOrKeyword(int line, int col)
        {
            int start = _pos;
            while (_pos < _src.Length && (char.IsLetterOrDigit(Cur) || Cur == '_'))
                Advance();
            string text = _src[start.._pos];
            var kind = Keywords.TryGetValue(text, out var kw) ? kw : TokenKind.Identifier;
            return new Token { Kind = kind, Text = text, Line = line, Col = col };
        }

        private Token ReadNumber(int line, int col)
        {
            int start = _pos;
            if (Cur == '0' && (Peek() == 'x' || Peek() == 'X'))
            {
                Advance(); Advance();
                int hexStart = _pos;
                while (_pos < _src.Length && Uri.IsHexDigit(Cur)) Advance();
                string hexText = _src[hexStart.._pos];
                long val = Convert.ToInt64(hexText, 16);
                return new Token { Kind = TokenKind.HexNumber, Text = _src[start.._pos], IntValue = val, Line = line, Col = col };
            }
            while (_pos < _src.Length && char.IsDigit(Cur)) Advance();
            string text = _src[start.._pos];
            return new Token { Kind = TokenKind.Number, Text = text, IntValue = long.Parse(text), Line = line, Col = col };
        }

        /// <summary>Reads a $XXXX hex address literal, as emitted by the decompiler
        /// for unnamed/unclassified addresses inside [ ], ?[ ], and @[ ] forms.</summary>
        private Token ReadDollarHex(int line, int col)
        {
            int start = _pos;
            Advance(); // $
            int hexStart = _pos;
            while (_pos < _src.Length && Uri.IsHexDigit(Cur)) Advance();
            if (_pos == hexStart)
                throw new LexError("Expected hex digits after '$'", line, col);
            string hexText = _src[hexStart.._pos];
            long val = Convert.ToInt64(hexText, 16);
            return new Token { Kind = TokenKind.HexNumber, Text = _src[start.._pos], IntValue = val, Line = line, Col = col };
        }

        private Token ReadString(int line, int col)
        {
            Advance(); // opening quote
            var sb = new StringBuilder();
            while (_pos < _src.Length && Cur != '"')
            {
                if (Cur == '\\' && _pos + 1 < _src.Length)
                {
                    Advance();
                    char esc = Cur;
                    sb.Append(esc switch { 'n' => '\n', 't' => '\t', '"' => '"', '\\' => '\\', _ => esc });
                    Advance();
                }
                else
                {
                    sb.Append(Cur);
                    Advance();
                }
            }
            if (_pos >= _src.Length) throw new LexError("Unterminated string literal", line, col);
            Advance(); // closing quote
            return new Token { Kind = TokenKind.String, Text = sb.ToString(), Line = line, Col = col };
        }

        private Token ReadPunctuation(int line, int col)
        {
            char c = Cur;
            char n = Peek();

            Token Make(TokenKind k, int len, string text)
            {
                for (int i = 0; i < len; i++) Advance();
                return new Token { Kind = k, Text = text, Line = line, Col = col };
            }

            switch (c)
            {
                case '(': return Make(TokenKind.LParen, 1, "(");
                case ')': return Make(TokenKind.RParen, 1, ")");
                case '{': return Make(TokenKind.LBrace, 1, "{");
                case '}': return Make(TokenKind.RBrace, 1, "}");
                case '[': return Make(TokenKind.LBracket, 1, "[");
                case ']': return Make(TokenKind.RBracket, 1, "]");
                case ',': return Make(TokenKind.Comma, 1, ",");
                case ':': return Make(TokenKind.Colon, 1, ":");
                case ';': return Make(TokenKind.Semicolon, 1, ";");
                case '?': return Make(TokenKind.Question, 1, "?");
                case '@':
                    if (n == '[') return Make(TokenKind.AtBracket, 2, "@[");
                    return Make(TokenKind.At, 1, "@");
                case '#':
                    if (n == '#') return Make(TokenKind.HashHash, 2, "##");
                    return Make(TokenKind.Hash, 1, "#");
                case '+':
                    if (n == '+') return Make(TokenKind.PlusPlus, 2, "++");
                    if (n == '=') return Make(TokenKind.PlusEquals, 2, "+=");
                    return Make(TokenKind.Plus, 1, "+");
                case '-':
                    if (n == '-') return Make(TokenKind.MinusMinus, 2, "--");
                    if (n == '=') return Make(TokenKind.MinusEquals, 2, "-=");
                    return Make(TokenKind.Minus, 1, "-");
                case '*': return Make(TokenKind.Star, 1, "*");
                case '/': return Make(TokenKind.Slash, 1, "/");
                case '&':
                    if (n == '&') return Make(TokenKind.AndAnd, 2, "&&");
                    return Make(TokenKind.Amp, 1, "&");
                case '|':
                    if (n == '|') return Make(TokenKind.OrOr, 2, "||");
                    return Make(TokenKind.Pipe, 1, "|");
                case '=':
                    if (n == '=') return Make(TokenKind.EqEq, 2, "==");
                    return Make(TokenKind.Equals, 1, "=");
                case '!':
                    if (n == '=') return Make(TokenKind.NotEq, 2, "!=");
                    return Make(TokenKind.Bang, 1, "!");
                case '<':
                    if (n == '=') return Make(TokenKind.LtEq, 2, "<=");
                    return Make(TokenKind.Lt, 1, "<");
                case '>':
                    if (n == '=') return Make(TokenKind.GtEq, 2, ">=");
                    return Make(TokenKind.Gt, 1, ">");
                case '.':
                    if (n == '.') return Make(TokenKind.DotDot, 2, "..");
                    throw new LexError($"Unexpected character '.'", line, col);
                default:
                    throw new LexError($"Unexpected character '{c}'", line, col);
            }
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // AST
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Operand as written in source — mirrors the decompiler's Operand model
    /// exactly so the same value always round-trips to the same bytes.
    /// </summary>
    public class OperandExpr
    {
        public OperandKind Kind;
        public byte ByteVal;
        public ushort Word;            // address, or resolved-later symbol address
        public string? SymbolName;     // unresolved name (variable/label/engine func/hw reg) — resolved in pass 2
        public string? StringVal;      // for StringInline

        public static OperandExpr Imm8(byte v) => new() { Kind = OperandKind.ByteImm, ByteVal = v };
        public static OperandExpr Imm16(ushort v) => new() { Kind = OperandKind.WordImm, Word = v };
        public static OperandExpr Imm16Sym(string sym) => new() { Kind = OperandKind.WordImm, SymbolName = sym };
        public static OperandExpr Ref(string sym) => new() { Kind = OperandKind.WordRef, SymbolName = sym };
        public static OperandExpr Ref(ushort addr) => new() { Kind = OperandKind.WordRef, Word = addr };
        public static OperandExpr Code02(ushort addr) => new() { Kind = OperandKind.Code02, Word = addr };
        public static OperandExpr Str(string s) => new() { Kind = OperandKind.StringInline, StringVal = s };
        public static OperandExpr StrPtr(string sym) => new() { Kind = OperandKind.StringPtr, SymbolName = sym };
        public static OperandExpr StrPtr(ushort addr) => new() { Kind = OperandKind.StringPtr, Word = addr };
    }

    public abstract class StmtNode
    {
        public int Line;
    }

    /// <summary>A raw instruction: opcode-driven, used as fallback and as the
    /// final lowered form all higher-level constructs reduce into.</summary>
    public class RawInstrNode : StmtNode
    {
        public byte Opcode;
        public List<OperandExpr> Operands = new();
    }

    public class LabelDeclNode : StmtNode
    {
        public string Name = "";
        public ushort? PinnedAddress;   // from "name @ 0xADDR:" syntax
    }

    public class AssignNode : StmtNode
    {
        public OperandExpr Dest = null!;
        public RvalueNode Value = null!;
    }

    public abstract class RvalueNode { }

    public class OperandRvalue : RvalueNode { public OperandExpr Operand = null!; }

    public class BinaryRvalue : RvalueNode
    {
        public OperandExpr Lhs = null!;
        public string Op = "";   // "+","-","*","/","&","|"
        public OperandExpr Rhs = null!;
    }

    public class RandomRvalue : RvalueNode { public OperandExpr Max = null!; }

    public class TableIndexRvalue : RvalueNode
    {
        public string TableName = "";
        public OperandExpr Index = null!;
    }

    /// <summary>x++ / ++x / x-- (-- prefix not used by decompiler but accepted)</summary>
    public class IncDecNode : StmtNode
    {
        public OperandExpr Target = null!;
        public bool IsIncrement;   // true = ++, false = --
        public bool IsPrefix;     // true = ++x, false = x++
    }

    /// <summary>x += n / x -= n</summary>
    public class CompoundAssignNode : StmtNode
    {
        public OperandExpr Target = null!;
        public string Op = "";   // "+","-"
        public OperandExpr Amount = null!;
    }

    public class TableAssignNode : StmtNode
    {
        public string TableName = "";
        public OperandExpr Index = null!;
        public OperandExpr Value = null!;
    }

    public class GotoNode : StmtNode { public string Target = ""; }

    public class CallSubNode : StmtNode { public string Name = ""; }     // name()  -> GOSUB
    public class CallEngineNode : StmtNode { public string Name = ""; }  // name()  -> CALL (engine_func)

    public class ExitNode : StmtNode { }
    public class ReturnNode : StmtNode { }

    public class OnGotoNode : StmtNode
    {
        public OperandExpr Index = null!;
        public List<string> Targets = new();
        public bool IsGosub;   // true = ON GOSUB (call), false = ON GOTO (goto)
    }

    /// <summary>Condition as parsed: either a simple compare or a COMPARE AND form.</summary>
    public class ConditionExpr
    {
        public bool IsCompoundAnd;     // true = "a==b && c==d" / "a!=b || c!=d" (COMPARE AND form)
        public OperandExpr A = null!, B = null!;
        public string Op = "";          // for simple form: ==, !=, <, >, <=, >=
        public OperandExpr? C, D;       // for compound form, second pair
        public bool CompoundIsOr;       // true if written with || (both-differ test)
    }

    /// <summary>if (cond) action — single statement, no braces.</summary>
    public class SingleIfNode : StmtNode
    {
        public ConditionExpr Condition = null!;
        public StmtNode Action = null!;
    }

    /// <summary>if (op) action  — flag-reuse, bare operator, no comparands.</summary>
    public class FlagReuseIfNode : StmtNode
    {
        public string Op = "";
        public StmtNode Action = null!;
    }

    public class BlockIfNode : StmtNode
    {
        public ConditionExpr Condition = null!;
        public List<StmtNode> ThenBody = new();
        public List<StmtNode>? ElseBody;   // null = no else
    }

    public class WhileNode : StmtNode
    {
        public ConditionExpr Condition = null!;
        public List<StmtNode> Body = new();
    }

    public class DoWhileNode : StmtNode
    {
        public List<StmtNode> Body = new();
        public ConditionExpr Condition = null!;
    }

    /// <summary>Generic command statement: mnemonic + operand list, for instructions
    /// with no specialised AST node (PRINT, PICTURE, LOAD_MONSTER, etc).</summary>
    public class CmdNode : StmtNode
    {
        public string Mnemonic = "";   // lowercase, underscored: "load_character", "horizontal_menu" etc
        public List<OperandExpr> Args = new();
    }

    public class VarDecl { public string Name = ""; public ushort Address; public bool IsWord = true; }
    public class DataTableDecl { public string Name = ""; public ushort Address; public List<byte> Bytes = new(); public bool IsWord; }

    public class CompilationUnit
    {
        public ushort Base;
        public string GameProfile = "pool_of_radiance";
        public string OnMove = "", OnSearch = "", OnPreCamp = "", OnCampInterrupted = "", OnEnter = "";
        public List<VarDecl> Vars = new();
        public List<DataTableDecl> DataTables = new();
        public List<StmtNode> Statements = new();   // top-level statement/label stream
    }

    // ════════════════════════════════════════════════════════════════════════
    // Parser
    // ════════════════════════════════════════════════════════════════════════

    public class ParseError : Exception
    {
        public int Line, Col;
        public ParseError(string msg, int line, int col) : base(msg) { Line = line; Col = col; }
    }

    public class Parser
    {
        private readonly List<Token> _toks;
        private int _pos;

        // Known mnemonics that take a fixed operand count, for generic CmdNode parsing.
        // Maps lowercase-underscored mnemonic -> fixed operand count (matches
        // EclhDecompiler.Opcodes table, mnemonic.ToLower().Replace(' ','_')).
        private static readonly Dictionary<string, byte> CmdOpcodes = new()
        {
            ["load_character"] = 0x0A,
            ["load_monster"] = 0x0B,
            ["setup_monster"] = 0x0C,
            ["approach"] = 0x0D,
            ["picture"] = 0x0E,
            ["input_number"] = 0x0F,
            ["input_string"] = 0x10,
            ["print"] = 0x11,
            ["printclear"] = 0x12,
            ["clearmonsters"] = 0x1C,
            ["partystrength"] = 0x1D,
            ["checkparty"] = 0x1E,
            ["notsure_1f"] = 0x1F,
            ["newecl"] = 0x20,
            ["load_files"] = 0x21,
            ["party_surprise"] = 0x22,
            ["surprise"] = 0x23,
            ["combat"] = 0x24,
            ["treasure"] = 0x27,
            ["rob"] = 0x28,
            ["encounter_menu"] = 0x29,
            ["parlay"] = 0x2C,
            ["damage"] = 0x2E,
            ["sprite_off"] = 0x31,
            ["find_item"] = 0x32,
            ["print_return"] = 0x33,
            ["ecl_clock"] = 0x34,
            ["add_npc"] = 0x36,
            ["load_pieces"] = 0x37,
            ["program"] = 0x38,
            ["who"] = 0x39,
            ["delay"] = 0x3A,
            ["spell"] = 0x3B,
            ["protection"] = 0x3C,
            ["clear_box"] = 0x3D,
            ["dump"] = 0x3E,
            ["find_special"] = 0x3F,
            ["destroy_items"] = 0x40,
            ["vertical_menu"] = 0x15,
            ["horizontal_menu"] = 0x2B,
        };

        public Parser(List<Token> tokens) { _toks = tokens; }

        private Token Cur => _toks[_pos];
        private Token PeekAt(int o) => _toks[Math.Min(_pos + o, _toks.Count - 1)];
        private bool Is(TokenKind k) => Cur.Kind == k;
        private bool IsAt(int o, TokenKind k) => PeekAt(o).Kind == k;

        private Token Advance() { var t = Cur; if (_pos < _toks.Count - 1) _pos++; return t; }

        private Token Expect(TokenKind k, string what)
        {
            if (Cur.Kind != k)
                throw new ParseError($"Expected {what}, got {Cur.Kind} '{Cur.Text}'", Cur.Line, Cur.Col);
            return Advance();
        }

        private bool Match(TokenKind k) { if (Is(k)) { Advance(); return true; } return false; }

        // ── Top level ────────────────────────────────────────────────────────

        public CompilationUnit ParseUnit()
        {
            var unit = new CompilationUnit();

            while (!Is(TokenKind.EOF))
            {
                if (Is(TokenKind.At) && IsAt(1, TokenKind.Identifier) && PeekAt(1).Text == "base")
                {
                    Advance(); Advance();
                    unit.Base = (ushort)ExpectHexOrDec();
                }
                else if (Is(TokenKind.At) && IsAt(1, TokenKind.Identifier) && PeekAt(1).Text == "game")
                {
                    Advance(); Advance();
                    unit.GameProfile = Expect(TokenKind.Identifier, "game profile name").Text;
                }
                else if (TryParseEntryPoint(unit)) { /* consumed */ }
                else if (Is(TokenKind.KwVar)) { ParseVarBlock(unit); }
                else if (Is(TokenKind.KwData)) { ParseDataBlock(unit); }
                else
                {
                    var stmt = ParseStatementOrLabel();
                    if (stmt != null) unit.Statements.Add(stmt);
                }
            }

            return unit;
        }

        private long ExpectHexOrDec()
        {
            if (Is(TokenKind.HexNumber)) return Advance().IntValue;
            if (Is(TokenKind.Number)) return Advance().IntValue;
            throw new ParseError("Expected number", Cur.Line, Cur.Col);
        }

        private bool TryParseEntryPoint(CompilationUnit unit)
        {
            string? field = Cur.Kind switch
            {
                TokenKind.KwOnMove => "on_move",
                TokenKind.KwOnSearch => "on_search",
                TokenKind.KwOnPreCamp => "on_pre_camp",
                TokenKind.KwOnCampInterrupted => "on_camp_interrupted",
                TokenKind.KwOnEnter => "on_enter",
                _ => null
            };
            if (field == null) return false;
            // Only treat as entry-point decl if followed by '='  (not by ':' which would
            // be a label use, though on_* labels are never declared as labels themselves)
            if (!IsAt(1, TokenKind.Equals)) return false;

            Advance(); // consume keyword
            Advance(); // consume '='
            string target = Expect(TokenKind.Identifier, "label name").Text;

            switch (field)
            {
                case "on_move": unit.OnMove = target; break;
                case "on_search": unit.OnSearch = target; break;
                case "on_pre_camp": unit.OnPreCamp = target; break;
                case "on_camp_interrupted": unit.OnCampInterrupted = target; break;
                case "on_enter": unit.OnEnter = target; break;
            }
            return true;
        }

        private void ParseVarBlock(CompilationUnit unit)
        {
            Advance(); // var
            Expect(TokenKind.LBrace, "{");
            while (!Is(TokenKind.RBrace))
            {
                bool isWord = true;
                if (Is(TokenKind.KwWord)) { Advance(); isWord = true; }
                else if (Is(TokenKind.KwByte)) { Advance(); isWord = false; }

                string name = Expect(TokenKind.Identifier, "variable name").Text;
                Expect(TokenKind.At, "@");
                ushort addr = (ushort)ExpectHexOrDec();
                unit.Vars.Add(new VarDecl { Name = name, Address = addr, IsWord = isWord });
            }
            Expect(TokenKind.RBrace, "}");
        }

        private void ParseDataBlock(CompilationUnit unit)
        {
            Advance(); // data
            Expect(TokenKind.LBrace, "{");
            while (!Is(TokenKind.RBrace))
            {
                bool isWord = false;
                if (Is(TokenKind.KwWord)) { Advance(); isWord = true; }
                else if (Is(TokenKind.KwByte)) { Advance(); isWord = false; }

                string name = Expect(TokenKind.Identifier, "table name").Text;
                Expect(TokenKind.At, "@");
                ushort addr = (ushort)ExpectHexOrDec();
                Expect(TokenKind.Equals, "=");
                Expect(TokenKind.LBrace, "{");
                var bytes = new List<byte>();
                if (!Is(TokenKind.RBrace))
                {
                    bytes.Add((byte)ExpectHexOrDec());
                    while (Match(TokenKind.Comma))
                        bytes.Add((byte)ExpectHexOrDec());
                }
                Expect(TokenKind.RBrace, "}");
                unit.DataTables.Add(new DataTableDecl { Name = name, Address = addr, Bytes = bytes, IsWord = isWord });
            }
            Expect(TokenKind.RBrace, "}");
        }

        // ── Statements ───────────────────────────────────────────────────────

        /// <summary>Parse one statement, or a label declaration. Returns null only
        /// in error-recovery scenarios (not currently used).</summary>
        private StmtNode? ParseStatementOrLabel()
        {
            int line = Cur.Line;

            // Label: identifier [@ 0xADDR] ':'
            if (Is(TokenKind.Identifier) &&
                (IsAt(1, TokenKind.Colon) || (IsAt(1, TokenKind.At) )))
            {
                // Look ahead far enough to confirm this is a label, not an assignment
                // or table-index. Labels are "name [@ hex] :" with no other tokens.
                int save = _pos;
                string name = Advance().Text;
                ushort? pin = null;
                if (Match(TokenKind.At))
                    pin = (ushort)ExpectHexOrDec();
                if (Is(TokenKind.Colon))
                {
                    Advance();
                    return new LabelDeclNode { Name = name, PinnedAddress = pin, Line = line };
                }
                // Not actually a label — restore and fall through to statement parsing
                _pos = save;
            }

            return ParseStatement();
        }

        private List<StmtNode> ParseBlock()
        {
            Expect(TokenKind.LBrace, "{");
            var stmts = new List<StmtNode>();
            while (!Is(TokenKind.RBrace))
            {
                var s = ParseStatementOrLabel();
                if (s != null) stmts.Add(s);
            }
            Expect(TokenKind.RBrace, "}");
            return stmts;
        }

        private StmtNode ParseStatement()
        {
            int line = Cur.Line;

            if (Is(TokenKind.KwIf)) return ParseIf();
            if (Is(TokenKind.KwWhile)) return ParseWhile();
            if (Is(TokenKind.KwDo)) return ParseDoWhile();
            if (Is(TokenKind.KwGoto)) return ParseGoto();
            if (Is(TokenKind.KwExit)) { Advance(); return new ExitNode { Line = line }; }
            if (Is(TokenKind.KwReturn)) { Advance(); return new ReturnNode { Line = line }; }

            // goto(idx) { targets }  or  call(idx) { targets }
            if ((Is(TokenKind.KwGoto) || Is(TokenKind.KwCall)) && IsAt(1, TokenKind.LParen))
                return ParseOnGoto();

            // ++x
            if (Is(TokenKind.PlusPlus))
            {
                Advance();
                var target = ParseOperand();
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = true, Line = line };
            }

            // identifier-led statement: assignment, x++, x--, x+=n, table[idx]=v, call(), sub()
            if (Is(TokenKind.Identifier))
            {
                return ParseIdentifierLedStatement();
            }

            throw new ParseError($"Unexpected token {Cur.Kind} '{Cur.Text}' at start of statement", Cur.Line, Cur.Col);
        }

        private StmtNode ParseIdentifierLedStatement()
        {
            int line = Cur.Line;
            string name = Cur.Text;

            // name()  -> sub call or engine call (disambiguated in codegen via symbol table)
            if (IsAt(1, TokenKind.LParen) && IsAt(2, TokenKind.RParen))
            {
                Advance(); Advance(); Advance();
                return new CallSubNode { Name = name, Line = line };   // codegen resolves engine vs sub
            }

            // name++ / name--
            if (IsAt(1, TokenKind.PlusPlus))
            {
                Advance();
                var target = OperandExpr.Ref(name);
                Advance(); // ++
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = false, Line = line };
            }
            if (IsAt(1, TokenKind.MinusMinus))
            {
                Advance();
                var target = OperandExpr.Ref(name);
                Advance(); // --
                return new IncDecNode { Target = target, IsIncrement = false, IsPrefix = false, Line = line };
            }

            // name += n / name -= n
            if (IsAt(1, TokenKind.PlusEquals) || IsAt(1, TokenKind.MinusEquals))
            {
                var target = ParseOperand();
                string op = Is(TokenKind.PlusEquals) ? "+" : "-";
                Advance();
                var amt = ParseOperand();
                return new CompoundAssignNode { Target = target, Op = op, Amount = amt, Line = line };
            }

            // tablename[idx] = value
            if (IsAt(1, TokenKind.LBracket))
            {
                Advance(); // table name
                Advance(); // [
                var idx = ParseOperand();
                Expect(TokenKind.RBracket, "]");
                Expect(TokenKind.Equals, "=");
                var val = ParseOperand();
                return new TableAssignNode { TableName = name, Index = idx, Value = val, Line = line };
            }

            // mnemonic-style command: identifier followed by operand list (no '=')
            if (CmdOpcodes.ContainsKey(name) && !IsAt(1, TokenKind.Equals))
                return ParseCmdStatement();

            // Otherwise: assignment   dest = rvalue
            return ParseAssignment();
        }

        private StmtNode ParseAssignment()
        {
            int line = Cur.Line;
            var dest = ParseOperand();
            Expect(TokenKind.Equals, "=");

            // random(max)
            if (Is(TokenKind.KwRandom))
            {
                Advance();
                Expect(TokenKind.LParen, "(");
                var max = ParseOperand();
                Expect(TokenKind.RParen, ")");
                return new AssignNode { Dest = dest, Value = new RandomRvalue { Max = max }, Line = line };
            }

            // tablename[idx]
            if (Is(TokenKind.Identifier) && IsAt(1, TokenKind.LBracket))
            {
                string tname = Advance().Text;
                Advance(); // [
                var idx = ParseOperand();
                Expect(TokenKind.RBracket, "]");
                return new AssignNode { Dest = dest, Value = new TableIndexRvalue { TableName = tname, Index = idx }, Line = line };
            }

            // String literal (string pointer assignment)
            if (Is(TokenKind.String))
            {
                string s = Advance().Text;
                return new AssignNode { Dest = dest, Value = new OperandRvalue { Operand = OperandExpr.Str(s) }, Line = line };
            }

            var lhs = ParseOperand();

            // binary op?
            if (Is(TokenKind.Plus) || Is(TokenKind.Minus) || Is(TokenKind.Star) || Is(TokenKind.Slash)
                || Is(TokenKind.Amp) || Is(TokenKind.Pipe))
            {
                string op = Advance().Text;
                var rhs = ParseOperand();
                return new AssignNode { Dest = dest, Value = new BinaryRvalue { Lhs = lhs, Op = op, Rhs = rhs }, Line = line };
            }

            return new AssignNode { Dest = dest, Value = new OperandRvalue { Operand = lhs }, Line = line };
        }

        private StmtNode ParseCmdStatement()
        {
            int line = Cur.Line;
            string mnem = Advance().Text;
            var args = new List<OperandExpr>();
            if (!IsStatementTerminator())
            {
                args.Add(ParseOperand());
                while (Match(TokenKind.Comma))
                    args.Add(ParseOperand());
            }
            return new CmdNode { Mnemonic = mnem, Args = args, Line = line };
        }

        private bool IsStatementTerminator() =>
            Is(TokenKind.RBrace) || Is(TokenKind.EOF) ||
            (Is(TokenKind.Identifier) && IsAt(1, TokenKind.Colon));

        private StmtNode ParseGoto()
        {
            int line = Cur.Line;
            Advance(); // goto
            string target = Expect(TokenKind.Identifier, "label name").Text;
            return new GotoNode { Target = target, Line = line };
        }

        private StmtNode ParseOnGoto()
        {
            int line = Cur.Line;
            bool isGosub = Is(TokenKind.KwCall);
            Advance(); // goto/call
            Expect(TokenKind.LParen, "(");
            var idx = ParseOperand();
            Expect(TokenKind.RParen, ")");
            Expect(TokenKind.LBrace, "{");
            var targets = new List<string>();
            if (!Is(TokenKind.RBrace))
            {
                targets.Add(Expect(TokenKind.Identifier, "label").Text);
                while (Match(TokenKind.Comma))
                    targets.Add(Expect(TokenKind.Identifier, "label").Text);
            }
            Expect(TokenKind.RBrace, "}");
            return new OnGotoNode { Index = idx, Targets = targets, IsGosub = isGosub, Line = line };
        }

        private StmtNode ParseIf()
        {
            int line = Cur.Line;
            Advance(); // if
            Expect(TokenKind.LParen, "(");

            // Flag-reuse form: if (op)  — just an operator, no operands
            if (IsComparisonOpToken(Cur.Kind) && IsAt(1, TokenKind.RParen))
            {
                string op = TokenToOp(Advance().Kind);
                Expect(TokenKind.RParen, ")");
                var action = ParseActionOrBlock(out bool isBlock, out List<StmtNode>? thenBody, out List<StmtNode>? elseBody);
                if (isBlock)
                    throw new ParseError("Flag-reuse if cannot have a block body", Cur.Line, Cur.Col);
                return new FlagReuseIfNode { Op = op, Action = action!, Line = line };
            }

            var cond = ParseCondition();
            Expect(TokenKind.RParen, ")");

            var act = ParseActionOrBlock(out bool blockForm, out List<StmtNode>? thenB, out List<StmtNode>? elseB);
            if (blockForm)
            {
                return new BlockIfNode { Condition = cond, ThenBody = thenB!, ElseBody = elseB, Line = line };
            }
            return new SingleIfNode { Condition = cond, Action = act!, Line = line };
        }

        /// <summary>Parses either "{ block }" with optional "else { block }" / "else if",
        /// or a single statement (no else allowed).</summary>
        private StmtNode? ParseActionOrBlock(out bool isBlock, out List<StmtNode>? thenBody, out List<StmtNode>? elseBody)
        {
            if (Is(TokenKind.LBrace))
            {
                isBlock = true;
                thenBody = ParseBlock();
                elseBody = null;
                if (Match(TokenKind.KwElse))
                {
                    if (Is(TokenKind.KwIf))
                    {
                        // else-if chain: wrap as a single-statement else body containing the nested if
                        var nested = ParseIf();
                        elseBody = new List<StmtNode> { nested };
                    }
                    else
                    {
                        elseBody = ParseBlock();
                    }
                }
                return null;
            }
            isBlock = false;
            thenBody = null; elseBody = null;
            return ParseStatement();
        }

        private static bool IsComparisonOpToken(TokenKind k) =>
            k is TokenKind.EqEq or TokenKind.NotEq or TokenKind.Lt or TokenKind.Gt
              or TokenKind.LtEq or TokenKind.GtEq;

        private static string TokenToOp(TokenKind k) => k switch
        {
            TokenKind.EqEq => "==", TokenKind.NotEq => "!=",
            TokenKind.Lt => "<", TokenKind.Gt => ">",
            TokenKind.LtEq => "<=", TokenKind.GtEq => ">=",
            _ => throw new InvalidOperationException()
        };

        private ConditionExpr ParseCondition()
        {
            // Could be: A op B
            //        or A == B && C == D   (compound, both-equal test)
            //        or A != B || C != D   (compound, either-differ test)
            var a = ParseOperand();
            if (!IsComparisonOpToken(Cur.Kind))
                throw new ParseError($"Expected comparison operator, got '{Cur.Text}'", Cur.Line, Cur.Col);
            string op = TokenToOp(Advance().Kind);
            var b = ParseOperand();

            if (Is(TokenKind.AndAnd) || Is(TokenKind.OrOr))
            {
                bool isOr = Is(TokenKind.OrOr);
                Advance();
                var c = ParseOperand();
                if (!IsComparisonOpToken(Cur.Kind))
                    throw new ParseError($"Expected comparison operator, got '{Cur.Text}'", Cur.Line, Cur.Col);
                Advance(); // op2 (always matches op for compound form; not separately stored)
                var d = ParseOperand();
                return new ConditionExpr { IsCompoundAnd = true, A = a, B = b, C = c, D = d, CompoundIsOr = isOr, Op = op };
            }

            return new ConditionExpr { IsCompoundAnd = false, A = a, B = b, Op = op };
        }

        private StmtNode ParseWhile()
        {
            int line = Cur.Line;
            Advance(); // while
            Expect(TokenKind.LParen, "(");
            var cond = ParseCondition();
            Expect(TokenKind.RParen, ")");
            var body = ParseBlock();
            return new WhileNode { Condition = cond, Body = body, Line = line };
        }

        private StmtNode ParseDoWhile()
        {
            int line = Cur.Line;
            Advance(); // do
            var body = ParseBlock();
            Expect(TokenKind.KwWhile, "while");
            Expect(TokenKind.LParen, "(");
            var cond = ParseCondition();
            Expect(TokenKind.RParen, ")");
            return new DoWhileNode { Body = body, Condition = cond, Line = line };
        }

        // ── Operands ─────────────────────────────────────────────────────────

        private OperandExpr ParseOperand()
        {
            // #N  -> byte immediate
            if (Is(TokenKind.Hash))
            {
                Advance();
                long v = ExpectHexOrDec();
                return OperandExpr.Imm8((byte)v);
            }
            // ##N or ##symbol -> word immediate
            if (Is(TokenKind.HashHash))
            {
                Advance();
                if (Is(TokenKind.Identifier))
                    return OperandExpr.Imm16Sym(Advance().Text);
                long v = ExpectHexOrDec();
                return OperandExpr.Imm16((ushort)v);
            }
            // [addr] or [$XXXX] reference (decompiler emits raw hex when unnamed)
            if (Is(TokenKind.LBracket))
            {
                Advance();
                ushort addr = ParseBracketAddress();
                Expect(TokenKind.RBracket, "]");
                return OperandExpr.Ref(addr);
            }
            // ?[addr] -> code 0x02, preserved verbatim
            if (Is(TokenKind.Question))
            {
                Advance();
                Expect(TokenKind.LBracket, "[");
                ushort addr = ParseBracketAddress();
                Expect(TokenKind.RBracket, "]");
                return OperandExpr.Code02(addr);
            }
            // @[addr] or @[symbol] -> string pointer
            if (Is(TokenKind.AtBracket))
            {
                Advance();
                if (Is(TokenKind.Identifier))
                {
                    string sym = Advance().Text;
                    Expect(TokenKind.RBracket, "]");
                    return OperandExpr.StrPtr(sym);
                }
                ushort addr = ParseBracketAddress();
                Expect(TokenKind.RBracket, "]");
                return OperandExpr.StrPtr(addr);
            }
            // "string"
            if (Is(TokenKind.String))
            {
                return OperandExpr.Str(Advance().Text);
            }
            // bare hex/decimal literal (rare; treat as word immediate)
            if (Is(TokenKind.HexNumber) || Is(TokenKind.Number))
            {
                long v = Advance().IntValue;
                return OperandExpr.Imm16((ushort)v);
            }
            // identifier -> named reference (variable, label, engine func, hardware reg)
            if (Is(TokenKind.Identifier))
            {
                return OperandExpr.Ref(Advance().Text);
            }

            throw new ParseError($"Expected operand, got {Cur.Kind} '{Cur.Text}'", Cur.Line, Cur.Col);
        }

        /// <summary>Parses the inside of [ ... ] / ?[ ... ] / @[ ... ] when the address
        /// is unnamed. Accepts both $XXXX (decompiler output format) and 0xXXXX,
        /// both of which lex as HexNumber tokens.</summary>
        private ushort ParseBracketAddress()
        {
            if (Is(TokenKind.HexNumber) || Is(TokenKind.Number))
                return (ushort)Advance().IntValue;
            throw new ParseError($"Expected address inside brackets, got {Cur.Kind} '{Cur.Text}'", Cur.Line, Cur.Col);
        }
    }
}
