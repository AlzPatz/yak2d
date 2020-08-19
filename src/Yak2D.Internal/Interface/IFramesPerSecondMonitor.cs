namespace Yak2D.Internal
{
    public interface IFramesPerSecondMonitor
    {
        float UpdateFps { get; }
        float DrawFps { get; }
        void Update();
        void RegisterUpdateFrame();
        void RegisterDrawFrame();
        void SetCalculationUpdatePeriod(float period);
    }
}