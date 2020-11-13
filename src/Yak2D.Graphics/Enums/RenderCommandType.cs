namespace Yak2D.Graphics 
{
    public enum RenderCommandType
    {
        ClearColourTarget,
        ClearDepthTarget,
        DrawStage,
        ColourEffectStage,
        BloomEffectStage,
        Blur1DEffectStage,
        Blur2DEffectStage,
        CopyStage,
        StyleEffect,
        MeshRender,
        DistortionStage,
        MixStage,
        SetViewport,
        ClearViewport,
        CustomShader,
        CustomVeldrid,
        GpuToCpuSurfaceCopy
    }
}
