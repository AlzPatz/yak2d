using Veldrid;

namespace Yak2D.Graphics
{
    public interface ICommandProcessor
    {
        void Process(CommandList cl, RenderCommandQueueItem command);
    }
}