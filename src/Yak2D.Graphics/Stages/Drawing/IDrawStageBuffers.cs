using System.Numerics;
using Veldrid;

namespace Yak2D.Graphics
{
    public interface IDrawStageBuffers
    {
        uint VertexBufferSize { get; }
        uint IndexBufferSize { get; }
        DeviceBuffer VertexBuffer { get; }
        DeviceBuffer IndexBuffer { get; }

        int ReturnMaxNumberLayersForDepthScaling();
        void SetMaxNumberOfLayersForDepthScaling(int max);

        void EnsureVertexBufferIsLargeEnough(int index, bool CopyExistingDataIfEnlarging = true);
        void EnsureIndexBufferIsLargeEnough(int index, bool CopyExistingDataIfEnlarging = true);

        void CopyAVertexToStagingArray(int index,
                        bool isWorld,
                        int texType,
                        int layer,
                        float depth,
                        ref Vertex2D vertex,
                        bool boundsCheck = false);

        void CopyAVertexToStagingArray(int index,
                        bool isWorld,
                        int texType,
                        Vector2 position,
                        int layer,
                        float depth,
                        Colour colour,
                        Vector2 texCoord0,
                        Vector2 texCoord1,
                        float texWeight0,
                        bool boundsCheck = false);
        void DestroyResources();
        void CopyAVertexToStagingArray(ref int index,
                        ref bool isWorld,
                        ref int texType,
                        ref Vector2 position,
                        ref int layer,
                        ref float depth,
                        ref Colour colour,
                        ref Vector2 texCoord0,
                        ref Vector2 texCoord1,
                        ref float texWeight0,
                        bool boundsCheck = false);

        void CopyAnIndexToStagingArray(uint index,
                        uint value,
                        bool boundsCheck = false);

        void CopyAnIndexToStagingArray(ref uint index,
                        ref uint value,
                        bool boundsCheck = false);

        void CopyVertexBufferSegmentToGpu(int start, int length);
        void CopyIndexBufferSegmentToGpu(int start, int length);
    }
}