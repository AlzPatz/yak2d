using System;
using Yak2D.Internal;

namespace Yak2D.Core
{
    /*
    Two base Update Approaches

    1. Update as quickly as possible, using last time step [ VARIABLE ]
    2. Update using fixed timesteps [ FIXED ]

    Sub-Options

    VARIABLE = FIXED -> Handling of Draw Timing vs. Last Update time
        1. Do fractional update to "catch up" just before running the draw method
        2. Send fractional time to draw method so movement can be interpolated by user if desired (to keep API constant, send 0 on fractional .. (or maybe measure tiny extra that even catch up would have..)


    FIXED HAS ADDITIONAL OPTIONS
        - Constant Fixed Updates (can mean compounding slow down if computer unable to process updates in the time it takes to do them)
        - Adaptive (with baseline smallest update if desired)

    Both can be monitored by measuring free time in draw to draw calls. If no idle time, then can decide to increase (double) interval. If too much idle time (50%) can half. 
    Some buffer on those percentages or atleast monitor them on the way smaller for a few frames (longer?)

    MUST handle use Vsync or not. Must enforce atleast one update before any draw (if no vsync and no one update then could always draw)

        public bool SyncToVerticalBlank { get; set; }
        public UpdatePeriod UpdatePeriodType { get; set; }
        public bool ProcessFractionalUpdatesBeforeDraw { get; set; }
        public float FixedOrSmallestUpdateTimeStepInSeconds { get; set; }
     */

    public class UpdatePeriodFactory : IUpdatePeriodFactory
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDebugAnalytics _debugAnalytics;

        public UpdatePeriodFactory(IFrameworkMessenger frameworkMessenger,
                                    IDebugAnalytics debugAnalytics)
        {
            _frameworkMessenger = frameworkMessenger;
            _debugAnalytics = debugAnalytics;
        }

        public IUpdatePeriod Create(UpdatePeriod updatePeriodType)
        {
            switch (updatePeriodType)
            {
                case UpdatePeriod.Fixed:
                    return new UpdatePeriodFixed(_frameworkMessenger, _debugAnalytics);
                case UpdatePeriod.Fixed_Adaptive:
                    return new UpdatePeriodFixedAdaptive(_frameworkMessenger, _debugAnalytics);
                case UpdatePeriod.Variable:
                    return new UpdatePeriodVariable(_frameworkMessenger, _debugAnalytics);
            }

            throw new Yak2DException("Invalid update period Enum -> shouldn't be possible :)");
        }
    }
}