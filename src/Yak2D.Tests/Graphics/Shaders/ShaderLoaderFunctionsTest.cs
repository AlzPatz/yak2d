using NSubstitute;
using System.Collections.Generic;
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

            Assert.Null(loader.CreateUniformResourceLayouts(new Veldrid.ResourceLayoutElementDescription[][]
                {
                    null,
                    new Veldrid.ResourceLayoutElementDescription[]
                    {
                        new Veldrid.ResourceLayoutElementDescription()
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
    }
}