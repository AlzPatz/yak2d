using Veldrid;

namespace Yak2D.Graphics
{
    public interface IPipelineFactory
    {
        void CreateAllDrawingPipelines(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription, OutputDescription outputDescription);
        Pipeline CreateAPipeline(ResourceLayout[] resourceLayouts,
                                                                                ShaderSetDescription shaderSetDescription,
                                                                                OutputDescription outputDescription,
                                                                                BlendStateDescription blendStateDescription,
                                                                                bool depthTest,
                                                                                FaceCullMode faceCullMode = FaceCullMode.None,
                                                                                PolygonFillMode polygonFillMode = PolygonFillMode.Solid,
                                                                                FrontFace frontFaceWindingOrder = FrontFace.Clockwise);
        Pipeline ReturnDrawingPipeline(BlendState blendState);
        Pipeline CreateNonDepthTestAlphaBlendPipeline(ResourceLayout[] uniformResourceLayout, ShaderSetDescription description, OutputDescription outputDescription);
        Pipeline CreateNonDepthTestOverrideBlendPipeline(ResourceLayout[] uniformResourceLayout, ShaderSetDescription description, OutputDescription outputDescription);
        Pipeline CreateDepthTestOverrideBlend(ResourceLayout[] uniformResourceLayout, ShaderSetDescription description, OutputDescription outputDescription);
        Pipeline CreateDistortionHeightMapPipeline(ResourceLayout[] uniformResourceLayout, ShaderSetDescription description);
        Pipeline CreateDistortionGradientShiftPipeline(ResourceLayout[] uniformResourceLayout, ShaderSetDescription description);
        Pipeline CreateDistortionRenderPipeline(ResourceLayout[] uniformResourceLayout, ShaderSetDescription description, OutputDescription outputDescription);
        void ClearPipelineList();
        void ReInitialise();
    }
}