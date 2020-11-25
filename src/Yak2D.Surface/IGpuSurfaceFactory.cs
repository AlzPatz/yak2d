using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public interface IGpuSurfaceFactory
    {
        GpuSurface CreateGpuSurfaceFromTexture(Texture texture,
                                               bool isFrameworkInternal,
                                               bool isFontTexture,
                                               SamplerType samplerType);

        GpuSurface CreateGpuSurface(bool isFrameworkInternal,
                                    uint width,
                                    uint height,
                                    PixelFormat pixelFormat,
                                    bool hasDepthBuffer,
                                    SamplerType samplerType,
                                    uint numberMipLevels,
                                    TexSampleCount sampleCount,
                                    bool isGpuToCpuStagingTexture);

        GpuSurface CreateSurfaceFromSwapChainOutputBuffer(Framebuffer framebuffer);

        void ReCreateCachedObjects();
    }
}