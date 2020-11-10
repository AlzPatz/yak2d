using Veldrid;
using Veldrid.Utilities;

namespace Yak2D.Internal
{
    public interface IFactory
    {
        DisposeCollectorResourceFactory RawFactory { get; }

        ResourceLayout CreateResourceLayout(ResourceLayoutDescription resourceLayoutDescription);
        Shader CreateShaderCompileFromSpirv(ShaderDescription shaderDescription);
        Shader CreateShader(ShaderDescription shaderDescription);
        Sampler CreateSampler(SamplerDescription samplerDescription);
        TextureView CreateTextureView(Texture texture);
        ResourceSet CreateResourceSet(ResourceSetDescription resourceSetDescription);
        Texture CreateTexture(TextureDescription textureDescription);
        Framebuffer CreateFramebuffer(FramebufferDescription framebufferDescription);
        DeviceBuffer CreateBuffer(BufferDescription bufferDescription);
        Pipeline CreateGraphicsPipeline(GraphicsPipelineDescription pipelineDescription);
        CommandList CreateCommandList();
        void DisposeAll();
    }
}