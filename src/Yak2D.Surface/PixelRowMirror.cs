namespace Yak2D.Surface
{
    //OpenGL stores texture rows bottom-up, so pixel data ordered top row first is mirrored over the x axis before upload
    //Shared by file / stream loads (ImageLoader) and the user raw pixel data creators (Surfaces)
    public static class PixelRowMirror
    {
        public static bool IsRequired(GraphicsApi backend)
        {
            return backend == GraphicsApi.OpenGL || backend == GraphicsApi.OpenGLES;
        }

        //Returns the same array when no mirroring is required, otherwise a new mirrored array (input is never mutated)
        public static T[] MirrorOverXAxisIfGraphicsApiRequires<T>(GraphicsApi backend, uint width, uint height, T[] data)
        {
            if (IsRequired(backend))
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
    }
}
