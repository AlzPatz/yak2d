namespace Yak2D
{
    //Using visible instance fields rather than properties as wish to be able to pass via reference 
    //Think practice is generally OK for DTOs

    /// <summary>
    /// Holds data for 2D drawing requests
    /// A drawing request is comprised of indexed vertices arranged in a triangle list
    /// </summary>
    public struct DrawRequest
    {
        /// <summary>
        /// Screen or World coordinate spaces. Triangles in World space are transformed by a camera2D's position of focus and zoom factor
        /// </summary>
        public CoordinateSpace CoordinateSpace;

        /// <summary>
        /// Triangle filltype - Solid Colour, Single Textured or Dual Textured
        /// </summary>
        public FillType FillType;

        /// <summary>
        /// Vertex Data
        /// </summary>
        public Vertex2D[] Vertices;

        /// <summary>
        /// Triangle indices data, referencing array positions of vertex data. Array length must be divisible by 3
        /// </summary>
        public int[] Indices;

        /// <summary>
        /// Colour to apply to all vertices
        /// </summary>
        public Colour Colour;

        /// <summary>
        /// Primary Texture Reference (texture used for single texturing)
        /// Note: If you are using ulong references, there is helper function in IDrawing that will WrapTextureId() into a reference object
        /// </summary>
        public ITexture Texture0;

        /// <summary>
        /// Secondary Texture Reference (testure used for dual texturing)
        /// Note: If you are using ulong references, there is helper function in IDrawing that will WrapTextureId() into a reference object
        /// </summary>
        public ITexture Texture1;

        /// <summary>
        /// Primary Texture Coordinate Wrap behaviour (Wrap or Mirror is the only defined behaviour)
        /// </summary>
        public TextureCoordinateMode TextureWrap0;

        /// <summary>
        /// Secondary Texture Coordinate Wrap behaviour (Wrap or Mirror is the only defined behaviour)
        /// </summary>
        public TextureCoordinateMode TextureWrap1;

        /// <summary>
        /// Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])
        /// </summary>
        public float Depth;

        /// <summary>
        /// >= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers
        /// </summary>
        public int Layer;
    }
}