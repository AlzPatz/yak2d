using System;
using System.Threading;
using NeoVeldrid;
using Yak2D.Internal;

namespace Yak2D.Tests.ManualFakes
{
    //GPU-free IDevice whose present (WaitForIdle + SwapBuffers) is gated by events, so tests can hold a frame "in flight"
    public class FakePresentDevice : IDevice
    {
        public ManualResetEventSlim PresentEntered { get; } = new ManualResetEventSlim(false);
        public ManualResetEventSlim ReleasePresent { get; } = new ManualResetEventSlim(false);
        //Set once SwapBuffers has finished (or thrown): nothing in the present touches the fake after this
        public ManualResetEventSlim PresentExited { get; } = new ManualResetEventSlim(false);

        public volatile bool ThrowOnSwapBuffers;

        private int _lastPresentThreadId;
        private int _presentCount;

        public int LastPresentThreadId => Volatile.Read(ref _lastPresentThreadId);
        public int PresentCount => Volatile.Read(ref _presentCount);

        //Only call when no present is waiting on ReleasePresent (the previous one has exited)
        public void ResetForNextFrame()
        {
            PresentEntered.Reset();
            ReleasePresent.Reset();
            PresentExited.Reset();
        }

        //Never SystemDefault (default enum 0 throws on the SPIR-V path)
        public GraphicsApi BackendType => GraphicsApi.Vulkan;
        public Framebuffer SwapchainFramebuffer => null;
        public OutputDescription SwapchainFramebufferOutputDescription => default;
        public bool SamplerAnisotropy => true;
        public GraphicsDevice RawVeldridDevice => null;
        public bool SyncToVerticalBlank { get; set; }

        public void UpdateTexture(Texture texture, IntPtr source, uint sizeInBytes, uint x, uint y, uint z, uint width, uint height, uint depth, uint mipLevel, uint arrayLayer) { }
        public void UpdateBuffer<T>(DeviceBuffer buffer, uint bufferOffsetInBytes, ref T source) where T : unmanaged { }
        public void UpdateBuffer<T>(DeviceBuffer buffer, uint bufferOffsetInBytes, T[] source) where T : unmanaged { }
        public void UpdateBuffer<T>(DeviceBuffer buffer, uint bufferOffsetInBytes, ref T source, uint sizeInBytes) where T : unmanaged { }
        public void UpdateBuffer(DeviceBuffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes) { }
        public void SubmitCommands(CommandList cl) { }

        public void WaitForIdle()
        {
            Volatile.Write(ref _lastPresentThreadId, Environment.CurrentManagedThreadId);
            PresentEntered.Set();
            ReleasePresent.Wait();
        }

        public bool TryWaitForIdle(TimeSpan timeout) => true;

        public void SwapBuffers()
        {
            try
            {
                Interlocked.Increment(ref _presentCount);
                if (ThrowOnSwapBuffers)
                {
                    throw new InvalidOperationException("injected present fault");
                }
            }
            finally
            {
                PresentExited.Set();
            }
        }

        public void Dispose() { }
        public void ResizeMainWindow(uint windowResolutionWidth, uint windowResolutionHeight) { }
    }
}
