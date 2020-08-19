using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DownSampleFactors
    {
        [FieldOffset(0)]
        public Vector2 TexelSize;
        [FieldOffset(8)]
        public int NumberOfSamples;
        [FieldOffset(12)]
        public float Pad0;
        [FieldOffset(16)]
        public Vector4 Pad1;

        public const uint SizeInBytes = 32;
    }
}