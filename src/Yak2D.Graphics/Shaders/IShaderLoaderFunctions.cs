using Veldrid;

namespace Yak2D.Graphics
{
    public interface IShaderLoaderFunctions
    {
        ResourceLayout[] CreateUniformResourceLayouts(ResourceLayoutElementDescription[][] elementDescriptions);
        Shader LoadShader(string name, AssetSourceEnum assetTypes, ShaderStages stage, bool useSpirvCompile = false);
    }
}
