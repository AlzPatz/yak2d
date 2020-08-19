using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Vertex data used in 3D mesh rendering operations
    /// </summary>
    public struct Vertex3D
    {
        /// <summary>
        /// X, Y, Z
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Used for lighting calculations
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// Texture Mapping coordinates
        /// </summary>
        public Vector2 TexCoord { get; set; }

        public const uint SizeInBytes = 32;
    }
}