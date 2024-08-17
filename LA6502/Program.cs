using LA6502.CPU;
using LA6502.Types;

class Program
{
    static int Main()
    {
        Memory memory = new Memory();
        CPU cpu = new CPU();
        cpu.Reset(memory);
        Opcodes opcode = Opcodes.LDA_ABS_X;
        memory[0xFFFC] = (byte)opcode;
        memory[0xFFFD] = 0xbb;
        memory[0xFFFE] = 0xaa;
        memory[0xaabc] = 0xfe;
        cpu.X = 0x01;
        cpu.Execute(Timing.OpcodeCycles[opcode], memory);
        return 0;
    }
}