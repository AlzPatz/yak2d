namespace Yak2D
{
    /// <summary>
    /// Holds the start up properties for a Yak2D application
    /// </summary>
    public class StartupConfig
    {
        /// <summary>
        /// Provides some default configuration settings
        /// Texture and Font folders are set to "Textures" and "Fonts" respectively
        /// </summary>
        /// <param name="windowWidth">Horizontal Window Resolution</param>
        /// <param name="windowHeight">Vertical Window Resolution</param>
        /// <param name="windowTitle">Window Title</param>
        /// <param name="fullScreen">Start the application in fullscreen</param>
        /// <returns></returns>
        public static StartupConfig Default(uint windowWidth, uint windowHeight, string windowTitle, bool fullScreen)
        {
            return new StartupConfig
            {
                PreferredGraphicsApi = GraphicsApi.SystemDefault,
                AvoidVulkanWherePossible = true,
                WindowIsResizable = false,
                WindowState = fullScreen ? DisplayState.FullScreen : DisplayState.Normal,
                WindowPositionX = 100,
                WindowPositionY = 100,
                WindowWidth = (int)windowWidth,
                WindowHeight = (int)windowHeight,
                WindowTitle = windowTitle,
                SyncToVerticalBlank = true,
                UpdatePeriodType = UpdatePeriod.Fixed,
                ProcessFractionalUpdatesBeforeDraw = true,
                FixedOrSmallestUpdateTimeStepInSeconds = 1.0f / 120.0f,
                RequireAtleastOneUpdatePerDraw = true,
                FpsCalculationUpdatePeriod = 1.0f,
                TextureFolderRootName = "Textures",
                FontFolder = "Fonts",
                AutoClearMainWindowColourEachFrame = false,
                AutoClearMainWindowDepthEachFrame = false
            };
        }

        /// <summary>
        /// Sets preferred graphics API. Unsupported APIs for the runtime system will be ignored and a system default will be chosen
        /// </summary>
        public GraphicsApi PreferredGraphicsApi { get; set; }

        /// <summary>
        /// If SystemDefault is selected, this toggle can be used to avoid Vulkan unless it is the only system option
        /// </summary>
        public bool AvoidVulkanWherePossible { get; set; }

        /// <summary>
        ///  Sets start up window state
        /// </summary>
        public DisplayState WindowState { get; set; }

        /// <summary>
        /// Sets whether the window is resizable (can be changed during application runtime) 
        /// </summary>
        public bool WindowIsResizable { get; set; }

        /// <summary>
        /// Sets X component of top-left position of window
        /// </summary>
        public int WindowPositionX { get; set; }

        /// <summary>
        /// Sets Y component of top-left position of window
        /// </summary>
        public int WindowPositionY { get; set; }

        /// <summary>
        /// Sets window width in pixels, aswell as the width of the main window swapchain backbuffer (main render surface)
        /// </summary>
        public int WindowWidth { get; set; }

        /// <summary>
        /// Sets window height in pixels, aswell as the height of the main window swapchain backbuffer (main render surface)
        /// </summary>
        public int WindowHeight { get; set; }

        /// <summary>
        /// Sets the window title bar text
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Render updates wait for monitor vertical sync (to avoid screen tearing)
        /// </summary>
        public bool SyncToVerticalBlank { get; set; }

        /// <summary>
        /// Defines the time period slicing for framework update calls. Framework update and draw iterations are on different loop cycles
        /// </summary>
        public UpdatePeriod UpdatePeriodType { get; set; }

        /// <summary>
        /// Sets whether when ready to draw, the engine does a 'catch up' update iteration to bring current system evolution closer to realtime
        /// </summary>
        public bool ProcessFractionalUpdatesBeforeDraw { get; set; }

        /// <summary>
        /// If a fixed update period type is chosen, this defines the time slice in seconds of each update. Should a variable update period be chosen, this defines the smallest time interval to process an update for
        /// </summary>
        public float FixedOrSmallestUpdateTimeStepInSeconds { get; set; }

        /// <summary>
        /// Sets whether at least one update iteration is required before starting a new draw
        /// </summary>
        public bool RequireAtleastOneUpdatePerDraw { get; set; }

        /// <summary>
        /// The period in seconds over which the update and draw loop 'frames per second' are calculated
        /// </summary>
        public float FpsCalculationUpdatePeriod { get; set; }

        /// <summary>
        /// The root folder (either on disk or as an embedded resource location) for the user texture files (.png)
        /// </summary>
        public string TextureFolderRootName { get; set; }

        /// <summary>
        /// The root folder (either on disk or as an embedded resource location) for the user font data (.fnt files and related .png's)
        /// </summary>
        public string FontFolder { get; set; }

        /// <summary>
        /// Sets whether before each draw iteration, the main window render target's depth buffer is cleared (helps avoid situations where this is forgotten)
        /// </summary>
        public bool AutoClearMainWindowDepthEachFrame { get; set; }

        /// <summary>
        /// Sets whether before each draw iteration, the main window render target's colour buffer (texture) is cleared to transparent black (0,0,0,0)
        /// </summary>
        public bool AutoClearMainWindowColourEachFrame { get; set; }
    }
}