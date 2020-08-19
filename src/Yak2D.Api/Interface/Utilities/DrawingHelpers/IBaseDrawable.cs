namespace Yak2D
{
    /// <summary>
    /// The base object type for the shape drawing fluent interface
    /// Each of the object's methods represent a possible fill type for shape drawing
    /// The fluent type interface is designed for simple, rapid, iterative shape drawing
    /// It currently only supports texture references (not raw unsigned integer ids). Wrap ids if required
    /// </summary>
    public interface IBaseDrawable
    {
        /// <summary>
        /// Draw shape with a single solid Colour
        /// </summary>
        /// <param name="colour">The colour</param>
        ITextured Coloured(Colour colour);

        /// <summary>
        /// Draw shape with a single Texture, along with an overall mix colour
        /// </summary>
        /// <param name="texture">The Texture reference</param>
        /// <param name="colour">The overall mix colour (use White for no change to texture colour)</param>
        ITextured Textured(TextureBrush texture,
                           Colour colour);
        /// <summary>
        /// Draw shape with a transition between two textures, along with an overall mix colour
        /// </summary>
        /// <param name="texture0">The first Texture reference</param>
        /// <param name="texture1">The second Texture reference</param>
        /// <param name="mix">The Texture transition / mixing direction</param>
        /// <param name="colour">The overall mix colour (use White for no change to texture colour)</param>
        ITextured DualTextured(TextureBrush texture0,
                               TextureBrush texture1,
                               TextureMixDirection mix,
                               Colour colour);
    }
}