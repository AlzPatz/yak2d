using System.Numerics;

namespace Yak2D.Graphics
{
    public struct VertexTextured2D
    {
        public Vector2 Position;
        public Vector2 TexCoord;

        public const uint SizeInBytes = 16;
    }
}
