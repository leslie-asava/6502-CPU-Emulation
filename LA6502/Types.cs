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

    public enum Opcodes : byte
    {
        // Load Accumulator
        LDA_IM = 0xA9,
        LDA_ZP = 0xA5,
        LDA_ZP_X = 0xB5,
        LDA_ABS = 0xAD,
        LDA_ABS_X = 0xBD,
        LDA_ABS_Y = 0xB9,
        LDA_IND_X = 0xA1,
        LDA_IND_Y = 0xB1,

        // Load X Register
        LDX_IM = 0xA2,
        LDX_ZP = 0xA6,
        LDX_ZP_Y = 0xB6,
        LDX_ABS = 0xAE,
        LDX_ABS_Y = 0xBE,
    }

    enum Instructions
    {
        LDA,
    }

    public struct Timing
    {
        // Dictionary to map opcodes to their cycle counts
        public static readonly Dictionary<Opcodes, uint> OpcodeCycles = new Dictionary<Opcodes, uint>
        {
            { Opcodes.LDA_IM, 2 },      // LDA Immediate
            { Opcodes.LDA_ZP, 3 },      // LDA Zero Page
            { Opcodes.LDA_ZP_X, 4 },    // LDA Zero Page, X
            { Opcodes.LDA_ABS, 4 },     // LDA Absolute
            { Opcodes.LDA_ABS_X, 4 },   // LDA Absolute, X (add 1 cycle if page boundary crossed)
            { Opcodes.LDA_ABS_Y, 4 },   // LDA Absolute, Y (add 1 cycle if page boundary crossed)
            { Opcodes.LDA_IND_X, 6 },   // LDA Indirect, X
            { Opcodes.LDA_IND_Y, 5 },   // LDA Indirect, Y (add 1 cycle if page boundary crossed)

            { Opcodes.LDX_IM, 2 },      // LDX Immediate
            { Opcodes.LDX_ZP, 3 },      // LDX Zero Page
            { Opcodes.LDX_ZP_Y, 4 },    // LDX Zero Page, Y
            { Opcodes.LDX_ABS, 4 },     // LDX Absolute
            { Opcodes.LDX_ABS_Y, 4 },   // LDX Absolute, Y (add 1 cycle if page boundary crossed)
        };
    }
}
