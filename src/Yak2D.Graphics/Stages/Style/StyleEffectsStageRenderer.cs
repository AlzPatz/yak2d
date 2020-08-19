using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class StyleEffectsStageRenderer : IStyleEffectsStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IShaderLoader _shaderLoader;
        private readonly IPipelineFactory _pipelineFactory;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly IViewportManager _viewportManager;

        private ShaderPackage _shaderPackage;
        private Veldrid.Pipeline _pipeline;

        public StyleEffectsStageRenderer(IFrameworkMessenger frameworkMessenger,
                                                                        ISystemComponents systemComponents,
                                                                        IShaderLoader shaderLoader,
                                                                        IPipelineFactory pipelineFactory,
                                                                        IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                                                        IGpuSurfaceManager gpuSurfaceManager,
                                                                        IViewportManager viewportManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _shaderLoader = shaderLoader;
            _pipelineFactory = pipelineFactory;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _gpuSurfaceManager = gpuSurfaceManager;
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

            var uniformDescriptions = new ResourceLayoutElementDescription[8][];

            uniformDescriptions[0] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler_Source", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture_Source", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[1] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler_Noise", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture_Noise", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[2] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("Sampler_ScanlineMask", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Texture_ScanlineMask", ResourceKind.Sampler, ShaderStages.Fragment)
            };

            uniformDescriptions[3] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("PixellateFactors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[4] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("EdgeDetectionFactors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[5] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("StaticFactors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[6] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("OldMovieFactors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            uniformDescriptions[7] = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("CrtEffectFactors", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            };

            _shaderPackage = _shaderLoader.CreateShaderPackage("Vertex2D", AssetSourceEnum.Embedded,"StyleFragment",AssetSourceEnum.Embedded, vertexLayout, uniformDescriptions);
        }

        private void CreatePipeline()
        {
            _pipeline = _pipelineFactory.CreateNonDepthTestOverrideBlendPipeline(_shaderPackage.UniformResourceLayout,
                                                                                _shaderPackage.Description,
                                                                                _systemComponents.Device.SwapchainFramebuffer.OutputDescription);
        }

        public void Render(CommandList cl, IStyleEffectsStageModel stage, GpuSurface source, GpuSurface target)
        {
            if (cl == null || stage == null || source == null || target == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Style Effect Stage Renderer null inputs, aborting");
                return;
            }

            //Updated every time as holds the shared TexelSize
            var factors = new PixellateFactors
            {
                PixAmount = stage.PixellateCurrent.Intensity,
                NumXDivisions = stage.PixellateCurrent.NumXDivisions,
                NumYDivisions = stage.PixellateCurrent.NumYDivisions,
                Pad0 = 0,
                TexelSize = new Vector2(1.0f / (1.0f * target.Framebuffer.Width), 1.0f / (1.0f * target.Framebuffer.Height)),
                Pad1 = Vector2.Zero
            };

            _systemComponents.Device.UpdateBuffer(stage.PixellateBuffer, 0, ref factors);

            cl.SetFramebuffer(target.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, source.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(1, _gpuSurfaceManager.Noise.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(2, _gpuSurfaceManager.CrtShadowMask.ResourceSet_TexWrap);
            cl.SetGraphicsResourceSet(3, stage.PixellateResourceSet);
            cl.SetGraphicsResourceSet(4, stage.EdgeDetectionResourceSet);
            cl.SetGraphicsResourceSet(5, stage.StaticResourceSet);
            cl.SetGraphicsResourceSet(6, stage.OldMovieResourceSet);
            cl.SetGraphicsResourceSet(7, stage.CrtEffectResourceSet);
            cl.Draw(6);
        }
    }
}