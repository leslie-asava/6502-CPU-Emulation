using LA6502.CPU;
using LA6502.Types;

namespace CPUTests
{
    public class InstructionTests_LDX
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
        public void LDX_Immediate()
        {
            Opcodes opcode = Opcodes.LDX_IM;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with 0xFE.");

            if (cpu.X == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.X, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDX_ZeroPage()
        {
            Opcodes opcode = Opcodes.LDX_ZP;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0042] = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with 0xFE.");

            if (cpu.X == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.X, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDX_ZeroPageY()
        {
            Opcodes opcode = Opcodes.LDX_ZP_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0043] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with 0xFE.");

            if (cpu.X == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.X, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDX_Absolute()
        {
            Opcodes opcode = Opcodes.LDX_ABS;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x00;
            memory[0xFFFE] = 0x80;
            memory[0x8000] = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with 0xFE.");

            if (cpu.X == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.X, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDX_AbsoluteY_NoPageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDX_ABS_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xBB;
            memory[0xFFFE] = 0xAA;
            memory[0xAABC] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with 0xFE.");

            if (cpu.X == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.X, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDX_AbsoluteY_PageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDX_ABS_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFF;
            memory[0xFFFE] = 0xAA;
            memory[0xAB00] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with 0xFE.");

            if (cpu.X == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.X, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }
    }
}
