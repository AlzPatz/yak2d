namespace Yak2D
{
    /// <summary>
    /// Description for shader uniforms used in a CustomShader render stage
    /// </summary>
    public struct ShaderUniformDescription
    {
        /// <summary>
        /// Uniform string name - used as a reference when updating values.
        /// For Texture uniforms the name is also the suffix of the shader resource names in that uniform's set
        /// (texture at binding 0, sampler at binding 1): Texture_&lt;Name&gt; / Sampler_&lt;Name&gt; on the SPIR-V (useSpirvCompile) path,
        /// and Sampler_&lt;Name&gt; / Texture_&lt;Name&gt; for precompiled shaders
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