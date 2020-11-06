using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Holds the result of a Transform from or too Window Position
    /// </summary>
    public struct TransformResult
    {
        /// <summary>
        /// Is Point Contained within area. 
        /// When converting from a Window Position, True if the is point visible by the camera within the viewport.
        /// When converting to a window position, True if the point within the window bounds (does not consider if the point is visible within source the camera/viewport)
        /// </summary>
        public bool Contained { get; set; }

        /// <summary>
        /// The Transformed Position
        /// </summary>
        public Vector2 Position { get; set; }
    }
}