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
        ITexture LoadTextureFromEmbeddedPngResourceInUserApplication(string texturePathWithoutExtension);
        ITexture LoadFontTextureFromEmbeddedPngResource(bool isFrameworkInternal, string texturePathWithoutExtension);
        ITexture LoadRgbaTextureFromPixelData(uint width, uint height, Rgba32[] pixelData);
        ITexture LoadFloat32TextureFromPixelData(uint width, uint height, float[] pixelData);
        ITexture LoadTextureFromPngFile(string path);
        ITexture LoadFontTextureFromPngFile(string texturePathWithoutExtension);
        IRenderTarget CreateRenderSurface(bool isInternal, uint width, uint height, PixelFormat pixelFormat, bool hasDepthBuffer, bool autoClearColour = false, bool autoClearDepth = false, bool userLinearFilter = false);
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