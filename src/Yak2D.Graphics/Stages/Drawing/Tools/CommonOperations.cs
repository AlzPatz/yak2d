using System;
using System.Numerics;

namespace Yak2D.Graphics
{
    public class CommonOperations : ICommonOperations
    {
        private float _degToRad;
        private float _radToDeg;

        public CommonOperations()
        {
            _degToRad = (float)Math.PI / 180.0f;
            _radToDeg = 180.0f / (float)Math.PI;
        }

        public float RadiansToDegrees(float degrees)
        {
            return _radToDeg * degrees;
        }

        public float DegressToRadians(float radians)
        {
            return _degToRad * radians;
        }

        public Vector2 RotateVectorClockwise(Vector2 v, float radians)
        {
            var ca = (float)Math.Cos(-radians);
            var sa = (float)Math.Sin(-radians);
            return new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        public Vector2 RotateVectorClockwise(ref Vector2 v, float radians)
        {
            var ca = (float)Math.Cos(-radians);
            var sa = (float)Math.Sin(-radians);
            return new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        public Tuple<Vector2[], int[]> LineAndArrowVertexAndIndicesGenerator(Vector2 start,
                                                                                Vector2 end,
                                                                                float width,
                                                                                bool rounded = false,
                                                                                bool centreOfCurveRadiusAtLineEndPoints = true,
                                                                                int numberOfCurveSegments = 32,
                                                                                bool isArrow = false,
                                                                                float headLength = 10.0f,
                                                                                float headWidth = 10.0f)

        {
            if (numberOfCurveSegments < 2)
            {
                numberOfCurveSegments = 2;
            }

            //Line maybe simple 4 vert rectangle, may have rounded ends, may have an arrow head at one end

            //Size arrays accordingly

            var numberOfRequiredVertices = 4;
            var numberOfRequiredIndices = 6;

            if (isArrow)
            {
                numberOfRequiredVertices += 3;
                numberOfRequiredIndices += 3;
            }

            if (rounded)
            {
                var additionalVerticesForAnEnd = numberOfCurveSegments - 1;
                var additionalIndicesForAnEnd = 3 * (numberOfCurveSegments - 1);

                numberOfRequiredVertices += isArrow ? additionalVerticesForAnEnd : 2 * additionalVerticesForAnEnd;
                numberOfRequiredIndices += isArrow ? additionalIndicesForAnEnd : 2 * additionalIndicesForAnEnd;
            }

            var verts = new Vector2[numberOfRequiredVertices];
            var indices = new int[numberOfRequiredIndices];

            //Calculate some normals to help move around

            var delta = end - start;
            var length = delta.Length();
            var normal = length == 0.0f ? Vector2.UnitX : delta / length;

            var right = new Vector2(normal.Y, -normal.X);

            //Adjust start / finish points of rectangular body of line

            var halfWidth = 0.5f * width;

            if (rounded && !centreOfCurveRadiusAtLineEndPoints)
            {
                start += normal * halfWidth;

                if (!isArrow)
                {
                    end -= normal * halfWidth;
                }
                else
                {
                    //If arrow, and some adjustment to start point for 
                    //line body, recalc length so accurate in arrow check
                    //of oversized head length in the below
                    delta = end - start;
                    length = delta.Length();
                }
            }

            if (isArrow)
            {
                if (headLength > length)
                {
                    headLength = length;
                }

                end -= normal * headLength;
            }

            //Fill 

            //Body

            verts[0] = start + (right * halfWidth);
            verts[1] = start - (right * halfWidth);
            verts[2] = end - (right * halfWidth);
            verts[3] = end + (right * halfWidth);

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;

            var v = 4;
            var i = 6;

            //Rounded Ends 

            if (rounded)
            {
                var segAngleDelta = (float)Math.PI / (1.0f * numberOfCurveSegments);

                for (var n = 1; n < numberOfCurveSegments; n++)
                {
                    var angle = n * segAngleDelta;

                    var fromStart = RotateVectorClockwise(right, angle);

                    verts[v] = start + (fromStart * halfWidth);
                    v++;
                }

                for (var n = 1; n < numberOfCurveSegments; n++)
                {
                    var m = n - 1;
                    var first = 1;
                    var second = m == 0 ? 0 : 3 + m;
                    var third = 4 + m;

                    indices[i] = first;
                    indices[i + 1] = second;
                    indices[i + 2] = third;

                    i += 3;
                }

                if (!isArrow)
                {
                    var baseIndex = v - 1;

                    for (var n = 1; n < numberOfCurveSegments; n++)
                    {
                        var angle = n * segAngleDelta;

                        var fromEnd = RotateVectorClockwise(-right, angle);

                        verts[v] = end + (fromEnd * halfWidth);
                        v++;
                    }

                    for (var n = 1; n < numberOfCurveSegments; n++)
                    {
                        var m = n - 1;
                        var first = 3;
                        var second = m == 0 ? 2 : baseIndex + m;
                        var third = baseIndex + 1 + m;

                        indices[i] = first;
                        indices[i + 1] = second;
                        indices[i + 2] = third;

                        i += 3;
                    }
                }
            }

            //Arrow

            if (isArrow)
            {
                var baseIndexArrow = v;
                var halfHeadWidth = 0.5f * headWidth;
                verts[v] = end - (right * halfHeadWidth);
                verts[v + 1] = end + (normal * headLength);
                verts[v + 2] = end + (right * halfHeadWidth);

                //v += 3; //No need

                indices[i] = baseIndexArrow;
                indices[i + 1] = baseIndexArrow + 1;
                indices[i + 2] = baseIndexArrow + 2;

                //i += 3; //No need
            }

            return new Tuple<Vector2[], int[]>(verts, indices);
        }

        public Tuple<Vector2[], int[]> RegularPolygonVertexAndIndicesGenerator(Vector2 position, int numSides, float radius)
        {
            var vertices = new Vector2[numSides + 1];

            vertices[0] = position;

            var indices = new int[3 * numSides];

            var deltaAngle = ((float)Math.PI * 2.0f) / (1.0f * numSides);

            var iCount = 0;
            for (var n = 0; n < numSides; n++)
            {
                var vCurrent = n + 1;
                var vNext = vCurrent + 1;
                if (vNext == numSides + 1)
                {
                    vNext = 1;
                }

                var iBase = 3 * iCount;

                indices[iBase] = 0;
                indices[iBase + 1] = vCurrent;
                indices[iBase + 2] = vNext;

                var angle = n * deltaAngle;

                var pos = Vector2.UnitY;
                pos = RotateVectorClockwise(ref pos, angle);

                pos *= radius;

                pos += position;

                vertices[vCurrent] = pos;

                iCount++;
            }

            return new Tuple<Vector2[], int[]>(vertices, indices);
        }
    }
}