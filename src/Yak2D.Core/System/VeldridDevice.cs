using System;
using System.Threading;
using NeoVeldrid;
using Yak2D.Internal;
using Yak2D.Utility;

namespace Yak2D.Core
{
    public class VeldridDevice : IDevice
    {
        public GraphicsDevice RawVeldridDevice { get { return _device; } }
        private GraphicsDevice _device;

        //Graphics.Render presents on a thread-pool task (WaitForIdle + SwapBuffers) while the main thread keeps pumping
        //SDL events, and a window resize event re-creates the swapchain from the main thread. A present that returns
        //out-of-date also re-creates the swapchain, so without this both threads could be in vkCreateSwapchainKHR on
        //the same surface at once (Vulkan requires the surface and old swapchain to be externally synchronised), which
        //segfaulted inside the NVIDIA driver during a window drag. Every swapchain-touching call takes this lock.
        private readonly object _swapchainLock = new object();

        public Framebuffer SwapchainFramebuffer => _device?.SwapchainFramebuffer;
        public OutputDescription SwapchainFramebufferOutputDescription => _device.SwapchainFramebuffer.OutputDescription;
        public GraphicsApi BackendType => GraphicsApiConverter.ConvertVeldridGraphicsBackendToApi(_device.BackendType);
        public bool SamplerAnisotropy => _device.Features.SamplerAnisotropy;

        public bool SyncToVerticalBlank { get { return _device.SyncToVerticalBlank; } set { _device.SyncToVerticalBlank = value; } }

        public VeldridDevice(GraphicsDevice device)
        {
            _device = device;
        }

        public void UpdateTexture(Texture texture,
                             IntPtr source,
                             uint sizeInBytes,
                             uint x,
                             uint y,
                             uint z,
                             uint width,
                             uint height,
                             uint depth,
                             uint mipLevel,
                             uint arrayLayer) => _device?.UpdateTexture(texture, source, sizeInBytes, x, y, z, width, height, depth, mipLevel, arrayLayer);

        public void UpdateBuffer<T>(DeviceBuffer buffer,
                            uint bufferOffsetInBytes,
                            ref T source) where T : unmanaged => _device?.UpdateBuffer<T>(buffer, bufferOffsetInBytes, ref source);

        public void UpdateBuffer<T>(DeviceBuffer buffer,
                                    uint bufferOffsetInBytes,
                                    T[] source) where T : unmanaged
                            => _device?.UpdateBuffer<T>(buffer, bufferOffsetInBytes, source);

        public void UpdateBuffer<T>(DeviceBuffer buffer,
                                    uint bufferOffsetInBytes,
                                    ref T source,
                                    uint sizeInBytes) where T : unmanaged
                            => _device?.UpdateBuffer<T>(buffer, bufferOffsetInBytes, ref source, sizeInBytes);

        public void UpdateBuffer(DeviceBuffer buffer,
                                 uint bufferOffsetInBytes,
                                 IntPtr source,
                                 uint sizeInBytes)
                            => _device?.UpdateBuffer(buffer, bufferOffsetInBytes, source, sizeInBytes);

        public void SubmitCommands(CommandList cl) => _device?.SubmitCommands(cl);

        public void SwapBuffers()
        {
            lock (_swapchainLock)
            {
                _device?.SwapBuffers();
            }
        }

        public void WaitForIdle()
        {
            lock (_swapchainLock)
            {
                _device?.WaitForIdle();
            }
        }

        //Bounded variant for shutdown: if a present is stuck holding the swapchain lock (e.g. vkAcquireNextImageKHR
        //blocked after the window was destroyed), give up after the timeout instead of blocking forever
        public bool TryWaitForIdle(TimeSpan timeout)
        {
            if (!Monitor.TryEnter(_swapchainLock, timeout))
            {
                return false;
            }

            try
            {
                _device?.WaitForIdle();
                return true;
            }
            finally
            {
                Monitor.Exit(_swapchainLock);
            }
        }

        //Test seam only (exposed to Yak2D.Tests via InternalsVisibleTo)
        internal object SwapchainLock => _swapchainLock;

        public void Dispose() => _device?.Dispose();

        public void ResizeMainWindow(uint windowResolutionWidth, uint windowResolutionHeight)
        {
            lock (_swapchainLock)
            {
                _device.ResizeMainWindow(windowResolutionWidth, windowResolutionHeight);
            }
        }
    }
}