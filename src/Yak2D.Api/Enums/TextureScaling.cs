namespace Yak2D
{
    /// <summary>
    /// Texture scaling / repetition descriptor for use in DrawingHelpers fluent draw interface
    /// </summary>
    public enum TextureScaling
    {
        /// <summary>
        /// Stretch Texture over the entire shape
        /// </summary>
        Stretch,

        /// <summary>
        /// Texture pixel size is maintained relative to shape
        /// Tiling occurs if shape larger than texture
        /// Tiling occurs relative to the centre of the shape (middle of shape == middle of texture)
        /// </summary>
        Tiled
    }
}