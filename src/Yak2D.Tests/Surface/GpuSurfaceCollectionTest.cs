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

        private static NeoVeldrid.Framebuffer CreateFramebuffer(NeoVeldrid.GraphicsDevice device)
        {
            var colour = device.ResourceFactory.CreateTexture(NeoVeldrid.TextureDescription.Texture2D(4, 4, 1, 1, NeoVeldrid.PixelFormat.R8_G8_B8_A8_UNorm, NeoVeldrid.TextureUsage.RenderTarget));
            return device.ResourceFactory.CreateFramebuffer(new NeoVeldrid.FramebufferDescription(null, colour));
        }

        [Fact]
        public void SurfaceCollection_TestRemoveSwapChainOutput_DoesNotDisposeSwapChainFramebuffer()
        {
            var device = new Yak2D.Tests.ManualFakes.FakeGraphicsDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var framebuffer = CreateFramebuffer(device.Device);

            var collection = new GpuSurfaceCollection(messenger);
            collection.Add(9, new GpuSurface { Type = GpuSurfaceType.SwapChainOutput | GpuSurfaceType.Internal, Framebuffer = framebuffer });

            Assert.True(collection.Remove(9));
            Assert.False(framebuffer.IsDisposed);
            framebuffer.Dispose();
            device.Dispose();
        }

        [Fact]
        public void SurfaceCollection_TestRemoveRenderTarget_DisposesItsFramebuffer()
        {
            var device = new Yak2D.Tests.ManualFakes.FakeGraphicsDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var framebuffer = CreateFramebuffer(device.Device);

            var collection = new GpuSurfaceCollection(messenger);
            collection.Add(9, new GpuSurface { Type = GpuSurfaceType.RenderTarget | GpuSurfaceType.User, Framebuffer = framebuffer });

            Assert.True(collection.Remove(9));
            Assert.True(framebuffer.IsDisposed);
            device.Dispose();
        }
    }
}
