using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public class GpuSurfaceManager : IGpuSurfaceManager
    {
        public ulong MainSwapChainFrameBufferKey { get; private set; }
        public GpuSurface SingleWhitePixel { get; private set; }
        public GpuSurface Noise { get; private set; }
        public GpuSurface CrtShadowMask { get; private set; }

        public int TotalUserSurfaceCount { get { return UserRenderTargetCount + UserTextureCount; } }
        public int UserRenderTargetCount { get { return _surfaceCollection.CountOfType(GpuSurfaceType.RenderTarget | GpuSurfaceType.User); } }
        public int UserTextureCount { get { return _surfaceCollection.CountOfType(GpuSurfaceType.Texture | GpuSurfaceType.User); } }

        public bool AutoClearMainWindowDepth { get; set; }
        public bool AutoClearMainWindowColour { get; set; }

        public List<ulong> GetAutoClearDepthSurfaceIds()
        {
            return _surfacesToAutoClearDepth;
        }

        public List<ulong> GetAutoClearColourSurfaceIds()
        {
            return _surfacesToAutoClearColour;
        }

        private readonly IAssembly _applicationAssembly;
        private readonly IAssembly _fontsAssembly;
        private readonly IAssembly _surfaceAssembly;
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IIdGenerator _idGenerator;
        private readonly IGpuSurfaceCollection _surfaceCollection;
        private readonly IImageSharpLoader _imageSharpLoader;
        private readonly IGpuSurfaceFactory _gpuSurfaceFactory;
        private readonly ISystemComponents _components;
        private readonly IFileSystem _fileSystem;

        private readonly StartupConfig _startUpProperties;

        private List<ulong> _surfacesToAutoClearDepth;
        private List<ulong> _surfacesToAutoClearColour;

        public GpuSurfaceManager(IApplicationAssembly applicationAssembly,
                                    IFontsAssembly fontsAssembly,
                                    ISurfaceAssembly surfaceAssembly,
                                    IFrameworkMessenger frameworkMessenger,
                                    IIdGenerator idGenerator,
                                    IStartupPropertiesCache startUpPropertiesCache,
                                    IGpuSurfaceCollection gpuSurfaceCollection,
                                    IImageSharpLoader imageSharpLoader,
                                    IGpuSurfaceFactory gpuSurfaceFactory,
                                    ISystemComponents components,
                                    IFileSystem fileSystem)
        {
            _startUpProperties = startUpPropertiesCache.User;
            _applicationAssembly = applicationAssembly;
            _fontsAssembly = fontsAssembly;
            _surfaceAssembly = surfaceAssembly;
            _frameworkMessenger = frameworkMessenger;
            _idGenerator = idGenerator;
            _surfaceCollection = gpuSurfaceCollection;
            _imageSharpLoader = imageSharpLoader;
            _gpuSurfaceFactory = gpuSurfaceFactory;
            _components = components;
            _fileSystem = fileSystem;

            Initalise();
        }

        private void Initalise()
        {
            RegisterSwapChainOutput(_components.Device.SwapchainFramebuffer, false);
            GenerateSingleWhitePixel();
            LoadSystemTextures();

            _surfacesToAutoClearDepth = new List<ulong>();
            _surfacesToAutoClearColour = new List<ulong>();

            AutoClearMainWindowDepth = _startUpProperties.AutoClearMainWindowDepthEachFrame;
            AutoClearMainWindowColour = _startUpProperties.AutoClearMainWindowColourEachFrame;
        }

        private void GenerateSingleWhitePixel()
        {
            var texture = _imageSharpLoader.GenerateSingleWhitePixel();

            SingleWhitePixel = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(texture, true);
        }

        private void LoadSystemTextures()
        {
            Noise = LoadSystemTexture("noise", false);
            CrtShadowMask = LoadSystemTexture("crtshadowmask", true);
        }

        private GpuSurface LoadSystemTexture(string name, bool mipMap)
        {
            var stream = _surfaceAssembly.GetManifestResourceStream(string.Concat("Yak2D.Surface.SystemTextures.", name, ".png"));

            if (stream == null)
            {
                _frameworkMessenger.Report("Unable to load system texture: " + name);
                return null;
            }

            var veldridTexture = _imageSharpLoader.GenerateVeldridTextureFromStream(stream, mipMap);

            return _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(veldridTexture, true);
        }

        public ITexture LoadTextureFromEmbeddedPngResourceInUserApplication(string texturePathWithoutExtension)
        {
            if (string.IsNullOrEmpty(texturePathWithoutExtension))
            {
                throw new Yak2DException("Load Embedded User Texture passed null or empty string for texture path", new ArgumentNullException("texturePathWithoutExtension"));
            }

            var fullPathWithinAssemblyWithoutFileExtension = string.Concat(_startUpProperties.TextureFolderRootName, ".", texturePathWithoutExtension);
            var pathWithSlashesChangedToDots = ReplaceForwardSlashesWithDots(fullPathWithinAssemblyWithoutFileExtension);
            return LoadTextureFromEmbeddedPngResource(false, pathWithSlashesChangedToDots);
        }

        private string ReplaceForwardSlashesWithDots(string texturePath)
        {
            return texturePath.Replace('/', '.');
        }

        public ITexture LoadFontTextureFromEmbeddedPngResource(bool isFrameworkInternal, string texturePathWithoutExtension)
        {
            if (string.IsNullOrEmpty(texturePathWithoutExtension))
            {
                throw new Yak2DException("Load Embedded Font Texture passed null or empty string for texture path", new ArgumentNullException("texturePathWithoutExtension"));
            }

            IAssembly assembly = isFrameworkInternal ? _fontsAssembly : _applicationAssembly;
            return LoadTextureFromEmbeddedPngResource(isFrameworkInternal, assembly, texturePathWithoutExtension);
        }

        private ITexture LoadTextureFromEmbeddedPngResource(bool isFrameworkInternal, string assetPathWithoutExtension)
        {
            var assembly = isFrameworkInternal ? _surfaceAssembly : _applicationAssembly;
            return LoadTextureFromEmbeddedPngResource(isFrameworkInternal, assembly, assetPathWithoutExtension);
        }

        private ITexture LoadTextureFromEmbeddedPngResource(bool isFrameworkInternal, IAssembly assembly, string assetPathWithoutExtension)
        {
            var fullAssemblyName = string.Concat(assembly.Name, ".", assetPathWithoutExtension, ".png");

            var stream = assembly.GetManifestResourceStream(fullAssemblyName);
            if (stream == null)
            {
                var names = assembly.GetManifestResourceNames();

                if (names.Contains(fullAssemblyName))
                {
                    _frameworkMessenger.Report("Unable to load texture. Unknown Error. Asset stream was found by name in manifest: " + fullAssemblyName);
                }
                else
                {
                    _frameworkMessenger.Report("Unable to load texture. The provided texture name was not found in assembly: " + fullAssemblyName + " | Expect location format as ASSEMBLYNAME.PATHTOTEXTURES.NAMEPROVIDED.PNG");
                }
                return null;
            }

            return GenerateTextureFromStream(stream, isFrameworkInternal);
        }

        private ITexture GenerateTextureFromStream(Stream stream, bool isFrameworkInternal)
        {
            var veldridTexture = _imageSharpLoader.GenerateVeldridTextureFromStream(stream, true);

            var id = _idGenerator.New();

            var surface = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(veldridTexture, isFrameworkInternal);

            return _surfaceCollection.Add(id, surface) ? new TextureReference(id) : null;
        }

        public ITexture LoadFontTextureFromPngFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Yak2DException("Load Font Texture from File passed null or empty string for texture path", new ArgumentNullException("path"));
            }

            var filePath = string.Concat(path, ".png");
            return LoadTextureFromPng(filePath);
        }

        public ITexture LoadTextureFromPngFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Yak2DException("Load Texture from File passed null or empty string for texture path", new ArgumentNullException("path"));
            }

            var filePath = Path.Combine(_startUpProperties.TextureFolderRootName, string.Concat(path, ".png"));
            return LoadTextureFromPng(filePath);
        }

        private ITexture LoadTextureFromPng(string path)
        {
            if (_fileSystem.Exists(path))
            {
                using (var stream = _fileSystem.OpenRead(path))
                {
                    return GenerateTextureFromStream(stream, false);
                }
            }
            else
            {
                _frameworkMessenger.Report(string.Concat("Error: Unable to find Texture file from path: ", path));
                return null;
            }
        }

        public ITexture LoadRgbaTextureFromPixelData(uint width, uint height, Rgba32[] pixelData)
        {
            var id = _idGenerator.New();

            var texture = _imageSharpLoader.GenerateRgbaVeldridTextureFromPixelData(pixelData, width, height);

            var surface = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(texture, false);

            _surfaceCollection.Add(id, surface);

            return new TextureReference(id);
        }

        public ITexture LoadFloat32TextureFromPixelData(uint width, uint height, float[] pixelData)
        {
            var id = _idGenerator.New();

            var texture = _imageSharpLoader.GenerateFloat32VeldridTextureFromPixelData(pixelData, width, height);

            var surface = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(texture, false);

            _surfaceCollection.Add(id, surface);

            return new TextureReference(id);
        }

        public IRenderTarget CreateRenderSurface(bool isInternal, uint width, uint height, PixelFormat pixelFormat, bool hasDepthBuffer, bool autoClearColour, bool autoClearDepth, bool useLinearFilter = false)
        {
            var id = _idGenerator.New();

            var surface = _gpuSurfaceFactory.CreateGpuSurface(isInternal, width, height, pixelFormat, hasDepthBuffer, useLinearFilter);

            if (autoClearDepth && !hasDepthBuffer)
            {
                autoClearColour = false;
            }

            if (autoClearDepth)
            {
                _surfacesToAutoClearDepth.Add(id);
            }

            if (autoClearColour)
            {
                _surfacesToAutoClearColour.Add(id);
            }

            return _surfaceCollection.Add(id, surface) ? new RenderTargetReference(id) : null;
        }

        public void RegisterSwapChainOutput(Framebuffer swapChainFrameBuffer, bool removeExisting)
        {
            if (removeExisting)
            {
                if (!_surfaceCollection.Remove(MainSwapChainFrameBufferKey))
                {
                    _frameworkMessenger.Report("The removal of a surface that caused an error was due to trying to remove a swapchain render target on recreation. Shouldn't happen!");
                }
            }

            var surface = _gpuSurfaceFactory.CreateSurfaceFromSwapChainOutputBuffer(swapChainFrameBuffer);

            MainSwapChainFrameBufferKey = _idGenerator.New();

            if (!_surfaceCollection.Add(MainSwapChainFrameBufferKey, surface))
            {
                _frameworkMessenger.Report("Error trying to add new swapchain output as new gpusurface to collection");
            }
        }

        public GpuSurface RetrieveSurface(ulong id, GpuSurfaceType[] disallowedTypes = null)
        {
            var surface = _surfaceCollection.Retrieve(id);

            if (surface == null)
            {
                _frameworkMessenger.Report(string.Concat("Unable return Gpu Surface id: ", id, " -> collection does not contain this key"));
                return null;
            }

            if (disallowedTypes != null)
            {
                for (var n = 0; n < disallowedTypes.Length; n++)
                {
                    if (disallowedTypes[n].HasFlag(surface.Type))
                    {
                        _frameworkMessenger.Report(string.Concat("Unable return Gpu Surface id: ", id, " -> surface is tagged as incompatible for this operation"));
                        return null;
                    }
                }
            }

            return surface;
        }

        public Size GetSurfaceDimensions(ulong id)
        {
            var surface = RetrieveSurface(id);

            if (surface == null)
            {
                return new Size(0, 0);
            }

            //All surfaces except the framebuffer have a texture
            if (surface.Texture == null)
            {
                if (surface.Framebuffer == null)
                {
                    _frameworkMessenger.Report("Error -> unable to find suitable component to measure surface size. Unexpected Framebuffer Null");
                    return new Size(0, 0);
                }

                return new Size((int)surface.Framebuffer.Width, (int)surface.Framebuffer.Height);
            }

            if (surface.Texture == null)
            {
                _frameworkMessenger.Report("Error -> unable to find suitable component to measure surface size. Unexpected Texture Null");
                return new Size(0, 0);
            }

            return new Size((int)surface.Texture.Width, (int)surface.Texture.Height);
        }

        public void DestroySurface(ulong id)
        {
            _surfaceCollection.Remove(id);

            if (_surfacesToAutoClearDepth.Contains(id))
            {
                _surfacesToAutoClearDepth.Remove(id);
            }

            if (_surfacesToAutoClearColour.Contains(id))
            {
                _surfacesToAutoClearColour.Remove(id);
            }
        }

        public void DestroyAllUserRenderTargets()
        {
            _surfaceCollection.RemoveAllOfType(GpuSurfaceType.RenderTarget | GpuSurfaceType.User);

            _surfacesToAutoClearDepth.Clear();
            _surfacesToAutoClearColour.Clear();
        }

        public void DestroyAllUserTextures()
        {
            _surfaceCollection.RemoveAllOfType(GpuSurfaceType.Texture | GpuSurfaceType.User);
        }

        public void DestroyAllUserSurfaces()
        {
            _surfaceCollection.RemoveAllOfType(GpuSurfaceType.User);

            _surfacesToAutoClearDepth.Clear();
            _surfacesToAutoClearColour.Clear();
        }

        public void Shutdown()
        {
            _surfaceCollection.RemoveAll(true);
        }

        public void ReInitialise()
        {
            Shutdown();
            _gpuSurfaceFactory.ReCreateCachedObjects();
            Initalise();
        }
    }
}
