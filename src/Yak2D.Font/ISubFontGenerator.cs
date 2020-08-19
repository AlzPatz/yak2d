using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public interface ISubFontGenerator
    {
        List<string> ExtractPngFilePathsFromDotFntLines(List<string> lines);
        SubFont Generate(CandidateSubFontDesc desc);
    }
}
