namespace Yak2D
{
    /// <summary>
    /// Dual texturing descriptor for use in DrawingHelpers fluent draw interface
    /// </summary>
    public enum TextureMixDirection
    {
        /// <summary>
        /// Mix Textures from left (T0) to right (T1) 
        /// </summary>
        Horizontal,

        /// <summary>
        /// Mix Textures from top (T0) to bottom (T1) 
        /// </summary>
        Vertical,

        /// <summary>
        /// Mix Textures from middle (inside) (T0) to outside (T1) 
        /// </summary>
        Radial
    }
}