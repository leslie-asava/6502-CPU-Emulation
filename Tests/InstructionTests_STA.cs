using LA6502.CPU;
using LA6502.Types;

namespace CPUTests
{
    public class InstructionTests_STA
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
        public void STA_ZeroPage()
        {
            Opcodes opcode = Opcodes.STA_ZP;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x0042], Is.EqualTo(0xFE), "Memory at 0x0042 should be loaded with 0xFE.");
        }

        [Test]
        public void STA_ZeroPageX()
        {
            Opcodes opcode = Opcodes.STA_ZP_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            cpu.X = 0x01;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x0043], Is.EqualTo(0xFE), "Memory at 0x0043 should be loaded with 0xFE.");
        }

        [Test]
        public void STA_Absolute()
        {
            Opcodes opcode = Opcodes.STA_ABS;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x00;
            memory[0xFFFE] = 0x80;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x8000], Is.EqualTo(0xFE), "Memory at 0x8000 should be loaded with 0xFE.");
        }

        [Test]
        public void STA_AbsoluteX()
        {
            Opcodes opcode = Opcodes.STA_ABS_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x00;
            memory[0xFFFE] = 0x80;
            cpu.X = 0x01;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x8001], Is.EqualTo(0xFE), "Memory at 0x8001 should be loaded with 0xFE.");
        }

        [Test]
        public void STA_AbsoluteY()
        {
            Opcodes opcode = Opcodes.STA_ABS_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x00;
            memory[0xFFFE] = 0x80;
            cpu.Y = 0x01;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x8001], Is.EqualTo(0xFE), "Memory at 0x8001 should be loaded with 0xFE.");
        }

        [Test]
        public void STA_IndirectX()
        {
            Opcodes opcode = Opcodes.STA_IND_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0043] = 0x00;
            memory[0x0044] = 0x80;
            cpu.X = 0x01;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x8000], Is.EqualTo(0xFE), "Memory at 0x8000 should be loaded with 0xFE.");
        }

        [Test]
        public void STA_IndirectY()
        {
            Opcodes opcode = Opcodes.STA_IND_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0042] = 0x00;
            memory[0x0043] = 0x80;
            cpu.Y = 0x01;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[0x8001], Is.EqualTo(0xFE), "Memory at 0x8001 should be loaded with 0xFE.");
        }
    }
}
