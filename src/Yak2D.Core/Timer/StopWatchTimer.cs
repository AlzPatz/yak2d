namespace Yak2D.Core
{
    public class StopWatchTimer : ITimer
    {
        public double Seconds { get { return _stopWatch.Elapsed.TotalSeconds; } }

        private System.Diagnostics.Stopwatch _stopWatch;

        public StopWatchTimer()
        {
            _stopWatch = new System.Diagnostics.Stopwatch();
        }

        public void Reset()
        {
            _stopWatch.Reset();
        }

        public void Start()
        {
            _stopWatch.Start();
        }
    }
}