using Veldrid.Sdl2;
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

        private void ProcessEvent(ref SDL_Event ev)
        {
            switch (ev.type)
            {
                case SDL_EventType.ControllerDeviceAdded:
                case SDL_EventType.ControllerDeviceRemoved:
                case SDL_EventType.ControllerDeviceRemapped:
                case SDL_EventType.ControllerButtonUp:
                case SDL_EventType.ControllerButtonDown:
                case SDL_EventType.ControllerAxisMotion:
                    _inputGameController.CacheEvent(ref ev);
                    break;
                
                    //Currently Veldrid Snapshot is used for all events. 
                    //However here is an attempt to faster sample mouse position
                case SDL_EventType.MouseMotion:
                    _inputMouseAndKeyboard.CacheEvent(ref ev);
                    break;

                case SDL_EventType.Quit:
                case SDL_EventType.Terminating:
                    _coreMessenger.QueueMessage(CoreMessage.Shutdown);
                    break;
                case SDL_EventType.LowMemory:
                    _applicationMessenger.QueueMessage(FrameworkMessage.LowMemoryReported);
                    break;

                case SDL_EventType.RenderDeviceReset:
                case SDL_EventType.RenderTargetsReset:
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