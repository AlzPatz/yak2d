using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PixellateFactors
    {
        [FieldOffset(0)]
        public float PixAmount;
        [FieldOffset(4)]
        public int NumXDivisions;
        [FieldOffset(8)]
        public int NumYDivisions;
        [FieldOffset(12)]
        public int Pad0;
        [FieldOffset(16)]
        public Vector2 TexelSize;
        [FieldOffset(24)]
        public Vector2 Pad1;

        public const uint SizeInBytes = 32;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct EdgeDetectionFactors
    {
        [FieldOffset(0)]
        public int DetectEdges;
        [FieldOffset(4)]
        public int IsFreichen;
        [FieldOffset(8)]
        public float EdgeAmount;
        [FieldOffset(16)]
        public Vector4 Pad2;

        public const uint SizeInBytes = 32;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct StaticFactors
    {
        [FieldOffset(0)]
        public float StaticAmount;
        [FieldOffset(4)]
        public float Time;
        [FieldOffset(8)]
        public int IgnoreTransparent;
        [FieldOffset(12)]
        public float TexelScaler;
        [FieldOffset(16)]
        public Vector4 Pad3;
        
        public const uint SizeInBytes = 32;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct OldMovieFactors
    {
        [FieldOffset(0)]
        public float OldMovieAmount;
        [FieldOffset(4)]
        public float Scratch;
        [FieldOffset(8)]
        public float Noise;
        [FieldOffset(12)]
        public float RndLine1;
        [FieldOffset(16)]
        public float RndLine2;
        [FieldOffset(20)]
        public float Frame;
        [FieldOffset(24)]
        public float CpuShift;
        [FieldOffset(28)]
        public float RndShiftCutOff;
        [FieldOffset(32)]
        public float RndShiftScalar;
        [FieldOffset(36)]
        public float Dim;
        [FieldOffset(40)]
        public Vector2 Pad4;
        [FieldOffset(48)]
        public Vector4 OverExposureColour;

        public const uint SizeInBytes = 64;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CrtEffectFactors
    {
        [FieldOffset(0)]
        public float RgbFilterIntensity;
        [FieldOffset(4)]
        public float RgbFilterAmount;
        [FieldOffset(8)]
        public int NumRgbFiltersHorizontal;
        [FieldOffset(12)]
        public int NumRgbFiltersVertical;
        [FieldOffset(16)]
        public float ScanLineAmount;
        [FieldOffset(20)]
        public float Pad5;
        [FieldOffset(24)]
        public float Pad6;
        [FieldOffset(28)]
        public float Pad7;

        public const uint SizeInBytes = 32;
    }
}