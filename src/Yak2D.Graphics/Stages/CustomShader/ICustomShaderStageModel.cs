using Veldrid;

namespace Yak2D.Graphics
{
    public interface ICustomShaderStageModel : IRenderStageModel
    {
        Pipeline Pipeline { get; }
        int NumberUserUniforms { get; }
        ShaderUniformType UserUniformType(int index);
        string UserUniformName(int index);
        ResourceSet UserUniformResourceSet(string name);
        ResourceSet UserUniformResourceSet(int index);
        void SetUniformValue<T>(string uniformName, T data) where T: unmanaged;
        void SetUniformValue<T>(string uniformName, T[] dataArray) where T: unmanaged;
    }
}