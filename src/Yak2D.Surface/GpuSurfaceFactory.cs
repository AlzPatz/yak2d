using System;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public class GpuSurfaceFactory : IGpuSurfaceFactory
    {
        private const uint _numberOfMipLevels = 1;
        private const TextureSampleCount _sampleCount = TextureSampleCount.Count1;
        private const int _maximumAnistrophy = 8;

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _components;

        private ResourceLayout _cachedTextureResourceLayout;

        private Sampler _anisotropicSamplerWrap;
        private Sampler _anisotropicSamplerMirror;
        private Sampler _linearSamplerWrap;
        private Sampler _linearSamplerMirror;
        private Sampler _pointSamplerWrap;
        private Sampler _pointSamplerMirror;

        public GpuSurfaceFactory(IFrameworkMessenger frameworkMessenger, ISystemComponents components)
        {
            _frameworkMessenger = frameworkMessenger;
            _components = components;

            GenerateAnisotropicTextureSamplers();
            GenerateLinearTextureSamplers();
            GeneratePointTextureSamplers();
            GenerateCachedTextureLayout();
        }

        private void GenerateAnisotropicTextureSamplers()
        {
            _anisotropicSamplerWrap = GenerateAnisotropicTextureSampler(TextureCoordinateMode.Wrap);
            _anisotropicSamplerMirror = GenerateAnisotropicTextureSampler(TextureCoordinateMode.Mirror);
        }

        private void GenerateLinearTextureSamplers()
        {
            _linearSamplerWrap = GenerateLinearTextureSampler(TextureCoordinateMode.Wrap);
            _linearSamplerMirror = GenerateLinearTextureSampler(TextureCoordinateMode.Mirror);
        }

        private void GeneratePointTextureSamplers()
        {
            _pointSamplerWrap = GeneratePointTextureSampler(TextureCoordinateMode.Wrap);
            _pointSamplerMirror = GeneratePointTextureSampler(TextureCoordinateMode.Mirror);
        }

        private Sampler GenerateAnisotropicTextureSampler(TextureCoordinateMode texCoordMode)
        {
            //Default to linear if not supported
            var samplerFilter = _components.Device.SamplerAnisotropy ? SamplerFilter.Anisotropic : SamplerFilter.MinLinear_MagLinear_MipLinear;

            SamplerAddressMode samplerAddressMode = SamplerAddressMode.Wrap;
            switch (texCoordMode)
            {
                case TextureCoordinateMode.Mirror:
                    samplerAddressMode = SamplerAddressMode.Mirror;
                    break;
                case TextureCoordinateMode.Wrap:
                    samplerAddressMode = SamplerAddressMode.Wrap;
                    break;
            }

            return _components.Factory.CreateSampler(new SamplerDescription(
                samplerAddressMode,
                samplerAddressMode,
                samplerAddressMode,
                samplerFilter,
                null,
                _maximumAnistrophy,
                0, 0, 0,
                SamplerBorderColor.TransparentBlack
            ));
        }

        private Sampler GenerateLinearTextureSampler(TextureCoordinateMode texCoordMode)
        {
            var samplerFilter = SamplerFilter.MinLinear_MagLinear_MipLinear;

            SamplerAddressMode samplerAddressMode = SamplerAddressMode.Wrap;
            switch (texCoordMode)
            {
                case TextureCoordinateMode.Mirror:
                    samplerAddressMode = SamplerAddressMode.Mirror;
                    break;
                case TextureCoordinateMode.Wrap:
                    samplerAddressMode = SamplerAddressMode.Wrap;
                    break;
            }

            return _components.Factory.CreateSampler(new SamplerDescription(
                samplerAddressMode,
                samplerAddressMode,
                samplerAddressMode,
                samplerFilter,
                null,
                _maximumAnistrophy,
                0, 0, 0,
                SamplerBorderColor.TransparentBlack
            ));
        }


        private Sampler GeneratePointTextureSampler(TextureCoordinateMode texCoordMode)
        {
            var samplerFilter = SamplerFilter.MinPoint_MagPoint_MipPoint;

            SamplerAddressMode samplerAddressMode = SamplerAddressMode.Wrap;
            switch (texCoordMode)
            {
                case TextureCoordinateMode.Mirror:
                    samplerAddressMode = SamplerAddressMode.Mirror;
                    break;
                case TextureCoordinateMode.Wrap:
                    samplerAddressMode = SamplerAddressMode.Wrap;
                    break;
            }

            return _components.Factory.CreateSampler(new SamplerDescription(
                samplerAddressMode,
                samplerAddressMode,
                samplerAddressMode,
                samplerFilter,
                null,
                _maximumAnistrophy,
                0, 0, 0,
                SamplerBorderColor.TransparentBlack
            ));

        }

        private void GenerateCachedTextureLayout()
        {
            _cachedTextureResourceLayout = _components.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Sampler", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            );
        }

        public GpuSurface CreateGpuSurfaceFromTexture(Texture texture,
                                                      bool isFrameworkInternal,
                                                      bool isFontTexture,
                                                      SamplerType samplerType = SamplerType.Anisotropic)
        {
            if (texture == null)
            {
                throw new Yak2DException("Internal Framework Exception: GpuSurfaceFactory passed a null Veldrid Texture", new ArgumentNullException("texture"));
            }

            var view = _components.Factory.CreateTextureView(texture);

            Sampler wrap = null;
            Sampler mirror = null;

            switch (samplerType)
            {
                case SamplerType.Anisotropic:
                    wrap = _anisotropicSamplerWrap;
                    mirror = _anisotropicSamplerMirror;
                    break;
                case SamplerType.Linear:
                    wrap = _linearSamplerWrap;
                    mirror = _linearSamplerMirror;
                    break;
                case SamplerType.Point:
                    wrap = _pointSamplerWrap;
                    mirror = _pointSamplerMirror;
                    break;
            }

            var resourceSet_Wrap = _components.Factory.CreateResourceSet(new ResourceSetDescription(
               _cachedTextureResourceLayout,
               view,
               wrap
            ));

            var resourceSet_Mirror = _components.Factory.CreateResourceSet(new ResourceSetDescription(
                _cachedTextureResourceLayout,
                view,
                mirror
            ));

            return new GpuSurface
            {
                //Note Surfaces related to USER FONTS are also tagged as internal. This is so their destruction is related to
                //The FONT item and not caught up in any DestroyAllUserSurfaces type calls (that being reserved for TEXTURES and RENDERTARGETS
                Type = GpuSurfaceType.Texture | (isFrameworkInternal ? GpuSurfaceType.Internal : isFontTexture ? GpuSurfaceType.Internal : GpuSurfaceType.User),
                Texture = texture,
                TextureView = view,
                Framebuffer = null,
                ResourceSet_TexWrap = resourceSet_Wrap,
                ResourceSet_TexMirror = resourceSet_Mirror
            };
        }

        public GpuSurface CreateGpuSurface(bool isFrameworkInternal,
                                                                            uint width,
                                                                            uint height,
                                                                            PixelFormat pixelFormat,
                                                                            bool hasDepthBuffer,
                                                                            SamplerType samplerType = SamplerType.Anisotropic)
        {
            if (width == 0 || height == 0)
            {
                throw new Yak2DException("Internal Framework Exception: GpuSurfaceFactory, texture size must be non 0");
            }

            var texture = _components.Factory.CreateTexture(TextureDescription.Texture2D(
                        width,
                        height,
                        _numberOfMipLevels,
                        1,
                        pixelFormat,
                        TextureUsage.RenderTarget | TextureUsage.Sampled,
                        _sampleCount
                        ));

            var view = _components.Factory.CreateTextureView(texture);

            var depthStencil = hasDepthBuffer ?
                            _components.Factory.CreateTexture(TextureDescription.Texture2D(
                                width, height, 1, 1, PixelFormat.R16_UNorm, TextureUsage.DepthStencil
                            )) : null;

            var buffer = _components.Factory.CreateFramebuffer(new FramebufferDescription(hasDepthBuffer ? depthStencil : null, texture));

            Sampler wrap = null;
            Sampler mirror = null;

            switch (samplerType)
            {
                case SamplerType.Anisotropic:
                    wrap = _anisotropicSamplerWrap;
                    mirror = _anisotropicSamplerMirror;
                    break;
                case SamplerType.Linear:
                    wrap = _linearSamplerWrap;
                    mirror = _linearSamplerMirror;
                    break;
                case SamplerType.Point:
                    wrap = _pointSamplerWrap;
                    mirror = _pointSamplerMirror;
                    break;
            }

            var resourceSet_wrap = _components.Factory.CreateResourceSet(new ResourceSetDescription(
                _cachedTextureResourceLayout,
                view,
                wrap
             ));

            var resourceSet_mirror = _components.Factory.CreateResourceSet(new ResourceSetDescription(
                _cachedTextureResourceLayout,
                view,
                mirror
            ));

            return new GpuSurface
            {
                Type = isFrameworkInternal ? GpuSurfaceType.RenderTarget | GpuSurfaceType.Internal : GpuSurfaceType.RenderTarget | GpuSurfaceType.User,
                Texture = texture,
                TextureView = view,
                Framebuffer = buffer,
                ResourceSet_TexWrap = resourceSet_wrap,
                ResourceSet_TexMirror = resourceSet_mirror
            };
        }

        public GpuSurface CreateSurfaceFromSwapChainOutputBuffer(Framebuffer framebuffer)
        {
            if (framebuffer == null)
            {
                throw new Yak2DException("Internal Framework Exception: GpuSurfaceFactory, null framebuffer passed to creatre surface");
            }

            return new GpuSurface
            {
                Type = GpuSurfaceType.SwapChainOutput | GpuSurfaceType.Internal,
                Texture = null,
                TextureView = null,
                Framebuffer = framebuffer,
                ResourceSet_TexWrap = null,
                ResourceSet_TexMirror = null
            };
        }

        public void ReCreateCachedObjects()
        {
            GenerateAnisotropicTextureSamplers();
            GenerateLinearTextureSamplers();
            GenerateCachedTextureLayout();
        }
    }
}