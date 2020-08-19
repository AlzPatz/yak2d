namespace Yak2D.Internal
{
    public interface IDebugAnalytics
    {
        bool Updater_OverutilisedFlag { get; set; }
        double Updater_UpdateProcessingPercentage { get; set; }
        bool Updater_UnderutilisedFlag { get; set; }
        UpdatePeriod Updater_TimestepType { get; set; }
        double Updater_AverageFrameTime { get; set; }
        double Updater_FrameTimeVariance { get; set; }
    }
}