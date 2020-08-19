namespace Yak2D.Internal
{
    public interface IFontModel
    {
        SubFont SubFontAtSize(float size);
        void ReleaseReources(IGpuSurfaceManager gpuSurfaceManager);
    }
}