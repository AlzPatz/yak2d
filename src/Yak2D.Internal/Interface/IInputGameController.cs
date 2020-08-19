using System.Collections.Generic;
using Veldrid.Sdl2;

namespace Yak2D.Internal
{
    public interface IInputGameController
    {
        void Update(float timeStepSeconds);
        void CacheEvent(ref SDL_Event ev);
        bool IsGamepadIdValid(int id);
        List<int> ConnectedGamepadIds();
        bool IsGamepadButtonCurrentlyPressed(int id, GamepadButton button);
        bool IsGamepadButtonPressedThisFrame(int id, GamepadButton button);
        bool WasGamepadButtonReleasedThisFrame(int id, GamepadButton button);
        float HowLongHasGamepadButtonBeenHeldDown(int id, GamepadButton button, bool countIfUpThisFrame = false);
        List<GamepadButton> GamepadButtonsPressedThisFrame(int id);
        List<GamepadButton> GamepadButtonsHeldDown(int id);
        List<GamepadButton> GamepadButtonsReleasedThisFrame(int id);
        float GamepadAxisValue(int id, GamepadAxis axis);
    }
}