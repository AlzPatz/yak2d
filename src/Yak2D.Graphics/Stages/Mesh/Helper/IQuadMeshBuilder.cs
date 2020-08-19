namespace Yak2D.Graphics
{
    public interface IQuadMeshBuilder
    {
        Vertex3D[] Build(float width, float height);
    }
}