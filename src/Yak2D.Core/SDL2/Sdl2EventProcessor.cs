using NeoVeldrid.Sdl2;
using Silk.NET.SDL;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class Sdl2EventProcessor : ISdl2EventProcessor
    {
        private readonly ICoreMessenger _coreMessenger;
        private readonly IApplicationMessenger _applicationMessenger;
        private readonly IInputGameController _inputGameController;
        private readonly IInputMouseKeyboard _inputMouseAndKeyboard;

        public Sdl2EventProcessor(ICoreMessenger coreMessenger,
                                  IApplicationMessenger applicationMessenger,
                                  IInputGameController inputGameController,
                                  IInputMouseKeyboard inputMouseAndKeyboard)
        {
            _coreMessenger = coreMessenger;
            _applicationMessenger = applicationMessenger;
            _inputGameController = inputGameController;
            _inputMouseAndKeyboard = inputMouseAndKeyboard;

            Sdl2Events.Subscribe(ProcessEvent);
        }

        public void ProcessEvents()
        {
            Sdl2Events.ProcessEvents();
        }

        private void ProcessEvent(ref Event ev)
        {
            switch ((EventType)ev.Type)
            {
                case EventType.Controllerdeviceadded:
                case EventType.Controllerdeviceremapped:
                case EventType.Controllerbuttonup:
                case EventType.Controllerbuttondown:
                case EventType.Controlleraxismotion:
                    _inputGameController.CacheEvent(ref ev);
                    break;

                //Currently Veldrid Snapshot is used for all events. 
                //However here is an attempt to faster sample mouse position
                case EventType.Mousemotion:
                    _inputMouseAndKeyboard.CacheEvent(ref ev);
                    break;

                case EventType.Quit:
                case EventType.AppTerminating:
                    _coreMessenger.QueueMessage(CoreMessage.Shutdown);
                    break;
                case EventType.AppLowmemory:
                    _applicationMessenger.QueueMessage(FrameworkMessage.LowMemoryReported);
                    break;

                case EventType.RenderDeviceReset:
                case EventType.RenderTargetsReset:
                    _coreMessenger.QueueMessage(CoreMessage.DeviceOrRenderTargetsReset);
                    break;
            }
        }

        public void Shutdown()
        {
            Sdl2Events.Unsubscribe(ProcessEvent);
        }
    }
}