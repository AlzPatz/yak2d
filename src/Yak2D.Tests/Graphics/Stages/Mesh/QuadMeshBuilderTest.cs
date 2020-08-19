using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class QuadMeshBuilderTest
    {
        [Fact]
        public void QuadMeshTest_SimpleTest_TestTwoPointsReturnsExpected()
        {
            IQuadMeshBuilder builder = new QuadMeshBuilder();

            var result = builder.Build(100.0f, 100.0f);

            Assert.Equal(-50.0f, result[0].Position.X);
            Assert.Equal(50.0f, result[4].Position.Y);
        }
    }
}