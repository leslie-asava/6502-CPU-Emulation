using LA6502.CPU;
using LA6502.Types;

namespace CPUTests
{
    public class InstructionTests_STY
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
        public void STY_ZeroPage()
        {
            Opcodes opcode = Opcodes.STY_ZP;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            cpu.Y = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x0042], Is.EqualTo(0xFE), "Y register should be stored in memory address 0x0042.");
        }

        [Test]
        public void STY_ZeroPageX()
        {
            Opcodes opcode = Opcodes.STY_ZP_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            cpu.X = 0x05;
            cpu.Y = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x0047], Is.EqualTo(0xFE), "Y register should be stored in memory address 0x0047.");
        }

        [Test]
        public void STY_Absolute()
        {
            Opcodes opcode = Opcodes.STY_ABS;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xBB;
            memory[0xFFFE] = 0xAA;
            cpu.Y = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0xAABB], Is.EqualTo(0xFE), "Y register should be stored in memory address 0xAABB.");
        }
    }
}
