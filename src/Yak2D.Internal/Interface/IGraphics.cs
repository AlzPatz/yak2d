using System;

namespace Yak2D.Internal
{
    public interface IGraphics
    {
        bool RenderingComplete { get; }
        void PrepareForDrawing();
        void Render(float timeSinceLastDraw);
        void ReInitalise();
        bool WaitForRenderingToComplete(TimeSpan timeout);

        /// <summary>
        /// Main thread only. If the current frame's present task has ended, runs the post-render work
        /// (surface-copy callbacks, pending destruction) on the calling thread and marks rendering complete.
        /// Returns RenderingComplete.
        /// </summary>
        bool TryCompleteFrame();
    }
}