using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawStageRenderer : IDrawStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _components;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IGpuSurfaceManager _surfaceManager;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;

        public DrawStageRenderer(IFrameworkMessenger frameworkMessenger,
                                    ISystemComponents components,
                                    IShaderLoader shaderLoader,
                                    IPipelineFactory pipelineFactory,
                                    IGpuSurfaceManager surfaceManager,
                                    IViewportManager viewportManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _components = components;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _surfaceManager = surfaceManager;
            _viewportManager = viewportManager;

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

            //Thes resource layouts will match those that are used to create the resouce sets for the camera matrices and textures.
            //It may be useful to feed a fixed version to both higher up. Also, I am assuming the string label means nothing as
            //For textures they can be either 0 or 1.. anyway, i'm sure i get headaches later when using different graphics api backends

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

            _shaderPackage = _shaderLoader.CreateShaderPackage("DrawingVertex", AssetSourceEnum.Embedded, "DrawingFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipelines()
        {
            _pipelineFactory.CreateAllDrawingPipelines(_shaderPackage.UniformResourceLayout,
                                                                 _shaderPackage.Description,
                                                                 _components.Device.SwapchainFramebuffer.OutputDescription);
        }

        public void Render(CommandList cl, IDrawStageModel stage, GpuSurface surface, ICameraModel2D camera)
        {
            if (cl == null || stage == null || surface == null || camera == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding DrawStage Renderer null inputs, aborting");
                return;
            }

            cl.SetFramebuffer(surface.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, stage.Buffers.VertexBuffer);
            cl.SetIndexBuffer(stage.Buffers.IndexBuffer, IndexFormat.UInt32); //Extract format and type 
            cl.SetPipeline(_pipelineFactory.ReturnDrawingPipeline(stage.BlendState));
            cl.SetGraphicsResourceSet(0, camera.ResourceSet);

            //When drawing commands are queued need to check and ensure the same texture is not used for both tex inputs
            //We also need to trigger the sort somewhere earlier than this and ensure if already sorted into batches dont do it
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
                    var retrieved = _surfaceManager.RetrieveSurface(batch.Texture0,new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });

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
                    var retrieved = _surfaceManager.RetrieveSurface(batch.Texture1,new GpuSurfaceType[] { GpuSurfaceType.SwapChainOutput, GpuSurfaceType.Internal });

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