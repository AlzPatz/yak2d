using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace Yak2D
{
    /// <summary>
    /// Input Operations
    /// Yak2D supports Keyboard, Mouse and Gamepad input 
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Am object of frame input data provided by the veldrid library
        /// </summary>
        InputSnapshot RawVeldridInputSnapshot { get; }

        /// <summary>
        /// Get mouse position over window (origin top-left)
        /// </summary>
        Vector2 MousePosition { get; }

        /// <summary>
        /// Whether the mouse is currently over the window. Please be aware:
        /// The implementation is reasonably performant, but registering an off screen requires
        /// capturing a previous SDL event that when extrapolating motion suggests the next frame the mouse will leave the window
        /// Therefore, an incorrect reading can (and usually will) be given if a window is covering the application window and the mouse moves into this window's area 
        /// </summary>
        bool IsMouseOverWindow { get; }

        /// <summary>
        /// How far the mouse has moved over the desktop since the last update
        /// </summary>
        Vector2 MousePositionDeltaSinceLastFrame { get; }

        /// <summary>
        /// The velocity in pixels per second, the mouse cursor moved over the window 
        /// </summary>
        Vector2 MouseVelocity { get; }

        /// <summary>
        /// Gets whether a button is pressed
        /// </summary>
        /// <param name="button">The button to query</param>
        bool IsMouseCurrentlyPressed(MouseButton button);

        /// <summary>
        /// Returns true if this is the first UPDATE since the button was pressed. . Do NOT use in DRAW calls
        /// </summary>
        /// <param name="button">The button to query</param>
        bool WasMousePressedThisFrame(MouseButton button);

        /// <summary>
        /// Returns true if this is the first UPDATE since the button was released. . Do NOT use in DRAW calls
        /// </summary>
        /// <param name="button">The button to query</param>
        bool WasMouseReleasedThisFrame(MouseButton button);

        /// <summary>
        /// Returns the number of seconds a button has been held down for
        /// </summary>
        /// <param name="button">The button to query</param>
        /// <param name="countIfUpThisFrame">If true, a time will be returned even if this is the first update since the button was released</param>
        float HowLongHasMouseBeenHeldDown(MouseButton button, bool countIfUpThisFrame = false);

        /// <summary>
        /// Returns list of buttons pressed since the last update
        /// </summary>
        List<MouseButton> MouseButtonsPressedThisFrame();

        /// <summary>
        /// Returns list of buttons currently held down
        /// </summary>
        List<MouseButton> MouseButtonsHeldDown();

        /// <summary>
        /// Returns list of buttons where this update is the first since being released
        /// </summary>
        List<MouseButton> MouseButtonsReleasedThisFrame();

        /// <summary>
        /// Gets whether a key is pressed
        /// </summary>
        /// <param name="key">The key to query</param>
        bool IsKeyCurrentlyPressed(KeyCode key);

        /// <summary>
        /// Returns true if this is the first UPDATE since the key was pressed. Do NOT use in DRAW calls
        /// </summary>
        /// <param name="key">The key to query</param>
        bool WasKeyPressedThisFrame(KeyCode key);

        /// <summary>
        /// Returns true if this is the first UPDATE since the key was released. . Do NOT use in DRAW calls
        /// </summary>
        /// <param name="key">The key to query</param>
        bool WasKeyReleasedThisFrame(KeyCode key);

        /// <summary>
        /// Returns the number of seconds a key has been held down for
        /// </summary>
        /// <param name="key">The key to query</param>
        /// <param name="countIfUpThisFrame">If true, a time will be returned even if this is the first update since the key was released</param>
        float HowLongHasKeyBeenHeldDown(KeyCode key, bool countIfUpThisFrame = false);

        /// <summary>
        /// Returns list of keys pressed since the last update
        /// </summary>
        List<KeyCode> KeysPressedThisFrame();

        /// <summary>
        /// Returns list of keys currently held down
        /// </summary>
        List<KeyCode> KeysHeldDown();

        /// <summary>
        /// Returns list of keys where this update is the first since being released
        /// </summary>
        List<KeyCode> KeysReleasedThisFrame();

        /// <summary>
        /// Returns whether the provided id is a valid for a connected gamepad
        /// </summary>
        /// <param name="id">positive integer reference id for gamepad</param>
        bool IsGamepadIdValid(int id);

        /// <summary>
        /// Returns list of positive integer indices which are connected gamepad references
        /// </summary>
        List<int> ConnectedGamepadIds();

        /// <summary>
        /// Gets whether a gamepad button is currently held down
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        /// <param name="button">The button to query</param>
        bool IsGamepadButtonCurrentlyPressed(int id, GamepadButton button);

        /// <summary>
        /// Returns true if this is the first UPDATE since the button was pressed. Do NOT use in DRAW calls
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        /// <param name="button">The button to query</param>        
        bool WasGamepadButtonPressedThisFrame(int id, GamepadButton button);

        /// <summary>
        /// Returns true if this is the first UPDATE since the button was released. Do NOT use in DRAW calls
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        /// <param name="button">The button to query</param>                
        bool WasGamepadButtonReleasedThisFrame(int id, GamepadButton button);

        /// <summary>
        /// Returns the number of seconds a button has been held down for
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        /// <param name="button">The button to query</param>
        /// <param name="countIfUpThisFrame">If true, a time will be returned even if this is the first update since the button was released</param>
        float HowLongHasGamepadButtonBeenHeldDown(int id, GamepadButton button, bool countIfUpThisFrame = false);

        /// <summary>
        /// Returns list of buttons pressed since the last update
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        List<GamepadButton> GamepadButtonsPressedThisFrame(int id);

        /// <summary>
        /// Returns list of buttons currently held down
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        List<GamepadButton> GamepadButtonsHeldDown(int id);

        /// <summary>
        /// Returns list of buttons where this update is the first since being released
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        List<GamepadButton> GamepadButtonsReleasedThisFrame(int id);

        /// <summary>
        /// Returns the current value of a gamepad axis (usually, -1 to +1) 
        /// </summary>
        /// <param name="id">Positive id reference for gamepad</param>
        /// <param name="axis">The axis to query</param>
        float GamepadAxisValue(int id, GamepadAxis axis);
    }
}