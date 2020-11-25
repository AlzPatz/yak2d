using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Input
{
    public class Input : IInput
    {
        private readonly IFrameworkMessenger _frameworkMessenger;
        private IInputGameController _inputGameController;
        private IInputMouseKeyboard _inputMouseKeyboard;
        private readonly IVeldridWindowUpdater _windowUpdater;

        public Input(IFrameworkMessenger frameworkMessenger,
                                IInputGameController inputGameController,
                                IInputMouseKeyboard inputMouseKeyboard,
                                IVeldridWindowUpdater windowUpdater)
        {
            _frameworkMessenger = frameworkMessenger;
            _inputGameController = inputGameController;
            _inputMouseKeyboard = inputMouseKeyboard;
            _windowUpdater = windowUpdater;
        }

        public bool IsMouseOverWindow => _inputMouseKeyboard.IsMouseOverWindow;
        public Vector2 MousePosition => _inputMouseKeyboard.MousePosition;
        public Vector2 MousePositionDeltaSinceLastFrame => _inputMouseKeyboard.MousePositionDeltaSinceLastFrame;
        public Vector2 MouseVelocity => _inputMouseKeyboard.MouseVelocity;

        public InputSnapshot RawVeldridInputSnapshot => _windowUpdater.LatestWindowInputSnapshot;

        public bool IsKeyCurrentlyPressed(KeyCode key) => _inputMouseKeyboard.IsKeyCurrentlyPressed(key);
        public bool WasKeyPressedThisFrame(KeyCode key) => _inputMouseKeyboard.IsKeyPressedThisFrame(key);
        public bool WasKeyReleasedThisFrame(KeyCode key) => _inputMouseKeyboard.WasKeyReleasedThisFrame(key);
        public float HowLongHasKeyBeenHeldDown(KeyCode key, bool countIfUpThisFrame = false) => _inputMouseKeyboard.HowLongHasKeyBeenHeldDown(key, countIfUpThisFrame);
        public List<KeyCode> KeysPressedThisFrame() => _inputMouseKeyboard.KeyPressedThisFrame();
        public List<KeyCode> KeysHeldDown() => _inputMouseKeyboard.KeysHeldDown();
        public List<KeyCode> KeysReleasedThisFrame() => _inputMouseKeyboard.KeysReleasedThisFrame();

        public bool IsMouseCurrentlyPressed(MouseButton button) => _inputMouseKeyboard.IsMouseCurrentlyPressed(button);
        public bool WasMousePressedThisFrame(MouseButton button) => _inputMouseKeyboard.IsMousePressedThisFrame(button);
        public bool WasMouseReleasedThisFrame(MouseButton button) => _inputMouseKeyboard.WasMouseReleasedThisFrame(button);
        public float HowLongHasMouseBeenHeldDown(MouseButton button, bool countIfUpThisFrame = false) => _inputMouseKeyboard.HowLongHasMouseBeenHeldDown(button, countIfUpThisFrame);
        public List<MouseButton> MouseButtonsPressedThisFrame() => _inputMouseKeyboard.MouseButtonsPressedThisFrame();
        public List<MouseButton> MouseButtonsHeldDown() => _inputMouseKeyboard.MouseButtonsHeldDown();
        public List<MouseButton> MouseButtonsReleasedThisFrame() => _inputMouseKeyboard.MouseButtonsReleasedThisFrame();

        public bool IsGamepadIdValid(int id) => _inputGameController.IsGamepadIdValid(id);
        public List<int> ConnectedGamepadIds() => _inputGameController.ConnectedGamepadIds();
        public bool IsGamepadButtonCurrentlyPressed(int id, GamepadButton button) => _inputGameController.IsGamepadButtonCurrentlyPressed(id, button);
        public bool WasGamepadButtonPressedThisFrame(int id, GamepadButton button) => _inputGameController.IsGamepadButtonPressedThisFrame(id, button);
        public bool WasGamepadButtonReleasedThisFrame(int id, GamepadButton button) => _inputGameController.WasGamepadButtonReleasedThisFrame(id, button);
        public float HowLongHasGamepadButtonBeenHeldDown(int id, GamepadButton button, bool countIfUpThisFrame = false) => _inputGameController.HowLongHasGamepadButtonBeenHeldDown(id, button);
        public List<GamepadButton> GamepadButtonsPressedThisFrame(int id) => _inputGameController.GamepadButtonsPressedThisFrame(id);
        public List<GamepadButton> GamepadButtonsHeldDown(int id) => _inputGameController.GamepadButtonsHeldDown(id);
        public List<GamepadButton> GamepadButtonsReleasedThisFrame(int id) => _inputGameController.GamepadButtonsReleasedThisFrame(id);
        public float GamepadAxisValue(int id, GamepadAxis axis) => _inputGameController.GamepadAxisValue(id, axis);
    }
}
