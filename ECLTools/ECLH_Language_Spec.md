# ECLH — ECL High-Level Language
## Language Specification, Decompiler Design, and Compiler Design

---

## 1. Background and Goals

ECL (Encounter Control Language) is a stack-machine bytecode used by Gold Box RPGs. Each
program is a flat array of bytes starting at a known base address (`initial_ecl_offset`). The
runtime executes instructions sequentially, with control flow via GOTO/GOSUB/RETURN and a
family of IF conditionals that work on a six-slot compare-flag register set.

**Goals of ECLH:**

1. **Lossless round-trip (phase 1 target):** decompile any ECL binary → ECLH source → compile
   back to bit-identical bytes. Every raw address, every operand encoding choice, and every
   label is preserved exactly.
2. **Readable source:** names instead of hex addresses, structured control flow visible at a
   glance, inline string literals.
3. **Compiler autonomy (phase 2):** compiler can allocate its own variable addresses and
   lay out code without being told where things live.

---

## 2. ECL Binary Encoding (the substrate)

Understanding encoding is essential for a correct compiler.

### 2.1 Instruction Layout

```
[ opcode : 1 byte ] [ operand₁ ] [ operand₂ ] … [ operandN ]
```

The number of operands (`N`) is fixed per opcode, **except** for three variable-arity opcodes:
`ON GOTO` (0x25), `ON GOSUB` (0x26), and `HORIZONTAL MENU` (0x2B). For those, the
disassembler reads two fixed operands first, then uses the value of operand 2 as `extraArgs`
and reads that many additional operands.

### 2.2 Operand (CmdSet) Encoding

Each operand is 2 or 3 bytes. All multi-byte values are **little-endian** (low byte first,
high byte second).

| `code` byte | Size  | Layout                | Meaning                                                          |
|-------------|-------|-----------------------|------------------------------------------------------------------|
| `0x00`      | 2 B   | `[0x00][val]`         | Immediate **8-bit** literal. `val` is the value (range 0–255). No high byte. |
| `0x01`      | 3 B   | `[0x01][lo][hi]`      | **ECL address reference.** `lo + (hi << 8)` is a 16-bit address in the ECL address space. Used for read sources, write destinations, and all jump targets. The instruction determines whether the address is read from, written to, or jumped to. |
| `0x02`      | 3 B   | `[0x02][lo][hi]`      | **Non-ECL reference.** Parsed identically to `0x01` but the word is explicitly excluded from ECL address tracking (`GetReferencedAddresses` skips it). Meaning is context-dependent. |
| `0x03`      | 3 B   | `[0x03][lo][hi]`      | Immediate **16-bit** literal. `lo + (hi << 8)` is the value (range 0–65535). Tracked as an address value where relevant (e.g. literal jump targets). |
| `0x80`      | 2+N B | `[0x80][len][chars…]` | Inline **string** literal. `len` is the byte length N; the next N bytes are the raw character data (6-bit compressed). |
| `0x81`      | 3 B   | `[0x81][lo][hi]`      | **String pointer.** `lo + (hi << 8)` is the ECL address of a string in memory. |

**Code `0x01` covers all ECL address operands:**
Confirmed from real binary data — `SAVE #0, [0x6E7B]` uses `0x01` for the write destination,
not a separate write-only code. GOTO, GOSUB, CALL, GETTABLE base, SAVETABLE base, and all
read/write data operands all use `0x01`. The instruction implementation determines whether
the `.Word` value is jumped to, read from via `vm_GetMemoryValue`, or written to via
`vm_SetMemoryValue`.

**Code `0x02`** is parsed as a 3-byte operand like `0x01` but is excluded from ECL address
tracking. Its precise semantics are still under investigation from real binary data.

**`initial_ecl_offset` is a game constant, not from the DAX header:**
For Pool of Radiance, `initial_ecl_offset = 0x8000`. The 2-byte DAX header (e.g. `88 13`
in the town ECL) is DAX format metadata, not the base address.

**Critical distinction — `0x00` vs `0x03`:**
`0x00` is always exactly 2 bytes (8-bit value); `0x03` is always exactly 3 bytes (16-bit
value). Using the wrong one changes instruction byte length, shifting all subsequent
addresses and corrupting labels. The decompiler must record which the original used; the
compiler must reproduce it faithfully.

### 2.3 Opcode Table

| Opcode | Mnemonic         | Fixed Operands | Notes                                               |
|--------|------------------|----------------|-----------------------------------------------------|
| 0x00   | EXIT             | 0              |                                                     |
| 0x01   | GOTO             | 1              | op1 = target address (word literal or mem-read)     |
| 0x02   | GOSUB            | 1              |                                                     |
| 0x03   | COMPARE          | 2              | Sets compare_flags[0..5]                            |
| 0x04   | ADD              | 3              | op1+op2 → op3                                       |
| 0x05   | SUBTRACT         | 3              | op2-op1 → op3                                       |
| 0x06   | DIVIDE           | 3              | op1/op2 → op3; remainder → field_67E                |
| 0x07   | MULTIPLY         | 3              | op1*op2 → op3                                       |
| 0x08   | RANDOM           | 2              | random(0..op1) → op2                                |
| 0x09   | SAVE             | 2              | op1 → op2  (assignment)                             |
| 0x0A   | LOAD CHARACTER   | 1              |                                                     |
| 0x0B   | LOAD MONSTER     | 3              |                                                     |
| 0x0C   | SETUP MONSTER    | 3              |                                                     |
| 0x0D   | APPROACH         | 0              |                                                     |
| 0x0E   | PICTURE          | 1              |                                                     |
| 0x0F   | INPUT NUMBER     | 2              |                                                     |
| 0x10   | INPUT STRING     | 2              |                                                     |
| 0x11   | PRINT            | 1              |                                                     |
| 0x12   | PRINTCLEAR       | 1              |                                                     |
| 0x13   | RETURN           | 0              |                                                     |
| 0x14   | COMPARE AND      | 4              |                                                     |
| 0x15   | VERTICAL MENU    | 3              | op3 = menu item count N; N more string operands follow |
| 0x16   | IF =             | 0              | Tests compare_flags[0]                              |
| 0x17   | IF <>            | 0              | Tests compare_flags[1]                              |
| 0x18   | IF <             | 0              | Tests compare_flags[2]                              |
| 0x19   | IF >             | 0              | Tests compare_flags[3]                              |
| 0x1A   | IF <=            | 0              | Tests compare_flags[4]                              |
| 0x1B   | IF >=            | 0              | Tests compare_flags[5]                              |
| 0x1C   | CLEARMONSTERS    | 0              |                                                     |
| 0x1D   | PARTYSTRENGTH    | 1              |                                                     |
| 0x1E   | CHECKPARTY       | 6              |                                                     |
| 0x1F   | (reserved)       | 2              |                                                     |
| 0x20   | NEWECL           | 1              |                                                     |
| 0x21   | LOAD FILES       | 3              |                                                     |
| 0x22   | PARTY SURPRISE   | 2              |                                                     |
| 0x23   | SURPRISE         | 4              |                                                     |
| 0x24   | COMBAT           | 0              |                                                     |
| 0x25   | ON GOTO          | 2+N            | op2=N; N address operands follow                    |
| 0x26   | ON GOSUB         | 2+N            | op2=N; N address operands follow                    |
| 0x27   | TREASURE         | 8              |                                                     |
| 0x28   | ROB              | 3              |                                                     |
| 0x29   | ENCOUNTER MENU   | 14             |                                                     |
| 0x2A   | GETTABLE         | 3              |                                                     |
| 0x2B   | HORIZONTAL MENU  | 2+N            | op2=N; N string operands follow                     |
| 0x2C   | PARLAY           | 6              |                                                     |
| 0x2D   | CALL             | 1              |                                                     |
| 0x2E   | DAMAGE           | 5              |                                                     |
| 0x2F   | AND              | 3              |                                                     |
| 0x30   | OR               | 3              |                                                     |
| 0x31   | SPRITE OFF       | 0              |                                                     |
| 0x32   | FIND ITEM        | 1              |                                                     |
| 0x33   | PRINT RETURN     | 0              |                                                     |
| 0x34   | ECL CLOCK        | 1              |                                                     |
| 0x35   | SAVE TABLE       | 3              |                                                     |
| 0x36   | ADD NPC          | 2              |                                                     |
| 0x37   | LOAD PIECES      | 3              |                                                     |
| 0x38   | PROGRAM          | 1              |                                                     |
| 0x39   | WHO              | 1              |                                                     |
| 0x3A   | DELAY            | 0              |                                                     |
| 0x3B   | SPELL            | 3              |                                                     |
| 0x3C   | PROTECTION       | 1              |                                                     |
| 0x3D   | CLEAR BOX        | 0              |                                                     |
| 0x3E   | DUMP             | 0              |                                                     |
| 0x3F   | FIND SPECIAL     | 1              |                                                     |
| 0x40   | DESTROY ITEMS    | 1              |                                                     |

---

## 3. ECLH Language Design

### 3.1 Philosophy

ECLH is a **statement-oriented**, **flat** language — not block-structured in the C/Pascal sense.
This matches ECL's own nature. The compiler does **not** invent block structure that has no
binary equivalent — but it does provide structured `if/else` and loop constructs that
compile to well-defined patterns of COMPARE, IF, GOTO, and back-edge GOTOs.

- Named **labels** instead of raw hex addresses
- Named **variables** instead of raw hex memory addresses
- Typed **literals** (byte, word, string)
- Explicit **operand-encoding annotations** for phase-1 round-trip fidelity
- A **var block** that declares the mapping from names to addresses (phase 1) or lets the
  compiler assign them (phase 2)

### 3.2 File Structure

Every ECL binary begins with a fixed **22-byte preamble** before any executable logic:

```
[ 2-byte DAX file header ]          ← stripped by load_ecl_dax before the VM sees it
[ 5 × GOTO instruction ]            ← read by vm_init_ecl as the entry-point table
                                       each GOTO is 4 bytes: [0x01][operand-code][lo][hi]
```

Each of the five GOTOs is a real GOTO instruction (opcode `0x01`) with a single operand.
The operand is typically a memory-read (`code=0x01`, giving a 3-byte `[0x01][lo][hi]` form),
meaning `vm_init_ecl` dereferences the address to get the actual entry-point address.
The five slots are read in this fixed order:

| Slot | Field name             | When the engine calls it                                        |
|------|------------------------|-----------------------------------------------------------------|
| 1    | `on_move`              | Each game loop iteration after `on_enter`, handles movement/actions |
| 2    | `on_search`            | Player searches or steps on a trigger location                  |
| 3    | `on_pre_camp`          | Before the party attempts to camp                               |
| 4    | `on_camp_interrupted`  | Camping is interrupted by an encounter                          |
| 5    | `on_enter`             | Main entry point — first thing called when the ECL loads        |

`vm_init_ecl` reads these five slots using `vm_LoadCmdSets` (one operand each), extracts
the `.Word` field from each result, and stores the five addresses into global fields before
execution begins. The VM's `ecl_offset` is then left pointing at the first byte after the
five GOTOs, which is where real executable code starts.

ECLH makes the five entry points explicit as top-level declarations. Because they compile
to real GOTO instructions (not a special header format), they participate in label resolution
normally:

```eclh
@module "ECL0B"
@base   0x9900              // initial_ecl_offset for this block — varies per block
@game   pool_of_radiance    // selects per-game engine function address table

// ── Entry point declarations (each compiles to one GOTO instruction) ──────
on_move              = handle_move
on_search            = handle_search
on_pre_camp          = pre_camp
on_camp_interrupted  = camp_interrupted
on_enter             = main

// ── Runtime variable declarations ─────────────────────────────────────────
var {
    word  result     @ 0x0200
    word  counter    @ 0x0202
    word  choice     @ 0x0204
}

// ── Inline data tables (embedded in the ECL binary after all code) ────────
data {
    word  damage_table @ 0x40F0 = { 0, 2, 4, 6, 8, 10, 12, 14 }
}

// ── Code ──────────────────────────────────────────────────────────────────
main:
    if (counter == 5) goto already_done
    counter = counter + 1
    do_work()
    exit

already_done:
    print "You're done."
    exit

handle_move:
    redraw()           // engine call — address resolved from @game table
    exit

handle_search:
    exit

pre_camp:
    exit

camp_interrupted:
    exit

sub do_work() {
    result = random(10)
    return
}
```

**Compilation of the entry point declarations:**

Each `on_* = label` compiles to a GOTO instruction with a `0x01` address operand carrying
the label's absolute ECL address — matching the `[0x01][lo][hi]` encoding the runtime
expects. The first instruction of real code therefore lives at `@base + 20` (5 × 4-byte
GOTOs), confirmed by the Pool of Radiance town ECL: base `0x9900` + 20 bytes = `0x9914`,
which is exactly where `on_move` points.

**`@base` is per-block, not a game constant:**
Different ECL blocks loaded by the same game use different base addresses. The `@base`
directive must be set correctly in each `.eclh` source file. The decompiler reads
`initial_ecl_offset` from the runtime context (passed via `--base` on the command line)
rather than inferring it from the file. The 2-byte DAX prefix is stripped before the
address space begins, so `file[0x0002]` = `ECL[@base + 0]`.

**Two distinct address spaces:**

| Block  | What it holds | Where it lives |
|--------|--------------|----------------|
| `var`  | Runtime variables — read and written during execution | Outside the ECL binary, in the game's working memory |
| `data` | Read-only tables — fixed values baked into the ECL at compile time | Inside the ECL binary, after all instructions |

### 3.3 Operand Syntax and Type System

An operand in ECLH has three pieces of information:
1. **Kind** (read / write / immediate / string)
2. **Value** (address, number, or string)
3. **Encoding** (which binary `code` byte to use)

For phase-1 round-trip the source must encode these exactly. For phase-2 the compiler
infers the best encoding.

#### 3.3.1 Operand Notation

| ECLH Syntax       | Binary `code` | Binary layout               | Meaning                                                     |
|-------------------|---------------|-----------------------------|-------------------------------------------------------------|
| `#N`              | `0x00`        | `[0x00][N]` (2 bytes)       | Immediate **8-bit** literal. N must be 0–255.               |
| `##N`             | `0x03`        | `[0x03][lo][hi]` (3 bytes)  | Immediate **16-bit** literal. N can be 0–65535.             |
| `[addr]`          | `0x01`        | `[0x01][lo][hi]` (3 bytes)  | ECL address reference — read, write, or jump depending on instruction context. |
| `"text"`          | `0x80`        | `[0x80][len][chars…]`       | Inline string literal.                                      |
| `@[addr]`         | `0x81`        | `[0x81][lo][hi]` (3 bytes)  | String pointer at ECL address.                              |

All of read sources, write destinations, jump targets, table bases, and engine callback
addresses use `[addr]` notation (code `0x01`). The ECLH compiler knows from the instruction
definition which position is a read versus a write; the programmer does not need to
distinguish them syntactically.

Code `0x02` appears in the binary but its semantics are not yet fully established. In
phase-1 round-trip mode the decompiler emits it as `?[addr]` to flag it for investigation;
the compiler reproduces it unchanged.

**Why `#` and `##` are not interchangeable:**
`#5` compiles to the 2-byte sequence `0x00 0x05`.
`##5` compiles to the 3-byte sequence `0x03 0x05 0x00`.
These produce different binary sizes, so using the wrong one shifts every subsequent
instruction address and corrupts all labels. The decompiler must emit the sigil that
matches the original encoding; the compiler must honour it.

`addr` can be:
- A **hex literal**: `0x0200`
- A **decimal literal**: `512`
- A **variable name** declared in `var {}`: `counter`
- A **label name** (for GOTO/GOSUB targets): `main`

#### 3.3.2 Operand Type Inference (phase 2)

When the programmer writes without explicit encoding sigils:

- `counter` on the right side of an instruction → `[counter]` (memory read, 3-byte operand)
- `counter` as a destination → `&counter` (memory write, 3-byte operand)
- An integer literal `N`:
  - If N ≤ 255 and the operand position accepts 8-bit values → `#N` (2-byte, `0x00` form)
  - If N > 255, or the position semantically requires a 16-bit value (e.g. a memory address
    or GOTO target) → `##N` (3-byte, `0x03` form)
- `"..."` → inline string literal (`0x80` form)

The compiler enforces this but always allows explicit sigils to override. In phase 2 there
is no original binary to match, so the compiler may choose the smallest correct form.

### 3.4 Label Syntax

A label is any identifier ending in `:` on its own logical line:

```
loop_top:
    ; code here
```

Labels occupy zero bytes. The compiler resolves them to the address of the next instruction.
In phase-1 mode a label may be **pinned** to an address:

```
loop_top @ 0x40A2:
    ; code here — compiler verifies this label resolves to 0x40A2
```

Pinned labels are useful when decompiling: the decompiler emits them automatically.

### 3.5 Statement Syntax

ECLH uses two distinct statement forms depending on whether an instruction computes or
moves a value:

- **Expression statements** — arithmetic, assignment, bitwise ops, and other
  value-producing instructions use standard notation (described in this section).
- **Command statements** — instructions that trigger game behaviour but don't produce a
  computed value retain keyword form (GOSUB, GOTO, EXIT, COMBAT, etc.).

Mnemonics are **case-insensitive**; the examples below use lowercase by convention.

---

#### Assignment (`SAVE` — opcode 0x09)

```eclh
dest = source
```

Examples:
```eclh
choice = 0          // immediate 8-bit:  SAVE #0, &choice
choice = counter    // variable read:    SAVE [counter], &choice
choice = 0x1234     // immediate 16-bit: SAVE ##0x1234, &choice
```

The compiler infers the operand encoding from the value and type of `source` (see §3.3.2).
The destination must always be a variable name or explicit `&addr` form.

---

#### Arithmetic (`ADD`, `SUBTRACT`, `MULTIPLY`, `DIVIDE` — opcodes 0x04–0x07)

```eclh
dest = lhs + rhs
dest = lhs - rhs
dest = lhs * rhs
dest = lhs / rhs
```

Examples:
```eclh
counter = counter + 1      // ADD  [counter], #1,       &counter
gold    = gold    - cost   // SUB  [gold],    [cost],   &gold
damage  = dice    * bonus  // MUL  [dice],    [bonus],  &damage
share   = total   / 4      // DIV  [total],   #4,       &share
```

**SUBTRACT operand order warning:** In the ECL binary, SUBTRACT is encoded as
`op2 − op1 → op3`, i.e. the right-hand side of the binary instruction is the minuend and
the left-hand side is the subtrahend. ECLH hides this: you always write `lhs - rhs` and
the compiler emits the operands in the correct swapped order automatically. The decompiler
similarly unswaps them when producing ECLH source.

**DIVIDE side-effect:** The remainder of `lhs / rhs` is stored in the engine's internal
`field_67E` register, which is not directly addressable as a named variable. If you need the
remainder, read it immediately via its raw address (typically exposed as a named constant in
`ecl_symbols.eclh`).

---

#### Bitwise Operations (`AND`, `OR` — opcodes 0x2F, 0x30)

```eclh
dest = lhs & rhs    // bitwise AND
dest = lhs | rhs    // bitwise OR
```

Examples:
```eclh
flags = flags & 0x0F    // mask lower nibble
flags = flags | 0x80    // set high bit
```

Note: these also update the compare flags (as a side-effect of `compare_variables(result, 0)`
in the runtime), so an `if` immediately after a bitwise op tests the result against zero
without needing an explicit compare.

---

#### Random (`RANDOM` — opcode 0x08)

```eclh
dest = random(max)
```

The runtime generates a value in the range `0 .. max` (inclusive, with a +1 adjustment for
values below 255 — matching the original game logic). Example:

```eclh
roll = random(20)     // random(20) → roll
```

---

#### Inline Data Tables (`data` block)

Tables are raw byte or word sequences embedded directly in the ECL binary, placed after the
last instruction. They are declared at file scope in a `data` block:

```eclh
data {
    word  damage_table @ 0x40F0 = { 0, 2, 4, 6, 8, 10, 12, 14 }
    byte  zone_ids     @ 0x4100 = { 3, 7, 1, 9, 2, 5 }
}
```

Each entry has:
- A **type** (`byte` or `word`) — controls element size and how many bytes the compiler emits
  per value. `byte` entries emit 1 byte each; `word` entries emit 2 bytes each, little-endian.
- A **name** — used to refer to the table in GETTABLE/SAVETABLE syntax
- An optional **`@ address`** pin — required for phase-1 round-trip; verifies the compiler
  places the table at the correct absolute address
- An **initialiser list** `= { v0, v1, v2, … }` — the actual data values

The compiler emits table data verbatim into the output binary at the specified address, after
all instruction bytes. In phase 2 (no address pin) the compiler places tables in address order
after the last instruction.

---

#### Table Read (`GETTABLE` — opcode 0x2A)

```eclh
dest = table_name[index]
```

Reads `vm_GetMemoryValue(table_base + index)` and stores the result in `dest`.

Operand breakdown from `CMD_GetTable`:
- op1: `cmd_ops[1].Word` — table base address, used raw (never dereferenced). Encoded as `[0x01][lo][hi]`, same pattern as GOTO. The `.Word` gives the absolute address of the table start.
- op2: `vm_GetCmdValue(2)` — index, evaluated normally (variable or literal)
- op3: `cmd_ops[3].Word` — destination write address, used raw. Encoded as `[0x02][lo][hi]`

```eclh
dmg = damage_table[tier]
```

Compiled operands:
```
op1: 0x01 0xF0 0x40   // [damage_table] — table base address, used directly
op2: 0x01 lo   hi     // [tier]         — index read from variable at runtime
op3: 0x01 lo   hi     // [dmg]          — destination write address
```

---

#### Table Write (`SAVE TABLE` — opcode 0x35)

```eclh
table_name[index] = source
```

Writes `source` to `vm_SetMemoryValue(value, table_base + index)`.

Operand breakdown from `CMD_SaveTable`:
- op1: `vm_GetCmdValue(1)` — source value, evaluated normally
- op2: `cmd_ops[2].Word` — table base address, used raw. Encoded as `[0x01][lo][hi]`
- op3: `vm_GetCmdValue(3)` — index, added to base at runtime

```eclh
damage_table[offset] = source
```

Compiled operands:
```
op1: 0x01 lo   hi     // [source]       — value read from variable
op2: 0x01 0xF0 0x40   // [damage_table] — table base address, used directly
op3: 0x01 lo   hi     // [offset]       — index read from variable
```

---

#### Input (`INPUT NUMBER`, `INPUT STRING` — opcodes 0x0F, 0x10)

```eclh
dest = input_number(max, prompt)
dest = input_string(max_len, prompt)
```

Examples:
```eclh
qty    = input_number(10, "")
name   = input_string(40, "Enter name:")
```

---

#### Function Definitions and Calls (`GOSUB`/`RETURN` — opcodes 0x02, 0x13)

Any label that is only ever reached via GOSUB is a **subroutine** and is written as a
function definition in ECLH. Calling it uses standard call syntax.

**Defining a subroutine:**

```eclh
sub do_work() {
    result = random(20)
    return
}
```

The `sub` keyword introduces a subroutine. The body continues until `return` (or `exit`).
The `{…}` braces are syntactic — the compiler emits no extra bytes for them; the body is
simply code at the label address.

**Calling a subroutine:**

```eclh
do_work()              // GOSUB do_work
```

**Conditional call:**

```eclh
if (choice == 2) do_work()    // COMPARE + IF = + GOSUB
```

**Indirect call (GOSUB via memory-read operand):**

```eclh
call([func_ptr])       // GOSUB [func_ptr]  — address read from variable at runtime
```

**Computed call (`ON GOSUB` — opcode 0x26):**

```eclh
call(index) {          // ON GOSUB [index], #N + N address operands
    sub_a
    sub_b
    sub_c
}
```

`index` selects which subroutine to call (0-based). If out of range, no call is made.

**`return`** inside a subroutine body emits opcode 0x13 exactly as before. It is only
valid inside a `sub` body or at the top level of code that is known to be called via
GOSUB.

---

#### Decompiler: Subroutine Detection

The decompiler's Name Assignment Pass (formerly pass 3, now pass 4 after fusion) classifies
each label by how it is reached:

- Reached **only** by GOSUB/ON GOSUB → emit as `sub name() {`; scan forward to the next
  `return` to find the body extent, then emit closing `}`.
- Reached **only** by GOTO/ON GOTO → emit as a plain `label:`.
- Reached by **both** → emit as a plain `label:` with a comment noting it is also called
  as a subroutine. The compiler accepts a bare label as a valid GOSUB target.

A `sub` body ends at the first `return` encountered at the same nesting depth. Because ECL
has no formal scope, a subroutine may contain internal `goto` labels; these are emitted
inside the `sub` body naturally by address order.

---

#### Command Statements (keyword form)

Instructions that trigger game behaviour without producing a computed value keep their
keyword names. `gosub` is no longer in this list — use call syntax instead. All are
case-insensitive:

```eclh
goto   label
return
exit
call   address          // CALL opcode (0x2D) — engine callback, not GOSUB
combat
approach
picture  block_id
delay
print       "text"
printclear  "text"
print_return
clear_box
sprite_off
newecl      block_id
load_files  a, b, c
load_pieces a, b, c
load_character  index
load_monster    id, count, block_id
setup_monster   sprite_id, max_dist, pic_id
clearmonsters
add_npc         npc_id, morale
who             "prompt"
dump
program         cmd
protection      addr
damage          flags, count, size, plus, var
partystrength   dest
party_surprise  dest_a, dest_b
surprise        a, b, c, d
checkparty      addr, affect, out_a, out_b, out_c, out_d
find_item       item_type
find_special    affect_type
destroy_items   item_type
ecl_clock       step
spell           spell_id, out_index, out_player
rob             all_party, pct_kept, rob_chance
treasure        cp, sp, ep, gp, pp, gems, jewels, item_block
parlay          haughty, sly, nice, meek, abusive, dest
```

Note: `call ##address` (with an explicit literal address, no parens) maps to the ECL `CALL`
opcode (0x2D) and is used for engine callbacks whose addresses are not declared in the game
profile — typically during reverse engineering. Once the function is named in a profile or
inline `engine_func` declaration, it should be called as `name()` instead.

The compiler resolves a bare `name()` call as follows, in priority order:
1. A `sub name()` defined in this source file → emits GOSUB (opcode 0x02)
2. An `engine_func name` declaration (profile or inline) → emits CALL (opcode 0x2D)
3. Neither found → compile error

---

#### Menu Statements

Menus keep their keyword form but with a cleaner syntax. The result variable comes first
(it's what the programmer cares about), the items follow in a block:

```eclh
// Vertical menu
choice = vertical_menu("Prompt text") {
    "FIGHT"
    "FLEE"
    "PARLAY"
}

// Horizontal menu
choice = horizontal_menu {
    "YES"
    "NO"
}

// Encounter menu (complex — retains named-argument form)
encounter_menu { … }
```

---

#### Computed Jump / Call (`ON GOTO`, `ON GOSUB` — opcodes 0x25, 0x26)

```eclh
goto(index) {          // ON GOTO — computed unconditional jump
    label_a
    label_b
    label_c
}

call(index) {          // ON GOSUB — computed subroutine call
    sub_a
    sub_b
    sub_c
}
```

`index` is evaluated at runtime (0-based). If out of range the jump/call is not taken.
The targets in the `{ }` block must be declared labels (`goto`) or `sub` names (`call`)
respectively, though the compiler does not enforce this distinction at parse time.

---

#### Conditional Statements (`if`, `if/else`, `if/else if/else`)

All conditional forms compile to a single consistent pattern:

```
COMPARE  lhs, rhs
IF <negated-op>       ← condition false: skip GOTO, fall into block
                         condition true (via negation: actually false): execute GOTO
GOTO _next_branch
  block body
GOTO _after           ← jump to after the entire if/else chain
_next_branch:
  ... next else-if or else or nothing ...
_after:
```

**The polarity rule:** `IF <op>` executes the next instruction when the flag is **true**,
skips it when **false**. The GOTO must fire when the condition is **false** (to skip the
block), so the compiler emits the **negated** operator:

| ECLH condition | Negated IF emitted |
|----------------|--------------------|
| `a == b`       | `IF <>` (0x17)     |
| `a != b`       | `IF =`  (0x16)     |
| `a < b`        | `IF >=` (0x1B)     |
| `a > b`        | `IF <=` (0x1A)     |
| `a <= b`       | `IF >`  (0x19)     |
| `a >= b`       | `IF <`  (0x18)     |

The compiler invariant for new code: **always emit a fresh COMPARE immediately before each
IF.** However, existing ECL binaries contain flag-reuse IFs — multiple IFs against the same
COMPARE result with non-compare instructions between them. The decompiler must handle both.

---

**Single-statement `if`** (no braces)

```eclh
if (a == b) goto somewhere
if (hp <= 0) exit
if (choice != 3) handle_choice()
```

Compiles to `COMPARE + IF<op> + action` — the action is the one instruction IF controls
directly, no negation or GOTO needed.

---

**Flag-reuse `if`** (bare operator, no operands — tests current compare flags)

Real ECL sometimes emits multiple IFs against the same COMPARE result, with non-compare
instructions between them. ECLH exposes this with a bare `if` using only an operator:

```eclh
compare player_6E81 vs #8     // sets flags once
if (>=) printclear "IT'S GREEN..."   // consumes one IF opcode
if (>=) gold = gold + wager          // reuses the same flags — bare form
if (>=) goto loc_after               // reuses again
```

The bare `if (op)` compiles to just `IF<op>` with no preceding COMPARE. Valid only when
the programmer knows the flags are still set — no intervening COMPARE or COMPARE AND.
The compiler does not verify this. The decompiler emits this form when it finds an IF
whose immediately preceding instruction is not a COMPARE.

---

**Block `if`**

```eclh
if (a == b) {
    statement1
    statement2
}
```

```
COMPARE  a, b
IF <>             ← a!=b (negated): GOTO _after, skipping block
GOTO _after
statement1
statement2
_after:
```

---

**`if / else`**

```eclh
if (a == b) {
    if_block
} else {
    else_block
}
```

```
COMPARE  a, b
IF <>             ← a!=b: GOTO _else, skipping if_block
GOTO _else
if_block
GOTO _after       ← skip else_block
_else:
else_block        ← plain fall-through, no special instruction
_after:
```

The `else` block is simply the code that all failed branches fall through to. No subroutine,
no extra COMPARE.

---

**`if / else if / … / else`**

```eclh
if (a == b) {
    block_a
} else if (c > d) {
    block_c
} else if (e != 0) {
    block_e
} else {
    block_default
}
```

```
COMPARE  a, b
IF <>
GOTO _elif1
block_a
GOTO _after

_elif1:
COMPARE  c, d
IF <=             ← negated >
GOTO _elif2
block_c
GOTO _after

_elif2:
COMPARE  e, #0
IF =              ← negated !=
GOTO _else
block_e
GOTO _after

_else:
block_default     ← fall-through, no GOTO needed

_after:
```

Each branch:
- Starts with a fresh `COMPARE + IF<negated> + GOTO _next`
- Ends with `GOTO _after` (shared by all branches)

The `else` block has no COMPARE, no IF, no GOTO — it is simply the code at `_else`, reached
by falling through all failed branches. If there is no `else`, `_else` and `_after` are the
same label.

---

**`COMPARE AND` two-pair condition**

```eclh
if (a == b && c == d) {
    block
}
```

`COMPARE AND` (opcode 0x14) sets flag[0] when both pairs equal, flag[1] when either differs.
For the block form, the GOTO must fire when the condition is false (either pair differs),
so emit `IF <>` (flag[1] = either differs = true → execute GOTO):

```
COMPARE AND  a, b, c, d
IF <>             ← either differs: GOTO _after
GOTO _after
block
_after:
```

Single-action form uses `IF =` directly: `COMPARE AND + IF = + action`.

---

**Nested `if`**

Nesting works naturally since each level is self-contained — the inner `COMPARE + IF + GOTO`
triple sits inside the outer block body and uses its own independent labels:

```eclh
if (a == b) {
    if (c > d) {
        inner_block
    }
    other_statement
}
```

```
COMPARE  a, b
IF <>
GOTO _after_outer
  COMPARE  c, d
  IF <=
  GOTO _after_inner
  inner_block
  _after_inner:
  other_statement
GOTO _after_outer   ← not needed here since nothing follows, but emitted for consistency
_after_outer:
```

---

**Decompiler: block reconstruction**

The decompiler recognises the pattern `COMPARE + IF<negated-op> + GOTO _next`:

1. Detect the triple; record the **un-negated** condition (reverse the operator).
2. Collect statements from fall-through to the `GOTO _after` as the block body.
3. If `_next` starts with another `COMPARE + IF + GOTO` triple, fold as `else if`.
4. If `_next` contains statements but no leading COMPARE, fold as `else`.
5. `_after` is the label after the final `GOTO _after` in the last branch.
6. A label targeted from outside the if/else structure cannot be folded — emit as a
   plain label with the block flattened back to raw GOTOs.

---

#### Loop Statements (`while`, `do/while`)

Loops are built from the same primitives as block `if` — COMPARE, IF, and GOTO — with
the addition of a back-edge GOTO that returns to the top of the loop.

---

**`while (cond) { block }`**

The condition is tested at the top. If it fails on entry the body never executes.

```eclh
while (a == b) {
    statement1
    statement2
}
```

```
_loop:
COMPARE  a, b
IF <>             ← negated: a!=b → GOTO _after, exit loop
GOTO _after
statement1
statement2
GOTO _loop        ← back-edge: re-test condition
_after:
```

Same negation table as block `if`. The only difference from a block `if` is the trailing
`GOTO _loop` instead of `GOTO _after`.

---

**`do { block } while (cond)`**

The body executes at least once. The condition is tested at the bottom. When true, jump
back; when false, fall through. This is the **one case where the condition is not negated**
— the GOTO fires when the condition is **true** (loop continues), and falls through when
false (loop exits).

```eclh
do {
    statement1
    statement2
} while (a == b)
```

```
_loop:
statement1
statement2
COMPARE  a, b
IF =              ← NOT negated: a==b (true) → GOTO _loop, continue
GOTO _loop
_after:           ← falls through here when a!=b
```

| Form       | IF operator used  | Fires when   | Effect              |
|------------|-------------------|--------------|---------------------|
| `while`    | Negated           | Cond false   | Exit loop           |
| `do/while` | Not negated       | Cond true    | Continue loop       |

---

**`COMPARE AND` loop forms**

```eclh
while (a == b && c == d) { block }
```
```
_loop:
COMPARE AND  a, b, c, d
IF <>             ← either differs → exit
GOTO _after
block
GOTO _loop
_after:
```

```eclh
do { block } while (a == b && c == d)
```
```
_loop:
block
COMPARE AND  a, b, c, d
IF =              ← both equal → continue
GOTO _loop
_after:
```

---

**Nested loops**

Each loop gets its own independent `_loop` and `_after` labels. The compiler generates
unique names (e.g. `_loop_1`, `_loop_2`) to avoid collisions.

---

**`break` and `continue`**

Within a loop body, `break` compiles to `GOTO _after` and `continue` compiles to
`GOTO _loop`. Both are resolved to the innermost enclosing loop's labels.

---

**Decompiler: loop reconstruction**

The decompiler distinguishes loops from if-blocks by the back-edge:

- **`while`:** detect `_loop` label, then `COMPARE + IF<negated> + GOTO _after`, then
  block body ending with `GOTO _loop`. The `_loop` label must not be targeted from
  outside the loop (other than the back-edge itself).
- **`do/while`:** detect `_loop` label, then block body, then
  `COMPARE + IF<non-negated> + GOTO _loop`. The GOTO target equals `_loop` and the
  operator is the un-negated condition.
- Any `GOTO _after` inside the body that is not the back-edge is a `break`.
- Any `GOTO _loop` inside the body that is not the final instruction is a `continue`.
- If `_loop` is targeted from outside the loop structure, it cannot be folded — emit as
  a plain label.

### 3.6 Comments

```
// Single-line comment (C++ style)
/* Multi-line
   comment */
```

### 3.7 Preprocessor / Named Constants

```
const SWORD   = 12      // item type ID
const MAX_HP  = 0x00FF

COMPARE [item_type], #SWORD
```

Constants are substituted at compile time; they have no binary representation.

### 3.8 Include

```
@include "shared_vars.eclh"
```

Allows splitting large ECL blocks across multiple source files (for phase 2 projects).

---

### 3.9 Engine Function Declarations and Game Profiles

The ECL `CALL` opcode (0x2D) invokes engine callbacks at hard-coded addresses that vary
between Gold Box games. ECLH allows these to be declared with meaningful names and resolved
automatically based on the target game.

#### Game Profile Files

A game profile is a `.eclg` file that declares the engine's callable functions and any
other per-game behavioural differences that affect compilation. The toolchain ships one
profile per supported game:

```
pool_of_radiance.eclg
curse_of_the_azure_bonds.eclg
secret_of_the_silver_blades.eclg
pools_of_darkness.eclg
...
```

A profile file uses `engine_func` declarations:

```eclg
// pool_of_radiance.eclg
@game_name "Pool of Radiance"

// ── ECL address space ─────────────────────────────────────────────────────
ecl_clock_args = 1     // ECL_CLOCK takes 1 argument (vs 2 in later games)

// ── Memory regions ────────────────────────────────────────────────────────
mem_region area_ptr    = 0x4900 .. 0x4CFF   // area state variables
mem_region player_ptr  = 0x6B00 .. 0x6EFF   // player/area2 state variables
mem_region shared_mem  = 0x9700 .. 0x98FF   // shared scratch + string buffers

// ── Map hardware registers (vm_GetMemoryValue case 4) ─────────────────────
hardware_reg map_x         = 0xC04B
hardware_reg map_y         = 0xC04C
hardware_reg map_direction = 0xC04D
hardware_reg map_wall_type = 0xC04E
hardware_reg map_wall_roof = 0xC04F

// ── Engine parameter registers (write before engine function call) ─────────
hardware_reg sound_param   = 0x03DE   // 8 = sound_a, 10 = sound_b
hardware_reg engine_b8     = 0x00B8   // word_1EE78 (internal engine state)
hardware_reg engine_b9     = 0x00B9   // word_1EE7A (internal engine state)

// ── Engine function call addresses ────────────────────────────────────────
engine_func redraw        = 0x2C90
engine_func duel_player   = 0x8000
engine_func duel_monster  = 0x8001
engine_func play_sound    = 0xBA03
engine_func move_forward  = 0xC01E
engine_func get_wall      = 0xC018
engine_func demo_frame    = 0x6803
```

#### Using Engine Functions in Source

Once a `@game` directive selects a profile, engine functions are called using standard
call syntax. The compiler resolves the name to the correct address for the target game
and emits a `CALL` instruction with that address as a 16-bit literal operand:

```eclh
@game pool_of_radiance

handle_move:
    redraw()          // compiles to: CALL ##0x2C90
    move_forward()    // compiles to: CALL ##0x2E00  ← wait, wrong game
```

Engine function calls look identical to GOSUB-based subroutine calls in source. The
compiler distinguishes them because `redraw` is declared in the game profile as an
`engine_func`, not as a `sub` in the source. It emits opcode `0x2D` (CALL) with a
word-literal operand, not opcode `0x02` (GOSUB).

#### Decompiler: Engine Function Resolution

When decompiling, the decompiler receives the `@game` profile and uses it to reverse-map
CALL addresses back to names:

```
CALL ##0x2C90   →   redraw()        // Pool of Radiance
CALL ##0x2E10   →   redraw()        // Curse of the Azure Bonds
CALL ##0xC01B   →   display_time()  // all games
```

If a CALL address is not found in the profile, the decompiler emits it as a raw call with
a comment:

```eclh
call ##0x1234    // unknown engine function
```

#### Per-Game Behavioural Differences

Some per-game differences affect instruction structure, not just addresses. These are also
declared in the profile:

```eclg
// ECL_CLOCK takes 1 argument in Pool of Radiance, 2 in later games
ecl_clock_args = 1
```

The compiler uses `ecl_clock_args` to determine how many operands to emit for the
`ecl_clock` instruction, matching the `gbl.game.EclClockArguments` runtime check.

#### Custom Engine Functions

For games or mods not covered by a built-in profile, engine functions can be declared
inline in the ECL source file:

```eclh
@game custom

engine_func redraw       = 0x3A00
engine_func play_sound   = 0x3A10
```

This is also useful for documenting a newly-discovered function during reverse engineering,
before it gets added to the official profile.

---

## 4. Example: Complete Round-Trip Fragment

Suppose the ECL binary at base `0x4000` starts with:

```
4000: 03  09 00  05 00    // COMPARE  [0x0009], [0x0005]    op1=mem-read, op2=mem-read
4005: 17                  // IF <>    (skip next if NOT unequal, i.e. skip if equal)
4006: 01  14 40           // GOTO ##0x4014                  op1=16-bit literal
4009: 09  01 09 00  02 09 00  // SAVE [0x0009] -> &0x0009   (read, write)
...
4014: 00                  // EXIT
```

ECLH decompilation:

```eclh
@module "fragment"
@base   0x4000

var {
    word  var_0009  @ 0x0009
    word  var_0005  @ 0x0005
}

entry @ 0x4000:
    if (var_0009 != var_0005) goto skip_save   // COMPARE + IF <> + GOTO

    var_0009 = var_0009                        // SAVE [0x0009], &0x0009

skip_save @ 0x4014:
    exit
```

The decompiler fuses the three-instruction sequence `COMPARE / IF <> / GOTO` into a single
`if` statement. The compiler unfolds it back to exactly three instructions at the correct
addresses. Re-compiling this source produces bit-identical bytes to the input.

**Compiler output verification** (what the compiler must emit from the `if` line above):

```
emit: 0x03                     // COMPARE opcode
emit: 0x01 0x09 0x00           // op1: [0x0009] — read from address
emit: 0x01 0x05 0x00           // op2: [0x0005] — read from address
emit: 0x17                     // IF <> opcode
emit: 0x01 0x14 0x40           // GOTO: [0x4014] — jump target address
                               // total: 10 bytes, address advances 4000→400A ✓
```

---

## 5. Decompiler Design

### 5.1 Pipeline

```
ECL binary
    │
    ▼
[0] Header Parse Pass
    - `initial_ecl_offset` (= @base) is provided externally (--base flag or
      runtime context). It is NOT read from the DAX file bytes.
    - The 2-byte DAX prefix is stripped before the address space begins:
        file_offset = (ecl_addr - base) + 2
        ecl_addr    = (file_offset - 2) + base
    - Read the 5 GOTO instructions starting at file offset 0x0002 (= ECL base):
        each is [0x01 opcode][0x01 operand-code][lo][hi] = 4 bytes
        total 20 bytes (file 0x0002..0x0015)
        the operand word (lo + hi<<8) is the entry-point ECL address
    - Record five entry-point addresses in slot order:
        slot 1 → on_move              (points to first real instruction = base+20)
        slot 2 → on_search
        slot 3 → on_pre_camp
        slot 4 → on_camp_interrupted
        slot 5 → on_enter
    - Seed the worklist (pass 1) with all five entry-point addresses
    │
    ▼
[1] Control Flow Traversal Pass  (replaces linear scan)
    - Maintains:
        · visited:   Set<ushort>  — addresses already decoded
        · worklist:  Queue<ushort> — addresses known to be code, not yet decoded
        · instructions: SortedDict<ushort, InstructionNode>
        · code_bytes: Set<ushort> — every byte claimed by a decoded instruction

    - Seed worklist with the 5 entry-point addresses from pass 0.

    - While worklist is not empty:
        · Dequeue address A
        · If A already in visited: skip
        · If A is outside [base .. base+filesize-1]: skip (cross-block reference)
        · Decode one instruction at A using the opcode table
        · Mark all bytes of the instruction in code_bytes
        · Add InstructionNode to instructions map
        · Mark A as visited
        · Compute successors and add them to worklist:

            EXIT, RETURN         → no successors (terminal)
            GOTO target          → worklist.add(target)
            GOSUB target         → worklist.add(target)    // also fall-through below
            ON GOTO  targets[]   → worklist.add each target; fall-through (out-of-range case)
            ON GOSUB targets[]   → worklist.add each target; fall-through
            IF <op>              → two successors:
                                     worklist.add(A + instr_size)     // condition true: skip
                                     worklist.add(A + instr_size + next_instr_size) // condition false: next
                                   NOTE: IF successor requires peeking at next instr size
                                         to find the two possible resume points
            NEWECL               → no successors (switches block, terminates this block)
            all others           → fall-through: worklist.add(A + instr_size)

        · Special handling for GOSUB: add fall-through successor in addition to
          the GOSUB target, because execution returns to the instruction after GOSUB.

    - Bytes in the file range [base..base+filesize-1] NOT in code_bytes after
      traversal is complete are DATA bytes — they belong to inline tables or
      padding, not instructions. Collect contiguous runs of data bytes as
      candidate data regions.

    - If a worklist entry decodes to an unknown opcode: mark it as data, do NOT
      add to instructions. Log a warning with the address.
    │
    ▼
[2] Reference Analysis Pass
    - For every GOTO/GOSUB/ON GOTO/ON GOSUB: record target as a code label
    - For every operand with code `0x01`: record the word as an address reference
    - Classify each referenced address using the game profile's `mem_region` declarations:
        · Falls in a declared `mem_region` → runtime variable (emit in `var {}`)
        · Falls within [base .. base+filesize-1] AND in code_bytes → code label
        · Falls within [base .. base+filesize-1] AND NOT in code_bytes → data table
        · Matches an `engine_func` address in the game profile → engine function call
        · Above all declared regions (e.g. 0xC000+) → hardware/engine register address
        · code `0x02` operand → flag for investigation, preserve verbatim
    │
    ▼
[3] Data Region Analysis Pass
    - For each address classified as a data table in pass 2:
        · Record the base address and all index values used with GETTABLE/SAVETABLE
        · Infer element type: if all indices are even and access pattern is stride-2,
          emit as `word` table; otherwise emit as `byte` table
        · Determine table length: contiguous data bytes from base address to either
          the next code_bytes address or end of file
        · Decode the raw bytes into typed initialiser values
    │
    ▼
[4] Conditional Fusion Pass
    - Scan instructions for COMPARE/COMPARE_AND + IF<op> + action triples
    - Fuse into IfNode / BlockIfNode / WhileNode / DoWhileNode as appropriate
    - Any IF not preceded by its own COMPARE is left as a raw instruction with warning
    │
    ▼
[5] Name Assignment Pass
    - Assign names: "sub_XXXX" for GOSUB-only targets,
                    "loc_XXXX" for other jump targets,
                    "var_XXXX" for mem_region variables,
                    "tbl_XXXX" for data tables
    - Apply known-names map if provided
    │
    ▼
[6] Var/Data Block Emission
    - Emit `var { … }` for runtime variable addresses
    - Emit `data { … }` for inline table declarations (with initialisers)
    │
    ▼
[7] Code Emission Pass
    - Walk instructions in address order
    - Before each instruction: emit any pinned labels at this address
    - Emit fused constructs (if/else, while, do/while) where fusion succeeded
    - Emit raw instructions where fusion did not apply
    - Emit `{…}` blocks for variable-arity instructions
    │
    ▼
ECLH source text
```

### 5.2 InstructionNode and IfNode Structure

```csharp
class InstructionNode
{
    public ushort  Address;
    public byte    Opcode;
    public string  Mnemonic;
    public List<OperandNode> Operands;
    public int     ByteLength;   // total bytes including opcode
}

/// <summary>
/// Produced by the Conditional Fusion Pass. Represents a fused
/// COMPARE + IF&lt;op&gt; + action triple as a single logical unit.
/// </summary>
class IfNode
{
    public ushort  CompareAddress;   // address of the COMPARE instruction
    public ushort  IfAddress;        // address of the IF opcode
    public ushort  ActionAddress;    // address of the skipped/taken instruction

    public OperandNode Lhs;          // first COMPARE operand
    public OperandNode Rhs;          // second COMPARE operand
    public string      Op;           // "==" | "!=" | "<" | ">" | "<=" | ">="
    public InstructionNode Action;   // the single statement after the IF

    // For COMPARE AND, both pairs are stored:
    public OperandNode? Lhs2;
    public OperandNode? Rhs2;

    public int ByteLength =>
        CompareNode.ByteLength + 1 /* IF opcode */ + Action.ByteLength;
}

class OperandNode
{
    public byte    Code;         // 0x00, 0x01, 0x02, 0x03, 0x80, 0x81
    public ushort  Word;         // address or literal value
    public string? StringValue;  // for code 0x80/0x81
    public string  ToEclh(NameMap names) { … }
}
```

### 5.3 Handling Variable-Arity Instructions

The disassembler already handles these correctly (see `EclDisassembler.cs` lines 28–57).
The decompiler reuses the same decode logic and emits the extra operands inside `{…}`.

### 5.4 Known-Names Map (Optional Input)

For projects that have already partially named variables, the decompiler accepts a JSON
sidecar:

```json
{
  "variables": {
    "0x0200": "player_hp",
    "0x0202": "player_mp"
  },
  "labels": {
    "0x4020": "combat_start",
    "0x4080": "flee_handler"
  }
}
```

Names from this map override the auto-generated `var_XXXX` / `loc_XXXX` defaults.

---

## 6. Compiler Design

### 6.1 Compilation Phases

```
ECLH source text
    │
    ▼
[1] Lexer
    - Tokenise: keywords, identifiers, sigils (#, ##, [, &, @),
      string literals, numeric literals (hex/decimal), punctuation
    │
    ▼
[2] Parser → AST
    - Directives: @module, @base, @entry, @include
    - var { } block → VarDecl list
    - const declarations → ConstMap
    - Label definitions (with optional @ pin)
    - `if (lhs op rhs) action` → IfNode in AST
    - All other statements → InstructionAST nodes
    │
    ▼
[3] Symbol Resolution (Pass 1)
    - Build SymbolTable: name → (kind=VAR|LABEL|CONST, address|value)
    - For phase-1: use pinned addresses from var{} and label @ annotations
    - For phase-2: defer VAR addresses to Pass 4
    - Expand all const references
    │
    ▼
[4] if-Statement and Loop Lowering
    Invariant: every IF opcode is always immediately preceded by its own COMPARE.

    - Single-action `if (cond) action`:
        emit COMPARE + IF<cond-op> + action

    - Block `if (cond) { block }`:
        emit COMPARE + IF<negated-op> + GOTO(_after)
             + block-stmts
             + GOTO(_after)    ← omitted if block ends with exit/return
        + _after label

    - `if/else`:
        emit COMPARE + IF<negated-op> + GOTO(_else)
             + if-block-stmts + GOTO(_after)
             + _else label + else-block-stmts
             + _after label

    - `if/else if/…/else` chain:
        emit each branch as COMPARE + IF<negated-op> + GOTO(_next) + block + GOTO(_after)
        final else (if present) falls through at _else with no COMPARE or GOTO
        all branches share one _after label

    - `while (cond) { block }`:
        emit _loop label
             + COMPARE + IF<negated-op> + GOTO(_after)
             + block-stmts
             + GOTO(_loop)
             + _after label
        `break` → GOTO(_after); `continue` → GOTO(_loop)

    - `do { block } while (cond)`:
        emit _loop label
             + block-stmts
             + COMPARE + IF<cond-op> + GOTO(_loop)   ← NOT negated
             + _after label
        `break` → GOTO(_after); `continue` → GOTO(_loop)

    - `COMPARE AND`:
        single-action: COMPARE AND + IF = + action
        block if/while: COMPARE AND + IF <> + GOTO(_after/next) + ...
        do/while: COMPARE AND + IF = + GOTO(_loop)

    - All synthetic labels added to symbol table;
        pinned to computed addresses in phase-1 mode for verification
    │
    ▼
[5] Address Assignment (Phase 2 only)
    - Walk instruction list in order, computing byte size of each instruction
    - Assign variable addresses from a free-address pool
      (starting after the code region, or in a user-specified data segment)
    - Two-iteration fixpoint: variable-arity instructions whose size depends
      on operand values may need a second pass
    │
    ▼
[6] Label Resolution (Pass 2)
    - Walk instruction list again now that all addresses are known
    - Resolve every label reference to its 16-bit address
    - In phase-1 mode: verify each pinned label matches the computed address;
      emit an error if there is a mismatch
    │
    ▼
[7] Code Generation
    - **First:** emit the five entry-point GOTOs — for each `on_* = label` declaration,
      in slot order (on_move, on_search, on_pre_camp, on_camp_interrupted, on_enter),
      resolve the label to its absolute address and emit:
        [0x01]                  // GOTO opcode
        [0x01][lo][hi]          // mem-read operand carrying the target address
      (20 bytes total before any other instruction)
    - Then emit each InstructionAST in order: opcode byte + operand bytes
    - Operand encoding follows the sigil rules in §3.3.1
    - Variable-arity instructions: compute count, emit header operands, then extra operands
    - **Last:** emit all `data` block entries verbatim as raw bytes at their declared
      (or assigned) addresses
    │
    ▼
ECL binary ([]byte)
```

### 6.2 Instruction Size Calculation

Knowing the byte size of each instruction is essential for label resolution.

```
size(instruction) = 1                          // opcode byte
                  + Σ size(operandᵢ)           // all operands
```

```
size(operand):
    code == 0x00 → 2           (code byte + 1 value byte; 8-bit literal)
    code == 0x01 → 3           (code byte + 2 address bytes, little-endian)
    code == 0x02 → 3           (code byte + 2 address bytes, little-endian)
    code == 0x03 → 3           (code byte + 2 value bytes, little-endian; 16-bit literal)
    code == 0x80 → 2 + len(string)   (code byte + length byte + N char bytes)
    code == 0x81 → 3           (code byte + 2 address bytes, little-endian)
```

Variable-arity instructions add `extraArgs × size(each_extra_operand)` on top of the
fixed-operand bytes.

### 6.3 Phase-1 Constraint: Preserving Encoding Choices

In phase 1 the compiler must reproduce the exact binary. This means:

- An operand originally encoded as `0x03 LO HI` (16-bit literal, 3 bytes) must not be
  silently re-encoded as `0x00 N` (8-bit literal, 2 bytes) even when the value fits in a
  byte — the instruction would shrink by one byte, shifting all subsequent addresses and
  corrupting every label that follows.
- Conversely, an operand originally encoded as `0x00 N` must not be widened to `0x03 N 0x00`.
- The decompiler **must emit the correct sigil** (`#` for `0x00`, `##` for `0x03`) to record
  which encoding the original used, so the compiler can reproduce it without guessing.
- If a label target was encoded as a `0x03` 16-bit literal (the normal case for GOTO/GOSUB
  addresses), the compiler emits `0x03`; if it was a `0x01` memory-read (indirect jump),
  the compiler emits `0x01`.

**Concrete size impact:**
An instruction `ADD [a], #5, &b` has operands of sizes 3 + 2 + 3 = 8, total 9 bytes.
If `#5` were silently promoted to `##5` it would become 3 + 3 + 3 = 9, total 10 bytes —
every label after this instruction would be off by one.

### 6.4 Phase-2 Variable Allocation

The allocator maintains a **free list** of word-aligned addresses in the data region.
Variables are allocated first-fit. The region is configurable:

```
@data_region  0x0200 .. 0x03FF   // compiler may use this range
```

Byte variables are allocated at any free offset. Word variables are aligned to 2-byte
boundaries (to match the original game's conventions).

### 6.5 Error Reporting

All errors carry source location (file, line, column):

```
error: label "skip_save" expected at 0x4014 but computed address is 0x4013
  --> fragment.eclh:18:1
   |
18 | skip_save @ 0x4014:
   | ^^^^^^^^^
```

---

## 7. Standard Library of Named Constants (optional include)

A companion file `ecl_symbols.eclh` will define well-known addresses found across Gold Box
games:

```
// ecl_symbols.eclh
// Gold Box shared ECL memory map (Pool of Radiance / Curse of the Azure Bonds)

const RESULT_LOC      = 0x02CB   // standard result location used by many commands
const PARTY_SIZE      = 0x02CD
const PLAYER_HP       = 0x0080   // current player's HP field (relative to SelectedPlayer)

// Item types
const ITEM_LONG_SWORD = 0x0004
const ITEM_PLATE_MAIL = 0x000F
// … etc.
```

---

## 8. Toolchain Command-Line Interface

```
eclh decompile  <input.ecl>  --base 0x9900  --game pool_of_radiance  [--names names.json]  [-o output.eclh]
eclh compile    <input.eclh> [-o output.ecl] [--verify <original.ecl>]
eclh roundtrip  <input.ecl>  --base 0x9900  --game pool_of_radiance   // decompile then recompile and diff
eclh symbols    <input.eclh>                                           // dump symbol table
eclh games                                                             // list known game profiles
eclh profile    <game>                                                 // dump engine functions for a game
```

`--base` specifies `initial_ecl_offset` for the ECL block being decompiled. This varies
per block — it is not read from the DAX file. For Pool of Radiance block 0 it is `0x9900`;
other blocks use different values. The value must be determined from the runtime context
(e.g. by inspecting what address the game loads the block at, or from block metadata).

`--game` accepts a built-in profile name (e.g. `pool_of_radiance`) or a path to a custom
`.eclg` file. If omitted, the decompiler emits raw addresses for all variable and call
references with comments noting they are unclassified.

---

## 9. Implementation Roadmap

### Phase 1 (Decompiler — In Progress)

| Step | Task | Status |
|------|------|--------|
| 1a | CFG-traversal decompiler (`EclhDecompiler.cs`) | ✓ Done |
| 1b | Opcode table, operand reader, string decompression | ✓ Done |
| 1c | Entry-point header parsing | ✓ Done |
| 1d | Reference analysis: variable/label/table/engine classification | ✓ Done |
| 1e | Name assignment: `sub_`, `loc_`, `area_`, `player_`, `shared_`, `tbl_` | ✓ Done |
| 1f | ECLH source emission with ECLH-style formatting | ✓ Done |
| 1g | Flag-reuse IF detection (bare `if (op)` form) | Needed |
| 1h | `##addr` operands in mem_regions named as variables | Needed |
| 1i | `hardware_reg` address naming from game profile | Needed |
| 1j | Fused `if/else`, `while`, `do/while` reconstruction | Needed |
| 1k | Known-names JSON sidecar support | Needed |
| 1l | Run against all Pool of Radiance ECL blocks and validate | Needed |

### Phase 2 (Compiler — Round-Trip Fidelity)

| Step | Task |
|------|------|
| 2a | ECLH lexer and parser → AST |
| 2b | Symbol resolution: `var`, `data`, `sub`, `label`, `const` |
| 2c | if-statement and loop lowering (COMPARE+IF+GOTO patterns) |
| 2d | Code generation: emit opcode bytes + operand bytes |
| 2e | `eclh roundtrip` command: decompile → recompile → byte-compare |
| 2f | Phase-1 pinned-label verification |

### Phase 3 (Compiler Autonomy)

| Step | Task |
|------|------|
| 3a | Free-list variable allocator for phase-2 variable placement |
| 3b | Two-pass label resolver with fixpoint for variable-arity instructions |
| 3c | Type inference for operand encoding (`#` vs `##`) |
| 3d | `@include` preprocessor |
| 3e | ECL standard library constants file |

### Phase 4 (Higher-Level Constructs)

| Step | Task |
|------|------|
| 4a | Fused `switch menu` construct (horizontal/vertical menu + dispatch) |
| 4b | Named compare-flag variables for multi-IF flag-reuse patterns |
| 4c | Loop detection improvement: handle multi-exit loops gracefully |

### Phase 5 (Tooling)

| Step | Task |
|------|------|
| 5a | Syntax highlighting grammar (TextMate / VS Code) |
| 5b | Language server for debugger integration |
| 5c | Integrate with existing `DebuggerWindow`: show ECLH source alongside disassembly |

---

## 10. Integration with Existing Debugger

The existing `DebuggerWindow` uses `EclDisassembler.Disassemble()` to produce text lines.
After phase 1 is complete, a new mode can show ECLH source lines in the disassembly panel
by maintaining a `(ushort address → source line)` mapping generated at compile time. The
compiler can emit a `.eclh.map` sidecar file:

```json
{
  "entries": [
    { "address": 16384, "file": "module.eclh", "line": 12 },
    { "address": 16389, "file": "module.eclh", "line": 13 }
  ]
}
```

`DebuggerWindow` loads this map and, when `EclDebug.CurrentOffset` matches a mapped
address, highlights the corresponding ECLH source line. This is exactly analogous to a
debug symbol / PDB file.

---

## 11. Grammar (EBNF Summary)

```ebnf
program       ::= directive* entry_points var_block? data_block? (statement | sub_def)*

directive     ::= '@module' string_lit
               |  '@base'   hex_lit
               |  '@game'   (identifier | string_lit)   (* game profile name or 'custom' *)
               |  '@include' string_lit
               |  '@data_region' hex_lit '..' hex_lit
               |  engine_func_decl                      (* inline engine function decl *)

engine_func_decl ::= 'engine_func' identifier '=' hex_lit

(* ── Game profile file (.eclg) ───────────────────────── *)
profile       ::= ('@game_name' string_lit newline)?
                  profile_entry*

profile_entry ::= engine_func_decl newline
               |  'ecl_clock_args' '=' dec_lit newline
               |  'mem_region' identifier '=' hex_lit '..' hex_lit newline
               |  'hardware_reg' identifier '=' hex_lit newline

(* ── ECL binary header — five entry point addresses, in fixed order ──── *)
entry_points  ::= 'on_enter'            '=' identifier newline
                  'on_move'             '=' identifier newline
                  'on_search'           '=' identifier newline
                  'on_pre_camp'         '=' identifier newline
                  'on_camp_interrupted' '=' identifier newline

var_block     ::= 'var' '{' var_decl* '}'
var_decl      ::= ('word' | 'byte') identifier ('@' hex_lit)? newline

data_block    ::= 'data' '{' data_decl* '}'
data_decl     ::= ('word' | 'byte') identifier ('@' hex_lit)? '=' '{' data_value (',' data_value)* '}' newline
data_value    ::= hex_lit | dec_lit

(* ── Subroutine definition ───────────────────────────── *)
sub_def       ::= 'sub' identifier '(' ')' ('@' hex_lit)? '{' statement* '}'

statement     ::= label_def
               |  const_def
               |  if_stmt
               |  while_stmt
               |  do_while_stmt
               |  assign_stmt
               |  call_stmt
               |  computed_goto
               |  computed_call
               |  cmd_stmt
               |  menu_stmt

label_def     ::= identifier ('@' hex_lit)? ':'
const_def     ::= 'const' identifier '=' (hex_lit | dec_lit)

(* ── Conditional ─────────────────────────────────────── *)
if_stmt       ::= 'if' '(' condition ')' ( single_action | block else_clause? )
               |  'if' '(' cmp_op ')' single_action   (* flag-reuse: no COMPARE emitted *)

condition     ::= operand cmp_op operand
               |  operand '==' operand '&&' operand '==' operand   (* COMPARE AND *)

cmp_op        ::= '==' | '!=' | '<' | '>' | '<=' | '>='

single_action ::= if_stmt          (* nested single-line if *)
               |  assign_stmt
               |  call_stmt
               |  cmd_stmt

block         ::= '{' statement* '}'

else_clause   ::= 'else' ( block | if_stmt )   (* if_stmt = else-if chain *)

(* ── Loops ───────────────────────────────────────────── *)
while_stmt    ::= 'while' '(' condition ')' block

do_while_stmt ::= 'do' block 'while' '(' condition ')'

(* ── Assignment: dest = rvalue ───────────────────────── *)
assign_stmt   ::= lvalue '=' rvalue

lvalue        ::= identifier
               |  addr_ref                              (* [addr] or hardware_reg name *)
               |  '@[' addr_expr ']'                   (* string pointer destination *)
               |  identifier '[' operand ']'
               |  '&' addr_expr '[' operand ']'

rvalue        ::= arith_expr
               |  'random' '(' operand ')'
               |  identifier '[' operand ']'
               |  'input_number' '(' operand ',' operand ')'
               |  'input_string' '(' operand ',' operand ')'
               |  string_lit                           (* for string pointer assignments *)
               |  operand

addr_ref      ::= '[' addr_expr ']'                    (* code 0x01 reference *)

arith_expr    ::= operand arith_op operand
arith_op      ::= '+' | '-' | '*' | '/' | '&' | '|'

(* ── Subroutine calls ────────────────────────────────── *)
call_stmt     ::= identifier '(' ')'                (* direct GOSUB       *)
               |  'call' '(' operand ')'            (* indirect GOSUB     *)

(* ── Computed jumps and calls ────────────────────────── *)
computed_goto ::= 'goto' '(' operand ')' label_block   (* ON GOTO  *)
computed_call ::= 'call' '(' operand ')' label_block   (* ON GOSUB *)

label_block   ::= '{' (identifier newline)+ '}'

(* ── Command statements (keyword form) ───────────────── *)
cmd_stmt      ::= 'goto'    operand
               |  'return'
               |  'exit'
               |  'call'    operand                 (* CALL opcode 0x2D   *)
               |  'combat'
               |  'approach'
               |  'picture'       operand
               |  'delay'
               |  'print'         operand
               |  'printclear'    operand
               |  'print_return'
               |  'clear_box'
               |  'sprite_off'
               |  'newecl'        operand
               |  'load_files'    operand ',' operand ',' operand
               |  'load_pieces'   operand ',' operand ',' operand
               |  'load_character' operand
               |  'load_monster'   operand ',' operand ',' operand
               |  'setup_monster'  operand ',' operand ',' operand
               |  'clearmonsters'
               |  'add_npc'       operand ',' operand
               |  'who'           operand
               |  'dump'
               |  'program'       operand
               |  'protection'    operand
               |  'damage'        operand ',' operand ',' operand ',' operand ',' operand
               |  'partystrength' operand
               |  'party_surprise' operand ',' operand
               |  'surprise'      operand ',' operand ',' operand ',' operand
               |  'checkparty'    operand ',' operand ',' operand ',' operand ',' operand ',' operand
               |  'find_item'     operand
               |  'find_special'  operand
               |  'destroy_items' operand
               |  'ecl_clock'     operand
               |  'spell'         operand ',' operand ',' operand
               |  'rob'           operand ',' operand ',' operand
               |  'treasure'      operand ',' operand ',' operand ',' operand ','
                                  operand ',' operand ',' operand ',' operand
               |  'parlay'        operand ',' operand ',' operand ','
                                  operand ',' operand ',' operand
               |  'encounter_menu' encounter_args

(* ── Menu statements ─────────────────────────────────── *)
menu_stmt     ::= identifier '=' 'vertical_menu'  '(' operand ')' menu_block
               |  identifier '=' 'horizontal_menu'               menu_block

menu_block    ::= '{' (string_lit newline)+ '}'

(* ── Operand leaf ────────────────────────────────────── *)
operand       ::= '#'  (hex_lit | dec_lit | identifier)
               |  '##' (hex_lit | dec_lit | identifier)
               |  '[' addr_expr ']'
               |  '&' addr_expr
               |  string_lit
               |  '@[' addr_expr ']'
               |  identifier
               |  dec_lit
               |  hex_lit

addr_expr     ::= hex_lit | dec_lit | identifier
hex_lit       ::= '0x' [0-9a-fA-F]+
dec_lit       ::= [0-9]+
string_lit    ::= '"' [^"]* '"'
identifier    ::= [a-zA-Z_] [a-zA-Z0-9_]*
```

---

## 13. Findings from Real Binary Analysis (Pool of Radiance Town ECL)

Running the decompiler against the Pool of Radiance town ECL (7473 bytes, base `0x9900`)
produced 734 instructions leaving 177 non-code bytes (data tables). The following findings
update or extend the spec.

### 13.1 Compare Flag Reuse — A Real Pattern

The spec states "every IF opcode is always immediately preceded by its own COMPARE." The
real binary violates this in 7 cases. The pattern is:

```
$A593:  compare player_6E81 vs #8
$A599:  [IF >=]
$A59A:  printclear "IT'S GREEN..."  // side effect, does NOT touch compare flags
$A5BE:  [IF >=]                     // reuses flag from $A593 compare
$A5BF:  ##4A1A = ##4A0D + ##4A1A
$A5C9:  [IF >=]                     // reuses flag again
$A5CA:  goto loc_A609
```

One COMPARE sets the flags, then multiple IFs fire against the same result, with
non-compare instructions between them. This is valid because instructions like PRINTCLEAR,
ASSIGN, RANDOM etc. do not modify the compare flags — only COMPARE and COMPARE AND do.

**Spec correction:** The absolute invariant "every IF has its own COMPARE" was wrong for
the compiler targeting existing code patterns. The correct rule is:

- **Compiler (generating new code):** always emit a fresh COMPARE before each IF. This is
  safe and simple.
- **Decompiler (reading existing code):** an IF whose immediately preceding instruction is
  not a COMPARE is a **flag-reuse IF**. These cannot be fused into a single `if(cond)`
  statement because there is no visible condition at that point. Emit them as a raw
  `[IF op]` annotation with a comment, or introduce a named flag variable.

**ECLH syntax for flag-reuse IFs:**

```eclh
compare player_6E81 vs #8       // sets the flag
if (>=) printclear "IT'S GREEN..."
if (>=) gold = gold + wager     // reuses the flag — no re-compare
if (>=) goto loc_A609
```

The bare `if (op)` form without a left-hand/right-hand operand means "test the current
compare flag" and compiles to just `IF<op>` with no preceding COMPARE. This is only valid
when the programmer explicitly knows the flag is still set from a prior compare.

### 13.2 Multiple IFs After One COMPARE — Six Cases

Related to 13.1, six COMPARE instructions each generate more than one IF downstream:

| Compare address | IF count | Gap between IFs |
|-----------------|----------|-----------------|
| `$9A14`         | 2        | intervening assign, string write |
| `$9BE3`         | 2        | intervening assign |
| `$9BF4`         | 2        | intervening goto |
| `$A3B1`         | 2        | intervening printclear |
| `$A593`         | 3        | intervening printclear, assign |
| `$A5CE`         | 2        | intervening printclear |

All six cases involve instructions that do not touch compare flags (PRINTCLEAR, ASSIGN,
GOTO). The decompiler must track flag-liveness to correctly identify these.

### 13.3 Code 0x02 Operand — One Occurrence Found, Context Identified

Only one `0x02` operand appears in the entire town ECL, at `$A576`:

```eclh
$A574:  compare ##4A0D vs ?[$03E8]
```

`0x03E8` = 1000 — the house betting limit in the gambling sequence. The `##4A0D` side is
the player's bet (a word immediate in the area region). The `?[$03E8]` side uses code
`0x02`, which is parsed as a 3-byte operand like `0x01` but excluded from ECL address
tracking. This may be a constant embedded at a fixed game-engine address rather than in
the ECL address space. Further investigation across other ECL blocks is needed.

### 13.4 Word Immediates (##addr) as Variable References — Confirmed

All 19 `##addr` occurrences are concentrated in the gambling subroutine (`$A513–$A62F`)
and use exactly two addresses: `##4A0D` (player's bet, in `area` region) and `##4A1A`
(player's gold, in `area` region). They are used interchangeably with `[addr]` syntax —
for example:

```eclh
$A513:  ##4A1A = ##6BC3        // copy gold from player region to area region
$A524:  print ##4A1A           // print gold value
$A56A:  compare ##4A1A vs ##4A0D   // compare gold vs bet
$A5BF:  ##4A1A = ##4A0D + ##4A1A   // gold += bet
```

These are identical in meaning to `area_4A1A = player_6BC3` etc. — the `##` form is just
the ECL binary's choice of encoding for that instruction. The decompiler must name `##addr`
operands in mem regions identically to `[addr]` operands at the same address, and emit
them using the same variable name. The `##` vs `[` distinction is preserved in the `var`
block via a type annotation if needed for round-trip; in normal decompilation they are
displayed identically.

### 13.5 Unclassified Address Operands — 12 Remaining After Classification

After applying mem_region and hardware_reg classification, only 12 distinct addresses
remain unresolved:

| Address | Uses | Classification |
|---------|------|----------------|
| `$985E` | 11   | String buffer — IS in `shared` region (0x9700–0x98FF); named `shared_985E` via StringPtr classification |
| `$9890` | 4    | String buffer — IS in `shared` region; named `shared_9890` |
| `$C04B` | 5    | `map_x` hardware register |
| `$C04C` | 4    | `map_y` hardware register |
| `$C04D` | 5    | `map_direction` hardware register |
| `$C04E` | 4    | `map_wall_type` hardware register |
| `$C04F` | 2    | `map_wall_roof` hardware register |
| `$6B00` | 1    | Player name string pointer (base of player region, used with `@[$6B00]` = player name) |
| `$00B8` | 1    | Engine internal `word_1EE78` — used in `on_enter` init |
| `$00B9` | 1    | Engine internal `word_1EE7A` — used in `on_enter` init |
| `$03DE` | 1    | Engine internal `word_1EE76` |
| `$03E8` | 1    | Unknown constant (code `0x02` operand — house betting limit?) |

After fixing the StringPtr naming pass, only 5 addresses are truly unclassified:
`$6B00`, `$00B8`, `$00B9`, `$03DE`, and `$03E8`. The first four are engine internal
registers that could be added to the game profile. The `$03E8` code-02 operand remains
under investigation.

### 13.6 Horizontal Menu Always Followed by ON GOTO — Confirmed Pattern

Every `horizontal_menu` in the town ECL is immediately followed by a `goto(var) { ... }`
dispatching on the same result variable. Confirmed example:

```eclh
$99D4:  sub_AE5D()                          // display menu helper
$99D8:  goto(player_6E79) { loc_AD85, loc_99E4 }  // dispatch on result
```

And from the gambling section:
```eclh
$A564:  input_number #4, ##4A0D             // get bet amount
$A56A:  compare ##4A1A vs ##4A0D            // check if player can afford it
```

The pattern is consistent enough to support a fused construct. Similarly, the main
28-way dispatch at `$9B4C` dispatches directly on `shared_9800` after it is set by the
loop above. The fused `switch` form would be:

```eclh
switch horizontal_menu("FIGHT", "FLEE") -> player_6E79 {
    goto loc_fight    // option 0
    goto loc_flee     // option 1
}
```

**This remains a phase-4 construct** — the decompiler emits the unfused two-instruction
form for now.

### 13.7 Large ON GOTO Dispatch Table

The main location dispatcher at `$9B4C` is a 28-way ON GOTO:

```eclh
$9B4C: goto(shared_9800) { loc_AE6D, loc_9BA6, loc_9C43, loc_9DD5, ... }
```

This is the primary dispatch for the 28 locations in the town. `loc_AE6D` appears
repeatedly as a "no-op / fall through" target. This is valid and already handled by the
`goto(index) { ... }` syntax. No spec change needed.

### 13.8 Loop Patterns Confirmed — 8 Backward GOTOs Found

The decompiler found 8 backward GOTOs. The clearest loop is the location iterator:

```eclh
loc_9A3D:                          // while top
    compare shared_9800 vs player_6E82
    [IF ==]                        // exit condition 1: found match
    goto loc_9B4C
    compare shared_9800 vs #27
    [IF >]                         // exit condition 2: past end
    exit
    shared_9800 = #1 + shared_9800 // increment
    goto loc_9A3D                  // back edge
```

This is a `while` loop with two exit conditions in the body — it cannot be cleanly
expressed as `while (cond) { }` because neither condition is at the top alone. The
decompiler emits it as labelled code with a comment.

A cleaner `do/while` candidate from sub `$AF5C`:
```eclh
loc_AF5C:                          // do top
    load_character shared_9802
    compare player_6C00 vs #127
    [IF >]
    goto loc_AF77
    ...
loc_AF77:
    shared_9802 = #1 + shared_9802
    compare shared_9802 vs #8
    [IF <]
    goto loc_AF5C                  // back edge with fresh compare → do/while candidate
```

**Decompiler loop lifting rule:** only lift to `while`/`do/while` when:
- Exactly one back-edge GOTO exists in the loop
- For `while`: the first instructions at the loop label form a COMPARE+IF+GOTO_out pattern
- For `do/while`: the last instructions before the back-edge form a COMPARE+IF+GOTO_back pattern
- No other GOTO from outside the loop targets any address inside the loop body

Any loop failing these criteria is emitted as plain labelled code.

### 13.9 String Pointer Assignments — 13 Confirmed Cases

All 13 string assignments use `@[addr] = "string"` form, writing to two shared string
buffers (`shared_985E` and `shared_9890`):

```eclh
$9A1B:  @[$9890] = "MAD MAN"       // write name to shared string buffer
$ABA0:  @[$985E] = "LIX."         // Roman numeral strings for location names
$ABA9:  @[$9890] = "SKULLCRUSHER"
```

After the naming pass correctly handles `StringPtr` operands (kind `0x81`) in mem regions,
these become:

```eclh
$9A1B:  shared_9890 = "MAD MAN"
$ABA0:  shared_985E = "LIX."
```

The decompiler emits this form: `dest = "string"` where dest is the named variable. The
compiler recognises a string RHS with a mem_region variable LHS and emits SAVE with a
`0x80` source operand. No new syntax needed — it is already valid ECLH assignment.

### 13.11 Engine Parameter Registers — Write-Before-Call Pattern

Three unclassified addresses appear as write-before-call sequences:

```eclh
$B1B9:  [$03DE] = #8       // set sound parameter
$B1BF:  play_sound()       // then call engine function
```

`$03DE` is `word_1EE76` from `vm_SetMemoryValue` case 4. In `CMD_Call`, when
`gbl.word_1EE76 == 8` it plays `Sound.sound_a`, when `== 10` plays `Sound.sound_b`.
This is a **parameter register** — a fixed engine address written immediately before an
engine function call to pass arguments.

Similarly `$00B8` (`word_1EE78`) and `$00B9` (`word_1EE7A`) are written as a pair during
`on_enter` initialisation (`#72` = `0x48`, `#189` = `0xBD`). Their combined value
`0xBD48` suggests they may form a 16-bit address or colour value used internally.

These should be added to the game profile:

```eclg
hardware_reg sound_param   = 0x03DE   // written before play_sound() — 8=sound_a, 10=sound_b
hardware_reg engine_b8     = 0x00B8   // internal engine state (word_1EE78)
hardware_reg engine_b9     = 0x00B9   // internal engine state (word_1EE7A)
```

### 13.12 Wrapper Subroutine Pattern

The two most-called subroutines are trivial wrappers:

```eclh
sub_AF1F @ 0xAF1F:                           // called 25 times
    horizontal_menu shared_9801, #1, "PRESS <RETURN> OR BUTTON TO CONTINUE"
    return

sub_AE5D @ 0xAE5D:                           // called 10 times
    horizontal_menu player_6E79, #2, "YES", "NO"
    return
```

These are one-instruction subroutines used purely to avoid repeating a long instruction.
In ECLH source they appear as `sub_AF1F()` and `sub_AE5D()` — indistinguishable from
complex subroutines at the call site, which is exactly right. The decompiler correctly
identifies them as GOSUB targets and emits them as `sub` definitions.

This pattern also suggests that the fused `switch menu` construct (§13.6) would make
`sub_AE5D` unnecessary — the caller would just write:

```eclh
switch horizontal_menu("YES", "NO") -> player_6E79 {
    goto yes_handler
    goto no_handler
}
```

### 13.13 Flag-Reuse IF Across GOTO — Confirmed Edge Case

The flag-reuse IF at `$9BFF` is the most unusual case — the immediately preceding
instruction is a GOTO, not a compare or data instruction:

```eclh
$9BF4:  compare area_4AC4 vs #2
$9BFA:  [IF <]
$9BFB:  goto loc_9C2E          // ← preceding instruction is GOTO
$9BFF:  [IF >]                 // ← flag-reuse IF
$9C00:  goto loc_9C19
```

This is a three-way branch on a single compare: `< 2` goes to `loc_9C2E`, `> 2` goes to
`loc_9C19`, and implicitly `== 2` falls through. The GOTO at `$9BFB` is reachable from
the CFG (it's the action of the first IF), but `$9BFF` is only reachable when the first
IF was false (i.e. `area_4AC4 >= 2`). The compare flags from `$9BF4` are still valid
because no instruction between `$9BFA` and `$9BFF` modifies them — the GOTO at `$9BFB`
only executes when the first IF fires, not when it falls through.

In ECLH source this emits as:

```eclh
compare area_4AC4 vs #2
if (<) goto loc_9C2E     // normal form
if (>) goto loc_9C19     // flag-reuse form — no re-compare
                         // implicit fall-through: area_4AC4 == 2
```

The decompiler correctly identifies `$9BFF` as a flag-reuse IF because its immediately
preceding *decoded* instruction (`$9BFB`) is not a COMPARE.

| # | Finding | Confirmed | Spec change |
|---|---------|-----------|-------------|
| 13.1 | Flag-reuse IFs — 7 cases | ✓ | Add bare `if (op)` syntax; decompiler emits it when IF not preceded by COMPARE |
| 13.2 | Multiple IFs per COMPARE — 6 cases | ✓ | Documented; decompiler tracks flag liveness |
| 13.3 | Code `0x02` — 1 occurrence at `$A576` | ✓ | No new syntax; preserve verbatim as `?[$addr]` |
| 13.4 | `##addr` as variable ref — 19 cases, all in gambling sub | ✓ | Naming pass treats `WordImm` in mem_region same as `WordRef` |
| 13.5 | Unclassified addrs — 5 truly unknown after hw/string fix | ✓ | Add engine internal regs to profile; `$03E8` under investigation |
| 13.6 | Menu always followed by dispatch | ✓ | Phase-4 `switch menu` construct; unfused for now |
| 13.7 | 28-way ON GOTO dispatch | ✓ | Already supported by `goto(index) { ... }` syntax |
| 13.8 | Loops with multiple exits — not clean while/do-while | ✓ | Decompiler only lifts unambiguous single-exit loops |
| 13.9 | String pointer assignments — 13 cases | ✓ | `var = "string"` form; fix StringPtr naming in decompiler |

**Why a separate `.eclg` game profile file rather than inline declarations?** The engine
function addresses are not part of the ECL binary — they are addresses in the game engine
executable that the ECL code calls into. They belong to the game, not to any individual
ECL module. Putting them in a shared profile file means every ECL source file for a given
game can use the same named functions without repeating the addresses, and a single profile
update covers all files if an address is corrected. The inline `engine_func` declaration in
source is provided for reverse engineering convenience — when you discover a new function,
you can name it in one file immediately without waiting to update the profile.

**Why `name()` for both GOSUB subroutines and engine functions?** From the ECL programmer's
perspective, both are "call this thing and come back." The distinction between GOSUB (ECL
call stack) and CALL (engine dispatch) is an implementation detail. The compiler resolves
the name: if it's a `sub`, emit GOSUB; if it's an `engine_func`, emit CALL. If the
programmer needs to call an address that is neither, they use the explicit `call ##addr`
form, which signals "I know what I'm doing here."

**`ecl_clock_args` in the profile.** The `ECL_CLOCK` instruction takes 1 argument in Pool
of Radiance and 2 in later games, matching `gbl.game.EclClockArguments` in the runtime.
This is not a new opcode — the same byte `0x34` is used — but the number of operands
differs, which changes the instruction's byte length and therefore the addresses of
everything that follows. The profile must declare this so the compiler emits the right
number of operands and the decompiler reads the right number of bytes. The five
opening GOTO instructions are never reached by the normal VM execution loop — they exist
solely so `vm_init_ecl` can read the five entry-point addresses before execution starts.
Treating them as ordinary code would mean the decompiler emits five bare `goto` statements
at the top of the file with no indication of their special role. Named `on_*` declarations
make the contract explicit, match what the engine actually does with each slot, and allow
the compiler to verify all five are provided and placed first.

**The header affects all label addresses.** Because the 20-byte GOTO preamble sits before
all user code, every instruction address in the binary is `@base + 20 + offset_within_code`.
The compiler must account for this when resolving label addresses, and the decompiler must
skip these five GOTOs before beginning normal instruction decode. Getting this wrong would
shift every single address in the binary by 20 bytes. The difference is fundamental: `var`
entries are addresses in the game's runtime memory that the VM reads and writes during
execution. `data` entries are bytes baked into the ECL binary itself at compile time —
they are read-only from the program's perspective and live at addresses within the ECL
buffer. Mixing them into a single block would obscure which addresses are in the ECL image
versus which are in working memory, making both the source harder to read and the compiler
harder to implement correctly. The `data` block also carries element-type information
(`byte` vs `word`) and initialiser values, neither of which apply to runtime variables.

**Why not `@data_region` for tables?** The existing `@data_region` directive reserves a
range of addresses for the compiler to assign runtime variables in phase 2. Inline data
tables are a completely different thing — they are part of the compiled binary output, not
variable storage. Named `data` declarations are far more readable and give the compiler
everything it needs to emit the raw bytes at the right position.

**Code `0x01` is the universal ECL address operand.** Verified from real binary data: write
destinations, read sources, jump targets, table bases, and engine callback addresses all use
code `0x01`. There is no separate write-only operand code in actual ECL binaries. The `0x02`
operand code is parsed by `vm_LoadCmdSets` identically to `0x01` (reads 3 bytes) but is
explicitly excluded from ECL address tracking in `GetReferencedAddresses`. Its precise
semantics remain under investigation; in phase-1 mode it is preserved verbatim.

**`initial_ecl_offset` is a game constant, not from the DAX header.** The 2-byte DAX
prefix is DAX format metadata (possibly block size or checksum). For Pool of Radiance,
`initial_ecl_offset = 0x8000`, confirmed by the town ECL entry point addresses
(0x9914, 0x99EB, etc.) all being within range when subtracted from 0x8000. `do_work()` is immediately readable as "call this
subroutine" in any programming language. `gosub do_work` is assembly notation that carries
no additional information and requires the reader to know what GOSUB means. Since ECL's
GOSUB/RETURN pair maps cleanly to a call/return pattern, there is no reason not to use the
familiar syntax. The `sub name() { … }` definition form makes subroutine boundaries
explicit in the source, which also helps the decompiler produce well-structured output.

The one disambiguation needed is between GOSUB (subroutine call) and the ECL `CALL` opcode
(0x2D), which invokes a hard-coded engine callback address. These are different mechanisms:
GOSUB pushes a return address on the VM call stack; `CALL` does not. ECLH uses `name()` for
GOSUB and `call address` for the engine callback opcode, keeping them visually distinct.

**`while` vs `do/while` — the one asymmetry in the language.** Every other block construct
negates the condition so the GOTO fires on failure. `do/while` is the single exception: the
GOTO at the bottom fires when the condition is **true** (to loop back), and the loop exits
by falling through when false. This is not an inconsistency — it is the natural binary
expression of "repeat until false". The decompiler identifies `do/while` specifically by
detecting a back-edge GOTO whose target is `_loop` and whose controlling IF uses the
**non-negated** operator. Any back-edge GOTO controlled by a negated IF would instead
indicate a `while` loop with an unusual structure and should be emitted as plain GOTOs.

**How block `if` and `if/else` are compiled without a block instruction.** All conditional
forms use one consistent pattern: `COMPARE + IF<negated-op> + GOTO _next + block + GOTO _after`.
The condition is negated so the GOTO fires when the condition is false, skipping the block;
when true the GOTO is skipped and execution falls into the block. Each `else if` branch is
simply another `COMPARE + IF<negated> + GOTO _next` triple starting at the label the
previous branch's GOTO lands on. The `else` block requires no COMPARE, IF, or leading GOTO
— it is just the code that all failed branches fall through to. All branches share one
`_after` label. This is uniform, has no special cases, supports arbitrary nesting, and
requires no subroutines. The invariant is absolute: every IF opcode is always immediately
preceded by its own COMPARE — no IF ever relies on flags set by a previous instruction.

**Why expression syntax for arithmetic and assignment?** Writing `counter = counter + 1`
is unambiguous and universally understood. The raw ECL form `ADD [counter], #1, &counter`
forces the programmer to track which operand is which, remember that the destination is
always last, and deal with the `&` write-address convention. None of that is meaningful at
the source level — it's all encoding detail. The compiler knows the rules and can apply
them mechanically. The decompiler knows them in reverse.

**The SUBTRACT operand-order trap.** ECL's SUBTRACT encodes as `op2 − op1 → op3`, not
`op1 − op2 → op3`. This is almost certainly a historical artefact of how the VM was
implemented. If ECLH exposed this as `SUBTRACT a, b, dest` it would be a constant source
of bugs. Instead ECLH requires `dest = lhs - rhs` and the compiler silently emits `op1=rhs,
op2=lhs` to produce the correct result. The decompiler similarly swaps operands back when
producing ECLH source, so the programmer never encounters the raw order.

**Why separate `#` and `##` sigils?** The ECL encoding has two distinct binary representations
for integers: a 2-byte form (`0x00 N`, 8-bit value, range 0–255) and a 3-byte little-endian
form (`0x03 lo hi`, 16-bit value, range 0–65535). These are not interchangeable — choosing
the wrong one changes the byte length of the instruction, which shifts every subsequent
instruction's address and corrupts all labels. The sigils make the encoding explicit: `#5`
is always `0x00 0x05` (2 bytes); `##5` is always `0x03 0x05 0x00` (3 bytes). The decompiler
records which the original used; the compiler reproduces it faithfully. In phase-2 code
written by hand, bare integer literals without sigils are allowed and the compiler picks the
smallest correct encoding.

**Why pinned labels (`@ address`)?** In phase 1 the decompiler pins every label to its source
address. The compiler then verifies its layout matches. Any divergence is a bug, caught
immediately. In phase 2, labels are unpinned and the compiler is free to lay out code as it
sees fit.

**Why a flat `var {}` block rather than scoped locals?** ECL has no stack frame or local
variable concept — every address is global. A flat var block models this truthfully. Phase-2
"local" variables are simply compiler-assigned addresses that happen not to collide with
anything else; the language doesn't need to special-case them.
