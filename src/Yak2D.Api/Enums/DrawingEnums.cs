namespace Yak2D
{
    /// <summary>
    /// DrawRequests (DrawStage and DistortionStage) are transformed to World or Screen Coordinates by the properties of the chosen Camera2D at render time
    /// Screen coordinates are (0,0) in the middle of the Camera's view and extend half of the camera's virtual resolution along the x and y axis
    /// The axis are positive X to the right, and positive Y upwards in terms of screen orientation
    /// The top left corner of the screen is therefore location (-0.5 VirtualResolutionX, +0.5 VirtualResolutionY)
    /// The camera's resolution is virtual (not pixel based), as when a DrawStage is rendered onto a RenderTarget, the camera's view is rendered across the entire RenderTarget (or current viewport portion)
    /// If the virtual resolution does not match the pixel resolution of the RenderTarget or Viewport, then the resultant render is scaled to the target shape
    /// This enables window / system resolution to remain seperate from the logical positioning of objects in both Screen and World space
    ///
    /// World coordinates are similar, but rendering position is transformed by both the camera's zoom and world focus point (and rotation)
    /// World axis are also (when the camera is not rotated) positive x to the right, and positive y upwards in terms of on-screen direction
    /// </summary>
    public enum CoordinateSpace
    {
        /// <summary>
        /// Vertex positions are transformed according to the camera's focal position, zoom factor and rotation
        /// </summary>
        World,
        /// <summary>
        /// Vertex positions are transformed simple in relation to the camera's current virtual resolution. The centre of the screen is position (0,0)
        /// </summary>
        Screen
    }

    /// <summary>
    /// Represents the Triangle Fill Type of a DrawRequest (when submitted to a DrawStage) 
    /// </summary>
    public enum FillType
    {
        /// <summary>
        /// Single Colour Triangles (no texturing)
        /// </summary>        
        Coloured,

        /// <summary>
        /// Textured Mapped - Single Texture
        /// </summary>     
        Textured,

        /// <summary>
        /// Textured Mapped - Dual Texturing with per Vertex weighted blending
        /// </summary>    
        DualTextured
    }
}