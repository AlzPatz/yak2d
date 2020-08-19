using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class FontCollection : IFontCollection
    {
        public int Count { get { return _fonts.Count; } }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;

        private Dictionary<ulong, IFontModel> _fonts;

        public FontCollection(IFrameworkMessenger frameworkMessenger,
                                                IGpuSurfaceManager gpuSurfaceManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _gpuSurfaceManager = gpuSurfaceManager;
            _fonts = new Dictionary<ulong, IFontModel>();
        }

        public bool Add(ulong id, IFontModel font)
        {
            if (_fonts.ContainsKey(id))
            {
                _frameworkMessenger.Report("FontCollection: Unable to add font, ulong exists in collection");
                return false;
            }

            _fonts.Add(id, font);
            return true;
        }

        public IFontModel Retrieve(ulong id)
        {
            IFontModel font;
            if (_fonts.TryGetValue(id, out font)) //TRYGET used as potential optimisation to check CONTAINS and then DICTIONARY LOOK UP (although surely compiler optimises for that anyway...)
            {
                return font;
            }
            else
            {
                _frameworkMessenger.Report("FontCollection: Unable to retrieve font as ulong does not exist in collection");
                return null;
            }
        }

        public bool Destroy(ulong id)
        {
            if (!_fonts.ContainsKey(id))
            {
                _frameworkMessenger.Report("FontCollection: Unable to remove font, ulong DOES NOT exist in collection");
                return false;
            }

            var font = _fonts[id];
            ReleaseFontResources(font);

            _fonts.Remove(id);
            return true;
        }

        public void DestroyAll(bool resourcesAlreadyDestroyed)
        {
            if (!resourcesAlreadyDestroyed)
            {
                foreach (var font in _fonts)
                {
                    ReleaseFontResources(font.Value);
                }
            }
            _fonts.Clear();
        }

        private void ReleaseFontResources(IFontModel font)
        {
            font?.ReleaseReources(_gpuSurfaceManager);
        }
    }
}