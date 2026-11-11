using System;
using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Graphics;
using Yak2D.Internal;
using Yak2D.Surface;
using Yak2D.Tests.ManualFakes;
using GraphicsEngine = Yak2D.Graphics.Graphics;

namespace Yak2D.Tests
{
    //No GPU device and no window: FakePresentDevice gates the present task so a frame can be held "in flight".
    //Tests call PrepareForDrawing + StartPresent (the present half of Render) and drive TryCompleteFrame as Core's loop does.
    //All waits are event-driven; the 10 s timeouts are upper bounds only
    public class GraphicsTest
    {
        private static readonly TimeSpan Bound = TimeSpan.FromSeconds(10);

        private static GraphicsEngine CreateGraphics(FakePresentDevice device,
                                                     IFrameworkMessenger messenger,
                                                     IGpuSurfaceManager surfaces = null,
                                                     ICameraManager cameras = null,
                                                     ISystemComponents components = null,
                                                     IRenderCommandQueue queue = null,
                                                     IRenderStageManager stages = null)
        {
            components ??= Substitute.For<ISystemComponents>();
            components.Device.Returns(device);
            components.Window.Exists.Returns(true);

            if (queue == null)
            {
                queue = Substitute.For<IRenderCommandQueue>();
                queue.FlushCallbackStageIds().Returns(Array.Empty<ulong>());
            }

            return new GraphicsEngine(messenger,
                                      Substitute.For<IStartupPropertiesCache>(),
                                      components,
                                      queue,
                                      Substitute.For<ICommandProcessor>(),
                                      stages ?? Substitute.For<IRenderStageManager>(),
                                      Substitute.For<IRenderStageVisitor>(),
                                      surfaces ?? Substitute.For<IGpuSurfaceManager>(),
                                      Substitute.For<IViewportManager>(),
                                      Substitute.For<IFontManager>(),
                                      cameras ?? Substitute.For<ICameraManager>(),
                                      Substitute.For<IFrameworkDebugOverlay>());
        }

        private static GpuSurfaceManager CreateSurfaceManager(IIdGenerator ids = null)
        {
            var props = Substitute.For<IStartupPropertiesCache>();
            props.User.Returns(Substitute.For<StartupConfig>());

            return new GpuSurfaceManager(Substitute.For<IApplicationAssembly>(),
                                         Substitute.For<IFontsAssembly>(),
                                         Substitute.For<ISurfaceAssembly>(),
                                         Substitute.For<IFrameworkMessenger>(),
                                         ids ?? Substitute.For<IIdGenerator>(),
                                         props,
                                         Substitute.For<IGpuSurfaceCollection>(),
                                         Substitute.For<IImageLoader>(),
                                         Substitute.For<IGpuSurfaceFactory>(),
                                         Substitute.For<ISystemComponents>(),
                                         Substitute.For<IFileSystem>());
        }

        //One frame: present held in flight while whileInFlight runs on this thread, then released and completed here
        private static void RunFrame(GraphicsEngine graphics,
                                     FakePresentDevice device,
                                     Action whileInFlight = null,
                                     Action afterPresentBeforeComplete = null)
        {
            device.ResetForNextFrame();
            graphics.PrepareForDrawing();
            graphics.StartPresent();

            Assert.True(device.PresentEntered.Wait(Bound));
            whileInFlight?.Invoke();
            Assert.False(graphics.TryCompleteFrame());

            device.ReleasePresent.Set();
            Assert.True(graphics.WaitForRenderingToComplete(Bound));

            afterPresentBeforeComplete?.Invoke();

            Assert.True(graphics.TryCompleteFrame());
            Assert.True(graphics.RenderingComplete);
        }

        [Fact]
        public void Graphics_PostRenderWork_RunsOnCallingThreadNotPresentTask()
        {
            var device = new FakePresentDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaces = Substitute.For<IGpuSurfaceManager>();
            var queue = Substitute.For<IRenderCommandQueue>();
            var stages = Substitute.For<IRenderStageManager>();
            var copyStage = Substitute.For<ISurfaceCopyStageModel>();

            var callbackThreadId = -1;
            var destructionThreadId = -1;

            queue.FlushCallbackStageIds().Returns(new ulong[] { 7UL });
            stages.RetrieveStageModel(7UL).Returns(copyStage);
            copyStage.When(c => c.CopyDataFromStagingTextureAndPassToUser()).Do(_ => callbackThreadId = Environment.CurrentManagedThreadId);
            surfaces.When(s => s.ProcessPendingDestruction()).Do(_ => destructionThreadId = Environment.CurrentManagedThreadId);

            var graphics = CreateGraphics(device, messenger, surfaces: surfaces, queue: queue, stages: stages);

            RunFrame(graphics, device, afterPresentBeforeComplete: () =>
            {
                //The present task has ended, but nothing post-render ran inside it
                copyStage.DidNotReceive().CopyDataFromStagingTextureAndPassToUser();
                surfaces.DidNotReceive().ProcessPendingDestruction();
            });

            copyStage.Received(1).CopyDataFromStagingTextureAndPassToUser();
            surfaces.Received(1).ProcessPendingDestruction();

            var testThreadId = Environment.CurrentManagedThreadId;
            Assert.Equal(testThreadId, callbackThreadId);
            Assert.Equal(testThreadId, destructionThreadId);
            Assert.NotEqual(device.LastPresentThreadId, callbackThreadId);
            Assert.NotEqual(device.LastPresentThreadId, destructionThreadId);
        }

        [Fact]
        public void Graphics_ResizeBurstWhileRenderInFlight_EveryFrameCompletesWithoutThrowing()
        {
            var device = new FakePresentDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaceManager = CreateSurfaceManager();
            var graphics = CreateGraphics(device, messenger, surfaces: surfaceManager);

            var exception = Record.Exception(() =>
            {
                for (var n = 0; n < 60; n++)
                {
                    //What Display.ProcessWindowReSize does on each resize: DestroySurface(MainSwapChainFrameBufferKey)
                    RunFrame(graphics, device, () => surfaceManager.RegisterSwapChainOutput(null, true));
                }
            });

            Assert.Null(exception);
            Assert.Equal(60, device.PresentCount);
        }

        [Fact]
        public void Graphics_PresentThrows_FrameCompletesMessengerToldNextFrameRenders()
        {
            var device = new FakePresentDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var graphics = CreateGraphics(device, messenger);

            device.ThrowOnSwapBuffers = true;
            RunFrame(graphics, device);

            Assert.True(graphics.RenderingComplete);
            messenger.Received().Report(Arg.Is<string>(s => s.Contains("injected present fault")));

            device.ThrowOnSwapBuffers = false;
            RunFrame(graphics, device);

            Assert.Equal(2, device.PresentCount);
        }

        [Fact]
        public void Graphics_PostRenderThrows_FrameCompletesMessengerToldNextFrameRenders()
        {
            var device = new FakePresentDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var surfaces = Substitute.For<IGpuSurfaceManager>();

            var first = true;
            surfaces.When(s => s.ProcessPendingDestruction()).Do(_ =>
            {
                if (first)
                {
                    first = false;
                    throw new InvalidOperationException("injected post-render fault");
                }
            });

            var graphics = CreateGraphics(device, messenger, surfaces: surfaces);

            RunFrame(graphics, device);

            Assert.True(graphics.RenderingComplete);
            messenger.Received().Report(Arg.Is<string>(s => s.Contains("injected post-render fault")));

            RunFrame(graphics, device);

            surfaces.Received(2).ProcessPendingDestruction();
            Assert.Equal(2, device.PresentCount);
        }

        [Fact]
        public void Graphics_UserDestroysSurfacesAndCameraEveryFrame_NothingThrowsEveryFrameCompletes()
        {
            var device = new FakePresentDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();

            ulong counter = 0;
            var ids = Substitute.For<IIdGenerator>();
            ids.New().Returns(_ => ++counter);

            var cameraFactory = Substitute.For<ICameraFactory>();
            cameraFactory.CreateCamera2D(Arg.Any<uint>(), Arg.Any<uint>(), Arg.Any<float>()).Returns(_ => Substitute.For<ICameraModel2D>());

            var surfaceManager = CreateSurfaceManager(ids);
            var cameraManager = new CameraManager(cameraFactory, ids, new SimpleDictionaryCollectionFactory(messenger));

            var graphics = CreateGraphics(device, messenger, surfaces: surfaceManager, cameras: cameraManager);

            var exception = Record.Exception(() =>
            {
                for (var n = 0; n < 100; n++)
                {
                    var camera = cameraManager.CreateCamera2D();
                    Assert.NotNull(camera);

                    RunFrame(graphics, device, () =>
                    {
                        cameraManager.DestroyCamera(camera.Id);
                        surfaceManager.DestroySurface(ids.New());
                        surfaceManager.DestroySurface(ids.New());
                    });
                }
            });

            Assert.Null(exception);
            Assert.Equal(0, cameraManager.Count2D);
            Assert.Equal(100, device.PresentCount);
        }

        [Fact]
        public void Graphics_ReinitialiseWhileRenderInFlight_GateReopensAndNextFrameRenders()
        {
            var device = new FakePresentDevice();
            var messenger = Substitute.For<IFrameworkMessenger>();
            var components = Substitute.For<ISystemComponents>();
            var graphics = CreateGraphics(device, messenger, components: components);

            components.CurrentlyReinitialisingDevices.Returns(true);

            device.ResetForNextFrame();
            graphics.PrepareForDrawing();
            graphics.StartPresent();
            Assert.True(device.PresentEntered.Wait(Bound));
            Assert.False(graphics.TryCompleteFrame());

            //What GraphicsResourceReinitialiser.ReInitialise does on the main thread during device re-creation.
            //The present on the old device is still blocked and is never released before these asserts
            graphics.ReInitalise();

            Assert.True(graphics.RenderingComplete);
            Assert.True(graphics.TryCompleteFrame());
            //Bounded shutdown no longer waits on the abandoned present
            Assert.True(graphics.WaitForRenderingToComplete(TimeSpan.FromMilliseconds(100)));

            components.CurrentlyReinitialisingDevices.Returns(false);

            //Let the abandoned present finish before the fake's events are reused
            device.ReleasePresent.Set();
            Assert.True(device.PresentExited.Wait(Bound));

            RunFrame(graphics, device);

            Assert.Equal(2, device.PresentCount);
        }
    }
}
