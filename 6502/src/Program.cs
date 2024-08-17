using _6502Core;
using _6502CPU;
using _6502Memory;

using Word = ushort;
using uint32 = uint;
using int32 = int;

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
        //cpu.Execute(4, memory);
        return 0;
    }
}