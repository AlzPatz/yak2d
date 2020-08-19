using Yak2D.Internal;

namespace Yak2D.Core
{
    public class DebugAnalytics : IDebugAnalytics
    {
        public UpdatePeriod Updater_TimestepType { get; set; }
        public bool Updater_OverutilisedFlag { get; set; }
        public bool Updater_UnderutilisedFlag { get; set; }
        public double Updater_UpdateProcessingPercentage { get; set; }
        public double Updater_AverageFrameTime { get; set; }
        public double Updater_FrameTimeVariance { get; set; }
    }
}
