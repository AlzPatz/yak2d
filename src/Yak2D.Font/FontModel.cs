using System;
using System.Collections.Generic;
using System.Linq;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class FontModel : IFontModel
    {
        private readonly int _numSubFonts;
        private readonly SubFont[] _subFonts;

        public FontModel(List<SubFont> subFonts)
        {
            if (subFonts == null || subFonts.Count == 0)
            {
                throw new Yak2DException("Internal Framework Error: Font creation was passed null or zero length subfont array", new ArgumentException("null or zero length array", "subFonts"));
            }

            _numSubFonts = subFonts.Count;
            _subFonts = subFonts.OrderBy(x => x.Size).ToArray();
        }

        public SubFont SubFontAtSize(float size)
        {
            if (_numSubFonts == 0)
            {
                return null;
            }

            //Defensive, as agnostic to ordering
            //Certainly not the most efficient algorithm if can assume the subfonts are in size order (as they should be)

            var closestLarger = -1;
            var closestSmaller = -1;

            for (var n = 0; n < _numSubFonts; n++)
            {
                var sbSize = _subFonts[n].Size;

                if (sbSize <= size)
                {
                    if (closestSmaller == -1)
                    {
                        closestSmaller = n;
                    }
                    else
                    {
                        if ((size - sbSize) < (size - _subFonts[closestSmaller].Size))
                        {
                            closestSmaller = n;
                        }
                    }
                }

                if (sbSize >= size)
                {
                    if (closestLarger == -1)
                    {
                        closestLarger = n;
                    }
                    else
                    {
                        if ((sbSize - size) < (_subFonts[closestLarger].Size - size))
                        {
                            closestSmaller = n;
                        }
                    }
                }
            }

            if (closestSmaller == -1)
            {
                return _subFonts[closestLarger];
            }

            if (closestLarger == -1)
            {
                return _subFonts[closestSmaller];
            }

            return _subFonts[closestLarger];
        }

        public void ReleaseReources(IGpuSurfaceManager gpuSurfaceManager)
        {
            if (gpuSurfaceManager == null)
            {
                throw new Yak2DException("Internal Framework Error: Font release resources was passed a null gpuSurfaceManager object");
            }

            //Not called if overall reinit (where all resources destroyed at the start (font collection level bool toggle))
            for (var n = 0; n < _numSubFonts; n++)
            {
                var subFont = _subFonts[n];

                subFont.Characters.Clear();
                subFont.Kernings.Clear();
                foreach (var tex in subFont.Textures)
                {
                    gpuSurfaceManager.DestroySurface(tex.Id);
                }
            }
        }
    }
}