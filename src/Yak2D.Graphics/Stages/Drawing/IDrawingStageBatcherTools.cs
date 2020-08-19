using System.Collections.Generic;

namespace Yak2D.Graphics
{
    public interface IDrawStageBatcherTools
    {
        DrawingBatch[] CreatePoolOfSize(int size, bool copyExisting, DrawingBatch[] existing = null);
        List<QueueWrap> CombinedList(QueueWrap first, List<QueueWrap> others);
        bool AreAllQueuesInactive(ref int numQueues, bool[] activeQueues);
        int FindLowestLayer(ref int numQueues, QueueWrap[] wraps, bool[] active, int[] nextRequest);
        void IdentifyWhichQueuesAreActiveInTheCurrentLayer(ref int numQueues, QueueWrap[] wraps, int[] nextRequest, int layer, bool[] activeInLayer, bool[] activeOverAllLayers);
        int FindDeepestQueueAtLowestLayer(ref int numQueues, QueueWrap[] wraps, int[] nextRequest, int lowestLayer, bool[] activeInCurrentLayer);
        bool GenerateTheNextBatch(DrawingBatch[] pool, int queue, int batchIndex, ref int numQueues, QueueWrap[] wraps, int[] nextRequestToConsume, int lowestLayer, bool[] activeInCurrentLayer, int[] indexCounter, bool[] activeOverAllLayers);
    }
}