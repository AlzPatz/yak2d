namespace Yak2D
{
    /// <summary>
    /// Provides access to all Yak2D core components
    /// </summary>
    public interface IServices
    {
        /// <summary>
        /// Platform API operations
        /// </summary>
        IBackend Backend { get; }

        /// <summary>
        /// Display / Window Operations: Resolution, Position, Title, State
        /// </summary>
        IDisplay Display { get; }

        /// <summary>
        /// Provides current per-second rates for update and draw/render iteration loops
        /// </summary>
        IFps FPS { get; }

        /// <summary>
        /// Render Stage Operations
        /// Creation, configuration and destruction of render stages and viewports
        /// </summary>
        IStages Stages { get; }

        /// <summary>
        /// Input Operations
        /// Yak2D supports Keyboard, Mouse and Gamepad input 
        /// </summary>
        IInput Input { get; }

        /// <summary>
        /// GPU Surface Operations
        /// Texture and Render Target Creation and Destruction
        /// </summary>
        ISurfaces Surfaces { get; }

        /// <summary>
        /// Camera Operations: Creation, Configuration and Destruction
        /// 2D Cameras are used by Drawing Stages (IDrawStage and IDistortionStage)
        /// 3D Cameras are used by the MeshRender Stage
        /// </summary>
        ICameras Cameras { get; }

        /// <summary>
        /// Font Operations: Load, Destroy and Count
        /// </summary>
        IFonts Fonts { get; }

        /// <summary>
        /// Helper objects to support / simplify rendering stages
        /// </summary>
        IHelpers Helpers { get; }
    }
}