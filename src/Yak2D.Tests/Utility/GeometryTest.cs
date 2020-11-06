using System;
using System.Numerics;
using Xunit;
using Yak2D.Utility;

namespace Yak2D.Tests
{
    public class GeometryTest
    {
        [Fact]
        public void DegreesToRadians_SimpleChecks()
        {
            Assert.Equal(0.0f, Utility.Geometry.DegreesToRadians(0.0f), 5);
            Assert.Equal((float)Math.PI / 2.0f, Utility.Geometry.DegreesToRadians(90.0f), 5);
            Assert.Equal((float)Math.PI, Utility.Geometry.DegreesToRadians(180.0f), 5);
            Assert.Equal((float)Math.PI * 3.0f / 2.0f, Utility.Geometry.DegreesToRadians(270.0f), 5);
            Assert.Equal((float)Math.PI * 2.0f, Utility.Geometry.DegreesToRadians(360.0f), 5);
            Assert.Equal(-(float)Math.PI, Utility.Geometry.DegreesToRadians(-180.0f), 5);
            Assert.Equal((float)Math.PI * 4.0f, Utility.Geometry.DegreesToRadians(720.0f), 5);
        }

        [Fact]
        public void RotateVectorClockwise_SimpleChecks()
        {
            Assert.Equal(-Vector2.UnitY, Utility.Geometry.RotateVectorClockwise(Vector2.UnitX, 0.5f * (float)Math.PI));
            Assert.Equal(-Vector2.UnitX, Utility.Geometry.RotateVectorClockwise(-Vector2.UnitY, 0.5f * (float)Math.PI));
            Assert.Equal(Vector2.UnitY, Utility.Geometry.RotateVectorClockwise(-Vector2.UnitX, 0.5f * (float)Math.PI));
            Assert.Equal(Vector2.UnitX, Utility.Geometry.RotateVectorClockwise(Vector2.UnitY, 0.5f * (float)Math.PI));
        }
    }
}