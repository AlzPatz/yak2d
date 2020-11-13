using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public class ImageSharpLoader : IImageSharpLoader
    {
        private readonly ISystemComponents _systemComponents;
        private readonly IFloatTextureBuilder _floatTextureBuilder;

        public ImageSharpLoader(ISystemComponents systemComponents, IFloatTextureBuilder floatTextureBuilder)
        {
            _systemComponents = systemComponents;
            _floatTextureBuilder = floatTextureBuilder;
        }

        public Texture GenerateVeldridTextureFromStream(Stream stream, bool mipMap)
        {
            var imageSharpTexture = ProcessStream(stream, mipMap);
            return imageSharpTexture.CreateDeviceTexture(_systemComponents.Device.RawVeldridDevice, _systemComponents.Factory.RawFactory);
        }

        public TextureData GenerateTextureDataFromStream(Stream stream)
        {
            var data = Image.Load<Rgba32>(stream);

            Image<Rgba32> image = MirrorOverXAxisIfGraphicsApiRequires(data);

            var width = image.Width;
            var height = image.Height;

            var pixels = new Vector4[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var index = (y * width) + x;

                    var b = image[x, y];

                    pixels[index] = b.ToScaledVector4();
                }
            }

            return new TextureData
            {
                Width = (uint)width,
                Height = (uint)height,
                Pixels = pixels
            };
        }

        private ImageSharpTexture ProcessStream(Stream stream, bool mipMap)
        {
            var data = Image.Load<Rgba32>(stream);

            Image<Rgba32> image = MirrorOverXAxisIfGraphicsApiRequires(data);

            return new ImageSharpTexture(image, mipMap);
        }

        private Image<Rgba32> MirrorOverXAxisIfGraphicsApiRequires(Image<Rgba32> image)
        {
            var backend = _systemComponents.Device.BackendType;
            if (backend == GraphicsApi.OpenGL || backend == GraphicsApi.OpenGLES)
            {
                var width = image.Width;
                var height = image.Height;

                var bytes = new Rgba32[width * height];

                for (var y = 0; y < height; y++)
                {
                    var flippedY = height - 1 - y;
                    var StartOfLineInLinearIndex = flippedY * width;
                    for (var x = 0; x < width; x++)
                    {
                        bytes[StartOfLineInLinearIndex + x] = image[x, y];
                    }
                }

                return Image.LoadPixelData(bytes, width, height);
            }
            return image;
        }

        public Texture GenerateSingleWhitePixel()
        {
            var image = SixLabors.ImageSharp.Image.LoadPixelData(new Rgba32[] {
                                                                    new Rgba32(1.0f, 1.0f, 1.0f, 1.0f)
                                                                    }, 1, 1);
            var imageSharpTexture = new ImageSharpTexture(image, true);

            var veldridTexture = imageSharpTexture.CreateDeviceTexture(_systemComponents.Device.RawVeldridDevice, _systemComponents.Factory.RawFactory);

            return veldridTexture;
        }

        public Texture GenerateRgbaVeldridTextureFromPixelData(Rgba32[] data, uint width, uint height)
        {
            var image = SixLabors.ImageSharp.Image.LoadPixelData(data, (int)width, (int)height);

            var imageSharpTexture = new ImageSharpTexture(image, true);

            var veldridTexture = imageSharpTexture.CreateDeviceTexture(_systemComponents.Device.RawVeldridDevice, _systemComponents.Factory.RawFactory);

            return veldridTexture;
        }

        public Texture GenerateFloat32VeldridTextureFromPixelData(float[] data, uint width, uint height)
        {
            return _floatTextureBuilder.GenerateFloat32VeldridTextureFromPixelData(data, width, height);
        }
    }
}