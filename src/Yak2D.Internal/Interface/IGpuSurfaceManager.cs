using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Drawing;
using Veldrid;

namespace Yak2D.Internal
{
    public interface IGpuSurfaceManager
    {
        int TotalUserSurfaceCount { get; }
        int UserRenderTargetCount { get; }
        int UserTextureCount { get; }

        GpuSurface SingleWhitePixel { get; }
        GpuSurface Noise { get; }
        GpuSurface CrtShadowMask { get; }
        ulong MainSwapChainFrameBufferKey { get; }
        bool AutoClearMainWindowDepth { get; set; }
        bool AutoClearMainWindowColour { get; set; }

        List<ulong> GetAutoClearDepthSurfaceIds();
        List<ulong> GetAutoClearColourSurfaceIds();

        ITexture LoadTextureFromEmbeddedPngResourceInUserApplication(string texturePathWithoutExtension,
                                                                     bool isFontTexture,
                                                                     SamplerType samplerType = SamplerType.Anisotropic);

        ITexture LoadFontTextureFromEmbeddedPngResource(bool isFrameworkInternal,
                                                        bool isFontTexture,
                                                        string texturePathWithoutExtension,
                                                        SamplerType samplerType = SamplerType.Anisotropic);

        ITexture LoadRgbaTextureFromPixelData(uint width,
                                              uint height,
                                              Rgba32[] pixelData,
                                              SamplerType samplerType = SamplerType.Anisotropic);

        ITexture LoadFloat32TextureFromPixelData(uint width,
                                                 uint height,
                                                 float[] pixelData,
                                                 SamplerType samplerType = SamplerType.Anisotropic);

        ITexture LoadTextureFromPngFile(string path,
                                        bool isFontTexture,
                                        SamplerType samplerType = SamplerType.Anisotropic);

        ITexture LoadFontTextureFromPngFile(string texturePathWithoutExtension,
                                            bool isFontTexture,
                                            SamplerType samplerType = SamplerType.Anisotropic);

        IRenderTarget CreateRenderSurface(bool isInternal, uint width, uint height, PixelFormat pixelFormat,
                                          bool hasDepthBuffer, bool autoClearColour = false, bool autoClearDepth = false,
                                          SamplerType samplerType = SamplerType.Anisotropic);

        TextureDataRgba LoadTextureColourDataFromPngFile(string path);
        TextureDataRgba LoadTextureColourDataFromEmbeddedPngResourceInUserApplication(string path);
        void RegisterSwapChainOutput(Framebuffer swapChainFrameBuffer, bool removeExisting);
        GpuSurface RetrieveSurface(ulong id, GpuSurfaceType[] disallowedTypes = null);
        Size GetSurfaceDimensions(ulong id);
        void DestroySurface(ulong id);
        void DestroyAllUserRenderTargets();
        void DestroyAllUserTextures();
        void DestroyAllUserSurfaces();
        void Shutdown();
        void ReInitialise();

    }
}