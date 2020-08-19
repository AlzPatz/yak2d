namespace Yak2D.Graphics
{
    public interface IViewportFactory
    {
        IViewportModel CreateViewport(uint minx, uint miny, uint width, uint height);
    }
}