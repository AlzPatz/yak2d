using System;
using Veldrid;

namespace Yak2D.Internal
{
    public interface IDevice
    {
        GraphicsApi BackendType { get; }
        Framebuffer SwapchainFramebuffer { get; }
        OutputDescription SwapchainFramebufferOutputDescription { get; }
        bool SamplerAnisotropy { get; }
        GraphicsDevice RawVeldridDevice { get; }
        bool SyncToVerticalBlank { get; set; }

        void UpdateTexture(Texture texture,
                           IntPtr source,
                           uint sizeInBytes,
                           uint x,
                           uint y,
                           uint z,
                           uint width,
                           uint height,
                           uint depth,
                           uint mipLevel,
                           uint arrayLayer);

        void UpdateBuffer<T>(DeviceBuffer buffer,
                             uint bufferOffsetInBytes,
                             ref T source) where T : struct;

        void UpdateBuffer<T>(DeviceBuffer buffer,
                     uint bufferOffsetInBytes,
                     T[] source) where T : struct;

        void UpdateBuffer<T>(DeviceBuffer buffer,
                             uint bufferOffsetInBytes,
                             ref T source,
                             uint sizeInBytes) where T : struct;

        void UpdateBuffer(DeviceBuffer buffer,
                          uint bufferOffsetInBytes,
                          IntPtr source,
                          uint sizeInBytes);
        void SubmitCommands(CommandList cl);

        void WaitForIdle();
        void SwapBuffers();
        void Dispose();
        void ResizeMainWindow(uint windowResolutionWidth, uint windowResolutionHeight);
    }
}