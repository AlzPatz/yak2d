using NSubstitute;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Yak2D.Font;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class FontLoaderTest
    {
        [Fact]
        public void FontLoader_FindCorrectPartialMatchesInAppEmbeddedResources_FindsTheRightNumber()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var subFontGenerator = Substitute.For<ISubFontGenerator>();
            var gpuSurfaceManager= Substitute.For<IGpuSurfaceManager>();



            IFontLoader loader = new FontLoader(
                        appAssembly,
                        fontsAssembly,
                        messenger,
                        gpuSurfaceManager,
                        properties,
                        subFontGenerator,
                        filesystem);

            appAssembly.GetManifestResourceNames().Returns(new List<string>
            {
                "partial.test.uniqueend0.fnt",
                "no.match.here",
                "partial.test.uniqueend1.fnt",
                "partial.test.uniqueend2.fnt",
                "no.match.here",
                "no.match.here",
                "partial.test.uniqueend3",
                "partial.test.uniqueend4.fnt",
                "no.match.here",
                "no.match.here",
            });

            appAssembly.Name.Returns("partial");

            var result = loader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(false, "test");

            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void FontLoader_FindCorrectPartialMatchesInFontsEmbeddedResources_FindsTheRightNumber()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var subFontGenerator = Substitute.For<ISubFontGenerator>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();



            IFontLoader loader = new FontLoader(
                        appAssembly,
                        fontsAssembly,
                        messenger,
                        gpuSurfaceManager,
                        properties,
                        subFontGenerator,
                        filesystem);

            fontsAssembly.GetManifestResourceNames().Returns(new List<string>
            {
                "partial.test.uniqueend0.fnt",
                "no.match.here",
                "partial.test.uniqueend1.fnt",
                "partial.test.uniqueend2.fnt",
                "no.match.here",
                "no.match.here",
                "partial.test.uniqueend3",
                "partial.test.uniqueend4.fnt",
                "no.match.here",
                "no.match.here",
            });

            appAssembly.Name.Returns("partial");

            var result = loader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(true, "test");

            Assert.Equal(4, result.Count);
        }


        [Fact]
        public void FontLoader_FindCorrectPartialMatchesInFileResources_FindsTheRightNumber()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var subFontGenerator = Substitute.For<ISubFontGenerator>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            filesystem.GetFilesInDirectory(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<SearchOption>()).Returns(new string[]
            {
                "partial/testi/iqueend0.fnt",
                "no/match/here",
                "partial/test/uniqueend1.fnt",
                "partial/test/uniqueend2.fnt",
                "no/match/here",
                "no/match/here",
                "partial/test/uniqueend3",
                "partial/test/uniqueend4.fnt",
                "no/match/here",
                "no/match/here",
            });

            properties.User.Returns(new StartupConfig
            {
                FontFolder = "partial"
            });

            IFontLoader loader = new FontLoader(
            appAssembly,
            fontsAssembly,
            messenger,
            gpuSurfaceManager,
            properties,
            subFontGenerator,
            filesystem);

            var result = loader.FindDotFntFileNamePartialMatchesFromFileResource("test");

            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void FontLoader_TryToLoadSubFontDescriptionInternalEmbedded_GeneratesCorrectPathsAndCallsGpuManagerCorrectly()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            var subFontGenerator = new SubFontGenerator(messenger, new SubFontTools(messenger));

            var gpuSurfaceManagerMock = Substitute.For<IGpuSurfaceManager>();
           
            gpuSurfaceManagerMock.LoadFontTextureFromEmbeddedResource(true, Arg.Any<string>(), ImageFormat.PNG).Returns(new TextureReference(20));

            IFontLoader loader = new FontLoader(
                                    appAssembly,
                                    fontsAssembly,
                                    messenger,
                                    gpuSurfaceManagerMock,
                                    properties,
                                    subFontGenerator,
                                    filesystem);

            var fntFile = new List<string>
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
            };

            var desc = loader.TryToLoadSubFontDescription("FontFolder", true, AssetSourceEnum.Embedded, ImageFormat.PNG, fntFile);

            gpuSurfaceManagerMock.Received(3).LoadFontTextureFromEmbeddedResource(true, Arg.Any<string>(), ImageFormat.PNG);

            Assert.Equal(fntFile, desc.DotFntLines);
            Assert.Equal(3, desc.Textures.Count);
            Assert.Equal("minstrel_96_0", desc.TexturePaths[0]);
            Assert.Equal("minstrel_96_1", desc.TexturePaths[1]);
            Assert.Equal("minstrel_96_2", desc.TexturePaths[2]);
        }

        [Fact]
        public void FontLoader_TryToLoadSubFontDescriptionFile_GeneratesCorrectPathsAndCallsGpuManagerCorrectly()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            var subFontGenerator = new SubFontGenerator(messenger, new SubFontTools(messenger));

            var gpuSurfaceManagerMock = Substitute.For<IGpuSurfaceManager>();

            gpuSurfaceManagerMock.LoadFontTextureFromFile(Arg.Any<string>(), ImageFormat.PNG).Returns(new TextureReference(20));

            IFontLoader loader = new FontLoader(
                                            appAssembly,
                                            fontsAssembly,
                                            messenger,
                                            gpuSurfaceManagerMock,
                                            properties,
                                            subFontGenerator,
                                            filesystem);

            var fntFile = new List<string>
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
            };

            var desc = loader.TryToLoadSubFontDescription("FontFolder", false, AssetSourceEnum.File, ImageFormat.PNG, fntFile);

            gpuSurfaceManagerMock.Received(3).LoadFontTextureFromFile(Arg.Any<string>(), ImageFormat.PNG);

            Assert.Equal(fntFile, desc.DotFntLines);
            Assert.Equal(3, desc.Textures.Count);
            Assert.Equal("minstrel_96_0", desc.TexturePaths[0]);
            Assert.Equal("minstrel_96_1", desc.TexturePaths[1]);
            Assert.Equal("minstrel_96_2", desc.TexturePaths[2]);
        }

        [Fact]
        public void FontLoader_GenerateFontFromDescriptionInfo_CallsGeneratorEnoughTimes()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            var subFontGeneratorMock = Substitute.For<ISubFontGenerator>();
            subFontGeneratorMock.Generate(Arg.Any<CandidateSubFontDesc>()).Returns(new SubFont(0, 0, new Dictionary<char, FontCharacter>(), new ITexture[3], false, null));

            IFontLoader loader = new FontLoader(
                                            appAssembly,
                                            fontsAssembly,
                                            messenger,
                                            gpuSurfaceManager,
                                            properties,
                                            subFontGeneratorMock,
                                            filesystem);

            var result = loader.GenerateFontFromDescriptionInfo(new CandidateFontDesc
            {
                CandidateSubFonts = new List<CandidateSubFontDesc>
                 {
                     new CandidateSubFontDesc(),
                     new CandidateSubFontDesc(),
                     new CandidateSubFontDesc()
                 }
            });

            subFontGeneratorMock.Received(3).Generate(Arg.Any<CandidateSubFontDesc>());

            Assert.NotNull(result);
        }

        [Fact]
        public void FontLoader_GenerateFontFromDescriptionInfoFailsOnNullDescription_ReturnsNull()
        {
            var appAssembly = Substitute.For<IApplicationAssembly>();
            var fontsAssembly = Substitute.For<IFontsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();
            var subFontGenerator = Substitute.For<ISubFontGenerator>();

            IFontLoader loader = new FontLoader(
                                    appAssembly,
                                    fontsAssembly,
                                    messenger,
                                    gpuSurfaceManager,
                                    properties,
                                    subFontGenerator,
                                    filesystem);

            Assert.Null(loader.GenerateFontFromDescriptionInfo(null));
        }
    }
}