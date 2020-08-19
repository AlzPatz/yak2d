using System;
using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Angle conversion, 2d vector rotation and helpers for generating line, arrow and regular polygon vertex positions and indices lists
    /// </summary>
    public interface ICommonOperations
    {
        /// <summary>
        /// Convert Angle in Radians to Angle in Degress
        /// </summary>
        /// <param name="degrees">Angle to convert in degrees</param>
        float RadiansToDegrees(float degrees);

        /// <summary>
        /// Convert Angle in Degrees to Angle in Radians
        /// </summary>
        /// <param name="radians">Angle to convert in radians</param>
        float DegressToRadians(float radians);

        /// <summary>
        /// Returns vector that is the input vector rotated clockwise by the requests angle in radians
        /// </summary>
        /// <param name="radians">Clockwise rotation angle in radians</param>
        Vector2 RotateVectorClockwise(Vector2 v, float radians);

        /// <summary>
        /// Returns vector that is the input vector rotated clockwise by the requests angle in radians
        /// </summary>
        /// <param name="radians">Clockwise rotation angle in radians</param>
        Vector2 RotateVectorClockwise(ref Vector2 v, float radians);

        /// <summary>
        /// Returns tuple containing the position vectors and draw order indices a line or arrow
        /// </summary>
        /// <param name="start">Location of the start of the line/arrow (middle of the line's thickness)</param>
        /// <param name="end">Location of the end of the line/arrow (middle of the line's thickness or arrow head point)</param>
        /// <param name="width">Width of the line (thickness)</param>
        /// <param name="rounded">Whether the line / arrow as rounded ends</param>
        /// <param name="centreOfCurveRadiusAtLineEndPoints">Whether the start and end points represent the centre of a rounded end's radius. False implies the line / arrow has no vertices more extreme than the start / end points</param>
        /// <param name="numberOfCurveSegments">Number of segments that a rounded end is divided up into</param>
        /// <param name="headLength">How long the arrow head is (aslong as the total arrow length can support it)</param>
        /// <param name="headWidth">Width arrow head at the widest point</param>
        Tuple<Vector2[], int[]> LineAndArrowVertexAndIndicesGenerator(  Vector2 start,
                                                                        Vector2 end,
                                                                        float width,
                                                                        bool rounded = false,
                                                                        bool centreOfCurveRadiusAtLineEndPoints = true,
                                                                        int numberOfCurveSegments = 32,
                                                                        bool isArrow = false,
                                                                        float headLength = 10.0f,
                                                                        float headWidth = 10.0f
                                                                        ); 
        /// <summary>
        /// Returns tuple containing the position vectors and draw order indices a regular polygon
        /// </summary>
        /// <param name="position">Centre position of the polygon</param>
        /// <param name="numSides">The number of sides that polygon has (minimum 3 == triangle)</param>
        /// <param name="radius">Distance of each outer edge vertex point from centre</param>
        Tuple<Vector2[], int[]> RegularPolygonVertexAndIndicesGenerator(Vector2 position,
                                                                        int numSides,
                                                                        float radius);
    }
}
