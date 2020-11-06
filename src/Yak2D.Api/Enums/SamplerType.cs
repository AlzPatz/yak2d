namespace Yak2D
{
    /// <summary>
    /// Texture Sampler Filter Type. A filter type is chosen at Gpu Surface creation time (Textures and RenderTargets)
    /// </summary>
    public enum SamplerType
    {
        /// <summary>
        /// Anisotropic Filtering used for Minification, Magnification and Mip Level Filtering. If not supported defaults to Linear Interpolation
        /// </summary>
        Anisotropic,

        /// <summary>
        /// Linear Interpolation used for Minification, Magnification and Mip Level Filtering
        /// </summary>
        Linear,

        /// <summary>
        /// Point Sampling used for Minification, Magnification and Mip Level Filtering
        /// </summary>
        Point
    }
}