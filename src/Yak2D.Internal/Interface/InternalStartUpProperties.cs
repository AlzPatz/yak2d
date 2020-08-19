using Veldrid;

namespace Yak2D.Internal
{
    public class InternalStartUpProperties
    {
        public PixelFormat PixelFormatForRenderingSurfaces { get; set; }
        public float DefaultFpsTrackerUpdatePeriodInSeconds { get; set; }
        public int DrawQueueInitialSizeNumberOfRequests { get; set; }
        public int DrawQueueInitialSizeElementsPerRequestScalar { get; set; }
        public int DrawStageInitialSizeVertexBuffer { get; set; }
        public int DrawStageInitialSizeIndexBuffer { get; set; }
        public int DrawStageInitialMaxNumberOfLayersForDepthScaling { get; set; }
        public int DrawStageBatcherInitialNumberOfBatches { get; set; }
        public int DrawRequestPoolMinSize { get; set; }
        public int RenderCommandMinPoolSize { get; set; }
        public RgbaFloat DefaultWindowClearColourOnNoRenderCommandsQueued { get; set; }
    }
}