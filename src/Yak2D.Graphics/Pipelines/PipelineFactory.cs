using System;
using System.Collections.Generic;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class PipelineFactory : IPipelineFactory
    {
        private readonly ISystemComponents _components;
        private readonly IBlendStateConverter _blendStateConverter;

        private Dictionary<BlendState, Pipeline> _drawingPipelines;

        public PipelineFactory(ISystemComponents components,
                                                IBlendStateConverter blendStateConverter)
        {
            _components = components;
            _blendStateConverter = blendStateConverter;
        }

        public void CreateAllDrawingPipelines(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription, OutputDescription outputDescription)
        {
            _drawingPipelines = new Dictionary<BlendState, Pipeline>();

            var states = Enum.GetValues(typeof(BlendState));

            foreach (BlendState state in states)
            {
                _drawingPipelines.Add(state, CreateAPipeline(resourceLayouts, shaderSetDescription, outputDescription, _blendStateConverter.Convert(state), true));
            }
        }

        public Pipeline ReturnDrawingPipeline(BlendState blendState)
        {
            return _drawingPipelines.ContainsKey(blendState) ? _drawingPipelines[blendState] : null;
        }

        public Pipeline CreateAPipeline(ResourceLayout[] resourceLayouts,
                                                                                ShaderSetDescription shaderSetDescription,
                                                                                OutputDescription outputDescription,
                                                                                BlendStateDescription blendStateDescription,
                                                                                bool depthTest,
                                                                                FaceCullMode faceCullMode = FaceCullMode.None,
                                                                                PolygonFillMode polygonFillMode = PolygonFillMode.Solid,
                                                                                FrontFace frontFaceWindingOrder = FrontFace.Clockwise)
        {
            var pipelineDescription = new GraphicsPipelineDescription()
            {
                BlendState = blendStateDescription,
                DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: depthTest,
                depthWriteEnabled: depthTest,
                comparisonKind: ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(
                cullMode: faceCullMode,
                fillMode: polygonFillMode,
                frontFace: frontFaceWindingOrder,
                depthClipEnabled: depthTest,
                scissorTestEnabled: false
            ),
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = resourceLayouts,
                ShaderSet = shaderSetDescription,
                Outputs = outputDescription
            };

            return _components.Factory.CreateGraphicsPipeline(pipelineDescription);
        }

        public Pipeline CreateDepthTestOverrideBlend(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription, OutputDescription outputDescription)
        {
            return CreateAPipeline(resourceLayouts, shaderSetDescription, outputDescription, BlendStateDescription.SingleOverrideBlend, true);
        }

        public Pipeline CreateNonDepthTestAlphaBlendPipeline(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription, OutputDescription outputDescription)
        {
            return CreateAPipeline(resourceLayouts, shaderSetDescription, outputDescription, BlendStateDescription.SingleAlphaBlend, false);
        }

        public Pipeline CreateNonDepthTestOverrideBlendPipeline(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription, OutputDescription outputDescription)
        {
            return CreateAPipeline(resourceLayouts, shaderSetDescription, outputDescription, BlendStateDescription.SingleOverrideBlend, false);
        }

        public Pipeline CreateDistortionHeightMapPipeline(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription)
        {
            var customBlendState = new BlendStateDescription(new RgbaFloat(), new BlendAttachmentDescription(
                true,
                BlendFactor.One,
                BlendFactor.One,
                BlendFunction.Add,
                BlendFactor.One,
                BlendFactor.One,
                BlendFunction.Add
            ));

            var outputDescription = new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.R32_Float));

            return CreateAPipeline(resourceLayouts, shaderSetDescription, outputDescription, customBlendState, false);
        }

        public Pipeline CreateDistortionGradientShiftPipeline(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription)
        {
            var outputDescription = new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.R32_G32_Float));

            return CreateAPipeline(resourceLayouts, shaderSetDescription, outputDescription, BlendStateDescription.SingleOverrideBlend, false);
        }

        public Pipeline CreateDistortionRenderPipeline(ResourceLayout[] resourceLayouts, ShaderSetDescription shaderSetDescription, OutputDescription outputDescription)
        {
            return CreateNonDepthTestAlphaBlendPipeline(resourceLayouts, shaderSetDescription, outputDescription);
        }

        public void ClearPipelineList()
        {
            _drawingPipelines.Clear();
        }

        public void ReInitialise() => ClearPipelineList();  //Nothing else to do for now... could remove and collapse
    }
}