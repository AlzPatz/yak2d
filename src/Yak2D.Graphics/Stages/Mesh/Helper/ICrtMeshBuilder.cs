namespace Yak2D.Graphics
{
    public interface ICrtMeshBuilder
    {
        Vertex3D[] Build(float width,
                         float height,
                         uint numMeshDivisionsPerAxis,
                         float horizontalAngularCurvatureFraction,
                         float cornerRoundingFraction);
    }
}