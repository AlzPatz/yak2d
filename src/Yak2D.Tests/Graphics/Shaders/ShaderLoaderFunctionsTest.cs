using NeoVeldrid;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class ShaderLoaderFunctionsTest
    {
        [Fact]
        public void ShaderLoaderFunctions_CreateUniformResourceLayouts_ReturnNullOnNullArray()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var applicationassembly = Substitute.For<IApplicationAssembly>();
            var graphicsassembly = Substitute.For<IGraphicsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();


            var loader = new ShaderLoaderFunctions(messenger, components, applicationassembly, graphicsassembly, filesystem);

            Assert.Null(loader.CreateUniformResourceLayouts(null));
        }

        [Fact]
        public void ShaderLoaderFunctions_CreateUniformResourceLayouts_ReturnNullOnNullSubArray()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var applicationassembly = Substitute.For<IApplicationAssembly>();
            var graphicsassembly = Substitute.For<IGraphicsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            var loader = new ShaderLoaderFunctions(messenger, components, applicationassembly, graphicsassembly, filesystem);

            Assert.Null(loader.CreateUniformResourceLayouts(new NeoVeldrid.ResourceLayoutElementDescription[][]
                {
                    null,
                    new NeoVeldrid.ResourceLayoutElementDescription[]
                    {
                        new NeoVeldrid.ResourceLayoutElementDescription()
                    },
                }));
        }

        [Fact]
        public void ShaderLoaderFunctions_IsResourcePathInAssembly_EnsureCorrectPathIsCreated()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var applicationassembly = Substitute.For<IApplicationAssembly>();
            var graphicsassembly = Substitute.For<IGraphicsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            graphicsassembly.Name.Returns("bigname");
            graphicsassembly.GetManifestResourceNames().Returns(new List<string>
            {
                "bi3name.test.shady",
                "bigname.test.shady",
                "bigtame.test.shadyz",
                "biglame.test.shadyy",
                "bigshame.test.shady",
            });

            var loader = new ShaderLoaderFunctions(messenger, components, applicationassembly, graphicsassembly, filesystem);

            Assert.True(loader.IsResourcePathInAssembly(graphicsassembly, "test.shady"));
        }

        [Fact]
        public void ShaderLoaderFunctions_TryLoadShaderBytesFromFile_CorrectFileNameReturnsExpectedData()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var applicationassembly = Substitute.For<IApplicationAssembly>();
            var graphicsassembly = Substitute.For<IGraphicsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            filesystem.Exists(Arg.Any<string>()).Returns(true);
            filesystem.ReadAllBytes("Directory\\Name.Ext").Returns(new byte[7]);
            filesystem.ReadAllBytes("Directory/Name.Ext").Returns(new byte[7]);

            var loader = new ShaderLoaderFunctions(messenger, components, applicationassembly, graphicsassembly, filesystem);

            Assert.Equal(7, loader.TryLoadShaderBytesFromFile(new ShaderFileInfo
            {
                Directory = "Directory",
                Name = "Name",
                Extension = "Ext",
                EntryPointMethod = "None"
            }).Length);
        }

        private const string FragmentSource = "#version 450\nlayout(location = 0) out vec4 fsout_Colour;\nvoid main()\n{\n    fsout_Colour = vec4(1.0);\n}\n";

        [Theory]
        [InlineData(GraphicsApi.OpenGL, "1")]
        [InlineData(GraphicsApi.OpenGLES, "1")]
        [InlineData(GraphicsApi.Vulkan, "0")]
        [InlineData(GraphicsApi.Direct3D11, "0")]
        [InlineData(GraphicsApi.Metal, "0")]
        public void ShaderLoaderFunctions_InsertTextureOriginDefine_InsertsOneLineAfterVersion(GraphicsApi backend, string value)
        {
            var source = Encoding.UTF8.GetBytes(FragmentSource);

            var result = ShaderLoaderFunctions.InsertTextureOriginDefine(source, backend);

            var lines = Encoding.UTF8.GetString(result).Split('\n');
            Assert.Equal("#version 450", lines[0]);
            Assert.Equal("#define YAK2D_TEXTURE_ROWS_BOTTOM_UP " + value, lines[1]);
            Assert.Equal("YAK2D_TEXTURE_ROWS_BOTTOM_UP", ShaderLoaderFunctions.TextureOriginDefineName);

            //Removing the inserted line gives back the input byte for byte
            var versionLineLength = "#version 450\n".Length;
            var insertedLength = ("#define YAK2D_TEXTURE_ROWS_BOTTOM_UP " + value + "\n").Length;
            Assert.Equal(source.Length + insertedLength, result.Length);
            Assert.Equal(source, result.Take(versionLineLength).Concat(result.Skip(versionLineLength + insertedLength)).ToArray());
        }

        [Fact]
        public void ShaderLoaderFunctions_InsertTextureOriginDefine_NoVersionLine_DefineIsFirstLine()
        {
            var text = "layout(location = 0) out vec4 fsout_Colour;\nvoid main() { fsout_Colour = vec4(1.0); }\n";
            var source = Encoding.UTF8.GetBytes(text);

            var result = ShaderLoaderFunctions.InsertTextureOriginDefine(source, GraphicsApi.Vulkan);

            Assert.Equal("#define YAK2D_TEXTURE_ROWS_BOTTOM_UP 0\n" + text, Encoding.UTF8.GetString(result));
        }

        [Fact]
        public void ShaderLoaderFunctions_InsertTextureOriginDefine_VersionAfterCommentWithCrLf_DefineFollowsVersionLine()
        {
            var text = "// header comment\r\n  #version 450\r\nvoid main() { }\r\n";
            var source = Encoding.UTF8.GetBytes(text);

            var result = ShaderLoaderFunctions.InsertTextureOriginDefine(source, GraphicsApi.OpenGL);

            Assert.Equal("// header comment\r\n  #version 450\r\n#define YAK2D_TEXTURE_ROWS_BOTTOM_UP 1\r\nvoid main() { }\r\n", Encoding.UTF8.GetString(result));
        }

        private static (ShaderLoaderFunctions loader, IFactory factory) SetUpLoaderReturningFileBytes(GraphicsApi backend, byte[] fileBytes)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var applicationassembly = Substitute.For<IApplicationAssembly>();
            var graphicsassembly = Substitute.For<IGraphicsAssembly>();
            var filesystem = Substitute.For<IFileSystem>();

            var device = Substitute.For<IDevice>();
            device.BackendType.Returns(backend);
            components.Device.Returns(device);

            var factory = Substitute.For<IFactory>();
            components.Factory.Returns(factory);

            filesystem.Exists(Arg.Any<string>()).Returns(true);
            filesystem.ReadAllBytes(Arg.Any<string>()).Returns(fileBytes);

            return (new ShaderLoaderFunctions(messenger, components, applicationassembly, graphicsassembly, filesystem), factory);
        }

        [Theory]
        [InlineData(GraphicsApi.OpenGL, "1")]
        [InlineData(GraphicsApi.Vulkan, "0")]
        public void ShaderLoaderFunctions_LoadShaderSpirvPath_CompiledBytesContainDefine(GraphicsApi backend, string value)
        {
            var fileBytes = Encoding.UTF8.GetBytes(FragmentSource);
            var (loader, factory) = SetUpLoaderReturningFileBytes(backend, fileBytes);

            byte[] compiled = null;
            factory.CreateShaderCompileFromSpirv(Arg.Do<ShaderDescription>(d => compiled = d.ShaderBytes)).Returns((Shader)null);

            loader.LoadShader("Custom", AssetSourceEnum.File, ShaderStages.Fragment, true);

            factory.Received(1).CreateShaderCompileFromSpirv(Arg.Any<ShaderDescription>());
            factory.DidNotReceive().CreateShader(Arg.Any<ShaderDescription>());
            Assert.NotNull(compiled);
            Assert.Equal("#define YAK2D_TEXTURE_ROWS_BOTTOM_UP " + value, Encoding.UTF8.GetString(compiled).Split('\n')[1]);
        }

        [Fact]
        public void ShaderLoaderFunctions_LoadShaderPrecompiledOpenGLPath_BytesUnmodified()
        {
            var fileBytes = Encoding.UTF8.GetBytes(FragmentSource);
            var (loader, factory) = SetUpLoaderReturningFileBytes(GraphicsApi.OpenGL, fileBytes);

            byte[] created = null;
            factory.CreateShader(Arg.Do<ShaderDescription>(d => created = d.ShaderBytes)).Returns((Shader)null);

            loader.LoadShader("Custom", AssetSourceEnum.File, ShaderStages.Fragment, false);

            factory.Received(1).CreateShader(Arg.Any<ShaderDescription>());
            factory.DidNotReceive().CreateShaderCompileFromSpirv(Arg.Any<ShaderDescription>());
            Assert.Equal(Encoding.UTF8.GetBytes(FragmentSource), created);
        }
    }
}