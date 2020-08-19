using Veldrid;

namespace Yak2D.Graphics
{
    public interface IShaderLoader
    {
        ShaderPackage CreateShaderPackage(string vertexShaderName,
                                            AssetSourceEnum vertexShaderAssetType,
                                            string fragmentShaderName,
                                            AssetSourceEnum fragmentShaderAssetType,
                                            VertexLayoutDescription layoutDescription,
                                            ResourceLayoutElementDescription[][] uniformDescription);
    }
}