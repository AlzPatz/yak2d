using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class RectangularCuboidMeshBuilderTest
    {
        [Fact]
        public void SphericalMeshBuilder_CreateMesh_TestPointPosition()
        {
            IRectangularCuboidMeshBuilder builder = new RectangularCuboidMeshBuilder();

            var result = builder.Build(new Vector3(25.0f, 50.0f, 75.0f),
                                       120.0f,
                                       60.0f,
                                       180.0f,
                                       90.0f);

            //Rect 120x60x180 wxhxd, rotatead 90 degress cw around vertical y axis and positioned with centre
            //at 25,50,75


            /*
                 Represents the Texture Coordinates used to map a single 
                 Texture onto a 6 sided rectangular cuboid (rectangle type cube) mesh

                 Coordinate System is LHS, with y upwards and z positive into camera

                 The faces are set out on the cube such that
                 1 to 4: side faces, with side one facing along z (into camera) for an unrotated cube mesh (up is positive y)
                     1 = back face, normal along positive z
                     2 = right face, normal along positive x
                     3 = front face, normal along negative z
                     4 = left face, normal along negative x
                 side 5 is top face. with the top of that 2d area being furthest along positive z in the above example
                 side 6 is bottom face. orientated so that upon rotating the above mentioned around the x axis the 
                 top and bottom face would appear the same way up when facing the camera
                 i.e. when unrotated the top of the 2d area will be closest to camera upside down.
                 a little tricky to describe - but should be roughly "as expected"

                 Most use cases will use the standard / auto generated coordinates. please note
                 the texture is split into 6 regions. 2 rows of 3, with each face assigned as:

                 [1][2][3]
                 [4][5][6]
            */

            //Front Face [#3], top right vertex #3 in list. Zero based etc
            var ffVert = result[(2 * 6) + 2];

            var ffPos = ffVert.Position;
            var ffTex = ffVert.TexCoord;
            var ffNorm = ffVert.Normal;

            Assert.Equal(-90.0f + 25.0f, ffPos.X, 3);
            Assert.Equal(30.0f + 50.0f, ffPos.Y, 3);
            Assert.Equal(60.0f + 75.0f, ffPos.Z, 3);
            
            Assert.Equal(1.0f, ffTex.X, 3);
            Assert.Equal(0.0f, ffTex.Y, 3);

            Assert.Equal(-1.0f, ffNorm.X, 3);
            Assert.Equal(0.0f, ffNorm.Y, 3);
            Assert.Equal(0.0f, ffNorm.Z, 3);

            //Bottom Face [#6], bottom left vertex #1 in list. Zero based etc
            var bfVert = result[(5 * 6) + 0];

            var bfPos = bfVert.Position;
            var bfTex = bfVert.TexCoord;
            var bfNorm = bfVert.Normal;

            Assert.Equal(90.0f + 25.0f, bfPos.X, 3);
            Assert.Equal(-30.0f + 50.0f, bfPos.Y, 3);
            Assert.Equal(60.0f + 75.0f, bfPos.Z, 3);

            Assert.Equal(2.0f / 3.0f, bfTex.X, 3);
            Assert.Equal(1.0f, bfTex.Y, 3);

            Assert.Equal(0.0f, bfNorm.X, 3);
            Assert.Equal(-1.0f, bfNorm.Y, 3);
            Assert.Equal(0.0f, bfNorm.Z, 3);
        }
    }
}