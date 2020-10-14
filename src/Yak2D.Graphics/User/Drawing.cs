using System.Drawing;
using System.Numerics;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class Drawing : IDrawing
    {
        public IDrawingHelpers DrawingHelpers
        {
            get
            {
                if (_drawingHelpers == null)
                {
                    _drawingHelpers = new DrawingHelpers(this, new CommonOperations());
                }
                return _drawingHelpers;

            }
        }
        private IDrawingHelpers _drawingHelpers;
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IRenderStageManager _renderStageManager;
        private readonly IRenderStageVisitor _renderStageVisitor;
        private readonly IFontManager _fontManager;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;

        private ulong _cachedDrawStageKey;
        private ulong _cachedDistortionDrawStageKey;

        public Drawing(IFrameworkMessenger frameworkMessenger,
                       IRenderStageManager renderStageManager,
                       IRenderStageVisitor renderStageVisitor,
                       IFontManager fontManager,
                       IGpuSurfaceManager gpuSurfaceManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _renderStageManager = renderStageManager;
            _renderStageVisitor = renderStageVisitor;
            _fontManager = fontManager;
            _gpuSurfaceManager = gpuSurfaceManager;
        }

        private bool CacheRenderStageModelInVisitor(ulong id)
        {
            var model = _renderStageManager.RetrieveStageModel(id);
            model?.CacheInstanceInVisitor(_renderStageVisitor);
            return model != null;
        }

        public Size GetSurfaceDimensions(ITexture texture) => _gpuSurfaceManager.GetSurfaceDimensions(texture.Id);
        public Size GetSurfaceDimensions(ulong texture) => _gpuSurfaceManager.GetSurfaceDimensions(texture);

        public ITexture WrapTextureId(ulong id)
        {
            return new TextureReference(id);
        }

        public IPersistentDrawQueue CreatePersistentDrawQueue(IDrawStage stage, DrawRequest[] requests, bool validate = false)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to create persistent draw queue, null draw stage passed");
            }

            return CreatePersistentDrawQueue(stage.Id, requests, validate);
        }

        public IPersistentDrawQueue CreatePersistentDrawQueue(ulong stage, DrawRequest[] requests, bool validate = false)
        {
            if (requests == null || requests.Length == 0)
            {
                throw new Yak2DException("Unable to create persistent draw queue, null or zero length requests array passed");
            }

            var internalRequests = new InternalDrawRequest[requests.Length];
            for (var r = 0; r < requests.Length; r++)
            {
                internalRequests[r] = new InternalDrawRequest
                {
                    CoordinateSpace = requests[r].CoordinateSpace,
                    FillType = requests[r].FillType,
                    Vertices = requests[r].Vertices,
                    Indices = requests[r].Indices,
                    Colour = requests[r].Colour,
                    Texture0 = requests[r].Texture0 == null ? 0UL : requests[r].Texture0.Id,
                    Texture1 = requests[r].Texture1 == null ? 0UL : requests[r].Texture1.Id,
                    TextureMode0 = requests[r].TextureWrap0,
                    TextureMode1 = requests[r].TextureWrap1,
                    Depth = requests[r].Depth,
                    Layer = requests[r].Layer
                };
            }
            if (CacheRenderStageModelInVisitor(stage))
            {
                return _renderStageVisitor.CachedDrawStageModel?.AddPersistentQueue(internalRequests, validate);
            }
            else
            {
                throw new Yak2DException("Unable to create persistent draw queue, draw stage provided was invalid");
            }
        }

        public void RemovePersistentDrawQueue(IDrawStage stage, IPersistentDrawQueue queue)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to remove persistent draw queue, null draw stage provided");
            }

            if (queue == null)
            {
                throw new Yak2DException("Unable to remove persistent draw queue, null queue provided");
            }

            RemovePersistentDrawQueue(stage.Id, queue.Id);
        }

        public void RemovePersistentDrawQueue(ulong stage, ulong queue)
        {
            if (CacheRenderStageModelInVisitor(stage))
            {
                _renderStageVisitor.CachedDrawStageModel?.RemovePersistentQueue(queue);
            }
            else
            {
                throw new Yak2DException("Unable to remove persistent draw queue, draw stage provided was invalid");
            }
        }

        public void ClearDynamicDrawRequestQueue(IDrawStage stage)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to clear dynamic draw queue, null draw stage provided");
            }

            ClearDynamicDrawRequestQueue(stage.Id);
        }

        public void ClearDynamicDrawRequestQueue(ulong stage)
        {
            if (CacheRenderStageModelInVisitor(stage))
            {
                _renderStageVisitor.CachedDrawStageModel?.ClearDynamicDrawQueue();
            }
            else
            {
                throw new Yak2DException("Unable to clear dynamic draw queue, draw stage provided was invalid");
            }
        }

        public void Draw(IDrawStage stage,
                         CoordinateSpace target,
                         FillType type,
                         Vertex2D[] vertices,
                         int[] indices,
                         Colour colour,
                         ITexture texture0,
                         ITexture texture1,
                         TextureCoordinateMode texWrap0,
                         TextureCoordinateMode texWrap1,
                         float depth,
                         int layer,
                         bool validate = false)
        {
            var request = new DrawRequest
            {
                CoordinateSpace = target,
                FillType = type,
                Vertices = vertices,
                Indices = indices,
                Colour = colour,
                Texture0 = texture0,
                Texture1 = texture1,
                TextureWrap0 = texWrap0,
                TextureWrap1 = texWrap1,
                Depth = depth,
                Layer = layer
            };

            Draw(stage, ref request, validate);
        }

        public void Draw(ulong stage,
                         CoordinateSpace target,
                         FillType type,
                         Vertex2D[] vertices,
                         int[] indices,
                         Colour colour,
                         ulong texture0,
                         ulong texture1,
                         TextureCoordinateMode texWrap0,
                         TextureCoordinateMode texWrap1,
                         float depth,
                         int layer,
                         bool validate = false)
        {
            var request = new DrawRequest
            {
                CoordinateSpace = target,
                FillType = type,
                Vertices = vertices,
                Indices = indices,
                Colour = colour,
                Texture0 = WrapTextureId(texture0),
                Texture1 = WrapTextureId(texture1),
                TextureWrap0 = texWrap0,
                TextureWrap1 = texWrap1,
                Depth = depth,
                Layer = layer
            };

            Draw(stage, ref request, validate);
        }

        public void Draw(IDrawStage stage, DrawRequest request, bool validate = false)
        {
            Draw(stage, ref request, validate);
        }

        public void Draw(ulong stage, DrawRequest request, bool validate = false)
        {
            Draw(stage, ref request, validate);
        }

        public void Draw(IDrawStage stage, ref DrawRequest request, bool validate = false)
        {
            if (stage == null)
            {
                throw new Yak2DException("Draw request failed. Null Draw Stage");
            }

            Draw(stage.Id, ref request, validate);
        }

        public void Draw(ulong stage, ref DrawRequest request, bool validate = false)
        {
            //Deeper validation, if requested, is done at queue level. 
            //Light / null check validation is always performed at this level. 
            //Note -> No validation steps ensure non-null/zero texture references have matching keys in texture collections

            if (request.FillType == FillType.Textured && (request.Texture0 == null || request.Texture0?.Id == 0UL))
            {
                throw new Yak2DException("Draw request failed. Invalid Texture0 input");
            }
            else if (request.FillType == FillType.DualTextured)
            {
                if (request.Texture0 == null || request.Texture0?.Id == 0UL || request.Texture1 == null || request.Texture1?.Id == 0UL)
                {
                    throw new Yak2DException("Draw request failed. Invalid Texture input for dual texturing (textures either null or Ids are zero)");
                }

                if (request.Texture0.Id == request.Texture1.Id)
                {
                    throw new Yak2DException("Draw request failed. The same Texture cannot be used for both Textures during Dual Texturing");
                }
            }

            if (request.Vertices == null)
            {
                throw new Yak2DException("Draw request failed. Null Vertex data");
            }

            if (request.Indices == null)
            {
                throw new Yak2DException("Draw request failed. Null Index data");
            }

            if (request.Texture0 == null)
            {
                request.Texture0 = new TextureReference(0UL);
            }

            if (request.Texture1 == null)
            {
                request.Texture1 = new TextureReference(0UL);
            }

            if (_cachedDrawStageKey != stage || _renderStageVisitor.CachedDrawStageModel == null)
            {
                _cachedDrawStageKey = stage;
                var model = _renderStageManager.RetrieveStageModel(_cachedDrawStageKey);
                if (model == null)
                {
                    throw new Yak2DException("Draw request failed. Requested DrawStage does not exist");
                }
                model.CacheInstanceInVisitor(_renderStageVisitor);
            }

            var t0 = request.Texture0.Id;
            var t1 = request.Texture1.Id;

            _renderStageVisitor.CachedDrawStageModel?.DrawToDynamicQueue(ref request.CoordinateSpace,
                                                                         ref request.FillType,
                                                                         ref request.Colour,
                                                                         ref request.Vertices,
                                                                         ref request.Indices,
                                                                         ref t0,
                                                                         ref t1,
                                                                         ref request.TextureWrap0,
                                                                         ref request.TextureWrap1,
                                                                         ref request.Depth,
                                                                         ref request.Layer);
        }

        public void DrawString(IDrawStage stage,
                        CoordinateSpace target,
                        string text,
                        Colour colour,
                        float fontSize,
                        Vector2 position,
                        TextJustify justify,
                        float depth,
                        int layer,
                        IFont font = null,
                        bool usKerningsWhereAvaliable = true)
        {
            var fntFamily = font == null ? _fontManager.SystemFont : _fontManager.RetrieveFont(font.Id);

            DrawString(stage.Id,
                       ref target,
                       ref text,
                       colour,
                       ref fontSize,
                       ref position,
                       ref justify,
                       ref depth,
                       ref layer,
                       fntFamily,
                       ref usKerningsWhereAvaliable);
        }

        public void DrawString(ulong stage,
                               CoordinateSpace target,
                               string text,
                               Colour colour,
                               float fontSize,
                               Vector2 position,
                               TextJustify justify,
                               float depth,
                               int layer,
                               ulong font,
                               bool usKerningsWhereAvaliable = true)
        {

            var fntFamily = _fontManager.RetrieveFont(font);

            DrawString(stage,
                       ref target,
                       ref text,
                       colour,
                       ref fontSize,
                       ref position,
                       ref justify,
                       ref depth,
                       ref layer,
                       fntFamily,
                       ref usKerningsWhereAvaliable);
        }

        public void DrawString(ulong stage,
                               CoordinateSpace target,
                               string text,
                               Colour colour,
                               float fontSize,
                               Vector2 position,
                               TextJustify justify,
                               float depth,
                               int layer,
                               bool usKerningsWhereAvaliable = true)
        {

            var fntFamily = _fontManager.SystemFont;

            DrawString(stage,
                       ref target,
                       ref text,
                       colour,
                       ref fontSize,
                       ref position,
                       ref justify,
                       ref depth,
                       ref layer,
                       fntFamily,
                       ref usKerningsWhereAvaliable);
        }

        private void DrawString(ulong stage,
                               ref CoordinateSpace target,
                               ref string text,
                               Colour colour,
                               ref float fontSize,
                               ref Vector2 position,
                               ref TextJustify justify,
                               ref float depth,
                               ref int layer,
                               IFontModel fntFamily,
                               ref bool usKerningsWhereAvaliable)
        {
            if (fntFamily == null)
            {
                return;
            }

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
                if (usKerningsWhereAvaliable && c != 0 && fnt.HasKernings)
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

                var tex = fnt.Textures[page].Id;
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

                Draw(stage,
                    target,
                    FillType.Textured,
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
                    colour,
                    tex,
                    0UL,
                    TextureCoordinateMode.Mirror,
                    TextureCoordinateMode.Mirror,
                    depth,
                    layer);

                x_curr += scalar * chr.XAdvance;
            }
        }

        public float MeasureStringLength(string text, float fontSize, IFont font = null)
        {
            var fnt = font == null ? _fontManager.SystemFont : _fontManager.RetrieveFont(font.Id);

            if (fnt == null)
            {
                return 0.0f;
            }

            return MeasureString(text, fontSize, fnt);
        }

        public float MeasureStringLength(string text, float fontSize, ulong font)
        {
            var fnt = _fontManager.RetrieveFont(font);

            if (fnt == null)
            {
                return 0.0f;
            }

            return MeasureString(text, fontSize, fnt);
        }

        public float MeasureStringLength(string text, float fontSize)
        {
            var fnt = _fontManager.SystemFont;

            if (fnt == null)
            {
                return 0.0f;
            }

            return MeasureString(text, fontSize, fnt);
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

        public IPersistentDistortionQueue CreatePersistentDistortQueue(IDistortionStage stage,
                                                                       DistortionDrawRequest[] requests,
                                                                       bool validate = false)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to create persistent distortion draw queue, null draw stage passed");
            }

            return CreatePersistentDistortQueue(stage.Id, requests, validate);
        }

        public IPersistentDistortionQueue CreatePersistentDistortQueue(ulong stage,
                                                                      DistortionDrawRequest[] requests,
                                                                      bool validate = false)
        {
            if (requests == null || requests.Length == 0)
            {
                throw new Yak2DException("Unable to create persistent distortion draw queue, null or zero length requests array passed");
            }

            var drawRequests = new InternalDrawRequest[requests.Length];
            for (var r = 0; r < requests.Length; r++)
            {
                drawRequests[r] = new InternalDrawRequest
                {
                    CoordinateSpace = requests[r].CoordinateSpace,
                    FillType = requests[r].FillType,
                    Vertices = requests[r].Vertices,
                    Indices = requests[r].Indices,
                    Colour = requests[r].Colour,
                    Texture0 = requests[r].Texture0.Id,
                    Texture1 = requests[r].Texture1.Id,
                    TextureMode0 = requests[r].TextureWrap0,
                    TextureMode1 = requests[r].TextureWrap1,
                    Depth = 0.0f,
                    Layer = 0
                };
            }

            if (CacheRenderStageModelInVisitor(stage))
            {
                var drawQueueTag = _renderStageVisitor.CachedDistortionEffectStageModel?.AddPersistentQueue(drawRequests, validate);
                return new PersistentDistortionQueueReference(drawQueueTag.Id);
            }
            else
            {
                throw new Yak2DException("Unable to create persistent distortion draw queue, draw stage provided was invalid");
            }
        }

        public void RemovePersistentDistortQueue(IDistortionStage stage, IPersistentDistortionQueue queue)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to remove persistent distortion draw queue, null draw stage provided");
            }

            if (queue == null)
            {
                throw new Yak2DException("Unable to remove persistent distortion draw queue, null queue provided");
            }

            RemovePersistentDistortQueue(stage.Id, queue.Id);
        }

        public void RemovePersistentDistortQueue(ulong stage, ulong queue)
        {
            if (CacheRenderStageModelInVisitor(stage))
            {
                _renderStageVisitor.CachedDistortionEffectStageModel?.RemovePersistentQueue(queue);
            }
            else
            {
                throw new Yak2DException("Unable to remove persistent distortion draw queue, draw stage provided was invalid");
            }
        }

        public void ClearDynamicDistortionRequestQueue(IDistortionStage stage)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to clear dynamic distortion draw queue, null draw stage provided");
            }

            ClearDynamicDistortionRequestQueue(stage.Id);
        }

        public void ClearDynamicDistortionRequestQueue(ulong stage)
        {
            if (CacheRenderStageModelInVisitor(stage))
            {
                _renderStageVisitor.CachedDistortionEffectStageModel?.ClearDynamicDrawQueue();
            }
            else
            {
                throw new Yak2DException("Unable to clear dynamic distortion draw queue, draw stage provided was invalid");
            }
        }

        public void DrawDistortion(IDistortionStage stage,
                                    CoordinateSpace target,
                                    FillType type,
                                    Vertex2D[] vertices,
                                    int[] indices,
                                    Colour colour,
                                    ITexture texture0,
                                    ITexture texture1,
                                    TextureCoordinateMode texWrap0,
                                    TextureCoordinateMode texWrap1,
                                    float intensity,
                                    bool validate = false)
        {
            var request = new DistortionDrawRequest
            {
                CoordinateSpace = target,
                FillType = type,
                Vertices = vertices,
                Indices = indices,
                Colour = new Colour(colour.R * intensity,
                                    colour.G * intensity,
                                    colour.B * intensity,
                                    colour.A * intensity),
                Texture0 = texture0,
                Texture1 = texture1,
                TextureWrap0 = texWrap0,
                TextureWrap1 = texWrap1
            };

            DrawDistortion(stage, ref request, validate);
        }

        public void DrawDistortion(ulong stage,
                                   CoordinateSpace target,
                                   FillType type,
                                   Vertex2D[] vertices,
                                   int[] indices,
                                   Colour colour,
                                   ulong texture0,
                                   ulong texture1,
                                   TextureCoordinateMode texWrap0,
                                   TextureCoordinateMode texWrap1,
                                   float intensity,
                                   bool validate = false)
        {
            var request = new DistortionDrawRequest
            {
                CoordinateSpace = target,
                FillType = type,
                Vertices = vertices,
                Indices = indices,
                Colour = new Colour(colour.R * intensity,
                                    colour.G * intensity,
                                    colour.B * intensity,
                                    colour.A * intensity),
                Texture0 = WrapTextureId(texture0),
                Texture1 = WrapTextureId(texture1),
                TextureWrap0 = texWrap0,
                TextureWrap1 = texWrap1
            };

            DrawDistortion(stage, ref request, validate);
        }

        public void DrawDistortion(IDistortionStage stage, DistortionDrawRequest request, bool validate = false)
        {
            DrawDistortion(stage, ref request, validate);
        }

        public void DrawDistortion(ulong stage, DistortionDrawRequest request, bool validate = false)
        {
            DrawDistortion(stage, ref request, validate);
        }

        public void DrawDistortion(IDistortionStage stage, ref DistortionDrawRequest request, bool validate = false)
        {
            if (stage == null)
            {
                throw new Yak2DException("Distortion Draw request failed. Null Draw Stage");
            }

            DrawDistortion(stage.Id, ref request, validate);
        }

        public void DrawDistortion(ulong stage, ref DistortionDrawRequest request, bool validate = false)
        {
            //Deeper validation, if requested, is done at queue level. 
            //Light / null check validation is always performed at this level. 
            //Note -> No validation steps ensure non-null/zero texture references have matching keys in texture collections

            if (request.FillType == FillType.Textured && (request.Texture0 == null || request.Texture0?.Id == 0UL))
            {
                throw new Yak2DException("DistortionDraw request failed. Invalid Texture0 input");
            }
            else if (request.FillType == FillType.DualTextured)
            {
                if (request.Texture0 == null || request.Texture0?.Id == 0UL || request.Texture1 == null || request.Texture1?.Id == 0UL)
                {
                    throw new Yak2DException("DistortionDraw request failed. Invalid Texture input for dual texturing (textures either null or Ids are zero)");
                }

                if (request.Texture0.Id == request.Texture1.Id)
                {
                    throw new Yak2DException("DistortionDraw request failed. The same Texture cannot be used for both Textures during Dual Texturing");
                }
            }

            if (request.Vertices == null)
            {
                throw new Yak2DException("DistortionDraw request failed. Null Vertex data");
            }

            if (request.Indices == null)
            {
                throw new Yak2DException("DistortionDraw request failed. Null Index data");
            }

            if (request.Texture0 == null)
            {
                request.Texture0 = new TextureReference(0UL);
            }

            if (request.Texture1 == null)
            {
                request.Texture1 = new TextureReference(0UL);
            }

            if (_cachedDistortionDrawStageKey != stage || _renderStageVisitor.CachedDistortionEffectStageModel == null)
            {
                _cachedDistortionDrawStageKey = stage;
                var model = _renderStageManager.RetrieveStageModel(_cachedDistortionDrawStageKey);
                if (model == null)
                {
                    throw new Yak2DException("DistortionDraw request failed. Requested DistortionDrawStage does not exist");
                }
                model.CacheInstanceInVisitor(_renderStageVisitor);
            }

            var t0 = request.Texture0.Id;
            var t1 = request.Texture1.Id;
            var depth = 0.0f;
            var layer = 0;

            _renderStageVisitor.CachedDistortionEffectStageModel?.DrawToDynamicQueue(ref request.CoordinateSpace,
                                                                                     ref request.FillType,
                                                                                     ref request.Colour,
                                                                                     ref request.Vertices,
                                                                                     ref request.Indices,
                                                                                     ref t0,
                                                                                     ref t1,
                                                                                     ref request.TextureWrap0,
                                                                                     ref request.TextureWrap1,
                                                                                     ref depth,
                                                                                     ref layer);
        }
    }
}