namespace Yak2D.Internal
{
    public class LoopProperties
    {
        public UpdatePeriod UpdatePeriodType { get; set; }
        public bool ProcessFractionalUpdatesBeforeDraw { get; set; }
        public double SmallestFixedUpdateTimeStepInSeconds { get; set; }
        public double FixedUpdateTimeStepInSeconds { get; set; }
        public bool DoNotDrawFrameUnlessThereHasBeenOneUpdate { get; set; }
        public bool Running { get; set; }
        public double TimeOfLastUpdate { get; set; }
        public double TimeOfLastDraw { get; set; }
        public bool HasThereBeenAnUpdateSinceTheLastDraw { get; set; }
    }
}