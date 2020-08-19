using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class SubFontGenerator : ISubFontGenerator
    {
        private readonly ISubFontTools _subFontTools;
        private readonly IFrameworkMessenger _frameworkMessenger;

        public SubFontGenerator(IFrameworkMessenger frameworkMessenger,
                                ISubFontTools subFontTools)
        {
            _frameworkMessenger = frameworkMessenger;
            _subFontTools = subFontTools;
        }

        public List<string> ExtractPngFilePathsFromDotFntLines(List<string> lines) => _subFontTools.ExtractPngFilePathsFromDotFntLines(lines);

        public SubFont Generate(CandidateSubFontDesc description)
        {
            if (description == null)
            {
                _frameworkMessenger.Report("Error loading a sub font, dsecription object is null");
                return null;
            }

            if (description.Textures == null || description.Textures.Count == 0)
            {
                _frameworkMessenger.Report("Error loading a sub font, dsecription textures array is null or zero length");
                return null;
            }

            if (description.TexturePaths == null || description.TexturePaths.Count == 0)
            {
                _frameworkMessenger.Report("Error loading a sub font, dsecription texture paths array is null or zero length");
                return null;
            }

            var lines = description.DotFntLines;

            if (lines == null)
            {
                _frameworkMessenger.Report("Error loading a sub font, .fnt file line data is null");
                return null;
            }

            if (lines.Count < 3)
            {
                _frameworkMessenger.Report("Error loading a sub font, less than three lines in .fnt data");
                return null;
            }

            //Pull font size from the first line
            var fontSize = ExtractNamedFloatFromLineWriteToDebugOnFail("size", lines[0]);
            if (fontSize == null) { return null; }

            //Pull data from the second line
            var lineHeight = ExtractNamedIntFromLineWriteToDebugOnFail("lineHeight", lines[1]);
            var pageWidth = ExtractNamedIntFromLineWriteToDebugOnFail("scaleW", lines[1]);
            var pageHeight = ExtractNamedIntFromLineWriteToDebugOnFail("scaleH", lines[1]);
            var numPages = ExtractNamedIntFromLineWriteToDebugOnFail("pages", lines[1]);

            if ((lineHeight == null) || (pageWidth == null) || (pageHeight == null) || (numPages == null))
            {
                return null;
            }

            var numberOfCharactersExpected = 0;
            var characters = new Dictionary<char, FontCharacter>();

            var numberOfKerningsExpected = 0;
            var kernings = new Dictionary<char, Dictionary<char, short>>();
            var numberOfKerningsExtracted = 0;

            for (var l = 2; l < lines.Count; l++)
            {
                var line = lines[l];

                if (_subFontTools.LineStartsWith("char ", line))
                {
                    var charExtracted = _subFontTools.ExtractCharacterFromLine(line, (int)pageWidth, (int)pageHeight);
                    if (charExtracted.Item2 == null)
                    {
                        _frameworkMessenger.Report("Error loading a sub font, a character line failed to pass. Continuing without");
                        continue;
                    }
                    characters.Add((char)charExtracted.Item1, charExtracted.Item2);
                    continue;
                }

                if (_subFontTools.LineStartsWith("kerning ", line))
                {
                    var kerningExtracted = _subFontTools.ExtractKerningFromLine(line);
                    //Item1 = first character (nullable), Item2 = second character, Item3 = kerning amount
                    if (kerningExtracted.Item1 == null)
                    {
                        _frameworkMessenger.Report("Error loading a sub font, a kerning line failed to pass. Continuing without");
                        continue;
                    }
                    if (kernings.ContainsKey((char)kerningExtracted.Item1))
                    {
                        var dictionaryOfCharacter = kernings[(char)kerningExtracted.Item1];

                        if (dictionaryOfCharacter.ContainsKey(kerningExtracted.Item2))
                        {
                            _frameworkMessenger.Report("Kerning dictionary already contains entry. Continuing without");
                            continue;
                        }

                        dictionaryOfCharacter.Add(kerningExtracted.Item2, kerningExtracted.Item3);
                        numberOfKerningsExtracted++;
                    }
                    else
                    {
                        var newDictionaryForCharacter = new Dictionary<char, short>();
                        newDictionaryForCharacter.Add(kerningExtracted.Item2, kerningExtracted.Item3);
                        kernings.Add((char)kerningExtracted.Item1, newDictionaryForCharacter);
                        numberOfKerningsExtracted++;
                    }
                    continue;
                }

                if (_subFontTools.LineStartsWith("chars", line))
                {
                    var num = _subFontTools.ExtractNamedIntFromLine("count", line);
                    if (num == null)
                    {
                        _frameworkMessenger.Report("Error loading a sub font, number of characters failed to extract from .fnt file");
                        return null;
                    }
                    numberOfCharactersExpected = (int)num;
                    continue;
                }

                if (_subFontTools.LineStartsWith("kernings", line))
                {
                    var num = _subFontTools.ExtractNamedIntFromLine("count", line);
                    if (num == null)
                    {
                        _frameworkMessenger.Report("Error loading a sub font, number of kernings failed to extract from .fnt file");
                        return null;
                    }
                    numberOfKerningsExpected = (int)num;
                    continue;
                }
            }

            if (characters.Count != numberOfCharactersExpected)
            {
                _frameworkMessenger.Report("Warning loading a sub font, the number of characters extracted does not match expected");
            }

            if (numberOfKerningsExtracted != numberOfKerningsExpected)
            {
                _frameworkMessenger.Report("Warning loading a sub font, the number of kernings extracted does not match expected");
            }

            //Fixing Space Character
            if (!characters.ContainsKey((char)32))
            {
                if (characters.ContainsKey((char)0) && characters[(char)0].XAdvance > 0)
                {
                    var zero = characters[(char)0];
                    characters.Add((char)32, zero);
                }
                else
                {
                    characters.Add((char)32, new FontCharacter((int)pageWidth, (int)pageHeight, 0, 0, 0, 0, 0, 0, (int)fontSize / 3, 0));
                }
            }

            if (characters.Count == 0)
            {
                _frameworkMessenger.Report("Error: No characters were loaded for subFont, returning null");
                return null;
            }

            //Note: The texture list is pulled out from the .fnt file (and textures loaded) before this function so we do not extract names here
            var subFont = new SubFont((float)fontSize,
                                      (float)lineHeight,
                                      characters,
                                      description.Textures.ToArray(),
                                      numberOfKerningsExtracted > 0,
                                      kernings);
            return subFont;
        }

        private float? ExtractNamedFloatFromLineWriteToDebugOnFail(string name, string line)
        {
            var value = _subFontTools.ExtractNamedFloatFromLine(name, line);
            if (value == null)
            {
                _frameworkMessenger.Report(string.Concat("Error loading a sub font, unable to extract: ", name, "from line of sub font description"));
            }
            return value;
        }

        private int? ExtractNamedIntFromLineWriteToDebugOnFail(string name, string line)
        {
            var value = _subFontTools.ExtractNamedIntFromLine(name, line);
            if (value == null)
            {
                _frameworkMessenger.Report(string.Concat("Error loading a sub font, unable to extract: ", name, "from line of sub font description"));
            }
            return value;
        }
    }
}