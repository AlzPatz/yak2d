using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yak2D.Internal;

namespace Yak2D.Font
{
    public class SubFontTools : ISubFontTools
    {
        private IFrameworkMessenger _frameworkMessenger;

        public SubFontTools(IFrameworkMessenger frameworkMessenger)
        {
            _frameworkMessenger = frameworkMessenger;

        }

        public List<string> ExtractPngFilePathsFromDotFntLines(List<string> fntFileLines)
        {
            if(fntFileLines == null)
            {
                return new List<string>();
            }

            Regex rx = new Regex("file=\"[^\" ]*[.]png\"");

            var matches = new List<string>();

            fntFileLines.ForEach(line =>
            {
                var match = rx.Match(line);

                if (match.Success)
                {
                    IEnumerable<Group> groups = (IEnumerable<Group>)match.Groups;
                    groups.ToList().ForEach(x =>
                    {
                        matches.Add(x.Value);
                    });
                }
            });

            if (matches.Count == 0)
            {
                _frameworkMessenger.Report(string.Concat("Unable to find any .png files for a sub font load, within a font"));
                return new List<string>() { };
            }

            //Extract file name between quotation marks
            var pngFileNames = matches.Select(x => ExtractPngFileNameBetweenQuotationMarks(x)).Where(x => !string.IsNullOrEmpty(x)).ToList();

            if (pngFileNames.Count == 0)
            {
                _frameworkMessenger.Report("Unable to find any .png filenames from regex matches for a sub font load, within a font");
                return new List<string>() { };
            }

            if (pngFileNames.Count != matches.Count)
            {
                _frameworkMessenger.Report("Warning: finding less valid .png filenames when parsing regex matches in font");
            }

            return pngFileNames;
        }

        private string ExtractPngFileNameBetweenQuotationMarks(string input)
        {
            var splits = input.Split('\"');

            if (splits.Length < 2)
            {
                return null;
            }

            return splits[1];
        }


        public float? ExtractNamedFloatFromLine(string name, string line)
        {
            if (name == null || line == null)
            {
                return null;
            }

            //Assumes input is a list of key=value entries with white space between them

            var splits = line.Split(null);

            for (var s = 0; s < splits.Length; s++)
            {
                var part = splits[s];

                if (part.Contains("="))
                {
                    var subsplit = part.Split('=');

                    if (subsplit.Length != 2)
                    {
                        continue;
                    }

                    var key = subsplit[0];
                    var valuetext = subsplit[1];

                    if (key == name)
                    {
                        float value;
                        if (float.TryParse(valuetext, out value))
                        {
                            return value;
                        }
                    }
                }
            }

            return null;
        }

        public int? ExtractNamedIntFromLine(string name, string line)
        {
            if (name == null || line == null)
            {
                return null;
            }

            //Assumes input is a list of key=value entries with white space between them

            var splits = line.Split(null);

            for (var s = 0; s < splits.Length; s++)
            {
                var part = splits[s];

                if (part.Contains("="))
                {
                    var subsplit = part.Split('=');

                    if (subsplit.Length != 2)
                    {
                        continue;
                    }

                    var key = subsplit[0];
                    var valuetext = subsplit[1];

                    if (key == name)
                    {
                        int value;
                        if (int.TryParse(valuetext, out value))
                        {
                            return value;
                        }
                    }
                }
            }

            return null;
        }

        public bool LineStartsWith(string name, string line)
        {
            if (name == null || line == null)
            {
                return false;
            }

            return line.StartsWith(name);
        }

        public Tuple<int, FontCharacter> ExtractCharacterFromLine(string line, int pageWidth, int pageHeight)
        {
            //Input expected in form such as:
            //char id=32   x=128   y=75    width=3     height=1     xoffset=-1    yoffset=94    xadvance=14    page=0  chnl=15

            var id = ExtractNamedIntFromLine("id", line);
            var x = ExtractNamedIntFromLine("x", line);
            var y = ExtractNamedIntFromLine("y", line);
            var width = ExtractNamedIntFromLine("width", line);
            var height = ExtractNamedIntFromLine("height", line);
            var xoffset = ExtractNamedIntFromLine("xoffset", line);
            var yoffset = ExtractNamedIntFromLine("yoffset", line);
            var xadvance = ExtractNamedIntFromLine("xadvance", line);
            var page = ExtractNamedIntFromLine("page", line);

            if (id == null
                || x == null
                || y == null
                || width == null
                || height == null
                || xoffset == null
                || yoffset == null
                || xadvance == null
                || page == null)
            {
                return new Tuple<int, FontCharacter>(-1, null);
            }

            var character = new FontCharacter(pageWidth,
                                                pageHeight,
                                                (int)x,
                                                (int)y,
                                                (int)width,
                                                (int)height,
                                                (int)xoffset,
                                                (int)yoffset,
                                                (int)xadvance,
                                                (int)page);

            return new Tuple<int, FontCharacter>((int)id, character);
        }

        public Tuple<char?, char, short> ExtractKerningFromLine(string line)
        {
            var first = ExtractNamedIntFromLine("first", line);
            var second = ExtractNamedIntFromLine("second", line);
            var amount = ExtractNamedIntFromLine("amount", line);

            if (first == null
                || second == null
                || amount == null)
            {
                return new Tuple<char?, char, short>(null, (char)0, (short)0);
            }

            return new Tuple<char?, char, short>((char)first, (char)second, (short)amount);
        }
    }
}