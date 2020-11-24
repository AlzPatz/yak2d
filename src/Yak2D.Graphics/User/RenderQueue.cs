using System;
using Veldrid;
using Yak2D.Utility;

namespace Yak2D.Graphics
{
    public class RenderQueue : IRenderQueue
    {
        private IRenderCommandQueue _commandQueue;

        public RenderQueue(IRenderCommandQueue renderQueue)
        {
            _commandQueue = renderQueue;
        }

        public void ClearColour(IRenderTarget target, Colour colour)
        {
            if (target == null)
            {
                throw new Yak2DException("Cannot clear render target Colour. Target null", new ArgumentNullException());
            }

            _commandQueue.Add(
                    RenderCommandType.ClearColourTarget,
                    0UL,
                    target.Id,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    ColourConverter.ConvertToRgbaFloat(colour));
        }

        public void ClearColour(ulong target, Colour colour)
        {
            _commandQueue.Add(
                    RenderCommandType.ClearColourTarget,
                    0UL,
                    target,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    ColourConverter.ConvertToRgbaFloat(colour));
        }

        public void ClearDepth(IRenderTarget target)
        {
            if (target == null)
            {
                throw new Yak2DException("Cannot clear render target depth. Target null", new ArgumentNullException());
            }

            _commandQueue.Add(
                    RenderCommandType.ClearDepthTarget,
                    0UL,
                    target.Id,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    RgbaFloat.Clear);
        }

        public void ClearDepth(ulong target)
        {
            _commandQueue.Add(
                    RenderCommandType.ClearDepthTarget,
                    0UL,
                    target,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    RgbaFloat.Clear);
        }

        public void Draw(IDrawStage stage, ICamera2D camera, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue DrawStage. Stage is null", new ArgumentNullException());
            }

            if (camera == null)
            {
                throw new Yak2DException("Unable to queue DrawStage. Camera is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue DrawStage. Target is null", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.DrawStage,
                                stage.Id,
                                target.Id,
                                camera.Id,
                                0UL,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Draw(ulong stage, ulong camera, ulong target)
        {
            _commandQueue.Add(RenderCommandType.DrawStage,
                                stage,
                                target,
                                camera,
                                0UL,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Distortion(IDistortionStage stage, ICamera2D camera, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue DistortionStage. Stage is null", new ArgumentNullException());
            }

            if (camera == null)
            {
                throw new Yak2DException("Unable to queue DistortionStage. Camera is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue DistortionStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue DistortionStage. Target is null", new ArgumentNullException());
            }

            if(source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue DistortionStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.DistortionStage,
                                stage.Id,
                                target.Id,
                                camera.Id,
                                source.Id,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Distortion(ulong stage, ulong camera, ulong source, ulong target)
        {
            if (source == target)
            {
                throw new Yak2DException("Unable to queue DistortionStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.DistortionStage,
                                stage,
                                target,
                                camera,
                                source,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void ColourEffects(IColourEffectsStage stage, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue ColourEffectStage. Stage is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue ColourEffectStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue ColourEffectStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue ColourEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.ColourEffectStage,
                                stage.Id,
                                target.Id,
                                0UL,
                                source.Id,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void ColourEffects(ulong stage, ulong source, ulong target)
        {
            if (source== target)
            {
                throw new Yak2DException("Unable to queue ColourEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.ColourEffectStage,
                                stage,
                                target,
                                0UL,
                                source,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Bloom(IBloomStage stage, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue BloomEffectStage. Stage is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue BloomEffectStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue BloomEffectStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue BloomEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.BloomEffectStage,
                                stage.Id,
                                target.Id,
                                0UL,
                                source.Id,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Bloom(ulong stage, ulong source, ulong target)
        {
            if (source == target)
            {
                throw new Yak2DException("Unable to queue BloomEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.BloomEffectStage,
                                stage,
                                target,
                                0UL,
                                source,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Blur(IBlurStage stage, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue Blur2DEffectStage. Stage is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue Blur2DEffectStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue Blur2DEffectStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue Blur2DEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.Blur2DEffectStage,
                                stage.Id,
                                target.Id,
                                0UL,
                                source.Id,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Blur(ulong stage, ulong source, ulong target)
        {
            if (source == target)
            {
                throw new Yak2DException("Unable to queue Blur2DEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.Blur2DEffectStage,
                                 stage,
                                 target,
                                 0UL,
                                 source,
                                 0UL,
                                 0UL,
                                 0UL,
                                 RgbaFloat.Clear);
        }

        public void Blur1D(IBlur1DStage stage, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue Blur1DEffectStage. Stage is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue Blur1DEffectStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue Blur1DEffectStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue Blur1DEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.Blur1DEffectStage,
                                stage.Id,
                                target.Id,
                                0UL,
                                source.Id,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }


        public void Blur1D(ulong stage, ulong source, ulong target)
        {
            if (source == target)
            {
                throw new Yak2DException("Unable to queue Blur1DEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.Blur1DEffectStage,
                                stage,
                                target,
                                0UL,
                                source,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Copy(ITexture source, IRenderTarget target)
        {
            if (source == null)
            {
                throw new Yak2DException("Unable to queue CopyStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue CopyStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue CopyStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.CopyStage,
                                0UL,
                                target.Id,
                                0UL,
                                source.Id,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void Copy(ulong source, ulong target)
        {
            if (source == target)
            {
                throw new Yak2DException("Unable to queue CopyStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.CopyStage,
                                0UL,
                                target,
                                0UL,
                                source,
                                0UL,
                                0UL,
                                0UL,
                                RgbaFloat.Clear);
        }

        public void StyleEffects(IStyleEffectsStage stage, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue StyleEffectStage. Stage is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue StyleEffectStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue StyleEffectStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue StyleEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.StyleEffect,
                            stage.Id,
                            target.Id,
                            0UL,
                            source.Id,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear);
        }

        public void StyleEffects(ulong stage, ulong source, ulong target)
        {
            if (source== target)
            {
                throw new Yak2DException("Unable to queue StyleEffectStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.StyleEffect,
                            stage,
                            target,
                            0UL,
                            source,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear);
        }

        public void MeshRender(IMeshRenderStage stage, ICamera3D camera, ITexture source, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue MeshRenderStage. Stage is null", new ArgumentNullException());
            }

            if (camera == null)
            {
                throw new Yak2DException("Unable to queue MeshRenderStage. Camera is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue MeshRenderStage. Source is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue MeshRenderStage. Target is null", new ArgumentNullException());
            }

            if (source.Id == target.Id)
            {
                throw new Yak2DException("Unable to queue MeshRenderStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.MeshRender,
                            stage.Id,
                            target.Id,
                            camera.Id,
                            source.Id,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear);
        }

        public void MeshRender(ulong stage, ulong camera, ulong source, ulong target)
        {
            if (source == target)
            {
                throw new Yak2DException("Unable to queue MeshRenderStage. Source and Target Surfaces cannot be the same", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.MeshRender,
                            stage,
                            target,
                            camera,
                            source,
                            0UL,
                            0UL,
                            0UL,
                            RgbaFloat.Clear);
        }

        public void Mix(IMixStage stage, ITexture texMix, ITexture tex0, ITexture tex1, ITexture tex2, ITexture tex3, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue MixStage. Stage is null", new ArgumentNullException());
            }

            //Null Mix Texture just uses a White Pixel Texture as equal 1.0 mixing
            //if (texMix == null)
            //{
            //    throw new Yak2DException("Unable to queue MixStage. Mix Texture is null", new ArgumentNullException());
            //}

            if (target == null)
            {
                throw new Yak2DException("Unable to queue MixStage. Target is null", new ArgumentNullException());
            }

            CheckForDuplicatedSurfaces("MixStage", texMix, tex0, tex1, tex2, tex3, target);

            //We are hijacking the Queue command
            _commandQueue.Add(RenderCommandType.MixStage,
                       stage.Id,
                       target.Id,
                       tex0 == null ? 0UL : tex0.Id,
                       tex1 == null ? 0UL : tex1.Id,
                       tex2 == null ? 0UL : tex2.Id,
                       tex3 == null ? 0UL : tex3.Id,
                       texMix == null ? 0UL : texMix.Id,
                       RgbaFloat.Clear
                       );
        }

        private void CheckForDuplicatedSurfaces(string stage, params ITexture[] surfaces)
        {
            var numNotNull = 0;
            for(var n = 0; n < surfaces.Length; n++)
            {
                if(surfaces[n] != null)
                {
                    numNotNull++;
                }
            }

            var ids = new ulong[numNotNull];
            var index = 0;
            for(var n = 0; n < surfaces.Length; n++)
            {
                if(surfaces[n] != null)
                {
                    ids[index] = surfaces[n].Id;
                    index++;
                }
            }

            CheckForDuplicatedSurfaces(stage, ids);
        }

        private void CheckForDuplicatedSurfaces(string stage, params ulong[] ids)
        {
            //0UL surfaces are not being used so are not compared

            var len = ids.Length;
            for(var n = 0; n < len; n++)
            {
                if(ids[n] == 0UL)
                {
                    continue; 
                }

                for(var c = 0 ; c < len; c++)
                {
                    if(n == c)
                    {
                        continue;
                    }

                    if(ids[c] == 0UL)
                    {
                        continue;
                    }

                    if(ids[n] == ids[c])
                    {
                       throw new Yak2DException(string.Concat("Unable to queue ", stage, ". The same surface has been passed more than once"), new ArgumentNullException()); 
                    }
                }
            }
        }

        public void Mix(ulong stage, ulong texMix, ulong tex0, ulong tex1, ulong tex2, ulong tex3, ulong target)
        {
            CheckForDuplicatedSurfaces("MixStage", texMix, tex0, tex1, tex2, tex3, target);

            //We are hijacking the Queue command
            _commandQueue.Add(RenderCommandType.MixStage,
                       stage,
                       target,
                       tex0,
                       tex1,
                       tex2,
                       tex3,
                       texMix,
                       RgbaFloat.Clear
                       );
        }

        public void CustomShader(ICustomShaderStage stage, ITexture tex0, ITexture tex1, ITexture tex2, ITexture tex3, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue CustomShaderStage. Stage is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue CustomShaderStage. Target is null", new ArgumentNullException());
            }

            CheckForDuplicatedSurfaces("CustomShaderStage", tex0, tex1, tex2, tex3, target);

            _commandQueue.Add(RenderCommandType.CustomShader,
                       stage.Id,
                       target.Id,
                       tex0 == null ? 0UL : tex0.Id,
                       tex1 == null ? 0UL : tex1.Id,
                       tex2 == null ? 0UL : tex2.Id,
                       tex3 == null ? 0UL : tex3.Id,
                       0UL,
                       RgbaFloat.Clear
                       );
        }

        public void CustomShader(ulong stage, ulong tex0, ulong tex1, ulong tex2, ulong tex3, ulong target)
        {
            CheckForDuplicatedSurfaces("CustomShaderStage", tex0, tex1, tex2, tex3, target);

            _commandQueue.Add(RenderCommandType.CustomShader,
                       stage,
                       target,
                       tex0,
                       tex1,
                       tex2,
                       tex3,
                       0UL,
                       RgbaFloat.Clear
                       );
        }

        public void CustomVeldrid(ICustomVeldridStage stage, ITexture tex0, ITexture tex1, ITexture tex2, ITexture tex3, IRenderTarget target)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue CustomVeldridStage. Stage is null", new ArgumentNullException());
            }

            if (target == null)
            {
                throw new Yak2DException("Unable to queue CustomVeldridStage. Target is null", new ArgumentNullException());
            }

            CheckForDuplicatedSurfaces("CustomVeldridStage", tex0, tex1, tex2, tex3, target);

            _commandQueue.Add(RenderCommandType.CustomVeldrid,
                        stage.Id,
                        target == null ? 0UL : target.Id,
                        tex0 == null ? 0UL : tex0.Id,
                        tex1 == null ? 0UL : tex1.Id,
                        tex2 == null ? 0UL : tex2.Id,
                        tex3 == null ? 0UL : tex3.Id,
                        0UL,
                        RgbaFloat.Clear
                        );
        }

        public void CustomVeldrid(ulong stage, ulong tex0, ulong tex1, ulong tex2, ulong tex3, ulong target)
        {
            CheckForDuplicatedSurfaces("CustomVeldridStage", tex0, tex1, tex2, tex3, target);

            _commandQueue.Add(RenderCommandType.CustomVeldrid,
                        stage,
                        target,
                        tex0,
                        tex1,
                        tex2,
                        tex3,
                        0UL,
                        RgbaFloat.Clear
                        );
        }

        public void CopySurfaceData(ISurfaceCopyStage stage, ITexture source)
        {
            if (stage == null)
            {
                throw new Yak2DException("Unable to queue CopySurfaceData. Stage is null", new ArgumentNullException());
            }

            if (source == null)
            {
                throw new Yak2DException("Unable to queue CopySurfaceData. Source is null", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.GpuToCpuSurfaceCopy,
                       stage.Id,
                       source.Id,
                       0UL, //Highjacked the camera slot for a user integer
                       0UL,
                       0UL,
                       0UL,
                       0UL,
                       RgbaFloat.Clear
                       );
        }

        public void CopySurfaceData(ulong stage, ulong source)
        {
            _commandQueue.Add(RenderCommandType.GpuToCpuSurfaceCopy,
                    stage,
                    source,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    0UL,
                    RgbaFloat.Clear
                    );
        }

        public void SetViewport(IViewport viewport)
        {
            if (viewport == null)
            {
                throw new Yak2DException("Unable to SetViewport. Viewport is null", new ArgumentNullException());
            }

            _commandQueue.Add(RenderCommandType.SetViewport,
                               0UL,
                               0UL,
                               0UL,
                               0UL,
                               0UL,
                               viewport.Id,
                               0UL,
                               RgbaFloat.Clear);
        }


        public void SetViewport(ulong viewport)
        {
            _commandQueue.Add(RenderCommandType.SetViewport,
                               0UL,
                               0UL,
                               0UL,
                               0UL,
                               0UL,
                               viewport,
                               0UL,
                               RgbaFloat.Clear);
        }

        public void RemoveViewport()
        {
            _commandQueue.Add(RenderCommandType.ClearViewport,
                   0UL,
                   0UL,
                   0UL,
                   0UL,
                   0UL,
                   0UL,
                   0UL,
                   RgbaFloat.Clear);
        }
    }
}