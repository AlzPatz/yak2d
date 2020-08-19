namespace Yak2D
{
    /// <summary>
    /// The Line object type for the shape drawing fluent interface
    /// A line can be converted into an arrow
    /// The fluent type interface is designed for simple, rapid, iterative shape drawing
    /// </summary>
    public interface ILine : IShape
    {
        /// <summary>
        /// Convert line into an Arrow
        /// </summary>
        /// <param name="headWidth">Width of head at widest point</param>
        /// <param name="headLength">Length of arrow head (assuming total line length has room, if not it is reduced</param>
        IShape Arrow(float headWidth, float headLength);
    }
}