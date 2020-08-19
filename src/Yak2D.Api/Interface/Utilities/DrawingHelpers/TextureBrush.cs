using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Descriptor object for the properties of a specific Texture use in a fluent interface drawable object
    /// The fluent drawing interface currently only supports texture references (not raw unsigned integer ids). Wrap ids if required
    /// </summary>
    public class TextureBrush
    {
        /// <summary>
        /// The Texture reference
        /// </summary>
        public ITexture Texture { get; set; }

        /// <summary>
        /// Texture Coordinate Wrap Behaviour
        /// Yak2D does not support border pixel or solid colour texture coordinate wrap
        /// </summary>
        public TextureCoordinateMode TextureMode { get; set; }

        /// <summary>
        /// Is the Texture tiled or stretched across the shape
        /// </summary>
        public TextureScaling TextureScaling { get; set; }

        /// <summary>
        /// If a texture is tiled, what is the ratio between a shape 'pixel' and a texture 'pixel' over distance
        /// </summary>
        public Vector2 TilingScale { get; set; }
    }
}