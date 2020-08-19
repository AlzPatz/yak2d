namespace Yak2D
{
    /// <summary>
    /// Provides current per-second rates for update and draw/render iteration loops
    /// </summary>
    public interface IFps
    {
        /// <summary>
        /// Gets current UPDATE iteration per-second rate (sampled over configured time period)
        /// </summary>
        float UpdateFPS { get; }

        /// <summary>
        /// Gets current DRAW/RENDER iteration per-second rate (sampled over configured time period)
        /// </summary>
        float DrawFPS { get; }
    }
}