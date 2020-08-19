using System;
using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class CommonOperationsTest
    {
        [Theory]
        [InlineData(0.0f, 0.0f)]
        [InlineData((float)Math.PI * 0.25f, 45.0f)]
        [InlineData((float)Math.PI * 0.5f, 90.0f)]
        [InlineData((float)Math.PI * 1.0f, 180.0f)]
        [InlineData((float)Math.PI * 2.0f, 360.0f)]
        [InlineData((float)Math.PI * -4.0f, -720.0f)]
        public void CommonOperations_RadsToDegress_CheckAgainstSimplePiFractions(float rads, float degrees)
        {
            ICommonOperations common = new CommonOperations();

            Assert.Equal(degrees, common.RadiansToDegrees(rads));
        }

        [Theory]
        [InlineData(0.0f, 0.0f)]
        [InlineData((float)Math.PI * 0.25f, 45.0f)]
        [InlineData((float)Math.PI * 0.5f, 90.0f)]
        [InlineData((float)Math.PI * 1.0f, 180.0f)]
        [InlineData((float)Math.PI * 2.0f, 360.0f)]
        [InlineData((float)Math.PI * -4.0f, -720.0f)]
        public void CommonOperations_DegressToRads_CheckAgainstSimplePiFractions(float rads, float degrees)
        {
            ICommonOperations common = new CommonOperations();

            Assert.Equal(rads, common.DegressToRadians(degrees));
        }

        [Fact]
        public void CommonOperations_RotateVectorClockwise_EnsureSimple90DegreesCase()
        {
            ICommonOperations common = new CommonOperations();

            var vec = Vector2.UnitY;
            var rotated = common.RotateVectorClockwise(vec, 0.5f * (float)Math.PI);

            Assert.Equal(1.0f, rotated.X, 5);
            Assert.Equal(0.0f, rotated.Y, 5);
        }

        [Fact]
        public void CommonOperations_RotateVectorClockwise_EnsureSimple90DegreesCaseRefVersion()
        {
            ICommonOperations common = new CommonOperations();

            var vec = Vector2.UnitY;
            var rotated = common.RotateVectorClockwise(ref vec, 0.5f * (float)Math.PI);

            Assert.Equal(1.0f, rotated.X, 5);
            Assert.Equal(0.0f, rotated.Y, 5);
        }

        [Fact]
        public void CommonOperations_LineAndArrowGenerator_ReturnCorrectDataForArrowCatchesSomeInvalidInput()
        {
            ICommonOperations common = new CommonOperations();

            /*
                    5
                    |\
              / 0---1 \
             4          6
              \ 3---2 /
                    |/
                    7

            4 = (-5, 0)
            5 = (5, 10)

             */

            //Num divisions sent in as 0, which will be corrected to 2
            var result = common.LineAndArrowVertexAndIndicesGenerator(Vector2.Zero, new Vector2(10.0f, 0.0f), 10.0f, true, true, 0, true, 5.0f, 20.0f);

            var verts = result.Item1;
            var indices = result.Item2;

            Assert.Equal(8, verts.Length);

            var topLeftCornerOfArrow = verts[5];
            Assert.Equal(5.0f, topLeftCornerOfArrow.X, 5);
            Assert.Equal(10.0f, topLeftCornerOfArrow.Y, 5);

            var OnlyVertexOnTheCurvedEndThatHappensToLieInTheMiddleY = verts[4];
            Assert.Equal(-5.0f, OnlyVertexOnTheCurvedEndThatHappensToLieInTheMiddleY.X, 5);
            Assert.Equal(0.0f, OnlyVertexOnTheCurvedEndThatHappensToLieInTheMiddleY.Y, 5);

            Assert.Equal(12, indices.Length);
            //Ensuring the index buffer indexes the vertex on the curve end at the expected time
            Assert.Equal(4, indices[8]);

        }

        [Fact]
        public void CommonOperations_RegularPolyGenerator_ReturnCorrectData()
        {
            ICommonOperations common = new CommonOperations();
            /*
                1
              / | \
             /  |  \
            4 --0-- 2
             \  |  /
              \ | /
                3

            3= (-5, 0)
            2= (0, -5)
           
            */

            var result = common.RegularPolygonVertexAndIndicesGenerator(Vector2.Zero, 4, 5.0f);

            var verts = result.Item1;
            var indices = result.Item2;

            Assert.Equal(5, verts.Length);

            var left = verts[4];
            Assert.Equal(-5.0f, left.X, 5);
            Assert.Equal(0.0f, left.Y, 5);

            var bottom = verts[3];
            Assert.Equal(0.0f, bottom.X, 5);
            Assert.Equal(-5.0f, bottom.Y, 5);

            Assert.Equal(12, indices.Length);
            Assert.Equal(1, indices[11]);
            Assert.Equal(3, indices[5]);
        }
    }
}