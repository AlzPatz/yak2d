using System;

namespace Yak2D.Internal
{
    public interface ISystemComponents
    {
        bool CurrentlyReinitialisingDevices { get; }
        IWindow Window { get;  }
        IDevice Device { get;  }
        IFactory Factory { get; }
        GraphicsApi GraphicsApi { get; }
        TexturePixelFormat SwapChainFramebufferPixelFormat { get; }
        TextureSampleCount SwapChainFramebufferSampleCount { get; }
        bool IsGraphicsApiSupported(GraphicsApi api);
        void SetGraphicsApi(GraphicsApi api, Action systemPreAppReinitialisation);
        void RecreateDeviceAndReinitialiseAllResources(Action systemPreAppReinitialisation);
        void ReleaseResources();
    }
}