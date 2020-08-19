using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.Utilities;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class VeldridFactory : IFactory
    {
        public DisposeCollectorResourceFactory RawFactory { get; private set; }

        public VeldridFactory(DisposeCollectorResourceFactory factory)
        {
            RawFactory = factory;
        }

        public DeviceBuffer CreateBuffer(BufferDescription bufferDescription) => RawFactory?.CreateBuffer(bufferDescription);

        public CommandList CreateCommandList() => RawFactory?.CreateCommandList();

        public Framebuffer CreateFramebuffer(FramebufferDescription framebufferDescription) => RawFactory?.CreateFramebuffer(framebufferDescription);

        public Pipeline CreateGraphicsPipeline(GraphicsPipelineDescription pipelineDescription) => RawFactory?.CreateGraphicsPipeline(pipelineDescription);

        public ResourceLayout CreateResourceLayout(ResourceLayoutDescription resourceLayoutDescription) => RawFactory?.CreateResourceLayout(resourceLayoutDescription);

        public ResourceSet CreateResourceSet(ResourceSetDescription resourceSetDescription) => RawFactory?.CreateResourceSet(resourceSetDescription);

        public Sampler CreateSampler(SamplerDescription samplerDescription) => RawFactory?.CreateSampler(samplerDescription);

        public Shader CreateShader(ShaderDescription shaderDescription) => RawFactory?.CreateShader(shaderDescription);

        public Texture CreateTexture(TextureDescription textureDescription) => RawFactory?.CreateTexture(textureDescription);

        public TextureView CreateTextureView(Texture texture) => RawFactory?.CreateTextureView(texture);

        public void DisposeAll() => RawFactory?.DisposeCollector.DisposeAll();
    }
}