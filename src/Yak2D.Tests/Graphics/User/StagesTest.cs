using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Numerics;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class StagesTest
    {
        [Fact]
        public void Stages_DestroyViewport_CatchNull()
        {
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var viewportManager = Substitute.For<IViewportManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();

            IStages stages = new Stages(renderStageManager, viewportManager, renderStageVisitor);

            Assert.Throws<Yak2DException>(() => { stages.DestroyViewport(null); });
        }

        [Fact]
        public void Stages_DestroyStage_CatchNull()
        {
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var viewportManager = Substitute.For<IViewportManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();

            IStages stages = new Stages(renderStageManager, viewportManager, renderStageVisitor);

            Assert.Throws<Yak2DException>(() => { stages.DestroyStage(null); });
        }

        [Fact]
        public void Stages_RenderStageCache_TestOnePathToEnsureModelSearchFailThrows()
        {
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var viewportManager = Substitute.For<IViewportManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();

            renderStageManager.RetrieveStageModel(Arg.Any<ulong>()).ReturnsNull();

            IStages stages = new Stages(renderStageManager, viewportManager, renderStageVisitor);

            var stage = Substitute.For<IColourEffectsStage>();

            Assert.Throws<Yak2DException>(() => { stages.SetColourEffectsConfig(stage, new ColourEffectConfiguration()); });
        }

        [Fact]
        public void Stages_SetEffects_SimpleTestAllToThrowOnNullStage()
        {
            //Too many asserts for one test, but all are very simple, so laziness prevails

            var renderStageManager = Substitute.For<IRenderStageManager>();
            var viewportManager = Substitute.For<IViewportManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();

            IStages stages = new Stages(renderStageManager, viewportManager, renderStageVisitor);

            Assert.Throws<Yak2DException>(() => { stages.SetColourEffectsConfig(null, new ColourEffectConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetBloomConfig(null, new BloomEffectConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetBlurConfig(null, new BlurEffectConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetBlur1DConfig(null, new Blur1DEffectConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetStyleEffectsGroupConfig(null, new StyleEffectGroupConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetStyleEffectsPixellateConfig(null, new PixellateConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetStyleEffectsEdgeDetectionConfig(null, new EdgeDetectionConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetStyleEffectsStaticConfig(null, new StaticConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetStyleEffectsOldMovieConfig(null, new OldMovieConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetStyleEffectsCrtConfig(null, new CrtEffectConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetMeshRenderLightingProperties(null, new MeshRenderLightingPropertiesConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetMeshRenderLights(null, new MeshRenderLightConfiguration[1]); });
            Assert.Throws<Yak2DException>(() => { stages.SetMeshRenderMesh(null, new Vertex3D[1]); });
            Assert.Throws<Yak2DException>(() => { stages.SetDistortionConfig(null, new DistortionEffectConfiguration()); });
            Assert.Throws<Yak2DException>(() => { stages.SetMixStageProperties(null, Vector4.One); });
            Assert.Throws<Yak2DException>(() => { stages.SetCustomShaderUniformValues(null, "none", new Vertex2D()); });
            Assert.Throws<Yak2DException>(() => { stages.SetCustomShaderUniformValues(null, "none", new Vertex2D[1]); });
        }
    }
}