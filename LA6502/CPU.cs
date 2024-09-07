using LA6502.Types;

using Word = ushort;
using uint32 = uint;
using int32 = int;

namespace LA6502.CPU
{
    // Emulate 6502 CPU registers and functioning
    public struct CPU
    {
        public Word PC;            // 16-bit Program Counter register
        public byte SP;            // 8-bit Stack Pointer register
        public byte A;             // 8-bit Accumulator register
        public byte X;             // 8-bit Index register X
        public byte Y;             // 8-bit Index register Y
        public byte StatusFlags;   // 8-bit Processor Status register

        // Reset the CPU e.g when powered on
        public void Reset(Memory memory)
        {
            PC = 0xFFFC;
            SP = 0xFF;
            A = 0x00;
            X = 0x00;
            Y = 0x00;
            StatusFlags = 0x00;

            memory.Initialize();
        }

        // Request an aditional clock cycle e.g., when the Page Boundary is crossed
        public void RequestCycle(ref uint32 cycles)
        {
            cycles++;
        }

        // Use clock cycle
        public void CycleTick(ref uint32 cycles)
        {
            cycles--;
        }

        // Check if a Processor Status flag is set
        public bool IsFlagSet(byte StatusFlags, ProcessorFlags bitPosition)
        {
            return (StatusFlags & 1 << (int)bitPosition) != 0;
        }

        // Check if a bit in a register is set
        public bool IsBitSet(byte Register, int bitPosition)
        {
            return (StatusFlags & 1 << bitPosition) != 0;
        }

        // Set a Processor Status flag
        public void SetFlag(byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags |= (byte)(1 << (int)bitPosition);
        }

        // Clear a Processor Status flag
        public void ClearFlag(byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags &= (byte)~(1 << (int)bitPosition);
        }

        // Toggle a Processor Status flag
        public void ToggleFlag(byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags ^= (byte)(1 << (int)bitPosition);
        }

        // Set Processor Status flags that are common for all addressing modes of an Instructions
        void SetStatus(Instructions instruction)
        {
            switch (instruction)
            {
                case Instructions.LDA:
                    {
                        
                    }
                    break;
            }
        }

        // Set the Zero and Negative flags appropriately
        void SetZeroAndNegativeFlags(Byte register)
        {
            if (register == 0)
            {
                SetFlag(StatusFlags, ProcessorFlags.Z);
            }

            if (IsBitSet(register, 7))
            {
                SetFlag(StatusFlags, ProcessorFlags.N);
            }
        }

        // Fetch the next Byte at the Program Counter
        public byte FetchByte(ref uint32 cycles, Memory memory)
        {
            byte Data = memory[PC];
            PC++;

            CycleTick(ref cycles);

            return Data;
        }

        // Fetch the next two Bytes at the Program Counter
        public Word FetchWord(ref uint32 cycles, Memory memory)
        {
            byte lowByte = memory[PC];
            PC++;
            CycleTick(ref cycles);

            byte highByte = memory[PC];
            PC++;
            CycleTick(ref cycles);

            Word Data = (Word)(highByte << 8 | lowByte);
            return Data;
        }

        // Read a Byte from a memory adress, does not advance the Program Counter
        public byte ReadByte(ref uint32 cycles, Memory memory, Word Address)
        {
            byte Data = memory[Address];

            CycleTick(ref cycles);

            return Data;
        }

        // Read the next two Bytes from a memory address, does not advance the Program Counter
        public Word ReadWord(ref uint32 cycles, Memory memory, Word address)
        {
            byte lowByte = memory[address];
            CycleTick(ref cycles);

            address++;

            byte highByte = memory[address];
            CycleTick(ref cycles);


            Word Data = (Word)(highByte << 8 | lowByte);
            return Data;
        }

        // Write a Byte to a memory address
        public void WriteByte(ref uint32 cycles, Memory memory, Word Address, byte Data)
        {
            memory[Address] = Data;

            CycleTick(ref cycles);
        }

        // Push a byte onto the Stack and decrement the Stack Pointer
        public void PushToStack(ref uint32 cycles, Memory memory, byte Data)
        {
            WriteByte(ref cycles, memory, (Word)(0x0100 + SP), Data);
            SP--;
            CycleTick(ref cycles);
        }

        // Pop a byte from the Stack and increment the Stack Pointer
        public Byte PopFromStack(ref uint32 cycles, Memory memory)
        {
            Byte data = ReadByte(ref cycles, memory, (Word)(0x0100 + SP));
            SP++;
            CycleTick(ref cycles);

            return data;
        }

        // Zero Page Adressing Mode
        public byte AddressZeroPage(ref uint32 cycles, Memory memory)
        {
            byte zeroPageAddress = FetchByte(ref cycles, memory);
            return zeroPageAddress;
        }

        // X Indexed,Zero Page Addressing Mode
        public byte AddressZeroPageX(ref uint32 cycles, Memory memory)
        {
            byte zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + X);

            CycleTick(ref cycles);

            return zeroPageAddress;
        }

        // Y Indexed,Zero Page Addressing Mode
        public byte AddressZeroPageY(ref uint32 cycles, Memory memory)
        {
            byte zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + Y);

            CycleTick(ref cycles);

            return zeroPageAddress;
        }

        // Absolute Addressing Mode
        public Word AddressAbsolute(ref uint32 cycles, Memory memory)
        {
            Word address = FetchWord(ref cycles, memory);
            return address;
        }

        // X Indexed, Absolute Addressing Mode
        public Word AddressAbsoluteX(ref uint32 cycles, Memory memory, bool variableCycles = false)
        {
            Word address = FetchWord(ref cycles, memory);
            Word originalHighByte = (Word)(address & 0xFF00);
            address = (Word)(address + X);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                RequestCycle(ref cycles);

                // Decrement cycles if a page boundary was crossed
                CycleTick(ref cycles);
            }

            // For instructions whose clock cycles don't vary
            if (!variableCycles)
            {
                CycleTick(ref cycles);
            }

            return address;
        }

        // Y Indexed, Absolute Addressing Mode
        public Word AddressAbsoluteY(ref uint32 cycles, Memory memory, bool variableCycles = false)
        {
            Word address = FetchWord(ref cycles, memory);
            Word originalHighByte = (Word)(address & 0xFF00);
            address = (Word)(address + Y);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                RequestCycle(ref cycles);

                // Decrement cycles if a page boundary was crossed
                CycleTick(ref cycles);
            }

            if (!variableCycles)
            {
                CycleTick(ref cycles);
            }

            return address;
        }

        // Indirect Addressing Mode
        public Word AddressIndirect(ref uint32 cycles, Memory memory)
        {
            // The address in the instruction points to another address
            Word pointerAddress = AddressAbsolute(ref cycles, memory);

            // The absolute address we've gotten point to yet another address that holds the target data
            Word address = ReadWord(ref cycles, memory, pointerAddress);

            return address;
        }

        // Indexed Indirect, X Addressing Mode
        public Word AddressIndirectX(ref uint32 cycles, Memory memory)
        {
            // The address in the instruction points to another address
            Word zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + X);

            CycleTick(ref cycles);

            // The absolute address we've gotten point to yet another address that holds the target data
            Word address = ReadWord(ref cycles, memory, zeroPageAddress);

            return address;
        }

        // Indirect Indexed,Y Addressing Mode
        public Word AddressIndirectY(ref uint32 cycles, Memory memory, bool variableCycles = false)
        {
            // The address in the instruction points to another address
            Word zeroPageAddress = FetchByte(ref cycles, memory);
            Word address = ReadWord(ref cycles, memory, zeroPageAddress);

            Word originalHighByte = (Word)(address & 0xFF00);
            address = (Word)(address + Y);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                RequestCycle(ref cycles);

                // Decrement cycles if a page boundary was crossed
                CycleTick(ref cycles);
            }

            if (!variableCycles)
            {
                CycleTick(ref cycles);
            }

            return address;
        }

        // Relative Addressing Mode
        public Word AddressRelative(ref uint32 cycles, Memory memory)
        {
            Byte offset = FetchByte(ref cycles, memory);

            var e = (sbyte)offset;
            Word address = (Word)(PC + (sbyte)offset);

            CycleTick(ref cycles);

            return address;
        }

        // Execute instructions in memory for a number of clock cycles
        public void Execute(uint32 cycles, Memory memory)
        {
            while (cycles > 0)
            {
                byte instruction = FetchByte(ref cycles, memory);

                switch (instruction)
                {
                    // Load Accumulator Register
                    case (byte)Opcodes.LDA_IM:
                        {
                            byte operand = FetchByte(ref cycles, memory);
                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_ZP:
                        {
                            byte address = AddressZeroPage(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_ZP_X:
                        {

                            byte address = AddressZeroPageX(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_ABS_X:
                        {
                            Word address = AddressAbsoluteX(ref cycles, memory, variableCycles: true);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_ABS_Y:
                        {
                            Word address = AddressAbsoluteY(ref cycles, memory, variableCycles: true);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_IND_X:
                        {
                            Word address = AddressIndirectX(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_IND_Y:
                        {
                            Word address = AddressIndirectY(ref cycles, memory, variableCycles: true);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    // Load X Register
                    case (byte)Opcodes.LDX_IM:
                        {
                            byte operand = FetchByte(ref cycles, memory);
                            X = operand;

                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    case (byte)Opcodes.LDX_ZP:
                        {
                            byte address = AddressZeroPage(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            X = operand;

                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    case (byte)Opcodes.LDX_ZP_Y:
                        {

                            byte address = AddressZeroPageY(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            X = operand;

                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    case (byte)Opcodes.LDX_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            X = operand;

                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    case (byte)Opcodes.LDX_ABS_Y:
                        {
                            Word address = AddressAbsoluteY(ref cycles, memory, variableCycles: true);
                            byte operand = ReadByte(ref cycles, memory, address);

                            X = operand;

                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    // Load Y Register
                    case (byte)Opcodes.LDY_IM:
                        {
                            byte operand = FetchByte(ref cycles, memory);
                            Y = operand;

                            SetZeroAndNegativeFlags(Y);
                        }
                        break;

                    case (byte)Opcodes.LDY_ZP:
                        {
                            byte address = AddressZeroPage(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            Y = operand;

                            SetZeroAndNegativeFlags(Y);
                        }
                        break;

                    case (byte)Opcodes.LDY_ZP_X:
                        {

                            byte address = AddressZeroPageX(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            Y = operand;

                            SetZeroAndNegativeFlags(Y);
                        }
                        break;

                    case (byte)Opcodes.LDY_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            Y = operand;

                            SetZeroAndNegativeFlags(Y);
                        }
                        break;

                    case (byte)Opcodes.LDY_ABS_X:
                        {
                            Word address = AddressAbsoluteX(ref cycles, memory, variableCycles: true);
                            byte operand = ReadByte(ref cycles, memory, address);

                            Y = operand;

                            SetZeroAndNegativeFlags(Y);
                        }
                        break;

                    // Store Accumulator Register
                    case (byte)Opcodes.STA_ZP:
                        {
                            byte address = AddressZeroPage(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STA_ZP_X:
                        {

                            byte address = AddressZeroPageX(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STA_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STA_ABS_X:
                        {
                            Word address = AddressAbsoluteX(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STA_ABS_Y:
                        {
                            Word address = AddressAbsoluteY(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STA_IND_X:
                        {
                            Word address = AddressIndirectX(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STA_IND_Y:
                        {
                            Word address = AddressIndirectY(ref cycles, memory);
                            byte data = A;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    // Store X Register
                    case (byte)Opcodes.STX_ZP:
                        {
                            byte address = AddressZeroPage(ref cycles, memory);
                            byte data = X;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STX_ZP_Y:
                        {

                            byte address = AddressZeroPageY(ref cycles, memory);
                            byte data = X;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STX_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            byte data = X;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    // Store Y Register
                    case (byte)Opcodes.STY_ZP:
                        {
                            byte address = AddressZeroPage(ref cycles, memory);
                            byte data = Y;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STY_ZP_X:
                        {

                            byte address = AddressZeroPageX(ref cycles, memory);
                            byte data = Y;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    case (byte)Opcodes.STY_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            byte data = Y;
                            WriteByte(ref cycles, memory, address, data);
                        }
                        break;

                    // Transfer Accumulator to X Register
                    case (byte)Opcodes.TAX:
                        {
                            X = A;
                            CycleTick(ref cycles);
                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    // Transfer Accumulator to Y Register
                    case (byte)Opcodes.TAY:
                        {
                            Y = A;
                            CycleTick(ref cycles);
                            SetZeroAndNegativeFlags(Y);
                        }
                        break;

                    // Transfer X to Accumulator Register
                    case (byte)Opcodes.TXA:
                        {
                            A = X;
                            CycleTick(ref cycles);
                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    // Transfer Y to Accumulator Register
                    case (byte)Opcodes.TYA:
                        {
                            A = Y;
                            CycleTick(ref cycles);
                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    // Transfer Stack Pointer to X Register
                    case (byte)Opcodes.TSX:
                        {
                            X = SP;
                            CycleTick(ref cycles);
                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    // Transfer X to Stack Pointer Register
                    case (byte)Opcodes.TXS:
                        {
                            SP = X;
                            CycleTick(ref cycles);
                            SetZeroAndNegativeFlags(X);
                        }
                        break;

                    // Push Accumulator
                    case (byte)Opcodes.PHA:
                        {
                            Byte data = A;
                            PushToStack(ref cycles, memory, data);
                        }
                        break;

                    // Push Processor Status
                    case (byte)Opcodes.PHP:
                        {
                            Byte data = StatusFlags;
                            PushToStack(ref cycles, memory, data);
                        }
                        break;

                    // Pull Accumulator
                    case (byte)Opcodes.PLA:
                        {
                            Byte data = PopFromStack(ref cycles, memory);
                            A = data;
                            CycleTick(ref cycles);
                        }
                        break;

                    // Pull Processor Status
                    case (byte)Opcodes.PLP:
                        {
                            Byte data = PopFromStack(ref cycles, memory);
                            StatusFlags = data;
                            CycleTick(ref cycles);
                        }
                        break;

                    default:
                        {
                            throw new IndexOutOfRangeException($"Opcode '{instruction}' at address '0x{(PC - 1).ToString("X")}' not handled");
                        }
                }

            }
        }
    }

    // Emulate 6502 memory
    public struct Memory
    {
        const uint32 MAX_MEMORY = 1024 * 64;
        public readonly Byte[] Data;

        public Memory()
        {
            Data = new Byte[MAX_MEMORY];
        }

        // Allow reading from and writing to memory
        public byte this[uint32 address]
        {
            get
            {
                if (address < 0 || address >= MAX_MEMORY)
                    throw new IndexOutOfRangeException("Index out of range.");
                return Data[address];
            }
            set
            {
                if (address < 0 || address >= MAX_MEMORY)
                    throw new IndexOutOfRangeException("Index out of range.");
                Data[address] = value;
            }
        }

        // Initialize Memory with 0s
        public void Initialize()
        {
            for (uint32 i = 0; i < MAX_MEMORY; i++)
            {
                Data[i] = 0;
            }
        }
    }
}
