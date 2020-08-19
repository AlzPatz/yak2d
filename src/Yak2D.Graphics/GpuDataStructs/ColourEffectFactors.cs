using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ColourEffectFactors
    {
        [FieldOffset(0)]
        public float SingleColourAmount;
        [FieldOffset(4)]
        public float GrayScaleAmount;
        [FieldOffset(8)]
        public float ColouriseAmount;
        [FieldOffset(12)]
        public float NegativeAmount;
        [FieldOffset(16)]
        public Vector4 Colour;
        [FieldOffset(32)]
        public float Opacity;
        [FieldOffset(36)]
        public float Pad0;
        [FieldOffset(40)]
        public float Pad1;
        [FieldOffset(44)]
        public float Pad2;
        [FieldOffset(48)]
        public Vector4 Pad3;

        public const uint SizeInBytes = 64;
    }
}