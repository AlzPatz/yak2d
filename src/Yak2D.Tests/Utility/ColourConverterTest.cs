using Xunit;
using Yak2D.Utility;

namespace Yak2D.Tests
{
    public class ColourConverterTest
    {
        [Fact]
        public void ConvertsColourToRgbaFloat()
        {
            var colour = new Colour(1.0f, 0.5f, 0.25f, 0.125f);

            var rgbaFloat = ColourConverter.ConvertToRgbaFloat(colour);

            Assert.Equal(1.0f, rgbaFloat.R);
            Assert.Equal(0.5f, rgbaFloat.G);
            Assert.Equal(0.25f, rgbaFloat.B);
            Assert.Equal(0.125f, rgbaFloat.A);
        }

        [Fact]
        public void ConvertsColourToVector4()
        {
            var colour = new Colour(1.0f, 0.5f, 0.25f, 0.125f);

            var vec4 = ColourConverter.ConvertToVec4(colour);

            Assert.Equal(1.0f, vec4.X);
            Assert.Equal(0.5f, vec4.Y);
            Assert.Equal(0.25f, vec4.Z);
            Assert.Equal(0.125f, vec4.W);
        }
    }
}