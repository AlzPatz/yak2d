namespace Yak2D
{
    /// <summary>
	/// Font Operations: Load, Destroy and Count
	/// </summary>
    public interface IFonts
    {
        /// <summary>
        /// Returns the number of user fonts currently loaded
        /// </summary>
        int UserFontCount { get; }

        /// <summary>
        /// Loads user font from asset path
        /// </summary>
        /// <param name="path">Font asset name including path (no file extension)"</param>
        /// <param name="assetType">Embedded in the binary or a file location"</param>
        /// <param name="imageFormat">The image file encoding for the font textures"</param>
        IFont LoadFont(string path, AssetSourceEnum assetType, ImageFormat imageFormat =  ImageFormat.PNG);

        /// <summary>
        /// Destroy a user font
        /// </summary>
        /// <param name="font">The Font reference to destroy</param>
        void DestroyFont(IFont font);

        /// <summary>
        /// Destroy a user font
        /// </summary>
        /// <param name="font">The Font id to destroy</param>
        void DestroyFont(ulong font);

        /// <summary>
        /// Destroy all the user fonts
        /// </summary>
        void DestroyAllUserFonts();

        /// <summary>
        /// Measure pixel length (width) of a string
        /// </summary>
        /// <param name="text">Text string</param>
        /// <param name="fontSize">Size of font in pixels</param>
        /// <param name="font">Font reference to use, null uses a default font</param>
        float MeasureStringLength(string text,
                                  float fontSize,
                                  IFont font = null);

        /// <summary>
        /// Measure pixel length (width) of a string
        /// </summary>
        /// <param name="text">Text string</param>
        /// <param name="fontSize">Size of font in pixels</param>
        /// <param name="font">Font id to use</param>
        float MeasureStringLength(string text,
                                  float fontSize,
                                  ulong font);
    }
}