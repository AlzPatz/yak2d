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

        private List<ulong> _surfacesForDestruction;

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

            _surfacesForDestruction = new List<ulong>();
        }

        private void GenerateSingleWhitePixel()
        {
            var texture = _imageSharpLoader.GenerateSingleWhitePixel();

            SingleWhitePixel = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(texture,
                                                                              true,
                                                                              false,
                                                                              SamplerType.Point);
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

            return _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(veldridTexture,
                                                                  true,
                                                                  false,
                                                                  SamplerType.Anisotropic);
        }

        public ITexture CreateTextureFromEmbeddedResourceInUserApplication(string texturePathWithoutExtension,
                                                                            ImageFormat imageFormat,
                                                                            SamplerType samplerType,
                                                                            bool generateMipMaps)
        {
            if (string.IsNullOrEmpty(texturePathWithoutExtension))
            {
                throw new Yak2DException("Load Embedded User Texture passed null or empty string for texture path", new ArgumentNullException("texturePathWithoutExtension"));
            }

            var fullPathWithinAssemblyWithoutFileExtension = string.Concat(_startUpProperties.TextureFolderRootName, ".", texturePathWithoutExtension);
            var pathWithSlashesChangedToDots = ReplaceForwardSlashesWithDots(fullPathWithinAssemblyWithoutFileExtension);
            return LoadTextureFromEmbeddedPngResource(false, false, pathWithSlashesChangedToDots, imageFormat, samplerType, generateMipMaps);
        }

        private string ReplaceForwardSlashesWithDots(string texturePath)
        {
            return texturePath.Replace('/', '.');
        }

        public TextureData LoadTextureColourDataFromEmbeddedResourceInUserApplication(string texturePathWithoutExtension, ImageFormat imageFormat)
        {
            if (string.IsNullOrEmpty(texturePathWithoutExtension))
            {
                throw new Yak2DException("Load Embedded User Texture Data passed null or empty string for texture path", new ArgumentNullException("texturePathWithoutExtension"));
            }

            var fullPathWithinAssemblyWithoutFileExtension = string.Concat(_startUpProperties.TextureFolderRootName, ".", texturePathWithoutExtension);
            var pathWithSlashesChangedToDots = ReplaceForwardSlashesWithDots(fullPathWithinAssemblyWithoutFileExtension);
            return LoadTextureDataFromEmbeddedPngResource(pathWithSlashesChangedToDots, imageFormat);
        }

        public ITexture CreateFontTextureFromEmbeddedResource(bool isFrameworkInternal,
                                                               string texturePathWithoutExtension,
                                                               ImageFormat imageFormat,
                                                               SamplerType samplerType)
        {
            if (string.IsNullOrEmpty(texturePathWithoutExtension))
            {
                throw new Yak2DException("Load Embedded Font Texture passed null or empty string for texture path", new ArgumentNullException("texturePathWithoutExtension"));
            }

            IAssembly assembly = isFrameworkInternal ? _fontsAssembly : _applicationAssembly;
            return LoadTextureFromEmbeddedPngResource(isFrameworkInternal,
                                                      true,
                                                      assembly,
                                                      texturePathWithoutExtension,
                                                      imageFormat,
                                                      samplerType,
                                                      true);
        }

        private ITexture LoadTextureFromEmbeddedPngResource(bool isFrameworkInternal,
                                                            bool isFontTexture,
                                                            string assetPathWithoutExtension,
                                                            ImageFormat imageFormat,
                                                            SamplerType samplerType,
                                                            bool generateMipMaps)
        {
            var assembly = isFrameworkInternal ? _surfaceAssembly : _applicationAssembly;
            return LoadTextureFromEmbeddedPngResource(isFrameworkInternal,
                                                      isFontTexture,
                                                      assembly,
                                                      assetPathWithoutExtension,
                                                      imageFormat,
                                                      samplerType,
                                                      generateMipMaps);
        }

        private ITexture LoadTextureFromEmbeddedPngResource(bool isFrameworkInternal,
                                                            bool isFontTexture,
                                                            IAssembly assembly,
                                                            string assetPathWithoutExtension,
                                                            ImageFormat imageFormat,
                                                            SamplerType samplerType,
                                                            bool generateMipMaps)
        {
            var extension = GetFileExtensionFromImageFormat(imageFormat);

            var fullAssemblyName = string.Concat(assembly.Name, ".", assetPathWithoutExtension, extension);

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

            return GenerateTextureFromStream(stream, isFrameworkInternal, isFontTexture, samplerType, generateMipMaps);
        }

        private string GetFileExtensionFromImageFormat(ImageFormat format)
        {
            var extension = "";
            switch (format)
            {
                case ImageFormat.PNG:
                    extension = ".png";
                    break;
                case ImageFormat.BMP:
                    extension = ".bmp";
                    break;
                case ImageFormat.TGA:
                    extension = ".tga";
                    break;
                case ImageFormat.GIF:
                    extension = ".gif";
                    break;
                case ImageFormat.JPG:
                    extension = ".jpg";
                    break;
            }

            return extension;
        }

        public ITexture GenerateTextureFromStream(Stream stream,
                                                  bool isFrameworkInternal,
                                                  bool isFontTexture,
                                                  SamplerType samplerType,
                                                  bool generateMipMaps)
        {
            var veldridTexture = _imageSharpLoader.GenerateVeldridTextureFromStream(stream, generateMipMaps);

            var id = _idGenerator.New();

            var surface = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(veldridTexture, isFrameworkInternal, isFontTexture, samplerType);

            return _surfaceCollection.Add(id, surface) ? new TextureReference(id) : null;
        }

        private TextureData LoadTextureDataFromEmbeddedPngResource(string assetPathWithoutExtension, ImageFormat imageFormat)
        {
            var extension = GetFileExtensionFromImageFormat(imageFormat);

            var fullAssemblyName = string.Concat(_applicationAssembly.Name, ".", assetPathWithoutExtension, extension);

            var stream = _applicationAssembly.GetManifestResourceStream(fullAssemblyName);
            if (stream == null)
            {
                var names = _applicationAssembly.GetManifestResourceNames();

                if (names.Contains(fullAssemblyName))
                {
                    _frameworkMessenger.Report("Unable to load texture data. Unknown Error. Asset stream was found by name in manifest: " + fullAssemblyName);
                }
                else
                {
                    _frameworkMessenger.Report("Unable to load texture colour data. The provided texture name was not found in assembly: " + fullAssemblyName + " | Expect location format as ASSEMBLYNAME.PATHTOTEXTURES.NAMEPROVIDED.PNG");
                }
                return default(TextureData);
            }

            return _imageSharpLoader.GenerateTextureDataFromStream(stream);
        }

        public ITexture CreateFontTextureFromFile(string path,
                                                ImageFormat imageFormat,
                                                SamplerType samplerType)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Yak2DException("Load Font Texture from File passed null or empty string for texture path", new ArgumentNullException("path"));
            }

            var extension = GetFileExtensionFromImageFormat(imageFormat);

            var filePath = string.Concat(path, extension);
            return LoadTextureFromPng(filePath, true, samplerType, true);
        }

        public ITexture CreateTextureFromFile(string path,
                                            ImageFormat imageFormat,
                                            SamplerType samplerType,
                                            bool generateMipMaps)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Yak2DException("Load Texture from File passed null or empty string for texture path", new ArgumentNullException("path"));
            }

            var extension = GetFileExtensionFromImageFormat(imageFormat);

            var filePath = Path.Combine(_startUpProperties.TextureFolderRootName, string.Concat(path, extension));
            return LoadTextureFromPng(filePath, false, samplerType, generateMipMaps);
        }

        private ITexture LoadTextureFromPng(string path,
                                            bool isFontTexture,
                                            SamplerType samplerType,
                                            bool generateMipMaps)
        {
            if (_fileSystem.Exists(path))
            {
                using (var stream = _fileSystem.OpenRead(path))
                {
                    return GenerateTextureFromStream(stream, false, isFontTexture, samplerType, generateMipMaps);
                }
            }
            else
            {
                _frameworkMessenger.Report(string.Concat("Error: Unable to find Texture file from path: ", path));
                return null;
            }
        }

        public TextureData LoadTextureColourDataFromFile(string path, ImageFormat imageFormat)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Yak2DException("Load Texture Colour Data from File passed null or empty string for texture path", new ArgumentNullException("path"));
            }

            var extension = GetFileExtensionFromImageFormat(imageFormat);

            var filePath = Path.Combine(_startUpProperties.TextureFolderRootName, string.Concat(path, extension));
            return LoadTextureDataFromPng(filePath);
        }

        private TextureData LoadTextureDataFromPng(string path)
        {
            if (_fileSystem.Exists(path))
            {
                using (var stream = _fileSystem.OpenRead(path))
                {
                    return _imageSharpLoader.GenerateTextureDataFromStream(stream);
                }
            }
            else
            {
                _frameworkMessenger.Report(string.Concat("Error: Unable to find Texture file from path: ", path));
                return default(TextureData);
            }
        }

        public ITexture CreateRgbaTextureFromPixelData(uint width,
                                                     uint height,
                                                     Rgba32[] pixelData,
                                                     SamplerType samplerType,
                                                     bool generateMipMaps,
                                                     bool isFrameworkInternal)
        {
            var id = _idGenerator.New();

            var texture = _imageSharpLoader.GenerateRgbaVeldridTextureFromPixelData(pixelData, width, height, generateMipMaps);

            var surface = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(texture, isFrameworkInternal, false, samplerType);

            _surfaceCollection.Add(id, surface);

            return new TextureReference(id);
        }

        public ITexture CreateFloat32TextureFromPixelData(uint width,
                                                        uint height,
                                                        float[] pixelData,
                                                        SamplerType samplerType)
        {
            var id = _idGenerator.New();

            var texture = _imageSharpLoader.GenerateFloat32VeldridTextureFromPixelData(pixelData, width, height);

            var surface = _gpuSurfaceFactory.CreateGpuSurfaceFromTexture(texture, false, false, samplerType);

            _surfaceCollection.Add(id, surface);

            return new TextureReference(id);
        }

        public IRenderTarget CreateRenderSurface(bool isInternal,
                                                 uint width,
                                                 uint height,
                                                 PixelFormat pixelFormat,
                                                 bool hasDepthBuffer,
                                                 bool autoClearColour,
                                                 bool autoClearDepth,
                                                 SamplerType samplerType,
                                                 uint numberOfMipLevels)
        {
            var id = _idGenerator.New();

            var surface = _gpuSurfaceFactory.CreateGpuSurface(isInternal,
                                                              width,
                                                              height,
                                                              pixelFormat,
                                                              hasDepthBuffer,
                                                              samplerType,
                                                              numberOfMipLevels,
                                                              false);

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

        public ITexture CreateGpuCpuStagingSurface(uint width, uint height, PixelFormat pixelFormat)
        {
            var id = _idGenerator.New();

            var surface = _gpuSurfaceFactory.CreateGpuSurface(true,
                                                              width,
                                                              height,
                                                              pixelFormat,
                                                              false,
                                                              SamplerType.Anisotropic,
                                                              1,
                                                              true);

            return _surfaceCollection.Add(id, surface) ? new TextureReference(id) : null;
        }

        public void RegisterSwapChainOutput(Framebuffer swapChainFrameBuffer, bool removeExisting)
        {
            if (removeExisting)
            {
                DestroySurface(MainSwapChainFrameBufferKey);
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

        public void ProcessPendingDestruction()
        {
            _surfacesForDestruction.ForEach(id =>
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
            });

            _surfacesForDestruction.Clear();
        }

        public void DestroySurface(ulong id)
        {
            _surfacesForDestruction.Add(id);
        }

        public void DestroyAllUserRenderTargets()
        {
            DestoryAllOfSurfaceType(GpuSurfaceType.RenderTarget | GpuSurfaceType.User);
        }

        public void DestroyAllUserTextures()
        {
            DestoryAllOfSurfaceType(GpuSurfaceType.Texture | GpuSurfaceType.User);
        }

        public void DestroyAllUserSurfaces()
        {
            DestoryAllOfSurfaceType(GpuSurfaceType.User);
        }

        private void DestoryAllOfSurfaceType(GpuSurfaceType type)
        {
            var ids = _surfaceCollection.ReturnAllOfType(type);

            ids.ForEach(id =>
            {
                _surfacesForDestruction.Add(id);
            });
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