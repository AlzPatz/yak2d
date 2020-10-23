using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Yak2D.Internal
{
    public class InputMouseKeyboard : IInputMouseKeyboard
    {
        private readonly IFrameworkDebugOverlay _debugOverlay;

        public Vector2 MousePosition { get; private set; }
        public Vector2 MousePositionDeltaSinceLastFrame { get; private set; }
        public Vector2 MouseVelocity { get; private set; }

        private Vector2 _mousePositionLastFrame;
        private readonly bool _isFirstFrame;
        private Dictionary<KeyCode, float> _keysDown;
        private HashSet<KeyCode> _keysDownThisFrame;
        private Dictionary<KeyCode, float> _keysUpThisFrame;

        private Dictionary<MouseButton, float> _mouseButtonsDown;
        private HashSet<MouseButton> _mouseButtonsDownThisFrame;
        private Dictionary<MouseButton, float> _mouseButtonsUpThisFrame;

        public InputMouseKeyboard(IFrameworkDebugOverlay debugOverlay)
        {
            _debugOverlay = debugOverlay;

            _keysDown = new Dictionary<KeyCode, float>();
            _keysDownThisFrame = new HashSet<KeyCode>();
            _keysUpThisFrame = new Dictionary<KeyCode, float>();

            _mouseButtonsDown = new Dictionary<MouseButton, float>();
            _mouseButtonsDownThisFrame = new HashSet<MouseButton>();
            _mouseButtonsUpThisFrame = new Dictionary<MouseButton, float>();

            _isFirstFrame = true;
        }

        public void UpdateVeldridInputSnapshot(InputSnapshot snapshot, float timeSinceLastUpdateSeconds)
        {
            ProcessKeys(snapshot, timeSinceLastUpdateSeconds);
            ProcessMouse(snapshot, timeSinceLastUpdateSeconds);
        }

        public bool IsKeyCurrentlyPressed(KeyCode key) => _keysDown.ContainsKey(key);
        public bool IsKeyPressedThisFrame(KeyCode key) => _keysDownThisFrame.Contains(key);
        public bool WasKeyReleasedThisFrame(KeyCode key) => _keysUpThisFrame.ContainsKey(key);
        public float HowLongHasKeyBeenHeldDown(KeyCode key, bool countIfUpThisFrame)
        {
            if (_keysDown.ContainsKey(key))
            {
                return _keysDown[key];
            }

            if (countIfUpThisFrame && _keysUpThisFrame.ContainsKey(key))
            {
                return _keysUpThisFrame[key];
            }

            return 0.0f;
        }

        private void ProcessKeys(InputSnapshot snapshot, float timeSinceLastUpdateSeconds)
        {
            _keysDownThisFrame.Clear();
            _keysUpThisFrame.Clear();

            for (var n = 0; n < snapshot.KeyEvents.Count; n++)
            {
                var ke = snapshot.KeyEvents[n];
                var key = ke.Key;
                var keyCode = ToKeyCode(key);

                if (ke.Down)
                {
                    KeyDown(keyCode);
                }
                else
                {
                    KeyUp(keyCode);
                }
            }

            foreach (var key in _keysDown.Keys.ToList())
            {
                _keysDown[key] += timeSinceLastUpdateSeconds;
            }

#if DEBUG
            CheckForDebugOverlayKeyCombination();
#endif

        }

        private void CheckForDebugOverlayKeyCombination()
        {
            if (_keysDown.ContainsKey(KeyCode.ControlLeft) &&
                _keysDown.ContainsKey(KeyCode.ShiftLeft) &&
                _keysDownThisFrame.Contains(KeyCode.D))
            {
                _debugOverlay.Visible = !_debugOverlay.Visible;
            }
        }

        private KeyCode ToKeyCode(Key key)
        {
            return (KeyCode)key;
        }

        private void KeyDown(KeyCode keyCode)
        {
            if (!_keysDown.ContainsKey(keyCode))
            {
                _keysDown.Add(keyCode, 0.0f);
                _keysDownThisFrame.Add(keyCode);
            }
        }

        private void KeyUp(KeyCode keyCode)
        {
            float timeDown = 0.0f;
            if (_keysDown.ContainsKey(keyCode))
            {
                timeDown = _keysDown[keyCode];
                _keysDown.Remove(keyCode);
            }

            if (!_keysUpThisFrame.ContainsKey(keyCode))
            {
                _keysUpThisFrame.Add(keyCode, timeDown);
            }
        }

        private void ProcessMouse(InputSnapshot snapshot, float timeSinceLastUpdateSeconds)
        { 
            _mousePositionLastFrame = MousePosition;
            MousePosition = snapshot.MousePosition;

            if (_isFirstFrame)
            {
                MousePositionDeltaSinceLastFrame = Vector2.Zero;
                MouseVelocity = Vector2.Zero;
            }
            else
            {
                MousePositionDeltaSinceLastFrame = MousePosition - _mousePositionLastFrame;
                MouseVelocity = MousePositionDeltaSinceLastFrame / timeSinceLastUpdateSeconds;
            }

            _mouseButtonsDownThisFrame.Clear();
            _mouseButtonsUpThisFrame.Clear();

            for (var n = 0; n < snapshot.MouseEvents.Count; n++)
            {
                var me = snapshot.MouseEvents[n];
                var button = me.MouseButton;
                var mouseButton = ToMouseButton(button);

                if (me.Down)
                {
                    MouseDown(mouseButton);
                }
                else
                {
                    MouseUp(mouseButton);
                }
            }

            foreach (var button in _mouseButtonsDown.Keys.ToList())
            {
                _mouseButtonsDown[button] += timeSinceLastUpdateSeconds;
            }
        }

        private MouseButton ToMouseButton(Veldrid.MouseButton button)
        {
            return (MouseButton)button;
        }

        private void MouseDown(MouseButton button)
        {
            if (!_mouseButtonsDown.ContainsKey(button))
            {
                _mouseButtonsDown.Add(button, 0.0f);
                _mouseButtonsDownThisFrame.Add(button);
            }
        }

        private void MouseUp(MouseButton button)
        {
            float timeDown = 0.0f;
            if (_mouseButtonsDown.ContainsKey(button))
            {
                timeDown = _mouseButtonsDown[button];
                _mouseButtonsDown.Remove(button);
            }

            if (!_mouseButtonsUpThisFrame.ContainsKey(button))
            {
                _mouseButtonsUpThisFrame.Add(button, timeDown);
            }
        }

        public bool IsMouseCurrentlyPressed(MouseButton button) => _mouseButtonsDown.ContainsKey(button);
        public bool IsMousePressedThisFrame(MouseButton button) => _mouseButtonsDownThisFrame.Contains(button);
        public bool WasMouseReleasedThisFrame(MouseButton button) => _mouseButtonsUpThisFrame.ContainsKey(button);
        public float HowLongHasMouseBeenHeldDown(MouseButton button, bool countIfUpThisFrame)
        {
            if (_mouseButtonsDown.ContainsKey(button))
            {
                return _mouseButtonsDown[button];
            }

            if (countIfUpThisFrame && _mouseButtonsUpThisFrame.ContainsKey(button))
            {
                return _mouseButtonsUpThisFrame[button];
            }

            return 0.0f;
        }

        public List<MouseButton> MouseButtonsPressedThisFrame()
        {
            return _mouseButtonsDownThisFrame.ToList();
        }

        public List<MouseButton> MouseButtonsHeldDown()
        {
            return _mouseButtonsDown.Keys.ToList();
        }

        public List<MouseButton> MouseButtonsReleasedThisFrame()
        {
            return _mouseButtonsUpThisFrame.Keys.ToList();
        }

        public List<KeyCode> KeyPressedThisFrame()
        {
            return _keysDownThisFrame.ToList();
        }

        public List<KeyCode> KeysHeldDown()
        {
            return _keysDown.Keys.ToList();
        }

        public List<KeyCode> KeysReleasedThisFrame()
        {
            return _keysUpThisFrame.Keys.ToList();
        }
    }
}