using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public interface IGpuSurfaceFactory
    {
        GpuSurface CreateGpuSurfaceFromTexture(Texture texture, bool isFrameworkInternal);
        GpuSurface CreateGpuSurface(bool isFrameworkInternal, uint width, uint height, PixelFormat pixelFormat, bool hasDepthBuffer, bool useLinearFilter);
        GpuSurface CreateSurfaceFromSwapChainOutputBuffer(Framebuffer framebuffer);
        void ReCreateCachedObjects();
    }
}