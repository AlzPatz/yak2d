using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class CrtMeshBuilderTest
    {
        [Fact]
        public void CrtMeshBuilder_BuildMesh_FiveDivisionsPiAzimuthRangeReturnsExpectedPointsAndNormals()
        {
            ICrtMeshBuilder builder = new CrtMeshBuilder(new CrtMeshBuilderFunctions(), new VertexLinearGridToTriangleListTool());

            var result = builder.Build(100.0f, 50.0f, 5, 1.0f, 0.0f);

            var centre_pos = result[32].Position;

            Assert.Equal(0.0f, centre_pos.X, 3);
            Assert.Equal(0.0f, centre_pos.Y, 3);
            Assert.Equal(0.0f, centre_pos.Z, 3);

            var topleft = result[0].Position;
            Assert.Equal(0.70710678 * -50.0f, topleft.X, 3);
            Assert.Equal(19.13417243f, topleft.Y, 3);
            Assert.Equal(-50.0f, topleft.Z, 3);

            var bottomright = result[94].Position;
            Assert.Equal(0.70710678 * 50.0f, bottomright.X, 3);
            Assert.Equal(-19.13417243f, bottomright.Y, 3);
            Assert.Equal(-50.0f, bottomright.Z, 3);
        }
    }
}