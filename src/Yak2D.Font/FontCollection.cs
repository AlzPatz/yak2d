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

        public void Destroy(ulong id, bool resourcesAlreadyDestroyed)
        {
            if (!_fonts.ContainsKey(id))
            {
                _frameworkMessenger.Report("FontCollection: Unable to remove font, ulong DOES NOT exist in collection");
                return;
            }

            var font = _fonts[id];
            if (!resourcesAlreadyDestroyed)
            {
                ReleaseFontResources(font);
            }

            _fonts.Remove(id);
        }

        private void ReleaseFontResources(IFontModel font)
        {
            font?.ReleaseReources(_gpuSurfaceManager);
        }

        public List<ulong> ReturnAllIds()
        {
            return new List<ulong>(_fonts.Keys);
        }
    }
}