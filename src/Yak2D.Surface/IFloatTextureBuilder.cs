using Veldrid;

namespace Yak2D.Surface
{
    public interface IFloatTextureBuilder
    {
        Texture GenerateFloat32VeldridTextureFromPixelData(float[] data, uint width, uint height);
    }
}
