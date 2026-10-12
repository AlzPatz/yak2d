using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using NeoVeldrid;
using StbImageSharp;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    //Decodes image files with StbImageSharp (public domain / MIT) and uploads them as 8 bit RGBA textures,
    //building any mip map levels on the CPU with a simple 2x2 box filter
    public class ImageLoader : IImageLoader
    {
        private readonly ISystemComponents _systemComponents;
        private readonly IFloatTextureBuilder _floatTextureBuilder;

        public ImageLoader(ISystemComponents systemComponents, IFloatTextureBuilder floatTextureBuilder)
        {
            _systemComponents = systemComponents;
            _floatTextureBuilder = floatTextureBuilder;
        }

        public Texture GenerateVeldridTextureFromStream(Stream stream, bool mipMap)
        {
            var pixels = Decode(stream, out var width, out var height);

            return CreateTexture(MirrorOverXAxisIfGraphicsApiRequires(width, height, pixels), width, height, mipMap);
        }

        public TextureData GenerateTextureDataFromStream(Stream stream)
        {
            var decoded = Decode(stream, out var width, out var height);

            var image = MirrorOverXAxisIfGraphicsApiRequires(width, height, decoded);

            var pixels = new Vector4[width * height];

            for (var p = 0; p < pixels.Length; p++)
            {
                var b = image[p];
                pixels[p] = new Vector4(b.R, b.G, b.B, b.A) / 255.0f;
            }

            return new TextureData
            {
                Width = width,
                Height = height,
                Pixels = pixels
            };
        }

        public Texture GenerateSingleWhitePixel()
        {
            return CreateTexture(new RgbaByte[] { new RgbaByte(255, 255, 255, 255) }, 1, 1, false);
        }

        public Texture GenerateRgbaVeldridTextureFromPixelData(RgbaByte[] data, uint width, uint height, bool mipMap)
        {
            return CreateTexture(data, width, height, mipMap);
        }

        public Texture GenerateFloat32VeldridTextureFromPixelData(float[] data, uint width, uint height)
        {
            return _floatTextureBuilder.GenerateFloat32VeldridTextureFromPixelData(data, width, height);
        }

        private static RgbaByte[] Decode(Stream stream, out uint width, out uint height)
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            width = (uint)image.Width;
            height = (uint)image.Height;

            return MemoryMarshal.Cast<byte, RgbaByte>(image.Data).ToArray();
        }

        private T[] MirrorOverXAxisIfGraphicsApiRequires<T>(uint width, uint height, T[] data)
        {
            var backend = _systemComponents.Device.BackendType;
            if (backend == GraphicsApi.OpenGL || backend == GraphicsApi.OpenGLES)
            {

                var bytes = new T[width * height];

                for (var y = 0; y < height; y++)
                {
                    var flippedY = height - 1 - y;
                    var StartOfLineInLinearIndex = flippedY * width;
                    for (var x = 0; x < width; x++)
                    {
                        bytes[StartOfLineInLinearIndex + x] = data[(y * width) + x];
                    }
                }
                return bytes;
            }

            return data;
        }

        private Texture CreateTexture(RgbaByte[] pixels, uint width, uint height, bool mipMap)
        {
            if (pixels == null)
            {
                throw new Yak2DException("Internal Framework Exception: Generating Texture from Pixel Data given a null data array", new ArgumentNullException("pixels"));
            }

            if (pixels.Length != width * height)
            {
                throw new Yak2DException("Internal Framework Exception: Generating Texture from Pixel Data mismatch between data array size and texture dimensions");
            }

            var mipLevels = mipMap ? NumberOfMipLevels(width, height) : 1u;

            var texture = _systemComponents.Factory.CreateTexture(TextureDescription.Texture2D(
                width, height, mipLevels, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled));

            var level = pixels;
            var levelWidth = width;
            var levelHeight = height;

            for (var mip = 0u; mip < mipLevels; mip++)
            {
                if (mip > 0)
                {
                    level = HalveSize(level, levelWidth, levelHeight, out levelWidth, out levelHeight);
                }

                _systemComponents.Device.RawVeldridDevice.UpdateTexture(texture, level, 0, 0, 0, levelWidth, levelHeight, 1, mip, 0);
            }

            return texture;
        }

        private static uint NumberOfMipLevels(uint width, uint height)
        {
            var levels = 1u;
            var size = Math.Max(width, height);
            while (size > 1)
            {
                size /= 2;
                levels++;
            }
            return levels;
        }

        //Each new pixel is the average of the (up to) 2x2 block of pixels it covers
        private static RgbaByte[] HalveSize(RgbaByte[] source, uint width, uint height, out uint newWidth, out uint newHeight)
        {
            newWidth = Math.Max(1u, width / 2);
            newHeight = Math.Max(1u, height / 2);

            var result = new RgbaByte[newWidth * newHeight];

            for (var y = 0u; y < newHeight; y++)
            {
                var y0 = Math.Min(y * 2, height - 1);
                var y1 = Math.Min(y * 2 + 1, height - 1);

                for (var x = 0u; x < newWidth; x++)
                {
                    var x0 = Math.Min(x * 2, width - 1);
                    var x1 = Math.Min(x * 2 + 1, width - 1);

                    var a = source[y0 * width + x0];
                    var b = source[y0 * width + x1];
                    var c = source[y1 * width + x0];
                    var d = source[y1 * width + x1];

                    result[y * newWidth + x] = new RgbaByte(
                        (byte)((a.R + b.R + c.R + d.R + 2) / 4),
                        (byte)((a.G + b.G + c.G + d.G + 2) / 4),
                        (byte)((a.B + b.B + c.B + d.B + 2) / 4),
                        (byte)((a.A + b.A + c.A + d.A + 2) / 4));
                }
            }

            return result;
        }
    }
}
