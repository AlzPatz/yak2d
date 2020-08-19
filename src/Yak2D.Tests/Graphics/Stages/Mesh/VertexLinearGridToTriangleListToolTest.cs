using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class VertexLinearGridToTriangleListToolTest
    {
        [Fact]
        public void VertexGridToTriangleListTool_Convert_TestSimpleQuad()
        {
            /*
             Linear Grid Form..
                0   1
                          ==    0   1   2   3
                2   3

            Triangle List Form..

                0, 3      1
                                == 0 to 6 --> 0, 1, 3, 0, 3, 2 from above
                 5       2, 4
             */ 

            IVertexLinearGridToTriangleListTool tool = new VertexLinearGridToTriangleListTool();

            var halfDimension = 5.0f;

            var vertexLinearGrid = new Vertex3D[]
            {
                new Vertex3D { Position = new Vector3(-halfDimension, halfDimension, 0.0f) },
                new Vertex3D { Position = new Vector3(halfDimension, halfDimension, 0.0f) },
                new Vertex3D { Position = new Vector3(-halfDimension, -halfDimension, 0.0f) },
                new Vertex3D { Position = new Vector3(halfDimension, -halfDimension, 0.0f) },
            };

            var list = tool.Convert(2, 2, vertexLinearGrid);

            Assert.Equal(6, list.Length);

            Assert.Equal(vertexLinearGrid[0].Position.X, list[0].Position.X, 3);
            Assert.Equal(vertexLinearGrid[0].Position.Y, list[0].Position.Y, 3);
            Assert.Equal(vertexLinearGrid[0].Position.Z, list[0].Position.Z, 3);
            Assert.Equal(vertexLinearGrid[0].Position.X, list[3].Position.X, 3);
            Assert.Equal(vertexLinearGrid[0].Position.Y, list[3].Position.Y, 3);
            Assert.Equal(vertexLinearGrid[0].Position.Z, list[3].Position.Z, 3);

            Assert.Equal(vertexLinearGrid[1].Position.X, list[1].Position.X, 3);
            Assert.Equal(vertexLinearGrid[1].Position.Y, list[1].Position.Y, 3);
            Assert.Equal(vertexLinearGrid[1].Position.Z, list[1].Position.Z, 3);

            Assert.Equal(vertexLinearGrid[3].Position.X, list[2].Position.X, 3);
            Assert.Equal(vertexLinearGrid[3].Position.Y, list[2].Position.Y, 3);
            Assert.Equal(vertexLinearGrid[3].Position.Z, list[2].Position.Z, 3);
            Assert.Equal(vertexLinearGrid[3].Position.X, list[4].Position.X, 3);
            Assert.Equal(vertexLinearGrid[3].Position.Y, list[4].Position.Y, 3);
            Assert.Equal(vertexLinearGrid[3].Position.Z, list[4].Position.Z, 3);

            Assert.Equal(vertexLinearGrid[2].Position.X, list[5].Position.X, 3);
            Assert.Equal(vertexLinearGrid[2].Position.Y, list[5].Position.Y, 3);
            Assert.Equal(vertexLinearGrid[2].Position.Z, list[5].Position.Z, 3);
        }
    }
}