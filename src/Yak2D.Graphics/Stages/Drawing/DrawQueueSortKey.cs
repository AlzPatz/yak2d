using System;
using System.Runtime.CompilerServices;

namespace Yak2D.Graphics
{
    // Packed, contiguous sort key for a single draw request. Comparing four ulongs from one
    // cache line is much faster than indexing seven separate per-property arrays per comparison.
    //
    // Word order defines priority (high to low):
    //   A: Layer (asc) | Depth (desc)
    //   B: Texture0
    //   C: Texture1
    //   D: FillType | TexMode0 | TexMode1 | submission index
    //
    // The submission index in the lowest bits of D makes every key unique, so the order is a
    // total order: any sort algorithm (including unstable introsort) then produces exactly the
    // result a stable sort would, keeping requests that tie on every property in submission order
    public readonly struct DrawQueueSortKey : IComparable<DrawQueueSortKey>
    {
        private readonly ulong _a;
        private readonly ulong _b;
        private readonly ulong _c;
        private readonly ulong _d;

        public int Index => (int)(uint)_d;

        public DrawQueueSortKey(bool includeLayerAndDepth,
                                int layer,
                                float depth,
                                ulong texture0,
                                ulong texture1,
                                FillType type,
                                TextureCoordinateMode texMode0,
                                TextureCoordinateMode texMode1,
                                int index)
        {
            _a = includeLayerAndDepth ? ((ulong)OrderedInt(layer) << 32) | ~OrderedFloat(depth) : 0UL;
            _b = texture0;
            _c = texture1;
            _d = ((ulong)(byte)type << 48) |
                 ((ulong)(byte)texMode0 << 40) |
                 ((ulong)(byte)texMode1 << 32) |
                 (uint)index;
        }

        // Maps int to uint preserving order (flip sign bit)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint OrderedInt(int value) => (uint)value ^ 0x80000000u;

        // Maps float to uint preserving order (IEEE 754 trick). Adding 0f folds -0 into +0 so
        // they compare equal, matching float.CompareTo
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint OrderedFloat(float value)
        {
            var bits = BitConverter.SingleToUInt32Bits(value + 0f);
            return (bits & 0x80000000u) != 0 ? ~bits : bits | 0x80000000u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(DrawQueueSortKey other)
        {
            if (_a != other._a) return _a < other._a ? -1 : 1;
            if (_b != other._b) return _b < other._b ? -1 : 1;
            if (_c != other._c) return _c < other._c ? -1 : 1;
            if (_d != other._d) return _d < other._d ? -1 : 1;
            return 0;
        }
    }
}
