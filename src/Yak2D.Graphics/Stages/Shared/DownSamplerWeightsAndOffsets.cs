using System.Numerics;

namespace Yak2D.Graphics
{
    public class DownSamplerWeightsAndOffsets : IDownSamplerWeightsAndOffsets
    {
        private const int SAMPLE_ARRAY_SIZE = 16;

        private DownSampleArrayComponent[] _nearestNeighbour;
        private DownSampleArrayComponent[] _average2x2;
        private DownSampleArrayComponent[] _average4x4;

        public int WeightsAndOffsetsArraySize => SAMPLE_ARRAY_SIZE;

        public DownSamplerWeightsAndOffsets()
        {
            CreateWeightsAndOffsetArrays();
        }

        private void CreateWeightsAndOffsetArrays()
        {
            _nearestNeighbour = new DownSampleArrayComponent[]
            {
                new DownSampleArrayComponent
                {
                    Offset = Vector2.Zero,
                    Weight = 1.0f
                }
            };

            _average2x2 = new DownSampleArrayComponent[]
            {
                new DownSampleArrayComponent
                {
                    Offset = new Vector2(-0.5f, -0.5f),
                    Weight = 0.25f,
                    Pad=0.0f
                },
                new DownSampleArrayComponent
                {
                    Offset = new Vector2(0.5f, -0.5f),
                    Weight = 0.25f,
                    Pad=0.0f
                },
                new DownSampleArrayComponent
                {
                    Offset = new Vector2(-0.5f, 0.5f),
                    Weight = 0.25f,
                    Pad=0.0f
                },
                new DownSampleArrayComponent
                {
                    Offset = new Vector2(0.5f, 0.5f),
                    Weight = 0.25f,
                    Pad = 0.0f
                }
            };

            _average4x4 = new DownSampleArrayComponent[16];

            var weight = 1.0f / 16.0f;
            var corner = new Vector2(-1.5f, -1.5f);
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    var index = (y * 4) + x;
                    var position = new Vector2(x, y) + corner;
                    _average4x4[index] = new DownSampleArrayComponent
                    {
                        Offset = position,
                        Weight = weight,
                        Pad = 0.0f
                    };
                }
            }
        }

        public int NumberOfSamples(ResizeSamplerType type)
        {
            switch (type)
            {
                case ResizeSamplerType.NearestNeighbour:
                    return 1;
                case ResizeSamplerType.Average2x2:
                    return 4;
                case ResizeSamplerType.Average4x4:
                    return 16;
            }
            return 0;
        }

        public DownSampleArrayComponent[] ReturnWeightsAndOffsets(ResizeSamplerType type)
        {
            switch (type)
            {
                case ResizeSamplerType.NearestNeighbour:
                    return _nearestNeighbour;
                case ResizeSamplerType.Average2x2:
                    return _average2x2;
                case ResizeSamplerType.Average4x4:
                    return _average4x4;
            }
            return null;
        }
    }
}
