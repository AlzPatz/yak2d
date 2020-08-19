using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Veldrid.Sdl2;
using Yak2D.Internal;

namespace Yak2D.Input
{
    public class InputGameController : IInputGameController
    {
        private IFrameworkMessenger _frameworkMessenger;
        private readonly IApplicationMessenger _applicationMessenger;
        private Dictionary<int, GameController> _controllers;

        private Queue<SDL_Event> _eventQueue;

        public InputGameController(IFrameworkMessenger frameworkMessenger, IApplicationMessenger applicationMessenger)
        {
            _frameworkMessenger = frameworkMessenger;
            _applicationMessenger = applicationMessenger;

            Sdl2Native.SDL_Init(SDLInitFlags.GameController);

            _eventQueue = new Queue<SDL_Event>();

            _controllers = new Dictionary<int, GameController>();

            UpdateGamepadRegister();
        }

        private unsafe void UpdateGamepadRegister()
        {
            var validIds = new List<int>();

            var numJoySticks = Sdl2Native.SDL_NumJoysticks();

            var numGameControllers = 0;
            for (var c = 0; c < numJoySticks; c++)
            {
                if (Sdl2Native.SDL_IsGameController(c))
                {
                    var index = c;
                    var controller = Sdl2Native.SDL_GameControllerOpen(index);
                    var joystick = Sdl2Native.SDL_GameControllerGetJoystick(controller);
                    var instanceId = Sdl2Native.SDL_JoystickInstanceID(joystick);

                    var name = Marshal.PtrToStringAnsi((IntPtr)Sdl2Native.SDL_GameControllerName(controller));
                    _frameworkMessenger.Report(string.Concat("Detected Controller name: ", name));

                    var gameController = new GameController(name, controller, joystick);
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
            var numJoySticks = Sdl2Native.SDL_NumJoysticks();

            var numGameControllers = 0;
            for (var c = 0; c < numJoySticks; c++)
            {
                if (Sdl2Native.SDL_IsGameController(c))
                {
                    var index = c;
                    var controller = Sdl2Native.SDL_GameControllerOpen(index);
                    var joystick = Sdl2Native.SDL_GameControllerGetJoystick(controller);
                    var instanceId = index; //Sdl2Native.SDL_JoystickInstanceID(joystick);
                                            //How to check for controller NULL?
                    var name = Marshal.PtrToStringAnsi((IntPtr)Sdl2Native.SDL_GameControllerName(controller));
                    _frameworkMessenger.Report(string.Concat("Detected Controller name: ", name));

                    if (_controllers.ContainsKey(instanceId))
                    {
                        _frameworkMessenger.Report("Cannot add controller as instanceId already exists in dictionary, skipping...");
                        continue;
                    }

                    var gameController = new GameController(name, controller, joystick);
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

        public void CacheEvent(ref SDL_Event ev)
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

        private void ProcessAnEvent(ref SDL_Event ev)
        {
            int id;
            switch (ev.type)
            {
                case SDL_EventType.ControllerDeviceAdded:
                    UpdateGamepadRegister();
                    _applicationMessenger.QueueMessage(FrameworkMessage.GamepadAdded);
                    break;
                case SDL_EventType.ControllerDeviceRemoved:
                    UpdateGamepadRegister();
                    _applicationMessenger.QueueMessage(FrameworkMessage.GamepadRemoved);
                    break;
                case SDL_EventType.ControllerDeviceRemapped:
                    //Unsure if am required to process this event. For future investigation
                    break;
                case SDL_EventType.ControllerButtonUp:
                case SDL_EventType.ControllerButtonDown:
                    SDL_ControllerButtonEvent buttonEvent = Unsafe.As<SDL_Event, SDL_ControllerButtonEvent>(ref ev);

                    id = buttonEvent.which;

                    if (_controllers.ContainsKey(id))
                    {
                        var controller = _controllers[id];

                        var button = ToGamepadButton(buttonEvent.button);

                        var pressed = buttonEvent.state == 1; //SDL_PRESSED and SDL_RELEASED don't appear to exist

                        controller.ProcessButtonEvent(button, pressed);
                    }
                    break;
                case SDL_EventType.ControllerAxisMotion:
                    SDL_ControllerAxisEvent axisEvent = Unsafe.As<SDL_Event, SDL_ControllerAxisEvent>(ref ev);

                    id = axisEvent.which;

                    if (_controllers.ContainsKey(id))
                    {
                        var controller = _controllers[id];

                        var axis = ToGamepadAxis(axisEvent.axis);

                        var value = NormalizeAxis(axisEvent.value);

                        controller.ProcessAxisEvent(axis, value);
                    }
                    break;
            }
        }

        private GamepadButton ToGamepadButton(SDL_GameControllerButton button)
        {
            return (GamepadButton)button;
        }

        private GamepadAxis ToGamepadAxis(SDL_GameControllerAxis axis)
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
