using System;

namespace LA6502.Types
{
    public enum ProcessorFlags
    {
        C = 0,      // Carry Flag
        Z = 1,      // Zero Flag
        I = 2,      // Interupt Disable Flag
        D = 3,      // Decimal Flag
        B = 4,      // Break Flag
        V = 6,      // Overflow Flag
        N = 7,      // Negative Flag
    }

    enum Opcodes : byte
    {
        LDA_IM = 0xA9,
        LDA_ZP = 0xA5,
        LDA_ZP_X = 0xB5,
        LDA_ABS = 0xAD,
    }

    enum Instructions
    {
        LDA,
    }
}
