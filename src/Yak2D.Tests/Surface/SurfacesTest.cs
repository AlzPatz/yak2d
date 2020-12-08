using NSubstitute;
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
    }
}