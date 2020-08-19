using System.Collections.Generic;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class RenderCommandQueue : IRenderCommandQueue
    {
        public int Size { get; private set; }

        private RenderCommandQueueItem[] _pool;

        public RenderCommandQueue(IStartupPropertiesCache startUpPropertiesCache)
        {
            _pool = new RenderCommandQueueItem[startUpPropertiesCache.Internal.RenderCommandMinPoolSize];
            Reset();
        }

        public void Add(RenderCommandType type, ulong stage, ulong surface, ulong camera, ulong texture0, ulong texture1, ulong spareId0, ulong spareId1, RgbaFloat colour)
        {
            if (Size >= _pool.Length)
            {
                _pool = Utility.ArrayFunctions.DoubleArraySizeAndKeepContents<RenderCommandQueueItem>(_pool);
            }

            _pool[Size].Type = type;
            _pool[Size].Stage = stage;
            _pool[Size].Surface = surface;
            _pool[Size].Camera = camera;
            _pool[Size].Texture0 = texture0;
            _pool[Size].Texture1 = texture1;
            _pool[Size].SpareId0 = spareId0;
            _pool[Size].SpareId1 = spareId1;
            _pool[Size].Colour = colour;

            Size++;
        }

        public void Reset()
        {
            Size = 0;
        }

        public IEnumerable<RenderCommandQueueItem> Flush()
        {
            var iterator = 0;

            while (iterator < Size)
            {
                yield return _pool[iterator];
                iterator++;
            }
        }
    }
}