using System;
using System.Numerics;

namespace Yak2D.Graphics
{
    public class CrtMeshBuilderFunctions : ICrtMeshBuilderFunctions
    {
        public float CalculateRadiusOfVirtualSphereFromPatchFeatures(float width,
                                                                     float halfHorizontalAngularRange)
        {
            if (halfHorizontalAngularRange <= 0)
            {
                return float.MaxValue;
            }

            //Find a position that would be on the edge of the patch on a sphere of radius 1 half way rotated in the vertical. ie.. middle equator line.
            //Then use this x position to calculate the radius of the virtual sphere
            //Horizontal range is never more than 180 degrees, so we know that half horizontal range will be max value / ie edge

            //Left handed coordinate system. Z points away towards positive z

            //inclination = angle from y axis towards z. 0 points straight up, PI is straight down
            //azimuth = angle of rotation in anticlockwise direction from x towards y (which points forward in the away direction)
            //Therefore... centre along x, pointing "towards" camera, ie pointing towards -ve z, is infact an azimth of - half pi

            //z = R.Sin(inclination).Sin(azimuth)
            //x = R.Sin(inclination).Cos(azimuth)
            //y = R.Cos(inclination)

            var inclination = 0.5f * (float)Math.PI;
            var azimuth = (0.5f * (float)Math.PI) + halfHorizontalAngularRange;

            var xOfPointOnEdgeOfPatch = (float)Math.Sin(inclination) * (float)Math.Cos(azimuth);

            var absX = (float)Math.Abs(xOfPointOnEdgeOfPatch);

            var radius = 0.5f * (width / absX);

            return radius;
        }

        public Vertex3D[] GenerateSphereSurfacePatchAtUnitRadiusWithExtraVertexOverhangForNormalCalculation(float aspect,
                                                                                                            int vertsPerAxis,
                                                                                                            float halfHorizontalAngularRange,
                                                                                                            float halfVerticalAngularRange,
                                                                                                            float rounding)
        {
            //We create an extra row and column for each side with an extra vertex used to help with normal calculation later on
            var arrayDimension = vertsPerAxis + 2;
            var linearArraySize = arrayDimension * arrayDimension;

            var vertsInLinearArray = new Vertex3D[linearArraySize];

            var divFraction = 1.0f / (1.0f * (vertsPerAxis - 1));

            var cornerRadius = 0.5f * rounding;

            float fracX, fracY;

            var halfPI = 0.5f * (float)Math.PI;

            var radiiFractions = CalculateCornerRadiiFractions(aspect, cornerRadius);
            var horizontalCornerRadiusFraction = radiiFractions.Item1;
            var verticalCornerRadiusFraction = radiiFractions.Item2;

            for (var ny = -1; ny <= vertsPerAxis; ny++)
            {
                fracY = (float)ny * divFraction;

                for (var nx = -1; nx <= vertsPerAxis; nx++)
                {
                    var index = ((ny + 1) * arrayDimension) + nx + 1;

                    fracX = (float)nx * divFraction;

                    fracX = AdjustFracXForCornerRoundingIfRequired(horizontalCornerRadiusFraction,
                                                                   verticalCornerRadiusFraction,
                                                                   fracX,
                                                                   fracY);

                    vertsInLinearArray[index].TexCoord = new Vector2(fracX, fracY);

                    //Should 'paint' points from left to right (-x to +x). azimuth here is angle clockwise from x towards z which is positive looking into camera. RHS
                    //So do push out into +z facing camera we go negative angular direction
                    var azimuth = halfPI + halfHorizontalAngularRange - (fracX * 2.0f * halfHorizontalAngularRange);

                    //We paint the tex coords y = 0 at the top, so we 'paint' points from top to bottom
                    //which is inclination from up to half pi downwards
                    var inclination = halfPI - halfVerticalAngularRange + (fracY * 2.0f * halfVerticalAngularRange);

                    var H = azimuth;
                    var V = inclination;

                    var sinH = (float)Math.Sin(H);
                    var cosH = (float)Math.Cos(H);

                    var sinV = (float)Math.Sin(V);
                    var cosV = (float)Math.Cos(V);

                    //z = R.Sin(inclination).Sin(azimuth)
                    //x = R.Sin(inclination).Cos(azimuth)
                    //y = R.Cos(inclination)

                    var x = sinV * cosH;
                    var y = cosV;
                    var z = sinV * sinH;

                    vertsInLinearArray[index].Position = new Vector3(x, y, z);
                }
            }

            return vertsInLinearArray;
        }

        public Tuple<float, float> CalculateCornerRadiiFractions(float aspect, float cornerRadius)
        {
            //rescale corner fractions to harmonise eventual physical scaled size. And account for largest dimension via aspect ratio
            float horizontalRadius, verticalRadius;
            if (aspect >= 1.0f)
            {
                //width larger than height
                verticalRadius = cornerRadius;
                horizontalRadius = cornerRadius * (1.0f / aspect);
            }
            else
            {
                //height larger than width
                horizontalRadius = cornerRadius;
                verticalRadius = cornerRadius * aspect;
            }
            return new Tuple<float, float>(horizontalRadius, verticalRadius);
        }

        public float AdjustFracXForCornerRoundingIfRequired(float horizontalCornerRadiusFraction,
                                                            float verticalCornerRadiusFraction,
                                                            float fracX,
                                                            float fracY)
        {
            /*
             For the extra skirt of vertices for normal calculation purposes, we assume they are unadjusted
             */

            if(fracX < 0.0f || fracX > 1.0f || fracY < 0.0f || fracY > 1.0f)
            {
                return fracX;
            }

            var fractionThroughCornerSectionInVerticalDirection = 1.0f;

            if (fracY < verticalCornerRadiusFraction)
            {
                fractionThroughCornerSectionInVerticalDirection = fracY / verticalCornerRadiusFraction;
            }

            if (fracY > 1.0f - verticalCornerRadiusFraction)
            {
                fractionThroughCornerSectionInVerticalDirection = 1.0f - ((fracY - (1.0f - verticalCornerRadiusFraction)) / verticalCornerRadiusFraction);
            }

            if (fractionThroughCornerSectionInVerticalDirection == 1.0f)
            {
                //No adjustment
                return fracX;
            }

            //Idealised spherical corner and some trig to convert vertical fraction (adjacent) to horizontal (opposite)
            var adjacent = 1.0f - fractionThroughCornerSectionInVerticalDirection;
            var opposite = (float)Math.Sqrt(1.0f - (adjacent * adjacent));
            //opposite is 0 -> 1 reprentative horizontal fraction of curve edge of idealised spherical corner radius

            opposite = 1.0f - opposite;

            var perCornerHorizontalFractionalSizeAdjustment = opposite * horizontalCornerRadiusFraction;

            var disFromMiddle = fracX - 0.5f;

            var scaler = 1.0f - (2.0f * perCornerHorizontalFractionalSizeAdjustment);

            disFromMiddle *= scaler;

            return 0.5f + disFromMiddle;
        }

        public void ScaleUpSphereSurfacePatch(Vertex3D[] mesh, float radius)
        {
            for (var n = 0; n < mesh.Length; n++)
            {
                mesh[n].Position = radius * mesh[n].Position;
            }
        }

        public void DeformSurfacePatchToFitRequiredDimensions(Vertex3D[] gridOfVertsInLinearArray,
                                                              float width,
                                                              float height)
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            var numVerts = gridOfVertsInLinearArray.Length;

            for (var n = 0; n < numVerts; n++)
            {
                var pos = gridOfVertsInLinearArray[n].Position;

                if (pos.X < minX)
                {
                    minX = pos.X;
                }

                if (pos.X > maxX)
                {
                    maxX = pos.X;
                }

                if (pos.Y < minY)
                {
                    minY = pos.Y;
                }

                if (pos.Y > maxY)
                {
                    maxY = pos.Y;
                }
            }

            var currentWidth = maxX - minX;
            var currentHeight = maxY - minY;

            var xScale = width / currentWidth;
            var yScale = height / currentHeight;

            for (var n = 0; n < numVerts; n++)
            {
                var pos = gridOfVertsInLinearArray[n].Position;

                gridOfVertsInLinearArray[n].Position = new Vector3(pos.X * xScale, pos.Y * yScale, pos.Z);
            }
        }

        public Vertex3D[] CalculateNormalsAndRePositionVertices(int vertsPerAxis, Vertex3D[] gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation, float radius)
        {
            var mesh = new Vertex3D[vertsPerAxis * vertsPerAxis];

            /* 
                  x and Letter = Vertex

                  x   x  /C\  x   x
                        / | \
                  x   D---A---B   x
                       \  | /
                  x   x  \E/  x   x


                  Vertex Normal of A == Normalise(ABxAC + ACxAD + ADxAE + AExAB)
            */

            var dimensionOfLargerGrid = vertsPerAxis + 2;

            for (var ny = 0; ny < vertsPerAxis; ny++)
            {
                for (var nx = 0; nx < vertsPerAxis; nx++)
                {
                    //(nx,ny) position in final grid
                    //(x,y) position in existing larger grid with normal helper curtain

                    var x = nx + 1;
                    var y = ny + 1;

                    var indexLargeGrid = (dimensionOfLargerGrid * y) + x;

                    /* 
                         x and Letter = Vertex

                         x   x  /C\  x   x
                               / | \
                         x   D---A---B   x
                              \  | /
                         x   x  \E/  x   x


                         Vertex Normal of A == Normalise(ABxAC + ACxAD + ADxAE + AExAB)
                     */

                    var A = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid].Position;
                    var B = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid + 1].Position;
                    var C = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid - dimensionOfLargerGrid].Position;
                    var D = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid - 1].Position;
                    var E = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid + dimensionOfLargerGrid].Position;

                    var sum = Vector3.Cross(B - A, C - A) + Vector3.Cross(C - A, D - A) + Vector3.Cross(D - A, E - A) + Vector3.Cross(E - A, B - A);

                    var indexFinalGrid = (ny * vertsPerAxis) + nx;

                    mesh[indexFinalGrid].Position = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid].Position;
                    mesh[indexFinalGrid].TexCoord = gridOfVerticesInLinearArrayWithExtraVertexOverhangForNormalCalculation[indexLargeGrid].TexCoord;
                    mesh[indexFinalGrid].Normal = Vector3.Normalize(mesh[indexFinalGrid].Position); //-Vector3.UnitZ; //Vector3.Normalize(sum);
                }
            }

            //Mesh created with MinZ position for a vertex being at z=0 -> vertices fall away into deeper negative z
            for (var n = 0; n < mesh.Length; n++)
            {
                mesh[n].Position = new Vector3(mesh[n].Position.X,
                                               mesh[n].Position.Y,
                                               mesh[n].Position.Z - radius);
            }

            return mesh;
        }
    }
}