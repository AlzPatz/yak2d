using System.Numerics;
using NeoVeldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawStageBuffers : IDrawStageBuffers
    {
        private IDevice _device;
        private IFactory _factory;

        public DeviceBuffer VertexBuffer { get; private set; }
        public DeviceBuffer IndexBuffer { get; private set; }
        public uint VertexBufferSize { get; private set; }
        public uint IndexBufferSize { get; private set; }

        private DrawingVertex[] _verts;
        private uint[] _indices;

        private int _maxNumberOfLayersForDepthScaling;
        private float _layerDepthDelta;

        public DrawStageBuffers(ISystemComponents components, IStartupPropertiesCache startUpPropertiesCache)
        {
            _device = components.Device;
            _factory = components.Factory;

            var props = startUpPropertiesCache.Internal;

            var sizeVertex = props.DrawStageInitialSizeVertexBuffer < 3 ? 3 : props.DrawStageInitialSizeVertexBuffer;

            var remainder = props.DrawStageInitialSizeVertexBuffer % 3;
            var sizeIndex = remainder == 0 ? props.DrawStageInitialSizeIndexBuffer : props.DrawStageInitialSizeIndexBuffer + (3 - remainder); //I don't think this does what i expect (there are not 3 indices used per vertex?)

            CreateDeviceBuffersAndDataStagingArrays(sizeVertex, sizeIndex);

            SetMaxNumberOfLayersForDepthScaling(props.DrawStageInitialMaxNumberOfLayersForDepthScaling);
        }

        private void CreateDeviceBuffersAndDataStagingArrays(int sizeVertex, int sizeIndex)
        {
            VertexBufferSize = (uint)sizeVertex;
            IndexBufferSize = (uint)sizeIndex;

            _verts = new DrawingVertex[VertexBufferSize];
            _indices = new uint[IndexBufferSize];

            VertexBuffer = CreateVertexBuffer(VertexBufferSize);
            IndexBuffer = CreateIndexBuffer(IndexBufferSize);
        }

        private DeviceBuffer CreateVertexBuffer(uint size)
        {
            return CreateDynamicBuffer(size, DrawingVertex.SizeInBytes, BufferUsage.VertexBuffer);
        }

        private DeviceBuffer CreateIndexBuffer(uint size)
        {
            return CreateDynamicBuffer(size, sizeof(uint), BufferUsage.IndexBuffer);
        }

        private DeviceBuffer CreateDynamicBuffer(uint numElements, uint sizeElementInBytes, BufferUsage usage)
        {
            return _factory.CreateBuffer(
                new BufferDescription(
                numElements * sizeElementInBytes,
                usage | BufferUsage.Dynamic
                )
            );
        }

        public int ReturnMaxNumberLayersForDepthScaling()
        {
            return _maxNumberOfLayersForDepthScaling;
        }

        public void SetMaxNumberOfLayersForDepthScaling(int max)
        {
            if (max != _maxNumberOfLayersForDepthScaling && max > 0)
            {
                ChangeMaxNumberOfLayers(max);
            }
        }

        private void ChangeMaxNumberOfLayers(int max)
        {
            var old = _maxNumberOfLayersForDepthScaling;

            _maxNumberOfLayersForDepthScaling = max;
            _layerDepthDelta = 1.0f / (1.0f * _maxNumberOfLayersForDepthScaling);

            ReBaseDepthDataAndUploadToGpu(old, max);
        }

        private void ReBaseDepthDataAndUploadToGpu(float previousMaxLayers, float currentMaxLayers)
        {
            //This is inefficient. I do not want to look upwards and find out what needs updating so the whole buffer gets it. Changing max number of layers should not be regular occurance
            for (var p = 0; p < VertexBufferSize; p++)
            {
                var currentDepth = _verts[p].Position.Z;

                var reScaledDepth = 1.0f - ((1.0f - currentDepth) * (previousMaxLayers / currentMaxLayers));

                if (reScaledDepth < 0.0f)
                {
                    reScaledDepth = 0.0f;
                }

                _verts[p].Position.Z = reScaledDepth;
            }

            _device.UpdateBuffer(VertexBuffer, 0, _verts);
        }

        public void EnsureVertexBufferIsLargeEnough(int minNumberExtraVertices, bool CopyExistingDataIfEnlarging = true)
        {
            bool upsized = false;
            while (_verts.Length < minNumberExtraVertices)
            {
                upsized = true;
                if (CopyExistingDataIfEnlarging)
                {
                    _verts = Utility.ArrayFunctions.DoubleArraySizeAndKeepContents<DrawingVertex>(_verts);
                }
                else
                {
                    _verts = new DrawingVertex[_verts.Length * 2];
                }
            }

            VertexBufferSize = (uint)_verts.Length;

            if (upsized)
            {
                VertexBuffer = CreateVertexBuffer(VertexBufferSize);
                _device.UpdateBuffer(VertexBuffer, 0, _verts);
            }
        }

        public void EnsureIndexBufferIsLargeEnough(int minNumberExtraIndices, bool CopyExistingDataIfEnlarging = true)
        {
            bool upsized = false;
            while (_indices.Length < minNumberExtraIndices)
            {
                upsized = true;
                if (CopyExistingDataIfEnlarging)
                {
                    _indices = Utility.ArrayFunctions.DoubleArraySizeAndKeepContents<uint>(_indices);
                }
                else
                {
                    _indices = new uint[_indices.Length * 2];
                }
            }

            IndexBufferSize = (uint)_indices.Length;

            if (upsized)
            {
                IndexBuffer = CreateIndexBuffer(IndexBufferSize);
                _device.UpdateBuffer(IndexBuffer, 0, _indices);
            }
        }

        public void CopyAVertexToStagingArray(int index,
                            bool isWorld,
                            int texType,
                            int layer,
                            float depth,
                            ref Vertex2D vertex,
                            bool boundsCheck = false)
        {
            CopyAVertexToStagingArray(index,
                            isWorld,
                            texType,
                            vertex.Position,
                            layer,
                            depth,
                            vertex.Colour,
                            vertex.TexCoord0,
                            vertex.TexCoord1,
                            vertex.TexWeighting,
                            boundsCheck);
        }

        public void CopyAVertexToStagingArray(int index,
                            bool isWorld,
                            int texType,
                            Vector2 position,
                            int layer,
                            float depth,
                            Colour colour,
                            Vector2 texCoord0,
                            Vector2 texCoord1,
                            float texWeight0,
                            bool boundsCheck = false)
        {
            CopyAVertexToStagingArray(ref index,
                            ref isWorld,
                            ref texType,
                            ref position,
                            ref layer,
                            ref depth,
                            ref colour,
                            ref texCoord0,
                            ref texCoord1,
                            ref texWeight0,
                            boundsCheck);
        }
        public void CopyAVertexToStagingArray(ref int index,
                            ref bool isWorld,
                            ref int texType,
                            ref Vector2 position,
                            ref int layer,
                            ref float depth,
                            ref Colour colour,
                            ref Vector2 texCoord0,
                            ref Vector2 texCoord1,
                            ref float texWeight0,
                            bool boundsCheck = false)
        {
            var processedDepth = CalculateProcessedDepth(ref depth, ref layer);
            var pos = new Vector3(position, processedDepth);
            var col = new RgbaFloat(colour.R, colour.G, colour.B, colour.A);
            CopyAVertexToStagingArray(ref index, ref isWorld, ref texType, ref pos, ref col, ref texCoord0, ref texCoord1, ref texWeight0, ref boundsCheck);
        }

        private float CalculateProcessedDepth(ref float depth, ref int layer)
        {
            if (layer >= _maxNumberOfLayersForDepthScaling)
            {
                GrowMaxNumberOfLayersToFit(layer);
            }

            //Depths are 1.0 = Back, 0.0 = Front. Layers however are 0-> Back to Front
            var layerStartFactor = 1.0f * layer * _layerDepthDelta;
            var layerStart = 1.0f - layerStartFactor;
            return layerStart - ((1.0f - depth) * _layerDepthDelta);
        }

        //Grows the depth scaling max so that 'layer' maps inside the 0..1 clip range (a layer >= max would give z < 0 and be clipped, silently dropping the draw)
        //The max is doubled until it exceeds the layer, rather than set straight to layer + 1:
        // - each growth walks the whole staging array and uploads the whole vertex buffer (ReBaseDepthDataAndUploadToGpu),
        //   so an app ramping through layers 10, 11, 12... pays O(log L) growths rather than one per new layer
        // - the rebase ratio previousMax / currentMax is always a power of two (0.5 per doubling), which is exact in binary floating point,
        //   so rebased depths match freshly computed ones without drift across repeated growths (ratios like 10/11, 11/12... would not)
        //Negative layers never trigger growth (the condition is layer >= max) and keep their existing behaviour:
        // - with validate = true, DrawQueue.AddIfValid rejects them ("Add draw request failed: a negative layer number is not valid")
        // - with the default validate = false they are queued, and CalculateProcessedDepth gives z >= 1 (exactly 1.0 only for layer -1 at depth 0),
        //   i.e. at or behind the far clip plane, so they are not drawn. The rebase only clamps values below 0, so z >= 1 stays >= 1 after a growth
        private void GrowMaxNumberOfLayersToFit(int layer)
        {
            var newMax = _maxNumberOfLayersForDepthScaling < 1 ? 1 : _maxNumberOfLayersForDepthScaling;

            while (newMax <= layer)
            {
                if (newMax > int.MaxValue / 2)
                {
                    newMax = int.MaxValue;
                    break;
                }
                newMax *= 2;
            }

            if (newMax != _maxNumberOfLayersForDepthScaling)
            {
                ChangeMaxNumberOfLayers(newMax);
            }
        }

        //Test seam only (exposed to Yak2D.Tests via InternalsVisibleTo)
        internal float ReturnStagedVertexDepth(int index) => _verts[index].Position.Z;

        private void CopyAVertexToStagingArray(ref int index,
                            ref bool isWorld,
                            ref int texType,
                            ref Vector3 position,
                            ref RgbaFloat color,
                            ref Vector2 texCoord0,
                            ref Vector2 texCoord1,
                            ref float texWeight0,
                            ref bool boundsCheck)
        {
            if (boundsCheck)
            {
                EnsureVertexBufferIsLargeEnough(index, true);
            }

            _verts[index].IsWorld = isWorld ? 0 : 1;
            _verts[index].Position = position;
            _verts[index].TexturingType = texType;
            _verts[index].Position = position;
            _verts[index].Color = color;
            _verts[index].TexCoord0 = texCoord0;
            _verts[index].TexCoord1 = texCoord1;
            _verts[index].TexWeight0 = texWeight0;
        }

        public void CopyAnIndexToStagingArray(uint index, uint value, bool boundsCheck = false)
        {
            CopyAnIndexToStagingArray(ref index, ref value, boundsCheck);
        }

        public void CopyAnIndexToStagingArray(ref uint index, ref uint value, bool boundsCheck = false)
        {
            if (boundsCheck)
            {
                EnsureIndexBufferIsLargeEnough((int)index, true);
            }
            _indices[index] = value;
        }

        //The following Buffer copies need blittable types. I believe my struct arrays are fine here
        public void CopyVertexBufferSegmentToGpu(int start, int length)
        {
            _device.UpdateBuffer(VertexBuffer, (uint)(start * DrawingVertex.SizeInBytes), ref _verts[start], (uint)(length * DrawingVertex.SizeInBytes));
        }

        public void CopyIndexBufferSegmentToGpu(int start, int length)
        {
            _device.UpdateBuffer(IndexBuffer, (uint)(start * sizeof(uint)), ref _indices[start], (uint)(length * sizeof(uint)));
        }

        public void DestroyResources()
        {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
        }
    }
}