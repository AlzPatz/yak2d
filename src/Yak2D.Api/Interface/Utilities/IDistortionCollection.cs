using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// A collection used to manage the evolution, position, size, intensity of distortion 'sprites' (textured square quads) over time
    /// The user should call Update() and Draw() to evolve and display the collection
    /// <summary>
    public interface IDistortionCollection
    {
        /// <summary>
        /// Create a new distortion object (textured quad) and add to the collection
        /// </summary>
        /// <param name="cycle">The distortion object can be looped</param>
        /// <param name="coordinateSpace">The coordinate space (world or screen) that the distortion object should be drawn in</param>
        /// <param name="durationInSeconds">The length of a single cycle</param>
        /// <param name="texture">The texture reference</param>
        /// <param name="initPosition">Location the textured quad is positioned at the start of a cycle</param>
        /// <param name="finalPosition">The textured quad's final position, position is interpolated from init to final over the cycle</param>
        /// <param name="initSize">Quad (square) dimensions at the start of a cycle</param>
        /// <param name="finalSize">Quad (square) dimensions at the end of a cycle, the size is interpolated over the cycle</param>
        /// <param name="initIntensity">Drawn intensity of texture at the start of a cycle</param>
        /// <param name="finalIntensity">Drawn intensity of texture at the end of a cycle, the intensity is interpolated over the cycle</param>
        /// <param name="initialRotation">Rotation in degress at the start of a cycle</param>
        /// <param name="finalRotation">Rotation in degress at the end of a cycle, angle is interpolated during a cycle</param>
        void Add(
            LifeCycle cycle,
            CoordinateSpace coordinateSpace,
            float durationInSeconds,
            ITexture texture,
            Vector2 initPosition,
            Vector2 finalPosition,
            Vector2 initSize,
            Vector2 finalSize,
            float initIntensity,
            float finalIntensity,
            float initialRotation,
            float finalRotation
        );

        /// <summary>
        /// The user should call this during any update that it is wished the distortion objects should be advanced
        /// </summary>
        void Update(float timeStep);
        
        /// <summary>
        /// The user should call this to draw the distortion collection each frame
        /// </summary>
        /// <param name="drawing">The framework drawing service</param>
        /// <param name="stage">The distortion drawing stage the collection is to be drawn too</param>
        void Draw(IDrawing drawing, IDistortionStage stage);

        /// <summary>
        /// Removes all current distortion drawing objects from the collection
        /// </summary>   
        void ClearCollection();
    }
}