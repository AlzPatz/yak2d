using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class StartupPropertiesCache : IStartupPropertiesCache
    {
        public StartupConfig User { get; private set; }
        public InternalStartUpProperties Internal { get; private set; }

        public StartupPropertiesCache(IApplication application)
        {
            User = application?.Configure();
            Internal = new InternalStartUpProperties
            {
                PixelFormatForRenderingSurfaces = PixelFormat.B8_G8_R8_A8_UNorm, //Unlikely or unable to change. RGB vs SRGB to explore. Or just remove
                DefaultFpsTrackerUpdatePeriodInSeconds = 1.0f,
                DrawQueueInitialSizeNumberOfRequests = 1024,
                DrawQueueInitialSizeElementsPerRequestScalar = 4,
                DrawStageInitialSizeVertexBuffer = 4096,
                DrawStageInitialSizeIndexBuffer = 8192,
                DrawStageInitialMaxNumberOfLayersForDepthScaling = 10,
                DrawStageBatcherInitialNumberOfBatches = 128,
                DrawRequestPoolMinSize = 256,
                RenderCommandMinPoolSize = 64,
                DefaultWindowClearColourOnNoRenderCommandsQueued = RgbaFloat.Pink
            };
        }
    }
}