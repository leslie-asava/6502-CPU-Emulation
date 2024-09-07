using LA6502.CPU;
using LA6502.Types;

namespace CPUTests
{
    public class InstructionTests_STX
    {
        private Memory memory;
        private CPU cpu;

        [SetUp]
        public void Setup()
        {
            memory = new Memory();
            cpu = new CPU();
            cpu.Reset(memory);
        }

        [Test]
        public void STX_ZeroPage()
        {
            Opcodes opcode = Opcodes.STX_ZP;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            cpu.X = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x0042], Is.EqualTo(0xFE), "X register should be stored in memory address 0x0042.");
        }

        [Test]
        public void STX_ZeroPageY()
        {
            Opcodes opcode = Opcodes.STX_ZP_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            cpu.Y = 0x05;
            cpu.X = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x0047], Is.EqualTo(0xFE), "X register should be stored in memory address 0x0047.");
        }

        [Test]
        public void STX_Absolute()
        {
            Opcodes opcode = Opcodes.STX_ABS;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xBB;
            memory[0xFFFE] = 0xAA;
            cpu.X = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0xAABB], Is.EqualTo(0xFE), "X register should be stored in memory address 0xAABB.");
        }
    }
}
