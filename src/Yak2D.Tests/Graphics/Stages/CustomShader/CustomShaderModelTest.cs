using NSubstitute;
using Veldrid;
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
                SampleCount = TextureSampleCount.Count1
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
                                                             BlendState.Alpha);
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
    }
}
