using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Holds Texture Dimensions and RGBA Colour Data
    /// </summary>
    public struct TextureDataRgba
    {
        /// <summary>
        /// Texture Width in Pixels
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Texture Height in Pixels
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// RGBA Texture Pixel Data as normalised four component floats
        /// </summary>
        public Vector4[] Pixels { get; set; }
    }
}