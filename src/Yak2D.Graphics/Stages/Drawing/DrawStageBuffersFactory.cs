using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class DrawStageBuffersFactory : IDrawStageBuffersFactory
    {
        private readonly ISystemComponents _components;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;

        public DrawStageBuffersFactory(ISystemComponents components, IStartupPropertiesCache startUpPropertiesCache)
        {
            _components = components;
            _startUpPropertiesCache = startUpPropertiesCache;
        }

        public IDrawStageBuffers Create()
        {
            return new DrawStageBuffers(_components, _startUpPropertiesCache);
        }
    }
}