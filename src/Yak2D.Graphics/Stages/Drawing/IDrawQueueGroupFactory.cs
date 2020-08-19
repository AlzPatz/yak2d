namespace Yak2D.Graphics
{
    public interface IDrawQueueGroupFactory
    {
        IDrawQueueGroup Create(bool skipDrawQueueSortingByDepthsAndLayers);
    }
}
