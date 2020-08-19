using System;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class UpdatePeriodFixedAdaptive : IUpdatePeriod
    {
        private const double OVER_UTILISATION_THRESHOLD = 0.95;
        private const double UNDER_UTILISATION_THRESHOLD = 0.40;

        private const int CONSECUTIVE_SLOW_FRAMES_REQUIRED_FOR_DOUBLING = 8;
        private const int CONSECUTIVE_FAST_FRAMES_REQUIRED_FOR_HALFING = 120;

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDebugAnalytics _debugAnalytics;

        private double _updatingTimeCount;
        private double _analysisPeriodTimeStart;

        private int _overutilisedFrameCount = 0;
        private int _underutilisedFrameCount = 0;

        public UpdatePeriodFixedAdaptive(IFrameworkMessenger frameworkMessenger, IDebugAnalytics debugAnalytics)
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

                if (_overutilisedFrameCount > CONSECUTIVE_SLOW_FRAMES_REQUIRED_FOR_DOUBLING)
                {
                    IncreaseFixedTimeStep(loopProps);
                    _overutilisedFrameCount = 0;
                }
            }
            else
            {
                _overutilisedFrameCount = 0;
            }

            if (percentageProcessingUpdates < UNDER_UTILISATION_THRESHOLD)
            {
                _underutilisedFrameCount++;
                if (_underutilisedFrameCount > CONSECUTIVE_FAST_FRAMES_REQUIRED_FOR_HALFING)
                {
                    DecreaseFixedTimeStep(loopProps);
                    _underutilisedFrameCount = 0;
                }
            }
            else
            {
                _underutilisedFrameCount = 0;
            }

            _debugAnalytics.Updater_TimestepType = UpdatePeriod.Fixed_Adaptive;
            _debugAnalytics.Updater_OverutilisedFlag = percentageProcessingUpdates > OVER_UTILISATION_THRESHOLD;
            _debugAnalytics.Updater_UnderutilisedFlag = percentageProcessingUpdates > UNDER_UTILISATION_THRESHOLD;
            _debugAnalytics.Updater_UpdateProcessingPercentage = percentageProcessingUpdates;
            _debugAnalytics.Updater_AverageFrameTime = loopProps.FixedUpdateTimeStepInSeconds;
            _debugAnalytics.Updater_FrameTimeVariance = 0.0f;
        }

        private void IncreaseFixedTimeStep(LoopProperties loopProps)
        {
            loopProps.FixedUpdateTimeStepInSeconds *= 2.0;
        }

        private void DecreaseFixedTimeStep(LoopProperties loopProps)
        {
            var halfed = 0.5 * loopProps.FixedUpdateTimeStepInSeconds;

            if(halfed >= loopProps.SmallestFixedUpdateTimeStepInSeconds)
            {
                loopProps.FixedUpdateTimeStepInSeconds = halfed;
            }
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