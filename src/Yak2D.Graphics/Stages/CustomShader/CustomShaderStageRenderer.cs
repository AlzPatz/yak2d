using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CustomShaderStageRenderer : ICustomShaderStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IViewportManager _viewportManager;
        private readonly IFullNdcSpaceQuadVertexBuffer _ndcQuadVertexBuffer;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;

        public CustomShaderStageRenderer(IFrameworkMessenger frameworkMessenger,
                                    IViewportManager viewportManager,
                                    IFullNdcSpaceQuadVertexBuffer ndcQuadVertexBuffer,
                                    IGpuSurfaceManager gpuSurfaceManager
                                    )
        {
            _frameworkMessenger = frameworkMessenger;
            _viewportManager = viewportManager;
            _ndcQuadVertexBuffer = ndcQuadVertexBuffer;
            _gpuSurfaceManager = gpuSurfaceManager;

            Initialise();
        }

        private void Initialise()
        {

        }

        public void DisposeOfGpuResources()
        {
            
        }

        public void ReInitialiseGpuResources()
        {
            DisposeOfGpuResources();
            Initialise();
        }

        public void Render(CommandList cl, ICustomShaderStageModel stage, GpuSurface t0, GpuSurface t1, GpuSurface t2, GpuSurface t3, GpuSurface target)
        {
            if (cl == null || stage == null || target == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding a Custom Effect Stage Renderer null inputs, aborting");
                return;
            }

            cl.SetPipeline(stage.Pipeline);
            cl.SetFramebuffer(target.Framebuffer);
            _viewportManager.ConfigureViewportForActiveFramebuffer(cl);
            cl.SetVertexBuffer(0, _ndcQuadVertexBuffer.Buffer);

            var assignedTextureCount = 0;
            var numUniforms = stage.NumberUserUniforms;
            for (var n = 0; n < numUniforms; n++)
            {
                if (stage.UserUniformType(n) == ShaderUniformType.Texture)
                {
                    if (assignedTextureCount < 4)
                    {
                        GpuSurface surface = null;
                        switch (assignedTextureCount)
                        {
                            case 0:
                                surface = t0;
                                break;
                            case 1:
                                surface = t1;
                                break;
                            case 2:
                                surface = t2;
                                break;
                            case 3:
                                surface = t3;
                                break;
                        }
                        var resourceSet = surface == null ? _gpuSurfaceManager.SingleWhitePixel.ResourceSet_TexWrap : surface.ResourceSet_TexWrap;

                        cl.SetGraphicsResourceSet((uint)n, resourceSet);

                        assignedTextureCount++;
                    }
                    else
                    {
                        _frameworkMessenger.Report("Custom shader requires more than 4 textures. Should not have reached this stage...");
                    }
                }
                else
                {
                    var resourceSet = stage.UserUniformResourceSet(n);
                    cl.SetGraphicsResourceSet((uint)n, resourceSet);
                }
            }
            cl.Draw(6);
        }
    }
}