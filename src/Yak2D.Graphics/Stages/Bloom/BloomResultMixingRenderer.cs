using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class BloomResultMixingRenderer : IBloomResultMixingRenderer
    {
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private DeviceBuffer _uniformBlockBuffer;
        private ResourceSet _uniformBlockResourceSet;
        private Veldrid.Pipeline _pipeline;

        private float _currentMixAmount = 1.0f;

        public BloomResultMixingRenderer(ISystemComponents systemComponents,
                                                                        IShaderLoader shaderLoader,
                                                                        IPipelineFactory pipelineFactory,
                                                                        IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                                                        IViewportManager viewportManager)
        {
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _viewportManager = viewportManager;

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
                new VertexElementDescription[]
                {
                    new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.Position),
                    new VertexElementDescription("VTex", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            var uniformDescriptions = new ResourceLayoutElementDescription[3][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("UniformBlock", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler_Source", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture_Source", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler_Processed", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture_Processed", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded, "BloomMixFragment",AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);

            var uniformBlockResourceLayoutDescription = uniformDescriptions[0][0];

            _uniformBlockBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(MixingShaderFactors.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var uniformBlockResourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    uniformBlockResourceLayoutDescription
                )
            );

            _uniformBlockResourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(uniformBlockResourceLayout, _uniformBlockBuffer)
            );
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestOverrideBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                                _shaderPackage.Description,
                                                                                _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
        }

        public void Render(CommandList cl, IBloomStageModel stage, GpuSurface original, GpuSurface bloom, GpuSurface target)
        {
            if (stage.MixAmount != _currentMixAmount)
            {
                _currentMixAmount = stage.MixAmount;
                var uniforms = new MixingShaderFactors
                {
                    MixAmount = _currentMixAmount,
                    Pad0 = 0.0f,
                    Pad1 = 0.0f,
                    Pad2 = 0.0f,
                    Pad3 = Vector4.Zero
                };

                cl.UpdateBuffer(_uniformBlockBuffer, 0, ref uniforms);
            }

            cl.SetFramebuffer(target.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, _uniformBlockResourceSet);
            cl.SetGraphicsResourceSet(1, original.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(2, bloom.ResourceSet_TexWrap);
            cl.Draw(6);
        }
    }
}