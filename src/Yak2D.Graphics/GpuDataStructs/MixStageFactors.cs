using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MixStageFactors
    {
        [FieldOffset(0)]
        public Vector4 MixAmounts;

        public const uint SizeInBytes = 32;
    }
}