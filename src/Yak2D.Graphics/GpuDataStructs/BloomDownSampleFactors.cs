using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BloomDownSampleFactors
    {
        [FieldOffset(0)]
        public Vector2 TexelSize;
        [FieldOffset(8)]
        public int NumberOfSamples;
        [FieldOffset(12)]
        public float BrightnessThreshold;
        [FieldOffset(16)]
        public Vector4 Pad0;

        public const uint SizeInBytes = 32;
    }
}