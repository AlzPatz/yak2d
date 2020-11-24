using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class MixStageRenderer : IMixStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private Veldrid.Pipeline _pipeline;

        private ResourceSet _mixFactorsResource;
        private DeviceBuffer _mixFactorsBuffer;

        private GpuSurface[] _whiteTextures;

        public MixStageRenderer(IFrameworkMessenger frameworkMessenger,
                                    ISystemComponents systemComponents,
                                    IShaderLoader shaderLoader,
                                    IPipelineFactory pipelineFactory,
                                    IFullNdcSpaceQuadVertexBuffer ndcSpaceQuadVertexBuffer,
                                    IGpuSurfaceManager gpuSurfaceManager,
                                    IViewportManager viewportManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcSpaceQuadVertexBuffer;
            _gpuSurfaceManager = gpuSurfaceManager;
            _viewportManager = viewportManager;

            Initialise();
        }

        private void Initialise()
        {
            CreatePlaceholderTextures();
            CreateShadersAndBuffer();
            CreatePipeline();
        }

        public void ReInitialiseGpuResources()
        {
            Initialise();
        }

        private void CreateShadersAndBuffer()
        {
            var vertexLayout = new VertexLayoutDescription
            (
                16,
                0,
                new VertexElementDescription[]
                {
                    new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.Position),
                    new VertexElementDescription("VTex", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            var uniformDescriptions = new ResourceLayoutElementDescription[6][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("MixFactors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("SamplerMix", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("TextureMix", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler0", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture0", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[3] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler1", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture1", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[4] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler2", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture2", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[5] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler3", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture3", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded,"MixFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);

            _mixFactorsBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(MixStageFactors.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var resourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    uniformDescriptions[0]
                )
            );

            _mixFactorsResource = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(resourceLayout, _mixFactorsBuffer)
            );
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestAlphaBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                                _shaderPackage.Description,
                                                                                _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
        }

        private void CreatePlaceholderTextures()
        {
            _whiteTextures = new GpuSurface[4];
            for(var t = 0; t < 4; t++)
            {
                var tex = _gpuSurfaceManager.LoadRgbaTextureFromPixelData(1, 1, new SixLabors.ImageSharp.PixelFormats.Rgba32[] { new SixLabors.ImageSharp.PixelFormats.Rgba32(1.0f, 1.0f, 1.0f, 1.0f) }, SamplerType.Point);
                _whiteTextures[t] = _gpuSurfaceManager.RetrieveSurface(tex.Id);
            }
        }

        public void Render(CommandList cl, IMixStageModel stage, GpuSurface mix, GpuSurface t0, GpuSurface t1, GpuSurface t2, GpuSurface t3, GpuSurface target)
        {
            if (cl == null || stage == null || target == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Mix Stage Renderer null inputs (for those that shouldn't be), aborting");
                return;
            }

            var factors = new MixStageFactors
            {
                MixAmounts = stage.MixAmount
            };
            cl.UpdateBuffer(_mixFactorsBuffer, 0, ref factors);

            cl.SetFramebuffer(target.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetPipeline(_pipeline);

            cl.SetGraphicsResourceSet(0, _mixFactorsResource);
            cl.SetGraphicsResourceSet(1, mix == null ? _gpuSurfaceManager.SingleWhitePixel.ResourceSet_TexWrap : mix.ResourceSet_TexWrap);

            cl.SetGraphicsResourceSet(2, t0 == null ? _whiteTextures[0].ResourceSet_TexWrap : t0.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(3, t1 == null ? _whiteTextures[1].ResourceSet_TexWrap : t1.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(4, t2 == null ? _whiteTextures[2].ResourceSet_TexWrap : t2.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(5, t3 == null ? _whiteTextures[3].ResourceSet_TexWrap : t3.ResourceSet_TexWrap);

            cl.Draw(6);
        }
    }
}