using System;
using Veldrid;
using Veldrid.StartupUtilities;
using Yak2D.Internal;

namespace Yak2D.Tests.ManualFakes
{
    public class FakeGraphicsDevice : IDevice
    {
        public GraphicsDevice Device { get; private set; }
        public void Dispose()
        {
            Device?.Dispose();
        }

        public FakeGraphicsDevice()
        {
            var backend = VeldridStartup.GetPlatformDefaultBackend();

            switch (backend)
            {
                case GraphicsBackend.Direct3D11:
                    Device = GraphicsDevice.CreateD3D11(new GraphicsDeviceOptions
                    {
                        Debug = true,
                        HasMainSwapchain = false,
                        PreferDepthRangeZeroToOne = true,
                        PreferStandardClipSpaceYDirection = true,
                        ResourceBindingModel = ResourceBindingModel.Improved,
                        SwapchainDepthFormat = PixelFormat.R16_UNorm,
                        SwapchainSrgbFormat = false,
                        SyncToVerticalBlank = true
                    });
                    break;
                case GraphicsBackend.OpenGL:
                    var window = VeldridStartup.CreateWindow(new WindowCreateInfo
                    {
                        WindowWidth = 100,
                        WindowHeight = 100,
                        WindowInitialState = WindowState.Normal,
                        WindowTitle = "Open GL Test Window - Unable to initialise headless",
                        X = 200,
                        Y = 200
                    });

                    Device = VeldridStartup.CreateGraphicsDevice(window, new GraphicsDeviceOptions
                    {
                        Debug = true,
                        HasMainSwapchain = false,
                        PreferDepthRangeZeroToOne = true,
                        PreferStandardClipSpaceYDirection = true,
                        ResourceBindingModel = ResourceBindingModel.Improved,
                        SwapchainDepthFormat = PixelFormat.R16_UNorm,
                        SwapchainSrgbFormat = false,
                        SyncToVerticalBlank = true
                    }, GraphicsBackend.OpenGL);

                    break;
                case GraphicsBackend.Metal:
                    Device = GraphicsDevice.CreateMetal(new GraphicsDeviceOptions
                    {
                        Debug = true,
                        HasMainSwapchain = false,
                        PreferDepthRangeZeroToOne = true,
                        PreferStandardClipSpaceYDirection = true,
                        ResourceBindingModel = ResourceBindingModel.Improved,
                        SwapchainDepthFormat = PixelFormat.R16_UNorm,
                        SwapchainSrgbFormat = false,
                        SyncToVerticalBlank = true
                    });
                    break;
                case GraphicsBackend.Vulkan:
                    Device = GraphicsDevice.CreateVulkan(new GraphicsDeviceOptions
                    {
                        Debug = true,
                        HasMainSwapchain = false,
                        PreferDepthRangeZeroToOne = true,
                        PreferStandardClipSpaceYDirection = true,
                        ResourceBindingModel = ResourceBindingModel.Improved,
                        SwapchainDepthFormat = PixelFormat.R16_UNorm,
                        SwapchainSrgbFormat = false,
                        SyncToVerticalBlank = true
                    });
                    break;
                case GraphicsBackend.OpenGLES:
                    throw new InvalidProgramException("OpenGL ES not supported");
            }

            if (Device == null)
            {
                throw new NullReferenceException("Fake Veldrid Components was unable to create Graphics Device");
            }
        }

        public GraphicsApi BackendType => GraphicsApi.SystemDefault;
        public Framebuffer SwapchainFramebuffer => Device.SwapchainFramebuffer;
        public bool SamplerAnisotropy => true;

        public GraphicsDevice RawVeldridDevice => throw new NotImplementedException();

        public OutputDescription SwapchainFramebufferOutputDescription => throw new NotImplementedException();

        public bool SyncToVerticalBlank { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void UpdateBuffer<T>(DeviceBuffer buffer, uint bufferOffsetInBytes, ref T source) where T : struct { }
        public void UpdateBuffer<T>(DeviceBuffer buffer, uint bufferOffsetInBytes, T[] source) where T : struct { }
        public void UpdateBuffer<T>(DeviceBuffer buffer, uint bufferOffsetInBytes, ref T source, uint sizeInBytes) where T : struct { }
        public void UpdateBuffer(DeviceBuffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes) { }
        public void UpdateTexture(Texture texture, IntPtr source, uint sizeInBytes, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) { }

        public void SubmitCommands(CommandList cl)
        {
            throw new NotImplementedException();
        }

        public void WaitForIdle()
        {
            throw new NotImplementedException();
        }

        public void SwapBuffers()
        {
            throw new NotImplementedException();
        }

        public void ResizeMainWindow(uint windowResolutionWidth, uint windowResolutionHeight)
        {
            throw new NotImplementedException();
        }
    }
}