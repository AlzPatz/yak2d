using System;

namespace Yak2D.Graphics
{
    public class CrtMeshBuilder : ICrtMeshBuilder
    {
        private readonly ICrtMeshBuilderFunctions _builderFunctions;
        private readonly IVertexLinearGridToTriangleListTool _vertexGridToTriangleListTool;

        public CrtMeshBuilder(  ICrtMeshBuilderFunctions builderFunctions,
                                IVertexLinearGridToTriangleListTool vertexGridToTriangleListTool)
        {
            _builderFunctions = builderFunctions;
            _vertexGridToTriangleListTool = vertexGridToTriangleListTool;
        }

        //Take a patch on a sphere's surface defined by horizontal and vertical angular size 
        //and scale into desired width / height curved section

        public Vertex3D[] Build(float width,
                                float height,
                                uint numMeshDivisionsPerAxis,
                                float horizontalAngularCurvatureFraction,
                                float cornerRoundingFraction)
        {
            if (width <= 0.0f)
            {
                width = 1.0f;
            }

            if (height <= 0.0f)
            {
                height = 1.0f;
            }

            if (numMeshDivisionsPerAxis <= 2)
            {
                numMeshDivisionsPerAxis = 3;
                //Three is a minimum, corresponds to two edge points and one in the centre
                //using 2 would mean a flat surface with edge points only
            }

            horizontalAngularCurvatureFraction = Utility.Clamper.Clamp(horizontalAngularCurvatureFraction, 0.0f, 1.0f);

            cornerRoundingFraction = Utility.Clamper.Clamp(cornerRoundingFraction, 0.0f, 1.0f);

            return Generate((int)numMeshDivisionsPerAxis, width, height, horizontalAngularCurvatureFraction, cornerRoundingFraction);
        }

        /*
		SPHERICAL COORDINATES - CREATE SURFACE PATCH
		Coordinate System Here is RHS, Y Vertical.
		V = Vertical Angle [inclination] .. (around horizontal X axis, 0 is pointing straight up)
		H = Horizontal Angle [azimuth] .. (around vertical Y axis, positive is clockwise from positive x direction towards positive z)
	
		z = R.Sin(V).Sin(H)
		x = R.Sin(V).Cos(H)
		y = R.Cos(V)
		*/

        private Vertex3D[] Generate(int divisionsPerAxis,
                                    float width,
                                    float height,
                                    float horizontalAngularCurvature,
                                    float cornerRounding)
        {
            var aspect = width / height;

            var approximateVerticalAngularCurvature = horizontalAngularCurvature / aspect;

            //Cannot be more than 1.0f
            if (approximateVerticalAngularCurvature > 1.0f)
            {
                approximateVerticalAngularCurvature = 1.0f;
            }

            var halfHorizontalAngularRange = 0.5f * (float)Math.PI * horizontalAngularCurvature;
            var approxHalfVerticalAngularRange = 0.5f * (float)Math.PI * approximateVerticalAngularCurvature;

            var virtualRadius = _builderFunctions.CalculateRadiusOfVirtualSphereFromPatchFeatures(width, halfHorizontalAngularRange);

            var gridOfVerticesInLinearArray = _builderFunctions.GenerateSphereSurfacePatchAtUnitRadiusWithExtraVertexOverhangForNormalCalculation(aspect,
                                                                                                                  divisionsPerAxis,
                                                                                                                  halfHorizontalAngularRange,
                                                                                                                  approxHalfVerticalAngularRange,
                                                                                                                  cornerRounding);

            _builderFunctions.ScaleUpSphereSurfacePatch(gridOfVerticesInLinearArray, virtualRadius);

            _builderFunctions.DeformSurfacePatchToFitRequiredDimensions(gridOfVerticesInLinearArray, width, height);

            var crtVerts = _builderFunctions.CalculateNormalsAndRePositionVertices(divisionsPerAxis, gridOfVerticesInLinearArray, virtualRadius);

            return _vertexGridToTriangleListTool.Convert((uint)divisionsPerAxis, (uint)divisionsPerAxis, crtVerts);
        }
    }
}