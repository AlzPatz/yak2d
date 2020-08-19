using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Drawing Helpers for Common Shapes
    /// Quad, Regular Polygon, Line and Arrow drawing functions
    /// Includes Fluent Interface for building and modifying shape drawing in steps
    /// </summary>
    public interface IDrawingHelpers
    {
        /// <summary>
        /// Operations to help generate vertex lists for regular polygons, lines and arrows
        /// Includes some helper functions for rotating 2d vectors and converting degress to radians
        /// </summary>
        ICommonOperations Common { get; }

        /// <summary>
        /// First step for generating shape drawing objects via a fluent interface
        /// </summary>
        IBaseDrawable Construct();

        /// <summary>
        /// Draw Textured Quad (rectangle) made from two triangles
        /// </summary>
        /// <param name="drawStage">DrawStage reference</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="texture">Primary Texture Reference (the only texture used for single texturing)</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="position">Centre position of the quad</param>
        /// <param name="width">Width of the quad</param>
        /// <param name="height">Height of the quad</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="float rotation_clockwise_radians">Clockwise rotation of quad around centre point in radians</param>
        /// <param name="texcoord_min_x">Texture coordinate of left edge of quad</param>
        /// <param name="texcoord_min_y">Texture coordinate of top edge of quad</param>
        /// <param name="texcoord_max_x">Texture coordinate of right edge of quad</param>
        /// <param name="texcoord_max_y">TExture coordinate of bottom edge of quad</param>
        void DrawTexturedQuad(IDrawStage drawStage,
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
                                TextureCoordinateMode textureMode = TextureCoordinateMode.Wrap);
        
        /// <summary>
        /// Draw Textured Quad (rectangle) made from two triangles
        /// </summary>
        /// <param name="drawStage">DrawStage id</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="texture">Primary Texture id (the only texture used for single texturing)</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="position">Centre position of the quad</param>
        /// <param name="width">Width of the quad</param>
        /// <param name="height">Height of the quad</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="float rotation_clockwise_radians">Clockwise rotation of quad around centre point in radians</param>
        /// <param name="texcoord_min_x">Texture coordinate of left edge of quad</param>
        /// <param name="texcoord_min_y">Texture coordinate of top edge of quad</param>
        /// <param name="texcoord_max_x">Texture coordinate of right edge of quad</param>
        /// <param name="texcoord_max_y">TExture coordinate of bottom edge of quad</param>
        void DrawTexturedQuad(ulong drawStage,
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
                                TextureCoordinateMode textureMode = TextureCoordinateMode.Wrap);

        /// <summary>
        /// Draw Coloured Quad (rectangle) made from two triangles
        /// </summary>
        /// <param name="drawStage">DrawStage reference</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="position">Centre position of the quad</param>
        /// <param name="width">Width of the quad</param>
        /// <param name="height">Height of the quad</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="float rotation_clockwise_radians">Clockwise rotation of quad around centre point in radians</param>
        void DrawColouredQuad(IDrawStage drawStage,
                                CoordinateSpace space,
                                Colour colour,
                                Vector2 position,
                                float width,
                                float height,
                                float depth,
                                int layer = 0,
                                float rotation_clockwise_radians = 0.0f);
        
        /// <summary>
        /// Draw Coloured Quad (rectangle) made from two triangles
        /// </summary>
        /// <param name="drawStage">DrawStage id</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="position">Centre position of the quad</param>
        /// <param name="width">Width of the quad</param>
        /// <param name="height">Height of the quad</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="float rotation_clockwise_radians">Clockwise rotation of quad around centre point in radians</param>
        void DrawColouredQuad(ulong drawStage,
                                CoordinateSpace space,
                                Colour colour,
                                Vector2 position,
                                float width,
                                float height,
                                float depth,
                                int layer = 0,
                                float rotation_clockwise_radians = 0.0f);
        
        /// <summary>
        /// Draw Coloured Regular Polygon (a regular polygon has equal length sides.
        ///  In the case where xScaling == yScaling, as the number of sides increases, the closer an approximation of a circle is made)
        /// </summary>
        /// <param name="drawStage">DrawStage reference</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="position">Centre position of the polygon</param>
        /// <param name="numSides">The number of sides that polygon has (minimum 3 == triangle)</param>
        /// <param name="radius">Distance of each outer edge vertex point from centre before scaling</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="xScaling">Scalar of polygon's x-dimensions (before rotation)</param>
        /// <param name="yScaling">Scalar of polygon's y-dimensions (before rotation)</param>
        /// <param name="float rotation_clockwise_radians">Clockwise rotation of final scaled polygon around centre point in radians</param>
        void DrawColouredPoly(IDrawStage drawStage,
                                CoordinateSpace space,
                                Colour colour,
                                Vector2 position,
                                int numSides,
                                float radius,
                                float depth,
                                int layer = 0,
                                float xScaling = 1.0f,
                                float yScaling = 1.0f,
                                float rotation_clockwise_radians = 0.0f);
        
        /// <summary>
        /// Draw Coloured Regular Polygon (a regular polygon has equal length sides.
        ///  In the case where xScaling == yScaling, as the number of sides increases, the closer an approximation of a circle is made)
        /// </summary>
        /// <param name="drawStage">DrawStage id</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="position">Centre position of the polygon</param>
        /// <param name="numSides">The number of sides that polygon has (minimum 3 == triangle)</param>
        /// <param name="radius">Distance of each outer edge vertex point from centre before scaling</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="xScaling">Scalar of polygon's x-dimensions (before rotation)</param>
        /// <param name="yScaling">Scalar of polygon's y-dimensions (before rotation)</param>
        /// <param name="float rotation_clockwise_radians">Clockwise rotation of final scaled polygon around centre point in radians</param>
        void DrawColouredPoly(ulong drawStage,
                                CoordinateSpace space,
                                Colour colour,
                                Vector2 position,
                                int numSides,
                                float radius,
                                float depth,
                                int layer = 0,
                                float xScaling = 1.0f,
                                float yScaling = 1.0f,
                                float rotation_clockwise_radians = 0.0f);

        /// <summary>
        /// Draw Coloured Line
        /// </summary>
        /// <param name="drawStage">DrawStage reference</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="from">Location of the start of the line (middle of the line's thickness)</param>
        /// <param name="to">Location of the end of the line (middle of the line's thickness)</param>
        /// <param name="width">Width of the line (thickness)</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="rounded">Set whether the ends of the lines are rounded</param>
        void DrawLine(IDrawStage drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false);
        
        /// <summary>
        /// Draw Coloured Line
        /// </summary>
        /// <param name="drawStage">DrawStage id</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="from">Location of the start of the line (middle of the line's thickness)</param>
        /// <param name="to">Location of the end of the line (middle of the line's thickness)</param>
        /// <param name="width">Width of the line (thickness)</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="rounded">Set whether the ends of the lines are rounded</param>
        void DrawLine(ulong drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false);


        /// <summary>
        /// Draw Coloured Arrow
        /// </summary>
        /// <param name="drawStage">DrawStage reference</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="from">Location of the start of the arrow (middle of the line's thickness)</param>
        /// <param name="to">Location of the end of the arrow head (the arrow point)</param>
        /// <param name="width">Width of the line (thickness)</param>
        /// <param name="headLength">How long the arrow head is (aslong as the total arrow length can support it)</param>
        /// <param name="headWidth">Width arrow head at the widest point</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="rounded">Set whether the non-point end of the arrow is rounded</param>
        void DrawArrow(IDrawStage drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        float headLength,
                        float headWidth,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false);

        /// <summary>
        // Draw Coloured Arrow
        /// </summary>
        /// <param name="drawStage">DrawStage id</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="from">Location of the start of the arrow (middle of the line's thickness)</param>
        /// <param name="to">Location of the end of the arrow head (the arrow point)</param>
        /// <param name="width">Width of the line (thickness)</param>
        /// <param name="headLength">How long the arrow head is (aslong as the total arrow length can support it)</param>
        /// <param name="headWidth">Width arrow head at the widest point</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="rounded">Set whether the non-point end of the arrow is rounded</param>
        void DrawArrow(ulong drawStage,
                        CoordinateSpace space,
                        Vector2 from,
                        Vector2 to,
                        float width,
                        float headLength,
                        float headWidth,
                        Colour colour,
                        float depth,
                        int layer = 0,
                        bool rounded = false);
    }
}