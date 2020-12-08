using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        ITexture CreateTextureFromEmbeddedResourceInUserApplication(string texturePathWithoutExtension,
                                                                    ImageFormat imageFormat,
                                                                    SamplerType samplerType,
                                                                    bool generateMipMaps);

        ITexture CreateFontTextureFromEmbeddedResource(bool isFrameworkInternal,
                                                       string texturePathWithoutExtension,
                                                       ImageFormat imageFormat,
                                                       SamplerType samplerType);

        ITexture CreateRgbaTextureFromPixelData(uint width,
                                                uint height,
                                                Rgba32[] pixelData,
                                                SamplerType samplerType,
                                                bool generateMipMaps,
                                                bool isFrameworkInternal);

        ITexture CreateFloat32TextureFromPixelData(uint width,
                                                   uint height,
                                                   float[] pixelData,
                                                   SamplerType samplerType);

        ITexture CreateTextureFromFile(string path,
                                       ImageFormat imageFormat,
                                       SamplerType samplerType,
                                       bool generateMipMaps);

        ITexture CreateFontTextureFromFile(string texturePathWithoutExtension,
                                           ImageFormat imageFormat,
                                           SamplerType samplerType);

        ITexture GenerateTextureFromStream(Stream stream,
                                           bool isFrameworkInternal,
                                           bool isFontTexture,
                                           SamplerType samplerType,
                                           bool generateMipMaps);

        IRenderTarget CreateRenderSurface(bool isInternal,
                                          uint width,
                                          uint height,
                                          PixelFormat pixelFormat,
                                          bool hasDepthBuffer,
                                          bool autoClearColour,
                                          bool autoClearDepth,
                                          SamplerType samplerType,
                                          uint numberOfMipLevels);

        ITexture CreateGpuCpuStagingSurface(uint width,
                                            uint height,
                                            PixelFormat pixelFormat);

        TextureData LoadTextureColourDataFromFile(string path,
                                                  ImageFormat imageFormat);

        TextureData LoadTextureColourDataFromEmbeddedResourceInUserApplication(string path,
                                                                                      ImageFormat imageFormat);

        void RegisterSwapChainOutput(Framebuffer swapChainFrameBuffer, bool removeExisting);
        GpuSurface RetrieveSurface(ulong id, GpuSurfaceType[] disallowedTypes = null);
        Size GetSurfaceDimensions(ulong id);
        void DestroySurface(ulong id);
        void DestroyAllUserRenderTargets();
        void DestroyAllUserTextures();
        void DestroyAllUserSurfaces();
        void ProcessPendingDestruction();
        void Shutdown();
        void ReInitialise();
    }
}