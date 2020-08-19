using System.Numerics;
using System.Runtime.InteropServices;

namespace Yak2D.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MeshVertexUniforms
    {
        [FieldOffset(0)]
        public Matrix4x4 WorldViewProjection;

        public const uint SizeInBytes = 64;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct FragUniforms
    {
        [FieldOffset(0)]
        public Vector4 CameraPosition; //We use 4 component rather than 3 component. As backends appear to have different strides for float3s
        [FieldOffset(16)]
        public Vector4 Pad0;

        public const uint SizeInBytes = 32;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct LightProperties
    {
        [FieldOffset(0)]
        public Vector4 SpecularColour; //We use 4 component rather than 3 component. As backends appear to have different strides for float3s
        [FieldOffset(16)]
        public float Shininess;
        [FieldOffset(20)]
        public int NumLights;
        [FieldOffset(24)]
        public Vector2 Pad1;

        public const uint SizeInBytes = 32;
    }

    public struct Light
    {
        public Vector4 Position; //fourth component dictates if directional or spotlight (w = 1 for spotlight)
        public Vector4 Colour; //ignore a
        public Vector4 ConeDirection; //ignore w
        public float Attenuation;
        public float AmbientCoefficient;
        public float ConeAngle;
        public float Pad;

        public const uint SizeInBytes = 64;
    }
}