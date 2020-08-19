using System.Numerics;
using Xunit;
using Yak2D.Utility;

namespace Yak2D.Tests
{
    //Added for completeness, but essentially just testing basic numeric operations ...

    public class InterpolatorTest
    {
        [Fact]
        public void Interpolator_Float_ReturnsMidPoint()
        {
            var initial = 3.0f;
            var final = 6.0f;
            var fraction = 0.5f;

            var result = Interpolator.Interpolate(initial, final, ref fraction);

            Assert.Equal(4.5f, result);
        }

        [Fact]
        public void Interpolator_Int_ReturnsMidPoint()
        {
            var initial = 3;
            var final = 7;
            var fraction = 0.5f;

            var result = Interpolator.Interpolate(initial, final, ref fraction);

            Assert.Equal(5, result);
        }

        [Fact]
        public void Interpolator_Vector2_ReturnsMidPoint()
        {
            var initial = new Vector2(0.0f, 3.0f);
            var final = new Vector2(4.0f, 7.0f);
            var fraction = 0.5f;

            var result = Interpolator.Interpolate(initial, final, ref fraction);

            Assert.Equal(new Vector2(2.0f, 5.0f), result);
        }

        [Fact]
        public void Interpolator_Vector3_ReturnsMidPoint()
        {
            var initial = new Vector3(0.0f, 3.0f, 4.0f);
            var final = new Vector3(4.0f, 7.0f, -10.0f);
            var fraction = 0.5f;

            var result = Interpolator.Interpolate(initial, final, ref fraction);

            Assert.Equal(new Vector3(2.0f, 5.0f, -3.0f), result);
        }

        [Fact]
        public void Interpolator_Vector4_ReturnsMidPoint()
        {
            var initial = new Vector4(0.0f, 3.0f, 4.0f, -20.0f);
            var final = new Vector4(4.0f, 7.0f, -10.0f, 135.0f);
            var fraction = 0.5f;

            var result = Interpolator.Interpolate(initial, final, ref fraction);

            Assert.Equal(new Vector4(2.0f, 5.0f, -3.0f, 57.5f), result);
        }

        [Fact]
        public void Interpolator_Colour_ReturnsMidPoint()
        {
            var initial = new Colour(1.0f, 0.0f, 0.5f, 0.0f);
            var final = new Colour(0.5f, 0.5f, 0.7f, 0.5f);
            var fraction = 0.5f;

            var result = Interpolator.Interpolate(initial, final, ref fraction);

            Assert.Equal(new Colour(0.75f, 0.25f, 0.6f, 0.25f), result);
        }
    }
}