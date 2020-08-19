namespace Yak2D
{
    /// <summary>
    /// Labels defining points on a sinusoidal curve
    /// </summary>
    public enum SineWavePoint
    {
        /// <summary>
        /// sin(0 degress)
        /// </summary>
        ZeroAscending,

        /// <summary>
        /// sin(180 degrees)
        /// </summary>
        PiZeroDescending,

        /// <summary>
        /// sin(90 degrees)
        /// </summary>
        HalfPiMaxDescending,

        /// <summary>
        /// sin(270 degrees)
        /// </summary>
        OneHalfPiMaxAscending
    }
}