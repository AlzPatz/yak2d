using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Represents a fluent interface object that is in a drawable state
    /// Contains methods represent additional modifier options for further shape modification
    /// The fluent type interface is designed for simple, rapid, iterative shape drawing
    /// It currently only supports references (not raw unsigned integer ids). Wrap ids if required
    /// </summary>
    public interface IConstructedDrawable
    {
        /// <summary>
        /// Create and submit the draw request to draw the shape in its current state to a draw stage
        /// </summary>
        /// <param name="drawStage">DrawStage reference<</param>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])<</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        void SubmitDraw(IDrawStage drawStage,
                        CoordinateSpace space,
                        float depth,
                        int layer);
        /// <summary>
        /// Create a draw request (to submit to a DrawStage later) to draw the shape in its current state
        /// </summary>
        /// <param name="space">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])<</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        DrawRequest GenerateDrawRequest(CoordinateSpace space,
                                        float depth,
                                        int layer);

        /// <summary>
        /// Return a drawable shape object with identical parameters except modified position
        /// </summary>
        /// <param name="position">New centre position of shape</param>
        ITransformable ChangePosition(Vector2 position);

        /// <summary>
        /// Return a drawable shape object with identical parameters except modified position
        /// </summary>
        /// <param name="position">Amount to shift shape position by</param>
        ITransformable ShiftPosition(Vector2 delta);

        /// <summary>
        /// Return a drawable shape object with identical parameters except a different colour
        /// </summary>
        /// <param name="colour">The new shape colour</param>
        ITransformable ChangeColour(Colour colour);

        /// <summary>
        /// Return a drawable shape object with identical parameters except a different primary texture reference
        /// </summary>
        /// <param name="texture">New primary Texture reference</param>
        ITransformable ChangeTexture0(ITexture texture);

        /// <summary>
        /// Return a drawable shape object with identical parameters except a different secondary texture reference
        /// </summary>
        /// <param name="texture">New secopndary Texture reference</param>
        ITransformable ChangeTexture1(ITexture texture);
    }
}