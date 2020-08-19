using Yak2D.Internal;

namespace Yak2D.Core
{
    public class Backend : IBackend
    {
        private readonly ISystemComponents _systemComponents;
        private readonly IResourceReinitialiser _systemResourcesReinitialiser;

        public GraphicsApi GraphicsApi { get => _systemComponents.GraphicsApi; }

        public Backend(ISystemComponents systemComponents, IResourceReinitialiser systemResourcesReinitialiser)
        {
            _systemComponents = systemComponents;
            _systemResourcesReinitialiser = systemResourcesReinitialiser;
        }

        public bool IsGraphicsApiSupported(GraphicsApi api)
        {
            return _systemComponents.IsGraphicsApiSupported(api);
        }

        public void SetGraphicsApi(GraphicsApi api)
        {
            _systemComponents.SetGraphicsApi(api, _systemResourcesReinitialiser.ReInitialise);
        }
    }
}