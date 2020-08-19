using System;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class UpdatePeriodVariable : IUpdatePeriod
    {
        private const int NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME = 60;

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDebugAnalytics _debugAnalytics;

        private double[] _times;
        private int _index;
        private int _valuesSinceLastAnalysis;        

        public UpdatePeriodVariable(IFrameworkMessenger frameworkMessenger, IDebugAnalytics debugAnalytics)
        {
            _frameworkMessenger = frameworkMessenger;
            _debugAnalytics = debugAnalytics;

            _times = new double[NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME];

            _valuesSinceLastAnalysis = 0;

            _index = 0;
        }

        public void AnalysePeriod(ITimer _loopTimer, LoopProperties _loopProps)
        {
            if (_valuesSinceLastAnalysis >= NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME)
            {
                var average = 0.0;
                for (var n = 0; n < NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME; n++)
                {
                    average += _times[n];
                }

                average *= 1.0 / (1.0 * NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME);

                var variance = 0.0;
                for (var n = 0; n < NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME; n++)
                {
                    var delta = _times[n] - average;
                    variance += delta * delta;
                }

                variance /= 1.0 * NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME;

                _valuesSinceLastAnalysis = 0;

                _debugAnalytics.Updater_AverageFrameTime = average;
                _debugAnalytics.Updater_FrameTimeVariance = variance;
            }

            _debugAnalytics.Updater_TimestepType = UpdatePeriod.Variable;
            _debugAnalytics.Updater_OverutilisedFlag = false;
            _debugAnalytics.Updater_UnderutilisedFlag = false;
            _debugAnalytics.Updater_UpdateProcessingPercentage = 1.0f;
        }

        public void MarkStartOfAnalysisPeriod(ITimer _loopTimer)
        {
            //No action required for this update period type
        }

        public void ProcessRequiredUpdates(double timeSinceLastUpdate, LoopProperties loopProps, IFramesPerSecondMonitor framesPerSecondMonitor, Func<float, bool> update, ITimer loopTimer)
        {
            if (timeSinceLastUpdate < loopProps.SmallestFixedUpdateTimeStepInSeconds)
            {
                return;
            }

            ProcessSingleUpdate(timeSinceLastUpdate, loopProps, framesPerSecondMonitor, update, loopTimer);
            loopProps.HasThereBeenAnUpdateSinceTheLastDraw = true;
        }

        public void ProcessSingleUpdate(double updatePeriod, LoopProperties loopProps, IFramesPerSecondMonitor framesPerSecondMonitor, Func<float, bool> update, ITimer loopTimer)
        {
            loopProps.Running = update((float)updatePeriod);
            loopProps.TimeOfLastUpdate += updatePeriod;
            framesPerSecondMonitor.RegisterUpdateFrame();

            AddPeriodToBuffer(updatePeriod);
        }

        private void AddPeriodToBuffer(double period)
        {
            _times[_index] = period;
            _index++;
            if(_index == NUMBER_OF_UPDATES_TO_SMOOTH_FRAMETIME)
            {
                _index = 0;
            }
            _valuesSinceLastAnalysis++;
        }
    }
}