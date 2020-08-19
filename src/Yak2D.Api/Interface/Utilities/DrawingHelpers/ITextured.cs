using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// The Filled object type for the shape drawing fluent interface
    /// Each of the object's methods represent a possible shape type for shape drawing
    /// The fluent type interface is designed for simple, rapid, iterative shape drawing
    /// </summary>
    public interface ITextured
    {
        /// <summary>
        /// A quad (rectangle/square) made of two triangles
        /// </summary>
        /// <param name="position">Centre position of shape</param>
        /// <param name="width">Width of shape</param>
        /// <param name="height">Height of shape</param>
        IShape Quad(Vector2 position,
                    float width,
                    float height);

        /// <summary>
        /// A regular polygon (equal side lengths). Can approximate a circle with a high enough number of sides 
        /// </summary>
        /// <param name="position">Centre position of shape</param>
        /// <param name="numSides">The number of sides (all equal length). Large n --> circle</param>
        /// <param name="radius">The shape radius (distance of each point on the outside from the centre)</param>
        IShape Poly(Vector2 position,
                    int numSides,
                    float radius);

        /// <summary>
        /// A polygon built out of user defined vertices, indices and texture coordiantes
        /// To render properly, the index referenced vertices must form a list of triangles
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="texCoords"></param>
        IShape Poly(Vector2[] vertices,
                    int[] indices,
                    Vector2[] texCoords);

        /// <summary>
        /// A polygon built out of user defined vertices and indices
        /// To render properly, the index referenced vertices must form a list of triangles
        /// If texture mapped, the texture coordinates are automatically generated
        /// If texture mapping is for a tiled texture, the centre point between all the shapes will be taken as the centre of the texture
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        IShape Poly(Vector2[] vertices,
                    int[] indices);

        /// <summary>
        /// A line
        /// </summary>
        /// <param name="from">Start point of the line</param>
        /// <param name="too">End point of the line</param>
        /// <param name="width">Width (thickness) of the line</param>
        ILine Line(Vector2 from,
                   Vector2 too,
                   float width);
        /// <summary>
        /// A line with rounded ends
        /// The radius of the end rounding is equal to the line width (thickness)
        /// </summary>
        /// <param name="from">Start point of the line</param>
        /// <param name="too">End point of the line</param>
        /// <param name="width">Width (thickness) of the line</param>
        /// <param name="numberOfCurveSegments">Rounded ends will be rendered by this number of segments (minimum 2)</param>
        /// <param name="centreOfCurveRadiusAtLineEndPoints">If true, the 'from' and 'too' line positions will be positioned in the centre of the theoretical end rounding circles (the rounded ends will spill over the end points of the lines)</param>
        ILine RoundedLine(Vector2 from,
                          Vector2 too,
                          float width,
                          int numberOfCurveSegments,
                          bool centreOfCurveRadiusAtLineEndPoints = true);
    }
}