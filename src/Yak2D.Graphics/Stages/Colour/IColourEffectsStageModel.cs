using Veldrid;

namespace Yak2D.Graphics
{
    public interface IColourEffectsStageModel : IRenderStageModel
    {
        ResourceSet FactorsResourceSet { get; }
        RgbaFloat ClearColour { get; }
        bool ClearBackgroundBeforeRender { get; }
        void SetEffectTransition(ref ColourEffectConfiguration config, ref float transitionSeconds);
    }
}