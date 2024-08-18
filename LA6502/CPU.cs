using LA6502.Types;

using Word = ushort;
using uint32 = uint;
using int32 = int;

namespace LA6502.CPU
{
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

    // Emulate 6502 CPU registers and functioning
    public struct CPU
    {
        public Word PC;            // 16-bit Program Counter register
        public byte SP;            // 8-bit Stack Pointer register
        public byte A;             // 8-bit Accumulator register
        public byte X;             // Index register X
        public byte Y;             // Index register Y
        public byte StatusFlags;   // Processor flags

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

        public void RequestAdditionalCycle(ref uint32 cycles)
        {
            cycles++;
        }

        public bool IsFlagSet(byte StatusFlags, ProcessorFlags bitPosition)
        {
            return (StatusFlags & 1 << (int)bitPosition) != 0;
        }

        public bool IsBitSet(byte Register, int bitPosition)
        {
            return (StatusFlags & 1 << bitPosition) != 0;
        }

        public void SetFlag(byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags |= (byte)(1 << (int)bitPosition);
        }

        public void ClearFlag(byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags &= (byte)~(1 << (int)bitPosition);
        }

        public void ToggleFlag(byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags ^= (byte)(1 << (int)bitPosition);
        }

        void SetStatus(Instructions instruction)
        {
            switch (instruction)
            {
                case Instructions.LDA:
                    {
                        if (A == 0)
                        {
                            SetFlag(StatusFlags, ProcessorFlags.Z);
                        }

                        if (IsBitSet(A, 7))
                        {
                            SetFlag(StatusFlags, ProcessorFlags.N);
                        }
                    }
                    break;
            }
        }

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

        public byte FetchByte(ref uint32 cycles, Memory memory)
        {
            byte Data = memory[PC];
            PC++;
            cycles--;

            return Data;
        }

        public Word FetchWord(ref uint32 cycles, Memory memory)
        {
            byte lowByte = memory[PC];
            PC++;

            byte highByte = memory[PC];
            PC++;

            cycles -= 2;

            Word Data = (Word)(highByte << 8 | lowByte);
            return Data;
        }

        public byte ReadByte(ref uint32 cycles, Memory memory, Word Address)
        {
            byte Data = memory[Address];
            cycles--;

            return Data;
        }

        public Word ReadWord(ref uint32 cycles, Memory memory, Word address)
        {
            byte lowByte = memory[address];
            address++;
            byte highByte = memory[address];

            cycles -= 2;

            Word Data = (Word)(highByte << 8 | lowByte);
            return Data;
        }

        public void WriteByte(ref uint32 cycles, Memory memory, Word Address, byte Data)
        {
            memory[Address] = Data;
            cycles--;
        }

        public byte AddressZeroPage(ref uint32 cycles, Memory memory)
        {
            byte zeroPageAddress = FetchByte(ref cycles, memory);
            return zeroPageAddress;
        }

        public byte AddressZeroPageX(ref uint32 cycles, Memory memory)
        {
            byte zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + X);

            cycles--;

            return zeroPageAddress;
        }

        public byte AddressZeroPageY(ref uint32 cycles, Memory memory)
        {
            byte zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + Y);

            cycles--;

            return zeroPageAddress;
        }

        public Word AddressAbsolute(ref uint32 cycles, Memory memory)
        {
            Word address = FetchWord(ref cycles, memory);
            return address;
        }

        public Word AddressAbsoluteX(ref uint32 cycles, Memory memory)
        {
            Word address = FetchWord(ref cycles, memory);
            Word originalHighByte = (Word)(address & 0xFF00);
            address = (Word)(address + X);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                RequestAdditionalCycle(ref cycles);
                // Decrement cycles if a page boundary was crossed
                cycles--;
            }

            return address;
        }

        public Word AddressAbsoluteY(ref uint32 cycles, Memory memory)
        {
            Word address = FetchWord(ref cycles, memory);
            Word originalHighByte = (Word)(address & 0xFF00);
            address = (Word)(address + Y);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                RequestAdditionalCycle(ref cycles);
                // Decrement cycles if a page boundary was crossed
                cycles--;
            }

            return address;
        }

        public Word AddressIndirect(ref uint32 cycles, Memory memory)
        {
            // The address in the instruction points to another address
            Word pointerAddress = AddressAbsolute(ref cycles, memory);

            // The absolute address we've gotten point to yet another address that holds the target data
            Word address = ReadWord(ref cycles, memory, pointerAddress);

            return address;
        }

        public Word AddressIndirectX(ref uint32 cycles, Memory memory)
        {
            // The address in the instruction points to another address
            Word zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + X);

            cycles--;

            // The absolute address we've gotten point to yet another address that holds the target data
            Word address = ReadWord(ref cycles, memory, zeroPageAddress);

            return address;
        }

        public Word AddressIndirectY(ref uint32 cycles, Memory memory)
        {
            // The address in the instruction points to another address
            Word zeroPageAddress = FetchByte(ref cycles, memory);
            Word address = ReadWord(ref cycles, memory, zeroPageAddress);

            Word originalHighByte = (Word)(address & 0xFF00);
            address = (Word)(address + Y);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                RequestAdditionalCycle(ref cycles);
                // Decrement cycles if a page boundary was crossed
                cycles--;
            }

            return address;
        }

        public Word AddressRelative(ref uint32 cycles, Memory memory)
        {
            Byte offset = FetchByte(ref cycles, memory);

            var e = (sbyte)offset;
            Word address = (Word)(PC + (sbyte)offset);

            cycles--;

            return address;
        }

        public void Execute(uint32 cycles, Memory memory)
        {
            while (cycles > 0)
            {
                byte instruction = FetchByte(ref cycles, memory);

                switch (instruction)
                {
                    // Load Accumulator
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
                            Word address = AddressAbsoluteX(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags(A);
                        }
                        break;

                    case (byte)Opcodes.LDA_ABS_Y:
                        {
                            Word address = AddressAbsoluteY(ref cycles, memory);
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
                            Word address = AddressIndirectY(ref cycles, memory);
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
                            Word address = AddressAbsoluteY(ref cycles, memory);
                            byte operand = ReadByte(ref cycles, memory, address);

                            X = operand;

                            SetZeroAndNegativeFlags(Y);
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
}
