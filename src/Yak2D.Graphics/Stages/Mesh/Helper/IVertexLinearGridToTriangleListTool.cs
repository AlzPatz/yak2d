namespace Yak2D.Graphics
{
    public interface IVertexLinearGridToTriangleListTool
    {
        Vertex3D[] Convert(uint numHDivs, uint numVDivs, Vertex3D[] vertGrid);
    }
}