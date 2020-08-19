using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class MeshRenderStageRenderer : IMeshRenderStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private Pipeline _pipeline;

        public MeshRenderStageRenderer(IFrameworkMessenger frameworkMessenger,
                                    ISystemComponents systemComponents,
                                    IShaderLoader shaderLoader,
                                    IPipelineFactory pipelineFactory,
                                    IViewportManager viewportManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _viewportManager = viewportManager;

            Initialise();
        }

        private void Initialise()
        {
            CreateShaders();
            CreatePipeline();
        }

        private void CreateShaders()
        {
            var vertexLayout = new VertexLayoutDescription
            (
                32,
                0,
                new VertexElementDescription[] {
                    new VertexElementDescription("VertPosition", VertexElementFormat.Float3, VertexElementSemantic.Position),
                    new VertexElementDescription("VertNormal", VertexElementFormat.Float3, VertexElementSemantic.Normal),
                    new VertexElementDescription("VertTexCoord", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate)
                }
            );

            var uniformDescriptions = new ResourceLayoutElementDescription[5][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("VertexUniforms", ResourceKind.UniformBuffer, ShaderStages.Vertex)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("FragUniforms", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[3] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("LightProperties", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[4] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("LightsUniformBlock", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex3D", AssetSourceEnum.Embedded,"MeshFragment", AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateDepthTestOverrideBlend(_shaderPackage.UniformResourceLayout,
                                                               _shaderPackage.Description,
                                                               _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
        }

        public void Render(CommandList cl, IMeshRenderStageModel stage, GpuSurface source, GpuSurface surface, ICameraModel3D camera)
        {
            if (cl == null || stage == null || source == null || surface == null || camera == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Mesh Stage Renderer null inputs, aborting");
                return;
            }

            cl.SetFramebuffer(surface.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.ClearDepthStencil(1.0f); 
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, camera.WvpResource);
            cl.SetGraphicsResourceSet(1, camera.PositionResource);
            cl.SetGraphicsResourceSet(2, source.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(3, stage.LightPropertiesResource);
            cl.SetGraphicsResourceSet(4, stage.LightsResource);
            cl.SetVertexBuffer(0, stage.MeshVertexBuffer);
            cl.Draw(stage.MeshNumberVertices);
        }
        
        public void ReInitialiseGpuResources()
        {
            Initialise();
        }
    }
}