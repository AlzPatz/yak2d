namespace Yak2D
{
    /// <summary>
    /// Description for shader uniforms used in a CustomShader render stage
    /// </summary>
    public struct ShaderUniformDescription
    {
        /// <summary>
        /// Uniform string name - used as a refrence when updating values
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data Type -> Texture, Struct or Struct Array
        /// </summary>
        public ShaderUniformType UniformType { get; set; }

        /// <summary>
        /// Size of uniform data in bytes
        /// </summary>
        public uint SizeInBytes { get; set; }
    }
}