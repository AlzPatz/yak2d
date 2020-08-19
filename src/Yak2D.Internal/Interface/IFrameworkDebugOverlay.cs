using Veldrid;

namespace Yak2D.Internal
{
    public interface IFrameworkDebugOverlay
    {
        bool Visible { get; set; }

        void Render(CommandList cl, ISystemComponents components);
        void ReInitialise();
    }
}