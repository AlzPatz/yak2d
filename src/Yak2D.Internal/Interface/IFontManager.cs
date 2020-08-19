namespace Yak2D.Internal
{
    public interface IFontManager
    {
        int UserFontCount { get; }
        IFontModel SystemFont { get; }
        IFontModel RetrieveFont(ulong id);

        IFont LoadUserFont(string fontName, AssetSourceEnum assetType);

        void DestroyFont(ulong id);
        void DestroyAllUserFonts(bool resourcesAlreadyDestroyed);
        void Shutdown();
        void ReInitialise();
    }
}