using LA6502.CPU;
using LA6502.Types;

namespace CPUTests
{
    public class InstructionTests_TransferRegisters
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
        public void TAX_TransferAccumulatorToXRegister()
        {
            Opcodes opcode = Opcodes.TAX;
            memory[0xFFFC] = (byte)opcode;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(0xFE), "X register should be loaded with the value from the accumulator.");

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
        public void TXA_TransferXToAccumulator()
        {
            Opcodes opcode = Opcodes.TXA;
            memory[0xFFFC] = (byte)opcode;
            cpu.X = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with the value from the X register.");

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
        public void TAY_TransferAccumulatorToYRegister()
        {
            Opcodes opcode = Opcodes.TAY;
            memory[0xFFFC] = (byte)opcode;
            cpu.A = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.Y, Is.EqualTo(0xFE), "Y register should be loaded with the value from the accumulator.");

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
        public void TYA_TransferYToAccumulator()
        {
            Opcodes opcode = Opcodes.TYA;
            memory[0xFFFC] = (byte)opcode;
            cpu.Y = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0xFE), "Accumulator should be loaded with the value from the Y register.");

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
