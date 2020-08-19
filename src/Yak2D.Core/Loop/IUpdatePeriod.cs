using System;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public interface IUpdatePeriod
    {
        void ProcessRequiredUpdates(double timeSinceLastUpdate, 
                                    LoopProperties loopProps, 
                                    IFramesPerSecondMonitor framesPerSecondMonitor,    
                                    Func<float, bool> update,
                                    ITimer loopTimer);
        void ProcessSingleUpdate(   double updatePeriod, 
                                    LoopProperties loopProps, 
                                    IFramesPerSecondMonitor framesPerSecondMonitor, 
                                    Func<float, bool> update,
                                    ITimer loopTimer);
        void MarkStartOfAnalysisPeriod(ITimer loopTimer);
        void AnalysePeriod(ITimer loopTimer, LoopProperties loopProps);
    }
}