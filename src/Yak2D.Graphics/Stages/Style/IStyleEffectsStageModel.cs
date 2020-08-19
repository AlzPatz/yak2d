using Veldrid;

namespace Yak2D.Graphics
{
    public interface IStyleEffectsStageModel : IRenderStageModel
    {
        DeviceBuffer PixellateBuffer { get; }
        PixellateConfiguration PixellateCurrent { get; }
        ResourceSet PixellateResourceSet { get; }
        ResourceSet EdgeDetectionResourceSet { get; }
        ResourceSet StaticResourceSet { get; }
        ResourceSet OldMovieResourceSet { get; }
        ResourceSet CrtEffectResourceSet { get; }
        void SetEffectGroupTransition(ref StyleEffectGroupConfiguration config, float transitionSeconds);
        void SetPixellateTransition(ref PixellateConfiguration config, float transitionSeconds);
        void SetEdgeDetectionTransition(ref EdgeDetectionConfiguration config, float transitionSeconds);
        void SetStaticTransition(ref StaticConfiguration config, float transitionSeconds);
        void SetOldMovieTransition(ref OldMovieConfiguration config, float transitionSeconds);
        void SetCrtEffectTransition(ref CrtEffectConfiguration config, float transitionSeconds);
    }
}