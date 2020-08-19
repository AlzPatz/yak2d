using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DistortionRenderer : IDistortionRenderer
    {
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private DeviceBuffer _distortionFactorBuffer;
        private ResourceSet _distortionFactorUniformResourceSet;

        private Pipeline _pipeline; //rename internal version

        public DistortionRenderer(ISystemComponents systemComponents,
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
                new VertexElementDescription[] {
                    new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.Position),
                    new VertexElementDescription("VTex", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            var uniformDescriptions = new ResourceLayoutElementDescription[3][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("DistortionFactor", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("SamplerGradientMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("TextureGradientMap", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("SamplerImage", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("TextureImage", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded, "DistortionFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);

            _distortionFactorBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(DistortionFactorUniform.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var distortionFactorBufferResourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    uniformDescriptions[0]
                )
            );

            _distortionFactorUniformResourceSet = _systemComponents.Factory.CreateResourceSet(
                    new ResourceSetDescription(distortionFactorBufferResourceLayout, _distortionFactorBuffer)
            );
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateDistortionRenderPipeline(_shaderPackage.UniformResourceLayout,
                                                                                _shaderPackage.Description,
                                                                                _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
        }

        public void Render(CommandList cl, IDistortionStageModel stage, GpuSurface source, GpuSurface shift, GpuSurface target)
        {
            float aspect = (1.0f * target.Framebuffer.Width) / (1.0f * target.Framebuffer.Height);
            float amount = stage.DistortionScalar / (1.0f * target.Framebuffer.Height);

            var distortionFactor = new DistortionFactorUniform
            {
                DistortionScalar = amount * new Vector2(aspect, 1.0f),
                Pad2 = Vector2.Zero,
                Pad3 = Vector4.Zero
            };

            cl.SetPipeline(_pipeline);
            cl.UpdateBuffer(_distortionFactorBuffer, 0, ref distortionFactor);
            cl.SetGraphicsResourceSet(0, _distortionFactorUniformResourceSet);
            cl.SetFramebuffer(target.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetGraphicsResourceSet(1, shift.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(2, source.ResourceSet_TexWrap);
            cl.Draw(6);
        }
    }
}