using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid.Sdl2;

namespace Yak2D.Input
{
    public class GameController
    {
        public string Name { get; private set; }
        public SDL_GameController Controller { get; private set; }
        public SDL_Joystick Joystick { get; private set; }

        private Dictionary<GamepadButton, float> _buttonsDown;
        private HashSet<GamepadButton> _buttonsDownThisFrame;
        private Dictionary<GamepadButton, float> _buttonsUpThisFrame;
        private Dictionary<GamepadAxis, float> _axes;

        public GameController(string name, SDL_GameController controller, SDL_Joystick joystick)
        {
            Name = name;
            Controller = controller;
            Joystick = joystick;

            _buttonsDown = new Dictionary<GamepadButton, float>();
            _buttonsDownThisFrame = new HashSet<GamepadButton>();
            _buttonsUpThisFrame = new Dictionary<GamepadButton, float>();

            _axes = new Dictionary<GamepadAxis, float>();
            foreach (GamepadAxis axis in Enum.GetValues(typeof(GamepadAxis)))
            {
                _axes.Add(axis, 0.0f);
            }
        }

        public void PrepareForEvents()
        {
            _buttonsDownThisFrame.Clear();
            _buttonsUpThisFrame.Clear();
        }

        public void UpdateButtonTimeCounters(float timeSinceLastUpdateSeconds)
        {
            foreach (var button in _buttonsDown.Keys.ToList())
            {
                _buttonsDown[button] += timeSinceLastUpdateSeconds;
            }
        }

        public void ProcessButtonEvent(GamepadButton button, bool pressed)
        {
            if (pressed)
            {
                ButtonDown(button);
            }
            else
            {
                ButtonUp(button);
            }
        }

        private void ButtonDown(GamepadButton button)
        {
            if (!_buttonsDown.ContainsKey(button))
            {
                _buttonsDown.Add(button, 0.0f);
                _buttonsDownThisFrame.Add(button);
            }
        }

        private void ButtonUp(GamepadButton button)
        {
            float timeDown = 0.0f;
            if (_buttonsDown.ContainsKey(button))
            {
                timeDown = _buttonsDown[button];
                _buttonsDown.Remove(button);
            }

            if (!_buttonsUpThisFrame.ContainsKey(button))
            {
                _buttonsUpThisFrame.Add(button, timeDown);
            }
        }

        public void ProcessAxisEvent(GamepadAxis axis, float value)
        {
            _axes[axis] = value;
        }

        public bool IsButtonCurrentlyPressed(GamepadButton button) => _buttonsDown.ContainsKey(button);
        public bool IsButtonPressedThisFrame(GamepadButton button) => _buttonsDownThisFrame.Contains(button);
        public bool WasButtonReleasedThisFrame(GamepadButton button) => _buttonsUpThisFrame.ContainsKey(button);
        public float HowLongHasButtonBeenHeldDown(GamepadButton button, bool countIfUpThisFrame)
        {
            if (_buttonsDown.ContainsKey(button))
            {
                return _buttonsDown[button];
            }

            if (countIfUpThisFrame && _buttonsUpThisFrame.ContainsKey(button))
            {
                return _buttonsUpThisFrame[button];
            }

            return 0.0f;
        }

        public List<GamepadButton> ButtonsPressedThisFrame()
        {
            return _buttonsDownThisFrame.ToList();
        }

        public List<GamepadButton> ButtonsHeldDown()
        {
            return _buttonsDown.Keys.ToList();
        }

        public List<GamepadButton> ButtonsReleasedThisFrame()
        {
            return _buttonsUpThisFrame.Keys.ToList();
        }

        public float AxisValue(GamepadAxis axis)
        {
            return _axes[axis];
        }
    }
}