namespace Yak2D
{
    /// <summary>
    /// Framework messages inform the user application of an important state change
    /// </summary>
    public enum FrameworkMessage
    {
        /// <summary>
        /// All Resources have been lost. Recreate all Yak2D objects: Stages, Surfaces, Fonts, etc
        /// </summary>
        GraphicsDeviceRecreated,

        /// <summary>
        /// Previously held references / ids for the Main Window render target are now obsolete, refresh where required
        /// </summary>
        SwapChainFramebufferReCreated,

        WindowWasResized,
        WindowLostFocus,
        WindowGainedFocus,
        ApplicationWindowClosing,
        GamepadAdded,
        GamepadRemoved,
        LowMemoryReported,
    }
}