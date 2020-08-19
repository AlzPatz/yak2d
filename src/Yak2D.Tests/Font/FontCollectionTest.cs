using NSubstitute;
using Xunit;
using Yak2D.Font;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class FontCollectionTest
    {
        [Fact]
        public void FontCollection_TestAddFont_ReturnsTrue()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            Assert.True(collection.Add(0, Substitute.For<IFontModel>()));
        }

        [Fact]
        public void FontCollection_TestAddFontWithSameId_ReturnsFalse()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            collection.Add(12, Substitute.For<IFontModel>());
            Assert.False(collection.Add(12, Substitute.For<IFontModel>()));
        }

        [Fact]
        public void FontCollection_TestAddFont_ReturnCorrectCount()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            collection.Add(0, Substitute.For<IFontModel>());
            collection.Add(1, Substitute.For<IFontModel>());
            collection.Add(2, Substitute.For<IFontModel>());
            collection.Add(3, Substitute.For<IFontModel>());
            collection.Add(4, Substitute.For<IFontModel>());

            Assert.Equal(5, collection.Count);
        }

        [Fact]
        public void FontCollection_TestRemoveFontAndReUseIndex_ReturnCorrectCountAndAddSuccessfully()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            collection.Add(0, Substitute.For<IFontModel>());
            collection.Add(1, Substitute.For<IFontModel>());
            collection.Add(2, Substitute.For<IFontModel>());
            collection.Add(3, Substitute.For<IFontModel>());
            collection.Add(4, Substitute.For<IFontModel>());

            collection.Destroy(3);
            Assert.Equal(4, collection.Count);
            Assert.True(collection.Add(3, Substitute.For<IFontModel>()));
        }

        [Fact]
        public void FontCollection_TestDestroyAllFonts_ReturnCorrectZeroCount()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            collection.Add(0, Substitute.For<IFontModel>());
            collection.Add(1, Substitute.For<IFontModel>());
            collection.Add(2, Substitute.For<IFontModel>());
            collection.Add(3, Substitute.For<IFontModel>());
            collection.Add(4, Substitute.For<IFontModel>());

            collection.DestroyAll(true);
            Assert.Equal(0, collection.Count);
        }

        [Fact]
        public void FontCollection_TestRetrieveExisting_ReturnCorrectFont()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            collection.Add(0, Substitute.For<IFontModel>());
            collection.Add(1, Substitute.For<IFontModel>());

            var fnt = Substitute.For<IFontModel>();
            var code = fnt.GetHashCode();
            collection.Add(2, fnt);

            collection.Add(3, Substitute.For<IFontModel>());
            collection.Add(4, Substitute.For<IFontModel>());


            var font = collection.Retrieve(2);

            Assert.Equal(code, font.GetHashCode());
        }

        [Fact]
        public void FontCollection_TestRetrieveNonExisting_ReturnNull()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();

            IFontCollection collection = new FontCollection(messenger, surfaceManager);

            collection.Add(0, Substitute.For<IFontModel>());
            collection.Add(1, Substitute.For<IFontModel>());
            collection.Add(3, Substitute.For<IFontModel>());
            collection.Add(4, Substitute.For<IFontModel>());


            var font = collection.Retrieve(2);

            Assert.Null(font);
        }
    }
}