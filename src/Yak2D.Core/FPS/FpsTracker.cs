using Yak2D.Internal;

namespace Yak2D.Core
{
    public class FpsTracker : IFpsTracker
    {
        public float FPS { get { return _fps; } }

        private ITimer _timer;
        private float _fps;
        private double _periodStartTime;
        private float _updatePeriod;
        private int _updateCount;

        public FpsTracker(ITimer timer, IStartupPropertiesCache propertiesCache)
        {
            _timer = timer;
            _updatePeriod = propertiesCache.Internal.DefaultFpsTrackerUpdatePeriodInSeconds;
            Reset();
        }

        public void Reset()
        {
            _fps = -1.0f;
            _timer.Reset();
            ResetCounters();
            _timer.Start();
        }

        private void ResetCounters()
        {
            _updateCount = 0;
            _periodStartTime = _timer.Seconds;
        }

        public void RegisterFrame()
        {
            _updateCount++;
        }

        public void SetCalculationUpdatePeriod(float period)
        {
            _updatePeriod = period > 0.0f ? period : 1.0f;
        }

        public void Update()
        {
            var delta = _timer.Seconds - _periodStartTime;
            if (delta >= _updatePeriod)
            {
                _fps = (float)((1.0f * _updateCount) / delta);

                ResetCounters();
            }
        }
    }
}