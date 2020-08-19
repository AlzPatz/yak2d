using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct KernelFactors
    {
        [FieldOffset(0)]
        public Matrix4x4 PixelShifts;
        [FieldOffset(64)]
        public Vector4 Weights;

        public const uint SizeInBytes = 128;
    }
}