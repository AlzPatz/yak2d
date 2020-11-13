using System;

namespace Yak2D.Graphics
{
    public interface ISurfaceCopyStageModel : IRenderStageModel
    {
        ulong StagingTextureId { get; }
        void SetCallback(Action<uint, TextureData> callback);
        void CreateStagingTextureAndDataArray(uint stagingTextureWidth, uint stagingTextureHeight);
        void CopyDataFromStagingTextureAndPassToUser();
    }
}