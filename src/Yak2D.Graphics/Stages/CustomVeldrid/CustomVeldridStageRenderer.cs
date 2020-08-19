using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CustomVeldridStageRenderer : ICustomVeldridStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly ISystemComponents _systemComponents;

        public CustomVeldridStageRenderer(IFrameworkMessenger frameworkMessenger,
                                    IGpuSurfaceManager gpuSurfaceManager,
                                    ISystemComponents systemComponents
                                    )
        {
            _frameworkMessenger = frameworkMessenger;
            _gpuSurfaceManager = gpuSurfaceManager;
            _systemComponents = systemComponents;
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

        public void Render(CommandList cl, ICustomVeldridStageModel stage, GpuSurface t0, GpuSurface t1, GpuSurface t2, GpuSurface t3, GpuSurface target)
        {
            if (cl == null || stage == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding a Custom Veldrid Stage Renderer null inputs, aborting");
                return;
            }
            
            //Auto set render target if not done in user code
            if (target != null)
            {
                cl.SetFramebuffer(target.Framebuffer);
            }

            var tex0 = t0 == null ? _gpuSurfaceManager.SingleWhitePixel.ResourceSet_TexWrap : t0.ResourceSet_TexWrap;
            var tex1 = t1 == null ? _gpuSurfaceManager.SingleWhitePixel.ResourceSet_TexWrap : t1.ResourceSet_TexWrap;
            var tex2 = t2 == null ? _gpuSurfaceManager.SingleWhitePixel.ResourceSet_TexWrap : t2.ResourceSet_TexWrap;
            var tex3 = t3 == null ? _gpuSurfaceManager.SingleWhitePixel.ResourceSet_TexWrap : t3.ResourceSet_TexWrap;

            stage.CustomStage.Render(cl, _systemComponents.Device.RawVeldridDevice, tex0, tex1, tex2, tex3, target == null ? null : target.Framebuffer);
        }
    }
}