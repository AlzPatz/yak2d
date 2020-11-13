using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Holds Texture Dimensions and Pixel Data
    /// </summary>
    public struct TextureData
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
        /// Texture Pixel Data as normalised four component RGBA floats when used for texture creation or reading RGBA texture data
        /// If used to pass Float32 Pixel data back from the GPU, then the first component is used only
        /// </summary>
        public Vector4[] Pixels { get; set; }
    }
}