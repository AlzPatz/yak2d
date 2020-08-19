using System;
using System.Numerics;

namespace Yak2D.Graphics
{
    public class RectangularCuboidMeshBuilder : IRectangularCuboidMeshBuilder
    {
        public Vertex3D[] Build(Vector3 position, float width, float height, float depth, float ClockwiseRotationDegreesAroundY, RectangularCuboidMeshTexCoords? texCoords = null)
        {
            if (width <= 0.0f)
            {
                width = 1.0f;
            }

            if (height <= 0.0f)
            {
                height = 1.0f;
            }

            if (depth <= 0.0f)
            {
                depth = 1.0f;
            }

            var tex = texCoords == null ? RectangularCuboidMeshTexCoords.StandardCuboidTexCoords() : texCoords.Value;

            var rads = ClockwiseRotationDegreesAroundY * (float)Math.PI / 180.0f;

            var twoPi = (float)Math.PI * 2.0f;

            //Don't really need to clamp to range here but why not..

            while (rads >= twoPi)
            {
                rads -= twoPi;
            }

            while (rads < 0.0f)
            {
                rads += twoPi;
            }

            return GenerateMesh(ref position, ref width, ref height, ref depth, ref rads, ref tex);
        }

        private Vertex3D[] GenerateMesh(ref Vector3 position, ref float width, ref float height, ref float depth, ref float cwRadsAroundY, ref RectangularCuboidMeshTexCoords tex)
        {
            var rotationMatrix = Matrix4x4.CreateRotationY(-cwRadsAroundY);

            var normals = new Vector3[]
            {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
            };

            for (var n = 0; n < 6; n++)
            {
                //Extra normalise not really needed, but might clean up some fp rounding errors
                normals[n] = Vector3.Normalize(Vector3.Transform(normals[n], rotationMatrix));
            }

            /*
                Coordinate System is RHS, with y upwards and z positive into camera

                The faces are set out on the cube such that
                1 to 4: side faces, with side one facing along z (into camera) for an unrotated cube mesh (up is positive y)
                    1 = back face, normal along negative z
                    2 = right face, normal along positive x
                    3 = front face, normal along positive z
                    4 = left face, normal along negative x
                side 5 is top face. with the top of that 2d area being furthest along negative z in the above example
                side 6 is bottom face. orientated so that upon rotating the above mentioned around the x axis the 
                top and bottom face would appear the same way up when facing the camera
                i.e. when unrotated the top of the 2d area will be closest to camera upside down (highest z value).
                a little tricky to describe - but should be roughly "as expected"
             */

            var mesh = new Vertex3D[]
            {
                //Face 1
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, -1.0f),
                    TexCoord = tex.One.BottomLeft,
                    Normal = normals[0]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, -1.0f),
                    TexCoord = tex.One.TopLeft,
                    Normal = normals[0]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, -1.0f),
                    TexCoord = tex.One.TopRight,
                    Normal = normals[0]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, -1.0f),
                    TexCoord = tex.One.BottomLeft,
                    Normal = normals[0]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, -1.0f),
                    TexCoord = tex.One.TopRight,
                    Normal = normals[0]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, -1.0f),
                    TexCoord = tex.One.BottomRight,
                    Normal = normals[0]
                },

                //Face 2
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Two.BottomLeft,
                    Normal = normals[1]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Two.TopLeft,
                    Normal = normals[1]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, -1.0f),
                    TexCoord = tex.Two.TopRight,
                    Normal = normals[1]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Two.BottomLeft,
                    Normal = normals[1]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, -1.0f),
                    TexCoord = tex.Two.TopRight,
                    Normal = normals[1]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, -1.0f),
                    TexCoord = tex.Two.BottomRight,
                    Normal = normals[1]
                },

                //Face 3
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Three.BottomLeft,
                    Normal = normals[2]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Three.TopLeft,
                    Normal = normals[2]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Three.TopRight,
                    Normal = normals[2]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Three.BottomLeft,
                    Normal = normals[2]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Three.TopRight,
                    Normal = normals[2]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Three.BottomRight,
                    Normal = normals[2]
                },

                //Face 4
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, -1.0f),
                    TexCoord = tex.Four.BottomLeft,
                    Normal = normals[3]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, -1.0f),
                    TexCoord = tex.Four.TopLeft,
                    Normal = normals[3]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Four.TopRight,
                    Normal = normals[3]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, -1.0f),
                    TexCoord = tex.Four.BottomLeft,
                    Normal = normals[3]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Four.TopRight,
                    Normal = normals[3]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Four.BottomRight,
                    Normal = normals[3]
                },

                //Face 5
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Five.BottomLeft,
                    Normal = normals[4]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, -1.0f),
                    TexCoord = tex.Five.TopLeft,
                    Normal = normals[4]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, -1.0f),
                    TexCoord = tex.Five.TopRight,
                    Normal = normals[4]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Five.BottomLeft,
                    Normal = normals[4]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, -1.0f),
                    TexCoord = tex.Five.TopRight,
                    Normal = normals[4]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, 1.0f, 1.0f),
                    TexCoord = tex.Five.BottomRight,
                    Normal = normals[4]
                },

                //Face 6
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, -1.0f),
                    TexCoord = tex.Six.BottomLeft,
                    Normal = normals[5]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Six.TopLeft,
                    Normal = normals[5]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Six.TopRight,
                    Normal = normals[5]
                },
                new Vertex3D
                {
                    Position = new Vector3(1.0f, -1.0f, -1.0f),
                    TexCoord = tex.Six.BottomLeft,
                    Normal = normals[5]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, 1.0f),
                    TexCoord = tex.Six.TopRight,
                    Normal = normals[5]
                },
                new Vertex3D
                {
                    Position = new Vector3(-1.0f, -1.0f, -1.0f),
                    TexCoord = tex.Six.BottomRight,
                    Normal = normals[5]
                },
            };

            var numVerts = mesh.Length;

            var hw = 0.5f * width;
            var hh = 0.5f * height;
            var hd = 0.5f * depth;

            for (var n = 0; n < numVerts; n++)
            {
                var pos = mesh[n].Position;
                mesh[n].Position = Vector3.Transform(new Vector3(hw * pos.X, hh * pos.Y, hd * pos.Z), rotationMatrix) + position;
            }

            return mesh;
        }
    }
}