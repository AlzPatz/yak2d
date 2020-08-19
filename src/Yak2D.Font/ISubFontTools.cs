using System;
using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public interface ISubFontTools
    {
        List<string> ExtractPngFilePathsFromDotFntLines(List<string> lines);
        float? ExtractNamedFloatFromLine(string name, string line);
        int? ExtractNamedIntFromLine(string name, string line);
        bool LineStartsWith(string name, string line);
        Tuple<int, FontCharacter> ExtractCharacterFromLine(string line, int pageWidth, int pageHeight);
        Tuple<char?, char, short> ExtractKerningFromLine(string line);
    }
}