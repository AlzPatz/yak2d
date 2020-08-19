using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DistortionStageRenderer : IDistortionStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDistortionHeightRenderer _heightRenderer;
        private readonly IDistortionGraidentShiftRenderer _gradientShiftRenderer;
        private readonly IDistortionRenderer _distortionRenderer;

        public DistortionStageRenderer(
                                    IFrameworkMessenger frameworkMessenger,
                                    IDistortionHeightRenderer heightRenderer,
                                    IDistortionGraidentShiftRenderer gradientShiftRenderer,
                                    IDistortionRenderer distortionRenderer)
        {
            _frameworkMessenger = frameworkMessenger;
            _heightRenderer = heightRenderer;
            _gradientShiftRenderer = gradientShiftRenderer;
            _distortionRenderer = distortionRenderer;
        }

        public void Render(CommandList cl, IDistortionStageModel stage, GpuSurface source, GpuSurface target, ICameraModel2D camera)
        {
            if (cl == null || stage == null || source == null || target == null || camera == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Distortion Stage Renderer null inputs, aborting");
                return;
            }

            _heightRenderer.Render(cl, stage, stage.HeightMapSurface, camera);
            _gradientShiftRenderer.Render(cl, stage, stage.HeightMapSurface, stage.GradientShiftSurface);
            _distortionRenderer.Render(cl, stage, source, stage.GradientShiftSurface, target);
        }
    }
}