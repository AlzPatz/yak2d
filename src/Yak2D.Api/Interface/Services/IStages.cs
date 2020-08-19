using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Render Stage Operations
    /// Creation, configuration and destruction of render stages and viewports
    /// </summary>
    public interface IStages
    {
        /// <summary>
        /// Returns the number of user created render stages currently managed by the framework
        /// </summary>
        int CountRenderStages { get; }

        /// <summary>
        /// Returns the number of user created viewports currently managed by the framework
        /// </summary>
        int CountViewports { get; }

        /// <summary>
        /// A viewport defines an area of a render surface used as a drawing area
        /// The topleft corner of a render surface is defined as the origin (0,0)
        /// </summary>
        /// <param name="minx">Left edge of viewport in pixels</param>
        /// <param name="miny">Top edge of viewport in pixels</param>
        /// <param name="width">Width of viewport in pixels</param>
        /// <param name="height">Height of viewport in pixels</param>
        IViewport CreateViewport(uint minx,
                                 uint miny,
                                 uint width,
                                 uint height);
        /// <summary>
        /// Destroy viewport object data
        /// </summary>
        /// <param name"viewport">The viewport reference to destroy</param>                                 
        void DestroyViewport(IViewport viewport);

        /// <summary>
        /// Destroy viewport object data
        /// </summary>
        /// <param name"viewport">The viewport id to destroy</param>                                 
        void DestroyViewport(ulong viewport);

        /// <summary>
        /// Destroy all user created viewport object data
        /// </summary>
        void DestroyAllViewports();

        /// <summary>
        /// Draw Queues are used to collect, sort and process 2d drawing requests
        /// </summary>
        /// <param name="clearDynamicRequestQueueEachFrame">Set to automatically remove (clear) draw requests from the stage's queue following each draw iteration. Stops the user being required to remember to clear the queues at the start of each draw update. A useful default as this can regularly catch users (me...) out, resulting in ever increasing draw queue sizes :)</param>
        /// <param name="blendState">How source pixels are mixed with destination pixels on surfaces</param>
        IDrawStage CreateDrawStage(bool clearDynamicRequestQueueEachFrame = true,
                                   BlendState blendState = BlendState.Alpha);

        /// <summary>
        /// Provides simple colour modification effects to a texture source and render to a target
        /// Effects are: single colour mix, grayscale, negative, colourise and opacity
        /// A combination of all the effects can be applied together
        /// </summary>
        IColourEffectsStage CreateColourEffectsStage();

        /// <summary>
	    /// A RenderStage that can apply a bloom effect to a texture source and render to a target
        /// Brightness threshold, additive mix amount with source and number of blur component samples can be configured for the stage
        /// The source texture is downsampled and threhold clipped onto a smaller surface before being blurred and re-mixed with the original
        /// </summary>
        /// <param name="sampleSurfaceWidth">The width of the downsampling texture used. Smaller == less computationally intensive, lower quality bloom, but larger blur distance per sample</param>
        /// <param name="sampleSurfaceHeight">The height of the downsampling texture used. Smaller == less computationally intensive, lower quality bloom, but larger blur distance per sample</param>
        IBloomStage CreateBloomStage(uint sampleSurfaceWidth,
                                     uint sampleSurfaceHeight);

        /// <summary>
        /// A RenderStage that applies a blur effect to a texture source and render to a target
        /// The blur is applied in both x and y texture coordinate directions, it is not a directional blur - use Blur1D effect instead in that scenario
        /// Fractional mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="sampleSurfaceWidth">The width of the downsampling texture used. Smaller == less computationally intensive, lower quality bloom, but larger blur distance per sample</param>
        /// <param name="sampleSurfaceHeight">The height of the downsampling texture used. Smaller == less computationally intensive, lower quality bloom, but larger blur distance per sample</param>                             
        IBlurStage CreateBlurStage(uint sampleSurfaceWidth,
                                   uint sampleSurfaceHeight);

        /// <summary>
        /// A RenderStage that applies a directional (1d) blur effect to a texture source and render to a target
        /// For non-directional blur, use the standard Blur effect instead
        /// Blur direction, fractional mix amount with source and number of blur component samples can be configured for the stage
        /// </summary>
        /// <param name="sampleSurfaceWidth">The width of the downsampling texture used. Smaller == less computationally intensive, lower quality bloom, but larger blur distance per sample</param>
        /// <param name="sampleSurfaceHeight">The height of the downsampling texture used. Smaller == less computationally intensive, lower quality bloom, but larger blur distance per sample</param>                             
        IBlur1DStage CreateBlur1DStage(uint sampleSurfaceWidth,
                                       uint sampleSurfaceHeight);

        /// <summary>
        /// A RenderStage that applies stylised effects to a texture source and render to a target
        /// Effects are: Pixellate, Edge Detection, Static Noise, Cathode Ray Tube and Old Movie Reel, 
        /// A combination of all the effects can be applied together
        /// </summary>                                       
        IStyleEffectsStage CreateStyleEffectsStage();

        /// <summary>
        /// A RenderStage that will render a 3D Textured Mesh
        /// Ambient, Directional and Spotlights can be configured for the stage, along with the Mesh to render
        /// </summary>
        IMeshRenderStage CreateMeshRenderStage();

        /// <summary>
        /// A RenderStage that enables the mixing of up to four textures together
        /// The general mix factors per texture are defined in the configuration
        /// A further 'per-pixel' mixing scalar can be defined using mixTexture
        /// </summary>
        IMixStage CreateMixStage();

        /// <summary>
        /// A RenderStage that applies a distortion effect to a texture source and render to a target
        /// The per-pixel distortion amount is generated from a distortion height map
        /// The distortion height map is drawn each frame, in a similar way that DrawStage draw requests are generated, queued and sorted
        /// The final distortion amount per-pixel is scaled by a configurable scalar quantity
        /// </summary>
        /// <param name="clearDynamicRequestQueueEachFrame">Set to automatically remove (clear) draw requests from the stage's queue following each draw iteration. Stops the user being required to remember to clear the queues at the start of each draw update. A useful default as this can regularly catch users (me...) out, resulting in ever increasing draw queue sizes :)</param>
        /// <param name="internalSurfaceWidth">The width of the internal textures used for generating the height and gradient maps. Smaller == less computationally intensive, lower quality distortion</param>
        /// <param name="internalSurfaceHeight">The height of the internal textures used for generating the height and gradient maps. Smaller == less computationally intensive, lower quality distortion</param>                             
        IDistortionStage CreateDistortionStage(uint internalSurfaceWidth,
                                               uint internalSurfaceHeight,
                                               bool clearDynamicRequestQueueEachFrame);

        /// <summary>
        /// Create a stage for a user defined custom piece of veldrid logic, enabling operations or integrations that require complete access to veldrid library functions
        /// </summary>
        /// <param name="customVeldridBase">User's custom render stage code must inherrit from this base class</param>
        ICustomVeldridStage CreateCustomVeldridStage(CustomVeldridBase customVeldridBase);

        /// <summary>
        /// Creates a stage for a user defined shader effect 
        /// The user defines the shader code, constrained by a set maximum number of texture and uniform configuration variable inputs
        /// </summary>
        /// <param name="fragmentShaderPathName">Shader code filename (no extension) including path"</param>
        /// <param name="assetType">Embedded in the binary or a file location"</param>
        /// <param name="uniformDescriptions">Description of shader uniforms (textures or data structs)</param>
        /// <param name="blendState">How source pixels are mixed with destination pixels on surfaces</param>
        ICustomShaderStage CreateCustomShaderStage(string fragmentShaderPathName,
                                                   AssetSourceEnum assetType,
                                                   ShaderUniformDescription[] uniformDescriptions,
                                                   BlendState blendState);

        /// <summary>
        /// Destroy a render stage
        /// </summary>
        /// <param name="stage">The stage reference to destroy</param>
        void DestroyStage(IRenderStage stage);

        /// <summary>
        /// Destroy a render stage
        /// </summary>
        /// <param name="stage">The stage id to destroy</param>
        void DestroyStage(ulong stage);

        /// <summary>
        /// Destroy all render stages
        /// </summary>
        void DestroyAllStages();

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetColourEffectsConfig(IColourEffectsStage stage,
                                    ColourEffectConfiguration config,
                                    float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetColourEffectsConfig(ulong stage,
                                    ColourEffectConfiguration config,
                                    float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetBloomConfig(IBloomStage stage,
                            BloomEffectConfiguration config,
                            float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetBloomConfig(ulong stage,
                            BloomEffectConfiguration config,
                            float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetBlurConfig(IBlurStage stage,
                           BlurEffectConfiguration config,
                           float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetBlurConfig(ulong stage,
                           BlurEffectConfiguration config,
                           float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetBlur1DConfig(IBlur1DStage stage,
                             Blur1DEffectConfiguration config,
                             float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetBlur1DConfig(ulong stage,
                             Blur1DEffectConfiguration config,
                             float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - All effects at once
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetStyleEffectsGroupConfig(IStyleEffectsStage stage,
                                        StyleEffectGroupConfiguration config,
                                        float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - All effects at once
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetStyleEffectsGroupConfig(ulong stage,
                                        StyleEffectGroupConfiguration config,
                                        float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Pixellate
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsPixellateConfig(IStyleEffectsStage stage,
                                            PixellateConfiguration config,
                                            float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Pixellate
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsPixellateConfig(ulong stage,
                                            PixellateConfiguration config,
                                            float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Edge Detection
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsEdgeDetectionConfig(IStyleEffectsStage stage,
                                                EdgeDetectionConfiguration config,
                                                float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Edge Detection
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsEdgeDetectionConfig(ulong stage,
                                                EdgeDetectionConfiguration config,
                                                float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Static
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsStaticConfig(IStyleEffectsStage stage,
                                         StaticConfiguration config,
                                         float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Static
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsStaticConfig(ulong stage,
                                         StaticConfiguration config,
                                         float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Old Movie Reel
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsOldMovieConfig(IStyleEffectsStage stage,
                                           OldMovieConfiguration config,
                                           float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Old Movie Reel
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsOldMovieConfig(ulong stage,
                                           OldMovieConfiguration config,
                                           float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Cathode Ray Tube
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsCrtConfig(IStyleEffectsStage stage,
                                      CrtEffectConfiguration config,
                                      float transitionSeconds = 0.0f);

        /// <summary>
        /// Update StyleEffects Configuration - Cathode Ray Tube
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetStyleEffectsCrtConfig(ulong stage,
                                      CrtEffectConfiguration config,
                                      float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Mesh Render Overall Light Properties - Number of lights, specular colour, shininess
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetMeshRenderLightingProperties(IMeshRenderStage stage,
                                             MeshRenderLightingPropertiesConfiguration configuration,
                                             float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Mesh Render Overall Light Properties - Number of lights, specular colour, shininess
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>        
        void SetMeshRenderLightingProperties(ulong stage,
                                             MeshRenderLightingPropertiesConfiguration configuration,
                                             float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Mesh Render Per Light Properties
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="lightConfigurations">Array of target light configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>    
        void SetMeshRenderLights(IMeshRenderStage stage,
                                 MeshRenderLightConfiguration[] lightConfigurations,
                                 float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Mesh Render Per Light Properties
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="lightConfigurations">Array of target light configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>    
        void SetMeshRenderLights(ulong stage,
                                 MeshRenderLightConfiguration[] lightConfigurations,
                                 float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Mesh Render - The Mesh
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="mesh">Array of target light configuration</param>
        void SetMeshRenderMesh(IMeshRenderStage stage,
                               Vertex3D[] mesh);

        /// <summary>
        /// Update Mesh Render - The Mesh
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="mesh">Array of target light configuration</param>
        void SetMeshRenderMesh(ulong stage,
                               Vertex3D[] mesh);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetDistortionConfig(IDistortionStage stage,
                                 DistortionEffectConfiguration config,
                                 float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="config">The target stage configuration</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        void SetDistortionConfig(ulong stage,
                                 DistortionEffectConfiguration config,
                                 float transitionSeconds = 0.0f);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="amounts">The target 4-component scalar mix amounts</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        /// <param name="normalise">Clamp mix values to 1.0</param>
        void SetMixStageProperties(IMixStage stage,
                                   Vector4 amounts,
                                   float transitionSeconds = 0.0f,
                                   bool normalise = false);

        /// <summary>
        /// Update Render Stage Configuration
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="amounts">The target 4-component scalar mix amounts</param>
        /// <param name="transitionSeconds">Time in seconds over which to interpolate the current configuration to the target configuration</param>
        /// <param name="normalise">Clamp mix values to 1.0</param>
        void SetMixStageProperties(ulong stage,
                                   Vector4 amounts,
                                   float transitionSeconds = 0.0f,
                                   bool normalise = false);

        /// <summary>
        /// Update user defined custom shader uniforms - Single value
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="uniformName">The string name reference of the uniform</param>
        /// <param name="data">The struct containing the uniform data</param>
        void SetCustomShaderUniformValues<T>(ICustomShaderStage stage,
                                             string uniformName,
                                             T data) where T : struct;

        /// <summary>
        /// Update user defined custom shader uniforms - Single value
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="uniformName">The string name reference of the uniform</param>
        /// <param name="data">The struct containing the uniform data</param>
        void SetCustomShaderUniformValues<T>(ulong stage,
                                             string uniformName,
                                             T data) where T : struct;

        /// <summary>
        /// Update user defined custom shader uniforms - Array of values
        /// </summary>
        /// <param name="stage">The stage reference</param>
        /// <param name="uniformName">The string name reference of the uniform</param>
        /// <param name="dataArray">The array of structs containing the uniform data</param>
        void SetCustomShaderUniformValues<T>(ICustomShaderStage stage,
                                             string uniformName,
                                             T[] dataArray) where T : struct;

        /// <summary>
        /// Update user defined custom shader uniforms - Array of values
        /// </summary>
        /// <param name="stage">The stage id</param>
        /// <param name="uniformName">The string name reference of the uniform</param>
        /// <param name="dataArray">The array of structs containing the uniform data</param>
        void SetCustomShaderUniformValues<T>(ulong stage,
                                             string uniformName,
                                             T[] dataArray) where T : struct;
    }
}