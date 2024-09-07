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
        // Load Accumulator Register
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

        // Load Y Register
        LDY_IM = 0xA0,
        LDY_ZP = 0xA4,
        LDY_ZP_X = 0xB4,
        LDY_ABS = 0xAC,
        LDY_ABS_X = 0xBC,

        // Store Accumulator Register
        STA_ZP = 0x85,
        STA_ZP_X = 0x95,
        STA_ABS = 0x8D,
        STA_ABS_X = 0x9D,
        STA_ABS_Y = 0x99,
        STA_IND_X = 0x81,
        STA_IND_Y = 0x91,

        // Store X Register
        STX_ZP = 0x86,
        STX_ZP_Y = 0x96,
        STX_ABS = 0x8E,

        // Store Y Register
        STY_ZP = 0x84,
        STY_ZP_X = 0x94,
        STY_ABS = 0x8C,
    }

    enum Instructions
    {
        LDA,
    }

    public struct Clock
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

            { Opcodes.LDY_IM, 2 },      // LDY Immediate
            { Opcodes.LDY_ZP, 3 },      // LDY Zero Page
            { Opcodes.LDY_ZP_X, 4 },    // LDY Zero Page, X
            { Opcodes.LDY_ABS, 4 },     // LDY Absolute
            { Opcodes.LDY_ABS_X, 4 },   // LDY Absolute, X (add 1 cycle if page boundary crossed)

            { Opcodes.STA_ZP, 3 },      // STA Zero Page
            { Opcodes.STA_ZP_X, 4 },    // STA Zero Page, X
            { Opcodes.STA_ABS, 4 },     // STA Absolute
            { Opcodes.STA_ABS_X, 5 },   // STA Absolute, X
            { Opcodes.STA_ABS_Y, 5 },   // STA Absolute, Y
            { Opcodes.STA_IND_X, 6 },   // STA Indirect, X
            { Opcodes.STA_IND_Y, 6 },   // STA Indirect, Y

            { Opcodes.STX_ZP, 3 },      // STX Zero Page
            { Opcodes.STX_ZP_Y, 4 },    // STX Zero Page, Y
            { Opcodes.STX_ABS, 4 },     // STX Absolute

            { Opcodes.STY_ZP, 3 },      // STY Zero Page
            { Opcodes.STY_ZP_X, 4 },    // STY Zero Page, Y
            { Opcodes.STY_ABS, 4 },     // STY Absolute
        };
    }
}
