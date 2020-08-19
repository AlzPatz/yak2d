using NSubstitute;
using Xunit;
using Yak2D.Internal;
using Yak2D.Surface;

namespace Yak2D.Tests
{
    public class FloatTextureBuilderTest
    {
        [Fact]
        public void FloatTextureBuilder_CatchesNullInput_ThrowsException()
        {
            ISystemComponents components = Substitute.For<ISystemComponents>();

            IFloatTextureBuilder builder = new FloatTextureBuilder(components);

            Assert.Throws<Yak2DException>(() => { builder.GenerateFloat32VeldridTextureFromPixelData(null, 100, 100); });

            components.ReleaseResources();
        }

        [Fact]
        public void FloatTextureBuilder_CatchesInvalidInput_ThrowsExceptionIfDataSetAndSizesDoNotMatch()
        {
            ISystemComponents components = Substitute.For<ISystemComponents>();

            IFloatTextureBuilder builder = new FloatTextureBuilder(components);

            Assert.Throws<Yak2DException>(() => { builder.GenerateFloat32VeldridTextureFromPixelData(new float[100], 10, 9); });

            components.ReleaseResources();
        }
    }
}