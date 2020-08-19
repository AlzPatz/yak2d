using System;
using System.Numerics;

namespace Yak2D.Graphics
{
    public interface ICrtMeshBuilderFunctions
    {
        float CalculateRadiusOfVirtualSphereFromPatchFeatures(float width,
                                                              float halfHorizontalAngularRange);

        Vertex3D[] GenerateSphereSurfacePatchAtUnitRadiusWithExtraVertexOverhangForNormalCalculation(float aspect,
                                                                                                     int vertsPerAxis,
                                                                                                     float halfHorizontalAngularRange,
                                                                                                     float halfVerticalAngularRange,
                                                                                                     float rounding);

        Tuple<float, float> CalculateCornerRadiiFractions(float aspect, float cornerRadius);

        float AdjustFracXForCornerRoundingIfRequired(float horizontalCornerRadiusFraction,
                                                     float verticalCornerRadiusFraction,
                                                     float fracX,
                                                     float fracY);

        void ScaleUpSphereSurfacePatch(Vertex3D[] mesh, float radius);

        void DeformSurfacePatchToFitRequiredDimensions(Vertex3D[] gridOfVertsInLinearArray,
                                                       float width,
                                                       float height);

        Vertex3D[] CalculateNormalsAndRePositionVertices(int vertsPerAxis,
                                                         Vertex3D[] gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation,
                                                         float virtualRadius);
    }
}