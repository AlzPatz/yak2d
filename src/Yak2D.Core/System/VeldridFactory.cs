using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.Utilities;
using Veldrid.SPIRV;
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

        public Shader CreateShaderCompileFromSpirv(ShaderDescription shaderDescription)
        {
            var stage = shaderDescription.Stage;

            if (stage == ShaderStages.Geometry || stage == ShaderStages.None || stage == ShaderStages.TessellationControl || stage == ShaderStages.TessellationEvaluation)
            {
                throw new Yak2DException("Unable to compile SPIRV shader - unsupported shader stage", new ArgumentException());
            }

            if (stage == ShaderStages.Compute)
            {
                return RawFactory?.CreateFromSpirv(shaderDescription);
            }

            var otherStage = stage == ShaderStages.Vertex ? ShaderStages.Fragment : ShaderStages.Vertex;
            var otherShaderDescription = new ShaderDescription
            {
                Debug = false,
                EntryPoint = "main",
                Stage = otherStage,
                ShaderBytes = GenerateValidUnusedShaderBytes(otherStage)
            };

            var shaders = RawFactory?.CreateFromSpirv(stage == ShaderStages.Vertex ? shaderDescription : otherShaderDescription,
                                                      stage == ShaderStages.Vertex ? otherShaderDescription : shaderDescription);

            return shaders[stage == ShaderStages.Vertex ? 0 : 1];
        }

        private byte[] GenerateValidUnusedShaderBytes(ShaderStages stage)
        {
            switch (stage)
            {
                case ShaderStages.Vertex:
                    var vertexCode = @"
                        #version 450
                        layout(location = 0) in vec2 Position;
                        layout(location = 1) in vec4 Color;
                        layout(location = 0) out vec4 fsin_Color;
                        void main()
                        {
                            gl_Position = vec4(Position, 0, 1);
                            fsin_Color = Color;
                        }";
                    return Encoding.UTF8.GetBytes(vertexCode);
                case ShaderStages.Fragment:
                    var fragmentCode = @"
                        #version 450
                        layout(location = 0) in vec4 fsin_Color;
                        layout(location = 0) out vec4 fsout_Color;
                        void main()
                        {
                            fsout_Color = fsin_Color;
                        }";
                    return Encoding.UTF8.GetBytes(fragmentCode);
            }

            return null;
        }

        public Texture CreateTexture(TextureDescription textureDescription) => RawFactory?.CreateTexture(textureDescription);

        public TextureView CreateTextureView(Texture texture) => RawFactory?.CreateTextureView(texture);

        public void DisposeAll() => RawFactory?.DisposeCollector.DisposeAll();
    }
}