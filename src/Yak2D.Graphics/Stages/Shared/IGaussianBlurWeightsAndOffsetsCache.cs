namespace Yak2D.Graphics
{
    public interface IGaussianBlurWeightsAndOffsetsCache
    {
        int WeightsAndOffsetsArraySize { get; }
        int MaxNumberOfSamplesToRequestAwayFromCentre { get; }
        GaussianBlurArrayComponent[] ReturnWeightsAndOffsets(int numSamplesTakenFromCentre);
    }
}