using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class CustomVeldridStageModel : ICustomVeldridStageModel
    {
        public CustomVeldridBase CustomStage { get; private set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IVeldridWindowUpdater _windowUpdater;

        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public CustomVeldridStageModel(IFrameworkMessenger frameworkMessenger,
                                    ISystemComponents systemComponents,
                                    IVeldridWindowUpdater windowUpdater,
                                    CustomVeldridBase stage)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;
            _windowUpdater = windowUpdater;
            CustomStage = stage;

            Initialise();
        }

        private void Initialise()
        {
            CustomStage.Initialise(_systemComponents.Device.RawVeldridDevice, _systemComponents.Window.RawWindow, _systemComponents.Factory.RawFactory);
        }

        public void DestroyResources()
        {
            CustomStage.DisposeOfResources();
        }

        public void Update(float seconds)
        {
            CustomStage.Update(seconds, _windowUpdater.LatestWindowInputSnapshot);
        }
    }
}