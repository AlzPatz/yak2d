using System;
using Yak2D.Internal;

namespace Yak2D.Tests.ManualFakes
{
    public class FakeComponents : ISystemComponents
    {
        public IWindow Window => throw new NotImplementedException();
        public IDevice Device { get; private set; }
        public IFactory Factory { get; private set; }

        private FakeGraphicsDevice _fakeDevice;
        private FakeFactory _fakeFactory;

        public FakeComponents()
        {
            _fakeDevice = new FakeGraphicsDevice();

            _fakeFactory = new FakeFactory(_fakeDevice.Device);

            Device = _fakeDevice;
            Factory = _fakeFactory;
        }

        public bool CurrentlyReinitialisingDevices => false;

        public GraphicsApi GraphicsApi => GraphicsApi.SystemDefault;

        public TexturePixelFormat SwapChainFramebufferPixelFormat => TexturePixelFormat.B8_G8_R8_A8_UNorm;

        public TextureSampleCount SwapChainFramebufferSampleCount => TextureSampleCount.X1;

        public bool IsGraphicsApiSupported(GraphicsApi api) => true;

        public void RecreateDeviceAndReinitialiseAllResources(Action systemPreAppReinitialisation) { }

        public void ReleaseResources()
        {
            _fakeDevice?.Dispose();
            _fakeFactory?.Dispose();
        }

        public void SetGraphicsApi(GraphicsApi api, Action systemPreAppReinitialisation) { }
    }
}