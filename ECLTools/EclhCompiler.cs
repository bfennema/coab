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
//   0.3.23 — load_monster/setup_monster now accept a two-argument form,
//            "setup_monster(#63, #2)", as shorthand for the common case
//            where the third argument repeats the first —
//            "setup_monster(#63, #2, #63)". ParseCmdStatement special-cases
//            these two mnemonics: reads arg1 and arg2 as usual, then either
//            reads an explicit arg3 (if a comma follows) or synthesizes one
//            as a copy of arg1 via the new CloneOperand helper. Companion to
//            EclhDecompiler v1.9.20, which omits arg3 from source exactly
//            when it repeats arg1 for that specific instance (arg3 isn't
//            always redundant, so this isn't a blanket per-opcode omission
//            the way horizontal_menu/vertical_menu's count is).
//   0.3.22 — Added x *= n compound-assignment support, now that a cross-game
//            survey confirmed MUL's real operand-order convention (see
//            EclhDecompiler v1.9.19's changelog): new StarEquals ('*=')
//            token in the lexer, wired into all four compound-assign parse
//            sites alongside +=/-=//=, and LayoutCompoundAssign now emits
//            MUL [x], n, [x] for it (x as op0, n as op1, x again as op2/
//            dest) — matching the confirmed-dominant destIsLhs pattern
//            (15/16 compound-assign-shaped MUL instances across both
//            games). This is the OPPOSITE operand order from +=/-=, which
//            put the amount first — MUL keeps the destination in its
//            natural first position instead.
//   0.3.21 — Added x /= n compound-assignment support: new SlashEquals ('/=')
//            token in the lexer, wired into all four compound-assign parse
//            sites (##name-led, @[name]-led, [$XXXX]-led, and the main
//            dotted-name identifier-led path), and LayoutCompoundAssign now
//            emits DIVIDE [x], n, [x] for it (x as op0/dividend, n as
//            op1/divisor, x again as op2/dest) — the only valid encoding,
//            since division isn't commutative and "x /= n" can only mean
//            x = x / n. *= is intentionally NOT added: multiplication
//            commutes, so unlike DIVIDE there are two different byte
//            encodings that produce the identical numeric result, and which
//            one the original ECL compiler actually used hasn't been
//            confirmed (the same class of ambiguity ADD's compound-assign
//            had before v1.6.8 pinned it down empirically). Companion to
//            EclhDecompiler v1.9.17, which emits this shorthand.
//   0.3.20 — horizontal_menu/vertical_menu's item-count operand is no longer
//            written in ECLH source — it's fully redundant with the number
//            of trailing item operands actually present (the decoder reads
//            exactly that many, so count == item count always holds by
//            construction). ParseCmdStatement now reads one fewer fixed
//            operand for these two mnemonics (CmdOpcodes' stored
//            FixedOperands still reflects the true binary count, including
//            the count operand — ParseCmdStatement subtracts 1 locally), and
//            LayoutCmd synthesizes the count as OperandExpr.Imm8 from
//            cmd.Args.Count at codegen time — the same pattern already used
//            for SwitchNode's table size. Companion to EclhDecompiler v1.9.16,
//            which stops emitting this operand.
//   0.3.19 — Added dotted variable name support (e.g. player.Int,
//            player.levels.MagicUser), companion to EclhDecompiler v1.9.13's
//            dotted KnownVariableOffsets names. Added ParseDottedName, which
//            consumes Identifier ('.' Identifier)* and is now used
//            everywhere a plain identifier name previously appeared:
//            ParseOperand's bare-identifier, ##symbol, and @[symbol] cases;
//            var {} block declaration names; and the tail of
//            ParseIdentifierLedStatement (assignment, ++/--/+=/-=). The
//            latter required restructuring: the old single-token lookahead
//            (IsAt(1, PlusPlus) etc.) assumed a name was exactly one token,
//            which breaks for a multi-segment name where the operator comes
//            after the WHOLE dotted chain, not immediately after the first
//            segment. The full name is now always consumed first via
//            ParseDottedName, and the operator/'=' is checked against
//            whatever token follows that — which also let the per-branch
//            "!CmdOpcodes.ContainsKey(name)" guards be dropped, since
//            CmdOpcodes mnemonics already return early via the function's
//            first check before the dotted-name path is ever reached. Table
//            names and command mnemonics are unaffected (never dotted, so
//            their existing single-token lookahead is unchanged).
//   0.3.18 — Added parsing for TypeName.Member enum-style command arguments
//            (e.g. Spell.Knock), companion to EclhDecompiler v1.9.11's
//            CommandArgEnums. Added the '.' token to the lexer (previously
//            only '..' was recognized; a lone '.' threw a LexError). Added a
//            new ParseOperand case: an identifier immediately followed by
//            '.' and another identifier is resolved via
//            EclhDecompiler.ArgEnumsByType — a shared reverse index (type
//            name -> member name -> byte value) derived from
//            CommandArgEnums — into a plain ByteImm operand, identical in
//            shape to writing "#N" directly. Only recognized when the
//            leading identifier actually names a registered enum type, so
//            it can never misfire on an ordinary identifier that happens to
//            be followed by punctuation elsewhere.
//   0.3.17 — Reverted v0.3.16: exit/return are bare keywords like goto, not
//            name(args) commands — both are zero-operand control-flow
//            terminals, not callable engine operations, so parenthesizing
//            them was the wrong fix for the underlying mismatch. The actual
//            problem (decompiler emitting a form the compiler couldn't
//            parse) is fixed on the decompiler side instead: EclhDecompiler
//            v1.9.9 gives EXIT/RETURN their own dedicated FormatInstruction
//            branch (bare "exit"/"return", matching goto) rather than
//            falling through to the generic name(args) fallback. exit;/
//            return; parsing here is back to its original bare form.
//   0.3.16 — CRITICAL FIX: exit()/return() now require parens, matching what
//            the decompiler actually emits. EXIT/RETURN have no dedicated
//            branch in FormatInstruction, so they've fallen through to the
//            generic name(args) fallback since v1.9.7 and been emitted as
//            "exit();"/"return();" ever since — but ExitNode/ReturnNode
//            parsing was never updated off its original bare "exit;"/
//            "return;" keyword handling, since it doesn't go through
//            ParseCmdStatement (exit/return are dedicated keyword tokens,
//            not CmdOpcodes entries). Any decompiled file containing exit()
//            or return() (i.e. every decompiled file since v1.9.7) failed to
//            recompile with "Expected ';', got LParen '('". Fixed by
//            requiring Expect(LParen)/Expect(RParen) in both branches.
//   0.3.15 — Removed legacy goto(idx){targets}/call(idx){targets} ON GOTO/
//            GOSUB dispatch parsing entirely: OnGotoNode, ParseOnGoto, its
//            dispatch check in ParseStatement, and its codegen case in
//            LayoutStatement are all gone. The decompiler stopped emitting
//            this form back in v1.8.7 (superseded by the switch statement),
//            and the compiler only needs to parse what the decompiler can
//            currently produce, so there was no remaining reason to keep it.
//            This also removes the "call" exception from v0.3.14's uniform
//            command-call syntax: "call(" is no longer ambiguous with
//            anything, so ParseCmdStatement now requires parens for "call"
//            too, same as every other command. Companion to
//            EclhDecompiler v1.9.8, which emits call(...) accordingly.
//   0.3.14 — Unified command call syntax: every command mnemonic now uses
//            name(args), including zero-arg commands ("combat()",
//            "clearmonsters()"), matching the name() convention GOSUB/known-
//            engine-function calls already used — previously commands used a
//            bare "mnemonic arg1, arg2" form with no parens, an inconsistency
//            with name()-style calls. ParseCmdStatement now wraps its operand
//            list in Expect(LParen)/Expect(RParen), which also closes another
//            sliver of the v0.3.12 silent-absorption risk: a stray token
//            before the closing ')' is now caught immediately, on top of the
//            trailing ';' every statement already requires. ParseIdentifierLedStatement
//            now checks CmdOpcodes BEFORE the generic "name()" sub/engine-call
//            check, since a zero-arg command like "combat()" has the exact
//            same token shape as a bare subroutine call and would otherwise
//            be misread as a GOSUB to a subroutine literally named "combat".
//            Exception: "call" (direct CALL, opcode 0x2D, to an address not
//            recognized as a named engine function) keeps its legacy
//            space-separated "call target" form with no parens — "call(" would
//            collide with the legacy call(idx){targets} ON GOSUB dispatch
//            syntax still accepted for older decompiled files. Companion to
//            EclhDecompiler v1.9.7, which now emits this syntax.
//   0.3.13 — Extended the mandatory-';' rule from v0.3.12 to declarations, not
//            just statements: @base, @game, entry-point declarations
//            (on_move = sub_name;), var {} entries, data {} entries, and
//            @dead blocks all now require a trailing ';'. Same rationale as
//            v0.3.12 — TryParseEntryPoint's Expect(Identifier) for an
//            entry-point target, and each var/data entry's fixed token
//            sequence, had the identical "no boundary" shape as command
//            operand lists: a missing/malformed value would silently
//            swallow the next line's leading token instead of failing at
//            the actual mistake. Companion to EclhDecompiler v1.9.6, which
//            now emits ';' on all of these declaration forms.
//   0.3.12 — CRITICAL FIX: trailing ';' is now REQUIRED after every simple
//            statement (assignment, command, goto, return, exit, table
//            assign, compare, inc/dec, compound-assign, sub/engine calls,
//            and single-action if actions), not just switch case actions.
//            Previously statement boundaries were inferred by lookahead
//            (IsStatementTerminator) or, for most statement kinds, not
//            marked at all — so a wrong FixedOperands arity for some opcode,
//            or any other parser bug reading too few/many operands, would
//            silently absorb the next statement's tokens as extra operands
//            instead of failing. A mandatory ';' turns that into an
//            immediate, precisely-located "Expected ';'" parse error at the
//            point of the mistake. ParseStatement is now a thin wrapper:
//            block-form constructs (if/while/do-while/switch/goto(idx){}/
//            call(idx){}) are dispatched first and manage their own
//            termination via braces (or, for ParseIf's bare-condition forms,
//            an explicit ';' check replacing the old IsStatementTerminator
//            lookahead); everything else routes through the new
//            ParseSimpleStatement and then requires an explicit ';'.
//            IsStatementTerminator and ParseActionOrBlock are removed
//            (superseded by ParseIf's direct Match(Semicolon) checks).
//            Switch case actions ("goto label;" / "label();") now REQUIRE
//            their ';' (previously optional via Match) for the same
//            consistency reason. Companion to EclhDecompiler v1.9.5, which
//            now emits a trailing ';' on every simple statement it produces.
//   0.3.11 — Companion to EclhDecompiler v1.9.0's multi-game GameProfile
//            support: EclhCompiler's constructor now auto-selects
//            EngineFunctions/HardwareRegisters from the parsed @game
//            directive via EclhDecompiler.GameProfiles, the same registry the
//            decompiler reads from — so a decompiled file's @game line just
//            works on recompile without the caller having to manually call
//            SetEngineFunctions/SetHardwareRegisters. Unknown/custom game
//            names are left with empty tables as before, and Set* can still
//            be called afterward to override.
//   0.3.10 — Companion fix to EclhDecompiler v1.8.10: ParseSwitch now accepts
//            an optional explicit table size, "switch (idx, N) { ... }", and
//            treats it as authoritative over the inferred (highest explicit
//            case + 1) count whenever present — fixing a table-size ambiguity
//            when a default group's own elided indices extend past the
//            highest explicit case (e.g. "case 0: ...; default: ...;" where
//            the real table has 2 entries but the highest explicit case index
//            is 0). Also relaxed: a switch containing only "default" is now
//            legal as long as an explicit count is given (previously an error
//            in all cases, since no case existed to imply a size).
//   0.3.9 — Added `default:` case support to switch statement parsing, companion
//            to EclhDecompiler v1.8.9's collapsing of fall-through-equivalent
//            case groups into `default:`. ParseSwitch now recognizes a bare
//            "default:" entry (at most one per switch) and, when flattening the
//            sparse case map into the ordered target list, fills any index in
//            [0, maxExplicitIndex] that has no explicit "case" with the default
//            target. This is purely a source-level convenience — it produces
//            the exact same flat target list, and therefore identical bytes, as
//            writing out every covered index explicitly. Table size is still
//            determined solely by explicit case indices; "default" cannot by
//            itself imply a table size (a switch with only "default" and no
//            case is a compile error, since the original count is unrecoverable
//            from source alone).
//   0.3.8 — Added C-style switch statement syntax for ON GOTO/ON GOSUB, replacing
//            goto(idx){targets}/call(idx){targets}. Supports fall-through case
//            groups ("case 0: case 1: goto loc_X;") for consecutive identical
//            targets, matching EclhDecompiler v1.8.7's switch emission. Added
//            SwitchNode AST node, KwSwitch token, ParseSwitch parser (validates
//            case indices are sequential starting at 0), and codegen identical
//            to OnGotoNode's ON GOTO/ON GOSUB lowering. goto(idx){}/call(idx){}
//            (OnGotoNode) is retained for parsing older decompiled files.
//   0.3.7 — Removed @tail support entirely: the decompiler no longer emits @tail
//            (superseded by @dead since v1.8.6), so the parser block, TailBytes
//            field, and legacy @tail→@dead conversion code are all removed.
//   0.3.6 — Unified @tail into @dead: fileSize is now extended to cover any
//            @dead block that ends past cursor (including trailing-padding blocks
//            formerly emitted as @tail). The withTail append logic is removed.
//            Legacy @tail directives in old ECLH files are converted to a @dead
//            block pinned at the post-layout cursor address for backwards compat.
//   0.3.5 — Fixed tablename[idx] rvalue false-positive in ParseAssignmentRvalue:
//            "name[" was matched even when the "[" was the start of a [$XXXX]
//            bracket-led statement on the NEXT line (e.g. "[$00FB] = area_49C3"
//            followed by "[$00FC] = area_49C4" — the "area_49C3[$00FC]" was
//            consumed as a table-index rvalue, leaving a stray "=" that caused
//            "Unexpected token Equals at start of statement"). Fixed by also
//            requiring !IsAt(2, HexNumber): genuine table indices are never raw
//            $XXXX addresses, only named variables or #N immediates.
//   0.3.4 — Fixed "Unexpected token Equals '=' at start of statement" for
//            "call [$C01B]" (direct CALL 0x2D with raw address operand). Added
//            "call" to CmdOpcodes (opcode 0x2D, 1 operand) and added a fallback
//            in ParseStatement: KwCall not followed by '(' routes to
//            ParseCmdStatement. This disambiguates "call [$XXXX]" (direct CALL)
//            from "call(idx){...}" (ON GOSUB, still handled first).
//   0.3.3 — Fixed incorrect GOTO target when a @dead block (unreachable assembler
//            padding, e.g. 0x00 after NEWECL) immediately follows a block-if body.
//            AdvancePastDataTables now also advances past @dead blocks, so
//            PinLabel and EmitPseudo correctly skip dead-padding bytes when
//            computing synthetic guard-label addresses. Previously the block-if
//            guard GOTO landed on the @dead block's address rather than the next
//            real instruction after it.
//   0.3.2 — Companion to EclhDecompiler v1.8.4's gating-pattern fix: added
//            IsStatementTerminator check for "compare"/"compare_and" identifiers,
//            so "if (A op B)" immediately before a compare statement is parsed as
//            SingleIfNode with null Action (COMPARE+IF only, no action). Made
//            SingleIfNode.Action nullable; LayoutSingleIf skips action layout and
//            clears _lastCompareOperands when Action is null.
//   0.3.1 — Fixed name++/name--/name+=/name-= firing for known command mnemonics
//            when the immediately following token on the next line happens to be
//            ++, --, +=, or -=. A 0-operand command like "combat" followed by a
//            separate "++area_4A10" statement was misparsed as "combat++"
//            (incrementing a variable named combat), desynchronising the parser
//            and producing "Expected =, got KwExit" several lines later. Same
//            class of bug as the tablename[idx] guard added in v0.2.2. Fixed by
//            adding !CmdOpcodes.ContainsKey(name) guards to all four checks.
//   0.3.0 — Fixed "Unexpected token LBracket '[' at start of statement":
//            [$XXXX] (a plain WordRef-kind destination, unnamed because the
//            address isn't classified as a variable, mem-region symbol, hardware
//            register, or label) can appear as a statement destination, e.g.
//            "[$C059] = area_4A16". Same pattern as ## (WordImm) and @[ (StringPtr)
//            led-statements fixed earlier. Added ParseBracketLedStatement and a
//            dispatch entry in ParseStatement for TokenKind.LBracket, handling
//            assignment, +=/-=, ++/--.
//   0.2.9 — Fixed a chain-clearing ordering bug in LayoutSingleIf: setting
//            _lastCompareOperands then immediately calling LayoutStatement(
//            sif.Action, ...) caused the recursive call's generic clearing guard
//            (GotoNode is not SingleIfNode/LabelDeclNode) to wipe the state
//            before LayoutSingleIf's own "is the action a GOTO" restoration logic
//            ever ran — so the reuse chain was cleared on every single-action-if,
//            even ones ending in GOTO, and a second chained if always re-emitted
//            its own COMPARE. This caused "layout already passed X — overlapping
//            instructions" pin errors whenever two consecutive single-action ifs
//            shared a COMPARE (the extra 6 bytes from the redundant COMPARE
//            pushed the cursor past the next pinned label). Fixed by capturing
//            _lastCompareOperands into a local BEFORE the recursive call, then
//            restoring it afterward if the action was a GOTO.
//   0.2.8 — Companion fix to EclhDecompiler v1.8.1's TryEmitChainedSingleIf:
//            LayoutSingleIf now detects when a single-action if's condition has
//            the same operands as the most recently emitted COMPARE and skips
//            re-emitting COMPARE, just emitting IF+action (reusing existing
//            flags). The reuse chain persists only through GOTO actions
//            (matching the decompiler's look-back rule, which only skips prior
//            IF/GOTO pairs) and is cleared by any other statement type. Added
//            _lastCompareOperands tracking field and OperandsEqual helper.
//   0.2.7 — Added @dead 0xADDR { bytes } parsing and compilation, companion to
//            EclhDecompiler v1.8.0's mid-file dead code detection. New
//            DeadBlockDecl class and CompilationUnit.DeadBlocks list. Dead blocks
//            are written verbatim at their pinned address during Compile(),
//            independent of the normal instruction layout/cursor mechanism —
//            they don't participate in control flow, so they're simply restored
//            to the exact address they originally occupied.
//   0.2.6 — Fixed fileSize double-counting data table bytes, producing an
//            oversized output array padded with trailing zeros past the real end
//            of file (e.g. 7630 bytes instead of 7473 for ECL3_0 — 157 extra
//            zero bytes). fileSize was computed as 2 + 20 + codeBytes + dataBytes,
//            but codeBytes (derived from cursor) already spans every data table's
//            byte extent since v0.2.5's AdvancePastDataTables/PinLabel changes —
//            adding dataBytes again double-counted them. Removed the dataBytes
//            term entirely. Also added a final AdvancePastDataTables call after
//            all top-level statements are laid out, to cover a trailing data
//            table with no instruction or label after it (EmitPseudo/PinLabel
//            only trigger the advance when something follows). Removed the
//            unused dataCursor variable.
//   0.2.5 — Fixed synthetic label addresses when data tables are interleaved with
//            code. The layout pass previously didn't account for data table byte
//            extents — cursor stopped at the table's start address instead of
//            advancing past it, causing synthetic block-if/while guard labels to
//            be pinned inside the table (e.g. goto 0x9AA6 instead of 0x9AAA when
//            tbl_9AA6 is 4 bytes). Fixed by: (1) calling AdvancePastDataTables
//            after every EmitPseudo so cursor skips any table that starts there;
//            (2) replacing all direct _labelAddrs[x]=cursor synthetic-label pins
//            with PinLabel(x, ref cursor) which also advances past tables first.
//   0.2.4 — Support for bare flag-reuse IF with no action: FlagReuseIfNode.Action
//            is now nullable; ParseIf emits a FlagReuseIfNode with null Action when
//            the flag-reuse if is followed by a statement terminator; LayoutFlagReuseIf
//            skips the action layout when Action is null, emitting just the IF opcode.
//            Companion to EclhDecompiler v1.7.7 which emits this form for labeled
//            flag-reuse IFs with no following action.
//   0.2.3 — Added RawCompareNode case to LayoutStatement. A bare compare/
//            compare_and statement (not lifted into an if block) was parsed
//            correctly but had no codegen path, causing "Unhandled statement
//            type RawCompareNode". Emits COMPARE (0x03) or COMPARE AND (0x14)
//            directly, setting flags for a subsequent flag-reuse IF or leaving
//            them set at a return instruction.
//   0.2.2 — Fixed tablename[idx] parse ambiguity: the check fired for any
//            identifier followed by '[', including command mnemonics like
//            "protection" whose operand starts with '[' (e.g. protection [$AAEE]).
//            Added guard: table-assignment path only taken when the identifier is
//            not a known command mnemonic or compare/compare_and.
//   0.2.1 — Re-added dead-code-gap zero-padding for label-pin mismatches caused
//            by unreachable bytes after genuinely terminal instructions (NEWECL,
//            and potentially others). A positive gap between natural layout cursor
//            and a pinned label address is filled with 0x00 bytes so subsequent
//            label addresses are correct. A negative gap (layout overshot the pin)
//            remains a hard error since that implies overlapping instructions.
//            This is distinct from the gap-silencing reverted in v0.1.4 — that
//            was masking real CFG bugs; this is handling confirmed-unreachable
//            assembler padding after instructions that truly have no fall-through.
//   0.2.0 — Added @tail { 0xXX, ... } directive support: CompilationUnit.TailBytes
//            stores bytes verbatim appended after all code and data tables. Parsed
//            in ParseUnit (@tail { hex, ... } block). Emitted by Compile() by
//            extending outBytes with the tail after all other content. Handles the
//            0xF7 alignment-padding byte at the end of ECL6_1 and any similar
//            trailing bytes in other ECL files.
//   0.1.9 — Fixed LayoutCompoundAssign: "x += n" now compiles to ADD n, [x], [x]
//            (amount as op0, variable as op1 and dest), matching the corrected
//            decompiler convention in v1.6.8. Previously compiled to the never-
//            occurring ADD [x], n, [x] form.
//   0.1.8 — Fixed "Unexpected token AtBracket '@[' at start of statement":
//            @[name] (StringPtr-kind) can appear as a statement destination
//            just as ##name (WordImm-kind) can, e.g. "@[shared_9890] = 'MAD MAN'"
//            produced by EclhDecompiler v1.6.6's @[name] marker fix. Added
//            ParseAtBracketLedStatement, structurally identical to
//            ParseHashHashLedStatement, handling assignment, +=/-=, ++/--.
//   0.1.7 — Companion fix to EclhDecompiler v1.6.7's ResolveLabel marker: GOTO,
//            GOSUB/CALL (name()), and ON GOTO/ON GOSUB target lists can now carry
//            a ## marker for WordImm-encoded targets, mirroring plain operands.
//            GotoNode, CallSubNode, CallEngineNode gained an IsWordImm flag;
//            OnGotoNode.Targets changed from List<string> to
//            List<(string Name, bool IsWordImm)>. Added ParseLabelRef (shared by
//            ParseGoto and ParseOnGoto) and ##name() call syntax in
//            ParseHashHashLedStatement. Added LabelOperand codegen helper used by
//            all three lowering sites. Synthetic compiler-generated labels
//            (if/else/while/do-while guards) are unaffected — they're always
//            WordRef, never sourced from parsed ## syntax.
//   0.1.6 — Fixed "Unexpected token HashHash '##' at start of statement": ##name
//            can be a statement's destination (e.g. "##player_6BC3 = #1" or
//            "##player_6BC3--") whenever the decompiler encoded that operand as
//            WordImm rather than WordRef (see EclhDecompiler v1.6.4's ## marker
//            fix). ParseStatement only ever checked for a leading Identifier
//            token, never HashHash. Added ParseHashHashLedStatement, handling the
//            subset of statement forms that make sense for a ##-prefixed
//            destination (assignment, +=, -=, ++, --); table indexing, bare
//            calls, and mnemonic commands remain identifier-only since they're
//            never written with a ## destination in valid ECLH. Refactored
//            ParseAssignment's rvalue parsing into a shared ParseAssignmentRvalue
//            helper used by both the identifier-led and ##-led paths.
//   0.1.5 — Fixed string-literal assignment (dest = "text") encoding the
//            destination as WordRef (0x01) instead of StringPtr (0x81). The
//            decompiler always classifies the destination of a SAVE whose source
//            is an inline string as StringPtr, even though both operand kinds
//            reference the same address — only the type-code byte differs.
//            ParseAssignment parses any plain identifier destination as WordRef
//            by default (correct for ordinary variable assignment), so the
//            codegen now explicitly converts it via ToStringPtr() when the
//            source is a string literal.
//   0.1.4 — Reverted the dead-code-gap padding added in response to a label
//            pin mismatch; the real cause was a decompiler CFG bug (COMBAT
//            incorrectly marked terminal, fixed in EclhDecompiler v1.6.2) that
//            silently omitted reachable instructions, not genuine dead code.
//            Pinned-label mismatches are a strict compile error again — silently
//            padding gaps would have masked this and future decompiler CFG bugs
//            instead of surfacing them.
//   0.1.3 — Replaced lookahead-based command argument parsing (ParseCmdStatement
//            guessed where a command's argument list ended via IsStatementTerminator,
//            which didn't recognize keyword-led statements like "goto") with exact
//            operand counts from a shared, authoritative CmdOpcodes table — the same
//            FixedOperands values as EclhDecompiler's Opcodes table. This fixes
//            "Expected operand, got KwGoto" for zero/short-arg commands (dump, delay,
//            combat, etc.) immediately followed by a goto/call/if with no separator.
//            Unified the previously-duplicated CmdOpcodes (Parser) and
//            CmdMnemonicToOpcode (EclhCompiler) tables into one shared source of truth.
//   0.1.2 — Fixed ParseStatement dispatch order: goto(idx){targets} and
//            call(idx){targets} (ON GOTO/ON GOSUB dispatch) were being routed to
//            the plain "goto label" parser first, since both start with the same
//            keyword token, causing "Expected label name, got LParen" on any
//            dispatch-table statement.
//   0.1.1 — Removed on_move/on_search/on_pre_camp/on_camp_interrupted/on_enter as
//            reserved keywords. They are now plain identifiers, disambiguated from
//            entry-point declarations purely by shape (IDENT '=' IDENT at top level).
//            This fixes a parse failure: the decompiler also emits these names as
//            ordinary label declarations (e.g. "on_move @ 0x9914:") wherever the
//            entry-point address is also a GOTO/GOSUB target, which previously
//            could not be parsed as a label since "on_move" was a keyword token.
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
        Equals, PlusEquals, MinusEquals, SlashEquals, StarEquals, PlusPlus, MinusMinus, PlusPlusPrefix, MinusMinusPrefix,
        Plus, Minus, Star, Slash, Amp, Pipe,
        EqEq, NotEq, Lt, Gt, LtEq, GtEq, AndAnd, OrOr, Bang, Question,
        DotDot, Dot, Arrow,
        // Keywords
        KwVar, KwData, KwByte, KwWord, KwIf, KwElse, KwWhile, KwDo,
        KwGoto, KwCall, KwReturn, KwExit, KwRandom, KwBase, KwGame, KwSwitch,
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
            ["switch"] = TokenKind.KwSwitch,
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
                case '*':
                    if (n == '=') return Make(TokenKind.StarEquals, 2, "*=");
                    return Make(TokenKind.Star, 1, "*");
                case '/':
                    if (n == '=') return Make(TokenKind.SlashEquals, 2, "/=");
                    return Make(TokenKind.Slash, 1, "/");
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
                    return Make(TokenKind.Dot, 1, ".");
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

    public class GotoNode : StmtNode { public string Target = ""; public bool IsWordImm; }
    public class CallSubNode : StmtNode { public string Name = ""; public bool IsWordImm; }     // name()  -> GOSUB
    public class CallEngineNode : StmtNode { public string Name = ""; public bool IsWordImm; }  // name()  -> CALL (engine_func)

    public class ExitNode : StmtNode { }

    /// <summary>A bare compare/compare_and statement not lifted into an if block.</summary>
    public class RawCompareNode : StmtNode
    {
        public OperandExpr A = null!, B = null!;
        public bool IsAnd;          // true = compare_and (0x14), false = compare (0x03)
        public OperandExpr? C, D;   // for compare_and
    }

    public class ReturnNode : StmtNode { }

    /// <summary>C-style switch dispatching to goto/gosub targets by index.
    /// Compiles to ON GOTO (0x25) or ON GOSUB (0x26). Each case entry maps a
    /// 0-based index to a target; consecutive identical targets produce
    /// fall-through case groups in the source but a single target in the flat list.</summary>
    public class SwitchNode : StmtNode
    {
        public OperandExpr Index = null!;
        /// <summary>Flat target list in case order (0, 1, 2, ...).
        /// Multiple consecutive identical entries represent fall-through cases.</summary>
        public List<(string Name, bool IsWordImm)> Targets = new();
        public bool IsGosub;   // false = goto targets, true = call (gosub) targets
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
        public StmtNode? Action;   // null = bare if(cond) with no action — emits COMPARE+IF only
    }

    /// <summary>if (op) action  — flag-reuse, bare operator, no comparands.</summary>
    public class FlagReuseIfNode : StmtNode
    {
        public string Op = "";
        public StmtNode? Action;   // null = bare IF with no action (just emits the IF opcode)
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

    /// <summary>A mid-file unreachable byte range, pinned to a specific address —
    /// emitted by the decompiler's @dead directive for dead code that lies between
    /// two regions of reachable content (not at end-of-file, which uses @tail).</summary>
    public class DeadBlockDecl { public ushort Address; public List<byte> Bytes = new(); }

    public class CompilationUnit
    {
        public ushort Base;
        public string GameProfile = "pool_of_radiance";
        public string OnMove = "", OnSearch = "", OnPreCamp = "", OnCampInterrupted = "", OnEnter = "";
        public List<VarDecl> Vars = new();
        public List<DataTableDecl> DataTables = new();
        public List<StmtNode> Statements = new();   // top-level statement/label stream
        public List<DeadBlockDecl> DeadBlocks = new(); // verbatim mid-file and trailing unreachable byte ranges
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
        /// <summary>Mnemonic -> (opcode, fixed operand count). Counts are authoritative,
        /// copied from EclhDecompiler's Opcodes table, so ParseCmdStatement reads
        /// exactly the right number of operands instead of guessing via lookahead.
        /// horizontal_menu/vertical_menu are handled specially in ParseCmdStatement/
        /// LayoutCmd: their FixedOperands value here includes the item-count operand
        /// (matching the true binary layout), but that operand is omitted from ECLH
        /// source entirely and synthesized at compile time from the actual number of
        /// trailing items, since it's fully redundant with that count.</summary>
        internal static readonly Dictionary<string, (byte Opcode, int FixedOperands)> CmdOpcodes = new()
        {
            ["load_character"] = (0x0A, 1),
            ["load_monster"] = (0x0B, 3),
            ["setup_monster"] = (0x0C, 3),
            ["approach"] = (0x0D, 0),
            ["picture"] = (0x0E, 1),
            ["input_number"] = (0x0F, 2),
            ["input_string"] = (0x10, 2),
            ["print"] = (0x11, 1),
            ["printclear"] = (0x12, 1),
            ["clearmonsters"] = (0x1C, 0),
            ["partystrength"] = (0x1D, 1),
            ["checkparty"] = (0x1E, 6),
            ["notsure_1f"] = (0x1F, 2),
            ["newecl"] = (0x20, 1),
            ["load_files"] = (0x21, 3),
            ["party_surprise"] = (0x22, 2),
            ["surprise"] = (0x23, 4),
            ["combat"] = (0x24, 0),
            ["treasure"] = (0x27, 8),
            ["rob"] = (0x28, 3),
            ["encounter_menu"] = (0x29, 14),
            ["parlay"] = (0x2C, 6),
            ["damage"] = (0x2E, 5),
            ["sprite_off"] = (0x31, 0),
            ["find_item"] = (0x32, 1),
            ["print_return"] = (0x33, 0),
            ["ecl_clock"] = (0x34, 1),
            ["add_npc"] = (0x36, 2),
            ["load_pieces"] = (0x37, 3),
            ["program"] = (0x38, 1),
            ["who"] = (0x39, 1),
            ["delay"] = (0x3A, 0),
            ["spell"] = (0x3B, 3),
            ["protection"] = (0x3C, 1),
            // Direct CALL instruction (0x2D) with 1 address operand — distinct from
            // name() GOSUB (0x02), which pushes a return address. Occurs when the
            // callee is a raw address not in the named engine-function table.
            ["call"] = (0x2D, 1),
            ["clear_box"] = (0x3D, 0),
            ["dump"] = (0x3E, 0),
            ["find_special"] = (0x3F, 1),
            ["destroy_items"] = (0x40, 1),
            // Variable-arity: FixedOperands here is the TRUE binary fixed-operand
            // count (2 for horizontal_menu, 3 for vertical_menu), including the
            // item-count operand — ParseCmdStatement subtracts 1 to get how many
            // operands ECLH source actually specifies (the count itself is omitted
            // from source and synthesized at compile time; see ParseCmdStatement
            // and LayoutCmd).
            ["vertical_menu"] = (0x15, 3),
            ["horizontal_menu"] = (0x2B, 2),
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
                    Expect(TokenKind.Semicolon, "';'");
                }
                else if (Is(TokenKind.At) && IsAt(1, TokenKind.Identifier) && PeekAt(1).Text == "game")
                {
                    Advance(); Advance();
                    unit.GameProfile = Expect(TokenKind.Identifier, "game profile name").Text;
                    Expect(TokenKind.Semicolon, "';'");
                }
                else if (Is(TokenKind.At) && IsAt(1, TokenKind.Identifier) && PeekAt(1).Text == "dead")
                {
                    Advance(); Advance(); // @ dead
                    ushort addr = (ushort)ExpectHexOrDec();
                    Expect(TokenKind.LBrace, "{");
                    var bytes = new List<byte>();
                    while (!Is(TokenKind.RBrace) && !Is(TokenKind.EOF))
                    {
                        bytes.Add((byte)ExpectHexOrDec());
                        Match(TokenKind.Comma);
                    }
                    Expect(TokenKind.RBrace, "}");
                    Expect(TokenKind.Semicolon, "';'");
                    unit.DeadBlocks.Add(new DeadBlockDecl { Address = addr, Bytes = bytes });
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

        private static readonly HashSet<string> EntryPointNames = new()
        {
            "on_move", "on_search", "on_pre_camp", "on_camp_interrupted", "on_enter"
        };

        private bool TryParseEntryPoint(CompilationUnit unit)
        {
            // on_move/on_search/etc are not keywords — they're ordinary identifiers
            // that also happen to be valid label names (the decompiler names the
            // entry-point address itself "on_move" etc, so the same text can appear
            // as a label declaration "on_move @ 0x9914:" elsewhere in the file).
            // Disambiguate purely on shape: IDENT '=' IDENT, where IDENT is one of
            // the five reserved entry-point names and NOT followed by ':' (label)
            // or '@' (pinned label) at the second position.
            if (!Is(TokenKind.Identifier) || !EntryPointNames.Contains(Cur.Text))
                return false;
            if (!IsAt(1, TokenKind.Equals))
                return false;

            string field = Advance().Text; // consume "on_move" etc
            Advance(); // consume '='
            string target = Expect(TokenKind.Identifier, "label name").Text;
            Expect(TokenKind.Semicolon, "';'");

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

                string name = ParseDottedName();
                Expect(TokenKind.At, "@");
                ushort addr = (ushort)ExpectHexOrDec();
                Expect(TokenKind.Semicolon, "';'");
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
                Expect(TokenKind.Semicolon, "';'");
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
            // Block-form / self-terminating constructs manage their own
            // termination (braces, or in ParseIf's case an explicit ';'
            // check for the bare-condition form) and must NOT go through the
            // trailing-';' requirement below — a "}" or ParseIf's own ';'
            // handling already unambiguously ends these.
            if (Is(TokenKind.KwIf)) return ParseIf();
            if (Is(TokenKind.KwWhile)) return ParseWhile();
            if (Is(TokenKind.KwDo)) return ParseDoWhile();
            if (Is(TokenKind.KwSwitch)) return ParseSwitch();

            // Every other statement form is "simple" — a single instruction
            // that must end with an explicit ';'. This is required (not
            // optional/inferred via lookahead) so that any arity mismatch in
            // a command's operand list — e.g. a wrong FixedOperands count for
            // some opcode — produces an immediate, precisely-located parse
            // error ("Expected ';', got ...") instead of silently absorbing
            // the next statement's tokens as extra operands and producing a
            // corrupted AST that compiles to the wrong bytes without warning.
            var stmt = ParseSimpleStatement();
            Expect(TokenKind.Semicolon, "';'");
            return stmt;
        }

        private StmtNode ParseSimpleStatement()
        {
            int line = Cur.Line;

            if (Is(TokenKind.KwGoto)) return ParseGoto();

            // call [$XXXX] or call name — direct CALL instruction (opcode 0x2D)
            // with a single address operand. "call" is in CmdOpcodes so route
            // through ParseCmdStatement which handles the operand generically.
            if (Is(TokenKind.KwCall))
                return ParseCmdStatement();

            // exit/return — bare keywords like goto, not name(args) commands.
            // Both are zero-operand control-flow terminals, not callable
            // engine operations, so they're excluded from the uniform
            // command-call convention and parsed the same way goto is.
            if (Is(TokenKind.KwExit)) { Advance(); return new ExitNode { Line = line }; }
            if (Is(TokenKind.KwReturn)) { Advance(); return new ReturnNode { Line = line }; }

            // ++x
            if (Is(TokenKind.PlusPlus))
            {
                Advance();
                var target = ParseOperand();
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = true, Line = line };
            }

            // @[name]-led statement: a StringPtr-kind destination (variable the
            // decompiler encoded as StringPtr rather than WordRef — see
            // EclhDecompiler FormatOp's @[name] marker, fixed in v1.6.6).
            // Only assignment and increment/decrement forms make sense here.
            if (Is(TokenKind.AtBracket))
            {
                return ParseAtBracketLedStatement();
            }

            // [$XXXX]-led statement: an unnamed WordRef destination — the
            // decompiler falls back to this raw bracket form when an address is
            // not classified as a named variable, mem-region symbol, hardware
            // register, or label (see EclhDecompiler FormatOp's unclassified-
            // address fallback). Only assignment and increment/decrement forms
            // make sense here, mirroring the ##/@[ led-statement handlers.
            if (Is(TokenKind.LBracket))
            {
                return ParseBracketLedStatement();
            }

            // identifier-led statement: assignment, x++, x--, x+=n, table[idx]=v, call(), sub()
            if (Is(TokenKind.Identifier))
            {
                return ParseIdentifierLedStatement();
            }

            // ##name-led statement: a WordImm-kind destination (named mem-region
            // variable that the decompiler encoded as WordImm rather than WordRef —
            // see EclhDecompiler FormatOp's ## marker). Assignment forms and
            // ##name() calls (also WordImm-encoded GOTO/GOSUB/CALL targets) are
            // handled; table indexing and mnemonic commands are syntactically tied
            // to plain identifiers and never appear after ##.
            if (Is(TokenKind.HashHash))
            {
                return ParseHashHashLedStatement();
            }

            throw new ParseError($"Unexpected token {Cur.Kind} '{Cur.Text}' at start of statement", Cur.Line, Cur.Col);
        }

        private StmtNode ParseHashHashLedStatement()
        {
            int line = Cur.Line;

            // ##name()  -> sub/engine call with a WordImm-encoded target
            if (IsAt(1, TokenKind.Identifier) && IsAt(2, TokenKind.LParen) && IsAt(3, TokenKind.RParen))
            {
                Advance(); // ##
                string callName = Advance().Text;
                Advance(); Advance(); // ( )
                return new CallSubNode { Name = callName, IsWordImm = true, Line = line };
            }

            var target = ParseOperand();   // consumes "##name" -> WordImm-kind operand

            // ##name++ / ##name--
            if (Is(TokenKind.PlusPlus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = false, Line = line };
            }
            if (Is(TokenKind.MinusMinus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = false, IsPrefix = false, Line = line };
            }

            // ##name += n / ##name -= n / ##name /= n / ##name *= n
            if (Is(TokenKind.PlusEquals) || Is(TokenKind.MinusEquals) || Is(TokenKind.SlashEquals) || Is(TokenKind.StarEquals))
            {
                string op = Is(TokenKind.PlusEquals) ? "+" : Is(TokenKind.MinusEquals) ? "-" : Is(TokenKind.SlashEquals) ? "/" : "*";
                Advance();
                var amt = ParseOperand();
                return new CompoundAssignNode { Target = target, Op = op, Amount = amt, Line = line };
            }

            // ##name = rvalue
            Expect(TokenKind.Equals, "=");
            return ParseAssignmentRvalue(target, line);
        }

        private StmtNode ParseAtBracketLedStatement()
        {
            int line = Cur.Line;
            var target = ParseOperand();   // consumes "@[name]" -> StringPtr-kind operand

            // @[name]++ / @[name]--
            if (Is(TokenKind.PlusPlus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = false, Line = line };
            }
            if (Is(TokenKind.MinusMinus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = false, IsPrefix = false, Line = line };
            }

            // @[name] += n / @[name] -= n / @[name] /= n / @[name] *= n
            if (Is(TokenKind.PlusEquals) || Is(TokenKind.MinusEquals) || Is(TokenKind.SlashEquals) || Is(TokenKind.StarEquals))
            {
                string op = Is(TokenKind.PlusEquals) ? "+" : Is(TokenKind.MinusEquals) ? "-" : Is(TokenKind.SlashEquals) ? "/" : "*";
                Advance();
                var amt = ParseOperand();
                return new CompoundAssignNode { Target = target, Op = op, Amount = amt, Line = line };
            }

            // @[name] = rvalue
            Expect(TokenKind.Equals, "=");
            return ParseAssignmentRvalue(target, line);
        }

        private StmtNode ParseBracketLedStatement()
        {
            int line = Cur.Line;
            var target = ParseOperand();   // consumes "[$XXXX]" -> WordRef-kind operand (unnamed address)

            // [$XXXX]++ / [$XXXX]--
            if (Is(TokenKind.PlusPlus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = false, Line = line };
            }
            if (Is(TokenKind.MinusMinus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = false, IsPrefix = false, Line = line };
            }

            // [$XXXX] += n / [$XXXX] -= n / [$XXXX] /= n / [$XXXX] *= n
            if (Is(TokenKind.PlusEquals) || Is(TokenKind.MinusEquals) || Is(TokenKind.SlashEquals) || Is(TokenKind.StarEquals))
            {
                string op = Is(TokenKind.PlusEquals) ? "+" : Is(TokenKind.MinusEquals) ? "-" : Is(TokenKind.SlashEquals) ? "/" : "*";
                Advance();
                var amt = ParseOperand();
                return new CompoundAssignNode { Target = target, Op = op, Amount = amt, Line = line };
            }

            // [$XXXX] = rvalue
            Expect(TokenKind.Equals, "=");
            return ParseAssignmentRvalue(target, line);
        }

        private StmtNode ParseIdentifierLedStatement()
        {
            int line = Cur.Line;
            string name = Cur.Text;

            // Known command mnemonics always route to ParseCmdStatement using
            // uniform name(args) syntax — including zero-arg commands like
            // "combat()"/"clearmonsters()". This MUST be checked before the
            // generic "name()" sub/engine-call check just below: a zero-arg
            // command written as "combat()" has exactly the same shape
            // (identifier, '(', ')') as a bare subroutine call, and without
            // this ordering it would be misread as a GOSUB/CALL to a
            // subroutine named "combat" instead of the combat command.
            if (CmdOpcodes.ContainsKey(name) && !IsAt(1, TokenKind.Equals))
                return ParseCmdStatement();

            // name()  -> sub call or engine call (disambiguated in codegen via symbol table)
            if (IsAt(1, TokenKind.LParen) && IsAt(2, TokenKind.RParen))
            {
                Advance(); Advance(); Advance();
                return new CallSubNode { Name = name, Line = line };   // codegen resolves engine vs sub
            }

            // tablename[idx] = value — only when name is not a known command mnemonic.
            // Commands like "protection [$AAEE]" have an operand that starts with [
            // and would otherwise be misread as a table assignment. Table names are
            // never dotted (always tbl_XXXX, or a plain variable used as a computed
            // base — see FormatTableRef), so a single-token lookahead is enough.
            if (IsAt(1, TokenKind.LBracket) && !CmdOpcodes.ContainsKey(name)
                && name != "compare" && name != "compare_and")
            {
                Advance(); // table name
                Advance(); // [
                var idx = ParseOperand();
                Expect(TokenKind.RBracket, "]");
                Expect(TokenKind.Equals, "=");
                var val = ParseOperand();
                return new TableAssignNode { TableName = name, Index = idx, Value = val, Line = line };
            }

            // compare and compare_and have their own special "vs" syntax,
            // distinct from the uniform name(args) command convention.
            if ((name == "compare" || name == "compare_and") && !IsAt(1, TokenKind.Equals))
                return ParseRawCompare();

            // Everything else may be a (possibly dotted) variable name — e.g.
            // a known struct-field name like player.Int or
            // player.levels.MagicUser (see EclhDecompiler.KnownVariableOffsets).
            // The full name is consumed first — Identifier ('.' Identifier)*
            // — and only then do we check what follows: a single-token
            // lookahead (as used above, where dots never apply) isn't enough
            // here, since the ++/--/+=/-=/= token comes after the WHOLE
            // dotted chain, not immediately after the first segment.
            var target = OperandExpr.Ref(ParseDottedName());

            if (Is(TokenKind.PlusPlus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = true, IsPrefix = false, Line = line };
            }
            if (Is(TokenKind.MinusMinus))
            {
                Advance();
                return new IncDecNode { Target = target, IsIncrement = false, IsPrefix = false, Line = line };
            }
            if (Is(TokenKind.PlusEquals) || Is(TokenKind.MinusEquals) || Is(TokenKind.SlashEquals) || Is(TokenKind.StarEquals))
            {
                string op = Is(TokenKind.PlusEquals) ? "+" : Is(TokenKind.MinusEquals) ? "-" : Is(TokenKind.SlashEquals) ? "/" : "*";
                Advance();
                var amt = ParseOperand();
                return new CompoundAssignNode { Target = target, Op = op, Amount = amt, Line = line };
            }

            // Otherwise: assignment   dest = rvalue
            Expect(TokenKind.Equals, "=");
            return ParseAssignmentRvalue(target, line);
        }

        private StmtNode ParseRawCompare()
        {
            int line = Cur.Line;
            string mnem = Advance().Text;   // "compare" or "compare_and"
            bool isAnd = mnem == "compare_and";

            var a = ParseOperand();
            // "vs" is just an identifier acting as a separator
            if (Is(TokenKind.Identifier) && Cur.Text == "vs") Advance();
            var b = ParseOperand();

            OperandExpr? c = null, d = null;
            if (isAnd)
            {
                // compare_and (a==b) && (c==d)  — decompiler emits:
                // compare_and a vs b && c vs d
                if (Is(TokenKind.AndAnd) || Is(TokenKind.OrOr)) Advance();
                c = ParseOperand();
                if (Is(TokenKind.Identifier) && Cur.Text == "vs") Advance();
                d = ParseOperand();
            }

            return new RawCompareNode { A = a, B = b, C = c, D = d, IsAnd = isAnd, Line = line };
        }



        /// <summary>Parses everything after "dest =" — shared by the plain-identifier
        /// assignment path and the ##name assignment path.</summary>
        private StmtNode ParseAssignmentRvalue(OperandExpr dest, int line)
        {
            // random(max)
            if (Is(TokenKind.KwRandom))
            {
                Advance();
                Expect(TokenKind.LParen, "(");
                var max = ParseOperand();
                Expect(TokenKind.RParen, ")");
                return new AssignNode { Dest = dest, Value = new RandomRvalue { Max = max }, Line = line };
            }

            // tablename[idx] — only when the bracket content looks like a real
            // table index (not a raw $XXXX address), to avoid mistaking
            // "area_49C3\n[$00FC] = ..." for "area_49C3[$00FC]" (the next line's
            // bracket-led statement being consumed as a table-index rvalue).
            if (Is(TokenKind.Identifier) && IsAt(1, TokenKind.LBracket)
                && !IsAt(2, TokenKind.HexNumber))
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

            var (opcode, fixedCount) = CmdOpcodes[mnem];

            // Every command uses uniform name(args) syntax, matching the
            // name() convention GOSUB/engine calls already use — including
            // "call" (direct CALL, opcode 0x2D, to an address not recognized
            // as a named engine function), now that the legacy
            // call(idx){targets} ON GOSUB dispatch syntax it used to collide
            // with is no longer parsed (superseded by the switch statement;
            // see SwitchNode/ParseSwitch).
            Expect(TokenKind.LParen, "(");

            // load_monster / setup_monster: arg3 frequently repeats arg1
            // (e.g. "setup_monster(#63, #2, #63)"). When only two arguments
            // are given, arg3 is synthesized as a copy of arg1 instead of
            // requiring the redundant value to be written out — mirroring
            // EclhDecompiler's FormatInstruction, which omits it from source
            // exactly when it does repeat arg1 (and keeps writing it in full
            // whenever it doesn't, since it isn't always redundant).
            if (opcode == 0x0B || opcode == 0x0C)
            {
                args.Add(ParseOperand());
                Expect(TokenKind.Comma, "','");
                args.Add(ParseOperand());
                if (Match(TokenKind.Comma))
                    args.Add(ParseOperand());
                else
                    args.Add(CloneOperand(args[0]));

                Expect(TokenKind.RParen, ")");
                return new CmdNode { Mnemonic = mnem, Args = args, Line = line };
            }

            // horizontal_menu / vertical_menu: the item-count operand is
            // omitted from source entirely — it's redundant with the number
            // of trailing item operands actually written, so LayoutCmd
            // synthesizes it at codegen time instead of requiring the
            // author (or decompiler) to keep a "#N" in sync by hand, the
            // same way SwitchNode's table size is computed from the case
            // list rather than written explicitly. sourceFixedCount is the
            // dict's true FixedOperands value (2/3, including the count
            // operand) minus one — the count operand itself.
            int sourceFixedCount = opcode switch
            {
                0x2B => fixedCount - 1,   // horizontal_menu: just the index variable
                0x15 => fixedCount - 1,   // vertical_menu: its two leading fixed args
                _ => fixedCount
            };

            // Read exactly sourceFixedCount operands — authoritative, no
            // lookahead guessing. The parens themselves now also give an
            // explicit boundary (a stray token before ')' is an immediate
            // error), on top of the trailing ';' every statement already
            // requires.
            for (int i = 0; i < sourceFixedCount; i++)
            {
                if (i > 0) Expect(TokenKind.Comma, "','");
                args.Add(ParseOperand());
            }

            // horizontal_menu / vertical_menu: variable arity. Source lists
            // every item explicitly, comma-separated, with no count operand
            // in between — just keep consuming ", operand" while a comma
            // follows.
            if (opcode is 0x15 or 0x2B)
            {
                while (Match(TokenKind.Comma))
                    args.Add(ParseOperand());
            }

            Expect(TokenKind.RParen, ")");
            return new CmdNode { Mnemonic = mnem, Args = args, Line = line };
        }

        /// <summary>
        /// Shallow-copies an OperandExpr — used when a value needs to appear
        /// twice in a lowered AST (e.g. load_monster/setup_monster's
        /// synthesized arg3) without two call sites accidentally sharing (and
        /// potentially mutating) the same instance.
        /// </summary>
        private static OperandExpr CloneOperand(OperandExpr o) => new()
        {
            Kind = o.Kind,
            ByteVal = o.ByteVal,
            Word = o.Word,
            SymbolName = o.SymbolName,
            StringVal = o.StringVal,
        };

        private StmtNode ParseGoto()
        {
            int line = Cur.Line;
            Advance(); // goto
            var (target, isWordImm) = ParseLabelRef();
            return new GotoNode { Target = target, IsWordImm = isWordImm, Line = line };
        }

        /// <summary>Parses a label reference, optionally prefixed with ## to mark it
        /// as WordImm-encoded rather than the default WordRef. Used for GOTO/GOSUB
        /// targets and ON GOTO/GOSUB target lists, mirroring the ## marker already
        /// used for plain operands (see EclhDecompiler FormatOp/ResolveLabel).</summary>
        private (string Name, bool IsWordImm) ParseLabelRef()
        {
            bool isWordImm = Match(TokenKind.HashHash);
            string name = Expect(TokenKind.Identifier, "label name").Text;
            return (name, isWordImm);
        }

        private StmtNode ParseSwitch()
        {
            int line = Cur.Line;
            Advance(); // switch
            Expect(TokenKind.LParen, "(");
            var idx = ParseOperand();

            // Optional explicit table size: "switch (idx, N) { ... }". Required
            // whenever a `default:` case's own (elided) index range extends past
            // the highest explicit `case` index — otherwise the true original
            // table size (operand[1] of the ON GOTO/GOSUB instruction) can't be
            // recovered from the case list alone. EclhDecompiler only emits this
            // form when that situation applies; it's optional here for backward
            // compatibility with switches that don't need it.
            int? explicitCount = null;
            if (Match(TokenKind.Comma))
                explicitCount = (int)ExpectHexOrDec();

            Expect(TokenKind.RParen, ")");
            Expect(TokenKind.LBrace, "{");

            // Case indices may appear in any order in the source (the decompiler
            // groups all indices sharing a target together, regardless of where
            // they fall in the original binary's target table), but every index
            // from 0..maxIndex must be covered exactly once. Collect into a
            // sparse map keyed by index, then validate and flatten at the end.
            var byIndex = new Dictionary<int, (string Name, bool IsWordImm)>();
            (string Name, bool IsWordImm)? defaultTarget = null;
            bool? isGosub = null;

            while (!Is(TokenKind.RBrace) && !Is(TokenKind.EOF))
            {
                bool isDefault = false;
                var pendingIndices = new List<int>();

                if (Is(TokenKind.Identifier) && Cur.Text == "default")
                {
                    // default: — collapses whatever indices aren't given an
                    // explicit "case" (up to the highest explicit case index)
                    // into this one target. Mirrors EclhDecompiler's collapsing
                    // of case groups whose target is the natural out-of-range
                    // fall-through address.
                    Advance();
                    Expect(TokenKind.Colon, ":");
                    isDefault = true;
                }
                else
                {
                    // case N:
                    if (!Is(TokenKind.Identifier) || Cur.Text != "case")
                        throw new ParseError($"Expected 'case' or 'default', got {Cur.Kind} '{Cur.Text}'", Cur.Line, Cur.Col);
                    Advance();
                    int caseIdx = (int)ExpectHexOrDec();
                    Expect(TokenKind.Colon, ":");

                    // If another "case" follows, this is a fall-through entry — collect
                    // all fall-through indices first, then read the shared action.
                    pendingIndices.Add(caseIdx);
                    while (Is(TokenKind.Identifier) && Cur.Text == "case")
                    {
                        Advance(); // case
                        pendingIndices.Add((int)ExpectHexOrDec());
                        Expect(TokenKind.Colon, ":");
                    }
                }

                // Now read the action: "goto label;" or "label();" — the
                // trailing ';' is required, matching every other statement
                // form (see ParseStatement).
                (string name, bool isWI) target;
                if (Is(TokenKind.KwGoto))
                {
                    Advance();
                    target = ParseLabelRef();
                    if (isGosub == null) isGosub = false;
                    Expect(TokenKind.Semicolon, "';'");
                }
                else if ((Is(TokenKind.Identifier) && IsAt(1, TokenKind.LParen) && IsAt(2, TokenKind.RParen)) ||
                         (Is(TokenKind.HashHash) && IsAt(1, TokenKind.Identifier) && IsAt(2, TokenKind.LParen) && IsAt(3, TokenKind.RParen)))
                {
                    var (n, wi) = ParseLabelRef();
                    Advance(); Advance(); // ( )
                    target = (n, wi);
                    if (isGosub == null) isGosub = true;
                    Expect(TokenKind.Semicolon, "';'");
                }
                else
                    throw new ParseError($"Expected goto or call in switch case", Cur.Line, Cur.Col);

                if (isDefault)
                {
                    if (defaultTarget != null)
                        throw new ParseError("Switch cannot have more than one 'default' case", line, 0);
                    defaultTarget = target;
                }
                else
                {
                    foreach (var pi in pendingIndices)
                    {
                        if (pi < 0)
                            throw new ParseError($"Switch case index {pi} cannot be negative", line, 0);
                        if (byIndex.ContainsKey(pi))
                            throw new ParseError($"Switch case {pi} declared more than once", line, 0);
                        byIndex[pi] = target;
                    }
                }
            }

            Expect(TokenKind.RBrace, "}");

            // Validate every index in [0, count) is covered, then flatten into
            // the ordered target list the binary format requires. Any index not
            // given an explicit "case" falls back to "default" when present —
            // this reproduces exactly the same flat target list as writing out
            // every index individually would, so it's byte-identical, just less
            // verbose.
            //
            // Table size: an explicit "switch (idx, N)" count is authoritative
            // when given — it's required whenever default's own elided indices
            // extend past the highest explicit case, since in that situation the
            // highest explicit case index alone under-counts the real table.
            // Without an explicit count, size falls back to (highest explicit
            // case index + 1), which is correct whenever default only fills
            // gaps below that index (the only case where omitting the count is
            // unambiguous).
            int inferredCount = byIndex.Count == 0 ? 0 : byIndex.Keys.Max() + 1;
            if (defaultTarget != null && byIndex.Count == 0 && explicitCount == null)
                throw new ParseError("Switch with only 'default' has no explicit case or count to establish the table size", line, 0);

            int count = explicitCount ?? inferredCount;
            if (explicitCount != null && explicitCount.Value <= (byIndex.Count == 0 ? -1 : byIndex.Keys.Max()))
                throw new ParseError($"Switch explicit count {explicitCount} is not large enough to cover case {byIndex.Keys.Max()}", line, 0);

            var targets = new List<(string Name, bool IsWordImm)>(count);
            for (int i = 0; i < count; i++)
            {
                if (byIndex.TryGetValue(i, out var t))
                    targets.Add(t);
                else if (defaultTarget != null)
                    targets.Add(defaultTarget.Value);
                else
                    throw new ParseError($"Switch is missing case {i} (cases must cover 0..{count - 1} with no gaps, or use 'default')", line, 0);
            }

            return new SwitchNode { Index = idx, Targets = targets, IsGosub = isGosub ?? false, Line = line };
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

                // Bare form: "if (op);" with no action — emits just the IF
                // opcode. Occurs when a labeled IF is the last instruction in
                // a range and there is no fused action (e.g.
                // "loc_9F56: if (!=);   // flag-reuse, no action").
                if (Match(TokenKind.Semicolon))
                    return new FlagReuseIfNode { Op = op, Action = null, Line = line };

                // Single-action form: the action is itself a full statement
                // parsed (and its trailing ';' consumed) via ParseStatement.
                // A bare "{" here falls through to ParseSimpleStatement's
                // "Unexpected token" error — a flag-reuse if never has a
                // block body — which is exactly the right diagnostic.
                var action = ParseStatement();
                return new FlagReuseIfNode { Op = op, Action = action, Line = line };
            }

            var cond = ParseCondition();
            Expect(TokenKind.RParen, ")");

            // Bare form with full condition: "if (A op B);" with no action —
            // occurs in the COMPARE-gating pattern where the IF gates whether
            // the next COMPARE's flags matter. Emits COMPARE+IF only; the next
            // statement emits the COMPARE that the gate controls.
            if (Match(TokenKind.Semicolon))
                return new SingleIfNode { Condition = cond, Action = null, Line = line };

            // Block form: "if (cond) { ... } [else { ... } | else if ...]"
            if (Is(TokenKind.LBrace))
            {
                var thenBody = ParseBlock();
                List<StmtNode>? elseBody = null;
                if (Match(TokenKind.KwElse))
                {
                    if (Is(TokenKind.KwIf))
                    {
                        // else-if chain: wrap as a single-statement else body
                        var nested = ParseIf();
                        elseBody = new List<StmtNode> { nested };
                    }
                    else
                    {
                        elseBody = ParseBlock();
                    }
                }
                return new BlockIfNode { Condition = cond, ThenBody = thenBody, ElseBody = elseBody, Line = line };
            }

            // Single-action form: as above, the action consumes its own ';'.
            var act = ParseStatement();
            return new SingleIfNode { Condition = cond, Action = act, Line = line };
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
            Expect(TokenKind.Semicolon, "';'");
            return new DoWhileNode { Body = body, Condition = cond, Line = line };
        }

        // ── Operands ─────────────────────────────────────────────────────────

        /// <summary>
        /// Consumes a possibly-dotted name: Identifier ('.' Identifier)*,
        /// joined with '.'. Assumes the cursor is at the leading identifier.
        /// Used for variable references that may be dotted struct-field
        /// names (e.g. "player.levels.MagicUser") — see
        /// EclhDecompiler.KnownVariableOffsets, whose registered names may
        /// contain dots to express nested-struct access (player.Int,
        /// player.class, player.levels.MagicUser). This mirrors
        /// TypeName.Member enum-argument parsing (see ParseOperand), but the
        /// joined text becomes the variable name itself rather than being
        /// resolved against a fixed enum registry — it's later resolved like
        /// any other name, against the var {} block's declared address.
        /// </summary>
        private string ParseDottedName()
        {
            var sb = new StringBuilder(Expect(TokenKind.Identifier, "identifier").Text);
            while (Is(TokenKind.Dot) && IsAt(1, TokenKind.Identifier))
            {
                Advance(); // .
                sb.Append('.').Append(Advance().Text);
            }
            return sb.ToString();
        }

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
                    return OperandExpr.Imm16Sym(ParseDottedName());
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
                    string sym = ParseDottedName();
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
            // TypeName.Member -> enum-style byte immediate (e.g. Spell.Knock),
            // resolved via EclhDecompiler.ArgEnumsByType — a shared, cross-
            // game registry of command-argument enums (see CommandArgEnums).
            // Only recognized when the identifier actually names a known enum
            // type; otherwise falls through to the plain identifier reference
            // below (so an ordinary variable/label name is never mistaken for
            // this form just because a '.' happens to follow it elsewhere).
            if (Is(TokenKind.Identifier) && IsAt(1, TokenKind.Dot) && IsAt(2, TokenKind.Identifier)
                && EclhDecompiler.ArgEnumsByType.ContainsKey(Cur.Text))
            {
                string typeName = Advance().Text;
                Advance(); // .
                string memberName = Advance().Text;
                if (!EclhDecompiler.ArgEnumsByType[typeName].TryGetValue(memberName, out byte val))
                    throw new ParseError($"Unknown {typeName} member '{memberName}'", Cur.Line, Cur.Col);
                return OperandExpr.Imm8(val);
            }
            // identifier -> named reference (variable, label, engine func, hardware
            // reg). May be a dotted struct-field name (player.Int,
            // player.levels.MagicUser) — see ParseDottedName.
            if (Is(TokenKind.Identifier))
            {
                return OperandExpr.Ref(ParseDottedName());
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

    // ════════════════════════════════════════════════════════════════════════
    // Code Generator
    // ════════════════════════════════════════════════════════════════════════

    public class CompileError : Exception
    {
        public int Line;
        public CompileError(string msg, int line = 0) : base(msg) { Line = line; }
    }

    /// <summary>
    /// Compiles a CompilationUnit AST to ECL bytecode.
    ///
    /// Two-pass design, mirroring the address-dependent nature of the format:
    ///   Pass 1 (Layout): walk the statement tree, lowering every high-level
    ///     construct (if/else, while, do/while, single-action if, flag-reuse if,
    ///     ++/--/+=/-=) into a flat list of "pseudo-instructions" — each with a
    ///     known byte size but DEFERRED operand resolution for symbolic jump
    ///     targets (labels may be forward references). This assigns every
    ///     instruction and label a final ECL address.
    ///   Pass 2 (Emit): walk the flat instruction list again, this time with
    ///     the complete address table available, and emit final bytes —
    ///     resolving every symbolic operand (label, variable, table, engine
    ///     function, hardware register) to its address.
    ///
    /// This mirrors the decompiler's TryEmitBlockIf/TryEmitWhile/TryEmitDoWhile
    /// exactly in reverse: each lowering function is the precise inverse of the
    /// corresponding Try* method, including the same negation rules.
    /// </summary>
    public class EclhCompiler
    {
        public const string Version = "0.3.23";

        private readonly CompilationUnit _unit;
        private readonly ushort _base;

        // Symbol tables
        private readonly Dictionary<string, ushort> _varAddrs = new();
        private readonly Dictionary<string, ushort> _tableAddrs = new();
        private readonly Dictionary<string, ushort> _engineFuncs = new();
        private readonly Dictionary<string, ushort> _hardwareRegs = new();
        private readonly Dictionary<string, ushort> _labelAddrs = new();   // filled during layout
        private readonly Dictionary<string, byte[]> _tableBytes = new();

        // Layout pass output: flat sequence of pseudo-instructions
        private readonly List<PseudoInstr> _flat = new();

        // Gaps between a terminal instruction and the next reachable label where
        // the decompiler's CFG correctly skipped unreachable bytes. Padded with
        // zeros in the output — addresses of subsequent labels are preserved but
        // the gap bytes won't match the original assembler padding exactly.
        private readonly List<(ushort Start, int Length)> _deadCodeGaps = new();

        public EclhCompiler(CompilationUnit unit)
        {
            _unit = unit;
            _base = unit.Base;

            // Auto-select engine functions / hardware registers from the
            // @game directive, using EclhDecompiler.GameProfiles as the single
            // source of truth (so decompile→compile round trips always agree
            // on which addresses are which without the caller having to wire
            // it up manually). Unknown/custom game names are left with empty
            // tables — SetEngineFunctions/SetHardwareRegisters can still be
            // called afterward to override or supply a custom profile.
            if (EclhDecompiler.GameProfiles.TryGetValue(unit.GameProfile, out var profile))
            {
                SetEngineFunctions(profile.EngineFunctions);
                SetHardwareRegisters(profile.HardwareRegisters);
            }
        }

        public void SetEngineFunctions(Dictionary<ushort, string> engineFuncs)
        {
            _engineFuncs.Clear();
            foreach (var (addr, name) in engineFuncs) _engineFuncs[name] = addr;
        }

        public void SetHardwareRegisters(Dictionary<ushort, string> hwRegs)
        {
            _hardwareRegs.Clear();
            foreach (var (addr, name) in hwRegs) _hardwareRegs[name] = addr;
        }

        /// <summary>A flattened instruction awaiting final byte emission.
        /// Operands may reference symbols that are resolved in pass 2.</summary>
        private class PseudoInstr
        {
            public ushort Address;          // assigned during layout pass
            public byte Opcode;
            public List<OperandExpr> Operands = new();
            public int Size;                // computed during layout
        }

        // ── Public entry point ──────────────────────────────────────────────

        /// <summary>Compile to a raw ECL block (including the 2-byte DAX prefix
        /// and 5-entry header), ready to write to a DAX file or feed back into
        /// the decompiler for round-trip verification.</summary>
        public byte[] Compile()
        {
            BuildSymbolTables();

            // Header occupies 20 bytes after the 2-byte DAX prefix, so the first
            // real instruction starts at _base + 20.
            ushort cursor = (ushort)(_base + 20);

            LayoutStatements(_unit.Statements, ref cursor);

            // Cover the case of a trailing data table with no instruction or label
            // after it — AdvancePastDataTables is otherwise only triggered by
            // EmitPseudo/PinLabel, neither of which fires after the very last table.
            AdvancePastDataTables(ref cursor);

            // Now every label has a final address. Resolve entry points.
            ushort onMove = ResolveLabelAddr(_unit.OnMove);
            ushort onSearch = ResolveLabelAddr(_unit.OnSearch);
            ushort onPreCamp = ResolveLabelAddr(_unit.OnPreCamp);
            ushort onCampInterrupted = ResolveLabelAddr(_unit.OnCampInterrupted);
            ushort onEnter = ResolveLabelAddr(_unit.OnEnter);

            // Compute total file size: 2 (DAX prefix) + 20 (header) + code.
            // codeBytes already spans every data table's byte extent — cursor was
            // advanced past each table by AdvancePastDataTables/PinLabel during
            // layout (see v0.2.5), so adding data table sizes again here would
            // double-count them and produce a buffer padded with trailing zeros
            // past the real end of file.
            int codeBytes = cursor - (_base + 20);
            int fileSize = 2 + 20 + codeBytes;

            // Extend fileSize to cover any @dead block that ends past cursor —
            // this includes trailing-padding blocks (formerly @tail) that lie past
            // all compiled code and are never reached by the layout cursor.
            foreach (var dead in _unit.DeadBlocks)
            {
                int deadEnd = FileOffset(dead.Address) + dead.Bytes.Count;
                if (deadEnd > fileSize) fileSize = deadEnd;
            }

            var outBytes = new byte[fileSize];

            // DAX prefix — 2 bytes.
            outBytes[0] = 0x00;
            outBytes[1] = 0x00;

            // Header: 5 GOTOs
            WriteGoto(outBytes, 2, onMove);
            WriteGoto(outBytes, 6, onSearch);
            WriteGoto(outBytes, 10, onPreCamp);
            WriteGoto(outBytes, 14, onCampInterrupted);
            WriteGoto(outBytes, 18, onEnter);

            // Emit code
            EmitFlatInstructions(outBytes);

            // Pad unreachable gaps between a terminal instruction and the next
            // reachable label with zeros (assembler padding, content unknown).
            foreach (var (start, length) in _deadCodeGaps)
            {
                int gapOff = FileOffset(start);
                for (int gi = 0; gi < length; gi++)
                    outBytes[gapOff + gi] = 0x00;
            }

            // Emit data tables — each written at its own declared address.
            foreach (var table in _unit.DataTables)
            {
                int off = FileOffset(table.Address);
                for (int i = 0; i < table.Bytes.Count; i++)
                    outBytes[off + i] = table.Bytes[i];
            }

            // Emit dead code blocks verbatim at their pinned addresses — covers
            // both mid-file unreachable gaps and trailing padding bytes (formerly
            // @tail). outBytes is now sized to accommodate all dead blocks.
            foreach (var dead in _unit.DeadBlocks)
            {
                int off = FileOffset(dead.Address);
                for (int i = 0; i < dead.Bytes.Count; i++)
                    outBytes[off + i] = dead.Bytes[i];
            }

            return outBytes;
        }

        private void WriteGoto(byte[] buf, int fileOffset, ushort target)
        {
            buf[fileOffset] = 0x01;       // GOTO opcode
            buf[fileOffset + 1] = 0x01;   // operand code: address reference
            buf[fileOffset + 2] = (byte)(target & 0xFF);
            buf[fileOffset + 3] = (byte)((target >> 8) & 0xFF);
        }

        private int FileOffset(ushort eclAddr) => (eclAddr - _base) + 2;

        private ushort ResolveLabelAddr(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new CompileError("Entry point not set");
            if (_labelAddrs.TryGetValue(name, out ushort addr)) return addr;
            throw new CompileError($"Undefined label '{name}' referenced as entry point");
        }

        // ── Symbol table construction ────────────────────────────────────────

        private void BuildSymbolTables()
        {
            foreach (var v in _unit.Vars)
                _varAddrs[v.Name] = v.Address;
            foreach (var t in _unit.DataTables)
                _tableAddrs[t.Name] = t.Address;
        }

        // ── Pass 1: Layout ───────────────────────────────────────────────────

        private void LayoutStatements(List<StmtNode> stmts, ref ushort cursor)
        {
            foreach (var stmt in stmts)
                LayoutStatement(stmt, ref cursor);
        }

        private void LayoutStatement(StmtNode stmt, ref ushort cursor)
        {
            // Clear the chained-compare-reuse state for any statement type that
            // doesn't manage it itself. SingleIfNode is the only statement that
            // can both consume and extend the chain (see LayoutSingleIf), so it's
            // excluded here and handles clearing internally based on whether its
            // action is a GOTO. A LabelDeclNode does NOT clear the chain — labels
            // are zero-width markers, not actual instructions, so a sequence like
            // "if (cond) goto X \n loc_Y: \n if (cond) goto Z" should still be
            // allowed to reuse the COMPARE since no real instruction intervened.
            if (stmt is not SingleIfNode && stmt is not LabelDeclNode)
                _lastCompareOperands = null;

            switch (stmt)
            {
                case LabelDeclNode lbl:
                    if (_labelAddrs.ContainsKey(lbl.Name))
                        throw new CompileError($"Duplicate label '{lbl.Name}'", lbl.Line);

                    _labelAddrs[lbl.Name] = lbl.PinnedAddress ?? cursor;

                    if (lbl.PinnedAddress.HasValue && lbl.PinnedAddress.Value != cursor)
                    {
                        int gap = lbl.PinnedAddress.Value - cursor;
                        if (gap < 0)
                            throw new CompileError(
                                $"Label '{lbl.Name}' pinned to 0x{lbl.PinnedAddress.Value:X4} " +
                                $"but layout already passed 0x{cursor:X4} — overlapping instructions",
                                lbl.Line);

                        // Positive gap: unreachable bytes after a terminal instruction
                        // (e.g. 0x00 padding after NEWECL). These are assembler artifacts
                        // that the decompiler's CFG correctly skips. Pad with zeros in the
                        // output — the bytes won't match the original (which could be
                        // anything), but the addresses of all subsequent labels are correct.
                        // A gap here is NOT a decompiler CFG bug — terminals genuinely have
                        // no fall-through, so the skipped byte is truly unreachable.
                        _deadCodeGaps.Add((cursor, gap));
                        cursor = lbl.PinnedAddress.Value;
                    }
                    break;

                case RawInstrNode raw:
                    EmitPseudo(raw.Opcode, raw.Operands, ref cursor);
                    break;

                case AssignNode asn:
                    LayoutAssign(asn, ref cursor);
                    break;

                case IncDecNode inc:
                    LayoutIncDec(inc, ref cursor);
                    break;

                case CompoundAssignNode ca:
                    LayoutCompoundAssign(ca, ref cursor);
                    break;

                case TableAssignNode ta:
                    // SAVE TABLE: op1=source(val), op2=table_base(addr-as-ref), op3=index
                    EmitPseudo(0x35, new List<OperandExpr> {
                        ta.Value, TableRefOperand(ta.TableName), ta.Index
                    }, ref cursor);
                    break;

                case GotoNode g:
                    EmitPseudo(0x01, new List<OperandExpr> { LabelOperand(g.Target, g.IsWordImm) }, ref cursor);
                    break;

                case CallSubNode call:
                    // Could be a GOSUB (user sub) or CALL (engine_func) — disambiguated
                    // by whether the name matches an engine_func; subs are resolved
                    // as label references at emit time.
                    if (_engineFuncs.ContainsKey(call.Name))
                        EmitPseudo(0x2D, new List<OperandExpr> { LabelOperand(call.Name, call.IsWordImm) }, ref cursor);
                    else
                        EmitPseudo(0x02, new List<OperandExpr> { LabelOperand(call.Name, call.IsWordImm) }, ref cursor);
                    break;

                case ExitNode:
                    EmitPseudo(0x00, new List<OperandExpr>(), ref cursor);
                    break;

                case ReturnNode:
                    EmitPseudo(0x13, new List<OperandExpr>(), ref cursor);
                    break;

                case RawCompareNode rc:
                    // Bare compare/compare_and not lifted into an if — emits the
                    // COMPARE or COMPARE AND instruction directly, setting flags
                    // for a subsequent flag-reuse IF or leaving them set at return.
                    EmitCompare(new ConditionExpr
                    {
                        IsCompoundAnd = rc.IsAnd,
                        A = rc.A, B = rc.B, C = rc.C, D = rc.D,
                        Op = "==",   // op field unused for bare COMPARE; EmitCompare only uses IsCompoundAnd
                        CompoundIsOr = false
                    }, ref cursor);
                    break;

                case SwitchNode sw:
                    {
                        var ops = new List<OperandExpr> { sw.Index, OperandExpr.Imm8((byte)sw.Targets.Count) };
                        ops.AddRange(sw.Targets.Select(t => LabelOperand(t.Name, t.IsWordImm)));
                        EmitPseudo(sw.IsGosub ? (byte)0x26 : (byte)0x25, ops, ref cursor);
                    }
                    break;

                case SingleIfNode sif:
                    LayoutSingleIf(sif, ref cursor);
                    break;

                case FlagReuseIfNode frif:
                    LayoutFlagReuseIf(frif, ref cursor);
                    break;

                case BlockIfNode bif:
                    LayoutBlockIf(bif, ref cursor);
                    break;

                case WhileNode wn:
                    LayoutWhile(wn, ref cursor);
                    break;

                case DoWhileNode dwn:
                    LayoutDoWhile(dwn, ref cursor);
                    break;

                case CmdNode cmd:
                    LayoutCmd(cmd, ref cursor);
                    break;

                default:
                    throw new CompileError($"Unhandled statement type {stmt.GetType().Name}", stmt.Line);
            }
        }

        private OperandExpr TableRefOperand(string tableName) => OperandExpr.Ref(tableName);

        /// <summary>
        /// Produces a label-reference operand for GOTO/GOSUB/CALL/ON-GOTO/ON-GOSUB
        /// targets, honoring the ## marker (WordImm) when present. Defaults to
        /// WordRef, matching every target seen in practice so far — but the marker
        /// must be respected when the source carries it, since GOTO/GOSUB targets
        /// are not guaranteed to always be WordRef-encoded (same ambiguity class as
        /// plain operands; see EclhDecompiler v1.6.7 ResolveLabel fix).
        /// </summary>
        private static OperandExpr LabelOperand(string name, bool isWordImm) =>
            isWordImm ? OperandExpr.Imm16Sym(name) : OperandExpr.Ref(name);

        // ── Assignment lowering ──────────────────────────────────────────────

        /// <summary>
        /// Converts an operand parsed as a plain reference (WordRef, from an
        /// ordinary identifier like "shared_9890") into the equivalent StringPtr
        /// operand. Used when an identifier appears as the destination of a
        /// string-literal assignment — the decompiler always classifies such
        /// destinations as StringPtr (code 0x81), never WordRef (code 0x01),
        /// even though both encode the same target address.
        /// </summary>
        private static OperandExpr ToStringPtr(OperandExpr dest)
        {
            if (dest.Kind == OperandKind.StringPtr) return dest;   // already correct (e.g. @[name] syntax)
            if (dest.Kind != OperandKind.WordRef)
                throw new CompileError(
                    $"Cannot assign a string literal to a destination of kind {dest.Kind} " +
                    $"— expected a plain variable/label reference");

            return dest.SymbolName != null
                ? OperandExpr.StrPtr(dest.SymbolName)
                : OperandExpr.StrPtr(dest.Word);
        }

        private void LayoutAssign(AssignNode asn, ref ushort cursor)
        {
            switch (asn.Value)
            {
                case OperandRvalue ov:
                    if (ov.Operand.Kind == OperandKind.StringInline)
                    {
                        // dest = "string literal"  ->  SAVE op1=StringInline, op2=StringPtr(dest)
                        // The destination must be encoded as a StringPtr operand (code 0x81),
                        // not a plain WordRef (code 0x01) — this is how the decompiler
                        // classifies the destination of a string-literal SAVE, and the two
                        // operand kinds produce different bytes despite both referencing the
                        // same address.
                        var strPtrDest = ToStringPtr(asn.Dest);
                        EmitPseudo(0x09, new List<OperandExpr> { ov.Operand, strPtrDest }, ref cursor);
                    }
                    else
                    {
                        // SAVE: op1=source, op2=dest
                        EmitPseudo(0x09, new List<OperandExpr> { ov.Operand, asn.Dest }, ref cursor);
                    }
                    break;

                case BinaryRvalue bv:
                    LayoutBinary(asn.Dest, bv, ref cursor);
                    break;

                case RandomRvalue rv:
                    // RANDOM: op1=max, op2=dest
                    EmitPseudo(0x08, new List<OperandExpr> { rv.Max, asn.Dest }, ref cursor);
                    break;

                case TableIndexRvalue tv:
                    // GETTABLE: op1=table_base, op2=index, op3=dest
                    EmitPseudo(0x2A, new List<OperandExpr> {
                        TableRefOperand(tv.TableName), tv.Index, asn.Dest
                    }, ref cursor);
                    break;

                default:
                    throw new CompileError($"Unhandled rvalue type {asn.Value.GetType().Name}", asn.Line);
            }
        }

        private void LayoutBinary(OperandExpr dest, BinaryRvalue bv, ref ushort cursor)
        {
            switch (bv.Op)
            {
                case "+":
                    // ADD: op1 + op2 -> dest, no operand swap
                    EmitPseudo(0x04, new List<OperandExpr> { bv.Lhs, bv.Rhs, dest }, ref cursor);
                    break;
                case "-":
                    // SUBTRACT: ECL encodes (rhs, lhs, dest) — op0 - op1 is wrong;
                    // actual semantics: dest = op1 - op0 (see decompiler FormatInstruction).
                    // bv.Lhs/bv.Rhs are already in source order (lhs - rhs), so swap for encoding.
                    EmitPseudo(0x05, new List<OperandExpr> { bv.Rhs, bv.Lhs, dest }, ref cursor);
                    break;
                case "*":
                    EmitPseudo(0x07, new List<OperandExpr> { bv.Lhs, bv.Rhs, dest }, ref cursor);
                    break;
                case "/":
                    EmitPseudo(0x06, new List<OperandExpr> { bv.Lhs, bv.Rhs, dest }, ref cursor);
                    break;
                case "&":
                    EmitPseudo(0x2F, new List<OperandExpr> { bv.Lhs, bv.Rhs, dest }, ref cursor);
                    break;
                case "|":
                    EmitPseudo(0x30, new List<OperandExpr> { bv.Lhs, bv.Rhs, dest }, ref cursor);
                    break;
                default:
                    throw new CompileError($"Unknown binary operator '{bv.Op}'");
            }
        }

        private void LayoutIncDec(IncDecNode inc, ref ushort cursor)
        {
            var one = OperandExpr.Imm8(1);
            if (inc.IsIncrement)
            {
                if (inc.IsPrefix)
                    // ++x  ->  ADD #1, [x], [x]   (1 is op0)
                    EmitPseudo(0x04, new List<OperandExpr> { one, inc.Target, inc.Target }, ref cursor);
                else
                    // x++  ->  ADD [x], #1, [x]   (x is op0)
                    EmitPseudo(0x04, new List<OperandExpr> { inc.Target, one, inc.Target }, ref cursor);
            }
            else
            {
                // x--  ->  SUB #1, [x], [x]   (SUBTRACT encodes rhs,lhs,dest; rhs=#1,lhs=x)
                EmitPseudo(0x05, new List<OperandExpr> { one, inc.Target, inc.Target }, ref cursor);
            }
        }

        private void LayoutCompoundAssign(CompoundAssignNode ca, ref ushort cursor)
        {
            if (ca.Op == "+")
                // x += n  ->  ADD n, [x], [x]   (amount is op0, variable is op1 and dest)
                // This is the form the original ECL compiler produces — the decompiler
                // emits "x += n" for the destIsRhs case (ADD n, [x], [x]), not destIsLhs.
                EmitPseudo(0x04, new List<OperandExpr> { ca.Amount, ca.Target, ca.Target }, ref cursor);
            else if (ca.Op == "-")
                // x -= n  ->  SUB n, [x], [x]   (rhs=n, lhs=x after SUBTRACT's unswap)
                EmitPseudo(0x05, new List<OperandExpr> { ca.Amount, ca.Target, ca.Target }, ref cursor);
            else if (ca.Op == "/")
                // x /= n  ->  DIVIDE [x], n, [x]   (dividend=op0=x, divisor=op1=n, dest=op2=x)
                // Unlike +=/-= (where either operand order gives the same numeric
                // result and the actual encoding had to be confirmed empirically —
                // see the note above), division isn't commutative: "x /= n" can
                // only ever mean x = x / n, never n / x, so x must be the dividend
                // (op0). There's no ambiguity to resolve here.
                EmitPseudo(0x06, new List<OperandExpr> { ca.Target, ca.Amount, ca.Target }, ref cursor);
            else
                // x *= n  ->  MUL [x], n, [x]   (x is op0, amount is op1, x is dest/op2)
                // Confirmed via a cross-game ADD/MUL operand-order survey (see
                // EclhDecompiler.EclhDecompilerProgram.AnalyzeCompoundAssignPatterns):
                // combined across Pool of Radiance and Curse of the Azure Bonds,
                // 15/16 compound-assign-shaped MUL instances had dest == op0, vs
                // 1/16 dest == op1. Note this runs OPPOSITE to ADD's convention —
                // MUL keeps the destination in the natural first (op0) position
                // rather than reordering to put the amount first.
                EmitPseudo(0x07, new List<OperandExpr> { ca.Target, ca.Amount, ca.Target }, ref cursor);
        }

        // ── if/while/do-while lowering ───────────────────────────────────────
        //
        // These are the precise inverses of EclhDecompiler's TryEmitSingleIf,
        // TryEmitFlagReuseIf, TryEmitBlockIf, TryEmitWhile, TryEmitDoWhile.

        // Tracks the operand pair of the most recently emitted COMPARE (not
        // COMPARE AND), so a chained single-action if with identical comparands
        // can skip re-emitting COMPARE — matching EclhDecompiler v1.8.1's
        // TryEmitChainedSingleIf, which detects this exact pattern when
        // decompiling. Cleared whenever any non-IF/non-GOTO instruction is
        // emitted, since that breaks the flag-reuse chain.
        private (OperandExpr A, OperandExpr B)? _lastCompareOperands;

        private void LayoutSingleIf(SingleIfNode sif, ref ushort cursor)
        {
            // If this condition's operands exactly match the most recently
            // emitted COMPARE (same A and B, by structural equality), and no
            // intervening instruction has broken the flag-reuse chain, skip
            // re-emitting COMPARE — just emit IF<op> + action, reusing the
            // existing flags. This mirrors the decompiler's chained single-
            // action-if pattern (COMPARE, IF, GOTO, IF, action) where the
            // second IF has no adjacent COMPARE of its own.
            bool reuseFlags = !sif.Condition.IsCompoundAnd
                && _lastCompareOperands.HasValue
                && OperandsEqual(_lastCompareOperands.Value.A, sif.Condition.A)
                && OperandsEqual(_lastCompareOperands.Value.B, sif.Condition.B);

            if (!reuseFlags)
            {
                EmitCompare(sif.Condition, ref cursor);
                if (!sif.Condition.IsCompoundAnd)
                    _lastCompareOperands = (sif.Condition.A, sif.Condition.B);
                else
                    _lastCompareOperands = null;
            }

            EmitIfOpcode(sif.Condition, negate: false, ref cursor);

            // Capture the compare-operands state before laying out the action
            var preservedCompare = _lastCompareOperands;

            if (sif.Action != null)
            {
                LayoutStatement(sif.Action, ref cursor);
                if (sif.Action is GotoNode)
                    _lastCompareOperands = preservedCompare;
                else
                    _lastCompareOperands = null;
            }
            // null action: COMPARE+IF only (gating pattern) — flags remain set
            // for the next statement's COMPARE to use. _lastCompareOperands is
            // cleared since the next thing will be an independent COMPARE statement
            // that sets its own fresh flags.
            else
            {
                _lastCompareOperands = null;
            }
        }

        /// <summary>Structural equality for two OperandExpr — same Kind and same
        /// resolved identity (SymbolName if present, else Word/ByteVal).</summary>
        private static bool OperandsEqual(OperandExpr a, OperandExpr b)
        {
            if (a.Kind != b.Kind) return false;
            if (a.SymbolName != null || b.SymbolName != null)
                return a.SymbolName == b.SymbolName;
            return a.Kind == OperandKind.ByteImm ? a.ByteVal == b.ByteVal : a.Word == b.Word;
        }

        private void LayoutFlagReuseIf(FlagReuseIfNode frif, ref ushort cursor)
        {
            // IF<op> — no COMPARE emitted; relies on prior flags.
            EmitPseudo(OpForCmpOp(frif.Op), new List<OperandExpr>(), ref cursor);
            // Action is null for bare flag-reuse IFs (labeled entry points with no
            // fused action — just emits the IF opcode, leaving the branch unresolved
            // at runtime since execution falls through unconditionally).
            if (frif.Action != null)
                LayoutStatement(frif.Action, ref cursor);
        }

        private void LayoutBlockIf(BlockIfNode bif, ref ushort cursor)
        {
            // COMPARE + IF<negated> + GOTO _after_or_else
            // then-body
            // [GOTO _after]   <- only if else exists
            // else-body (if any)
            // _after:
            EmitCompare(bif.Condition, ref cursor);
            EmitIfOpcode(bif.Condition, negate: true, ref cursor);

            string guardLabel = NewSyntheticLabel("if_else");
            EmitPseudo(0x01, new List<OperandExpr> { OperandExpr.Ref(guardLabel) }, ref cursor);

            LayoutStatements(bif.ThenBody, ref cursor);

            if (bif.ElseBody != null)
            {
                string afterLabel = NewSyntheticLabel("if_after");
                EmitPseudo(0x01, new List<OperandExpr> { OperandExpr.Ref(afterLabel) }, ref cursor);

                PinLabel(guardLabel, ref cursor);
                LayoutStatements(bif.ElseBody, ref cursor);

                PinLabel(afterLabel, ref cursor);
            }
            else
            {
                PinLabel(guardLabel, ref cursor);
            }
        }

        private void LayoutWhile(WhileNode wn, ref ushort cursor)
        {
            // _top:
            // COMPARE + IF<negated> + GOTO _after
            // body
            // GOTO _top
            // _after:
            string topLabel = NewSyntheticLabel("while_top");
            PinLabel(topLabel, ref cursor);

            EmitCompare(wn.Condition, ref cursor);
            EmitIfOpcode(wn.Condition, negate: true, ref cursor);

            string afterLabel = NewSyntheticLabel("while_after");
            EmitPseudo(0x01, new List<OperandExpr> { OperandExpr.Ref(afterLabel) }, ref cursor);

            LayoutStatements(wn.Body, ref cursor);

            EmitPseudo(0x01, new List<OperandExpr> { OperandExpr.Ref(topLabel) }, ref cursor);

            PinLabel(afterLabel, ref cursor);
        }

        private void LayoutDoWhile(DoWhileNode dwn, ref ushort cursor)
        {
            // _top:
            // body
            // COMPARE + IF<op> + GOTO _top   (condition NOT negated)
            string topLabel = NewSyntheticLabel("do_top");
            PinLabel(topLabel, ref cursor);

            LayoutStatements(dwn.Body, ref cursor);

            EmitCompare(dwn.Condition, ref cursor);
            EmitIfOpcode(dwn.Condition, negate: false, ref cursor);
            EmitPseudo(0x01, new List<OperandExpr> { OperandExpr.Ref(topLabel) }, ref cursor);
        }

        private int _syntheticCounter;
        private string NewSyntheticLabel(string prefix) => $"__{prefix}_{_syntheticCounter++}";

        /// <summary>Emit the COMPARE or COMPARE AND instruction for a condition.</summary>
        private void EmitCompare(ConditionExpr cond, ref ushort cursor)
        {
            if (cond.IsCompoundAnd)
                EmitPseudo(0x14, new List<OperandExpr> { cond.A, cond.B, cond.C!, cond.D! }, ref cursor);
            else
                EmitPseudo(0x03, new List<OperandExpr> { cond.A, cond.B }, ref cursor);
        }

        /// <summary>Emit the bare IF&lt;op&gt; opcode for a condition, optionally negated
        /// (used by block-if/while guards, which test the inverse condition to skip).</summary>
        private void EmitIfOpcode(ConditionExpr cond, bool negate, ref ushort cursor)
        {
            string op = cond.IsCompoundAnd
                ? (cond.CompoundIsOr ? "!=" : "==")   // compound form encodes as == or != on the AND result
                : cond.Op;
            string effective = negate ? NegateOp(op) : op;
            EmitPseudo(OpForCmpOp(effective), new List<OperandExpr>(), ref cursor);
        }

        private static string NegateOp(string op) => op switch
        {
            "==" => "!=", "!=" => "==",
            "<" => ">=", ">=" => "<",
            ">" => "<=", "<=" => ">",
            _ => throw new CompileError($"Cannot negate operator '{op}'")
        };

        private static byte OpForCmpOp(string op) => op switch
        {
            "==" => 0x16, "!=" => 0x17, "<" => 0x18,
            ">" => 0x19, "<=" => 0x1A, ">=" => 0x1B,
            _ => throw new CompileError($"Unknown comparison operator '{op}'")
        };

        // ── Generic command lowering ─────────────────────────────────────────
        // Uses the same authoritative Parser.CmdOpcodes table (opcode + fixed
        // operand count) so the parser and code generator can never disagree
        // about a mnemonic's arity.

        private void LayoutCmd(CmdNode cmd, ref ushort cursor)
        {
            if (!Parser.CmdOpcodes.TryGetValue(cmd.Mnemonic, out var info))
                throw new CompileError($"Unknown command mnemonic '{cmd.Mnemonic}'", cmd.Line);

            List<OperandExpr> operands = cmd.Args;

            // horizontal_menu/vertical_menu: the item-count operand isn't in
            // cmd.Args (ParseCmdStatement no longer parses it — see there),
            // so it's synthesized here from the actual number of trailing
            // item operands, the same way SwitchNode's table size is
            // computed from its case list rather than carried in the AST.
            if (info.Opcode == 0x2B)   // HORIZONTAL MENU: [indexVar, ...items] -> [indexVar, #count, ...items]
            {
                if (cmd.Args.Count < 1)
                    throw new CompileError("horizontal_menu requires at least an index variable", cmd.Line);
                operands = new List<OperandExpr> { cmd.Args[0], OperandExpr.Imm8((byte)(cmd.Args.Count - 1)) };
                operands.AddRange(cmd.Args.Skip(1));
            }
            else if (info.Opcode == 0x15)   // VERTICAL MENU: [a, b, ...items] -> [a, b, #count, ...items]
            {
                if (cmd.Args.Count < 2)
                    throw new CompileError("vertical_menu requires at least its two leading fixed arguments", cmd.Line);
                operands = new List<OperandExpr> { cmd.Args[0], cmd.Args[1], OperandExpr.Imm8((byte)(cmd.Args.Count - 2)) };
                operands.AddRange(cmd.Args.Skip(2));
            }

            EmitPseudo(info.Opcode, operands, ref cursor);
        }

        // ── Pseudo-instruction emission & sizing ─────────────────────────────

        private void EmitPseudo(byte opcode, List<OperandExpr> operands, ref ushort cursor)
        {
            var pi = new PseudoInstr { Address = cursor, Opcode = opcode, Operands = operands };
            pi.Size = 1 + operands.Sum(OperandSize);
            cursor = (ushort)(cursor + pi.Size);
            _flat.Add(pi);

            // Advance past any data tables that start exactly at the new cursor
            // position — tables are interspersed with code in address space and
            // the layout pass must account for their byte extent so synthetic
            // labels (e.g. block-if guard targets) are assigned the correct
            // address rather than landing inside the table data.
            AdvancePastDataTables(ref cursor);
        }

        /// <summary>
        /// If <paramref name="cursor"/> is at the start of a data table, advances it
        /// past the table (and any further adjacent tables). Called after each
        /// pseudo-instruction emit so that synthetic guard labels computed during
        /// layout land at the correct post-table address.
        /// </summary>
        private void AdvancePastDataTables(ref ushort cursor)
        {
            bool advanced;
            do
            {
                advanced = false;
                foreach (var t in _unit.DataTables)
                {
                    if (t.Address == cursor)
                    {
                        cursor = (ushort)(cursor + t.Bytes.Count);
                        advanced = true;
                        break;
                    }
                }
                // Also skip @dead blocks — unreachable assembler-padding bytes
                // (e.g. 0x00 after NEWECL) that lie between two reachable regions.
                // Without this, the block-if guard GOTO target lands on the dead
                // byte address rather than the next real instruction after it.
                foreach (var d in _unit.DeadBlocks)
                {
                    if (d.Address == cursor)
                    {
                        cursor = (ushort)(cursor + d.Bytes.Count);
                        advanced = true;
                        break;
                    }
                }
            } while (advanced);
        }

        /// <summary>
        /// Advance past any data tables at the current cursor, then pin the
        /// synthetic label. Use this instead of direct _labelAddrs assignment
        /// wherever a label is pinned to the current layout position.
        /// </summary>
        private void PinLabel(string label, ref ushort cursor)
        {
            AdvancePastDataTables(ref cursor);
            _labelAddrs[label] = cursor;
        }

        /// <summary>Byte size of one operand as it will be encoded — must match
        /// EclhDecompiler.TryReadOperand exactly for round-trip fidelity.</summary>
        private int OperandSize(OperandExpr op) => op.Kind switch
        {
            OperandKind.ByteImm => 2,
            OperandKind.WordRef => 3,
            OperandKind.Code02 => 3,
            OperandKind.WordImm => 3,
            OperandKind.StringInline => 2 + CompressedStringLength(op.StringVal ?? ""),
            OperandKind.StringPtr => 3,
            _ => throw new CompileError($"Unknown operand kind {op.Kind}")
        };

        /// <summary>Computes the compressed byte length for a string, matching the
        /// decompiler's DecompressString 3-bytes-per-4-chars 6-bit packing.</summary>
        private static int CompressedStringLength(string s)
        {
            // 4 chars pack into 3 bytes (6 bits each); round up.
            return (s.Length * 6 + 7) / 8;
        }

        // ── Pass 2: Emit ─────────────────────────────────────────────────────

        /// <summary>
        /// Walk the flat pseudo-instruction list and write final bytes, resolving
        /// every symbolic operand against the now-complete symbol tables.
        /// Resolution priority mirrors the decompiler's FormatOp order:
        ///   hardware register → named variable / table → label → engine function.
        /// </summary>
        private void EmitFlatInstructions(byte[] outBytes)
        {
            foreach (var pi in _flat)
            {
                int off = FileOffset(pi.Address);
                outBytes[off] = pi.Opcode;
                int cursor = off + 1;

                foreach (var op in pi.Operands)
                    cursor = EmitOperand(outBytes, cursor, op, pi);
            }
        }

        private int EmitOperand(byte[] outBytes, int cursor, OperandExpr op, PseudoInstr owner)
        {
            switch (op.Kind)
            {
                case OperandKind.ByteImm:
                    outBytes[cursor] = 0x00;
                    outBytes[cursor + 1] = op.ByteVal;
                    return cursor + 2;

                case OperandKind.WordRef:
                    {
                        ushort addr = ResolveSymbolicAddress(op, owner);
                        outBytes[cursor] = 0x01;
                        outBytes[cursor + 1] = (byte)(addr & 0xFF);
                        outBytes[cursor + 2] = (byte)((addr >> 8) & 0xFF);
                        return cursor + 3;
                    }

                case OperandKind.Code02:
                    outBytes[cursor] = 0x02;
                    outBytes[cursor + 1] = (byte)(op.Word & 0xFF);
                    outBytes[cursor + 2] = (byte)((op.Word >> 8) & 0xFF);
                    return cursor + 3;

                case OperandKind.WordImm:
                    {
                        ushort val = op.SymbolName != null
                            ? ResolveSymbolicAddress(op, owner)
                            : op.Word;
                        outBytes[cursor] = 0x03;
                        outBytes[cursor + 1] = (byte)(val & 0xFF);
                        outBytes[cursor + 2] = (byte)((val >> 8) & 0xFF);
                        return cursor + 3;
                    }

                case OperandKind.StringInline:
                    {
                        byte[] compressed = CompressString(op.StringVal ?? "");
                        outBytes[cursor] = 0x80;
                        outBytes[cursor + 1] = (byte)compressed.Length;
                        Array.Copy(compressed, 0, outBytes, cursor + 2, compressed.Length);
                        return cursor + 2 + compressed.Length;
                    }

                case OperandKind.StringPtr:
                    {
                        ushort addr = op.SymbolName != null
                            ? ResolveSymbolicAddress(op, owner)
                            : op.Word;
                        outBytes[cursor] = 0x81;
                        outBytes[cursor + 1] = (byte)(addr & 0xFF);
                        outBytes[cursor + 2] = (byte)((addr >> 8) & 0xFF);
                        return cursor + 3;
                    }

                default:
                    throw new CompileError($"Cannot emit operand kind {op.Kind}");
            }
        }

        /// <summary>
        /// Resolve a symbolic operand to its final address. Lookup priority
        /// mirrors the decompiler's FormatOp: hardware register → named variable
        /// or table → label → engine function. If the operand already carries a
        /// literal Word value (no SymbolName), it's used as-is.
        /// </summary>
        private ushort ResolveSymbolicAddress(OperandExpr op, PseudoInstr owner)
        {
            if (op.SymbolName == null) return op.Word;
            string name = op.SymbolName;

            if (_hardwareRegs.TryGetValue(name, out ushort hw)) return hw;
            if (_varAddrs.TryGetValue(name, out ushort va)) return va;
            if (_tableAddrs.TryGetValue(name, out ushort ta)) return ta;
            if (_labelAddrs.TryGetValue(name, out ushort la)) return la;
            if (_engineFuncs.TryGetValue(name, out ushort ef)) return ef;

            throw new CompileError(
                $"Undefined symbol '{name}' referenced in instruction at 0x{owner.Address:X4} " +
                $"(opcode 0x{owner.Opcode:X2})");
        }

        /// <summary>
        /// Compress a string into the 6-bit-packed format used by ECL inline
        /// strings — the exact inverse of EclhDecompiler.DecompressString.
        /// 4 source characters pack into 3 output bytes, 6 bits each.
        /// Characters are deflated: subtract 0x40 if the result of inflation
        /// would be in the printable range (mirrors InflateChar's "+0x40 if <=0x1F").
        /// </summary>
        private static byte[] CompressString(string s)
        {
            // DeflateChar is the inverse of InflateChar:
            //   InflateChar: v <= 0x1F  -> v + 0x40
            //   DeflateChar: v >= 0x40  -> v - 0x40   (only for values that came from inflation)
            // Since ECL strings only use printable ASCII (0x20-0x5F after inflation,
            // corresponding to 6-bit codes 0x00-0x1F mapped to 0x40-0x5F, and codes
            // 0x20-0x3F left as-is), the deflate rule is: if char code is in 0x40-0x5F,
            // subtract 0x40; otherwise (0x20-0x3F) use as-is. This matches InflateChar's
            // domain exactly since it only ever adds 0x40 to values <= 0x1F.
            static byte DeflateChar(char c)
            {
                uint v = c;
                if (v is >= 0x40 and <= 0x5F) return (byte)(v - 0x40);
                if (v is >= 0x20 and <= 0x3F) return (byte)v;
                throw new CompileError($"Character '{c}' (0x{v:X2}) is outside the encodable range for ECL strings");
            }

            var sixBitCodes = new List<byte>();
            foreach (char c in s)
                sixBitCodes.Add(DeflateChar(c));

            var output = new List<byte>();
            int i = 0;
            while (i < sixBitCodes.Count)
            {
                byte c0 = sixBitCodes[i];
                byte c1 = i + 1 < sixBitCodes.Count ? sixBitCodes[i + 1] : (byte)0;
                byte c2 = i + 2 < sixBitCodes.Count ? sixBitCodes[i + 2] : (byte)0;
                byte c3 = i + 3 < sixBitCodes.Count ? sixBitCodes[i + 3] : (byte)0;

                // Inverse of the decompiler's 3-byte/4-char state machine:
                //   byte0 = (c0 << 2) | (c1 >> 4)
                //   byte1 = (c1 << 4) | (c2 >> 2)
                //   byte2 = (c2 << 6) | c3
                int have = Math.Min(4, sixBitCodes.Count - i);
                byte b0 = (byte)(((c0 & 0x3F) << 2) | ((c1 >> 4) & 0x03));
                output.Add(b0);
                if (have >= 2)
                {
                    byte b1 = (byte)(((c1 & 0x0F) << 4) | ((c2 >> 2) & 0x0F));
                    output.Add(b1);
                }
                if (have >= 3)
                {
                    byte b2 = (byte)(((c2 & 0x03) << 6) | (c3 & 0x3F));
                    output.Add(b2);
                }
                i += 4;
            }

            return output.ToArray();
        }
    }
}
