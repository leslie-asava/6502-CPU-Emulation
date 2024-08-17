using _6502Core;
using _6502Memory;
using _6502CPU;
using System;

using Word = ushort;
using uint32 = uint;
using int32 = int;

namespace _6502Tests
{
    public class AddressingModeTests
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
        public void AddressZeroPageTest()
        {
            Byte targetAddress = 0x42;
            Byte targetValue = 0x37;

            uint32 cycles = 2;
            cpu.PC = 0x0200;

            cpu.WriteByte(ref cycles, memory, cpu.PC, targetAddress);
            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Byte fetchedAddress = cpu.AddressZeroPage(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched zero-page address should be {targetAddress:X2}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from zero-page address {fetchedAddress:X2} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressZeroPageXTest()
        {
            Byte baseAddress = 0x42;
            Byte offsetX = 0x10;
            Byte targetAddress = (Byte)(baseAddress + offsetX);
            Byte targetValue = 0x37;

            uint32 cycles = 3;
            cpu.PC = 0x0200;

            cpu.X = offsetX;

            cpu.WriteByte(ref cycles, memory, cpu.PC, baseAddress);
            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Byte fetchedAddress = cpu.AddressZeroPageX(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched zero-page,X address should be {targetAddress:X2}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from zero-page,X address {fetchedAddress:X2} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressAbsoluteTest()
        {
            Word targetAddress = 0x1234;
            Byte targetValue = 0x37;

            uint32 cycles = 4;
            cpu.PC = 0x0200;

            Byte lowByte = (Byte)(targetAddress & 0xFF);
            Byte highByte = (Byte)((targetAddress >> 8) & 0xFF);

            cpu.WriteByte(ref cycles, memory, cpu.PC, lowByte);
            cpu.WriteByte(ref cycles, memory, (Word)(cpu.PC + 1), highByte);
            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Word fetchedAddress = cpu.AddressAbsolute(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched absolute address should be {targetAddress:X4}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from absolute address {fetchedAddress:X4} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressAbsoluteXTest()
        {
            Word baseAddress = 0x1234;
            Byte offsetX = 0x10;
            Word targetAddress = (Word)(baseAddress + offsetX);
            Byte targetValue = 0x37;

            uint32 cycles = 4;
            cpu.PC = 0x0200;
            cpu.X = offsetX;

            Byte lowByte = (Byte)(baseAddress & 0xFF);
            Byte highByte = (Byte)((baseAddress >> 8) & 0xFF);

            cpu.WriteByte(ref cycles, memory, cpu.PC, lowByte);
            cpu.WriteByte(ref cycles, memory, (Word)(cpu.PC + 1), highByte);
            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Word fetchedAddress = cpu.AddressAbsoluteX(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched absolute,X address should be {targetAddress:X4}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from absolute,X address {fetchedAddress:X4} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressAbsoluteYTest()
        {
            Word baseAddress = 0x1234;
            Byte offsetY = 0x20;
            Word targetAddress = (Word)(baseAddress + offsetY);
            Byte targetValue = 0x37;

            uint32 cycles = 4;
            cpu.PC = 0x0200;
            cpu.Y = offsetY;

            Byte lowByte = (Byte)(baseAddress & 0xFF);
            Byte highByte = (Byte)((baseAddress >> 8) & 0xFF);

            cpu.WriteByte(ref cycles, memory, cpu.PC, lowByte);
            cpu.WriteByte(ref cycles, memory, (Word)(cpu.PC + 1), highByte);
            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Word fetchedAddress = cpu.AddressAbsoluteY(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched absolute,Y address should be {targetAddress:X4}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from absolute,Y address {fetchedAddress:X4} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressIndirectTest()
        {
            Word pointerAddress = 0x0100;
            Word targetAddress = 0x1234;
            Byte targetValue = 0x37;

            uint32 cycles = 5;
            cpu.PC = 0x0200;

            Byte lowByte = (Byte)(pointerAddress & 0xFF);
            Byte highByte = (Byte)((pointerAddress >> 8) & 0xFF);

            cpu.WriteByte(ref cycles, memory, cpu.PC, lowByte);
            cpu.WriteByte(ref cycles, memory, (Word)(cpu.PC + 1), highByte);

            cpu.WriteByte(ref cycles, memory, pointerAddress, (Byte)(targetAddress & 0xFF));
            cpu.WriteByte(ref cycles, memory, (Word)(pointerAddress + 1), (Byte)((targetAddress >> 8) & 0xFF));

            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Word fetchedAddress = cpu.AddressIndirect(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched indirect address should be {targetAddress:X4}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from indirect address {fetchedAddress:X4} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressIndirectXTest()
        {
            Byte baseZeroPageAddress = 0x40;
            Byte offsetX = 0x10;
            Byte zeroPageAddress = (Byte)(baseZeroPageAddress + offsetX);
            Word targetAddress = 0x1234;
            Byte targetValue = 0x37;

            uint32 cycles = 6;
            cpu.PC = 0x0200;
            cpu.X = offsetX;

            cpu.WriteByte(ref cycles, memory, cpu.PC, baseZeroPageAddress);

            cpu.WriteByte(ref cycles, memory, zeroPageAddress, (Byte)(targetAddress & 0xFF));
            cpu.WriteByte(ref cycles, memory, (Byte)(zeroPageAddress + 1), (Byte)((targetAddress >> 8) & 0xFF));

            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Word fetchedAddress = cpu.AddressIndirectX(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched indirect,X address should be {targetAddress:X4}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from indirect,X address {fetchedAddress:X4} should be {targetValue:X2}.");
        }

        [Test]
        public void AddressIndirectYTest()
        {
            Byte zeroPageAddress = 0x40;
            Byte offsetY = 0x20;
            Word baseAddress = 0x1000;
            Word targetAddress = (Word)(baseAddress + offsetY);
            Byte targetValue = 0x37;

            uint32 cycles = 5;
            cpu.PC = 0x0200;
            cpu.Y = offsetY;

            cpu.WriteByte(ref cycles, memory, cpu.PC, zeroPageAddress);

            cpu.WriteByte(ref cycles, memory, zeroPageAddress, (Byte)(baseAddress & 0xFF));
            cpu.WriteByte(ref cycles, memory, (Byte)(zeroPageAddress + 1), (Byte)((baseAddress >> 8) & 0xFF));

            cpu.WriteByte(ref cycles, memory, targetAddress, targetValue);

            Word fetchedAddress = cpu.AddressIndirectY(ref cycles, memory);
            Byte fetchedValue = cpu.ReadByte(ref cycles, memory, fetchedAddress);

            Assert.That(fetchedAddress, Is.EqualTo(targetAddress), $"The fetched indirect,Y address should be {targetAddress:X4}.");
            Assert.That(fetchedValue, Is.EqualTo(targetValue), $"The value fetched from indirect,Y address {fetchedAddress:X4} should be {targetValue:X2}.");
        }
        

    }
}