using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CopyStageRenderer : ICopyStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private Pipeline _pipeline;

        public CopyStageRenderer(IFrameworkMessenger frameworkMessenger,
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

            var uniformDescriptions = new ResourceLayoutElementDescription[1][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded,"CopyFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestOverrideBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                            _shaderPackage.Description,
                                                                            _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
            //new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm)));
        }

        public void Render(CommandList cl, GpuSurface source, GpuSurface surface)
        {
            if (cl == null || surface == null || source == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Copy Stage Renderer null inputs, aborting");
                return;
            }

            cl.SetFramebuffer(surface.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, source.ResourceSet_TexWrap);
            cl.Draw(6);
        }
    }
}