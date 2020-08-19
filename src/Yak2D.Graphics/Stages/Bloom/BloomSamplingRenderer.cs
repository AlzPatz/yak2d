using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class BloomSamplingRenderer : IBloomSamplingRenderer
    {
        //SAME AS DOWNSAMPLING RENDERER BUT JUST HAS THE THRESHOLD VALUE... ELECTING TO COPY DESPITE JUST BEING A SINGLE CONDITION EXTRA IN SHADER
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IDownSamplerWeightsAndOffsets _downSamplerValues;

        private ShaderPackage _shaderPackage;
        private Veldrid.Pipeline _pipeline;

        private DeviceBuffer _samplerFactorsUniformBlockBuffer;
        private ResourceSet _samplerFactorsUniformBlockResourceSet;
        private DeviceBuffer _weightsAndOffsetsDeviceBuffer;
        private ResourceSet _weightsAndOffsetsResourceSet;

        private ResizeSamplerType? _sampleType;

        public BloomSamplingRenderer(ISystemComponents systemComponents,
                                        IShaderLoader shaderLoader,
                                        IPipelineFactory pipelineFactory,
                                        IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                        IDownSamplerWeightsAndOffsets downSamplerWeightsAndOffsets)
        {
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _downSamplerValues = downSamplerWeightsAndOffsets;

            Initialise();

            _sampleType = null;
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
                new ResourceLayoutElementDescription("Sampler", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("SampleUniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("WeightsAndOffsets", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded, "BloomSamplingFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);

            var samplerUniformBlockResourceLayoutDescription = uniformDescriptions[1][0];

            _samplerFactorsUniformBlockBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(BloomDownSampleFactors.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var samplerFactorsResourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    samplerUniformBlockResourceLayoutDescription
                )
            );

            _samplerFactorsUniformBlockResourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(samplerFactorsResourceLayout, _samplerFactorsUniformBlockBuffer)
            );

            var weightsAndOffsetsResourceLayoutDescription = uniformDescriptions[2][0];

            _weightsAndOffsetsDeviceBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription((uint)_downSamplerValues.WeightsAndOffsetsArraySize * DownSampleArrayComponent.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var weightsAndOffsetsResourceLayoutFactors = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    weightsAndOffsetsResourceLayoutDescription
                )
            );

            _weightsAndOffsetsResourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(weightsAndOffsetsResourceLayoutFactors, _weightsAndOffsetsDeviceBuffer)
            );
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestOverrideBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                            _shaderPackage.Description,
                                                                            new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm)));
        }
        public void Render(CommandList cl, float brightnessThreshold, GpuSurface source, GpuSurface target, ResizeSamplerType samplerType)
        {
            if (_sampleType == null || _sampleType != samplerType)
            {
                _sampleType = samplerType;
                LoadWeightsAndOffsets(cl);
            }

            UpdateSamplerUniformBuffer(cl, source, brightnessThreshold);

            cl.SetFramebuffer(target.Framebuffer);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, source.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(1, _samplerFactorsUniformBlockResourceSet);
            cl.SetGraphicsResourceSet(2, _weightsAndOffsetsResourceSet);
            cl.Draw(6);
        }

        private void LoadWeightsAndOffsets(CommandList cl)
        {
            cl.UpdateBuffer(_weightsAndOffsetsDeviceBuffer, 0, _downSamplerValues.ReturnWeightsAndOffsets((ResizeSamplerType)_sampleType));
        }

        private void UpdateSamplerUniformBuffer(CommandList cl, GpuSurface source, float brightnessThreshold)
        {
            var factors = new BloomDownSampleFactors
            {
                TexelSize = new Vector2(1.0f / (1.0f * source.Texture.Width), 1.0f / (1.0f * source.Texture.Height)),
                NumberOfSamples = _downSamplerValues.NumberOfSamples((ResizeSamplerType)_sampleType),
                BrightnessThreshold = brightnessThreshold
            };

            cl.UpdateBuffer(_samplerFactorsUniformBlockBuffer, 0, ref factors);
        }
    }
}