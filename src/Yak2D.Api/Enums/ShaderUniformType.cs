namespace Yak2D
{
    /// <summary>
    /// Descriptor for Custom User Shader stage uniform type 
    /// </summary>
    public enum ShaderUniformType
    {
        /// <summary>
        /// Uniform is a texture to be sampled in the shader
        /// </summary>
        Texture,

        /// <summary>
        /// Uniform is a struct or array of structs
        /// </summary>
        Data
    }
}