using System.Collections.Generic;

namespace Yak2D.Graphics
{
    public interface IDrawStageBatcher
    {
        DrawingBatch[] Pool { get; }
        int NumberOfBatches { get; }
        void Process(QueueWrap dyanmic, List<QueueWrap> persistent);
    }
}