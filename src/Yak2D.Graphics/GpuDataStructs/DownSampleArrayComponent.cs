using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics 
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DownSampleArrayComponent
    {
        [FieldOffset(0)]
        public Vector2 Offset;
        [FieldOffset(8)]
        public float Weight;
        [FieldOffset(12)]
        public float Pad;

        public const int SizeInBytes = 16;
    }
}