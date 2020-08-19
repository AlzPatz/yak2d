namespace Yak2D.Graphics
{
    public interface IDownSamplerWeightsAndOffsets
    {
        int WeightsAndOffsetsArraySize { get; }
        int NumberOfSamples(ResizeSamplerType sampleType);
        DownSampleArrayComponent[] ReturnWeightsAndOffsets(ResizeSamplerType sampleType);
    }
}