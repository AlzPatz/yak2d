using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DistortionHeightRenderer : IDistortionHeightRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IGpuSurfaceManager _surfaceManager;

        private ShaderPackage _shaderPackage;
        private Pipeline _pipeline;

        public DistortionHeightRenderer(IFrameworkMessenger frameworkMessenger,
                                        IShaderLoader shaderLoader,
                                        IPipelineFactory pipelineFactory,
                                        IGpuSurfaceManager surfaceManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _surfaceManager = surfaceManager;

            Initialise();
        }

        private void Initialise()
        {
            CreateShaders();
            CreatePipelines();
        }


        public void ReInitialiseGpuResources()
        {
            Initialise();
        }

        private void CreateShaders()
        {
            var vertexLayout = new VertexLayoutDescription
            (
                56,
                0,
                new VertexElementDescription[] {
                    new VertexElementDescription("isWorld", VertexElementFormat.UInt1, VertexElementSemantic.TextureCoordinate),
                    new VertexElementDescription("TexuringType", VertexElementFormat.UInt1, VertexElementSemantic.TextureCoordinate),
                    new VertexElementDescription("Position", VertexElementFormat.Float3, VertexElementSemantic.Position),
                    new VertexElementDescription("Color", VertexElementFormat.Float4, VertexElementSemantic.Color),
                    new VertexElementDescription("TexCoord0", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                    new VertexElementDescription("TexCoord1", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                    new VertexElementDescription("TexWeight0", VertexElementFormat.Float1, VertexElementSemantic.TextureCoordinate)
                }
            );

            var uniformDescriptions = new ResourceLayoutElementDescription[3][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("WorldViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("texSampler0", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("tex0", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("texSampler1", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("tex1", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("DrawingVertex", AssetSourceEnum.Embedded, "DistortionHeightFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipelines()
        {
            //Different pipeline to DRAWING_RENDERER, ONE of only TWO differnces to drawing renderer, could pull out to base later?
            _pipeline = _pipelineFactory.CreateDistortionHeightMapPipeline(_shaderPackage.UniformResourceLayout,
                                                                            _shaderPackage.Description);
        }

        public void Render(CommandList cl, IDistortionStageModel stage, GpuSurface target, ICameraModel2D camera)
        {
            cl.SetPipeline(_pipeline);
            cl.SetFramebuffer(target.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Clear); // HERE DIFFERENT TO STANDARD DRAW ONLY (FUTURE PULL OUT TO BASE?)
            cl.SetVertexBuffer(0, stage.Buffers.VertexBuffer);
            cl.SetIndexBuffer(stage.Buffers.IndexBuffer, IndexFormat.UInt32); //Extract format and type 
            cl.SetGraphicsResourceSet(0, camera.ResourceSet);

            var batcher = stage.Batcher;

            for (var b = 0; b < batcher.NumberOfBatches; b++)
            {
                var batch = batcher.Pool[b];

                ResourceSet t0;
                if (batch.Texture0 == 0UL)
                {
                    t0 = _surfaceManager.SingleWhitePixel.ResourceSet_TexWrap;
                }
                else
                {
                    var retrieved = _surfaceManager.RetrieveSurface(batch.Texture0, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });

                    if (retrieved == target)
                    {
                        _frameworkMessenger.Report("Warning: A distortion stage is attempting to draw a surface onto itself. Aborting");
                        return;
                    }

                    t0 = retrieved == null ? _surfaceManager.SingleWhitePixel.ResourceSet_TexWrap :
                                            batch.TextureMode0 == TextureCoordinateMode.Mirror ?
                                            retrieved.ResourceSet_TexMirror : retrieved.ResourceSet_TexWrap;
                }
                cl.SetGraphicsResourceSet(1, t0);

                ResourceSet t1;
                if (batch.Texture1 == 0UL)
                {
                    t1 = _surfaceManager.SingleWhitePixel.ResourceSet_TexWrap;
                }
                else
                {
                    var retrieved = _surfaceManager.RetrieveSurface(batch.Texture1, new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });

                    if (retrieved == target)
                    {
                        _frameworkMessenger.Report("Warning: A distortion stage is attempting to draw a surface onto itself. Aborting");
                        return;
                    }

                    t1 = retrieved == null ? _surfaceManager.SingleWhitePixel.ResourceSet_TexWrap :
                                            batch.TextureMode1 == TextureCoordinateMode.Mirror ?
                                            retrieved.ResourceSet_TexMirror : retrieved.ResourceSet_TexWrap;
                }
                cl.SetGraphicsResourceSet(2, t1);

                cl.DrawIndexed((uint)batch.NumIndices, 1, (uint)batch.StartIndex, 0, 0);
            }
        }
    }
}