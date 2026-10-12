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
        /// Destination[A] = Source[A] + (Destination[A] * (1.0 - Source[A]))
        /// On an opaque target the alpha stays 1.0. On a transparent render target the result holds
        /// premultiplied colour with the correct coverage in alpha, ready to be composited with Premultiplied
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
        AdditiveComponentWise,

        /// <summary>
        /// For source colours that are already multiplied by their alpha, such as a render target that was drawn
        /// into with Alpha blending. Use it to composite such a surface onto another without darkening its
        /// semi-transparent pixels
        /// Destination[RGB] = Source[RGB] + (Destination[RGB] * (1.0 - Source[A]))
        /// Destination[A] = Source[A] + (Destination[A] * (1.0 - Source[A]))
        /// </summary>
        Premultiplied
    }
}