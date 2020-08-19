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
        IFont LoadFont(string path, AssetSourceEnum assetType);

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
    }
}