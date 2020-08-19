using NSubstitute;
using Xunit;
using Yak2D.Graphics;

namespace Yak2D.Tests
{
    public class RenderQueueTest
    {
        [Fact]
        public void RenderQueue_ClearTargetColour_CatchNullInput()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            Assert.Throws<Yak2DException>(() => { queue.ClearColour(null, Colour.White); });
        }


        [Fact]
        public void RenderQueue_ClearTargetDepth_CatchNullInput()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            Assert.Throws<Yak2DException>(() => { queue.ClearDepth(null); });
        }

        [Fact]
        public void RenderQueue_DrawStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IDrawStage>();
            var camera = Substitute.For<ICamera2D>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Draw(null, camera, target); });
            Assert.Throws<Yak2DException>(() => { queue.Draw(stage, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Draw(stage, camera, null); });
        }

        [Fact]
        public void RenderQueue_ColourEffectStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IColourEffectsStage>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.ColourEffects(null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.ColourEffects(stage, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.ColourEffects(stage, source, null); });
        }

        [Fact]
        public void RenderQueue_BloomEffectStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IBloomStage>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Bloom(null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.Bloom(stage, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Bloom(stage, source, null); });
        }

        [Fact]
        public void RenderQueue_Blur2DEffectStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IBlurStage>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Blur(null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.Blur(stage, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Blur(stage, source, null); });
        }

        [Fact]
        public void RenderQueue_Blur1DEffectStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IBlur1DStage>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Blur1D(null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.Blur1D(stage, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Blur1D(stage, source, null); });
        }

        [Fact]
        public void RenderQueue_CopyStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Copy(null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Copy(source, null); });
        }

        [Fact]
        public void RenderQueue_StyleEffectStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IStyleEffectsStage>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.StyleEffects(null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.StyleEffects(stage, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.StyleEffects(stage, source, null); });
        }

        [Fact]
        public void RenderQueue_MeshRenderStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IMeshRenderStage>();
            var camera = Substitute.For<ICamera3D>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.MeshRender(null, camera, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.MeshRender(stage, null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.MeshRender(stage, camera, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.MeshRender(stage, camera, source, null); });
        }

        [Fact]
        public void RenderQueue_DistortionStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IDistortionStage>();
            var camera = Substitute.For<ICamera2D>();
            var source = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Distortion(null, camera, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.Distortion(stage, null, source, target); });
            Assert.Throws<Yak2DException>(() => { queue.Distortion(stage, camera, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Distortion(stage, camera, source, null); });
        }

        [Fact]
        public void RenderQueue_MixStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<IMixStage>();
            var mix = Substitute.For<ITexture>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.Mix(null, null, null, null, null, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.Mix(stage, null, null, null, null, null, null); });
        }

        [Fact]
        public void RenderQueue_CustomShaderStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<ICustomShaderStage>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.CustomShader(null, null, null, null, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.CustomShader(stage, null, null, null, null, null); });
        }

        [Fact]
        public void RenderQueue_CustomVeldridStage_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            var stage = Substitute.For<ICustomVeldridStage>();
            var target = Substitute.For<IRenderTarget>();

            Assert.Throws<Yak2DException>(() => { queue.CustomVeldrid(null, null, null, null, null, target); });
            Assert.Throws<Yak2DException>(() => { queue.CustomVeldrid(stage, null, null, null, null, null); });
        }

        [Fact]
        public void RenderQueue_SetViewport_CatchNullInputs()
        {
            var commandQueue = Substitute.For<IRenderCommandQueue>();

            IRenderQueue queue = new RenderQueue(commandQueue);

            Assert.Throws<Yak2DException>(() => { queue.SetViewport(null); });
        }
    }
}