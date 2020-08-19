using NSubstitute;
using Xunit;
using Yak2D.Font;

namespace Yak2D.Tests
{
    public class SubFontGeneratorTest
    {
        [Fact]
        public void SubFontGenerator_Generate_SuccessfullyParsesDataAndReturnsValidSubFont()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = new System.Collections.Generic.List<ITexture>() { null },
                TexturePaths = new System.Collections.Generic.List<string>() { "random" },
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "info face=\"Minstrel Poster NF\" size=96 bold=0 italic=0 charset=\"\" unicode=1 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=1,1 outline=0",
                    "common lineHeight=95 base=77 scaleW=256 scaleH=256 pages=8 packed=0 alphaChnl=1 redChnl=0 greenChnl=0 blueChnl=0",
                    "page id=0 file=\"minstrel_96_0.png\"",
                    "page id=1 file=\"minstrel_96_1.png\"",
                    "page id=2 file=\"minstrel_96_2.png\"",
                    "chars count=5",
                    "char id=32   x=128   y=75    width=3     height=1     xoffset=-1    yoffset=94    xadvance=14    page=0  chnl=15",
                    "char id=33   x=221   y=0     width=23    height=56    xoffset=1     yoffset=22    xadvance=24    page=3  chnl=15",
                    "char id=34   x=104   y=238   width=24    height=15    xoffset=1     yoffset=22    xadvance=25    page=3  chnl=15",
                    "char id=35   x=57    y=209   width=42    height=46    xoffset=1     yoffset=23    xadvance=43    page=1  chnl=15",
                    "char id=36   x=46    y=110   width=44    height=54    xoffset=1     yoffset=23    xadvance=45    page=5  chnl=15",
                    "kernings count=4",
                    "kerning first=34  second=65  amount=-11",
                    "kerning first=34  second=79  amount=-3",
                    "kerning first=34  second=192 amount=-11",
                    "kerning first=34  second=193 amount=-11"
                }
            };

            var result = generator.Generate(desc);

            Assert.Equal(5, result.Characters.Count);
            Assert.Equal(96, result.Size);
            Assert.Equal(95, result.LineHeight);
            Assert.True(result.HasKernings);
            Assert.Equal(94, result.Characters[(char)32].YOffset);
            Assert.Equal(-11, result.Kernings[(char)34][(char)192]);
        }

        [Fact]
        public void SubFontGenerator_Generate_FailsAsMissingLineHeightARandomlyChosenParam()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = new System.Collections.Generic.List<ITexture>() { null },
                TexturePaths = new System.Collections.Generic.List<string>() { "random" },
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "info face=\"Minstrel Poster NF\" size=96 bold=0 italic=0 charset=\"\" unicode=1 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=1,1 outline=0",
                    "common base=77 scaleW=256 scaleH=256 pages=8 packed=0 alphaChnl=1 redChnl=0 greenChnl=0 blueChnl=0", //lineheight=95 removed
                    "page id=0 file=\"minstrel_96_0.png\"",
                    "page id=1 file=\"minstrel_96_1.png\"",
                    "page id=2 file=\"minstrel_96_2.png\"",
                    "chars count=5",
                    "char id=32   x=128   y=75    width=3     height=1     xoffset=-1    yoffset=94    xadvance=14    page=0  chnl=15",
                    "char id=33   x=221   y=0     width=23    height=56    xoffset=1     yoffset=22    xadvance=24    page=3  chnl=15",
                    "char id=34   x=104   y=238   width=24    height=15    xoffset=1     yoffset=22    xadvance=25    page=3  chnl=15",
                    "char id=35   x=57    y=209   width=42    height=46    xoffset=1     yoffset=23    xadvance=43    page=1  chnl=15",
                    "char id=36   x=46    y=110   width=44    height=54    xoffset=1     yoffset=23    xadvance=45    page=5  chnl=15",
                    "kernings count=4",
                    "kerning first=34  second=65  amount=-11",
                    "kerning first=34  second=79  amount=-3",
                    "kerning first=34  second=192 amount=-11",
                    "kerning first=34  second=193 amount=-11"
                }
            };

            Assert.Null(generator.Generate(desc));
        }

        [Fact]
        public void SubFontGenerator_Generate_FailsOnNullTextureList()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = null,
                TexturePaths = new System.Collections.Generic.List<string>() { "random" },
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "x",
                    "x",
                    "x"
                }
            };


            Assert.Null(generator.Generate(desc));
        }

        [Fact]
        public void SubFontGenerator_Generate_FailsOnTextureListOfZero()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = new System.Collections.Generic.List<ITexture>() { },
                TexturePaths = new System.Collections.Generic.List<string>() { "random" },
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "x",
                    "x",
                    "x"
                }
            };

            Assert.Null(generator.Generate(desc));
        }

        [Fact]
        public void SubFontGenerator_Generate_FailsOnNullTextureStrings()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = new System.Collections.Generic.List<ITexture>() { null },
                TexturePaths = null,
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "x",
                    "x",
                    "x"
                }
            };

            Assert.Null(generator.Generate(desc));
        }

        [Fact]
        public void SubFontGenerator_Generate_FailsOnTextureStringListOfZero()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = new System.Collections.Generic.List<ITexture>() { null },
                TexturePaths = new System.Collections.Generic.List<string>() { },
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "x",
                    "x",
                    "x"
                }
            };

            Assert.Null(generator.Generate(desc));
        }

        [Fact]
        public void SubFontGenerator_Generate_FailsOnLessThanThreeLinesInInputText()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISubFontTools tools = new SubFontTools(messenger);

            ISubFontGenerator generator = new SubFontGenerator(messenger, tools);

            var desc = new CandidateSubFontDesc
            {
                Textures = new System.Collections.Generic.List<ITexture>() { null },
                TexturePaths = new System.Collections.Generic.List<string>() { "random" },
                DotFntLines = new System.Collections.Generic.List<string>
                {
                    "x",
                    "x"
                }
            };

            Assert.Null(generator.Generate(desc));
        }
    }
}