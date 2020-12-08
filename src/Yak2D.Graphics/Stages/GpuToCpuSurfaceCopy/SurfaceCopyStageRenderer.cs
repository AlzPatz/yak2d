using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class SurfaceCopyStageRenderer : ISurfaceCopyStageRenderer
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;

        public SurfaceCopyStageRenderer(IFrameworkMessenger frameworkMessenger,
                                            ISystemComponents systemComponents,
                                            IGpuSurfaceManager gpuSurfaceManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _gpuSurfaceManager = gpuSurfaceManager;
        }

        public void Render(CommandList cl, ISurfaceCopyStageModel stage, GpuSurface source)
        {
            if (cl == null || stage == null || source == null)
            {
                _frameworkMessenger.Report("Warning: you are feeding the Surface Copy Stage Renderer null inputs, aborting");
                return;
            }

            //Ensure the staging texture is of the correct size
            var stagingTexture = _gpuSurfaceManager.RetrieveSurface(stage.StagingTextureId);

            var doStagingTexturePropertiesMatch = source.Texture.Width == stagingTexture.Texture.Width &&
                                                    source.Texture.Height == stagingTexture.Texture.Height;

            var pixelFormatOfSource = source.Texture.Format;

            if (pixelFormatOfSource != stagingTexture.Texture.Format)
            {
                doStagingTexturePropertiesMatch = false;
            }

            if (!doStagingTexturePropertiesMatch)
            {
                stage.SetPixelFormatAndCreateStagingTextureAndDataArray(source.Texture.Width,
                                                                        source.Texture.Height,
                                                                        TexturePixelFormatConverter.ConvertVeldridToYak(pixelFormatOfSource));
                stagingTexture = _gpuSurfaceManager.RetrieveSurface(stage.StagingTextureId);
            }

            cl.CopyTexture(source.Texture, stagingTexture.Texture);
        }
    }
}