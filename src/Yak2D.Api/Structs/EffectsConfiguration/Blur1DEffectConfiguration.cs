using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// The directional blur effect downsamples and then blurs the image along a direction 
    /// </summary>
    public struct Blur1DEffectConfiguration
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
        /// Direction Vector of 1 dimensional blur
        /// </summary>
        public Vector2 BlurDirection { get; set; }

        /// <summary>
        /// Pixel sampling method (nearest neighbour, 2x2, 4x4)
        /// </summary>
        public ResizeSamplerType ReSamplerType { get; set; }
    }
}