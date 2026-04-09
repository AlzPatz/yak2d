using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NeoVeldrid.Sdl2;
using Silk.NET.SDL;
using Yak2D.Internal;

namespace Yak2D.Input
{
    public class InputGameController : IInputGameController
    {
        private IFrameworkMessenger _frameworkMessenger;
        private readonly IApplicationMessenger _applicationMessenger;
        private Dictionary<int, GameController> _controllers;

        private Queue<Event> _eventQueue;

        public InputGameController(IFrameworkMessenger frameworkMessenger, IApplicationMessenger applicationMessenger)
        {
            _frameworkMessenger = frameworkMessenger;
            _applicationMessenger = applicationMessenger;

            Sdl2Window.SdlInstance.Init(Silk.NET.SDL.Sdl.InitGamecontroller); //Correct?
            //Sdl2Native.SDL_Init(SDLInitFlags.GameController);

            _eventQueue = new Queue<Event>();

            _controllers = new Dictionary<int, GameController>();

            UpdateGamepadRegister();
        }

        private unsafe void UpdateGamepadRegister()
        {
            var validIds = new List<int>();

            var numJoySticks = Sdl2Window.SdlInstance.NumJoysticks();
            //var numJoySticks = Sdl2Native.SDL_NumJoysticks();

            var numGameControllers = 0;
            for (var c = 0; c < numJoySticks; c++)
            {
                if(Sdl2Window.SdlInstance.IsGameController(c) == SdlBool.True)
                //if (Sdl2Native.SDL_IsGameController(c))
                {
                    var index = c;
                    var controller = Sdl2Window.SdlInstance.GameControllerOpen(index);
                    var joystick = Sdl2Window.SdlInstance.GameControllerGetJoystick(controller);
                    var instanceId = Sdl2Window.SdlInstance.JoystickInstanceID(joystick);
                    //var controller = Sdl2Native.SDL_GameControllerOpen(index);
                    //var joystick = Sdl2Native.SDL_GameControllerGetJoystick(controller);
                    //var instanceId = Sdl2Native.SDL_JoystickInstanceID(joystick);

                    var name = Marshal.PtrToStringAnsi((IntPtr)Sdl2Window.SdlInstance.GameControllerName(controller));
                    //var name = Marshal.PtrToStringAnsi((IntPtr)Sdl2Native.SDL_GameControllerName(controller));
                    _frameworkMessenger.Report(string.Concat("Detected Controller name: ", name));

                    var gameController = new GameController(name, *controller, *joystick);
                    //var gameController = new GameController(name, controller, joystick);
                    if (_controllers.ContainsKey(instanceId))
                    {
                        if (_controllers[instanceId].Name != name)
                        {
                            _frameworkMessenger.Report(string.Concat("Warning: Controller replaced at instance id: ", instanceId, " does not have the same name as the previous controller here"));
                        }
                        _controllers.Remove(instanceId);
                    }

                    _controllers.Add(instanceId, gameController);
                    validIds.Add(instanceId);

                    numGameControllers++;
                }
            }

            var ids = _controllers.Keys.ToList();
            ids.ForEach(x =>
            {
                if (!validIds.Contains(x))
                {
                    _controllers.Remove(x);
                }
            });

            _frameworkMessenger.Report(string.Concat("Number of Joysticks detected: ", numJoySticks, " -> of those, number of GameControllers: ", numGameControllers));
        }

        private unsafe void RegisterInitialGamepads()
        {
            var numJoySticks = Sdl2Window.SdlInstance.NumJoysticks();
            //var numJoySticks = Sdl2Native.SDL_NumJoysticks();

            var numGameControllers = 0;
            for (var c = 0; c < numJoySticks; c++)
            {
                //if (Sdl2Native.SDL_IsGameController(c))
                if (Sdl2Window.SdlInstance.IsGameController(c) == SdlBool.True)
                    {
                    var index = c;
                    var controller = Sdl2Window.SdlInstance.GameControllerOpen(index);
                    var joystick = Sdl2Window.SdlInstance.GameControllerGetJoystick(controller);
                    //var controller = Sdl2Native.SDL_GameControllerOpen(index);
                    //var joystick = Sdl2Native.SDL_GameControllerGetJoystick(controller);
                    var instanceId = index; //Sdl2Native.SDL_JoystickInstanceID(joystick);
                                            //How to check for controller NULL?
                    var name = Marshal.PtrToStringAnsi((IntPtr)Sdl2Window.SdlInstance.GameControllerName(controller));
                    //var name = Marshal.PtrToStringAnsi((IntPtr)Sdl2Native.SDL_GameControllerName(controller));
                    _frameworkMessenger.Report(string.Concat("Detected Controller name: ", name));

                    if (_controllers.ContainsKey(instanceId))
                    {
                        _frameworkMessenger.Report("Cannot add controller as instanceId already exists in dictionary, skipping...");
                        continue;
                    }

                    var gameController = new GameController(name, *controller, *joystick);
                    //var gameController = new GameController(name, controller, joystick);
                    _controllers.Add(instanceId, gameController);

                    numGameControllers++;
                }
            }

            _frameworkMessenger.Report(string.Concat("Number of Joysticks detected: ", numJoySticks, " -> of those, number of GameControllers: ", numGameControllers));
        }

        public void Update(float timeStepSeconds)
        {
            foreach (var controller in _controllers.Values)
            {
                controller.PrepareForEvents();
            }
            ProcessEventCache();
            foreach (var controller in _controllers.Values)
            {
                controller.UpdateButtonTimeCounters(timeStepSeconds);
            }
        }

        public void CacheEvent(ref Event ev)
        {
            _eventQueue.Enqueue(ev);
        }

        private void ProcessEventCache()
        {
            while (_eventQueue.Count > 0)
            {
                var ev = _eventQueue.Dequeue();
                ProcessAnEvent(ref ev);
            }
        }

        private void ProcessAnEvent(ref Event ev)
        {
            int id;
            switch ((EventType)ev.Type)
            {
                case EventType.Controllerdeviceadded:
                    UpdateGamepadRegister();
                    _applicationMessenger.QueueMessage(FrameworkMessage.GamepadAdded);
                    break;
                case EventType.Controllerdeviceremoved:
                    UpdateGamepadRegister();
                    _applicationMessenger.QueueMessage(FrameworkMessage.GamepadRemoved);
                    break;
                case EventType.Controllerdeviceremapped:
                    //Unsure if am required to process this event. For future investigation
                    break;
                case EventType.Controllerbuttonup:
                case EventType.Controllerbuttondown:
                    ControllerButtonEvent buttonEvent = Unsafe.As<Event, ControllerButtonEvent>(ref ev);

                    id = buttonEvent.Which;

                    if (_controllers.ContainsKey(id))
                    {
                        var controller = _controllers[id];

                        var button = ToGamepadButton((GameControllerButton)buttonEvent.Button);

                        var pressed = buttonEvent.State == 1; //SDL_PRESSED and SDL_RELEASED don't appear to exist

                        controller.ProcessButtonEvent(button, pressed);
                    }
                    break;
                case EventType.Controlleraxismotion:
                    ControllerAxisEvent axisEvent = Unsafe.As<Event, ControllerAxisEvent>(ref ev);

                    id = axisEvent.Which;

                    if (_controllers.ContainsKey(id))
                    {
                        var controller = _controllers[id];

                        var axis = ToGamepadAxis((GameControllerAxis)axisEvent.Axis);

                        var value = NormalizeAxis(axisEvent.Value);

                        controller.ProcessAxisEvent(axis, value);
                    }
                    break;
            }
        }

        private GamepadButton ToGamepadButton(GameControllerButton button)
        {
            return (GamepadButton)button;
        }

        private GamepadAxis ToGamepadAxis(GameControllerAxis axis)
        {
            return (GamepadAxis)axis;
        }

        private float NormalizeAxis(short value)
        {
            return value < 0
                ? -(value / (float)short.MinValue)
                : (value / (float)short.MaxValue);
        }

        public bool IsGamepadIdValid(int id)
        {
            return _controllers.ContainsKey(id);
        }

        public List<int> ConnectedGamepadIds()
        {
            return _controllers.Keys.ToList();
        }

        public bool IsGamepadButtonCurrentlyPressed(int id, GamepadButton button)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].IsButtonCurrentlyPressed(button);
            }
            return false;
        }

        public bool IsGamepadButtonPressedThisFrame(int id, GamepadButton button)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].IsButtonPressedThisFrame(button);
            }
            return false;
        }

        public bool WasGamepadButtonReleasedThisFrame(int id, GamepadButton button)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].WasButtonReleasedThisFrame(button);
            }
            return false;
        }

        public float HowLongHasGamepadButtonBeenHeldDown(int id, GamepadButton button, bool countIfUpThisFrame = false)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].HowLongHasButtonBeenHeldDown(button, countIfUpThisFrame);
            }
            return 0.0f;
        }

        public List<GamepadButton> GamepadButtonsPressedThisFrame(int id)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].ButtonsPressedThisFrame();
            }
            return new List<GamepadButton> { };
        }

        public List<GamepadButton> GamepadButtonsHeldDown(int id)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].ButtonsHeldDown();
            }
            return new List<GamepadButton> { };
        }

        public List<GamepadButton> GamepadButtonsReleasedThisFrame(int id)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].ButtonsReleasedThisFrame();
            }
            return new List<GamepadButton> { };
        }

        public float GamepadAxisValue(int id, GamepadAxis axis)
        {
            if (IsGamepadIdValid(id))
            {
                return _controllers[id].AxisValue(axis);
            }
            return 0.0f;
        }
    }
}
