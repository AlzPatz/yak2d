using System;
using System.Diagnostics;
using System.Threading;
using Xunit;
using Yak2D.Core;

namespace Yak2D.Tests
{
    public class VeldridDeviceTest
    {
        [Fact]
        public void VeldridDevice_TryWaitForIdle_ReturnsWithinTwoSecondsWhileSwapchainLockHeldElsewhere()
        {
            //Null raw device: every device call is null-conditional, so no GPU is needed to exercise the lock
            var device = new VeldridDevice(null);

            using var lockHeld = new ManualResetEventSlim(false);
            using var release = new ManualResetEventSlim(false);

            //Stands in for a render task stuck in SwapBuffers (vkAcquireNextImageKHR) with the swapchain lock held
            var holder = new Thread(() =>
            {
                Monitor.Enter(device.SwapchainLock);
                try
                {
                    lockHeld.Set();
                    release.Wait();
                }
                finally
                {
                    Monitor.Exit(device.SwapchainLock);
                }
            });
            holder.IsBackground = true;
            holder.Start();

            lockHeld.Wait();

            var sw = Stopwatch.StartNew();
            var result = device.TryWaitForIdle(TimeSpan.FromMilliseconds(500));
            sw.Stop();

            release.Set();
            holder.Join();

            Assert.False(result);
            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(2), $"TryWaitForIdle took {sw.Elapsed.TotalMilliseconds}ms");

            //Lock now free: the wait succeeds
            Assert.True(device.TryWaitForIdle(TimeSpan.FromMilliseconds(500)));
        }
    }
}