namespace Yak2D
{
    /// <summary>
    /// Represents the various Graphics APIs
    /// </summary>
    public enum GraphicsApi
    {
        /// <summary>
        /// Allows the user to request the default Graphics API for the current system
        /// </summary>
        SystemDefault,
        
        Direct3D11,
        Vulkan,
        OpenGL,
        Metal,
        OpenGLES
    }
}