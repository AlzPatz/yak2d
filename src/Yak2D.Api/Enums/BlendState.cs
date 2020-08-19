namespace Yak2D
{
    /// <summary>
    /// How pixel colours are combined when a source pixel is drawn onto a target pixel
    /// </summary>
    public enum BlendState
    {
        /// <summary>
        /// Destination pixel colour is overwritten with new source pixel
        /// </summary>
        Override,

        /// <summary>
        /// Common BlendState (default)
        /// Destination[RGB] = (Source[RGB] * Source[A]) + (Destination[RGB] * (1.0 - Source[A]))
        /// Desination[A] = Source[A] + (1.0 - Source[A]) == 1.0 Constant
        /// </summary>
        Alpha,

        /// <summary>
        /// Source pixel is added to destination based on the source alpha channel
        /// Desintation[RGB] = Destination[RGB] + (Source[RGB] * Source[A])
        /// Destination[A] = 1.0 + Source[A]
        /// </summary>
        AdditiveAlpha,

        /// <summary>
        /// Destination[RGBA] = Destination[RGBA] + Source[RGBA]
        /// </summary>
        AdditiveComponentWise
    }
}