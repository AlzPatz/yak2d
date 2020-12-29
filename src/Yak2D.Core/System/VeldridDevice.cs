using System;
using Veldrid;
using Yak2D.Internal;
using Yak2D.Utility;

namespace Yak2D.Core
{
    public class VeldridDevice : IDevice
    {
        public GraphicsDevice RawVeldridDevice { get { return _device; } }
        private GraphicsDevice _device;

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

        public void SwapBuffers() => _device?.SwapBuffers();

        public void WaitForIdle() => _device?.WaitForIdle();

        public void Dispose() => _device?.Dispose();

        public void ResizeMainWindow(uint windowResolutionWidth, uint windowResolutionHeight) => _device.ResizeMainWindow(windowResolutionWidth, windowResolutionHeight);
    }
}