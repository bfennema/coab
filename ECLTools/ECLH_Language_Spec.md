# ECLH — ECL High-Level Language
## Language Specification v2.0

---

## 1. Background and Goals

ECL (Encounter Control Language) is a bytecode interpreter used by Gold Box RPGs (Pool of
Radiance, Curse of the Azure Bonds, Secret of the Silver Blades, etc.). Each ECL file is a
flat array of bytes loaded into a fixed address range. The runtime executes instructions
sequentially, with control flow via GOTO/GOSUB/RETURN and a family of IF conditionals that
test a comparison flag set by the most recent COMPARE instruction.

**Goals of ECLH:**

1. **Lossless round-trip (phase 1):** decompile any ECL binary → ECLH source → compile back
   to bit-identical bytes. Every raw address, every operand encoding choice, and every label
   is preserved exactly.
2. **Readable source:** symbolic names instead of hex addresses, structured control flow,
   inline string literals, expression syntax for arithmetic.
3. **Authoring (phase 2, future):** write new ECL from scratch without being constrained to
   reproduce an original binary layout.

Phase 1 is complete for all Pool of Radiance ECL files as of this specification.

---

## 2. ECL Binary Encoding

### 2.1 File Structure

An ECL file has the following layout:

```
[2-byte DAX prefix]  [5 × 4-byte GOTO header]  [code + data interleaved]
```

The **DAX prefix** (2 bytes) is container metadata, not the base address. The decompiler
and compiler treat it as opaque — it is preserved verbatim.

The **5-GOTO header** is always present. Each GOTO is 4 bytes: opcode `0x01`, operand code
`0x01`, lo byte, hi byte. The five GOTOs name the five ECL entry points (see §4).

The **base address** (`@base`) is a game constant. For Pool of Radiance, `@base = 0x9900`.
All addresses in the file are ECL addresses in the range `[@base, @base + filesize)`.
`FileOffset(addr) = (addr − @base) + 2`.

### 2.2 Operand Encoding

Each operand begins with a 1-byte **code** that determines its size and type:

| Code   | Size    | Syntax in ECLH         | Meaning |
|--------|---------|------------------------|---------|
| `0x00` | 2 bytes | `#N`                   | 8-bit immediate (0–255) |
| `0x01` | 3 bytes | `name` or `[$XXXX]`    | WordRef: ECL address (read, write, or jump target) |
| `0x02` | 3 bytes | `?[$XXXX]`             | Code02: non-ECL reference, preserved verbatim |
| `0x03` | 3 bytes | `##name` or `##N`      | WordImm: 16-bit immediate or address literal |
| `0x80` | 2+N     | `"string"`             | Inline string (6-bit compressed, length-prefixed) |
| `0x81` | 3 bytes | `@[name]` or `@[$XXXX]`| StringPtr: ECL address of a string in memory |

**Critical:** `#N` (code `0x00`) and `##N` (code `0x03`) are NOT interchangeable. They
produce different byte lengths, shifting all subsequent instruction addresses. The `##`
prefix is mandatory when the original used code `0x03`.

All addresses and multi-byte values are **little-endian**.

**Code `0x01` is universal:** write destinations, read sources, jump targets, GETTABLE/
SAVETABLE bases, engine callback addresses — all use `0x01`. The instruction determines
whether the address is read, written, or jumped to.

**Code `0x02`** is parsed identically to `0x01` but excluded from ECL address tracking.
Its precise semantics are context-dependent; ECLH preserves it verbatim as `?[$XXXX]`.

### 2.3 Opcode Table

| Opcode | ECLH mnemonic      | Operands | Notes |
|--------|--------------------|----------|-------|
| 0x00   | `exit`             | 0        | Terminal: no fall-through |
| 0x01   | `goto`             | 1        | Terminal for fall-through; target enqueued |
| 0x02   | `name()` / GOSUB   | 1        | Pushes return address |
| 0x03   | `compare`          | 2        | Sets comparison flags |
| 0x04   | ADD                | 3        | op0 + op1 → op2 |
| 0x05   | SUB                | 3        | op1 − op0 → op2 (operands reversed in binary!) |
| 0x06   | `/ ` (DIV)         | 3        | op0 / op1 → op2 |
| 0x07   | `*` (MUL)          | 3        | op0 * op1 → op2 |
| 0x08   | `random(N)`        | 2        | random(0..op0) → op1 |
| 0x09   | `=` (SAVE)         | 2        | op0 → op1 |
| 0x0A   | `load_character`   | 1        | |
| 0x0B   | `load_monster`     | 3        | |
| 0x0C   | `setup_monster`    | 3        | |
| 0x0D   | `approach`         | 0        | |
| 0x0E   | `picture`          | 1        | |
| 0x0F   | `input_number`     | 2        | |
| 0x10   | `input_string`     | 2        | |
| 0x11   | `print`            | 1        | |
| 0x12   | `printclear`       | 1        | |
| 0x13   | `return`           | 0        | Terminal |
| 0x14   | `compare_and`      | 4        | Two comparisons ANDed |
| 0x15   | `vertical_menu`    | 3+N      | Variable arity; op2 = item count |
| 0x16   | IF ==              | 0        | |
| 0x17   | IF !=              | 0        | |
| 0x18   | IF <               | 0        | |
| 0x19   | IF >               | 0        | |
| 0x1A   | IF <=              | 0        | |
| 0x1B   | IF >=              | 0        | |
| 0x1C   | `clearmonsters`    | 0        | |
| 0x1D   | `partystrength`    | 1        | |
| 0x1E   | `checkparty`       | 6        | |
| 0x1F   | `notsure_1f`       | 2        | Semantics unknown |
| 0x20   | `newecl`           | 1        | Terminal: replaces ECL in memory |
| 0x21   | `load_files`       | 3        | |
| 0x22   | `party_surprise`   | 2        | |
| 0x23   | `surprise`         | 4        | |
| 0x24   | `combat`           | 0        | Falls through (not terminal) |
| 0x25   | `goto(idx){…}`     | 2+N      | ON GOTO dispatch; N = op1 (count) |
| 0x26   | `call(idx){…}`     | 2+N      | ON GOSUB dispatch; N = op1 (count) |
| 0x27   | `treasure`         | 8        | |
| 0x28   | `rob`              | 3        | |
| 0x29   | `encounter_menu`   | 14       | |
| 0x2A   | `tbl[idx]`         | 3        | GETTABLE: op0=base, op1=idx, op2=dest |
| 0x2B   | `horizontal_menu`  | 2+N      | Variable arity; op1 = item count |
| 0x2C   | `parlay`           | 6        | |
| 0x2D   | `call addr`        | 1        | Direct engine callback (no return addr) |
| 0x2E   | `damage`           | 5        | |
| 0x2F   | AND (op)           | 3        | op0 & op1 → op2 |
| 0x30   | OR (op)            | 3        | op0 \| op1 → op2 |
| 0x31   | `sprite_off`       | 0        | |
| 0x32   | `find_item`        | 1        | |
| 0x33   | `print_return`     | 0        | |
| 0x34   | `ecl_clock`        | 1 or 2   | Game-dependent (PoR: 1) |
| 0x35   | `tbl[idx] = v`     | 3        | SAVETABLE: op0=val, op1=base, op2=idx |
| 0x36   | `add_npc`          | 2        | |
| 0x37   | `load_pieces`      | 3        | |
| 0x38   | `program`          | 1        | |
| 0x39   | `who`              | 1        | |
| 0x3A   | `delay`            | 0        | |
| 0x3B   | `spell`            | 3        | |
| 0x3C   | `protection`       | 1        | |
| 0x3D   | `clear_box`        | 0        | |
| 0x3E   | `dump`             | 0        | |
| 0x3F   | `find_special`     | 1        | |
| 0x40   | `destroy_items`    | 1        | |

**Terminal instructions** (no fall-through): `exit` (0x00), `return` (0x13), `newecl` (0x20).
`combat` (0x24) is NOT terminal — subsequent code processes combat results.
`goto` (0x01) terminates sequential fall-through but enqueues its target.

---

## 3. ECLH Source File Structure

A complete ECLH file has this top-level structure (order matters for the header section,
flexible thereafter):

```eclh
@base   0x9900
@game   pool_of_radiance

var {
    …
}

data {
    …
}

on_move             = sub_name
on_search           = sub_name
on_pre_camp         = sub_name
on_camp_interrupted = sub_name
on_enter            = sub_name

// subroutines and code follow
sub_name @ 0xXXXX:
    …
```

---

## 4. Directives

### 4.1 `@base`

```eclh
@base 0x9900
```

The ECL base address for this file. All ECL addresses are in `[@base, @base + filesize)`.
Required, must come first.

### 4.2 `@game`

```eclh
@game pool_of_radiance
```

Selects game-specific constants (memory map regions, engine function table, `ecl_clock`
operand count). Required.

### 4.3 Entry Points

```eclh
on_move             = sub_name
on_search           = sub_name
on_pre_camp         = sub_name
on_camp_interrupted = sub_name
on_enter            = sub_name
```

Names the five ECL entry points compiled into the 5-GOTO header. All five must be present.
The named subroutines must be declared somewhere in the file.

### 4.4 `var {}`

```eclh
var {
    player_6E79    = 0x6E79
    area_4A34      = 0x4A34
    shared_9800    = 0x9800
    map_direction  = [$C051]   // hardware register
    …
}
```

Declares symbolic names for ECL addresses (runtime variables, hardware registers, shared
memory). These are purely aliases; the compiler substitutes the address wherever the name
appears. No bytes are emitted for `var` declarations.

Memory region naming conventions (Pool of Radiance):
- `player_XXXX` — player/character data (`0x6B00`–`0x6FFF`)  
- `area_XXXX` — area-local variables (`0x4900`–`0x4FFF`)  
- `shared_XXXX` — cross-area shared state (`0x9700`–`0x98FF`)  
- `map_XXXX` — hardware/map registers (`0xC000`+)  

### 4.5 `data {}`

```eclh
data {
    byte  tbl_9B99  @ 0x9B99 = { 0x00, 0x01, 0x02, 0x03, 0x04 }
    word  tbl_A400  @ 0xA400 = { 0x1234, 0x5678 }
}
```

Declares named data tables embedded in the ECL binary. Each entry specifies element type
(`byte` or `word`), a symbolic name, a pinned address, and the initial byte values. The
compiler emits these bytes at exactly the declared address.

Data tables may be interleaved with code in address space — this is normal and the compiler
handles it. Tables at addresses outside the ECL file (e.g. hardware registers like `0xD000`)
are declared with empty initialiser lists `= {}` and generate no output bytes.

### 4.6 `@dead`

```eclh
@dead 0xA898 { 0x03, 0x01, 0x64, 0x4A, 0x00, 0x00, 0x16, 0x00 }
```

Verbatim bytes at a specific address that are unreachable by any code path. This includes:
- Dead code after terminal instructions with no incoming jumps
- Assembler alignment padding (typically `0x00`) after `newecl`
- Trailing bytes at end-of-file

The decompiler emits `@dead` for all such regions. The compiler writes the bytes verbatim
at the pinned address, independent of normal code layout.

---

## 5. Statements

### 5.1 Assignment

```eclh
dest = rvalue
```

Compiles to `SAVE rvalue, dest` (opcode `0x09`). The operand order in the binary is
reversed from the source: `SAVE source, destination`.

**Rvalue forms:**

| Source form | Meaning |
|-------------|---------|
| `#N` | 8-bit immediate |
| `##N` | 16-bit immediate |
| `name` | WordRef to named variable |
| `[$XXXX]` | WordRef to unnamed address |
| `##name` | WordImm (16-bit literal encoding of address) |
| `@[name]` | StringPtr to named string variable |
| `"string"` | Inline string literal |
| `random(#N)` | Random number 0..N-1 |
| `tbl_XXXX[idx]` | Table lookup (GETTABLE) |
| `lhs + rhs` | Addition (ADD) |
| `lhs - rhs` | Subtraction (SUB, operands reversed in binary) |
| `lhs * rhs` | Multiply |
| `lhs / rhs` | Divide |
| `lhs & rhs` | Bitwise AND |
| `lhs \| rhs` | Bitwise OR |

### 5.2 Compound Assignment and Increment

```eclh
x += #5       // x = #5 + x   (ADD #5, [x], [x] — amount first)
x -= #3       // x = x - #3
x++           // x = x + #1   (x is op1, #1 is op0)
++x           // x = #1 + x   (x is op0, #1 is op1 — different bytes!)
x--
--x
```

The `++x`/`x++` and `x +=`/`-=` forms produce different byte sequences. The decompiler
preserves the original encoding; the compiler reproduces it faithfully.

The original ECL compiler always uses **amount-first** (`ADD n, [x], [x]`) for `+=`, so
`x += n` compiles to `ADD n, [x], [x]`.

### 5.3 Table Assignment

```eclh
tbl_XXXX[idx] = value    // SAVETABLE
```

Compiles to `SAVETABLE value, tbl_XXXX, idx` (opcode `0x35`).

### 5.4 Control Flow

```eclh
goto label              // GOTO
label()                 // GOSUB (subroutine call)
return                  // RETURN
exit                    // EXIT
newecl #N               // NEWECL — load new ECL file; terminal
```

Jump targets may carry `##` prefix if the original used WordImm encoding:
```eclh
goto ##label
##label()
```

### 5.5 Single-Action If

```eclh
if (area_4A34 == #5) goto loc_9B27
if (player_6E79 != #0) player_6E79 = #1
if (area_4A10 > #0) exit
```

Compiles to `COMPARE op1 vs op2, IF<op>, action`. The IF operator is used directly
(not negated). Action may be any single statement including goto, assignment, or exit.

### 5.6 Chained Single-Action If

When two or more single-action ifs share the same originating COMPARE (the second IF
reuses flags set by the first COMPARE), they appear as:

```eclh
if (area_4A00 == #1) goto loc_9D06
if (area_4A00 > #1) goto loc_9DC6
```

The decompiler detects this pattern via look-back; the compiler skips re-emitting COMPARE
for the second if when the operands match the most recently emitted COMPARE.

### 5.7 COMPARE-Gating If

When a COMPARE+IF pair gates whether the NEXT COMPARE's flags matter to a later IF:

```eclh
if (player_6DCA == #1)   // gates next compare
if (player_6E79 > #16) goto loc_9B53
goto loc_9E1E
```

The bare `if (cond)` with no action emits only COMPARE+IF (no action instruction). The
next statement emits the second COMPARE, which the IF gates.

### 5.8 Flag-Reuse If

```eclh
if (==) action            // flag-reuse: reuses prior COMPARE's flags
if (!=)   // flag-reuse, no action
```

Used when an IF reuses flags from a COMPARE that was set earlier in the control flow (not
immediately preceding). This occurs for labeled IF targets reachable from multiple code
paths. The decompiler identifies flag-reuse IFs during analysis; the compiler emits only
the IF opcode (no COMPARE).

### 5.9 Block If / If-Else

```eclh
if (area_4A34 >= #64) {
    …
}

if (player_6E79 == #0) {
    …
} else {
    …
}
```

Compiles to `COMPARE + IF<negated> + GOTO guard, body, [GOTO after, else body]`.
The condition is **negated** in the binary (the guard GOTO fires when the condition is
false), which is the opposite of single-action-if form.

### 5.10 While Loop

```eclh
while (area_4A00 != #0) {
    …
}
```

Compiles to: `COMPARE + IF<negated> + GOTO _after, body, GOTO _top`. The test is at
the top; the body is skipped if the condition is initially false.

### 5.11 Do-While Loop

```eclh
do {
    …
} while (shared_9802 < #8)
```

Compiles to: `body, COMPARE + IF<same-op> + GOTO _top`. The test is at the bottom; the
GOTO fires when the condition is **true** (to loop), exits by falling through when false.
This is the opposite convention from block-if (where the GOTO fires when false).

### 5.12 ON GOTO / ON GOSUB

```eclh
goto(map_direction) { loc_A, loc_B, loc_C, loc_D }
call(idx) { sub_A, sub_B, sub_C }
```

Compiles to ON GOTO (0x25) / ON GOSUB (0x26). The index operand is read from the named
variable; control transfers to the Nth target. Out-of-range indices fall through (but the
original ECL typically ensures indices are always in range, so no fall-through instruction
is needed).

### 5.13 Raw Compare

```eclh
compare area_4A34 vs #5
compare_and area_4A00 vs #1 && player_6E79 vs #3
```

A bare COMPARE with no associated IF. Occurs when flags are set for a subsequent
flag-reuse IF, or at the end of a subroutine.

### 5.14 Direct CALL

```eclh
call [$C01B]
call move_forward
```

Compiles to opcode `0x2D` (engine callback, no return address pushed). Distinct from
`name()` GOSUB which pushes a return address and uses opcode `0x02`.

### 5.15 Commands

All other instructions use mnemonic-style syntax:

```eclh
print "Hello"
printclear "Message"
approach
combat
load_monster #114, player_6E79, #114
treasure #0, #0, #0, a, b, c, d, e
clearmonsters
picture #255
protection [$AAEE]
horizontal_menu shared_9800, #1, "Option A", "Option B"
```

---

## 6. Labels and Subroutines

### 6.1 Label Declaration

```eclh
loc_9B27 @ 0x9B27:
sub_9F86 @ 0x9F86:
on_move  @ 0x9914:
```

The `@ 0xXXXX` pin is required in phase-1 decompiled output. The compiler verifies its
natural layout cursor matches the pin; a mismatch indicates a codegen bug.

In phase-2 authored code, labels may be unpinned:
```eclh
loop_top:
after_block:
```

### 6.2 Subroutine Definition

Subroutines have no special syntax beyond a label at their entry point and `return` at
their end. The `sub_` prefix is conventional but not required.

---

## 7. Operand Markers Summary

| Syntax | Operand kind | Binary code | Use case |
|--------|-------------|-------------|----------|
| `name` | WordRef | `0x01` | Named variable/label |
| `[$XXXX]` | WordRef | `0x01` | Unnamed address |
| `##name` | WordImm | `0x03` | Named address as literal |
| `##N` | WordImm | `0x03` | 16-bit integer literal |
| `#N` | ByteImm | `0x00` | 8-bit integer literal |
| `"str"` | StringInline | `0x80` | Inline string |
| `@[name]` | StringPtr | `0x81` | Named string pointer |
| `@[$XXXX]` | StringPtr | `0x81` | Unnamed string pointer |
| `?[$XXXX]` | Code02 | `0x02` | Non-ECL reference (verbatim) |

The `##` and `@[...]` markers are mandatory when the original used WordImm or StringPtr
encoding respectively — omitting them produces a different binary.

---

## 8. Decompiler Design

### 8.1 Passes

1. **Pre-scan** (`PreScanTableBases`): linear scan for GETTABLE/SAVETABLE opcodes to
   pre-populate `_dataAddrs` before CFG traversal. This prevents ON GOTO/GOSUB
   fall-through from being enqueued when a data table immediately follows.

2. **CFG traversal** (`TraverseCfg`): worklist-based reachability analysis from the five
   entry points. Enqueues fall-through successors and jump targets. Decodes instructions
   and populates `_instructions`, `_codeBytes`, `_dataAddrs`.
   - ON GOTO/GOSUB fall-through is suppressed when `_dataAddrs.Contains(fallThrough)`.
   - GOTO/GOSUB/CALL targets enqueued as jump successors.
   - IF opcodes enqueue both fall-through (false path) and skip (true path).

3. **Reference analysis** (`AnalyseReferences`): identifies labels (addresses with
   incoming jumps), flag-reuse IFs (IFs without adjacent preceding COMPARE), data table
   sizes, and GOSUB targets.

4. **Name assignment** (`AssignNames`): assigns symbolic names to variables by memory
   region, labels by type (sub_, loc_, on_), and data tables by address.

5. **Source emission** (`EmitSource`): structured control flow lifting via `EmitRange`,
   then `@dead` block emission via `FindDeadCodeGaps`.

### 8.2 Flag-Reuse IF Detection

An IF is classified as **flag-reuse** when no COMPARE immediately precedes it in
sequential code flow. The look-back walks backward through the address-ordered instruction
list, skipping `IF/GOTO` pairs (single-action-if actions) that don't interrupt the flag
chain, stopping only when a COMPARE is found (not flag-reuse) or when a GOTO that jumps
PAST the current IF is encountered (block-if guard — genuine control-flow break).

Additionally, any IF with an incoming label (a GOTO target from elsewhere) is always
classified as flag-reuse, since it's independently reachable from paths with their own
COMPARE.

### 8.3 Structured Control Flow Lifting

`EmitRange` tries each pattern in priority order at each position:

1. `TryEmitBlockIf` — COMPARE + IF<negated> + GOTO_fwd + body [+ GOTO_after + else]
2. `TryEmitWhile` — COMPARE + IF<negated> + GOTO_after + body + GOTO_top
3. `TryEmitDoWhile` — body + COMPARE + IF<same> + GOTO_top (detected by back-edge)
4. `TryEmitSingleIf` — COMPARE + IF<op> + action (adjacent triple)
5. `TryEmitChainedSingleIf` — IF<op> + action (COMPARE found via look-back)
6. `TryEmitFlagReuseIf` — IF<op> [+ action] (flag-reuse form)
7. Plain instruction emit (mnemonic fallback)

### 8.4 Dead Code Gap Detection

`FindDeadCodeGaps` builds a `bool[fileSize]` bitmap marking every byte covered by a
decoded instruction (`_codeBytes`) or data table (`_dataAddrs`). It then scans the full
file for uncovered byte ranges and emits each as a `@dead` block. This covers both
mid-file dead code and trailing padding, replacing the former `@tail` mechanism.

Out-of-file table addresses (hardware registers beyond the ECL image) are skipped when
building the bitmap.

---

## 9. Compiler Design

### 9.1 Passes

**Pass 1 — Layout:** walks the AST and assigns addresses to every instruction and label.
- `EmitPseudo(opcode, operands, ref cursor)` advances cursor and calls
  `AdvancePastDataTables(ref cursor)` after each instruction to skip over any data table
  or `@dead` block that starts at the new cursor position.
- `PinLabel(name, ref cursor)` also calls `AdvancePastDataTables` before recording the
  label address, ensuring synthetic guard labels land after any interleaved tables/gaps.
- Dead-code gaps (positive label-pin mismatches after terminal instructions) are recorded
  in `_deadCodeGaps` and zero-filled in the output.

**Pass 2 — Emit:** resolves all symbols and writes bytes into `outBytes`.

### 9.2 Symbol Resolution

Priority order: hardware registers → var addresses → table addresses → label addresses
→ engine function addresses. This matches the decompiler's `FormatOp` priority.

### 9.3 File Size Calculation

```
fileSize = 2 (DAX) + 20 (header) + codeBytes
```

Where `codeBytes = finalCursor − (@base + 20)`. The cursor already spans all data tables
(via `AdvancePastDataTables`), so data bytes are not added separately. `@dead` blocks that
end past `finalCursor` extend `fileSize` to accommodate them.

### 9.4 Chained Compare Reuse

`LayoutSingleIf` tracks `_lastCompareOperands`. When consecutive single-action-ifs have
identical condition operands and are connected only by GOTO actions, the compiler skips
re-emitting COMPARE for the second if, matching the original ECL's byte layout. The chain
is cleared by any non-GOTO action or non-SingleIfNode statement.

---

## 10. Round-Trip Invariants

These invariants are maintained by the decompiler/compiler pair to guarantee byte-exact
round-trip:

1. **Operand kind markers** (`##`, `@[...]`, `[$XXXX]`): any address resolved to a name
   still carries its encoding marker if the original used a non-default kind.

2. **GOTO/GOSUB/CALL target kinds**: jump targets preserve `##` when WordImm-encoded.

3. **Table-index vs next-statement disambiguation**: `name[...]` is only a table index
   when the bracket content is NOT a raw `$XXXX` address (which would be the start of a
   bracket-led statement on the next line).

4. **Label pins**: all labels in phase-1 output are pinned (`@ 0xXXXX`). Any layout
   mismatch is a hard error.

5. **Dead blocks**: all unreachable bytes are preserved verbatim in `@dead` blocks and
   written at their exact original addresses.

6. **SUBTRACT operand reversal**: the decompiler swaps operands (lhs, rhs) back to
   natural order; the compiler swaps them into binary order (rhs, lhs).

7. **ADD compound-assign convention**: `x += n` always compiles to `ADD n, [x], [x]`
   (amount as op0, variable as op1/dest) — the form the original ECL always uses.

