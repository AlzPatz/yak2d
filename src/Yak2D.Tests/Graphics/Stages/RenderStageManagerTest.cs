using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class RenderStageManagerTest
    {
        private static RenderStageManager CreateManager(IDrawStageModel model)
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var ids = Substitute.For<IIdGenerator>();
            ids.New().Returns(1UL);
            var factory = Substitute.For<IRenderStageModelFactory>();
            factory.CreateDrawStageModel(Arg.Any<BlendState>()).Returns(model);

            return new RenderStageManager(messenger, ids, Substitute.For<ISystemComponents>(), factory,
                                          new SimpleDictionaryCollectionFactory(messenger));
        }

        [Fact]
        public void RenderStageManager_DestroyStageThenShutdownInSameFrame_DestroysOnceWithoutThrowing()
        {
            var model = Substitute.For<IDrawStageModel>();
            var manager = CreateManager(model);

            var stage = manager.CreateDrawStage(true, BlendState.Alpha);
            manager.DestroyStage(stage.Id);

            var exception = Record.Exception(() => manager.Shutdown());

            Assert.Null(exception);
            model.Received(1).DestroyResources();
            Assert.Equal(0, manager.StageCount);
        }

        [Fact]
        public void RenderStageManager_DestroyStageThenReInitialiseInSameFrame_DestroysOnceWithoutThrowing()
        {
            var model = Substitute.For<IDrawStageModel>();
            var manager = CreateManager(model);

            var stage = manager.CreateDrawStage(true, BlendState.Alpha);
            manager.DestroyStage(stage.Id);

            var exception = Record.Exception(() => manager.ReInitialise());

            Assert.Null(exception);
            model.Received(1).DestroyResources();
        }

        [Fact]
        public void RenderStageManager_DestroySameStageTwiceInSameFrame_DestroysOnce()
        {
            var model = Substitute.For<IDrawStageModel>();
            var manager = CreateManager(model);

            var stage = manager.CreateDrawStage(true, BlendState.Alpha);
            manager.DestroyStage(stage.Id);
            manager.DestroyStage(stage.Id);
            manager.ProcessPendingDestruction();

            model.Received(1).DestroyResources();
        }
    }
}
