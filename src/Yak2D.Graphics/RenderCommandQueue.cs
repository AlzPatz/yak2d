using System.Collections.Generic;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class RenderCommandQueue : IRenderCommandQueue
    {
        public int CommandQueueSize { get; private set; }
        public int CallbackQueueSize { get; private set; }

        private RenderCommandQueueItem[] _pool;
        private ulong[] _surfaceCopyStageCallbackPool;

        public RenderCommandQueue(IStartupPropertiesCache startUpPropertiesCache)
        {
            _pool = new RenderCommandQueueItem[startUpPropertiesCache.Internal.RenderCommandMinPoolSize];
            _surfaceCopyStageCallbackPool = new ulong[32];

            Reset();
        }

        public void Add(RenderCommandType type, ulong stage, ulong surface, ulong camera, ulong texture0, ulong texture1, ulong spareId0, ulong spareId1, RgbaFloat colour)
        {
            if (type == RenderCommandType.GpuToCpuSurfaceCopy)
            {
                if (CallbackQueueSize > 0)
                {
                    var copyStageContainedAlready = false;
                    for (var n = 0; n < CallbackQueueSize; n++)
                    {
                        if (_surfaceCopyStageCallbackPool[n] == stage)
                        {
                            copyStageContainedAlready = true;
                        }
                    }

                    if (copyStageContainedAlready)
                    {
                        throw new Yak2DException(string.Concat("Gpu to Cpu Surface Copy Stage (id: ", stage, " , is already queued. You should only queue specific queue once per render as a stage only contains a single data storage array that would be overwritten a second time"));
                    }
                }

                if (CallbackQueueSize >= _surfaceCopyStageCallbackPool.Length)
                {
                    _surfaceCopyStageCallbackPool = Utility.ArrayFunctions.DoubleArraySizeAndKeepContents<ulong>(_surfaceCopyStageCallbackPool);
                }

                _surfaceCopyStageCallbackPool[CallbackQueueSize] = stage;

                CallbackQueueSize++;
            }

            if (CommandQueueSize >= _pool.Length)
            {
                _pool = Utility.ArrayFunctions.DoubleArraySizeAndKeepContents<RenderCommandQueueItem>(_pool);
            }

            _pool[CommandQueueSize].Type = type;
            _pool[CommandQueueSize].Stage = stage;
            _pool[CommandQueueSize].Surface = surface;
            _pool[CommandQueueSize].Camera = camera;
            _pool[CommandQueueSize].Texture0 = texture0;
            _pool[CommandQueueSize].Texture1 = texture1;
            _pool[CommandQueueSize].SpareId0 = spareId0;
            _pool[CommandQueueSize].SpareId1 = spareId1;
            _pool[CommandQueueSize].Colour = colour;

            CommandQueueSize++;
        }

        public void Reset()
        {
            CommandQueueSize = 0;
            CallbackQueueSize = 0;
        }

        public IEnumerable<RenderCommandQueueItem> FlushCommands()
        {
            var iterator = 0;

            while (iterator < CommandQueueSize)
            {
                yield return _pool[iterator];
                iterator++;
            }
        }

        public IEnumerable<ulong> FlushCallbackStageIds()
        {
            var iterator = 0;

            while (iterator < CallbackQueueSize)
            {
                yield return _surfaceCopyStageCallbackPool[iterator];
                iterator++;
            }
        }
    }
}