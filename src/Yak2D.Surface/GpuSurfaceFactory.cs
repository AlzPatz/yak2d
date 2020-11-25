using System;
using Veldrid;
using Yak2D.Internal;
using Yak2D.Utility;

namespace Yak2D.Surface
{
    public class GpuSurfaceFactory : IGpuSurfaceFactory
    {
        private const int MAXIMUM_ANISTROPHY = 8;

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _components;

        private ResourceLayout _cachedTextureResourceLayout;

        private Sampler _anisotropicSamplerWrap;
        private Sampler _anisotropicSamplerMirror;
        private Sampler _linearSamplerWrap;
        private Sampler _linearSamplerMirror;
        private Sampler _pointSamplerWrap;
        private Sampler _pointSamplerMirror;
        private Sampler _pointMagLinearMinSamplerWrap;
        private Sampler _pointMagLinearMinSamplerMirror;

        public GpuSurfaceFactory(IFrameworkMessenger frameworkMessenger, ISystemComponents components)
        {
            _frameworkMessenger = frameworkMessenger;
            _components = components;

            GenerateTextureSamplers();
            GenerateCachedTextureLayout();
        }

        private void GenerateTextureSamplers()
        {
            _anisotropicSamplerWrap = GenerateTextureSampler(TextureCoordinateMode.Wrap, SamplerFilter.Anisotropic);
            _anisotropicSamplerMirror = GenerateTextureSampler(TextureCoordinateMode.Mirror, SamplerFilter.Anisotropic);
            _linearSamplerWrap = GenerateTextureSampler(TextureCoordinateMode.Wrap, SamplerFilter.MinLinear_MagLinear_MipLinear);
            _linearSamplerMirror = GenerateTextureSampler(TextureCoordinateMode.Mirror, SamplerFilter.MinLinear_MagLinear_MipLinear);
            _pointSamplerWrap = GenerateTextureSampler(TextureCoordinateMode.Wrap, SamplerFilter.MinPoint_MagPoint_MipPoint);
            _pointSamplerMirror = GenerateTextureSampler(TextureCoordinateMode.Mirror, SamplerFilter.MinPoint_MagPoint_MipPoint);
            _pointMagLinearMinSamplerWrap = GenerateTextureSampler(TextureCoordinateMode.Wrap, SamplerFilter.MinLinear_MagPoint_MipLinear);
            _pointMagLinearMinSamplerMirror = GenerateTextureSampler(TextureCoordinateMode.Mirror, SamplerFilter.MinLinear_MagPoint_MipLinear);
        }

        private Sampler GenerateTextureSampler(TextureCoordinateMode texCoordMode, SamplerFilter samplerFilter)
        {
            //Default to linear if not supported
            if (samplerFilter == SamplerFilter.Anisotropic && !_components.Device.SamplerAnisotropy)
            {
                samplerFilter = SamplerFilter.MinLinear_MagLinear_MipLinear;
            }

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
                MAXIMUM_ANISTROPHY,
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
                                                      SamplerType samplerType)
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
                                           SamplerType samplerType,
                                           uint numberMipLevels,
                                           TexSampleCount sampleCount,
                                           bool isGpuToCpuStagingTexture)
        {
            if (width == 0 || height == 0)
            {
                throw new Yak2DException("Internal Framework Exception: GpuSurfaceFactory, texture size must be non 0");
            }

            if(numberMipLevels == 0)
            {
                numberMipLevels = 1;
            }

            var texture = _components.Factory.CreateTexture(TextureDescription.Texture2D(
                        width,
                        height,
                        numberMipLevels,
                        1,
                        pixelFormat,
                        isGpuToCpuStagingTexture ? TextureUsage.Staging : TextureUsage.RenderTarget | TextureUsage.Sampled,
                        TextureSampleCountConverter.ConvertYakToVeldrid(sampleCount)
                        ));

            var view = isGpuToCpuStagingTexture ? null : _components.Factory.CreateTextureView(texture);

            var depthStencil = hasDepthBuffer ?
                            _components.Factory.CreateTexture(TextureDescription.Texture2D(
                                width, height, 1, 1, PixelFormat.R16_UNorm, TextureUsage.DepthStencil
                            )) : null;

            var buffer = isGpuToCpuStagingTexture ? null : _components.Factory.CreateFramebuffer(new FramebufferDescription(hasDepthBuffer ? depthStencil : null, texture));

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
                case SamplerType.PointMagLinearMin:
                    wrap = _pointMagLinearMinSamplerWrap;
                    mirror = _pointMagLinearMinSamplerMirror;
                    break;
            }

            var resourceSet_wrap = isGpuToCpuStagingTexture ? null : _components.Factory.CreateResourceSet(new ResourceSetDescription(
                _cachedTextureResourceLayout,
                view,
                wrap
             ));

            var resourceSet_mirror = isGpuToCpuStagingTexture ? null : _components.Factory.CreateResourceSet(new ResourceSetDescription(
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
            GenerateTextureSamplers();
            GenerateCachedTextureLayout();
        }
    }
}