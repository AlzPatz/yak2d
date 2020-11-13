namespace Yak2D
{
    /// <summary>
	/// Builds the rendering queue
    /// Each call to this object will add a step to the queue
	/// </summary>
    public interface IRenderQueue
    {
        /// <summary>
        /// Sets all the pixels on render target to the chosen colour
        /// </summary>
        /// <param name="target">Render Target reference</param>
        /// <param name="colour">Clear colour</param>
        void ClearColour(IRenderTarget target,
                         Colour colour);

        /// <summary>
        /// Sets all the pixels on render target to the chosen colour
        /// </summary>
        /// <param name="target">Render Target id</param>
        /// <param name="colour">Clear colour</param>
        void ClearColour(ulong target,
                         Colour colour);

        /// <summary>
        /// Clears each depth buffer pixel to 1.0 on a render target
        /// </summary>
        /// <param name="target">Render Target reference</param>
        void ClearDepth(IRenderTarget target);

        /// <summary>
        /// Clears each depth buffer pixel to 1.0 on a render target
        /// </summary>
        /// <param name="target">Render Target reference</param>
        void ClearDepth(ulong target);

        /// <summary>
        /// Render a draw stage to a target surface
        /// </summary>
        /// <param name="stage">The DrawStage reference</param>
        /// <param name="camera">The 2D camera reference used to transform and position the draw vertices</param>
        /// <param name="target">The render target reference surface upon which to draw</param>
        void Draw(IDrawStage stage,
                  ICamera2D camera,
                  IRenderTarget target);

        /// <summary>
        /// Render a draw stage to a target surface
        /// </summary>
        /// <param name="stage">The DrawStage id</param>
        /// <param name="camera">The 2D camera id used to transform and position the draw vertices</param>
        /// <param name="target">The render target surface id upon which to draw</param>
        void Draw(ulong stage,
                  ulong camera,
                  ulong target);

        /// <summary>
        /// Applies a distortion effect to a texture source and render to a target
        /// The per-pixel distortion amount is generated from a distortion height map
        /// The distortion height map is drawn each frame, in a similar way that DrawStage draw requests are generated, queued and sorted
        /// The final distortion amount per-pixel is scaled by a configurable scalar quantity
        /// </summary>
        /// <param name="stage">The Distortion stage reference</param>
        /// <param name="camera">The 2D camera reference used to transform and position the draw vertices</param>
        /// <param name="source">The Texture reference of the source surface the effect will use</param>
        /// <param name="target">The render target surface reference that the post-effect result will be drawn too</param>
        void Distortion(IDistortionStage stage,
                        ICamera2D camera,
                        ITexture source,
                        IRenderTarget target);

        /// <summary>
        /// Applies a distortion effect to a texture source and render to a target
        /// The per-pixel distortion amount is generated from a distortion height map
        /// The distortion height map is drawn each frame, in a similar way that DrawStage draw requests are generated, queued and sorted
        /// The final distortion amount per-pixel is scaled by a configurable scalar quantity
        /// </summary>
        /// <param name="stage">The Distortion stage id</param>
        /// <param name="camera">The 2D camera id used to transform and position the draw vertices</param>
        /// <param name="source">The Texture id of the source surface the effect will use</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>
        void Distortion(ulong stage,
                        ulong camera,
                        ulong source,
                        ulong target);

        /// <summary>
        /// Apply simple colour modification effects to a texture source and render to a target
        /// Effects are: single colour mix, grayscale, negative, colourise and opacity
        /// A combination of all the effects can be applied together
        /// </summary>
        /// <param name="stage">The ColourEffects stage reference</param>
        /// <param name="source">The Texture reference of the source surface the effects will use</param>
        /// <param name="target">The render target surface that the post-effect result will be drawn too</param>
        void ColourEffects(IColourEffectsStage stage,
                           ITexture source,
                           IRenderTarget target);

        /// <summary>
        /// Apply simple colour modification effects to a texture source and render to a target
        /// Effects are: single colour mix, grayscale, negative, colourise and opacity
        /// A combination of all the effects can be applied together
        /// </summary>
        /// <param name="stage">The ColourEffects stage id</param>
        /// <param name="source">The Texture id of the source surface the effects will use</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>
        void ColourEffects(ulong stage,
                           ulong source,
                           ulong target);

        /// <summary>
        /// Apply bloom effect to a texture source and render to a target
        /// Brightness threshold, additive mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="stage">The Bloom stage reference</param>
        /// <param name="source">The Texture reference of the source surface the effect will use</param>
        /// <param name="target">The render target surface reference that the post-effect result will be drawn too</param>
        void Bloom(IBloomStage stage,
                   ITexture source,
                   IRenderTarget target);

        /// <summary>
        /// Apply bloom effect to a texture source and render to a target
        /// Brightness threshold, additive mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="stage">The Bloom stage id</param>
        /// <param name="source">The Texture id of the source surface the effect will use</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>
        void Bloom(ulong stage,
                   ulong source,
                   ulong target);

        /// <summary>
        /// Apply a blur effect to a texture source and render to a target
        /// The blur is applied in both x and y texture coordinate directions, it is not a directional blur - use Blur1D effect instead in that scenario
        /// Fractional mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="stage">The Blur stage reference</param>
        /// <param name="source">The Texture reference of the source surface the effect will use</param>
        /// <param name="target">The render target surface reference that the post-effect result will be drawn too</param>
        void Blur(IBlurStage stage,
                  ITexture source,
                  IRenderTarget target);

        /// <summary>
        /// Apply a blur effect to a texture source and render to a target
        /// The blur is applied in both x and y texture coordinate directions, it is not a directional blur - use Blur1D effect instead in that scenario
        /// Fractional mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="stage">The Blur stage id</param>
        /// <param name="source">The Texture id of the source surface the effect will use</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>
        void Blur(ulong stage,
                  ulong source,
                  ulong target);

        /// <summary>
        /// Apply a directional (1d) blur effect to a texture source and render to a target
        /// For non-directional blur, use the standard Blur effect instead
        /// Blur direction, fractional mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="stage">The Blur1D stage reference</param>
        /// <param name="source">The Texture reference of the source surface the effect will use</param>
        /// <param name="target">The render target surface reference that the post-effect result will be drawn too</param>      
        void Blur1D(IBlur1DStage stage,
                    ITexture source,
                    IRenderTarget target);

        /// <summary>
        /// Apply a directional (1d) blur effect to a texture source and render to a target
        /// For non-directional blur, use the standard Blur effect instead
        /// Blur direction, fractional mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="stage">The Blur1D stage id</param>
        /// <param name="source">The Texture id of the source surface the effect will use</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>      
        void Blur1D(ulong stage,
                    ulong source,
                    ulong target);

        /// <summary>
        /// Copies colour data from one texture source to a render target
        /// The surfaces do not need to be the same dimensions
        /// </summary>
        /// <param name="source">The Texture reference of the source to copy colour data from</param>
        /// <param name="target">The render target surface reference to copy too</param>     
        void Copy(ITexture source,
                  IRenderTarget target);

        /// <summary>
        /// Copies colour data from one texture source to a render target
        /// The surfaces do not need to be the same dimensions
        /// </summary>
        /// <param name="source">The Texture id of the source to copy colour data from</param>
        /// <param name="target">The render target surface id to copy too</param>     
        void Copy(ulong source,
                  ulong target);

        /// <summary>
        /// Apply stylised effects to a texture source and render to a target
        /// Effects are: Pixellate, Edge Detection, Cathode Ray Tube and Old Movie Reel, 
        /// A combination of all the effects can be applied together
        /// </summary>
        /// <param name="stage">The StyleEffects stage reference</param>
        /// <param name="source">The Texture reference of the source surface the effects will use</param>
        /// <param name="target">The render target surface reference that the post-effect result will be drawn too</param>
        void StyleEffects(IStyleEffectsStage stage,
                          ITexture source,
                          IRenderTarget target);

        /// <summary>
        /// Apply stylised effects to a texture source and render to a target
        /// Effects are: Pixellate, Edge Detection, Cathode Ray Tube and Old Movie Reel, 
        /// A combination of all the effects can be applied together
        /// </summary>
        /// <param name="stage">The StyleEffects stage id</param>
        /// <param name="source">The Texture id of the source surface the effects will use</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>
        void StyleEffects(ulong stage,
                          ulong source,
                          ulong target);

        /// <summary>
        /// Render a 3D Textured Mesh
        /// Ambient, Directional and Spotlights can be configured for the stage, along with the Mesh to render
        /// </summary>
        /// <param name="stage">The MeshRender stage reference</param>
        /// <param name="camera">The Camera (3D) reference that will transform and project the 3D mesh</param>
        /// <param name="source">The Texture reference of the source surface the mesh will be texture mapped with</param>
        /// <param name="target">The render target surface reference that the result will be drawn too</param>    
        void MeshRender(IMeshRenderStage stage,
                        ICamera3D camera,
                        ITexture source,
                        IRenderTarget target);

        /// <summary>
        /// Render a 3D Textured Mesh
        /// Ambient, Directional and Spotlights can be configured for the stage, along with the Mesh to render
        /// </summary>
        /// <param name="stage">The MeshRender stage id</param>
        /// <param name="camera">The Camera (3D) id that will transform and project the 3D mesh</param>
        /// <param name="source">The Texture id of the source surface the mesh will be texture mapped with</param>
        /// <param name="target">The render target surface id that the result will be drawn too</param>    
        void MeshRender(ulong stage,
                        ulong camera,
                        ulong source,
                        ulong target);


        /// <summary>
        /// Enables the mixing of up to four textures together
        /// The general mix factors per texture are defined in the configuration
        /// A further 'per-pixel' mixing scalar can be defined using mixTexture
        /// </summary>
        /// <param name="stage">The Mix stage reference</param>
        /// <param name="mixTexture">Optional texture reference providing 'per-pixel' mixing factors. R, G, B, A components represents textures 0, 1, 2, 3. The textures do not need to be the same dimensions, hence per-pixel is only an accurate description when they are all the same size</param>
        /// <param name="tex0">An input Texture reference</param>
        /// <param name="tex1">An input Texture reference</param>
        /// <param name="tex2">An input Texture reference</param>
        /// <param name="tex3">An input Texture reference</param>
        /// <param name="target">The render target surface reference that the post-effect result will be drawn too</param>
        void Mix(IMixStage stage,
                 ITexture mixTexture,
                 ITexture tex0,
                 ITexture tex1,
                 ITexture tex2,
                 ITexture tex3,
                 IRenderTarget target);

        /// <summary>
        /// Enables the mixing of up to four textures together
        /// The general mix factors per texture are defined in the configuration
        /// A further 'per-pixel' mixing scalar can be defined using mixTexture
        /// </summary>
        /// <param name="stage">The Mix stage id</param>
        /// <param name="mixTexture">Optional texture id providing 'per-pixel' mixing factors. R, G, B, A components represents textures 0, 1, 2, 3. The textures do not need to be the same dimensions, hence per-pixel is only an accurate description when they are all the same size</param>
        /// <param name="tex0">An input Texture id</param>
        /// <param name="tex1">An input Texture id</param>
        /// <param name="tex2">An input Texture id</param>
        /// <param name="tex3">An input Texture id</param>
        /// <param name="target">The render target surface id that the post-effect result will be drawn too</param>
        void Mix(ulong stage,
                 ulong mixTexture,
                 ulong tex0,
                 ulong tex1,
                 ulong tex2,
                 ulong tex3,
                 ulong target);

        /// <summary>
        /// Applies a user defined custom shader effect
        /// The user defines the shader code, constrained by a set maximum number of texture and uniform configuration variable inputs
        /// </summary>
        /// <param name="stage">The CustomShader stage reference</param>
        /// <param name="tex0">An input Texture reference</param>
        /// <param name="tex1">An input Texture reference</param>
        /// <param name="tex2">An input Texture reference</param>
        /// <param name="tex3">An input Texture reference</param>
        /// <param name="target">The render target surface reference that the shader result will be drawn too</param>
        void CustomShader(ICustomShaderStage stage,
                          ITexture tex0,
                          ITexture tex1,
                          ITexture tex2,
                          ITexture tex3,
                          IRenderTarget target);

        /// <summary>
        /// Applies a user defined custom shader effect
        /// The user defines the shader code, constrained by a set maximum number of texture and uniform configuration variable inputs
        /// </summary>
        /// <param name="stage">The CustomShader stage id</param>
        /// <param name="tex0">An input Texture id</param>
        /// <param name="tex1">An input Texture id</param>
        /// <param name="tex2">An input Texture id</param>
        /// <param name="tex3">An input Texture id</param>
        /// <param name="target">The render target surface id that the shader result will be drawn too</param>
        void CustomShader(ulong stage,
                          ulong tex0,
                          ulong tex1,
                          ulong tex2,
                          ulong tex3,
                          ulong target);

        /// <summary>
        /// Applies a user defined custom piece of veldrid logic, enabling operations or integrations that require complete access to veldrid library functions
        /// </summary>
        /// </summary>
        /// <param name="stage">The CustomVeldrid stage reference</param>
        /// <param name="tex0">An input Texture reference</param>
        /// <param name="tex1">An input Texture reference</param>
        /// <param name="tex2">An input Texture reference</param>
        /// <param name="tex3">An input Texture reference</param>
        /// <param name="target">The render target surface reference that the custom veldrid code may draw too, to enable use in wider yak2D rendering</param>                  
        void CustomVeldrid(ICustomVeldridStage stage,
                           ITexture tex0,
                           ITexture tex1,
                           ITexture tex2,
                           ITexture tex3,
                           IRenderTarget target);

        /// <summary>
        /// Applies a user defined custom piece of veldrid logic, enabling operations or integrations that require complete access to veldrid library functions
        /// </summary>
        /// </summary>
        /// <param name="stage">The CustomVeldrid stage id</param>
        /// <param name="tex0">An input Texture id</param>
        /// <param name="tex1">An input Texture id</param>
        /// <param name="tex2">An input Texture id</param>
        /// <param name="tex3">An input Texture id</param>
        /// <param name="target">The render target surface id that the custom veldrid code may draw too, to enable use in wider yak2D rendering</param>                  
        void CustomVeldrid(ulong stage,
                           ulong tex0,
                           ulong tex1,
                           ulong tex2,
                           ulong tex3,
                           ulong target);

        /// <summary>
        /// Transfer surface (Texture or RenderTarget) pixel data from GPU to CPU
        /// The Main window swapchain backbuffer target cannot be copied
        /// A single copy stage must only be used once per render queue (as allowing a second use would either require overwriting data or more temporary possible heap allocation)
        /// </summary>
        /// <param name="stage">Stage Reference. Holds callback delegates used to pass pixel data back to user</param>
        /// <param name="source">Surface Reference. The surface (RenderTarget or Texture) to copy pixel data from</param>
        void CopySurfaceData(ISurfaceCopyStage stage, ITexture source);

        /// <summary>
        /// Transfer surface (Texture or RenderTarget) pixel data from GPU to CPU
        /// The Main window swapchain backbuffer target cannot be copied
        /// A single copy stage must only be used once per render queue (as allowing a second use would either require overwriting data or more temporary possible heap allocation)
        /// </summary>
        /// <param name="stage">Stage id. Holds callback delegates used to pass pixel data back to user</param>
        /// <param name="source">Surface id. The surface (RenderTarget or Texture) to copy pixel data from</param>
        void CopySurfaceData(ulong stage, ulong source);

        /// <summary>
        /// Sets the portion of the render surfaces that subsequent redner operations will draw too
        /// The user must clear the viewport if subsequent rendering operations wish to draw to entire surfaces
        /// </summary>
        /// <param name="viewport">The Viewport reference to apply to subsequent rendering operations</summary>
        void SetViewport(IViewport viewport);

        /// <summary>
        /// Sets the portion of the render surfaces that subsequent redner operations will draw too
        /// The user must clear the viewport if subsequent rendering operations wish to draw to entire surfaces
        /// </summary>
        /// <param name="viewport">The Viewport id to apply to subsequent rendering operations</summary>
        void SetViewport(ulong viewport);

        /// <summary>
        /// Clears any set viewport, allowing subsequent rendering operations to draw to entire surfaces
        /// </summary>
        void RemoveViewport();
    }
}