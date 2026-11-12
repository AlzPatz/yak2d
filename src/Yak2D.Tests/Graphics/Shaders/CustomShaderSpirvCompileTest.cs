using System.Text;
using NeoVeldrid;
using NeoVeldrid.Utilities;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Tests.ManualFakes;

namespace Yak2D.Tests
{
    //Compiles custom fragment shaders through the production SPIR-V path (Yak2D.Core.VeldridFactory -> NeoVeldrid.SPIRV)
    //on the real test device, after the texture origin define has been inserted for each backend value
    public class CustomShaderSpirvCompileTest
    {
        //Input matches the placeholder vertex shader VeldridFactory pairs a fragment shader with (vec4 at location 0)
        private const string ShaderUsingDefine = @"#version 450
layout(set = 0, binding = 0) uniform texture2D Texture_Tex;
layout(set = 0, binding = 1) uniform sampler Sampler_Tex;
layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Colour;
void main()
{
    vec2 uv = fract(fsin_Color.xy * 4.0);
#if YAK2D_TEXTURE_ROWS_BOTTOM_UP
    uv.y = 1.0 - uv.y;
#endif
    fsout_Colour = texture(sampler2D(Texture_Tex, Sampler_Tex), uv);
}
";

        private const string ShaderNotUsingDefine = @"#version 450
layout(set = 0, binding = 0) uniform texture2D Texture_Tex;
layout(set = 0, binding = 1) uniform sampler Sampler_Tex;
layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Colour;
void main()
{
    vec2 uv = fract(fsin_Color.xy * 4.0);
    fsout_Colour = texture(sampler2D(Texture_Tex, Sampler_Tex), uv);
}
";

        [Theory]
        [InlineData(true, GraphicsApi.OpenGL)]
        [InlineData(true, GraphicsApi.Vulkan)]
        [InlineData(false, GraphicsApi.OpenGL)]
        [InlineData(false, GraphicsApi.Vulkan)]
        public void CustomShaderSpirvCompile_FragmentShaderWithInsertedDefine_Compiles(bool usesDefine, GraphicsApi defineFor)
        {
            var source = Encoding.UTF8.GetBytes(usesDefine ? ShaderUsingDefine : ShaderNotUsingDefine);
            var bytes = ShaderLoaderFunctions.InsertTextureOriginDefine(source, defineFor);

            //FakeGraphicsDevice is not IDisposable (it has a public Dispose method), so dispose explicitly
            var device = new FakeGraphicsDevice();
            var factory = new Yak2D.Core.VeldridFactory(new DisposeCollectorResourceFactory(device.Device.ResourceFactory));
            try
            {
                var shader = factory.CreateShaderCompileFromSpirv(new ShaderDescription(ShaderStages.Fragment, bytes, "main", true));

                Assert.NotNull(shader);
            }
            finally
            {
                factory.DisposeAll();
                device.Dispose();
            }
        }
    }
}
