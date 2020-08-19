using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class FrameworkDebugOverlay : IFrameworkDebugOverlay
    {
        /*
        This render steps sits outside of the wider framework render pipeline
        It simply draws onto the framebuffer after the rest of the rending is done
        It uses some of the components in the wider framework, but is a simplifed copy of some parts
        Perhaps this stage should be fully wrapped into the earlier draw queue without code duplication
        But for now to produce simple text and shape drawing, this will suffice
        */

        public bool Visible { get; set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IDebugAnalytics _debugAnalytics;
        private readonly IFramesPerSecondMonitor _fpsMonitor;
        private readonly ISystemComponents _systemComponents;
        private readonly IDrawStageRenderer _drawStageRenderer;
        private readonly IIdGenerator _idGenerator;
        private readonly IDrawQueueFactory _drawQueueFactory;
        private readonly IDrawQueueGroupFactory _drawQueueGroupFactory;
        private readonly IQueueToBufferBlitter _queueToBufferBlitter;
        private readonly IDrawStageBuffersFactory _drawStageBuffersFactory;
        private readonly IDrawStageBatcherFactory _drawStageBatcherFactory;
        private readonly IFontManager _fontManager;

        private CameraModel2D _camera;
        private DrawStageModel _drawStageModel;

        public FrameworkDebugOverlay(IFrameworkMessenger frameworkMessenger,
                                        IDebugAnalytics debugAnalytics,
                                        IFramesPerSecondMonitor fpsMonitor,
                                        ISystemComponents systemComponents,
                                        IDrawStageRenderer drawStageRenderer,
                                        IIdGenerator idGenerator,
                                        IDrawQueueFactory drawQueueFactory,
                                        IDrawQueueGroupFactory drawQueueGroupFactory,
                                        IQueueToBufferBlitter queueToBufferBlitter,
                                        IDrawStageBuffersFactory drawStageBuffersFactory,
                                        IDrawStageBatcherFactory drawStageBatcherFactory,
                                        IFontManager fontManager
                                        )
        {
            _frameworkMessenger = frameworkMessenger;
            _debugAnalytics = debugAnalytics;
            _fpsMonitor = fpsMonitor;
            _systemComponents = systemComponents;
            _drawStageRenderer = drawStageRenderer;
            _idGenerator = idGenerator;
            _drawQueueFactory = drawQueueFactory;
            _drawQueueGroupFactory = drawQueueGroupFactory;
            _queueToBufferBlitter = queueToBufferBlitter;
            _drawStageBuffersFactory = drawStageBuffersFactory;
            _drawStageBatcherFactory = drawStageBatcherFactory;
            _fontManager = fontManager;

            Visible = false;

            ReInitialise();
        }

        public void ReInitialise()
        {
            _camera = new CameraModel2D(_systemComponents, 960, 540, 1.0f, Vector2.Zero);

            _drawStageModel = new DrawStageModel(_drawStageBatcherFactory.Create(),
                                                 _drawQueueGroupFactory.Create(false),
                                                 BlendState.Alpha);
        }

        public void Render(CommandList cl, ISystemComponents systemComponents)
        {
            var framebuffer = systemComponents.Device.SwapchainFramebuffer;

            _drawStageModel.ClearDynamicDrawQueue();

            CalculateAndDraw((int)framebuffer.Width, (int)framebuffer.Height);

            _drawStageModel.Process();

            cl.SetFramebuffer(framebuffer);

            cl.ClearDepthStencil(1.0f);

            var surface = new GpuSurface
            {
                Type = GpuSurfaceType.SwapChainOutput,
                Framebuffer = framebuffer,
                Texture = null,
                TextureView = null,
                ResourceSet_TexMirror = null,
                ResourceSet_TexWrap = null
            };

            _drawStageRenderer.Render(cl, _drawStageModel, surface, _camera);
        }

        private void CalculateAndDraw(int fb_width, int fb_height)
        {
            //Drawing will assume a 1920x1080 virtual space
            //Camera virtual resolution will be adjusted by aspect ratio to avoid distortion 
            //But would allow rendering to occur off the right side of the screen

            var ratio = 1.0f * fb_width / (1.0f * fb_height);

            var height = 1080.0f;

            var width = height * ratio;

            _camera.SetVirtualResolution((uint)width, (uint)height);

            var backgroundColour = new Colour(0.3f, 0.3f, 0.3f, 0.3f);

            var TitleColour = new Colour(204.0f / 256.0f, 0.0f, 0.0f, 1.0f);
            var Heading0Colour = new Colour(1.0f, 0.5f, 0.0f, 1.0f);
            var Heading1Colour = new Colour(1.0f, 1.0f, 102.0f / 256.0f, 1.0f);
            var KeyColour = new Colour(0.0f, 0.7f, 0.3f, 1.0f);
            var ValueColour = new Colour(0.2f, 0.9f, 0.4f, 1.0f);

            Draw(CoordinateSpace.Screen, backgroundColour, new Vertex2D[]
            {
                new Vertex2D { Position = new Vector2(-width / 2, height /2), TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = Colour.White },
                new Vertex2D { Position = new Vector2(width / 2, height /2), TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = Colour.White },
                new Vertex2D { Position = new Vector2(-width / 2, -height /2), TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = Colour.White },
                new Vertex2D { Position = new Vector2(width / 2, -height /2), TexCoord0 = Vector2.Zero, TexCoord1 = Vector2.Zero, TexWeighting = 0.0f, Colour = Colour.White }
            },
            new int[]
            {
                0, 1, 2, 2, 1, 3
            },
            0.5f,
            0);

            var fnt = _fontManager.SystemFont;

            var topLeft = new Vector2(-width / 2, height / 2);

            var spacer = 4.0f;

            var yShift = 14.0f;
            DrawString("Framework Debug Overlay", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 28.0f, TitleColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 14.0f;

            yShift += spacer;

            yShift += 12.0f;
            DrawString("Frames Per Second", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 24.0f, Heading1Colour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 12.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Update FPS:", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w01 = MeasureString("Update FPS: ", 22.0f, fnt);
            DrawString(_fpsMonitor.UpdateFps.ToString("0.0"), topLeft + new Vector2(w01, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Draw FPS: ", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w02 = MeasureString("Draw FPS: ", 22.0f, fnt);
            DrawString(_fpsMonitor.DrawFps.ToString("0.0"), topLeft + new Vector2(w02, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 12.0f;
            DrawString("Loop Timings", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 24.0f, Heading0Colour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 12.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Update Type:", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w0 = MeasureString("Update Type: ", 22.0f, fnt);
            DrawString(_debugAnalytics.Updater_TimestepType.ToString(), topLeft + new Vector2(w0, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Average Update Time: ", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w1 = MeasureString("Average Update Time: ", 22.0f, fnt);
            DrawString((_debugAnalytics.Updater_AverageFrameTime * 1000.0).ToString("0.00") + "ms", topLeft + new Vector2(w1, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Update Time Variance: ", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w2 = MeasureString("Update Time Variance: ", 22.0f, fnt);
            DrawString((_debugAnalytics.Updater_FrameTimeVariance * 1000.0).ToString("0.000"), topLeft + new Vector2(w2, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Update Processing Usage: ", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w3 = MeasureString("Update Processing Usage: ", 22.0f, fnt);
            DrawString((_debugAnalytics.Updater_UpdateProcessingPercentage * 100.0).ToString("0.0") + "%", topLeft + new Vector2(w3, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Update Overutilised Flag: ", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w4 = MeasureString("Update Overutilised Flag: ", 22.0f, fnt);
            DrawString(_debugAnalytics.Updater_OverutilisedFlag.ToString(), topLeft + new Vector2(w4, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

            yShift += spacer;

            yShift += 11.0f;
            DrawString("Update Underutilised Flag: ", topLeft + new Vector2(0.0f, -yShift), TextJustify.Left, 22.0f, KeyColour, 0.5f, 1, CoordinateSpace.Screen);
            var w5 = MeasureString("Update Underutilised Flag: ", 22.0f, fnt);
            DrawString(_debugAnalytics.Updater_OverutilisedFlag.ToString(), topLeft + new Vector2(w5, -yShift), TextJustify.Left, 22.0f, ValueColour, 0.5f, 1, CoordinateSpace.Screen);
            yShift += 11.0f;

        }

        private void Draw(CoordinateSpace coordinateSpace,
                          Colour colour,
                          Vertex2D[] vertices,
                          int[] indices,
                          float depth,
                          int layer,
                          FillType fillType = FillType.Coloured,
                          ulong t0 = 0UL,
                          ulong t1 = 0UL,
                          TextureCoordinateMode tm0 = TextureCoordinateMode.None,
                          TextureCoordinateMode tm1 = TextureCoordinateMode.None)
        {
            _drawStageModel.DrawToDynamicQueue(ref coordinateSpace,
                                                ref fillType,
                                                ref colour,
                                                ref vertices,
                                                ref indices,
                                                ref t0,
                                                ref t1,
                                                ref tm0,
                                                ref tm1,
                                                ref depth,
                                                ref layer);
        }

        private void DrawString(string text,
                                    Vector2 position,
                                    TextJustify justify,
                                    float fontSize,
                                    Colour colour,
                                    float depth,
                                    int layer = 0,
                                    CoordinateSpace coordinateSpace = CoordinateSpace.Screen)
        {
            var fntFamily = _fontManager.SystemFont;

            var fnt = fntFamily.SubFontAtSize(fontSize);

            var lineHeight = fontSize * (fnt.LineHeight / fnt.Size);

            float strLength = 0.0f;
            if (justify == TextJustify.Centre || justify == TextJustify.Right)
            {
                strLength = MeasureString(text, fontSize, fntFamily);
            }

            var x_curr = position.X;
            var y_curr = position.Y + (0.5f * fontSize);
            var scalar = fontSize / fnt.Size;

            if (justify == TextJustify.Right || justify == TextJustify.Centre)
            {
                switch (justify)
                {
                    case TextJustify.Right:
                        x_curr -= strLength;
                        break;
                    case TextJustify.Centre:
                        x_curr -= 0.5f * strLength;
                        break;
                }
            }

            var startx = (int)x_curr; //this forces pixel line ups.. maybe this is a toggle later

            for (var c = 0; c < text.Length; c++)
            {
                var ch = text[c];

                //Deal with specials (fucks up string measure and justify for now)
                switch (ch)
                {
                    case '\n':
                        x_curr = startx;
                        y_curr -= lineHeight;
                        continue;
                    case '\t':
                        x_curr += fontSize;
                        continue;
                }

                int xPosKerningAdjust = 0;
                if (c != 0 && fnt.HasKernings)
                {
                    var lastCh = text[c - 1];

                    if (fnt.Kernings.ContainsKey(lastCh))
                    {
                        var d0 = fnt.Kernings[lastCh];
                        if (d0.ContainsKey(ch))
                        {
                            xPosKerningAdjust = d0[ch];
                        }
                    }
                }

                FontCharacter chr;

                if (fnt.Characters.ContainsKey(ch))
                {
                    chr = fnt.Characters[ch];
                }
                else
                {
                    chr = fnt.Characters['?'];
                }

                var page = chr.Page;

                var tex = fnt.Textures[page];
                var ww = chr.Width * scalar;
                var hh = chr.Height * scalar;

                var x0 = x_curr + (scalar * (chr.XOffset + xPosKerningAdjust));
                var x1 = x0 + ww;
                var y0 = y_curr - (scalar * chr.YOffset);
                var y1 = y0 - hh;

                var xt0 = chr.X0;
                var yt0 = chr.Y0;
                var xt1 = chr.X1;
                var yt1 = chr.Y1;

                Draw(coordinateSpace,
                    colour,
                    new Vertex2D[]
                    {
                        new Vertex2D { Position = new Vector2(x0, y0), TexCoord0 = new Vector2(xt0, yt0), TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = Colour.White },
                        new Vertex2D { Position = new Vector2(x1, y0), TexCoord0 = new Vector2(xt1, yt0), TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour =  Colour.White },
                        new Vertex2D { Position = new Vector2(x0, y1), TexCoord0 = new Vector2(xt0, yt1), TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = Colour.White },
                        new Vertex2D { Position = new Vector2(x1, y1), TexCoord0 = new Vector2(xt1, yt1), TexCoord1 = Vector2.Zero, TexWeighting = 1.0f, Colour = Colour.White },

                    },
                    new int[]
                    {
                        0, 1, 2, 2, 1, 3
                    },
                    depth,
                    layer,
                    FillType.Textured,
                    tex.Id);

                x_curr += scalar * chr.XAdvance;
            }
        }

        private float MeasureString(string text, float fontSize, IFontModel font)
        {
            var fnt = font.SubFontAtSize(fontSize);

            var length = 0.0f;

            for (var c = 0; c < text.Length; c++)
            {
                var ch = text[c];

                if (fnt.Characters.ContainsKey(ch))
                {
                    length += fnt.Characters[ch].XAdvance;

                }
            }
            length *= fontSize / fnt.Size;
            return length;
        }
    }
}
