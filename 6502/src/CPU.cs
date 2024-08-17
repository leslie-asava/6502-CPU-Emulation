using _6502Core;
using _6502Memory;

using Word = ushort;
using uint32 = uint;
using int32 = int;

namespace _6502CPU
{
    public struct CPU
    {
        public Word PC;            // 16-bit Program Counter register
        public Byte SP;            // 8-bit Stack Pointer register
        public Byte A;             // 8-bit Accumulator register
        public Byte X;             // Index register X
        public Byte Y;             // Index register Y
        public Byte StatusFlags;   // Processor flags

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

        public bool IsFlagSet(Byte StatusFlags, ProcessorFlags bitPosition)
        {
            return (StatusFlags & (1 << (int)bitPosition)) != 0;
        }

        public bool IsBitSet(Byte Register, int bitPosition)
        {
            return (StatusFlags & (1 << bitPosition)) != 0;
        }

        public void SetFlag(Byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags |= (byte)(1 << (int)bitPosition);
        }

        public void ClearFlag(Byte StatusFlags, ProcessorFlags bitPosition)
        {
            StatusFlags &= (byte)~(1 << (int)bitPosition);
        }

        public void ToggleFlag(Byte StatusFlags, ProcessorFlags bitPosition)
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

        void SetZeroAndNegativeFlags()
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

        public Byte FetchByte(ref uint32 cycles, Memory memory)
        {
            Byte Data = memory[PC];
            PC++;
            cycles--;

            return Data;
        }

        public Word FetchWord(ref uint32 cycles, Memory memory)
        {
            Byte lowByte = memory[PC];
            PC++;

            Byte highByte = memory[PC];
            PC++;

            cycles -= 2;

            Word Data = (Word)((highByte << 8) | lowByte);
            return Data;
        }

        public Byte ReadByte(ref uint32 cycles, Memory memory, Word Address)
        {
            Byte Data = memory[Address];
            cycles--;

            return Data;
        }

        public Word ReadWord(ref uint32 cycles, Memory memory, Word address)
        {
            Byte lowByte = memory[address];
            address++;
            Byte highByte = memory[address];

            cycles -= 2;

            Word Data = (Word)((highByte << 8) | lowByte);
            return Data;
        }

        public void WriteByte(ref uint32 cycles, Memory memory, Word Address, Byte Data)
        {
            memory[Address] = Data;
            cycles--;
        }

        public Byte AddressZeroPage(ref uint32 cycles, Memory memory)
        {
            Byte zeroPageAddress = FetchByte(ref cycles, memory);
            return zeroPageAddress;
        }

        public Byte AddressZeroPageX(ref uint32 cycles, Memory memory)
        {
            Byte zeroPageAddress = FetchByte(ref cycles, memory);
            zeroPageAddress = (byte)(zeroPageAddress + X);

            cycles--;

            return zeroPageAddress;
        }

        public Byte AddressZeroPageY(ref uint32 cycles, Memory memory)
        {
            Byte zeroPageAddress = FetchByte(ref cycles, memory);
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
            address = (byte)(address + Y);

            // Check if the high byte has changed, indicating a page boundary crossing
            if ((address & 0xFF00) != originalHighByte)
            {
                // Decrement cycles if a page boundary was crossed
                cycles--;
            }

            return address;
        }

        public void Execute(uint32 cycles, Memory memory)
        {
            while (cycles > 0)
            {
                Byte instruction = FetchByte(ref cycles, memory);

                switch (instruction)
                {
                    case (byte)Opcodes.LDA_IM:
                        {
                            Byte operand = FetchByte(ref cycles, memory);
                            A = operand;

                            SetZeroAndNegativeFlags();
                        }
                        break;

                    case (byte)Opcodes.LDA_ZP:
                        {
                            Byte address = AddressZeroPage(ref cycles, memory);
                            Byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags();
                        }
                        break;

                    case (byte)Opcodes.LDA_ZP_X:
                        {

                            Byte address = AddressZeroPageX(ref cycles, memory);
                            Byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags();
                        }
                        break;

                    case (byte)Opcodes.LDA_ABS:
                        {
                            Word address = AddressAbsolute(ref cycles, memory);
                            Byte operand = ReadByte(ref cycles, memory, address);

                            A = operand;

                            SetZeroAndNegativeFlags();
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
