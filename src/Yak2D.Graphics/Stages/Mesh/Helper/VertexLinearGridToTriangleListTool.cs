namespace Yak2D.Graphics
{
    public class VertexLinearGridToTriangleListTool : IVertexLinearGridToTriangleListTool
    {
        public Vertex3D[] Convert(uint numHDivs, uint numVDivs, Vertex3D[] grid)
        {
            var numH = (int)numHDivs;
            var numV = (int)numVDivs;

            var numRectsH = numH - 1;
            var numRectsV = numV - 1;

            var numTriangles = 2 * numRectsH * numRectsV;

            var numVerticesInTotal = 3 * numTriangles;

            var vertices = new Vertex3D[numVerticesInTotal];

            var vCount = 0;

            int x, y, index;

            for (var yi = 0; yi < numVDivs - 1; yi++)
            {
                var yf = yi + 1;

                for (var xi = 0; xi < numHDivs - 1; xi++)
                {
                    var xf = xi + 1;

                    x = xi;
                    y = yi;
                    index = (y * numH) + x;
                    vertices[vCount] = grid[index];
                    vCount++;

                    x = xf;
                    y = yi;
                    index = (y * numH) + x;
                    vertices[vCount] = grid[index];
                    vCount++;

                    x = xf;
                    y = yf;
                    index = (y * numH) + x;
                    vertices[vCount] = grid[index];
                    vCount++;

                    x = xi;
                    y = yi;
                    index = (y * numH) + x;
                    vertices[vCount] = grid[index];
                    vCount++;

                    x = xf;
                    y = yf;
                    index = (y * numH) + x;
                    vertices[vCount] = grid[index];
                    vCount++;

                    x = xi;
                    y = yf;
                    index = (y * numH) + x;
                    vertices[vCount] = grid[index];
                    vCount++;
                }
            }

            return vertices;
        }
    }
}