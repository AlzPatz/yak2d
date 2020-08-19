using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class ColourEffectsStageRenderer : IColourEffectsStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private Pipeline _pipeline;

        public ColourEffectsStageRenderer(IFrameworkMessenger frameworkMessenger,
                                            ISystemComponents systemComponents,
                                            IShaderLoader shaderLoader,
                                            IPipelineFactory pipelineFactory,
                                            IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                            IViewportManager viewportManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _viewportManager = viewportManager;

            Initialise();
        }

        private void Initialise()
        {
            CreateShaders();
            CreatePipeline();
        }

        public void ReInitialiseGpuResources()
        {
            Initialise();
        }

        private void CreateShaders()
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

            var uniformDescriptions = new ResourceLayoutElementDescription[2][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Factors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded,"ColourFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestAlphaBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                                _shaderPackage.Description,
                                                                                _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
        }

        public void Render(CommandList cl, IColourEffectsStageModel stage, GpuSurface surface, GpuSurface source)
        {
            if (cl == null || stage == null || source == null || surface == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Colour Effect Stage Renderer null inputs, aborting");
                return;
            }

            cl.SetPipeline(_pipeline);
            cl.SetFramebuffer(surface.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            if (stage.ClearBackgroundBeforeRender)
            {
               cl.ClearColorTarget(0, stage.ClearColour);
            }
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetGraphicsResourceSet(0, stage.FactorsResourceSet);
            cl.SetGraphicsResourceSet(1, source.ResourceSet_TexWrap);
            cl.Draw(6);
        }
    }
}