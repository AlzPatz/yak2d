using System.Collections.Generic;

namespace Yak2D.Graphics
{
    public class DrawStageBatcher : IDrawStageBatcher
    {
        public DrawingBatch[] Pool { get; set; }

        private int _poolSize;
        public int NumberOfBatches { get; set; }

        private IDrawStageBatcherTools _tools;

        public DrawStageBatcher(int initialBatchPoolSize, IDrawStageBatcherTools tools)
        {
            _tools = tools;

            _poolSize = initialBatchPoolSize;
            Pool = _tools.CreatePoolOfSize(_poolSize, false);
        }

        public void Process(QueueWrap dyanmic, List<QueueWrap> persistent)
        {
            var combinedList = _tools.CombinedList(dyanmic, persistent);

            ProcessAndGenerateBatches(combinedList.ToArray());
        }

        private void ProcessAndGenerateBatches(QueueWrap[] queues)
        {
            var numQueues = queues.Length;

            var activeQueues = new bool[numQueues];
            for (var n = 0; n < numQueues; n++)
            {
                activeQueues[n] = true;
            }

            var nextRequestToConsume = new int[numQueues];
            for (var n = 0; n < numQueues; n++)
            {
                nextRequestToConsume[n] = 0;
            }

            var indexCounter = new int[numQueues];
            for (var n = 0; n < numQueues; n++)
            {
                indexCounter[n] = 0;
            }

            var activeInCurrentLayer = new bool[numQueues];

            NumberOfBatches = 0;

            while (true)
            {
                if (_tools.AreAllQueuesInactive(ref numQueues, activeQueues))
                {
                    //Batching is complete
                    break;
                }

                var lowestLayer = _tools.FindLowestLayer(ref numQueues, queues, activeQueues, nextRequestToConsume);

                _tools.IdentifyWhichQueuesAreActiveInTheCurrentLayer(ref numQueues, queues, nextRequestToConsume, lowestLayer, activeInCurrentLayer, activeQueues);

                var queueForBatch = _tools.FindDeepestQueueAtLowestLayer(ref numQueues, queues, nextRequestToConsume, lowestLayer, activeInCurrentLayer);

                CheckIfPoolFullAndReSize();

                if (_tools.GenerateTheNextBatch(Pool, queueForBatch, NumberOfBatches, ref numQueues, queues, nextRequestToConsume, lowestLayer, activeInCurrentLayer, indexCounter, activeQueues))
                {
                    NumberOfBatches++;
                }
            }
        }

        private void CheckIfPoolFullAndReSize()
        {
            if (NumberOfBatches == _poolSize)
            {
                _poolSize *= 2;
                Pool = _tools.CreatePoolOfSize(_poolSize, true, Pool);
            }
        }
    }
}