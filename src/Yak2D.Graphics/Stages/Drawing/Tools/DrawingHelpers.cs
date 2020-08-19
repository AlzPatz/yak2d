using System;
using System.Linq;
using System.Numerics;

namespace Yak2D.Graphics
{
    public class DrawingHelpers : IDrawingHelpers
    {
        public ICommonOperations Common { get; private set; }
        private IDrawing _drawing;

        public DrawingHelpers(IDrawing drawing, ICommonOperations commonOperations)
        {
            Common = commonOperations;
            _drawing = drawing;
        }

        public IBaseDrawable Construct()
        {
            return new DrawBuilder(_drawing, Common);
        }

        public void DrawTexturedQuad(IDrawStage drawStage,
                            CoordinateSpace space,
                            ITexture texture,
                            Colour colour,
                            Vector2 position,
                            float width,
                            float height,
                            float depth,
                            int layer = 0,
                            float rotation_clockwise_radians = 0.0f,
                            float texcoord_min_x = 0.0f,
                            float texcoord_min_y = 0.0f,
                            float texcoord_max_x = 1.0f,
                            float texcoord_max_y = 1.0f,
                            TextureCoordinateMode textureMode = TextureCoordinateMode.Wrap)
        {
            var halfSize = 0.5f * new Vector2(width, height);

            _drawing.Draw(drawStage,
                            space,
                            FillType.Textured,
                            new Vertex2D[]
                            {
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, halfSize.Y), rotation_clockwise_radians)  + position, TexCoord0 = new Vector2(texcoord_min_x, texcoord_min_y), TexCoord1 = new Vector2(0.0f, 0.0f), TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, halfSize.Y) ,  rotation_clockwise_radians)+ position, TexCoord0 = new Vector2(texcoord_max_x, texcoord_min_y), TexCoord1 = new Vector2(1.0f, 0.0f), TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, -halfSize.Y), rotation_clockwise_radians)+ position, TexCoord0 = new Vector2(texcoord_min_x, texcoord_max_y), TexCoord1 = new Vector2(0.0f, 1.0f), TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, -halfSize.Y) , rotation_clockwise_radians)+ position, TexCoord0 = new Vector2(texcoord_max_x, texcoord_max_y), TexCoord1 = new Vector2(1.0f, 1.0f), TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },

                            },
                            new int[]
                            {
                                0, 1, 2, 2, 1, 3
                            },
                            colour,
                            texture,
                            null,
                            textureMode,
                            TextureCoordinateMode.None,
                            depth,
                            layer);
        }

         public void DrawTexturedQuad(ulong drawStage,
                            CoordinateSpace space,
                            ulong texture,
                            Colour colour,
                            Vector2 position,
                            float width,
                            float height,
                            float depth,
                            int layer = 0,
                            float rotation_clockwise_radians = 0.0f,
                            float texcoord_min_x = 0.0f,
                            float texcoord_min_y = 0.0f,
                            float texcoord_max_x = 1.0f,
                            float texcoord_max_y = 1.0f,
                            TextureCoordinateMode textureMode = TextureCoordinateMode.Wrap)
        {
            var halfSize = 0.5f * new Vector2(width, height);

            _drawing.Draw(drawStage,
                            space,
                            FillType.Textured,
                            new Vertex2D[]
                            {
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, halfSize.Y), rotation_clockwise_radians)  + position, TexCoord0 = new Vector2(texcoord_min_x, texcoord_min_y), TexCoord1 = new Vector2(0.0f, 0.0f), TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, halfSize.Y) ,  rotation_clockwise_radians)+ position, TexCoord0 = new Vector2(texcoord_max_x, texcoord_min_y), TexCoord1 = new Vector2(1.0f, 0.0f), TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, -halfSize.Y), rotation_clockwise_radians)+ position, TexCoord0 = new Vector2(texcoord_min_x, texcoord_max_y), TexCoord1 = new Vector2(0.0f, 1.0f), TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, -halfSize.Y) , rotation_clockwise_radians)+ position, TexCoord0 = new Vector2(texcoord_max_x, texcoord_max_y), TexCoord1 = new Vector2(1.0f, 1.0f), TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },

                            },
                            new int[]
                            {
                                0, 1, 2, 2, 1, 3
                            },
                            colour,
                            texture,
                            0UL,
                            textureMode,
                            TextureCoordinateMode.None,
                            depth,
                            layer);
        }

        public void DrawColouredQuad(IDrawStage drawStage,
                                CoordinateSpace space,
                                Colour colour,
                                Vector2 position,
                                float width,
                                float height,
                                float depth,
                                int layer = 0,
                                float rotation_clockwise_radians = 0.0f)
        {
            var halfSize = 0.5f * new Vector2(width, height);

            _drawing.Draw(drawStage,
                            space,
                            FillType.Coloured,
                            new Vertex2D[]
                            {
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, halfSize.Y), rotation_clockwise_radians)  + position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, halfSize.Y) ,  rotation_clockwise_radians)+ position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, -halfSize.Y), rotation_clockwise_radians)+ position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, -halfSize.Y) , rotation_clockwise_radians)+ position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },

                            },
                            new int[]
                            {
                                0, 1, 2, 2, 1, 3
                            },
                            colour,
                            null,
                            null,
                            TextureCoordinateMode.None,
                            TextureCoordinateMode.None,
                            depth,
                            layer);
        }

          public void DrawColouredQuad(ulong drawStage,
                                CoordinateSpace space,
                                Colour colour,
                                Vector2 position,
                                float width,
                                float height,
                                float depth,
                                int layer = 0,
                                float rotation_clockwise_radians = 0.0f)
        {
            var halfSize = 0.5f * new Vector2(width, height);

            _drawing.Draw(drawStage,
                            space,
                            FillType.Coloured,
                            new Vertex2D[]
                            {
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, halfSize.Y), rotation_clockwise_radians)  + position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, halfSize.Y) ,  rotation_clockwise_radians)+ position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(-halfSize.X, -halfSize.Y), rotation_clockwise_radians)+ position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                new Vertex2D { Position = Common.RotateVectorClockwise(new Vector2(halfSize.X, -halfSize.Y) , rotation_clockwise_radians)+ position, TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },

                            },
                            new int[]
                            {
                                0, 1, 2, 2, 1, 3
                            },
                            colour,
                            0UL,
                            0UL,
                            TextureCoordinateMode.None,
                            TextureCoordinateMode.None,
                            depth,
                            layer);
        }

        public void DrawColouredPoly(IDrawStage drawStage,
                                        CoordinateSpace space,
                                        Colour colour,
                                        Vector2 position,
                                        int numSides,
                                        float radius,
                                        float depth,
                                        int layer = 0,
                                        float xScaling = 1.0f,
                                        float yScaling = 1.0f,
                                        float rotation_clockwise_radians = 0.0f)
        {
            var verticesAndIndices = Common.RegularPolygonVertexAndIndicesGenerator(Vector2.Zero, numSides, radius);
            
            var numVertices = numSides + 1;

            var vertices = new Vertex2D[numVertices];

            for(var v = 0; v < numVertices; v++)
            {
                var pos = verticesAndIndices.Item1[v];

                pos.X *= xScaling;
                pos.Y *= yScaling;
                pos = Common.RotateVectorClockwise(ref pos, rotation_clockwise_radians);
                pos += position;

                vertices[v] = new Vertex2D
                {
                    Position = pos,
                    TexCoord0 = Vector2.Zero,
                    TexCoord1 = Vector2.Zero,
                    Colour = Colour.White,
                    TexWeighting = 1.0f
                };
            }

            _drawing.Draw(drawStage,
                            space,
                            FillType.Coloured,
                            vertices,
                            verticesAndIndices.Item2,
                            colour,
                            null,
                            null,
                            TextureCoordinateMode.None,
                            TextureCoordinateMode.None,
                            depth,
                            layer);
        }

        public void DrawColouredPoly(ulong drawStage,
                                        CoordinateSpace space,
                                        Colour colour,
                                        Vector2 position,
                                        int numSides,
                                        float radius,
                                        float depth,
                                        int layer = 0,
                                        float xScaling = 1.0f,
                                        float yScaling = 1.0f,
                                        float rotation_clockwise_radians = 0.0f)
        {
            var verticesAndIndices = Common.RegularPolygonVertexAndIndicesGenerator(Vector2.Zero, numSides, radius);
            
            var numVertices = numSides + 1;

            var vertices = new Vertex2D[numVertices];

            for(var v = 0; v < numVertices; v++)
            {
                var pos = verticesAndIndices.Item1[v];

                pos.X *= xScaling;
                pos.Y *= yScaling;
                pos = Common.RotateVectorClockwise(ref pos, rotation_clockwise_radians);
                pos += position;

                vertices[v] = new Vertex2D
                {
                    Position = pos,
                    TexCoord0 = Vector2.Zero,
                    TexCoord1 = Vector2.Zero,
                    Colour = Colour.White,
                    TexWeighting = 1.0f
                };
            }

            _drawing.Draw(drawStage,
                            space,
                            FillType.Coloured,
                            vertices,
                            verticesAndIndices.Item2,
                            colour,
                            0UL,
                            0UL,
                            TextureCoordinateMode.None,
                            TextureCoordinateMode.None,
                            depth,
                            layer);
        }

        public void DrawLine(IDrawStage drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false)
        {
            var vertsAndIndices = Common.LineAndArrowVertexAndIndicesGenerator(from, to, width, rounded, true);

        DrawColouredVertsAndIndices(drawStage, vertsAndIndices, space, colour, depth, layer);
        }

          public void DrawLine(ulong drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false)
        {
            var vertsAndIndices = Common.LineAndArrowVertexAndIndicesGenerator(from, to, width, rounded, true);

            DrawColouredVertsAndIndices(drawStage, vertsAndIndices, space, colour, depth, layer);
        }

        public void DrawArrow(IDrawStage drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        float headLength,
                        float headWidth,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false)
        {
            var vertsAndIndices = Common.LineAndArrowVertexAndIndicesGenerator(from, to, width, rounded, true, 32, true, headLength, headWidth);

            DrawColouredVertsAndIndices(drawStage, vertsAndIndices, space, colour, depth, layer);
        }

        public void DrawArrow(ulong drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        float headLength,
                        float headWidth,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false)
        {
            var vertsAndIndices = Common.LineAndArrowVertexAndIndicesGenerator(from, to, width, rounded, true, 32, true, headLength, headWidth);

            DrawColouredVertsAndIndices(drawStage, vertsAndIndices, space, colour, depth, layer);
        }    

        private void DrawColouredVertsAndIndices(IDrawStage drawStage, Tuple<Vector2[], int[]> vertsAndIndices, CoordinateSpace space, Colour colour, float depth, int layer)
        {
            _drawing.Draw(drawStage,
               space,
               FillType.Coloured,
               vertsAndIndices.Item1.Select(v => new Vertex2D
               {
                   Position = v,
                   Colour = Colour.White,
                   TexCoord0 = Vector2.Zero,
                   TexCoord1 = Vector2.Zero,
                   TexWeighting = 1.0f
               }).ToArray(),
               vertsAndIndices.Item2,
               colour,
               null,
               null,
               TextureCoordinateMode.None,
               TextureCoordinateMode.None,
               depth,
               layer);
        }

        private void DrawColouredVertsAndIndices(ulong drawStage, Tuple<Vector2[], int[]> vertsAndIndices, CoordinateSpace space, Colour colour, float depth, int layer)
        {
            _drawing.Draw(drawStage,
               space,
               FillType.Coloured,
               vertsAndIndices.Item1.Select(v => new Vertex2D
               {
                   Position = v,
                   Colour = Colour.White,
                   TexCoord0 = Vector2.Zero,
                   TexCoord1 = Vector2.Zero,
                   TexWeighting = 1.0f
               }).ToArray(),
               vertsAndIndices.Item2,
               colour,
               0UL,
               0UL,
               TextureCoordinateMode.None,
               TextureCoordinateMode.None,
               depth,
               layer);
        }
    }
}