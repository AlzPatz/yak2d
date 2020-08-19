using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace Yak2D.Internal
{
    public interface IInputMouseKeyboard
    {
        Vector2 MousePosition { get; }
        Vector2 MousePositionDeltaSinceLastFrame { get; }
        Vector2 MouseVelocity { get; }

        bool IsMouseCurrentlyPressed(MouseButton button);
        bool IsMousePressedThisFrame(MouseButton button);
        bool WasMouseReleasedThisFrame(MouseButton button);
        float HowLongHasMouseBeenHeldDown(MouseButton button, bool countIfUpThisFrame = false);
        List<MouseButton> MouseButtonsPressedThisFrame();
        List<MouseButton> MouseButtonsHeldDown();
        List<MouseButton> MouseButtonsReleasedThisFrame();

        bool IsKeyCurrentlyPressed(KeyCode key);
        bool IsKeyPressedThisFrame(KeyCode key);
        bool WasKeyReleasedThisFrame(KeyCode key);
        float HowLongHasKeyBeenHeldDown(KeyCode key, bool countIfUpThisFrame);
        List<KeyCode> KeyPressedThisFrame();
        List<KeyCode> KeysHeldDown();
        List<KeyCode> KeysReleasedThisFrame();

        void UpdateVeldridInputSnapshot(InputSnapshot snapshot, float timeSinceLastUpdateSeconds);
    }
}