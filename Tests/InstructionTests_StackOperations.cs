using LA6502.CPU;
using LA6502.Types;

using Word = ushort;
using uint32 = uint;

namespace CPUTests
{
    public class InstructionTests_StackOperations
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
        public void TSX_Test()
        {
            Opcodes opcode = Opcodes.TSX;
            memory[0xFFFC] = (byte)opcode;
            cpu.SP = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.X, Is.EqualTo(cpu.SP), "X register should be loaded with Stack Pointer.");
        }

        [Test]
        public void TXS_Test()
        {
            Opcodes opcode = Opcodes.TXS;
            memory[0xFFFC] = (byte)opcode;
            cpu.X = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.SP, Is.EqualTo(cpu.X), "Stack Pointer should be loaded with X register.");
        }

        [Test]
        public void PHA_Test()
        {
            Opcodes opcode = Opcodes.PHA;
            memory[0xFFFC] = (byte)opcode;
            cpu.A = 0x42;
            cpu.SP = 0xFF;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[(uint32)(0x0100 + cpu.SP + 1)], Is.EqualTo(cpu.A), "Accumulator should be pushed to the stack.");
        }

        [Test]
        public void PHP_Test()
        {
            Opcodes opcode = Opcodes.PHP;
            memory[0xFFFC] = (byte)opcode;
            cpu.StatusFlags = 0x42;
            cpu.SP = 0xFF;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(memory[(uint32)(0x0100 + cpu.SP + 1)], Is.EqualTo(cpu.StatusFlags), "Processor Status Flags should be pushed to the stack.");
        }

        [Test]
        public void PLA_Test()
        {
            Opcodes opcode = Opcodes.PLA;
            memory[0xFFFC] = (byte)opcode;
            memory[0x0100 + 0xFE] = 0x42;
            cpu.SP = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.A, Is.EqualTo(0x42), "Accumulator should be pulled from the stack.");
        }

        [Test]
        public void PLP_Test()
        {
            Opcodes opcode = Opcodes.PLP;
            memory[0xFFFC] = (byte)opcode;
            memory[0x0100 + 0xFE] = 0x42;
            cpu.SP = 0xFE;
            cpu.Execute(Clock.OpcodeCycles[opcode], memory);

            Assert.That(cpu.StatusFlags, Is.EqualTo(0x42), "Processor Status Flags should be pulled from the stack.");
        }
    }
}
