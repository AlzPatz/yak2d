namespace Yak2D.Graphics
{
    public interface IDrawQueueFactory
    {
        IDrawQueue Create(bool skipDrawQueueSortingByDepthsAndLayers);
    }
}