using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DistortionGradientShiftRenderer : IDistortionGraidentShiftRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IGpuSurfaceManager _surfaceManager;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;

        private ShaderPackage _shaderPackage;
        private Pipeline _pipeline; //rename internal version

        public DistortionGradientShiftRenderer(IFrameworkMessenger frameworkMessenger,
                                    ISystemComponents systemComponents,
                                    IShaderLoader shaderLoader,
                                    IPipelineFactory pipelineFactory,
                                    IGpuSurfaceManager surfaceManager,
                                    IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _surfaceManager = surfaceManager;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;

            Initialise();

        }

        private void Initialise()
        {
            CreateShadersAndFactorsUniform();
            CreatePipeline();
        }

        public void ReInitialiseGpuResources()
        {
            Initialise();
        }

        private void CreateShadersAndFactorsUniform()
        {
            var vertexLayout = new VertexLayoutDescription
            (
                16,
                0,
                new VertexElementDescription[] {
                    new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.Position),
                    new VertexElementDescription("VTex", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            var uniformDescriptions = new ResourceLayoutElementDescription[2][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("PixelSizeFactor", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("SamplerHeightMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("textureHeightMap", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded,"DistortionGradientFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateDistortionGradientShiftPipeline(_shaderPackage.UniformResourceLayout,
                                                                                _shaderPackage.Description);
        }

        public void Render(CommandList cl, IDistortionStageModel stage, GpuSurface source, GpuSurface target)
        {
            cl.SetPipeline(_pipeline);
            cl.SetFramebuffer(target.Framebuffer);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetGraphicsResourceSet(0, stage.InternalSurfacePixelShiftUniform);
            cl.SetGraphicsResourceSet(1, source.ResourceSet_TexWrap);
            cl.ClearColorTarget(0, RgbaFloat.Clear);
            cl.Draw(6);
        }

    }
}