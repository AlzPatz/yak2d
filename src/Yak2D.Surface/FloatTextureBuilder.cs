using System;
using System.Runtime.InteropServices;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Surface
{
    public class FloatTextureBuilder : IFloatTextureBuilder
    {
        private ISystemComponents _components;

        public FloatTextureBuilder(ISystemComponents components)
        {
            _components = components;
        }

        public Texture GenerateFloat32VeldridTextureFromPixelData(float[] data, uint width, uint height)
        {
            if (data == null)
            {
                throw new Yak2DException("Internal Framework Exception: Generating Texture from Pixel Data given a null data array", new ArgumentNullException("data"));
            }

            if (data.Length != width * height)
            {
                throw new Yak2DException("Internal Framework Exception: Generating Texture from Pixel Data mismatch between data array size and texture dimensions");
            }

            return Build(data, width, height);
        }

        private unsafe Texture Build(float[] data, uint width, uint height)
        {
            Veldrid.Texture texture = _components.Factory.CreateTexture(TextureDescription.Texture2D(
                width, height, 1, 1, PixelFormat.R32_Float, TextureUsage.Sampled));

            Span<float> span = data; //Not needed, as conversion of simple array implicit to span
            var pixelSizeInBytes = 4u; //Not needed, but just explains what the magic four would be for, geddit? :)

            fixed (void* pin = &MemoryMarshal.GetReference(span))
            {
                _components.Device.UpdateTexture(
                    texture,
                    (IntPtr)pin,
                    pixelSizeInBytes * width * height,
                    0,
                    0,
                    0,
                    width,
                    height,
                    1,
                    0,
                    0);
            }

            return texture;
        }
    }
}
