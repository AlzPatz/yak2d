namespace Yak2D
{
    /// <summary>
    /// Simple colour modification effects
    /// Mulitple effects can be combined, the result of one effect is fed into the input source of the next
    /// The ordering is: [Clear Surface Background?] -> Single Colour -> GrayScale -> Colourise -> Negative -> Opacity
    /// </summary>
    public struct ColourEffectConfiguration
    {
        /// <summary>
        /// Mix source with a single colour (0 to 1), 1 being entirely the chosen colour
        /// </summary>
        public float SingleColour { get; set; }

        /// <summary>
        /// Mix source with grayscaled image (0 to 1). 1 being entirely grayscale
        /// </summary>
        public float GrayScale { get; set; }

        /// <summary>
        /// Mix source with colourised image (0 to 1). 1 being entirely colourised
        /// </summary>
        public float Colourise { get; set; }

        /// <summary>
        /// Mix source with negative image (0 to 1). 1 being entirely negative
        /// </summary>
        public float Negative { get; set; }

        /// <summary>
        /// Multiply all RGBA components of colour by opacity value (0 to 1)
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Selected colour to mix in single colour and colourise effects
        /// </summary>
        public Colour ColourForSingleColourAndColourise { get; set; }

        /// <summary>
        /// If the user elects to clear the desination surface being rendering, this is the colour used
        /// </summary>
        public Colour BackgroundClearColour { get; set; }

        /// <summary>
        /// Destination surface can be cleared with a chosen colour before rendering if desired
        /// </summary>
        public bool ClearBackground { get; set; }
    }
}