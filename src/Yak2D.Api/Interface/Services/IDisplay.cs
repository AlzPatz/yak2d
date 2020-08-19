namespace Yak2D
{
    /// <summary>
	/// Display / Window Operations: Resolution, Position, Title, State
	/// </summary>
    public interface IDisplay
    {
        /// <summary>
        /// Returns whether application window is currently in focus
        /// </summary>
        bool WindowIsInFocus { get; }

        /// <summary>
        /// X Location of left edge of Window of Desktop
        /// </summary>
        int WindowPositionX { get; set; }
        /// <summary>
        /// Y Location of left edge of Window of Desktop
        /// Note: Certain desktops (including Gnome and KDE when tested) appear to read this as the top of the visible client area, but set as the top of the entire window including title bar
        /// </summary> 
        int WindowPositionY { get; set; }

        /// <summary>
        /// Window opactiy on desktop
        /// </summary>
        float WindowOpacity { get; set; }

        /// <summary>
        /// Controls whether the window is user resizable 
        /// </summary>
        bool WindowResizable { get; set; }
        
        /// <summary>
        /// Controls window border visibility
        /// </summary>
        bool WindowBorderVisible { get; set; }

        /// <summary>
        /// Returns current window pixel width
        /// </summary>
        uint WindowResolutionWidth { get; }
        
        /// <summary>
        /// Returns current window pixel height
        /// </summary>
        uint WindowResolutionHeight { get; }

        /// <summary>
        /// Returns current window state
        /// </summary>
        DisplayState DisplayState { get; }

        /// <summary>
        /// Sets display state
        /// </summary>
        /// <param name="state">The desired window state</param>
        void SetDisplayState(DisplayState state);
        
        /// <summary>
        /// Sets windows pixel resolution
        /// </summary>
        /// <param name="width">Width in pixels"</param>
        /// <param name="height">Height in pixels"</param>
        void SetWindowResolution(uint width, uint height);
        
        /// <summary>
        /// Sets vsync
        /// </summary>        
        /// <param name="vsync">Vsync on or off"</param>
        void SetVsync(bool vsync);

        /// <summary>
        /// Sets window title bar text
        /// </summary>
        /// <param name="title">Title text"</param>
        void SetWindowTitle(string title);

        /// <summary>
        /// Sets whether the cursor is visible over the window area
        /// </summary>
        /// <param name="visible">Cursor visibility"</param>
        void SetCursorVisible(bool visible);
    }
}