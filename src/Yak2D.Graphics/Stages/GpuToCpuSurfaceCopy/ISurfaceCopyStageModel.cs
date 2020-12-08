using System;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public interface ISurfaceCopyStageModel : IRenderStageModel
    {
        ulong StagingTextureId { get; }
        void SetCallback(Action<TextureData> callback);
        void SetPixelFormatAndCreateStagingTextureAndDataArray(uint stagingTextureWidth, uint stagingTextureHeight, TexturePixelFormat pixelFormat);
        void CopyDataFromStagingTextureAndPassToUser();
    }
}