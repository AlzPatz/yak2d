using System.Numerics;

namespace Yak2D.Graphics
{
    public interface ISphericalMeshBuilder
    {
        Vertex3D[] Build(Vector3 position, float width, float height, float depth, float rotationClockwiseDegreesAroundPositiveY, uint numHorizontalDivisions, uint numVerticalDivisions);
    }
}