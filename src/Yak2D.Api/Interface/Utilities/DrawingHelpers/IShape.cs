namespace Yak2D
{
    /// <summary>
    /// The shape object type for the shape drawing fluent interface
    /// Each of the object's methods represent a possible fill type for shape drawing
    /// The fluent type interface is designed for simple, rapid, iterative shape drawing
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// Only fill outline of shape
        /// </summary>
        /// <param name="lineWidth">Outline width</param>
        ITransformable Outline(float lineWidth);

        /// <summary>
        /// Fill all of shape
        /// </summary>
        ITransformable Filled();
    }
}