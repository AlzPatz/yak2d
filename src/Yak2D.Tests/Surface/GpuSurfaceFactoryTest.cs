using NSubstitute;
using Xunit;
using Yak2D.Internal;
using Yak2D.Surface;
using Yak2D.Tests.ManualFakes;

namespace Yak2D.Tests
{
    public class GpuSurfaceFactoryTest
    {
        [Fact]
        public void SurfaceFactory_CreateSurfaceFromTextureCatchesNullInput_ThrowsException()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var components = Substitute.For<ISystemComponents>();

            IGpuSurfaceFactory factory = new GpuSurfaceFactory(messenger, components);

            Assert.Throws<Yak2DException>(() => { factory.CreateGpuSurfaceFromTexture(null, false); });

            components.ReleaseResources();
        }

        [Fact]
        public void SurfaceFactory_CreatesSurfaceFromTextureSucceeds_ComponentsAreNotNull()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var components = new FakeComponents();

           IGpuSurfaceFactory factory = new GpuSurfaceFactory(messenger, components);

            Veldrid.Texture texture = components.Factory.CreateTexture(new Veldrid.TextureDescription
            {
                Width = 64,
                Height = 64,
                ArrayLayers = 1,
                Depth = 1,
                Format = Veldrid.PixelFormat.R32_G32_B32_A32_Float,
                MipLevels = 1,
                SampleCount = Veldrid.TextureSampleCount.Count1,
                Type = Veldrid.TextureType.Texture2D,
                Usage = Veldrid.TextureUsage.Sampled
            });

            var surface = factory.CreateGpuSurfaceFromTexture(texture, false);

            Assert.Equal(texture, surface.Texture);
            Assert.NotNull(surface.TextureView);
            Assert.NotNull(surface.ResourceSet_TexMirror);
            Assert.NotNull(surface.ResourceSet_TexWrap);

            components.ReleaseResources();
        }

        [Fact]
        public void SurfaceFactory_CreatesSurfaceSucceeds_ComponentsAreNotNullAndTypeIsCorrect()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var components = new FakeComponents();

            IGpuSurfaceFactory factory = new GpuSurfaceFactory(messenger, components);

            var surface = factory.CreateGpuSurface(true, 100, 100, Veldrid.PixelFormat.R32_G32_B32_A32_Float, false, SamplerType.Anisotropic);

            Assert.Equal(GpuSurfaceType.RenderTarget | GpuSurfaceType.Internal, surface.Type);
            Assert.NotNull(surface.Texture);
            Assert.NotNull(surface.TextureView);
            Assert.NotNull(surface.ResourceSet_TexMirror);
            Assert.NotNull(surface.ResourceSet_TexWrap);

            components.ReleaseResources();
        }

        [Fact]
        public void SurfaceFactory_CreatesSurfaceCatchesInvalidInput_ThrowsExceptionWhenDimensionsAreZero()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var components = Substitute.For<ISystemComponents>();

            IGpuSurfaceFactory factory = new GpuSurfaceFactory(messenger, components);

            Assert.Throws<Yak2DException>(() => { factory.CreateGpuSurface(false, 0, 0, Veldrid.PixelFormat.R32_G32_B32_A32_Float, false, SamplerType.Anisotropic); });

            components.ReleaseResources();
        }

        [Fact]
        public void SurfaceFactory_CreatesSurfaceFromSwapChainOutputBufferCatchesNullInput_ThrowsException()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            var components = Substitute.For<ISystemComponents>();

            IGpuSurfaceFactory factory = new GpuSurfaceFactory(messenger, components);

            Assert.Throws<Yak2DException>(() => { factory.CreateSurfaceFromSwapChainOutputBuffer(null); });

            components.ReleaseResources();
        }
    }
}
