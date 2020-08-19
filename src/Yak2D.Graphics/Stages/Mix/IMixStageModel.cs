using System.Numerics;

namespace Yak2D.Graphics
{
    public interface IMixStageModel : IRenderStageModel
    {
        Vector4 MixAmount { get; }

        void SetEffectTransition(ref Vector4 mixAmounts, ref float transitionSeconds, bool normalise);
    }
}