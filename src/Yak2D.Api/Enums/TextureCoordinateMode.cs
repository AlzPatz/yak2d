namespace Yak2D
{
    /// <summary>
    /// Texture Coordinate Wrap Behaviour
    /// Yak2D does not support border pixel or solid colour texture coordinate wrap
    /// </summary>
    public enum TextureCoordinateMode
    {
        /// <summary>
        /// Does not define any behaviour, represents null / no option only
        /// Note: Yak2D does not support border pixel or solid colour texture coordinate wrap
        /// </summary>
        None,

        /// <summary>
        /// Coordinates Wrap -> values over 1 wrap around to 0, values under 0 wrap around to 1
        /// </summary>
        Wrap,

        /// <summary>
        /// Coordinate Mirroring -> values over 1 start decreasing again to 0, values under 0 start increasing again towards 1
        /// </summary>
        Mirror
    }
}
