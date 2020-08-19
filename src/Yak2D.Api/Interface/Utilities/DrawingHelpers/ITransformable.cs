namespace Yak2D
{
    /// <summary>
    /// Represents a fluent interface object that is in a drawable state, but can also be scaled and rotated
    /// in addition to the operations possible on a IConstructedDrawable type
    /// Contains methods for those scaling and rotation operations
    /// The fluent type interface is designed for simple, rapid, iterative shape drawing
    /// </summary>

    public interface ITransformable : IConstructedDrawable
    {
        /// <summary>
        /// Return a drawable (transformable) shape object with identical parameters except scaled
        /// </summary>
        /// <param name="xScaling">X dimension scaling factor (negative numbers will be mulitplied by -1)</param>
        /// <param name="yScaling">Y dimension scaling factor (negative numbers will be mulitplied by -1)<</param>
        ITransformable Scale(float xScaling,
                             float yScaling);


        /// <summary>
        /// Return a drawable (transformable) shape object with identical parameters except rotated
        /// </summary>
        /// <param name="angle_clockwise_radians">Angle to rotate the shape</param>
        ITransformable Rotate(float angle_clockwise_radians);
    }
}