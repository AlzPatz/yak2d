using Veldrid;

namespace Yak2D.Internal
{
    public interface IVeldridWindowUpdater
    {
        InputSnapshot LatestWindowInputSnapshot { get; }
        bool UpdateAndReturnIfWindowStillExists();
    }
}