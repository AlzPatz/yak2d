using NSubstitute;
using Xunit;
using Yak2D.Internal;
using Yak2D.Surface;

namespace Yak2D.Tests
{
    public class GpuSurfaceCollectionTest
    {
        private void AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(IGpuSurfaceCollection collection)
        {
            collection.Add(0, new GpuSurface { Type = GpuSurfaceType.Internal | GpuSurfaceType.Texture });
            collection.Add(1, new GpuSurface { Type = GpuSurfaceType.Undefined });
            collection.Add(2, new GpuSurface { Type = GpuSurfaceType.RenderTarget });
            collection.Add(3, new GpuSurface { Type = GpuSurfaceType.RenderTarget });
            collection.Add(4, new GpuSurface { Type = GpuSurfaceType.Texture });
        }

        [Fact]
        public void SurfaceCollection_TestCountAll_Returns5()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            Assert.Equal(5, collection.CountAll());
        }

        [Fact]
        public void SurfaceCollection_TestCountType_Returns2CountOfRenderTargets()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            Assert.Equal(2, collection.CountOfType(GpuSurfaceType.RenderTarget));
        }

        [Fact]
        public void SurfaceCollection_TestMultiFlagCountType_Returns1CountOfInternalTexture()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            Assert.Equal(1, collection.CountOfType(GpuSurfaceType.Internal | GpuSurfaceType.Texture));
        }

        [Fact]
        public void SurfaceCollection_TestWillNotAddNullSurface_ReturnsFalse()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            Assert.False(collection.Add(0, null));
        }

        [Fact]
        public void SurfaceCollection_TestWillNotAddSurfaceAtExistingId_ReturnsFalse()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            collection.Add(50, new GpuSurface());
            Assert.False(collection.Add(50, new GpuSurface()));
        }

        [Fact]
        public void SurfaceCollection_TestRemoveAll_Returns0()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            collection.RemoveAll(true);

            Assert.Equal(0, collection.CountAll());
        }

        [Fact]
        public void SurfaceCollection_TestRemoveTypeTexture_Returns3()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            var ids = collection.ReturnAllOfType(GpuSurfaceType.Texture);

            ids.ForEach(id =>
            {
                collection.Remove(id);
            });

            Assert.Equal(3, collection.CountAll());
        }

        [Fact]
        public void SurfaceCollection_TestRemoveAtId_Returns4()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            collection.Remove(2);

            Assert.Equal(4, collection.CountAll());
        }

        [Fact]
        public void SurfaceCollection_TestRemoveAtIdFailsIfIdNotPresent_ReturnsFalse()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            Assert.False(collection.Remove(7));
        }

        [Fact]
        public void SurfaceCollection_TestRetrieveAtId_ReturnsSurfaceWithUndefinedType()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var collection = new GpuSurfaceCollection(messenger);

            AddToCollectionTwoInternalTwoRenderTargetsAndOneTexture(collection);

            var surface = collection.Retrieve(1);

            Assert.Equal(GpuSurfaceType.Undefined, surface.Type);
        }
    }
}