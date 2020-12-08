namespace Yak2D.Internal
{
    /// <summary>
    /// Replication of Veldrid Enum. To Keep Veldrid types out of interfaces
    /// "The format of data stored in a Veldrid.Texture. Each name is a compound identifier,
    /// where each component denotes a name and a number of bits used to store that component.
    /// The final component identifies the storage type of each component. "Float" identifies
    /// a signed, floating-point type, UNorm identifies an unsigned integer type which
    /// is normalized, meaning it occupies the full space of the integer type. The SRgb
    /// suffix for normalized integer formats indicates that the RGB components are stored
    /// in sRGB format." - From Veldrid Docs
    /// </summary>
    public enum TexturePixelFormat : byte
    {
        /// <summary>
        /// RGBA component order. Each component is an 8-bit unsigned normalized integer.
        /// </summary>
        R8_G8_B8_A8_UNorm = 0,

        /// <summary>
        /// BGRA component order. Each component is an 8-bit unsigned normalized integer.
        /// </summary>
        B8_G8_R8_A8_UNorm = 1,

        /// <summary>
        /// Single-channel, 8-bit unsigned normalized integer.
        /// </summary>
        R8_UNorm = 2,

        /// <summary>
        /// Single-channel, 16-bit unsigned normalized integer. Can be used as a depth format.
        /// </summary>
        R16_UNorm = 3,

        /// <summary>
        /// RGBA component order. Each component is a 32-bit signed floating-point value.
        /// </summary>
        R32_G32_B32_A32_Float = 4,

        /// <summary>
        /// Single-channel, 32-bit signed floating-point value. Can be used as a depth format.
        /// </summary>
        R32_Float = 5,

        /// <summary>
        /// BC3 block compressed format.
        /// </summary>
        BC3_UNorm = 6,

        /// <summary>
        /// A depth-stencil format where the depth is stored in a 24-bit unsigned normalized
        /// integer, and the stencil is stored in an 8-bit unsigned integer.
        /// </summary>
        D24_UNorm_S8_UInt = 7,

        /// <summary>
        /// A depth-stencil format where the depth is stored in a 32-bit signed floating-point
        /// value, and the stencil is stored in an 8-bit unsigned integer.
        /// </summary>
        D32_Float_S8_UInt = 8,

        /// <summary>
        /// RGBA component order. Each component is a 32-bit unsigned integer.
        /// </summary>
        R32_G32_B32_A32_UInt = 9,

        /// <summary>
        /// RG component order. Each component is an 8-bit signed normalized integer.
        /// </summary>
        R8_G8_SNorm = 10,

        /// <summary>
        /// BC1 block compressed format with no alpha.
        /// </summary>
        BC1_Rgb_UNorm = 11,

        /// <summary>
        /// BC1 block compressed format with a single-bit alpha channel.
        /// </summary>
        BC1_Rgba_UNorm = 12,

        /// <summary>
        /// BC2 block compressed format.
        /// </summary>
        BC2_UNorm = 13,

        /// <summary>
        /// A 32-bit packed format. The 10-bit R component occupies bits 0..9, the 10-bit
        /// G component occupies bits 10..19, the 10-bit A component occupies 20..29, and
        /// the 2-bit A component occupies bits 30..31. Each value is an unsigned, normalized
        /// integer.
        /// </summary>
        R10_G10_B10_A2_UNorm = 14,

        /// <summary>
        /// A 32-bit packed format. The 10-bit R component occupies bits 0..9, the 10-bit
        /// G component occupies bits 10..19, the 10-bit A component occupies 20..29, and
        /// the 2-bit A component occupies bits 30..31. Each value is an unsigned integer.
        /// </summary>
        R10_G10_B10_A2_UInt = 15,

        /// <summary>
        /// A 32-bit packed format. The 11-bit R componnent occupies bits 0..10, the 11-bit
        /// G component occupies bits 11..21, and the 10-bit B component occupies bits 22..31.
        /// Each value is an unsigned floating point value.
        /// </summary>
        R11_G11_B10_Float = 16,

        /// <summary>
        /// Single-channel, 8-bit signed normalized integer.
        /// </summary>
        R8_SNorm = 17,

        /// <summary>
        /// Single-channel, 8-bit unsigned integer.
        /// </summary>
        R8_UInt = 18,

        /// <summary>
        /// Single-channel, 8-bit signed integer.
        /// </summary>
        R8_SInt = 19,

        /// <summary>
        /// Single-channel, 16-bit signed normalized integer.
        /// </summary>
        R16_SNorm = 20,

        /// <summary>
        /// Single-channel, 16-bit unsigned integer.
        /// </summary>
        R16_UInt = 21,

        /// <summary>
        /// Single-channel, 16-bit signed integer.
        /// </summary>
        R16_SInt = 22,

        /// <summary>
        /// Single-channel, 16-bit signed floating-point value.
        /// </summary>
        R16_Float = 23,

        /// <summary>
        /// Single-channel, 32-bit unsigned integer
        /// </summary>
        R32_UInt = 24,

        /// <summary>
        /// Single-channel, 32-bit signed integer
        /// </summary>
        R32_SInt = 25,

        /// <summary>
        /// RG component order. Each component is an 8-bit unsigned normalized integer.
        /// </summary>
        R8_G8_UNorm = 26,

        /// <summary>
        /// RG component order. Each component is an 8-bit unsigned integer.
        /// </summary>
        R8_G8_UInt = 27,

        /// <summary>
        /// RG component order. Each component is an 8-bit signed integer.
        /// </summary>
        R8_G8_SInt = 28,

        /// <summary>
        /// RG component order. Each component is a 16-bit unsigned normalized integer.
        /// </summary>
        R16_G16_UNorm = 29,

        /// <summary>
        /// RG component order. Each component is a 16-bit signed normalized integer.
        /// </summary>
        R16_G16_SNorm = 30,

        /// <summary>
        /// RG component order. Each component is a 16-bit unsigned integer.
        /// </summary>
        R16_G16_UInt = 31,

        /// <summary>
        /// RG component order. Each component is a 16-bit signed integer.
        /// </summary>
        R16_G16_SInt = 32,

        /// <summary>
        /// RG component order. Each component is a 16-bit signed floating-point value.
        /// </summary>
        R16_G16_Float = 33,

        /// <summary>
        /// RG component order. Each component is a 32-bit unsigned integer.
        /// </summary>
        R32_G32_UInt = 34,

        /// <summary>
        /// RG component order. Each component is a 32-bit signed integer.
        /// </summary>
        R32_G32_SInt = 35,

        /// <summary>
        /// RG component order. Each component is a 32-bit signed floating-point value.
        /// </summary>
        R32_G32_Float = 36,
        
        /// <summary>
        /// RGBA component order. Each component is an 8-bit signed normalized integer.
        /// </summary>
        R8_G8_B8_A8_SNorm = 37,

        /// <summary>
        /// RGBA component order. Each component is an 8-bit unsigned integer.
        /// </summary>
        R8_G8_B8_A8_UInt = 38,

        /// <summary>
        /// RGBA component order. Each component is an 8-bit signed integer.
        /// </summary>
        R8_G8_B8_A8_SInt = 39,

        /// <summary>
        /// RGBA component order. Each component is a 16-bit unsigned normalized integer.
        /// </summary>
        R16_G16_B16_A16_UNorm = 40,

        /// <summary>
        /// RGBA component order. Each component is a 16-bit signed normalized integer.
        /// </summary>
        R16_G16_B16_A16_SNorm = 41,

        /// <summary>
        /// RGBA component order. Each component is a 16-bit unsigned integer.
        /// </summary>
        R16_G16_B16_A16_UInt = 42,

        /// <summary>
        /// RGBA component order. Each component is a 16-bit signed integer.
        /// </summary>
        R16_G16_B16_A16_SInt = 43,

        /// <summary>
        /// RGBA component order. Each component is a 16-bit floating-point value.
        /// </summary>
        R16_G16_B16_A16_Float = 44,

        /// <summary>
        /// RGBA component order. Each component is a 32-bit signed integer.
        /// </summary>
        R32_G32_B32_A32_SInt = 45,

        /// <summary>
        /// A 64-bit, 4x4 block-compressed format storing unsigned normalized RGB data.
        /// </summary>
        ETC2_R8_G8_B8_UNorm = 46,

        /// <summary>
        /// A 64-bit, 4x4 block-compressed format storing unsigned normalized RGB data, as
        /// well as 1 bit of alpha data.
        /// </summary>
        ETC2_R8_G8_B8_A1_UNorm = 47,

        /// <summary>
        /// A 128-bit, 4x4 block-compressed format storing 64 bits of unsigned normalized
        /// RGB data, as well as 64 bits of alpha data.
        /// </summary>
        ETC2_R8_G8_B8_A8_UNorm = 48,

        /// <summary>
        /// BC4 block compressed format, unsigned normalized values.
        /// </summary>
        BC4_UNorm = 49,

        /// <summary>
        /// BC4 block compressed format, signed normalized values.
        /// </summary>
        BC4_SNorm = 50,

        /// <summary>
        /// BC5 block compressed format, unsigned normalized values.
        /// </summary>
        BC5_UNorm = 51,

        /// <summary>
        /// BC5 block compressed format, signed normalized values.
        /// </summary>
        BC5_SNorm = 52,

        /// <summary>
        /// BC7 block compressed format.
        /// </summary>
        BC7_UNorm = 53,

        /// <summary>
        /// RGBA component order. Each component is an 8-bit unsigned normalized integer.
        /// This is an sRGB format.
        /// </summary>
        R8_G8_B8_A8_UNorm_SRgb = 54,

        /// <summary>
        /// BGRA component order. Each component is an 8-bit unsigned normalized integer.
        /// This is an sRGB format.
        /// </summary>
        B8_G8_R8_A8_UNorm_SRgb = 55,

        /// <summary>
        /// BC1 block compressed format with no alpha. This is an sRGB format.
        /// </summary>
        BC1_Rgb_UNorm_SRgb = 56,

        /// <summary>
        /// BC1 block compressed format with a single-bit alpha channel. This is an sRGB
        /// format.
        /// </summary>
        BC1_Rgba_UNorm_SRgb = 57,

        /// <summary>
        /// BC2 block compressed format. This is an sRGB format.
        /// </summary>
        BC2_UNorm_SRgb = 58,

        /// <summary>
        /// BC3 block compressed format. This is an sRGB format.
        /// </summary>
        BC3_UNorm_SRgb = 59,

        /// <summary>
        /// BC7 block compressed format. This is an sRGB format.
        /// </summary>
        BC7_UNorm_SRgb = 60
    }
}