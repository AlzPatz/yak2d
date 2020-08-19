using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MixingShaderFactors
    {
        [FieldOffset(0)]
        public float MixAmount;
        [FieldOffset(4)]
        public float Pad0;
        [FieldOffset(8)]
        public float Pad1;
        [FieldOffset(12)]
        public float Pad2;
        [FieldOffset(16)]
        public Vector4 Pad3;

        public const uint SizeInBytes = 32;
    }
}