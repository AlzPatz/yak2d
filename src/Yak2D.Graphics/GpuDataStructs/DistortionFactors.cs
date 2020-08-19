using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics 
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PixelSizeUniform
    {
        [FieldOffset(0)]
        public Vector2 PixelShift;
        [FieldOffset(8)]
        public Vector2 Pad0;
        [FieldOffset(16)]
        public Vector4 Pad1;

        public const uint SizeInBytes = 32;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DistortionFactorUniform
    {
        [FieldOffset(0)]
        public Vector2 DistortionScalar;
        [FieldOffset(8)]
        public Vector2 Pad2;
        [FieldOffset(16)]
        public Vector4 Pad3;

        public const uint SizeInBytes = 32;
    }
}