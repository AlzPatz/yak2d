using System.Drawing;
using System.IO;
using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// GPU Surface Operations
    /// Texture and Render Target Creation and Destruction
    /// A surface is either a Texture or a RenderTarget
    /// A RenderTarget can be drawn too, a Texture cannot
    /// Both RenderTarget and Textures can be used as Textures (pixel sources)
    /// </summary>
    public interface ISurfaces
    {
        /// <summary>
        /// Surfaces includes all Textures and RenderTargets
        /// </summary>
        int TotalUserSurfaceCount { get; }

        /// <summary>
        /// Render Targets can be drawn too, and used in place of textures as a source of pixel data
        /// </summary>
        int UserRenderTargetCount { get; }

        /// <summary>
        /// Textures cannot be drawn too, they do not have a depth component
        /// <summary>
        int UserTextureCount { get; }

        /// <summary>
        /// Returns the RenderTarget reference of the current window's main rendering surface
        /// </summary>
        IRenderTarget ReturnMainWindowRenderTarget();

        /// <summary>
        /// Create a GPU drawing surface. RenderTargets can be sampled inplace of Textures
        /// </summary>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="autoClearColourAndDepthEachFrame">Instruct the framework to clear the depth and colour pixel data of the render target before each render iteration. Helps ensure the user does not forget too</param>
        /// <param name="samplerType">When the Render Target is sampled as a texture, which sampler filter should be used</param>
        /// <param name="numberOfMipMapLevels">How many MipMap levels should be created (>0, 1 for none)</param>
        IRenderTarget CreateRenderTarget(uint width,
                                         uint height,
                                         bool autoClearColourAndDepthEachFrame = true,
                                         SamplerType samplerType = SamplerType.Anisotropic,
                                         uint numberOfMipMapLevels = 1);

        /// <summary>
        /// Load a texture from an image asset
        /// </summary>
        /// <param name="path">Image asset name including path (no file extension)"</param>
        /// <param name="assetType">Embedded in the binary or a file location"</param>
        /// <param name="imageFormat">Image file encoding"</param>
        /// <param name="samplerType">Sampler filter to be when sampling the Texture/param>
        /// <param name="generateMipMaps">Should MipMaps be created</param>
        ITexture LoadTexture(string path,
                             AssetSourceEnum assetType,
                             ImageFormat imageFormat = ImageFormat.PNG,
                             SamplerType samplerType = SamplerType.Anisotropic,
                             bool generateMipMaps = true);

        /// <summary>
        /// Load a texture from a stream
        /// </summary>
        /// <param name="stream">"Data stream containing image data"</param>
        /// <param name="samplerType">Sampler filter to be when sampling the Texture/param>
        /// <param name="generateMipMaps">Should MipMaps be created</param>
        ITexture LoadTexture(Stream stream, 
                             SamplerType samplerType = SamplerType.Anisotropic,
                             bool generateMipMaps = true);

        /// <summary>
        /// Loads texture colour data from a .png image asset
        /// </summary>
        /// <param name="path">Image (.png) asset name including path (no file extension)"</param>
        /// <param name="assetType">Embedded in the binary or a file location"</param>
        /// <param name="imageFormat">Image file encoding"</param>
        TextureData LoadTextureColourData(string path,
                                              AssetSourceEnum assetType,
                                              ImageFormat imageFormat = ImageFormat.PNG);

        /// <summary>
        /// Geneate a texture of floats (used in distortion stages) directly from an array
        /// </summary>
        /// <param name="textureWidth">Width in pixels</param>
        /// <param name="textureHeight">Height in pixels</param>
        /// <param name="pixels">Pixel data, one dimensional array, texture top-left pixel at 0 index, data ordered in rows</param>
        /// <param name="samplerType">Sampler filter to be when sampling the Texture/param>
        ITexture CreateFloat32FromData(uint textureWidth,
                                       uint textureHeight,
                                       float[] pixels,
                                       SamplerType samplerType = SamplerType.Anisotropic);


        /// <summary>
        /// Geneate a texture of RGBA pixels directly from an array of Vector4s
        /// </summary>
        /// <param name="textureWidth">Width in pixels</param>
        /// <param name="textureHeight">Height in pixels</param>
        /// <param name="pixels">Pixel data, one dimensional array, texture top-left pixel at 0 index, data ordered in rows</param>
        /// <param name="samplerType">Sampler filter to be when sampling the Texture/param>
        ITexture CreateRgbaFromData(uint textureWidth,
                                    uint textureHeight,
                                    Vector4[] pixels,
                                    SamplerType samplerType = SamplerType.Anisotropic,
                                    bool generateMipMaps = true);


        /// <summary>
        /// Return dimensions of a surface
        /// </summary>
        /// <param name="surface">Surface (Texture or RenderTarget) reference</param>
        Size GetSurfaceDimensions(ITexture surface);

        /// <summary>
        /// Return dimensions of a surface
        /// </summary>
        /// <param name="surface">Surface (Texture or RenderTarget) id</param>
        Size GetSurfaceDimensions(ulong surface);

        /// <summary>
        /// Modify whether the framework clears the main window render target depth at the start of each draw frame
        /// </summary>
        /// <param name="autoClearDepth">To clear or not to clear</param>
        void SetMainWindowRenderTargetAutoClearDepth(bool autoClearDepth);

        /// <summary>
        /// Modify whether the framework clears the main window render target colour at the start of each draw frame
        /// Automatic clearing of colour sets pixels to RGBA 0,0,0,0 (transparent black)
        /// </summary>
        /// <param name="autoClearColour">To clear or not to clear</param>
        void SetMainWindowRenderTargetAutoClearColour(bool autoClearColour);

        /// <summary>
        /// Remove surface object data from framework
        /// </summary>
        /// <param name="surface">The surface (RenderTarget or Texture) reference to destroy</param>
        void DestroySurface(ITexture surface);

        /// <summary>
        /// Remove surface object data from framework
        /// </summary>
        /// <param name="surface">The surface (RenderTarget or Texture) id to destroy</param>
        void DestroySurface(ulong surface);

        /// <summary>
        /// Destroy all user surfaces (render targets and textures)
        /// </summary>
        void DestoryAllUserSurfaces();

        /// <summary>
        /// Destroy all user render targets
        /// </summary>
        void DestroyAllUserRenderTargets();

        /// <summary>
        /// Destroy all user textures
        /// </summary>
        void DestroyAllUserTextures();
    }
}