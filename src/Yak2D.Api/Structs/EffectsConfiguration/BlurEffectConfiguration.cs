namespace Yak2D
{
    /// <summary>
    /// The blur effect downsamples and then blurs the image in both horizontal and vertical directions
    /// </summary>
    public struct BlurEffectConfiguration
    {
        /// <summary>
        /// Fraction of result that is blur result versus original source (0 to 1)
        /// </summary>
        public float MixAmount { get; set; }

        /// <summary>
        /// Number of blur samples (MAX 8, will be capped)
        /// </summary>
        public int NumberOfBlurSamples { get; set; }

        /// <summary>
        /// Pixel sampling method (nearest neighbour, 2x2, 4x4)
        /// </summary>
        public ResizeSamplerType ReSamplerType { get; set; }
    }
}