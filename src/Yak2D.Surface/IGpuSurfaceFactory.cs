using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public interface IGpuSurfaceFactory
    {
        GpuSurface CreateGpuSurfaceFromTexture(Texture texture, bool isFrameworkInternal, SamplerType samplerType = SamplerType.Anisotropic);
        GpuSurface CreateGpuSurface(bool isFrameworkInternal, uint width, uint height, PixelFormat pixelFormat, bool hasDepthBuffer, SamplerType samplerType = SamplerType.Anisotropic);
        GpuSurface CreateSurfaceFromSwapChainOutputBuffer(Framebuffer framebuffer);
        void ReCreateCachedObjects();
    }
}