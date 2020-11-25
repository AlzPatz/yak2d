using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using Veldrid;

namespace Yak2D.Internal
{
    public interface IImageSharpLoader
    {
        Texture GenerateVeldridTextureFromStream(Stream stream, bool mipMap);
        TextureData GenerateTextureDataFromStream(Stream stream);
        Texture GenerateRgbaVeldridTextureFromPixelData(Rgba32[] data, uint width, uint height, bool mipMap);
        Texture GenerateFloat32VeldridTextureFromPixelData(float[] data, uint width, uint height);
        Texture GenerateSingleWhitePixel();
    }
}