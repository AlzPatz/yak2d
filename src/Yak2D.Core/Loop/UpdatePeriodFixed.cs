using System;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class UpdatePeriodFixed : IUpdatePeriod
    {
        private const double OVER_UTILISATION_THRESHOLD = 0.95;
        private const int OVERUTILISED_FRAME_SUM_OVERFLOW_FOR_FLAGGING = 256;

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDebugAnalytics _debugAnalytics;
        
        private double _updatingTimeCount;
        private double _analysisPeriodTimeStart;
        private int _overutilisedFrameCount = 0;

        public UpdatePeriodFixed(IFrameworkMessenger frameworkMessenger, IDebugAnalytics debugAnalytics)
        {
            _frameworkMessenger = frameworkMessenger;
            _debugAnalytics = debugAnalytics;
        }

        public void MarkStartOfAnalysisPeriod(ITimer loopTimer)
        {
            _updatingTimeCount = 0.0;
            _analysisPeriodTimeStart = loopTimer.Seconds;
        }

        public void AnalysePeriod(ITimer loopTimer, LoopProperties loopProps)
        {
            var timeDelta = loopTimer.Seconds - _analysisPeriodTimeStart;

            var percentageProcessingUpdates = _updatingTimeCount / timeDelta;

            if (percentageProcessingUpdates > OVER_UTILISATION_THRESHOLD)
            {
                _overutilisedFrameCount++;
            }
            else
            {
                _overutilisedFrameCount--;
                if (_overutilisedFrameCount < 0)
                {
                    _overutilisedFrameCount = 0;
                }
            }

            _debugAnalytics.Updater_TimestepType = UpdatePeriod.Fixed;
            _debugAnalytics.Updater_OverutilisedFlag = percentageProcessingUpdates > OVERUTILISED_FRAME_SUM_OVERFLOW_FOR_FLAGGING;
            _debugAnalytics.Updater_UpdateProcessingPercentage = percentageProcessingUpdates;
            _debugAnalytics.Updater_AverageFrameTime = loopProps.FixedUpdateTimeStepInSeconds;
            _debugAnalytics.Updater_FrameTimeVariance = 0.0f;
        }

        public void ProcessRequiredUpdates(double timeSinceLastUpdate, LoopProperties loopProps, IFramesPerSecondMonitor framesPerSecondMonitor, Func<float, bool> update, ITimer loopTimer)
        {
            var period = timeSinceLastUpdate;

            while (period > loopProps.FixedUpdateTimeStepInSeconds && loopProps.Running)
            {
                ProcessSingleUpdate(loopProps.FixedUpdateTimeStepInSeconds, loopProps, framesPerSecondMonitor, update, loopTimer);
                period -= loopProps.FixedUpdateTimeStepInSeconds;
                loopProps.HasThereBeenAnUpdateSinceTheLastDraw = true;
            }
        }

        public void ProcessSingleUpdate(double updatePeriod, LoopProperties loopProps, IFramesPerSecondMonitor framesPerSecondMonitor, Func<float, bool> update, ITimer loopTimer)
        {
            var startTime = loopTimer.Seconds;
            loopProps.Running = update((float)updatePeriod);
            var timeDelta = loopTimer.Seconds - startTime;

            _updatingTimeCount += timeDelta;

            loopProps.TimeOfLastUpdate += updatePeriod;
            framesPerSecondMonitor.RegisterUpdateFrame();
       }
    }
}