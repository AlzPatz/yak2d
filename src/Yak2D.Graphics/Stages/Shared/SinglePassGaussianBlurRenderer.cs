using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class SinglePassGaussianBlurRenderer : ISinglePassGaussianBlurRenderer
    {
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IGaussianBlurWeightsAndOffsetsCache _gaussianWeightsAndOffsetsCache;
        private ShaderPackage _shaderPackage;
        private DeviceBuffer _gaussianFactorsUniformBlockBuffer;
        private ResourceSet _gaussianFactorsUniformBlockResourceSet;
        private DeviceBuffer _weightsAndOffsetsDeviceBuffer;
        private ResourceSet _weightsAndOffsetsResourceSet;
        private Pipeline _pipeline;

        private Vector2 _currentBlurDirection;
        private int _currentNumSamplesIncludingCentre;

        public SinglePassGaussianBlurRenderer(ISystemComponents systemComponents,
                                                                                IShaderLoader shaderLoader,
                                                                                IPipelineFactory pipelineFactory,
                                                                                IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                                                                IGaussianBlurWeightsAndOffsetsCache gaussianWeightsAndOffsetsCache)
        {
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _gaussianWeightsAndOffsetsCache = gaussianWeightsAndOffsetsCache;

            Initialise();
        }

        private void Initialise()
        {
            CreateShadersAndFactorsUniform();
            CreatePipeline();

            _currentBlurDirection = Vector2.UnitX;
            _currentNumSamplesIncludingCentre = -1;
        }

        public void DisposeOfGpuResources()
        {
            _pipeline?.Dispose();
            _weightsAndOffsetsDeviceBuffer?.Dispose();
            _weightsAndOffsetsResourceSet?.Dispose();
            _gaussianFactorsUniformBlockBuffer?.Dispose();
            _gaussianFactorsUniformBlockResourceSet?.Dispose();
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
                new ResourceLayoutElementDescription("LinearSampledGaussianUniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("WeightsAndOffsets", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded, "LinearFilterGaussianBlur1DFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);

            var gaussianUniformBlockResourceLayoutDescription = uniformDescriptions[1][0];

            _gaussianFactorsUniformBlockBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription(GaussianBlurFactors.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var gaussianFactorsResourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    gaussianUniformBlockResourceLayoutDescription
                )
            );

            _gaussianFactorsUniformBlockResourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(gaussianFactorsResourceLayout, _gaussianFactorsUniformBlockBuffer)
            );

            var weightsAndOffsetsResourceLayoutDescription = uniformDescriptions[2][0];

            _weightsAndOffsetsDeviceBuffer = _systemComponents.Factory.CreateBuffer(new BufferDescription((uint)_gaussianWeightsAndOffsetsCache.WeightsAndOffsetsArraySize * GaussianBlurArrayComponent.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            var weightsAndOffsetsResourceLayoutFactors = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    weightsAndOffsetsResourceLayoutDescription
                )
            );

            _weightsAndOffsetsResourceSet = _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(weightsAndOffsetsResourceLayoutFactors, _weightsAndOffsetsDeviceBuffer)
            );
        }

        private void UpdateGaussianUniformBuffer(CommandList cl, Vector2 texelShiftSize)
        {
            var gaussianUniforms = new GaussianBlurFactors
            {
                TexelShiftSize = texelShiftSize,
                NumberOfSamples = _currentNumSamplesIncludingCentre - 1,
                Pad0 = 0.0f,
                Pad1 = Vector4.Zero
            };

            cl.UpdateBuffer(_gaussianFactorsUniformBlockBuffer, 0, ref gaussianUniforms);
        }

        private void UpdateWeightsAndOffsetsBuffer(CommandList cl)
        {
            cl.UpdateBuffer(_weightsAndOffsetsDeviceBuffer, 0, _gaussianWeightsAndOffsetsCache.ReturnWeightsAndOffsets(_currentNumSamplesIncludingCentre - 1));
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestOverrideBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                            _shaderPackage.Description,
                                                                            new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm)));
        }

        public void Render(CommandList cl, Vector2 texelShiftSize, int numberSamplesPerSideNotIncludingCentre, Vector2 blurDirectionUnit, GpuSurface source, GpuSurface target)
        {
            var num = numberSamplesPerSideNotIncludingCentre + 1;

            if (num != _currentNumSamplesIncludingCentre)
            {
                _currentNumSamplesIncludingCentre = num;
                _currentBlurDirection = blurDirectionUnit;
                UpdateWeightsAndOffsetsBuffer(cl);
            }

            UpdateGaussianUniformBuffer(cl, texelShiftSize);

            cl.SetFramebuffer(target.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Clear);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, source.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(1, _gaussianFactorsUniformBlockResourceSet);
            cl.SetGraphicsResourceSet(2, _weightsAndOffsetsResourceSet);
            cl.Draw(6);
        }
    }
}