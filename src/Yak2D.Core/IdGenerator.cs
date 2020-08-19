using Yak2D.Internal;

namespace Yak2D.Core
{
    public class IdGenerator : IIdGenerator
    {
        private IFrameworkMessenger _frameworkMessenger;

        private ulong _idTracker;

        private const ulong initValue = (2 << 15) - 1; //Do not start at zero to allow lower numbers to reference blank Ids or Errors. -1 as we increment before returning an id and i want to start at 2^16.

        public IdGenerator(IFrameworkMessenger frameworkMessenger)
        {
            _frameworkMessenger = frameworkMessenger;

            _idTracker = initValue;
        }

        public ulong New()
        {
            if (_idTracker == ulong.MaxValue)
            {
                _frameworkMessenger.Report("Upcoming Integer64 overflow - wow, most likely we have a bug, or perhaps this system is running on it's own for billions of years after an apocalypse, or perhaps an advanced entity is running it super fast. if the latter, hello! I don't think we need to seriously handle the upcoming error... but let's wrap the count for good order :)");
                _idTracker = initValue;
            }

            _idTracker++;

            return _idTracker;
        }
    }
}