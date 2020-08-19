using Yak2D.Internal;

namespace Yak2D.Core
{
    public class Fps : IFps
    {
        public float UpdateFPS => _fpsMonitor.UpdateFps;
        public float DrawFPS => _fpsMonitor.DrawFps;

        private IFramesPerSecondMonitor _fpsMonitor;

        public Fps(IFramesPerSecondMonitor fpsMonitor)
        {
            _fpsMonitor = fpsMonitor;
        }
    }
}