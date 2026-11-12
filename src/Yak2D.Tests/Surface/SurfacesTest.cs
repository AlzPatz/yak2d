using NeoVeldrid;
using NSubstitute;
using System.Linq;
using System.Numerics;
using Xunit;
using Yak2D.Internal;
using Yak2D.Surface;

namespace Yak2D.Tests
{
    public class SurfacesTest
    {
        [Fact]
        public void Surfaces_CreateRenderTargetInvalidDimensions_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateRenderTarget(0, 0); });
        }

        [Fact]
        public void Surfaces_GetSurfaceDimensionsNullTexture_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.GetSurfaceDimensions(null); });
        }

        [Fact]
        public void Surfaces_LoadTexturePathNullOrEmpty_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.GetSurfaceDimensions(null); });
        }

        [Fact]
        public void Surfaces_CreateFloat32FromDataNullData_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateFloat32FromData(10, 20, null); });
        }

        [Fact]
        public void Surfaces_CreateFloat32FromDataArraySizeAndDimensionMisMatch_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateFloat32FromData(10, 10, new float[90]); });
        }

        [Fact]
        public void Surfaces_CreateFloat32FromDataArraySizeDimensionOfZero_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateFloat32FromData(0, 10, new float[0]); });
        }

        [Fact]
        public void Surfaces_CreateRgbaFromDataNullData_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateRgbaFromData(10, 10, new Vector4[90]); });
        }

        [Fact]
        public void Surfaces_CreateRgbaFromDataArraySizeAndDimensionMisMatch_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateRgbaFromData(10, 10, new Vector4[90]); });
        }

        [Fact]
        public void Surfaces_CreateRgbaFromDataArraySizeDimensionOfZero_ThrowsException()
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);

            Assert.Throws<Yak2DException>(() => { surfaces.CreateRgbaFromData(0, 10, new Vector4[0]); });
        }

        //2x2 image, row 0 (top) = red, green; row 1 (bottom) = blue, white
        private static Vector4[] TwoByTwoColours() => new Vector4[]
        {
            new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
        };

        private static (byte, byte, byte, byte)[] AsTuples(RgbaByte[] data) => data.Select(c => (c.R, c.G, c.B, c.A)).ToArray();

        private static RgbaByte[] CaptureRgbaPassedToManager(GraphicsApi backend, Vector4[] pixels)
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            systemComponents.Device.BackendType.Returns(backend);
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            RgbaByte[] captured = null;
            surfaceManager.CreateRgbaTextureFromPixelData(Arg.Any<uint>(), Arg.Any<uint>(), Arg.Do<RgbaByte[]>(x => captured = x), Arg.Any<SamplerType>(), Arg.Any<bool>(), Arg.Any<bool>())
                          .Returns(Substitute.For<ITexture>());

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);
            surfaces.CreateRgbaFromData(2, 2, pixels);

            surfaceManager.Received(1).CreateRgbaTextureFromPixelData(2, 2, Arg.Any<RgbaByte[]>(), Arg.Any<SamplerType>(), true, false);

            return captured;
        }

        private static float[] CaptureFloat32PassedToManager(GraphicsApi backend, float[] pixels)
        {
            var systemComponents = Substitute.For<ISystemComponents>();
            systemComponents.Device.BackendType.Returns(backend);
            var surfaceManager = Substitute.For<IGpuSurfaceManager>();
            var properties = Substitute.For<IStartupPropertiesCache>();

            float[] captured = null;
            surfaceManager.CreateFloat32TextureFromPixelData(Arg.Any<uint>(), Arg.Any<uint>(), Arg.Do<float[]>(x => captured = x), Arg.Any<SamplerType>())
                          .Returns(Substitute.For<ITexture>());

            ISurfaces surfaces = new Surfaces(properties, surfaceManager, systemComponents);
            surfaces.CreateFloat32FromData(2, 2, pixels);

            surfaceManager.Received(1).CreateFloat32TextureFromPixelData(2, 2, Arg.Any<float[]>(), Arg.Any<SamplerType>());

            return captured;
        }

        [Theory]
        [InlineData(GraphicsApi.OpenGL)]
        [InlineData(GraphicsApi.OpenGLES)]
        public void Surfaces_CreateRgbaFromData_RowsMirroredOnOpenGL(GraphicsApi backend)
        {
            var pixels = TwoByTwoColours();

            var captured = CaptureRgbaPassedToManager(backend, pixels);

            Assert.NotNull(captured);
            Assert.Equal(new (byte, byte, byte, byte)[]
            {
                (0, 0, 255, 255), (255, 255, 255, 255),
                (255, 0, 0, 255), (0, 255, 0, 255),
            }, AsTuples(captured));

            Assert.Equal(TwoByTwoColours(), pixels);
        }

        [Theory]
        [InlineData(GraphicsApi.Vulkan)]
        [InlineData(GraphicsApi.Direct3D11)]
        [InlineData(GraphicsApi.Metal)]
        public void Surfaces_CreateRgbaFromData_RowsUnchangedOnOtherBackends(GraphicsApi backend)
        {
            var pixels = TwoByTwoColours();

            var captured = CaptureRgbaPassedToManager(backend, pixels);

            Assert.NotNull(captured);
            Assert.Equal(new (byte, byte, byte, byte)[]
            {
                (255, 0, 0, 255), (0, 255, 0, 255),
                (0, 0, 255, 255), (255, 255, 255, 255),
            }, AsTuples(captured));
        }

        [Theory]
        [InlineData(GraphicsApi.OpenGL)]
        [InlineData(GraphicsApi.OpenGLES)]
        public void Surfaces_CreateFloat32FromData_RowsMirroredOnOpenGL(GraphicsApi backend)
        {
            var pixels = new float[] { 0.0f, 1.0f, 2.0f, 3.0f };

            var captured = CaptureFloat32PassedToManager(backend, pixels);

            Assert.Equal(new float[] { 2.0f, 3.0f, 0.0f, 1.0f }, captured);
            Assert.Equal(new float[] { 0.0f, 1.0f, 2.0f, 3.0f }, pixels);
        }

        [Theory]
        [InlineData(GraphicsApi.Vulkan)]
        [InlineData(GraphicsApi.Direct3D11)]
        [InlineData(GraphicsApi.Metal)]
        public void Surfaces_CreateFloat32FromData_RowsUnchangedOnOtherBackends(GraphicsApi backend)
        {
            var pixels = new float[] { 0.0f, 1.0f, 2.0f, 3.0f };

            var captured = CaptureFloat32PassedToManager(backend, pixels);

            Assert.Equal(new float[] { 0.0f, 1.0f, 2.0f, 3.0f }, captured);
        }
    }
}