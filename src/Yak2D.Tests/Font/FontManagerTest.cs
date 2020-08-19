using NSubstitute;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Yak2D.Font;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class FontManagerTest
    {
        //Do not test the simple colleciton retrieval methods (as done via font collection)
        //Some program flow tests below using mocks, although not particularly insightful tests
        //and quite rigid, as testing hidden flow. Doubt this survives any major code refactor

        [Fact]
        public void FontManager_LoadSystemFontAtInitialisationCheckValidFlow_MakesCorrectCalls()
        {
            var id = Substitute.For<IIdGenerator>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var collection = Substitute.For<IFontCollection>();
            var loader = Substitute.For<IFontLoader>();

            id.New().Returns(0UL);
            collection.Add(Arg.Any<ulong>(), Arg.Any<IFontModel>()).Returns(true);

            properties.User.Returns(new StartupConfig
            {
                FontFolder = "folder"
            });

            loader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(Arg.Any<bool>(), Arg.Any<string>()).Returns(new List<string> { "random" });
            loader.FindDotFntFileNamePartialMatchesFromFileResource(Arg.Any<string>()).Returns(new List<string> { "random" });

            loader.LoadEmbeddedStream(Arg.Any<bool>(), Arg.Any<string>()).Returns(default(Stream));
            loader.LoadFileStream(Arg.Any<string>()).Returns(default(Stream));

            loader.ReadStreamToStringList(Arg.Any<Stream>()).Returns(new List<string> { "single line" });

            loader.TryToLoadSubFontDescription(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<AssetSourceEnum>(), Arg.Any<List<string>>()).Returns(new CandidateSubFontDesc());

            loader.GenerateFontFromDescriptionInfo(Arg.Any<CandidateFontDesc>())
                  .Returns(new Yak2D.Font.FontModel(new List<SubFont> { new SubFont(1, 1, null, null, false, null) }));

            IFontManager manager = new FontManager(id, properties, collection, loader);

            loader.Received(1).FindDotFntFileNamePartialMatchesFromEmbeddedResource(true, Arg.Any<string>());
            loader.Received(1).LoadEmbeddedStream(true, Arg.Any<string>());
            loader.Received(1).ReadStreamToStringList(Arg.Any<Stream>());
            loader.Received(1).TryToLoadSubFontDescription(Arg.Any<string>(), true, AssetSourceEnum.Embedded, Arg.Any<List<string>>());
            loader.Received(1).GenerateFontFromDescriptionInfo(Arg.Any<CandidateFontDesc>());
        }


        [Theory]
        [InlineData(AssetSourceEnum.Embedded)]
        [InlineData(AssetSourceEnum.File)]
        public void FontManager_LoadUserFontCheckValidFlow_MakesCorrectCalls(AssetSourceEnum source)
        {
            var id = Substitute.For<IIdGenerator>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var collection = Substitute.For<IFontCollection>();
            var loader = Substitute.For<IFontLoader>();

            id.New().Returns(0UL);
            collection.Add(Arg.Any<ulong>(), Arg.Any<IFontModel>()).Returns(true);

            properties.User.Returns(new StartupConfig
            {
                FontFolder = "folder"
            });

            loader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(Arg.Any<bool>(), Arg.Any<string>()).Returns(new List<string> { "random" });
            loader.FindDotFntFileNamePartialMatchesFromFileResource(Arg.Any<string>()).Returns(new List<string> { "random" });

            loader.LoadEmbeddedStream(Arg.Any<bool>(), Arg.Any<string>()).Returns(default(Stream));
            loader.LoadFileStream(Arg.Any<string>()).Returns(default(Stream));

            loader.ReadStreamToStringList(Arg.Any<Stream>()).Returns(new List<string> { "single line" });

            loader.TryToLoadSubFontDescription(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<AssetSourceEnum>(), Arg.Any<List<string>>()).Returns(new CandidateSubFontDesc());

            loader.GenerateFontFromDescriptionInfo(Arg.Any<CandidateFontDesc>())
                  .Returns(new Yak2D.Font.FontModel(new List<SubFont> { new SubFont(1, 1, null, null, false, null) }));

            IFontManager manager = new FontManager(id, properties, collection, loader);

            loader.ClearReceivedCalls();

            var result = manager.LoadUserFont("path", source);

            Assert.NotNull(result);

            switch (source)
            {
                case AssetSourceEnum.Embedded:
                    loader.Received(1).FindDotFntFileNamePartialMatchesFromEmbeddedResource(false, Arg.Any<string>());
                    loader.Received(1).LoadEmbeddedStream(false, Arg.Any<string>());
                    break;
                case AssetSourceEnum.File:
                    loader.Received(1).FindDotFntFileNamePartialMatchesFromFileResource(Arg.Any<string>());
                    loader.Received(1).LoadFileStream(Arg.Any<string>());
                    break;
            }
            loader.Received(1).ReadStreamToStringList(Arg.Any<Stream>());
            loader.Received(1).TryToLoadSubFontDescription(Arg.Any<string>(), false, source, Arg.Any<List<string>>());
            loader.Received(1).GenerateFontFromDescriptionInfo(Arg.Any<CandidateFontDesc>());
        }

        [Fact]
        public void FontManager_LoadUserFontCheckInvalidPath_ThrowsExceptionOnWhiteSpace()
        {
            var id = Substitute.For<IIdGenerator>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var collection = Substitute.For<IFontCollection>();
            var loader = Substitute.For<IFontLoader>();

            id.New().Returns(0UL);
            collection.Add(Arg.Any<ulong>(), Arg.Any<IFontModel>()).Returns(true);

            properties.User.Returns(new StartupConfig
            {
                FontFolder = "folder"
            });

            loader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(Arg.Any<bool>(), Arg.Any<string>()).Returns(new List<string> { "random" });
            loader.FindDotFntFileNamePartialMatchesFromFileResource(Arg.Any<string>()).Returns(new List<string> { "random" });

            loader.LoadEmbeddedStream(Arg.Any<bool>(), Arg.Any<string>()).Returns(default(Stream));
            loader.LoadFileStream(Arg.Any<string>()).Returns(default(Stream));

            loader.ReadStreamToStringList(Arg.Any<Stream>()).Returns(new List<string> { "single line" });

            loader.TryToLoadSubFontDescription(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<AssetSourceEnum>(), Arg.Any<List<string>>()).Returns(new CandidateSubFontDesc());

            loader.GenerateFontFromDescriptionInfo(Arg.Any<CandidateFontDesc>())
                  .Returns(new Yak2D.Font.FontModel(new List<SubFont> { new SubFont(1, 1, null, null, false, null) }));

            IFontManager manager = new FontManager(id, properties, collection, loader);

            Assert.Throws<Yak2DException>(() => { manager.LoadUserFont("has whitespace", AssetSourceEnum.Embedded); });
        }

        [Fact]
        public void FontManager_LoadUserFontCheckInvalidPath_ThrowsExceptionOnNull()
        {
            var id = Substitute.For<IIdGenerator>();
            var properties = Substitute.For<IStartupPropertiesCache>();
            var collection = Substitute.For<IFontCollection>();
            var loader = Substitute.For<IFontLoader>();

            id.New().Returns(0UL);
            collection.Add(Arg.Any<ulong>(), Arg.Any<IFontModel>()).Returns(true);

            properties.User.Returns(new StartupConfig
            {
                FontFolder = "folder"
            });

            loader.FindDotFntFileNamePartialMatchesFromEmbeddedResource(Arg.Any<bool>(), Arg.Any<string>()).Returns(new List<string> { "random" });
            loader.FindDotFntFileNamePartialMatchesFromFileResource(Arg.Any<string>()).Returns(new List<string> { "random" });

            loader.LoadEmbeddedStream(Arg.Any<bool>(), Arg.Any<string>()).Returns(default(Stream));
            loader.LoadFileStream(Arg.Any<string>()).Returns(default(Stream));

            loader.ReadStreamToStringList(Arg.Any<Stream>()).Returns(new List<string> { "single line" });

            loader.TryToLoadSubFontDescription(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<AssetSourceEnum>(), Arg.Any<List<string>>()).Returns(new CandidateSubFontDesc());

            loader.GenerateFontFromDescriptionInfo(Arg.Any<CandidateFontDesc>())
                  .Returns(new Yak2D.Font.FontModel(new List<SubFont> { new SubFont(1, 1, null, null, false, null) }));

            IFontManager manager = new FontManager(id, properties, collection, loader);

            Assert.Throws<Yak2DException>(() => { manager.LoadUserFont(null, AssetSourceEnum.Embedded); });
        }
    }
}