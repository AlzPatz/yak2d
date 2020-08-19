using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class SphericalMeshBuilderTest
    {
        [Fact]
        public void SphericalMeshBuilder_CreateMesh_TestPointPosition()
        {
            ISphericalMeshBuilder builder = new SphericalMeshBuilder(new VertexLinearGridToTriangleListTool());

            var result = builder.Build(Vector3.Zero, 100.0f, 50.0f, 100.0f, 0.0f, 9, 9);

            Assert.Equal(384, result.Length);

            var centre = result[216].Position;

            Assert.Equal(0.0f, centre.X, 3);
            Assert.Equal(0.0f, centre.Y, 3);
            Assert.Equal(50.0f, centre.Z, 3);

            var topleft = result[0].Position;

            Assert.Equal(0.0f, topleft.X, 3);
            Assert.Equal(25.0f, topleft.Y, 3);
            Assert.Equal(00.0f, topleft.Z, 3);

            var bottomright = result[383].Position;

            Assert.Equal(0.0f, bottomright.X, 3);
            Assert.Equal(-25.0f, bottomright.Y, 3);
            Assert.Equal(0.0f, bottomright.Z, 3);

        }
    }
}