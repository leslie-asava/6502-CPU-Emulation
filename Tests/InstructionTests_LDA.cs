
using LA6502.CPU;
using LA6502.Types;

using Word = ushort;
using uint32 = uint;
using int32 = int;

namespace CPUTests
{
    public class InstructionTests_LDA
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
        public void LDA_AbsoluteX_NoPageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDA_ABS_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xBB;
            memory[0xFFFE] = 0xAA;
            memory[0xAABC] = 0xFE;
            cpu.X = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00) 
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_AbsoluteX_PageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDA_ABS_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFF;
            memory[0xFFFE] = 0xAA;
            memory[0xAB00] = 0xFE;
            cpu.X = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_AbsoluteY_NoPageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDA_ABS_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xBB;
            memory[0xFFFE] = 0xAA;
            memory[0xAABC] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_AbsoluteY_PageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDA_ABS_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFF;
            memory[0xFFFE] = 0xAA;
            memory[0xAB00] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_Immediate()
        {
            Opcodes opcode = Opcodes.LDA_IM;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0xFE;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_ZeroPage()
        {
            Opcodes opcode = Opcodes.LDA_ZP;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0042] = 0xFE;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_ZeroPageX()
        {
            Opcodes opcode = Opcodes.LDA_ZP_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0044] = 0xFE;
            cpu.X = 0x02;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_IndirectX()
        {
            Opcodes opcode = Opcodes.LDA_IND_X;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0044] = 0x00;
            memory[0x0045] = 0x80;
            memory[0x8000] = 0xFE;
            cpu.X = 0x02;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_IndirectY_NoPageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDA_IND_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0042] = 0x00;
            memory[0x0043] = 0x80;
            memory[0x8001] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }

        [Test]
        public void LDA_IndirectY_PageBoundaryCross()
        {
            Opcodes opcode = Opcodes.LDA_IND_Y;
            memory[0xFFFC] = (byte)opcode;
            memory[0xFFFD] = 0x42;
            memory[0x0042] = 0xFF;
            memory[0x0043] = 0x80;
            memory[0x8100] = 0xFE;
            cpu.Y = 0x01;
            cpu.Execute(Timing.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with 0xFE.");

            if (cpu.A == 0x00)
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.Z), Is.True, "Zero flag should be set.");
            }
            if (cpu.IsBitSet(cpu.A, 7))
            {
                Assert.That(cpu.IsFlagSet(cpu.StatusFlags, ProcessorFlags.N), Is.True, "Negative flag should be set.");
            }
        }


    }
}