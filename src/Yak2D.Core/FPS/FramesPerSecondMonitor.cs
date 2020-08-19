using Yak2D.Internal;

namespace Yak2D.Core
{
    public class FramesPerSecondMonitor : IFramesPerSecondMonitor
    {
        private FpsTracker _updateFpsTracker;
        private FpsTracker _drawFpsTracker;

        public float UpdateFps => _updateFpsTracker.FPS;
        public float DrawFps => _drawFpsTracker.FPS;

        public FramesPerSecondMonitor(ITimerFactory timerFactory, IStartupPropertiesCache propertiesCache)
        {
            _updateFpsTracker = new FpsTracker(timerFactory.Create(), propertiesCache);
            _drawFpsTracker = new FpsTracker(timerFactory.Create(), propertiesCache);

            _updateFpsTracker.Reset();
            _drawFpsTracker.Reset();
        }

        public void RegisterUpdateFrame()
        {
            _updateFpsTracker.RegisterFrame();
        }

        public void RegisterDrawFrame()
        {
            _drawFpsTracker.RegisterFrame();
        }

        public void Update()
        {
            _updateFpsTracker.Update();
            _drawFpsTracker.Update();
        }

        public void SetCalculationUpdatePeriod(float period)
        {
            _updateFpsTracker.SetCalculationUpdatePeriod(period);
            _drawFpsTracker.SetCalculationUpdatePeriod(period);
        }
    }
}