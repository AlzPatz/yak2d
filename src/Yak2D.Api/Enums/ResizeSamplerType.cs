namespace Yak2D
{
    /// <summary>
    /// Sampling method used when re-sizing / re-sampling an image during bloom and blur stages
    /// </summary>
    public enum ResizeSamplerType
    {
        /// <summary>
        /// Samples just the nearest source pixel
        /// </summary>
        NearestNeighbour,

        /// <summary>
        /// Samples and averages 4 source pixels around the theoretical pixel position
        /// </summary>
        Average2x2,

        /// <summary>
        /// Samples and averages 8 source pixels around the theoretical pixel position
        /// </summary>
        Average4x4
    }
}