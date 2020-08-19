using System;
using System.Linq;
using System.Numerics;

namespace Yak2D.Graphics
{
    public class DrawBuilder : IBaseDrawable, ITextured, IShape, ILine, ITransformable, IConstructedDrawable
    {
        private readonly IDrawing _drawing;
        private readonly ICommonOperations _common;

        private FillType _fill;
        private Colour _colour;

        private Vector2 _position;

        private Vector2[] _relativeVertexPositions;
        private int[] _indices;
        private int _numVertices;

        private TextureBrush _texture0;
        private TextureBrush _texture1;
        private TextureMixDirection _textureMix;
        private Vector2[] _texCoord0s;
        private Vector2[] _texCoord1s;
        private float[] _tex0Weightings;
        private Colour[] _vertColours;

        private DrawBuilderShape _shape;
        private float _quadWidth;
        private float _quadHeight;
        private float _arrowHeadWidth;
        private float _arrowHeadLength;
        private Vector2[] _polyVertsVertices;
        private int[] _polyVertsIndices;
        private Vector2[] _polyVertsTexCoords;
        private bool _polyVertsSuppliedTexCoords;
        private int _polyNumSides;
        private float _polyRadius;
        private Vector2 _from;
        private Vector2 _to;
        private float _lineWidth;
        private int _roundedLineNumDivisions;
        private bool _roundedLineEndsAreCentreOfRoundedPart;
        private float _outlineWidth;

        //An atempt at a fluent interface (with some ordering constraints..), with chained calls, 
        //but only creating a single object per chain
        //I am sure this is a naive reimplementation of an established pattern
        //but I feel the specialised constraints and use cases are well taken care of here
        //This is to replace a drawtools helper that just has a huge number of overloaded methods to 
        //simplfy drawing

        /*
            Notes
            - Arrow Outline approach takes a simple view on the arrow head. a custom implementation would look better
            - If the option is NOT selected to have the end of the line section of the rounded lines at the start and end point
            - at zero length lines the curves and arrows are drawn backwards, ie some artifcat for small lines within which the curves
            - do not fit
         */ 

        public DrawBuilder(IDrawing drawing, ICommonOperations commonOperations)
        {
            _drawing = drawing;
            _common = commonOperations;
        }

        //Texturing Types

        public ITextured Coloured(Colour colour)
        {
            _fill = FillType.Coloured;
            _colour = colour;
            _texture0 = new TextureBrush { Texture = null, TextureMode = TextureCoordinateMode.None, TextureScaling = TextureScaling.Stretch, TilingScale = Vector2.One };
            _texture1 = new TextureBrush { Texture = null, TextureMode = TextureCoordinateMode.None, TextureScaling = TextureScaling.Stretch, TilingScale = Vector2.One };

            return this;
        }

        public ITextured DualTextured(TextureBrush texture0, TextureBrush texture1, TextureMixDirection mix, Colour colour)
        {
            _fill = FillType.DualTextured;
            _texture0 = texture0;
            _texture1 = texture1;
            _textureMix = mix;
            _colour = colour;

            return this;
        }

        public ITextured Textured(TextureBrush texture, Colour colour)
        {
            _fill = FillType.Textured;
            _texture0 = texture;
            _texture1 = new TextureBrush { Texture = null, TextureMode = TextureCoordinateMode.None, TextureScaling = TextureScaling.Stretch, TilingScale = Vector2.One };
            _colour = colour;

            return this;
        }

        //Shapes

        public ILine Line(Vector2 from, Vector2 to, float width)
        {
            _shape = DrawBuilderShape.Line;
            _from = from;
            _to = to;
            _lineWidth = width;
            return this;
        }

        public ILine RoundedLine(Vector2 from, Vector2 to, float width, int roundedNumDivisions, bool endLineAtCentreRounded)
        {
            _shape = DrawBuilderShape.RoundedLine;
            _from = from;
            _to = to;
            _lineWidth = width;
            _roundedLineNumDivisions = roundedNumDivisions;
            _roundedLineEndsAreCentreOfRoundedPart = endLineAtCentreRounded;
            return this;
        }

        public IShape Arrow(float headWidth, float headLength)
        {
            _shape = _shape == DrawBuilderShape.RoundedLine ? DrawBuilderShape.RoundedArrow : DrawBuilderShape.Arrow;
            _arrowHeadWidth = headWidth;
            _arrowHeadLength = headLength;
            return this;
        }

        public IShape Poly(Vector2 position, int numSides, float radius)
        {
            _shape = DrawBuilderShape.PolyRegular;
            _position = position;
            _polyNumSides = numSides;
            _polyRadius = radius;
            return this;
        }

        public IShape Poly(Vector2[] vertices, int[] indices, Vector2[] texCoords)
        {
            _shape = DrawBuilderShape.PolyVerts;
            _polyVertsVertices = vertices;
            _polyVertsIndices = indices;
            _polyVertsTexCoords = texCoords;
            _polyVertsSuppliedTexCoords = true;
            return this;
        }

        public IShape Poly(Vector2[] vertices, int[] indices)
        {
            _shape = DrawBuilderShape.PolyVerts;
            _polyVertsVertices = vertices;
            _polyVertsIndices = indices;
            _polyVertsSuppliedTexCoords = false;
            return this;
        }

        public IShape Quad(Vector2 position, float width, float height)
        {
            _shape = DrawBuilderShape.Quad;
            _position = position;
            _quadWidth = width;
            _quadHeight = height;
            return this;
        }

        //Fill (we trigger vertex, index, texcoord and texweight creation at this stage)
        //Note all vertex positions must remain as relative too the shapes position
        //To allow later transform (scale / rotate) steps to work

        public ITransformable Outline(float lineWidth)
        {
            _outlineWidth = lineWidth;

            //Calculate Vertices, Indices, TexCoords and TexWeights for Filled Shapes
            //Check if the concept of outline even makes sense here

            switch (_shape)
            {
                case DrawBuilderShape.Quad:
                    CalculateQuadVerticesAndIndices_Outline();
                    break;
                case DrawBuilderShape.PolyRegular:
                    CalculatePolyRegularVerticesAndIndices_Outline();
                    break;
                case DrawBuilderShape.PolyVerts:
                    //Creating an outline is not valid from a custom colleciton of points
                    //A convex hull could be created from a point cloud and then reduced to make
                    //an outline. But simply if an outline of a custom shape is desired. the user 
                    //should create their own vertices
                    //To do: bring in frameworkMessenger to relay a message?
                    CalculatePolyVertsVerticesAndIndices_Filled();
                    break;
                case DrawBuilderShape.Line:
                    CalculateLineVerticesAndIndices_Outline(false, false);
                    break;
                case DrawBuilderShape.RoundedLine:
                    CalculateLineVerticesAndIndices_Outline(true, false);
                    break;
                case DrawBuilderShape.Arrow:
                    CalculateLineVerticesAndIndices_Outline(false, true);
                    break;
                case DrawBuilderShape.RoundedArrow:
                    CalculateLineVerticesAndIndices_Outline(true, true);
                    break;
            }

            if (_shape != DrawBuilderShape.PolyVerts || !_polyVertsSuppliedTexCoords)
            {
                CalculateRequiredTextureCoordinates();
            }
            else
            {
                _texCoord0s = _polyVertsTexCoords;
                _texCoord1s = _polyVertsTexCoords;
            }

            return this;
        }

        private void CalculateQuadVerticesAndIndices_Outline()
        {
            var halfWidth = 0.5f * _quadWidth;
            var halfHeight = 0.5f * _quadHeight;

            var minHalfDimension = halfWidth < halfHeight ? halfWidth : halfHeight;

            var oWidth = _outlineWidth < minHalfDimension ? _outlineWidth : minHalfDimension;

            var innerHalfWidth = halfWidth - oWidth;
            var innerHalfHeight = halfHeight - oWidth;

            SetVertArraySizes(8);

            _relativeVertexPositions[0] = new Vector2(-halfWidth, halfHeight);
            _relativeVertexPositions[1] = new Vector2(halfWidth, halfHeight);
            _relativeVertexPositions[2] = new Vector2(halfWidth, -halfHeight);
            _relativeVertexPositions[3] = new Vector2(-halfWidth, -halfHeight);

            _relativeVertexPositions[4] = new Vector2(-innerHalfWidth, innerHalfHeight);
            _relativeVertexPositions[5] = new Vector2(innerHalfWidth, innerHalfHeight);
            _relativeVertexPositions[6] = new Vector2(innerHalfWidth, -innerHalfHeight);
            _relativeVertexPositions[7] = new Vector2(-innerHalfWidth, -innerHalfHeight);

            _indices = new int[]
            {
                0, 1, 4,
                4, 1, 5,
                1, 2, 5,
                5, 2, 6,
                2, 3, 7,
                2, 7, 6,
                3, 0, 4,
                3, 4, 7
            };
        }

        private void CalculatePolyRegularVerticesAndIndices_Outline()
        {
            var verticesAndIndicesOuter = _common.RegularPolygonVertexAndIndicesGenerator(Vector2.Zero,
                                                                         _polyNumSides,
                                                                         _polyRadius);

            var oWidth = _outlineWidth < _polyRadius ? _outlineWidth : _polyRadius;
            var innerRadius = _polyRadius - oWidth;

            var verticesAndIndicesInner = _common.RegularPolygonVertexAndIndicesGenerator(Vector2.Zero,
                                                                         _polyNumSides,
                                                                         innerRadius);

            var numVerts = verticesAndIndicesOuter.Item1.Length;
            SetVertArraySizes(2 * numVerts);

            //We discard the indices provided by the other functions

            //We create a new index list referencing the two polgons. We skip the centre vertex #0 on each
            //But we do not remove it from the data. That's just a bit lazy but probably quicker code wise

            var numIndices = 6 * _polyNumSides;

            var indices = new int[numIndices];

            var deltaShift = numVerts;

            var index = 0;
            for (var s = 0; s < _polyNumSides; s++)
            {
                var n0 = s;
                var n1 = s + 1;

                if (n1 == _polyNumSides)
                {
                    n1 = 0;
                }

                n0++;
                n1++;

                var m0 = n0 + deltaShift;
                var m1 = n1 + deltaShift;

                indices[index + 0] = n0;
                indices[index + 1] = n1;
                indices[index + 2] = m1;
                indices[index + 3] = n0;
                indices[index + 4] = m1;
                indices[index + 5] = m0;

                index += 6;
            }

            _relativeVertexPositions = verticesAndIndicesOuter.Item1.Concat(verticesAndIndicesInner.Item1).ToArray();

            _indices = indices;
        }

        private void CalculateLineVerticesAndIndices_Outline(bool rounded, bool arrow)
        {
            //I'm afraid this will be writeonly code as I am going to untangle the assumed drawing order 
            //of filled lines and arrows, to then enable a outline to be created by drawing between
            //the vertices of two different sized lines and arrows. As this is helper code
            //and not core framework, I am relatively relaxed if it achieves the function, (for now)

            //Generate two sets of vertices.. the Original.. and then modify params:
            //reduce line thickness by 2*outline width
            //reduce length of line by 2*outline width
            //if arrow, reduce the head width by 2*outline width
            //reduce arrow head lenght by 2*outline width

            //Some guards against overly large outline thickness. Will not ensure perfect compatibility with 
            //some arrow sizes
            var halfLineWidth = 0.5f * _lineWidth;
            var oWidth = _outlineWidth > halfLineWidth ? halfLineWidth : _outlineWidth;
            var rawLineLength = (_to - _from).Length();

            var halfRawLineLength = 0.5f * rawLineLength;
            if (rounded && _roundedLineEndsAreCentreOfRoundedPart)
            {
                halfRawLineLength += 0.25f * _lineWidth;
                if (!arrow)
                {
                    halfRawLineLength += 0.25f * _lineWidth;
                }
            }
            oWidth = oWidth > halfRawLineLength ? halfRawLineLength : oWidth;

            var vertsAndInd_Outer = _common.LineAndArrowVertexAndIndicesGenerator(_from,
                                                               _to,
                                                               _lineWidth,
                                                               rounded,
                                                               _roundedLineEndsAreCentreOfRoundedPart,
                                                               _roundedLineNumDivisions,
                                                               arrow,
                                                               _arrowHeadLength,
                                                               _arrowHeadWidth
                                                               );

            var forward = (_to - _from);
            if (forward == Vector2.Zero)
            {
                forward = Vector2.UnitX;
            }
            else
            {
                forward /= rawLineLength;
            }
            var newFrom = _from + (forward * oWidth);
            var newTo = _to - (forward * oWidth);

            var newLineWidth = _lineWidth - (2.0f * oWidth);
            var correctionShift = (0.5f * (_lineWidth - newLineWidth));
            if (rounded && _roundedLineEndsAreCentreOfRoundedPart)
            {
                newFrom -= forward * correctionShift;
                if (!arrow)
                {
                    newTo += forward * correctionShift;
                }
            }

            var newArrowHeadLength = _arrowHeadLength;
            var newArrowHeadWidth = _arrowHeadWidth;

            if (_shape == DrawBuilderShape.Arrow || _shape == DrawBuilderShape.RoundedArrow)
            {
                newArrowHeadLength -= 1.0f * oWidth;
                newArrowHeadWidth -= 2.0f * oWidth;
            }

            var vertsAndInd_Inner = _common.LineAndArrowVertexAndIndicesGenerator(newFrom,
                                                   newTo,
                                                   newLineWidth,
                                                   rounded,
                                                   _roundedLineEndsAreCentreOfRoundedPart,
                                                   _roundedLineNumDivisions,
                                                   arrow,
                                                   newArrowHeadLength,
                                                   newArrowHeadWidth
                                                   );

            //Now we combine the two vertex sets and generate indices

            var setSize = vertsAndInd_Outer.Item1.Length;
            var roundedDivs = _roundedLineNumDivisions < 2 ? 2 : _roundedLineNumDivisions;

            //Hitting it manually per type
            switch (_shape)
            {
                case DrawBuilderShape.Line:
                    SetVertArraySizes(8);
                    _indices = new int[]
                    {
                        0,1,4,4,1,5,1,2,5,5,2,6,2,3,7,2,7,6,3,0,4,3,4,7
                    };
                    break;
                case DrawBuilderShape.Arrow:
                    SetVertArraySizes(14);
                    _indices = new int[]
                    {
                        1,2,9,
                        1,9,8,
                        9,2,4,
                        9,4,11,
                        4,5,11,
                        11,5,12,
                        5,6,13,
                        5,13,12,
                        6,13,10,
                        6,10,3,
                        3,0,7,
                        3,7,10,
                        0,1,8,
                        0,8,7
                    };
                    break;

                case DrawBuilderShape.RoundedLine:
                case DrawBuilderShape.RoundedArrow:

                    SetVertArraySizes(2 * setSize);

                    var numberOfIndices = (6 * roundedDivs) + 12;

                    if (_shape == DrawBuilderShape.RoundedLine)
                    {
                        numberOfIndices += 6 * roundedDivs;
                    }
                    else
                    {
                        numberOfIndices += 24;
                    }

                    var indices = new int[numberOfIndices];

                    //Line
                    var n0 = 0;
                    var m0 = setSize;
                    var n1 = 1;
                    var m1 = setSize + 1;
                    var n2 = 2;
                    var m2 = setSize + 2;
                    var n3 = 3;
                    var m3 = setSize + 3;

                    indices[0] = m1;
                    indices[1] = n1;
                    indices[2] = n2;
                    indices[3] = m1;
                    indices[4] = n2;
                    indices[5] = m2;
                    indices[6] = m3;
                    indices[7] = n3;
                    indices[8] = n0;
                    indices[9] = m3;
                    indices[10] = n0;
                    indices[11] = m0;

                    var index = 12;

                    //Rounded Start

                    for (var v = 0; v < roundedDivs; v++)
                    {
                        var curr = v == 0 ? 0 : 4 + v - 1;
                        var next = v == roundedDivs - 1 ? 1 : 4 + v - 0;

                        var nc = curr;
                        var nn = next;
                        var mc = curr + setSize;
                        var mn = next + setSize;

                        indices[index] = mc;
                        index++;
                        indices[index] = nc;
                        index++;
                        indices[index] = nn;
                        index++;

                        indices[index] = mc;
                        index++;
                        indices[index] = nn;
                        index++;
                        indices[index] = mn;
                        index++;
                    }

                    var firstVertexOfEndVerts = 4 + roundedDivs - 1;

                    //If we set divs manually to 3 on the arrow, we can use the same code
                    var divs = roundedDivs;
                    if (_shape == DrawBuilderShape.RoundedArrow)
                    {
                        divs = 4;
                    }

                    for (var v = 0; v < divs; v++)
                    {
                        var curr = v == 0 ? 2 : firstVertexOfEndVerts + v - 1;
                        var next = v == divs - 1 ? 3 : firstVertexOfEndVerts + v + 0;

                        var nc = curr;
                        var nn = next;
                        var mc = curr + setSize;
                        var mn = next + setSize;

                        indices[index] = mc;
                        index++;
                        indices[index] = nc;
                        index++;
                        indices[index] = nn;
                        index++;

                        indices[index] = mc;
                        index++;
                        indices[index] = nn;
                        index++;
                        indices[index] = mn;
                        index++;
                    }

                    _indices = indices;
                    break;
            }

            _relativeVertexPositions = vertsAndInd_Outer.Item1.Concat(vertsAndInd_Inner.Item1).ToArray();

            _position = FindCentreOfPointCloud(_relativeVertexPositions);
            CalculateRelativePositions(_relativeVertexPositions, _position);
        }

        private void CalculateRelativePositions(Vector2[] vertices, Vector2 position)
        {
            for (var v = 0; v < vertices.Length; v++)
            {
                vertices[v] = vertices[v] - position;
            }
        }

        private Vector2 FindCentreOfPointCloud(Vector2[] vertices)
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);

            for (var v = 0; v < vertices.Length; v++)
            {
                if (vertices[v].X < min.X)
                {
                    min.X = vertices[v].X;
                }
                if (vertices[v].Y < min.Y)
                {
                    min.Y = vertices[v].Y;
                }
                if (vertices[v].X > max.X)
                {
                    max.X = vertices[v].X;
                }
                if (vertices[v].Y > max.Y)
                {
                    max.Y = vertices[v].Y;
                }
            }

            return 0.5f * (min + max);
        }

        public ITransformable Filled()
        {
            //Calculate Vertices, Indices, TexCoords and TexWeights for Filled Shapes

            switch (_shape)
            {
                case DrawBuilderShape.Quad:
                    CalculateQuadVerticesAndIndices_Filled();
                    break;
                case DrawBuilderShape.PolyRegular:
                    CalculatePolyRegularVerticesAndIndices_Filled();
                    break;
                case DrawBuilderShape.PolyVerts:
                    CalculatePolyVertsVerticesAndIndices_Filled();
                    break;
                case DrawBuilderShape.Line:
                    CalculateLineVerticesAndIndices_Filled(false, false);
                    break;
                case DrawBuilderShape.RoundedLine:
                    CalculateLineVerticesAndIndices_Filled(true, false);
                    break;
                case DrawBuilderShape.Arrow:
                    CalculateLineVerticesAndIndices_Filled(false, true);
                    break;
                case DrawBuilderShape.RoundedArrow:
                    CalculateLineVerticesAndIndices_Filled(true, true);
                    break;
            }

            if (_shape != DrawBuilderShape.PolyVerts || !_polyVertsSuppliedTexCoords)
            {
                CalculateRequiredTextureCoordinates();
            }
            else
            {
                _texCoord0s = _polyVertsTexCoords;
                _texCoord1s = _polyVertsTexCoords;
            }

            return this;
        }

        private void SetVertArraySizes(int numVertices)
        {
            _numVertices = numVertices;

            _relativeVertexPositions = new Vector2[_numVertices];
            _vertColours = new Colour[_numVertices];
            _texCoord0s = new Vector2[_numVertices];
            _texCoord1s = new Vector2[_numVertices];
            _tex0Weightings = new float[_numVertices];

            for (var n = 0; n < _numVertices; n++)
            {
                _vertColours[n] = Colour.White; //Need default to be visible  /white..
                //_texCoord0s[n] = Vector2.Zero; //Don't need to init these structs as can't be null
                //_texCoord1s[n] = Vector2.Zero;
            }
        }

        private void CalculateQuadVerticesAndIndices_Filled()
        {
            SetVertArraySizes(4);

            var halfWidth = 0.5f * _quadWidth;
            var halfHeight = 0.5f * _quadHeight;

            _relativeVertexPositions[0] = new Vector2(-halfWidth, halfHeight);
            _relativeVertexPositions[1] = new Vector2(halfWidth, halfHeight);
            _relativeVertexPositions[2] = new Vector2(halfWidth, -halfHeight);
            _relativeVertexPositions[3] = new Vector2(-halfWidth, -halfHeight);

            _indices = new int[]
            {
                0, 1, 2, 0, 2, 3
            };

            //Add version which accounts for radial due texture that requires a middle vertex to display properly
        }

        private void CalculatePolyRegularVerticesAndIndices_Filled()
        {
            var verticesAndIndices = _common.RegularPolygonVertexAndIndicesGenerator(Vector2.Zero,
                                                                                     _polyNumSides,
                                                                                     _polyRadius);

            var numVerts = verticesAndIndices.Item1.Length;

            SetVertArraySizes(numVerts);

            _relativeVertexPositions = verticesAndIndices.Item1;
            _indices = verticesAndIndices.Item2;
        }

        private void CalculatePolyVertsVerticesAndIndices_Filled()
        {
            var numVerts = _polyVertsVertices.Length;

            SetVertArraySizes(numVerts);

            _position = FindCentreOfPointCloud(_polyVertsVertices);
         
            for (var v = 0; v < numVerts; v++)
            {
                _relativeVertexPositions[v] = _polyVertsVertices[v] - _position;
            }

            _indices = _polyVertsIndices;
        }

        private void CalculateLineVerticesAndIndices_Filled(bool rounded, bool arrow)
        {
            var vertsAndInd = _common.LineAndArrowVertexAndIndicesGenerator(_from,
                                                                            _to,
                                                                            _lineWidth,
                                                                            rounded,
                                                                            _roundedLineEndsAreCentreOfRoundedPart,
                                                                            _roundedLineNumDivisions,
                                                                            arrow,
                                                                            _arrowHeadLength,
                                                                            _arrowHeadWidth
                                                                            );

            var numVerts = vertsAndInd.Item1.Length;

            SetVertArraySizes(numVerts);

            _position = FindCentreOfPointCloud(vertsAndInd.Item1);
      
            for (var v = 0; v < numVerts; v++)
            {
                _relativeVertexPositions[v] = vertsAndInd.Item1[v] - _position;
            }

            _indices = vertsAndInd.Item2;
        }

        private void CalculateRequiredTextureCoordinates()
        {
            switch (_fill)
            {
                case FillType.Textured:
                    CalculateTextureTexCoords(0);
                    break;
                case FillType.DualTextured:
                    CalculateTextureTexCoords(1);
                    CalculateTextureTexCoords(0);
                    break;
            }
        }

        private void CalculateTextureTexCoords(int textureIndex)
        {
            var brush = textureIndex == 0 ? _texture0 : _texture1;
            var texCoords = textureIndex == 0 ? _texCoord0s : _texCoord1s;

            switch (_shape)
            {
                case DrawBuilderShape.Quad:
                case DrawBuilderShape.PolyRegular:
                case DrawBuilderShape.PolyVerts:
                    CalculateTexCoordsAndWeightings(brush, texCoords, _relativeVertexPositions);
                    break;
                case DrawBuilderShape.Line:
                case DrawBuilderShape.RoundedLine:
                case DrawBuilderShape.Arrow:
                case DrawBuilderShape.RoundedArrow:
                    CalculateTexCoordsAndWeightings(brush, texCoords, TransformVertexPositionsIntoLineReferenceFrame(_relativeVertexPositions));
                    break;
            }
        }

        private Vector2[] TransformVertexPositionsIntoLineReferenceFrame(Vector2[] vertices)
        {
            var forward = _from - _to;
            var len = forward.Length();
            forward = len == 0.0f ? Vector2.Zero : forward / len;
            var right = new Vector2(forward.Y, -forward.X);

            return vertices.Select(v => new Vector2(Vector2.Dot(v, forward), Vector2.Dot(v, right))).ToArray();
        }

        private void CalculateTexCoordsAndWeightings(TextureBrush brush, Vector2[] texCoords, Vector2[] vertexPositions)
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);

            for (var v = 0; v < _numVertices; v++)
            {
                var vx = vertexPositions[v];

                if (vx.X < min.X)
                {
                    min.X = vx.X;
                }

                if (vx.Y < min.Y)
                {
                    min.Y = vx.Y;
                }

                if (vx.X > max.X)
                {
                    max.X = vx.X;
                }

                if (vx.Y > max.Y)
                {
                    max.Y = vx.Y;
                }
            }

            var areaSize = max - min;

            var textureSize = Vector2.Zero;
            if (brush.TextureScaling == TextureScaling.Tiled)
            {
                var size = _drawing.GetSurfaceDimensions(brush.Texture);
                textureSize = new Vector2(size.Width, size.Height);
                textureSize *= brush.TilingScale;
            }

            for (var v = 0; v < _numVertices; v++)
            {
                var vx = vertexPositions[v];
                var position_fraction = (vx - min) / areaSize;

                if (_fill == FillType.DualTextured)
                {
                    _tex0Weightings[v] = CalculateTexture0WeightFromAreaPositionFraction(position_fraction);
                }

                switch (brush.TextureScaling)
                {
                    case TextureScaling.Stretch:
                        position_fraction.Y = 1.0f - position_fraction.Y; //Flip Y to match origin for texcoords at topleft not bottomleft
                        texCoords[v] = position_fraction;
                        break;
                    case TextureScaling.Tiled:
                        var pixelDistanceFromCentre = position_fraction - new Vector2(0.5f, 0.5f);
                        pixelDistanceFromCentre *= areaSize;
                        var texSizeMultiplesFromCentre = pixelDistanceFromCentre / textureSize;
                        texSizeMultiplesFromCentre.Y = -texSizeMultiplesFromCentre.Y;  //Switch from calcuation origin bottom left to top left
                        var texSizeMultiplesFromTopLeft = texSizeMultiplesFromCentre + new Vector2(0.5f, 0.5f);
                        texCoords[v] = texSizeMultiplesFromTopLeft;
                        break;
                }
            }
        }

        private float CalculateTexture0WeightFromAreaPositionFraction(Vector2 pos)
        {
            switch (_textureMix)
            {
                case TextureMixDirection.Horizontal:
                    return 1.0f - pos.X;
                case TextureMixDirection.Vertical:
                    return 1.0f - pos.Y;
                case TextureMixDirection.Radial:
                    var deltaFromCentre = pos - new Vector2(0.5f, 0.5f);
                    var distance = deltaFromCentre.Length();
                    var frac = distance / (0.5f * (float)Math.Sqrt(2.0f)); //Pull out const
                    return frac;
            }

            return 0.0f;
        }

        //Transforms

        public ITransformable Scale(float xScaling, float yScaling)
        {
            if(xScaling < 0.0f)
            {
                xScaling *= -1.0f;
            }

            if (yScaling < 0.0f)
            {
                yScaling *= -1.0f;
            }

            for (var n = 0; n < _numVertices; n++)
            {
                _relativeVertexPositions[n].X *= xScaling;
                _relativeVertexPositions[n].Y *= yScaling;
            }
            return this;
        }

        public ITransformable Rotate(float angle_clockwise_radians)
        {
            for (var n = 0; n < _numVertices; n++)
            {
                _relativeVertexPositions[n] = _common.RotateVectorClockwise(_relativeVertexPositions[n], angle_clockwise_radians);
            }
            return this;
        }

        //Usage (Vertex2Ds are created and vertices are finally positioned. Previously all relative to single position point to enable transforms)

        public void SubmitDraw(IDrawStage drawStage, CoordinateSpace space, float depth, int layer)
        {
            _drawing.Draw(drawStage,
                            space,
                            _fill,
                             CreateVerticesInFinalPosition(),
                            _indices,
                            _colour,
                            _texture0.Texture,
                            _texture1.Texture,
                            _texture0.TextureMode,
                            _texture1.TextureMode,
                            depth,
                            layer);
        }

        public DrawRequest GenerateDrawRequest(CoordinateSpace space, float depth, int layer)
        {
            return new DrawRequest
            {
                CoordinateSpace = space,
                FillType = _fill,
                Vertices = CreateVerticesInFinalPosition(),
                Indices = _indices,
                Colour = _colour,
                Texture0 = _texture0.Texture,
                Texture1 = _texture1.Texture,
                TextureWrap0 = _texture0.TextureMode,
                TextureWrap1 = _texture1.TextureMode,
                Depth = depth,
                Layer = layer
            };
        }

        private Vertex2D[] CreateVerticesInFinalPosition()
        {
            var verts = new Vertex2D[_numVertices];
            for (var v = 0; v < _numVertices; v++)
            {
                verts[v] = new Vertex2D
                {
                    Position = _relativeVertexPositions[v] + _position,
                    Colour = _vertColours[v],
                    TexCoord0 = _texCoord0s[v],
                    TexCoord1 = _texCoord1s[v],
                    TexWeighting = _tex0Weightings[v]
                };
            }
            return verts;
        }

        //Change Modifiers
        public IConstructedDrawable ChangePosition(Vector2 position)
        {
            _position = position;
            return this;
        }

        public IConstructedDrawable ShiftPosition(Vector2 delta)
        {
            _position += delta;
            return this;
        }

        public IConstructedDrawable ChangeColour(Colour colour)
        {
            _colour = colour;
            return this;
        }

        public IConstructedDrawable ChangeTexture0(ITexture texture)
        {
            _texture0.Texture = texture;
            return this;
        }

        public IConstructedDrawable ChangeTexture1(ITexture texture)
        {
            _texture1.Texture = texture;
            return this;
        }
    }
}