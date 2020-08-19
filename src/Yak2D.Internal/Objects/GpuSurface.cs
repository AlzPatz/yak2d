using Veldrid;

namespace Yak2D.Internal
{
    public class GpuSurface
    {
        public GpuSurfaceType Type { get; set; }
        public Texture Texture { get; set; }
        public TextureView TextureView { get; set; }
        public Framebuffer Framebuffer { get; set; }
        public ResourceSet ResourceSet_TexWrap { get; set; }
        public ResourceSet ResourceSet_TexMirror { get; set; }
    }
}
