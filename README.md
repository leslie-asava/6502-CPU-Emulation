# 6502 CPU Emulation (Work in Progress)

This is my attempt at implementing an emulator for the 6502 microprocessor in C#

## 1. Architecture

- **Data Bus:** 8-bit (1 byte), meaning it can handle 8 bits of data at a time.
- **Address Bus:** 16-bit, allowing it to address up to 64 KB (65,536 bytes) of memory.
- **Registers:**
  - **Accumulator (A):** 8-bit register used for arithmetic and logic operations.
  - **Index Register X (X):** 8-bit register used primarily for indexed addressing modes.
  - **Index Register Y (Y):** 8-bit register used for indexed addressing modes.
  - **Stack Pointer (SP):** 8-bit register pointing to the top of the stack in memory.
  - **Program Counter (PC):** 16-bit register holding the address of the next instruction to be executed.
  - **Status Register (P):** 8-bit register containing status flags.

## 2. Instruction Set

The 6502 has a rich instruction set, with instructions falling into several categories:
- **Load and Store Instructions:** Move data between registers and memory (e.g., `LDA`, `STA`, `LDX`, `STX`).
- **Arithmetic Instructions:** Perform mathematical operations (e.g., `ADC`, `SBC`, `INC`, `DEC`).
- **Logic Instructions:** Perform bitwise operations (e.g., `AND`, `OR`, `EOR`, `BIT`).
- **Branch Instructions:** Control the flow of execution (e.g., `BEQ`, `BNE`, `BCC`, `BCS`, `JMP`, `JSR`, `RTS`).
- **Stack Instructions:** Interact with the stack (e.g., `PHA`, `PLA`, `PHP`, `PLP`).
- **Transfer Instructions:** Move data between registers (e.g., `TAX`, `TXA`, `TAY`, `TYA`).
- **Control Instructions:** Manage the processor state (e.g., `NOP`, `BRK`, `RTI`).

## 3. Addressing Modes

The 6502 supports several addressing modes for accessing memory:
- **Immediate:** Directly specifies the value to be used (e.g., `LDA #$FF`).
- **Zero Page:** Accesses data from the first 256 bytes of memory (e.g., `LDA $00`).
- **Absolute:** Uses a 16-bit address to access memory (e.g., `LDA $1234`).
- **Indexed (X and Y):** Adds the value of the X or Y register to the base address (e.g., `LDA $00,X`).
- **Indirect:** Uses a memory location to point to another address (e.g., `JMP ($FF00)`).
- **Indexed Indirect:** Uses a base address with the X register for indexing (e.g., `LDA ($00,X)`).
- **Indirect Indexed:** Uses a base address and adds the Y register value (e.g., `LDA ($00),Y`).

## 4. Clock Speed and Timing

- The 6502 operates at various clock speeds, typically ranging from 1 MHz to 2 MHz.
- Each instruction takes a variable number of clock cycles to execute. Timing can vary based on the addressing mode and whether memory access crosses a page boundary.

## 5. Status Register

The Status Register (P) contains flags that reflect the results of operations:
- **Carry Flag (C):** Indicates an overflow from the most significant bit.
- **Zero Flag (Z):** Set if the result of an operation is zero.
- **Interrupt Disable Flag (I):** Controls the ability to respond to interrupts.
- **Decimal Mode Flag (D):** Used for binary-coded decimal (BCD) operations.
- **Break Flag (B):** Used to indicate a software interrupt.
- **Overflow Flag (V):** Indicates an overflow in signed arithmetic operations.
- **Negative Flag (N):** Set if the result of an operation has the highest bit set.

## 6. Interrupts

The 6502 supports several types of interrupts:
- **NMI (Non-Maskable Interrupt):** A high-priority interrupt that cannot be disabled.
- **IRQ (Interrupt Request):** A maskable interrupt that can be disabled or enabled via the Status Register.
