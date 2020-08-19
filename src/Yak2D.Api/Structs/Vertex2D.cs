using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Vertex data used in 2D drawing operations
    /// </summary>
    public struct Vertex2D
    {
        /// <summary>
        /// X, Y position
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Texture Coordinates for the primary texture reference
        /// </summary>
        public Vector2 TexCoord0 { get; set; }

        /// <summary>
        /// Texture Coordinates for the secondary texture reference
        /// </summary>
        public Vector2 TexCoord1 { get; set; }

        /// <summary>
        /// Weighting of primary texture in dual texturing scenarios (0 to 1)
        /// Secondary texture gets weighting 1 - value
        /// </summary>
        public float TexWeighting { get; set; }

        /// <summary>
        /// Additional mixing colour for vertex 
        /// </summary>
        public Colour Colour { get; set; }
    }
}