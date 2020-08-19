using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GaussianBlurArrayComponent
    {
        [FieldOffset(0)]
        public float Offset;
        [FieldOffset(4)]
        public float Weight;
        [FieldOffset(8)]
        public Vector2 Pad;

        public const int SizeInBytes = 16;//maybe should be 32 so dont need even numbers? (TO TEST)
    }
}