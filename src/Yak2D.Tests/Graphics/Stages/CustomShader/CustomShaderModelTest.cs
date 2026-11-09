using NSubstitute;
using NeoVeldrid;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class CustomShaderModelTest
    {
        private ICustomShaderStageModel SetUpModelForSimpleAccessTests(IFrameworkMessenger messenger, ISystemComponents components = null)
        {

            if (components == null)
            {
                components = Substitute.For<ISystemComponents>();
            }

            var idevice = Substitute.For<IDevice>();

            idevice.SwapchainFramebufferOutputDescription.Returns(new OutputDescription
            {
                ColorAttachments = new OutputAttachmentDescription[] { },
                DepthAttachment = new OutputAttachmentDescription(),
                SampleCount = NeoVeldrid.TextureSampleCount.Count1
            });
            components.Device.Returns(idevice);
            var loader = Substitute.For<IShaderLoader>();
            loader.CreateShaderPackage(Arg.Any<string>(),
                                       Arg.Any<AssetSourceEnum>(),
                                       Arg.Any<string>(),
                                       Arg.Any<AssetSourceEnum>(),
                                       Arg.Any<VertexLayoutDescription>(),
                                       Arg.Any<ResourceLayoutElementDescription[][]>()).Returns(new ShaderPackage
                                       {
                                           Description = new ShaderSetDescription(),
                                           UniformResourceLayout = new ResourceLayout[] { }
                                       });

            var factory = Substitute.For<IPipelineFactory>();

            var ifactory = Substitute.For<IFactory>();
            ifactory.CreateBuffer(Arg.Any<BufferDescription>()).Returns(Substitute.For<DeviceBuffer>());
            components.Factory.Returns(Substitute.For<IFactory>());

            ICustomShaderStageModel model = new CustomShaderStageModel(messenger,
                                                             components,
                                                             loader,
                                                             factory,
                                                             new BlendStateConverter(),
                                                             "test.frag",
                                                             AssetSourceEnum.Embedded,
                                                             new ShaderUniformDescription[] {
                                                                new ShaderUniformDescription
                                                                {
                                                                     Name = "test",
                                                                     SizeInBytes = 32,
                                                                     UniformType = ShaderUniformType.Data
                                                                },
                                                             },
                                                             BlendState.Alpha, false);
            return model;
        }

        [Fact]
        public void CustomShaderModel_TestingUniformTypeAccess_FailOnOutOfRangeDebugReportReturnDataLabel()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var model = SetUpModelForSimpleAccessTests(messenger);
            _ = model.UserUniformType(1);

            messenger.Received(1).Report(Arg.Any<string>());
        }

        [Fact]
        public void CustomShaderModel_TestingUniformNameArrayAccess_FailOnOutOfRangeDebugReportReturnEmptyString()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var model = SetUpModelForSimpleAccessTests(messenger);

            var result = model.UserUniformName(1);

            Assert.Equal("", result);
            messenger.Received(1).Report(Arg.Any<string>());
        }

        [Fact]
        public void CustomShaderModel_TestingResourceSetAccess_FailOnOutOfRangeDebugReportReturnNull()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var model = SetUpModelForSimpleAccessTests(messenger);

            var result = model.UserUniformResourceSet(1);

            Assert.Null(result);
            messenger.Received(1).Report(Arg.Any<string>());
        }

        [Fact]
        public void CustomShaderModel_TestingResourceSetAccessByUserString_ThrowException()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var model = SetUpModelForSimpleAccessTests(messenger);

            Assert.Throws<Yak2DException>(() => { model.UserUniformResourceSet("invalid name"); });
        }

        [Fact]
        public void CustomShaderModel_TestingSetShaderValues_CallsSystemComponents()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var model = SetUpModelForSimpleAccessTests(messenger, components);

            model.SetUniformValue<float>("test", 0.5f);

            components.Device.Received(1).UpdateBuffer(Arg.Any<DeviceBuffer>(), Arg.Any<uint>(), ref Arg.Any<float>());
        }

        [Fact]
        public void CustomShaderModel_TestingSetShaderValuesArrayVersion_CallsSystemComponents()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var model = SetUpModelForSimpleAccessTests(messenger, components);

            model.SetUniformValue<float>("test", new float[] { 0.5f, 0.5f });

            components.Device.Received(1).UpdateBuffer(Arg.Any<DeviceBuffer>(), Arg.Any<uint>(), Arg.Any<float[]>());
        }

        private ResourceLayoutElementDescription[][] CaptureUniformDescriptions(bool useSpirvCompile)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();

            var idevice = Substitute.For<IDevice>();
            idevice.SwapchainFramebufferOutputDescription.Returns(new OutputDescription
            {
                ColorAttachments = new OutputAttachmentDescription[] { },
                DepthAttachment = new OutputAttachmentDescription(),
                SampleCount = NeoVeldrid.TextureSampleCount.Count1
            });
            //The SPIR-V path throws for GraphicsApi.SystemDefault (the substitute's default value)
            idevice.BackendType.Returns(GraphicsApi.OpenGL);
            components.Device.Returns(idevice);

            ResourceLayoutElementDescription[][] captured = null;
            var loader = Substitute.For<IShaderLoader>();
            loader.CreateShaderPackage(Arg.Any<string>(),
                                       Arg.Any<AssetSourceEnum>(),
                                       Arg.Any<string>(),
                                       Arg.Any<AssetSourceEnum>(),
                                       Arg.Any<VertexLayoutDescription>(),
                                       Arg.Do<ResourceLayoutElementDescription[][]>(x => captured = x),
                                       Arg.Any<bool>(),
                                       Arg.Any<bool>()).Returns(new ShaderPackage
                                       {
                                           Description = new ShaderSetDescription(),
                                           UniformResourceLayout = new ResourceLayout[] { }
                                       });

            var factory = Substitute.For<IPipelineFactory>();
            components.Factory.Returns(Substitute.For<IFactory>());

            _ = new CustomShaderStageModel(messenger,
                                           components,
                                           loader,
                                           factory,
                                           new BlendStateConverter(),
                                           "test.frag",
                                           AssetSourceEnum.Embedded,
                                           new ShaderUniformDescription[] {
                                               new ShaderUniformDescription { Name = "Tex", UniformType = ShaderUniformType.Texture },
                                               new ShaderUniformDescription { Name = "Block", SizeInBytes = 32, UniformType = ShaderUniformType.Data },
                                               new ShaderUniformDescription { Name = "Two", UniformType = ShaderUniformType.Texture },
                                           },
                                           BlendState.Alpha,
                                           useSpirvCompile);

            return captured;
        }

        private static void AssertElement(ResourceLayoutElementDescription element, string name, ResourceKind kind)
        {
            Assert.Equal(name, element.Name);
            Assert.Equal(kind, element.Kind);
        }

        [Fact]
        public void CustomShaderModel_SpirvPath_NamesTextureElementAfterTextureSoOpenGLBindsIt()
        {
            var captured = CaptureUniformDescriptions(true);

            Assert.NotNull(captured);
            Assert.Equal(3, captured.Length);

            Assert.Equal(2, captured[0].Length);
            AssertElement(captured[0][0], "Texture_Tex", ResourceKind.TextureReadOnly);
            AssertElement(captured[0][1], "Sampler_Tex", ResourceKind.Sampler);

            Assert.Single(captured[1]);
            AssertElement(captured[1][0], "Block", ResourceKind.UniformBuffer);

            Assert.Equal(2, captured[2].Length);
            AssertElement(captured[2][0], "Texture_Two", ResourceKind.TextureReadOnly);
            AssertElement(captured[2][1], "Sampler_Two", ResourceKind.Sampler);
        }

        [Fact]
        public void CustomShaderModel_PrecompiledPath_KeepsLegacyTextureElementNames()
        {
            var captured = CaptureUniformDescriptions(false);

            Assert.NotNull(captured);
            Assert.Equal(3, captured.Length);

            Assert.Equal(2, captured[0].Length);
            AssertElement(captured[0][0], "Sampler_Tex", ResourceKind.TextureReadOnly);
            AssertElement(captured[0][1], "Texture_Tex", ResourceKind.Sampler);

            Assert.Single(captured[1]);
            AssertElement(captured[1][0], "Block", ResourceKind.UniformBuffer);

            Assert.Equal(2, captured[2].Length);
            AssertElement(captured[2][0], "Sampler_Two", ResourceKind.TextureReadOnly);
            AssertElement(captured[2][1], "Texture_Two", ResourceKind.Sampler);
        }
    }
}
