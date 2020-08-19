using Veldrid;

namespace Yak2D.Graphics
{
    public interface IBlendStateConverter
    {
        BlendStateDescription Convert(BlendState state);
    }
}