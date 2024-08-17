using _6502Core;

using Word = ushort;
using uint32 = uint;
using int32 = int;

namespace _6502Memory
{
    public struct Memory
    {
        const uint32 MAX_MEMORY = 1024 * 64;
        public readonly Byte[] Data;

        public Memory()
        {
            Data = new Byte[MAX_MEMORY];
        }

        public byte this[uint32 address]
        {
            get
            {
                if (address < 0 || address >= MAX_MEMORY)
                    throw new IndexOutOfRangeException("Index out of range.");
                return Data[address];
            }
            set
            {
                if (address < 0 || address >= MAX_MEMORY)
                    throw new IndexOutOfRangeException("Index out of range.");
                Data[address] = value;
            }
        }

        public void Initialize()
        {
            for (uint32 i = 0; i < MAX_MEMORY; i++)
            {
                Data[i] = 0;
            }
        }
    }
}
