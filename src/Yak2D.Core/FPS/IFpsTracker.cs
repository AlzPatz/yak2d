namespace Yak2D.Core
{
    public interface IFpsTracker
    {
        float FPS { get; }
        void SetCalculationUpdatePeriod(float period);
        void Reset();
        void RegisterFrame();
        void Update();
    }
}