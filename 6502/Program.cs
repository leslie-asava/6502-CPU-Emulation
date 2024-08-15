using System;

using Word = ushort;
using uint32 = uint;
using int32 = int;

enum ProcessorFlags
{
    C = 0,      // Carry Flag
    Z = 1,      // Zero Flag
    I = 2,      // Interupt Disable Flag
    D = 3,      // Decimal Flag
    B = 4,      // Break Flag
    V = 6,      // Overflow Flag
    N = 7,      // Negative Flag
}

enum Opcodes : Byte
{
    LDA_IM = 0xA9,
    LDA_ZP = 0xA5,
    LDA_ZP_X = 0xB5,
    LDA_ABS = 0xAD,
}

enum Instructions
{
    LDA,
}

struct CPU
{
    Word PC;            // 16-bit Program Counter register
    Byte SP;            // 8-bit Stack Pointer register
    Byte A;             // 8-bit Accumulator register
    Byte X;             // Index register X
    Byte Y;             // Index register Y
    Byte StatusFlags;   // Processor flags

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

    Byte FetchByte(ref uint32 cycles, Memory memory)
    {
        Byte Data = memory[PC];
        PC++;
        cycles--;

        return Data;
    }

    Word FetchWord(ref uint32 cycles, Memory memory)
    {
        Byte lowByte = memory[PC];
        PC++;

        Byte highByte = memory[PC];
        PC++;

        cycles -= 2;

        Word Data = (Word)((highByte << 8) | lowByte);
        return Data;
    }

    Byte ReadByte(ref uint32 cycles, Memory memory, Word Address)
    {
        Byte Data = memory[Address];
        cycles--;

        return Data;
    }

    Byte AddressZeroPage(ref uint32 cycles, Memory memory)
    {
        Byte zeroPageAddress = FetchByte(ref cycles, memory);
        return zeroPageAddress;
    }

    Byte AddressZeroPageX(ref uint32 cycles, Memory memory)
    {
        Byte zeroPageAddress = FetchByte(ref cycles, memory);
        zeroPageAddress = (byte)(zeroPageAddress + X);

        cycles--;

        return zeroPageAddress;
    }

    Byte AddressZeroPageY(ref uint32 cycles, Memory memory)
    {
        Byte zeroPageAddress = FetchByte(ref cycles, memory);
        zeroPageAddress = (byte)(zeroPageAddress + X);

        cycles--;

        return zeroPageAddress;
    }

    Word AddressAbsolute(ref uint32 cycles, Memory memory)
    {
        Word address = FetchWord(ref cycles, memory);
        return address;
    }

    Word AddressAbsoluteX(ref uint32 cycles, Memory memory)
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

    Word AddressAbsoluteY(ref uint32 cycles, Memory memory)
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
                    }break;

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


struct Memory
{
    const uint32 MAX_MEMORY = 1024 * 64;
    public readonly Byte[] Data;

    public Memory()
    {
        Data = new Byte[MAX_MEMORY];
    }

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

    public void Initialize()
    {
        for (uint32 i = 0; i < MAX_MEMORY; i++)
        {
            Data[i] = 0;
        }
    }
}

class Program
{
    static int Main()
    {
        Memory memory = new Memory();
        CPU cpu = new CPU();
        cpu.Reset(memory);
        memory[0xFFFC] = (byte)Opcodes.LDA_ABS;
        memory[0xFFFD] = 0x01;
        memory[0xFFFD] = 0x01;
        memory[0x0001] = 0xfe;
        cpu.Execute(4, memory);
        var c = cpu;
        return 0;
    }
}