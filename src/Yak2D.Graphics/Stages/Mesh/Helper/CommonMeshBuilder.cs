using System.Numerics;

namespace Yak2D.Graphics
{
    public class CommonMeshBuilder : ICommonMeshBuilder
    {
        private ICrtMeshBuilder _crtMeshBuilder;
        private ISphericalMeshBuilder _sphericalMeshBuilder;
        private IRectangularCuboidMeshBuilder _rectangularCuboidMeshBuilder;
        private IQuadMeshBuilder _quadMeshBuilder;

        public CommonMeshBuilder(ICrtMeshBuilder crtMeshBuilder,
                                    ISphericalMeshBuilder sphericalMeshBuilder,
                                    IRectangularCuboidMeshBuilder rectangularCuboidMeshBuilder,
                                    IQuadMeshBuilder quadMeshBuilder)
        {
            _crtMeshBuilder = crtMeshBuilder;
            _sphericalMeshBuilder = sphericalMeshBuilder;
            _rectangularCuboidMeshBuilder = rectangularCuboidMeshBuilder;
            _quadMeshBuilder = quadMeshBuilder;
        }

        public Vertex3D[] CreateCrtMesh(float width, float height, uint numMeshDivisionsPerAxis, float horizontalAngularCurvatureFraction, float cornerRoundingFraction)
        {
            return _crtMeshBuilder.Build(width, height, numMeshDivisionsPerAxis, horizontalAngularCurvatureFraction, cornerRoundingFraction);
        }

        public Vertex3D[] CreateQuadMesh(float width, float height)
        {
            return _quadMeshBuilder.Build(width, height);
        }

        public Vertex3D[] CreateRectangularCuboidMesh(Vector3 position, float width, float height, float depth, float rotationDegreesAroundY, RectangularCuboidMeshTexCoords? textureCoords = null)
        {
            return _rectangularCuboidMeshBuilder.Build(position, width, height, depth, rotationDegreesAroundY, textureCoords);
        }

        public Vertex3D[] CreateSphericalMesh(Vector3 position, float width, float height, float depth, float rotationDegreesClockwiseYAxis, uint numDivsHorizontal, uint numDivsVertical)
        {
            return _sphericalMeshBuilder.Build(position, width, height, depth, rotationDegreesClockwiseYAxis, numDivsHorizontal, numDivsVertical);
        }
    }
}