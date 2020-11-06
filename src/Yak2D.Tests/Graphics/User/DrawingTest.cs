using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class DrawingTest
    {
        [Fact]
        public void Drawing_DrawHelpers_NotNull()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var helpers = drawing.Helpers;

            Assert.NotNull(helpers);
        }

        [Fact]
        public void Drawing_ValidatingInput_CatchNullDrawStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            IDrawStage stage = null;

            var request = new DrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                Depth = 0.0f,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Layer = 0,
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingInput_CatchInvalidTexturedInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDrawStage>();

            var request = new DrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                Depth = 0.0f,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Layer = 0,
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.FillType = FillType.Textured;

            request.Texture0 = null;
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });

            request.Texture0 = new TextureReference(0UL);
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingInput_CatchInvalidTextureInputDualTextured()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDrawStage>();

            var request = new DrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                Depth = 0.0f,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Layer = 0,
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.FillType = FillType.DualTextured;

            request.Texture1 = null;
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });

            request.Texture1 = new TextureReference(0UL);
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });

            request.Texture1 = new TextureReference(2UL);

            request.Texture0 = null;
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });

            request.Texture0 = new TextureReference(0UL);
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });

            request.Texture0 = new TextureReference(2UL);
            //Catch as both the same
            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingInput_CatchNullVerticesInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDrawStage>();

            var request = new DrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                Depth = 0.0f,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Layer = 0,
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.Vertices = null;

            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingInput_CatchNullIndicesInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDrawStage>();

            var request = new DrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                Depth = 0.0f,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Layer = 0,
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.Indices = null;

            Assert.Throws<Yak2DException>(() => { drawing.Draw(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingDistortionInput_CatchNullDistortionStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            IDistortionStage stage = null;

            var request = new DistortionDrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingDistortionInput_CatchInvalidTexturedInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            var request = new DistortionDrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.FillType = FillType.Textured;

            request.Texture0 = null;
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });

            request.Texture0 = new TextureReference(0UL);
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingDistortionInput_CatchInvalidTextureInputDualTextured()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            var request = new DistortionDrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.FillType = FillType.DualTextured;

            request.Texture1 = null;
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });

            request.Texture1 = new TextureReference(0UL);
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });

            request.Texture1 = new TextureReference(2UL);

            request.Texture0 = null;
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });

            request.Texture0 = new TextureReference(0UL);
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });

            request.Texture0 = new TextureReference(2UL);
            //Catch as both the same
            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingDistortionInput_CatchNullVerticesInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            var request = new DistortionDrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.Vertices = null;

            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });
        }

        [Fact]
        public void Drawing_ValidatingDistortionInput_CatchNullIndicesInput()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            var request = new DistortionDrawRequest
            {
                Colour = Colour.White,
                CoordinateSpace = CoordinateSpace.Screen,
                FillType = FillType.Coloured,
                Indices = new int[] { 0, 1, 2 },
                Texture0 = new TextureReference(1UL),
                Texture1 = new TextureReference(2UL),
                TextureWrap0 = TextureCoordinateMode.Mirror,
                TextureWrap1 = TextureCoordinateMode.Mirror,
                Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
            };

            request.Indices = null;

            Assert.Throws<Yak2DException>(() => { drawing.DrawDistortion(stage, ref request, false); });
        }

        //--------------

        [Fact]
        public void Drawing_CreatePersistentDistortQueue_CatchInvalidStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            var requests = new DistortionDrawRequest[]
            {
                new DistortionDrawRequest
                {
                    Colour = Colour.White,
                    CoordinateSpace = CoordinateSpace.Screen,
                    FillType = FillType.Coloured,
                    Indices = new int[] { 0, 1, 2 },
                    Texture0 = new TextureReference(1UL),
                    Texture1 = new TextureReference(2UL),
                    TextureWrap0 = TextureCoordinateMode.Mirror,
                    TextureWrap1 = TextureCoordinateMode.Mirror,
                    Vertices = new Vertex2D[] { new Vertex2D(), new Vertex2D(), new Vertex2D() }
                }
            };

            renderStageManager.RetrieveStageModel(Arg.Any<ulong>()).ReturnsNull();

            Assert.Throws<Yak2DException>(() => { drawing.CreatePersistentDistortQueue(stage, requests, false); });
        }

        [Fact]
        public void Drawing_CreatePersistentDistortQueue_CatchNullStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            IDistortionStage stage = null;

            Assert.Throws<Yak2DException>(() => { drawing.CreatePersistentDistortQueue(stage, null, false); });
        }

        [Fact]
        public void Drawing_CreatePersistentDistortQueue_CatchNullRequests()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            Assert.Throws<Yak2DException>(() => { drawing.CreatePersistentDistortQueue(stage, null, false); });
        }

        [Fact]
        public void Drawing_CreatePersistentDistortQueue_CatchZeroLengthRequests()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            Assert.Throws<Yak2DException>(() => { drawing.CreatePersistentDistortQueue(stage, new DistortionDrawRequest[] { }, false); });
        }

        [Fact]
        public void Drawing_RemovePersistentDistortQueue_CatchNullStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            IDistortionStage stage = null;
            var queue = Substitute.For<IPersistentDistortionQueue>();

            Assert.Throws<Yak2DException>(() => { drawing.RemovePersistentDistortQueue(stage, queue); });
        }

        [Fact]
        public void Drawing_RemovePersistentDistortQueue_CatchNullQueue()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();
            IPersistentDistortionQueue queue = null;

            Assert.Throws<Yak2DException>(() => { drawing.RemovePersistentDistortQueue(stage, queue); });
        }

        [Fact]
        public void Drawing_RemovePersistentDistortQueue_CatchInvalidStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();
            var queue = Substitute.For<IPersistentDistortionQueue>();

            renderStageManager.RetrieveStageModel(Arg.Any<ulong>()).ReturnsNull();

            Assert.Throws<Yak2DException>(() => { drawing.RemovePersistentDistortQueue(stage, queue); });
        }

        [Fact]
        public void Drawing_ClearDynamicDrawQueue_CatchNullStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            IDistortionStage stage = null;

            Assert.Throws<Yak2DException>(() => { drawing.ClearDynamicDistortionRequestQueue(stage); });
        }

        [Fact]
        public void Drawing_ClearDynamicDrawQueue_CatchInvalidStage()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();
            var renderStageManager = Substitute.For<IRenderStageManager>();
            var renderStageVisitor = Substitute.For<IRenderStageVisitor>();
            var fontManager = Substitute.For<IFontManager>();
            var gpuSurfaceManager = Substitute.For<IGpuSurfaceManager>();

            IDrawing drawing = new Drawing(messenger,
                                           renderStageManager,
                                           renderStageVisitor,
                                           fontManager,
                                           gpuSurfaceManager);

            var stage = Substitute.For<IDistortionStage>();

            renderStageManager.RetrieveStageModel(Arg.Any<ulong>()).ReturnsNull();

            Assert.Throws<Yak2DException>(() => { drawing.ClearDynamicDistortionRequestQueue(stage); });
        }
    }
}