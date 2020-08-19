using System;
using System.Numerics;
using Veldrid;

namespace Yak2D.Graphics
{
    public class SphericalMeshBuilder : ISphericalMeshBuilder
    {
        private IVertexLinearGridToTriangleListTool _vertexGridToTriangleListTool;

        public SphericalMeshBuilder(IVertexLinearGridToTriangleListTool vertexGridToTriangleListTool)
        {
            _vertexGridToTriangleListTool = vertexGridToTriangleListTool;
        }

        public Vertex3D[] Build(Vector3 position, float width, float height, float depth, float rotationClockwiseDegreesAroundPositiveY, uint numHorizontalDivisions, uint numVerticalDivisions)
        {
            //If perfect circle then width = height = depth = 2*radius

            if (width <= 0.0f)
            {
                width = 1.0f;
            }

            if (height <= 0.0f)
            {
                height = 1.0f;
            }

            var radians = rotationClockwiseDegreesAroundPositiveY * (float)Math.PI / 180.0f;

            if (numHorizontalDivisions < 4)
            {
                numHorizontalDivisions = 4;
            }

            if (numVerticalDivisions < 3)
            {
                numVerticalDivisions = 3;
            }

            return GenerateSphere(position, width, height, depth, radians, numHorizontalDivisions, numVerticalDivisions);
        }

        private Vertex3D[] GenerateSphere(Vector3 position, float width, float height, float depth, float radians, uint numHorizontalDivisions, uint numVerticalDivisions)
        {
            /*
		        Coordinate System Here is RHS, Y Vertical.
		        V = Vertical Angle [inclination] .. (around horizontal X axis, 0 is pointing straight up)
		        H = Horizontal Angle [azimuth] .. (around vertical Y axis, positive is clockwise from positive x direction towards positive z)
	
		        z = R.Sin(V).Sin(H)
		        x = R.Sin(V).Cos(H)
		        y = R.Cos(V)
            */

            var verts = new Vertex3D[numHorizontalDivisions * numVerticalDivisions];

            var divFracH = 1.0f / (1.0f * (numHorizontalDivisions - 1));
            var divFracV = 1.0f / (1.0f * (numVerticalDivisions - 1));

            var hw = 0.5f * width;
            var hh = 0.5f * height;
            var hd = 0.5f * depth;

            float fracX, fracY;

            for (var ny = 0; ny < numVerticalDivisions; ny++)
            {
                fracY = ny * divFracV;

                for (var nx = 0; nx < numHorizontalDivisions; nx++)
                {
                    fracX = nx * divFracH;

                    var horizontalAngle = (-0.5f + (-fracX * 2.0f)) * (float)Math.PI; //Rotate 90 degress CCW (negative direction) to pointing along negative z. then wrap around in CCW direction from there
                    var verticalAngle = fracY * (float)Math.PI;

                    var H = horizontalAngle;
                    var V = verticalAngle;

                    var sinH = (float)Math.Sin(H);
                    var cosH = (float)Math.Cos(H);

                    var sinV = (float)Math.Sin(V);
                    var cosV = (float)Math.Cos(V);

                    var x = sinV * cosH;
                    var y = cosV;
                    var z = sinV * sinH;

                    var index = (numHorizontalDivisions * ny) + nx;

                    var positionScaled = new Vector3(x * hw, y * hh, z * hd);
                    var positionRotated = Vector3.Transform(positionScaled, Matrix4x4.CreateRotationY(-radians));

                    verts[index] = new Vertex3D
                    {
                        Position = positionRotated + position,
                        TexCoord = new Vector2(fracX, fracY),
                    };
                }
            }

            /*Calculate Normals
             
                Given the user has the option to distort the sphere by arbitary width, height and depth
                The per vertex normals are not guaranteed to be just normalised vectors of it's position
                (as is the case for a uniform sphere of constant radius around the origin)
                We therefore need to employ the standard vertex normal calculation technique:

                Sum all Face Normals of the faces surrounding each vertex and normalise
                    
                Note: although in this mesh each face will be the same size, this technique does
                account for the different weighting that differently sized faces should have on a vertex
                normal. This is because a face normal is calculated using the cross product of two sides 
                of a triangle. The length of the resulting vector is proportional the area of the face
                and it's direction is naturally in the direction of the face normal

                Finally, care must be taken to ensure the face-out normals are calculated rather than face-in.

                There are a select number of normals we already know. namely that those in the top and bottom
                Vertical row are +/- UnitY
            */

            for (var ny = 0; ny < numVerticalDivisions; ny++)
            {
                for (var nx = 0; nx < numHorizontalDivisions; nx++)
                {
                    var index = (numHorizontalDivisions * ny) + nx;

                    if (ny == 0)
                    {
                        //Top row points straight up
                        verts[index].Normal = Vector3.UnitY;
                        continue;
                    }

                    if (ny == numVerticalDivisions - 1)
                    {
                        //Bottom row points straight down
                        verts[index].Normal = -Vector3.UnitY;
                        continue;
                    }

                    //Vector3.Cross -> Positive Unit X crossed with Positive Unit Y == Unit Positive Z
                    //To get an outward facing normal, I will use those vectors along azimuth/inclination 
                    //directions (psuedo vertical and horizontal lines in the reference frame of the vertex)
                    //and i will each one with the next in the counter clockwise direction if facing straight on

                    /* 
                        x and Letter = Vertex

                        x   x  /C\  x   x
                              / | \
                        x   D---A---B   x
                             \  | /
                        x   x  \E/  x   x


                        Vertex Normal of A == Normalise(ABxAC + ACxAD + ADxAE + AExAB)
                    */

                    var indexMinusOne = index - 1;
                    if (nx == 0)
                    {
                        indexMinusOne = (numHorizontalDivisions * (ny + 1)) - 2; //-2 not -1 NOTE
                    }

                    var indexPlusOne = index + 1;
                    if (nx == numHorizontalDivisions - 1)
                    {
                        indexPlusOne -= numHorizontalDivisions;
                        indexPlusOne++; //Adding an extra 1 NOTE
                    }

                    //Why NOTE.. because the edge azimuth / horizontal points actually overlap at 0 and max azimuth. wrap points are not shared
                    //There is a seem at the back with overlapping points

                    var A = verts[index].Position;
                    var B = verts[indexPlusOne].Position;
                    var C = verts[index - numHorizontalDivisions].Position;
                    var D = verts[indexMinusOne].Position;
                    var E = verts[index + numHorizontalDivisions].Position;

                    var sum = Vector3.Cross(B - A, C - A) + Vector3.Cross(C - A, D - A) + Vector3.Cross(D - A, E - A) + Vector3.Cross(E - A, B - A);

                    verts[index].Normal = Vector3.Normalize(sum);
                }
            }

            return _vertexGridToTriangleListTool.Convert(numHorizontalDivisions, numVerticalDivisions, verts);
        }
    }
}