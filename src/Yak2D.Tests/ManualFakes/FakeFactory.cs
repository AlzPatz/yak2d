using Veldrid;
using Veldrid.Utilities;
using Veldrid.SPIRV;
using Yak2D.Internal;

namespace Yak2D.Tests.ManualFakes
{
    public class FakeFactory : IFactory
    {
        public void Dispose()
        {
            _factory?.DisposeCollector.DisposeAll();
        }

        private DisposeCollectorResourceFactory _factory;

        public DisposeCollectorResourceFactory RawFactory => throw new System.NotImplementedException();

        public FakeFactory(GraphicsDevice device)
        {
            _factory = new DisposeCollectorResourceFactory(device.ResourceFactory);
        }

        public DeviceBuffer CreateBuffer(BufferDescription bufferDescription) => _factory.CreateBuffer(bufferDescription);
        public Framebuffer CreateFramebuffer(FramebufferDescription framebufferDescription) => _factory.CreateFramebuffer(framebufferDescription);
        public Pipeline CreateGraphicsPipeline(GraphicsPipelineDescription pipelineDescription) => _factory.CreateGraphicsPipeline(pipelineDescription);
        public ResourceLayout CreateResourceLayout(ResourceLayoutDescription resourceLayoutDescription) => _factory.CreateResourceLayout(resourceLayoutDescription);
        public ResourceSet CreateResourceSet(ResourceSetDescription resourceSetDescription) => _factory.CreateResourceSet(resourceSetDescription);
        public Sampler CreateSampler(SamplerDescription samplerDescription) => _factory.CreateSampler(samplerDescription);
        public Shader CreateShader(ShaderDescription shaderDescription) => _factory.CreateShader(shaderDescription);
        public Shader CreateShaderCompileFromSpirv(ShaderDescription shaderDescription) => _factory.CreateFromSpirv(shaderDescription);
        public Texture CreateTexture(TextureDescription textureDescription) => _factory.CreateTexture(textureDescription);
        public TextureView CreateTextureView(Texture texture) => _factory.CreateTextureView(texture);

        public CommandList CreateCommandList()
        {
            throw new System.NotImplementedException();
        }

        public void DisposeAll()
        {
            throw new System.NotImplementedException();
        }

    }
}