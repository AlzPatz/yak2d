using System.Collections.Generic;

namespace Yak2D.Font
{
    public class CandidateFontDesc
    {
        public string Name { get; set; }
        public List<CandidateSubFontDesc> CandidateSubFonts { get; set; }
    }

    public class CandidateSubFontDesc
    {
        public List<string> DotFntLines { get; set; }
        public List<string> TexturePaths { get; set; }
        public List<ITexture> Textures { get; set; }
    }
}
