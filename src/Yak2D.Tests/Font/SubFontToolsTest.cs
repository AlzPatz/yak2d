using System.Collections.Generic;
using NSubstitute;
using Xunit;
using Yak2D.Font;

namespace Yak2D.Tests
{
    public class SubFontToolsTest
    {
        [Fact]
        public void SubFontTools_TestExtractPngFilePathsFromLineList_ReturnFilenames()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var lines = new List<string>
            {
                "nothing",
                "dirty text 23123&8734///",
                "page id=0 file=\"minstrel_96_0.png\"",
                "page id=1 file=\"minstrel_96_1.png\"",
                "page id=2 file=\"minstrel_96_2.png\"",
                "page id=3 file=\"minstrel_96_3.png\"",
                "page id=4 file=\"minstrel_96_4.png\"",
                "page id=5 file=\"minstrel_96_5.png\"",
                "page id=6 file=\"minstrel_96_6.png\"",
                "page id=7 file=\"minstrel_96_7.png\"",
                "extra poop",
                "how can this be broken?? png ?? ."
            };

            var result = tools.ExtractPngFilePathsFromDotFntLines(lines);

            Assert.Equal(8, result.Count);
            Assert.Equal("minstrel_96_0.png", result[0]);
            Assert.Equal("minstrel_96_1.png", result[1]);
            Assert.Equal("minstrel_96_2.png", result[2]);
            Assert.Equal("minstrel_96_3.png", result[3]);
            Assert.Equal("minstrel_96_4.png", result[4]);
            Assert.Equal("minstrel_96_5.png", result[5]);
            Assert.Equal("minstrel_96_6.png", result[6]);
            Assert.Equal("minstrel_96_7.png", result[7]);
        }

        [Fact]
        public void SubFontTools_TestExtractPngFilePathsFromLineListNoMatches_ReturnZeroSizedArray()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var lines = new List<string>
            {
                "nothing",
                "dirty text 23123&8734///",
                "extra poop",
                "how can this be broken?? png ?? ."
            };

            var result = tools.ExtractPngFilePathsFromDotFntLines(lines);

            Assert.Empty(result);
        }

        [Fact]
        public void SubFontTools_TestExtractPngFilePathsFromLineListNullInput_ReturnsZeroSizedArray()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var result = tools.ExtractPngFilePathsFromDotFntLines(null);

            Assert.Empty(result);
        }

        [Fact]
        public void SubFontTools_ExtractNamedFloatFromLine_ReturnCorrectValue()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var name = "happiness";
            var line = "what s      @ ?? .. 98TH happ i n ess ..kk /* happiness=205.3 _ whatever ";

            var result = tools.ExtractNamedFloatFromLine(name, line);

            Assert.Equal(205.3f, (float)result);
        }

        [Fact]
        public void SubFontTools_ExtractNamedFloatFromLine_ReturnNullFromNullInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);


            var result = tools.ExtractNamedFloatFromLine(null, "test");
            Assert.Null(result);

            result = tools.ExtractNamedFloatFromLine("test", null);
            Assert.Null(result);

            result = tools.ExtractNamedFloatFromLine(null, null);
            Assert.Null(result);
        }

        [Fact]
        public void SubFontTools_ExtractNamedIntFromLine_ReturnCorrectValue()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var name = "anger";
            var line = "what s      @ ?? .. 98TH happ i n ess ..kk /* anger=205 _ whatever ";

            var result = tools.ExtractNamedIntFromLine(name, line);

            Assert.Equal(205f, (int)result);
        }

        [Fact]
        public void SubFontTools_ExtractNamedIntFromLine_ReturnNullFromNullInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var result = tools.ExtractNamedIntFromLine(null, "test");
            Assert.Null(result);

            result = tools.ExtractNamedIntFromLine("test", null);
            Assert.Null(result);

            result = tools.ExtractNamedIntFromLine(null, null);
            Assert.Null(result);
        }

        [Fact]
        public void SubFontTools_LineStartsWith_CorrectlyReturnTrue()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            Assert.True(tools.LineStartsWith("start", "start of the fun cirus"));
        }

        [Fact]
        public void SubFontTools_LineStartsWith_CorrectlyReturnFalse()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            Assert.False(tools.LineStartsWith("start", "end of the fun cirus"));
        }

        [Fact]
        public void SubFontTools_LineStartsWith_ReturnsFalseOnNullInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            Assert.False(tools.LineStartsWith("what", null));
            Assert.False(tools.LineStartsWith(null, "where"));
            Assert.False(tools.LineStartsWith(null, null));
        }


        [Theory]
        [InlineData("char id=32 x=128 y=75 width=3 height=1 xoffset=-1 yoffset=94 xadvance=94 page=0 chnl=15",
                    100, 100, 32, 1.28f, 1.31f, 0.75f, 0.76f, -1.0f, 94.0f, 3.0f, 1.0f, 94.0f, 0)]
        [InlineData("char   id=32 sd aasd x=128 *778sad y=75 akja dirty width=3 xxxxxx height=1 ___ === xoffset=-1 =  yoffset=94 sad  xadvance=94 pp page=0 xx chnl=15",
                    100, 100, 32, 1.28f, 1.31f, 0.75f, 0.76f, -1.0f, 94.0f, 3.0f, 1.0f, 94.0f, 0)]
        public void SubFontTools_ExtractCharacterFromLine_CorrectlyExtractsCharacters(string line,
                                                                                      int pagewidth,
                                                                                      int pageheight,
                                                                                      int id,
                                                                                      float x0,
                                                                                      float x1,
                                                                                      float y0,
                                                                                      float y1,
                                                                                      float xoffset,
                                                                                      float yoffset,
                                                                                      float width,
                                                                                      float height,
                                                                                      float xadvance,
                                                                                      int page)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var result = tools.ExtractCharacterFromLine(line, pagewidth, pageheight);

            var chr = result.Item2;

            Assert.Equal(id, result.Item1);
            Assert.Equal(x0, chr.X0);
            Assert.Equal(x1, chr.X1);
            Assert.Equal(y0, chr.Y0);
            Assert.Equal(y1, chr.Y1);
            Assert.Equal(xoffset, chr.XOffset);
            Assert.Equal(yoffset, chr.YOffset);
            Assert.Equal(width, chr.Width);
            Assert.Equal(height, chr.Height);
            Assert.Equal(xadvance, chr.XAdvance);
            Assert.Equal(page, chr.Page);
        }

        [Theory]
        [InlineData("char id=32 x=128 y=75 width=3 height=1 xoffset=-1 yoffset=94 xadvance=94 chnl=15", 100, 100)]
        [InlineData("char id=32 y=75 width=3 height=1 xoffset=-1 yoffset=94 xadvance=94 page=0 chnl=15", 100, 100)]
        [InlineData("char x=128 y=75 width=3 height=1 xoffset=-1 yoffset=94 xadvance=94 page=0 chnl=15", 100, 100)]
        [InlineData("char id=32 x=128 xoffset=-1 yoffset=94 xadvance=94 page=0 chnl=15", 100, 100)]
        [InlineData("char id=32 x=128 y=75 width=3 height=1 xadvance=94 page=0 chnl=15", 100, 100)]
        public void SubFontTools_ExtractCharacterFromLine_FailsOnMissingData(string line,
                                                                        int pagewidth,
                                                                        int pageheight)

        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            Assert.Null(tools.ExtractCharacterFromLine(line, pagewidth, pageheight).Item2);
        }

        [Theory]
        [InlineData("kerning first=34 second=65 amount=-11", (char)34, (char)65, -11)]
        [InlineData("kerning first=65 second=78 amount=12", (char)65, (char)78, 12)]
        [InlineData("kerning first=72 second=13 amount=-14", (char)72, (char)13, -14)]
        [InlineData("kerning first=44 second=23 amount=15", (char)44, (char)23, 15)]
        [InlineData("kerning first=45 second=98 amount=50", (char)45, (char)98, 50)]
        [InlineData("kerning     first=45   random=whathat == second=98  amount=50 xxxx", (char)45, (char)98, 50)]
        public void SubFontTools_ExtractKerningFromLine_CorrectlyExtracts(string line,
                                                                                char first,
                                                                                char second,
                                                                                short amount)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            var result = tools.ExtractKerningFromLine(line);

            Assert.Equal(first, result.Item1);
            Assert.Equal(second, result.Item2);
            Assert.Equal(amount, result.Item3);
        }

        [Theory]
        [InlineData("kerning second=65 amount=-11")]
        [InlineData("kerning first=65 amount=12")]
        [InlineData("kerning first=72 second=13 ")]
        [InlineData("kerning first=44 second=23 mount=15")]
        [InlineData("kerning first=45 second=98x amount=50")]
        [InlineData("kerning     first=45xx   random=whathat == second=98  amount=50 xxxx")]
        public void SubFontTools_ExtractKerningFromLine_FailsOnMissingData(string line)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            Assert.Null(tools.ExtractKerningFromLine(line).Item1);
        }
    }
}