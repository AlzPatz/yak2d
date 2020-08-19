namespace Yak2D
{
    /// <summary>
    /// Enumeration of gamepad axes (xbox style)
    /// Identical to Veldrid Enum
    /// </summary>
    public enum GamepadAxis : byte
    {
        LeftX = 0,
        LeftY = 1,
        RightX = 2,
        RightY = 3,
        TriggerLeft = 4,
        TriggerRight = 5,
        Max = 6,
        Invalid = 255
    }
}