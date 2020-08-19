using System;
using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class CrtMeshBuilderFunctionsTest
    {
        [Theory]
        [InlineData(100.0f, 1.57079632679f, 50.0f)] //180 degree horizontal curve (half PI input)
        [InlineData(100.0f, 0.78539816339f, 70.71067810f)] //90 degree horizontal curve (quarter PI input)
        [InlineData(100.0f, 0.0f, 3.40282347E+38f)] //radius is inifinity @ 0 angular variance, but catch this and return max value
        [InlineData(100.0f, -1.0f, 3.40282347E+38f)] //catch negatives similarly
        public void CrtMeshBuilderFunctions_CalculateRadiusOfVirtualSphere_ValidateAlgorithmResults(float patchWidth, float halfHorizontalAngularRange, float expectedRadius)
        {
            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var radius = functions.CalculateRadiusOfVirtualSphereFromPatchFeatures(patchWidth, halfHorizontalAngularRange);

            Assert.Equal(expectedRadius, radius, 3);
        }

        [Fact]
        public void CrtMeshBuilderFunctions_GenerateApproximateSphereAtUnitRadius_TestSimple180DegreeAzimuthAndInclination()
        {
            //Basically creates have a sphere covered, so top and bottom verts are all at the poles

            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var verts = functions.GenerateSphereSurfacePatchAtUnitRadiusWithExtraVertexOverhangForNormalCalculation(1.0f,
                                                                                                                    3,
                                                                                                                    0.5f * (float)Math.PI,
                                                                                                                    0.5f * (float)Math.PI,
                                                                                                                    0.0f);

            var centre = verts[12];
            var cpos = centre.Position;
            var ctex = centre.TexCoord;

            Assert.Equal(1.0f, cpos.Z, 3);
            Assert.Equal(0.0f, cpos.X, 3);
            Assert.Equal(0.0f, cpos.Y, 3);

            Assert.Equal(0.5f, ctex.X, 3);
            Assert.Equal(0.5f, ctex.Y, 3);

            var topleft = verts[6];
            var tpos = topleft.Position;
            var ttex = topleft.TexCoord;

            Assert.Equal(0.0f, tpos.Z, 3);
            Assert.Equal(0.0f, tpos.X, 3);
            Assert.Equal(1.0f, tpos.Y, 3);

            Assert.Equal(0.0f, ttex.X, 3);
            Assert.Equal(0.0f, ttex.Y, 3);

            var bottomright = verts[18];
            var bpos = bottomright.Position;
            var btex = bottomright.TexCoord;

            Assert.Equal(0.0f, bpos.Z, 3);
            Assert.Equal(0.0f, bpos.X, 3);
            Assert.Equal(-1.0f, bpos.Y, 3);

            Assert.Equal(1.0f, btex.X, 3);
            Assert.Equal(1.0f, btex.Y, 3);
        }

        [Fact]
        public void CrtMeshBuilderFunctions_GenerateApproximateSphereAtUnitRadius_TestSimple90DegreeAzimuthAndInclination()
        {
            //Covers the centre one quarter of one side of a sphere

            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var verts = functions.GenerateSphereSurfacePatchAtUnitRadiusWithExtraVertexOverhangForNormalCalculation(1.0f,
                                                                                                                    3,
                                                                                                                    0.25f * (float)Math.PI,
                                                                                                                    0.25f * (float)Math.PI,
                                                                                                                    0.0f);

            var ort = 0.70710678118f; // 1/sqrt(2)

            var centre = verts[12];
            var cpos = centre.Position;
            var ctex = centre.TexCoord;

            Assert.Equal(1.0f, cpos.Z, 3);
            Assert.Equal(0.0f, cpos.X, 3);
            Assert.Equal(0.0f, cpos.Y, 3);

            Assert.Equal(0.5f, ctex.X, 3);
            Assert.Equal(0.5f, ctex.Y, 3);

            var topleft = verts[6];
            var tpos = topleft.Position;
            var ttex = topleft.TexCoord;

            Assert.Equal(0.5f, tpos.Z, 3);
            Assert.Equal(-0.5f, tpos.X, 3);
            Assert.Equal(ort, tpos.Y, 3);

            Assert.Equal(0.0f, ttex.X, 3);
            Assert.Equal(0.0f, ttex.Y, 3);

            var bottomright = verts[18];
            var bpos = bottomright.Position;
            var btex = bottomright.TexCoord;

            Assert.Equal(0.5f, bpos.Z, 3);
            Assert.Equal(0.5f, bpos.X, 3);
            Assert.Equal(-ort, bpos.Y, 3);

            Assert.Equal(1.0f, btex.X, 3);
            Assert.Equal(1.0f, btex.Y, 3);
        }

        [Fact]
        public void CrtMeshBuilderFunctions_GenerateApproximateSphereAtUnitRadius_TestSimpleCornerRounding()
        {
            //Covers the centre one quarter of one side of a sphere

            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var verts = functions.GenerateSphereSurfacePatchAtUnitRadiusWithExtraVertexOverhangForNormalCalculation(1.0f,
                                                                                                                    4,
                                                                                                                    0.25f * (float)Math.PI,
                                                                                                                    0.25f * (float)Math.PI,
                                                                                                                    1.0f);
            /*
             
            0  1  2  3
            4  5  6  7
            8  9  10 11
            12 13 14 15

            4 and 11 should be half way along corner radii

            */

            var ort = 0.70710678118f; // 1/sqrt(2)

            var topleft = verts[8];
            var tpos = topleft.Position;
            var ttex = topleft.TexCoord;

            Assert.Equal(ort, tpos.Z, 3);
            Assert.Equal(0.0f, tpos.X, 3);
            Assert.Equal(0.707106f, tpos.Y, 3);

            Assert.Equal(0.5f, ttex.X, 3);
            Assert.Equal(0.0f, ttex.Y, 3);

            var bottomright = verts[28];
            var bpos = bottomright.Position;
            var btex = bottomright.TexCoord;

            Assert.Equal(ort, bpos.Z, 3);
            Assert.Equal(0.0f, bpos.X, 3);
            Assert.Equal(-0.707106f, bpos.Y, 3);

            Assert.Equal(0.5f, btex.X, 3);
            Assert.Equal(1.0f, btex.Y, 3);


            var leftthird = verts[13];
            var lpos = leftthird.Position;
            var ltex = leftthird.TexCoord;

            Assert.Equal(0.712992f, lpos.Z, 3);
            Assert.Equal(-0.651654f, lpos.X, 3);
            Assert.Equal(0.25881f, lpos.Y, 3);

            Assert.Equal(0.0285954f, ltex.X, 3);
            Assert.Equal(0.333f, ltex.Y, 3);

            var rightlastthird = verts[22];
            var rpos = rightlastthird.Position;
            var rtex = rightlastthird.TexCoord;

            Assert.Equal(0.712992f, rpos.Z, 3);
            Assert.Equal(0.651654f, rpos.X, 3);
            Assert.Equal(-0.25881f, rpos.Y, 3);

            Assert.Equal(0.97140f, rtex.X, 3);
            Assert.Equal(0.6666f, rtex.Y, 3);
        }

        [Theory]
        [InlineData(1.0f, 1.0f, 1.0f, 1.0f)]
        [InlineData(2.0f, 1.0f, 0.5f, 1.0f)]
        [InlineData(0.5f, 1.0f, 1.0f, 0.5f)]
        public void CrtMeshBuilderFunctions_CalculateCornerRadiiFractions(float aspect, float cornerRadius, float horizontal, float vertical)
        {
            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var result = functions.CalculateCornerRadiiFractions(aspect, cornerRadius);

            Assert.Equal(horizontal, result.Item1);
            Assert.Equal(vertical, result.Item2);
        }

        [Theory]
        [InlineData(0.5f, 0.5f, 1.0f, 1.0f, 0.5f)]
        [InlineData(0.25f, 0.25f, 1.0f, 0.75f, 1.0f)]
        [InlineData(0.25f, 0.25f, 1.0f, 0.25f, 1.0f)]
        [InlineData(0.25f, 0.25f, 1.0f, 0.875f, 0.75f + (0.866025 * 0.25))]
        [InlineData(0.25f, 0.25f, 1.0f, 0.125f, 0.75f + (0.866025 * 0.25))]
        public void CrtMeshBuilderFunctions_AdjustFracXForCornerRoundingIfRequired(float horizontalCornerRadiusFraction,
                                                                                   float verticalCornerRadiusFraction,
                                                                                   float fracX,
                                                                                   float fracY,
                                                                                   float result)
        {
            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            Assert.Equal(result, functions.AdjustFracXForCornerRoundingIfRequired(horizontalCornerRadiusFraction,
                                                                                  verticalCornerRadiusFraction,
                                                                                  fracX,
                                                                                  fracY), 3);
        }

        [Fact]
        public void CrtMeshBuilderFunctions_ScaleUpSurfacePatch_BasicTest()
        {
            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var vec = new Vector3(17.0f, 56.0f, 43.0f);
            vec *= 1.0f / vec.Length();

            var vertices = new Vertex3D[] { new Vertex3D { Position = vec } };

            vec *= 12.0f;

            functions.ScaleUpSphereSurfacePatch(vertices, 12.0f);

            Assert.Equal(vec.X, vertices[0].Position.X);
            Assert.Equal(vec.Y, vertices[0].Position.Y);
            Assert.Equal(vec.Z, vertices[0].Position.Z);
        }

        [Fact]
        public void CrtMeshBuilderFunctions_DeformSurfacePatch_SimpleTest()
        {
            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var vertices = new Vertex3D[]
            {
                new Vertex3D { Position = new Vector3(-10.0f, 10.0f, 0.0f) },
                new Vertex3D { Position = new Vector3(10.0f, 10.0f, 0.0f) },
                new Vertex3D { Position = new Vector3(-10.0f, -10.0f, 0.0f) },
                new Vertex3D { Position = new Vector3(10.0f, -10.0f, 0.0f) }
            };

            functions.DeformSurfacePatchToFitRequiredDimensions(vertices, 10.0f, 7.0f);

            var topleft = vertices[0].Position;
            Assert.Equal(-5.0f, topleft.X);
            Assert.Equal(3.5f, topleft.Y);

            var bottomright = vertices[3].Position;
            Assert.Equal(5.0f, bottomright.X);
            Assert.Equal(-3.5f, bottomright.Y);
        }

        [Fact]
        public void CrtMeshBuilderFunctions_CalculateNormalsAndShiftPositions_SimpleTest()
        {
            ICrtMeshBuilderFunctions functions = new CrtMeshBuilderFunctions();

            var vertices = new Vertex3D[]
            {
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = new Vector3(-10.0f, 10.0f, 10.0f) },
                new Vertex3D { Position = new Vector3(10.0f, 10.0f, 10.0f) },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = new Vector3(-10.0f, -10.0f, 10.0f) },
                new Vertex3D { Position = new Vector3(10.0f, -10.0f, 10.0f) },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero },
                new Vertex3D { Position = Vector3.Zero }
            };

            var normtopleft = vertices[5].Position / vertices[5].Position.Length();

            var result = functions.CalculateNormalsAndRePositionVertices(2, vertices, 10.0f);

            var topleft = result[0].Position;
            Assert.Equal(0.0f, topleft.Z, 3);
            Assert.Equal(normtopleft.X, result[0].Normal.X, 3);
            Assert.Equal(normtopleft.Y, result[0].Normal.Y, 3);
            Assert.Equal(normtopleft.Z, result[0].Normal.Z, 3);

            var bottomright = result[3].Position;
            Assert.Equal(0.0f, bottomright.Z, 3);
        }
    }
}