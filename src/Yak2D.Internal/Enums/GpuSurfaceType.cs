using System;

namespace Yak2D.Internal
{
    [Flags]
    public enum GpuSurfaceType
    {
        SwapChainOutput = 1,
        Texture = 2,
        RenderTarget = 4,
        User = 8,
        Internal = 16,
        Undefined = 32
    }
}