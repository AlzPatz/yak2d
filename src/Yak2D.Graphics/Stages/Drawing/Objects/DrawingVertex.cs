using System.Numerics;
using Veldrid;

public struct DrawingVertex
{
    public int IsWorld;
    public int TexturingType;
    public Vector3 Position;
    public RgbaFloat Color;
    public Vector2 TexCoord0;
    public Vector2 TexCoord1;
    public float TexWeight0;
    public const uint SizeInBytes = 56;
}