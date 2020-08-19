using System.Numerics;

namespace Yak2D.Graphics
{
    public interface IRectangularCuboidMeshBuilder
    {
        Vertex3D[] Build(Vector3 position,
                         float width,
                         float height,
                         float depth,
                         float ClockwiseRotationDegreesAroundY,
                         RectangularCuboidMeshTexCoords? texCoords = null);
    }
}