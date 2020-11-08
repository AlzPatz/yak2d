using System;
using System.Linq;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class Fonts : IFonts
    {
        //Passes through to font manager, does some input guarding
        //Internally used Font Manager could be made to implement IFonts
        //But keeping it seperate for now

        private IFontManager _fontManager;

        public int UserFontCount { get { return _fontManager.UserFontCount; } }

        public Fonts(IFontManager fontManager)
        {
            _fontManager = fontManager;
        }

        public IFont LoadFont(string path, AssetSourceEnum assetType)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new Yak2DException("Fonts -> Load font passed null or empty path", new ArgumentNullException("path"));
            }

            var trimmedPath = path.Trim();

            if (trimmedPath.Any(char.IsWhiteSpace))
            {
                throw new Yak2DException("Fonts -> Asset path name cannot contain any whitespace", new ArgumentNullException("path"));
            }

            return _fontManager.LoadUserFont(trimmedPath, assetType);
        }

        public void DestroyFont(IFont font)
        {
            if (font != null)
            {
                _fontManager.DestroyFont(font.Id);
            }
        }

        public void DestroyFont(ulong font)
        {
            _fontManager.DestroyFont(font);
        }

        public void DestroyAllUserFonts()
        {
            _fontManager.DestroyAllUserFonts(false);
        }

        public float MeasureStringLength(string text, float fontSize, IFont font = null)
        {
            return _fontManager.MeasureStringLength(text, fontSize, font);
        }

        public float MeasureStringLength(string text, float fontSize, ulong font)
        {
            return _fontManager.MeasureStringLength(text, fontSize, font);
        }
    }
}
