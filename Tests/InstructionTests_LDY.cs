using LA6502.CPU;
using LA6502.Types;

namespace CPUTests
{
    [TestFixture]
    public class InstructionTests_LDY
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
        public void LDY_Immediate()
        {
            Opcodes opcode = Opcodes.LDY_IM;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFE;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with 0xFE.");

            if (cpu.Y == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.Y, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDY_ZeroPage()
        {
            Opcodes opcode = Opcodes.LDY_ZP;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0042] = 0xFE;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with 0xFE.");

            if (cpu.Y == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.Y, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDY_ZeroPageX()
        {
            Opcodes opcode = Opcodes.LDY_ZP_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0043] = 0xFE;
            cpu.X = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with 0xFE.");

            if (cpu.Y == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.Y, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDY_Absolute()
        {
            Opcodes opcode = Opcodes.LDY_ABS;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x00;
            memory[0xFFFE] = 0x80;
            memory[0x8000] = 0xFE;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with 0xFE.");

            if (cpu.Y == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.Y, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDY_AbsoluteX_NoPageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDY_ABS_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xBB;
            memory[0xFFFE] = 0xAA;
            memory[0xAABC] = 0xFE;
            cpu.X = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with 0xFE.");

            if (cpu.Y == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.Y, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDY_AbsoluteX_PageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDY_ABS_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFF;
            memory[0xFFFE] = 0xAA;
            memory[0xAB00] = 0xFE;
            cpu.X = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with 0xFE.");

            if (cpu.Y == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.Y, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }
    }
}
