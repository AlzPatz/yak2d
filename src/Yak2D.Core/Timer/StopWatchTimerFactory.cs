namespace Yak2D.Core
{
    public class StopWatchTimerFactory : ITimerFactory
    {
        public StopWatchTimerFactory() { }

        public ITimer Create()
        {
            return new StopWatchTimer();
        }
    }
}