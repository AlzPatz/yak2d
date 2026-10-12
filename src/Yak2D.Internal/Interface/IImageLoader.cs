using System.IO;
using NeoVeldrid;

namespace Yak2D.Internal
{
    public interface IImageLoader
    {
        Texture GenerateVeldridTextureFromStream(Stream stream, bool mipMap);
        TextureData GenerateTextureDataFromStream(Stream stream);
        Texture GenerateRgbaVeldridTextureFromPixelData(RgbaByte[] data, uint width, uint height, bool mipMap);
        Texture GenerateFloat32VeldridTextureFromPixelData(float[] data, uint width, uint height);
        Texture GenerateSingleWhitePixel();
    }
}
