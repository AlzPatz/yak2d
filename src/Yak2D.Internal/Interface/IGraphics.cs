namespace Yak2D.Internal
{
    public interface IGraphics
    {
        bool RenderingComplete { get; }
        void PrepareForDrawing();
        void Render(float timeSinceLastDraw);
        void ReInitalise();
    }
}