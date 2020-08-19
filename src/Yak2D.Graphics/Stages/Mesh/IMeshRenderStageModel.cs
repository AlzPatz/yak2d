using Veldrid;

namespace Yak2D.Graphics
{
    public interface IMeshRenderStageModel : IRenderStageModel
    {
        ResourceSet LightPropertiesResource { get; }
        ResourceSet LightsResource { get; }
        DeviceBuffer MeshVertexBuffer { get; }
        uint MeshNumberVertices { get; }

        void SetLightingProperties(ref MeshRenderLightingPropertiesConfiguration config, float transitionSeconds);
        void SetLights(MeshRenderLightConfiguration[] lightConfigurations, float transitionSeconds);
        void SetMesh(Vertex3D[] mesh);
    }
}