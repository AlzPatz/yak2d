using NSubstitute;
using Xunit;
using Xunit.Sdk;
using Yak2D.Internal;
using Yak2D.Surface;

namespace Yak2D.Tests
{
    public class GpuSurfaceManagerTest
    {
        private IGpuSurfaceManager SetUpManagerWithFakes()
        {
            var components = Substitute.For<ISystemComponents>();
            var appassembly = Substitute.For<IApplicationAssembly>();
            var fonts = Substitute.For<IFontsAssembly>();
            var surface = Substitute.For<ISurfaceAssembly>();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var id = Substitute.For<IIdGenerator>();
            var props = Substitute.For<IStartupPropertiesCache>();
            var surfaceCollection = Substitute.For<IGpuSurfaceCollection>();
            var imagesharp = Substitute.For<IImageSharpLoader>();
            var surfacefactory = Substitute.For<IGpuSurfaceFactory>();
            var filesystem = Substitute.For<IFileSystem>();

            var userProps = Substitute.For<StartupConfig>();
            userProps.AutoClearMainWindowColourEachFrame = true;
            userProps.AutoClearMainWindowDepthEachFrame = true;
            props.User.Returns(userProps);

            IGpuSurfaceManager manager = new GpuSurfaceManager(appassembly,
                                                               fonts,
                                                               surface,
                                                               messenger,
                                                               id,
                                                               props,
                                                               surfaceCollection,
                                                               imagesharp,
                                                               surfacefactory,
                                                               components,
                                                               filesystem);

            return manager;
        }

        [Fact]
        public void SurfaceManager_CheckSurfaceCounts_ReturnStubbedFigures()
        {
            var components = Substitute.For<ISystemComponents>();
            var appassembly = Substitute.For<IApplicationAssembly>();
            var fonts = Substitute.For<IFontsAssembly>();
            var surface = Substitute.For<ISurfaceAssembly>();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var id = Substitute.For<IIdGenerator>();
            var props = Substitute.For<IStartupPropertiesCache>();
            var surfaceCollection = Substitute.For<IGpuSurfaceCollection>();
            var imagesharp = Substitute.For<IImageSharpLoader>();
            var surfacefactory = Substitute.For<IGpuSurfaceFactory>();
            var filesystem = Substitute.For<IFileSystem>();

            var userProps = Substitute.For<StartupConfig>();
            userProps.AutoClearMainWindowColourEachFrame = true;
            userProps.AutoClearMainWindowDepthEachFrame = true;
            props.User.Returns(userProps);

            IGpuSurfaceManager manager = new GpuSurfaceManager(appassembly,
                                                               fonts,
                                                               surface,
                                                               messenger,
                                                               id,
                                                               props,
                                                               surfaceCollection,
                                                               imagesharp,
                                                               surfacefactory,
                                                               components,
                                                               filesystem);

            surfaceCollection.CountOfType(GpuSurfaceType.RenderTarget | GpuSurfaceType.User).Returns(6);
            surfaceCollection.CountOfType(GpuSurfaceType.Texture | GpuSurfaceType.User).Returns(4);

            Assert.Equal(10, manager.TotalUserSurfaceCount);
            Assert.Equal(6, manager.UserRenderTargetCount);
            Assert.Equal(4, manager.UserTextureCount);
        }

        [Fact]
        public void SurfaceManager_LoadTextureFromEmbeddedPngCatchesNullOrEmptyString_ThrowsException()
        {
            var manager = SetUpManagerWithFakes();

            Assert.Throws<Yak2DException>(() => { manager.CreateTextureFromEmbeddedResourceInUserApplication(null, ImageFormat.PNG, SamplerType.Anisotropic, true); });
            Assert.Throws<Yak2DException>(() => { manager.CreateTextureFromEmbeddedResourceInUserApplication("", ImageFormat.PNG, SamplerType.Anisotropic, true); });
        }

        [Fact]
        public void SurfaceManager_LoadFontTextureFromEmbeddedPngCatchesNullOrEmptyString_ThrowsException()
        {
            var manager = SetUpManagerWithFakes();

            Assert.Throws<Yak2DException>(() => { manager.CreateFontTextureFromEmbeddedResource(true, null, ImageFormat.PNG, SamplerType.Anisotropic); });
            Assert.Throws<Yak2DException>(() => { manager.CreateFontTextureFromEmbeddedResource(true, "", ImageFormat.PNG, SamplerType.Anisotropic); });
        }

        [Fact]
        public void SurfaceManager_LoadTextureFromPngFileCatchesNullOrEmptyString_ThrowsException()
        {
            var manager = SetUpManagerWithFakes();

            Assert.Throws<Yak2DException>(() => { manager.CreateTextureFromFile(null, ImageFormat.PNG, SamplerType.Anisotropic, true); });
            Assert.Throws<Yak2DException>(() => { manager.CreateTextureFromFile("", ImageFormat.PNG, SamplerType.Anisotropic, true); });
        }

        [Fact]
        public void SurfaceManager_LoadFontTextureFromPngFileCatchesNullOrEmptyString_ThrowsException()
        {
            var manager = SetUpManagerWithFakes();

            Assert.Throws<Yak2DException>(() => { manager.CreateFontTextureFromFile(null, ImageFormat.PNG, SamplerType.Anisotropic); });
            Assert.Throws<Yak2DException>(() => { manager.CreateFontTextureFromFile("", ImageFormat.PNG, SamplerType.Anisotropic); });
        }
    }
}