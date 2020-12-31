using SimpleInjector;
using System.Reflection;
using Yak2D.Core;
using Yak2D.Internal;
using Yak2D.Graphics;
using Yak2D.Surface;
using Yak2D.Font;
using Yak2D.Input;

namespace Yak2D
{
    /// <summary>
    /// Welcome to the Yak2D cross platform c# graphics framework!!! To get started, please provide Run() your own implementation of the IApplication interface
    /// </summary>
    public class Launcher
    {
        /// <summary>
        /// Starts the framework with the provided IApplication object
        /// </summary>
        /// <param name="application">User implementation of IApplication. The framework controls the lifecycle of the application via calls to this object</param>
        /// <param name="frameworkMessenger">(Optional) The user may wish to provide an object that re-routes any framework messages. By default they are written to console</param>
        public static void Run(IApplication application, IFrameworkMessenger frameworkMessenger = null)
        {
            var container = new Container();

            container.RegisterInstance<IApplication>(application);

            var applicationAssembly = new ApplicationAssembly(ExtractApplicationAssembly(application));
            container.RegisterInstance<IApplicationAssembly>(applicationAssembly);

            if (frameworkMessenger == null)
            {
                container.Register<IFrameworkMessenger, ConsoleMessenger>(Lifestyle.Singleton);
            }
            else
            {
                container.RegisterInstance<IFrameworkMessenger>(frameworkMessenger);
            }

            container.Register<IGraphicsAssembly, GraphicsAssembly>(Lifestyle.Singleton);
            container.Register<ISurfaceAssembly, SurfaceAssembly>(Lifestyle.Singleton);
            container.Register<IFontsAssembly, FontsAssembly>(Lifestyle.Singleton);
            container.Register<IFileSystem, FileSystem>(Lifestyle.Singleton);
            container.Register<IDebugAnalytics, DebugAnalytics>(Lifestyle.Singleton);
            container.Register<IShutdownManager, ShutdownManager>(Lifestyle.Singleton);
            container.Register<IGraphicsShutdownManager, GraphicsShutdownManager>(Lifestyle.Singleton);
            container.Register<IResourceReinitialiser, ResourcesReinitialiser>(Lifestyle.Singleton);
            container.Register<IGraphicsResourceReinitialiser, GraphicsResourceReinitialiser>(Lifestyle.Singleton);
            container.Register<IFrameworkDebugOverlay, FrameworkDebugOverlay>(Lifestyle.Singleton);
            container.Register<IApplicationMessenger, ApplicationMessenger>(Lifestyle.Singleton);
            container.Register<ICoreMessenger, CoreMessenger>(Lifestyle.Singleton);
            container.Register<IStartupPropertiesCache, StartupPropertiesCache>(Lifestyle.Singleton);
            container.Register<ISystemComponents, SystemComponents>(Lifestyle.Singleton);
            container.Register<IVeldridWindowUpdater, VeldridWindowUpdater>(Lifestyle.Singleton);
            container.Register<ITimerFactory, StopWatchTimerFactory>(Lifestyle.Singleton);
            container.Register<IUpdatePeriodFactory, UpdatePeriodFactory>(Lifestyle.Singleton);
            container.Register<IFramesPerSecondMonitor, FramesPerSecondMonitor>(Lifestyle.Singleton);
            container.Register<IIdGenerator, IdGenerator>(Lifestyle.Singleton);
            container.Register<IGraphics, Graphics.Graphics>(Lifestyle.Singleton);
            container.Register<ISimpleCollectionFactory, SimpleDictionaryCollectionFactory>(Lifestyle.Singleton);
            container.Register<IComparerCollection, ComparerCollection>(Lifestyle.Singleton);
            container.Register<ICameraFactory, CameraFactory>(Lifestyle.Singleton);
            container.Register<ICameraManager, CameraManager>(Lifestyle.Singleton);
            container.Register<IQueueToBufferBlitter, QueueToBufferBlitter>(Lifestyle.Singleton);
            container.Register<IDrawStageBatcherTools, DrawStageBatcherTools>(Lifestyle.Singleton);
            container.Register<IDrawStageBatcherFactory, DrawStageBatcherFactory>(Lifestyle.Singleton);
            container.Register<IDrawStageBuffersFactory, DrawStageBuffersFactory>(Lifestyle.Singleton);
            container.Register<IDrawQueueGroupFactory, DrawQueueGroupFactory>(Lifestyle.Singleton);
            container.Register<IDrawQueueFactory, DrawQueueFactory>(Lifestyle.Singleton);
            container.Register<IRenderCommandQueue, RenderCommandQueue>(Lifestyle.Singleton);
            container.Register<ICommandProcessor, CommandProcessor>(Lifestyle.Singleton);
            container.Register<IRenderStageModelFactory, RenderStageModelFactory>(Lifestyle.Singleton);
            container.Register<IRenderStageManager, RenderStageManager>(Lifestyle.Singleton);
            container.Register<IRenderStageVisitor, RenderStageVisitor>(Lifestyle.Singleton);
            container.Register<IGpuSurfaceCollection, GpuSurfaceCollection>(Lifestyle.Singleton);
            container.Register<IGpuSurfaceManager, GpuSurfaceManager>(Lifestyle.Singleton);
            container.Register<IFontLoader, FontLoader>(Lifestyle.Singleton);
            container.Register<ISubFontTools, SubFontTools>(Lifestyle.Singleton);
            container.Register<ISubFontGenerator, SubFontGenerator>(Lifestyle.Singleton);
            container.Register<IFontCollection, FontCollection>(Lifestyle.Singleton);
            container.Register<IFontManager, FontManager>(Lifestyle.Singleton);
            container.Register<IPipelineFactory, PipelineFactory>(Lifestyle.Singleton);
            container.Register<IBlendStateConverter, BlendStateConverter>(Lifestyle.Singleton);
            container.Register<IShaderLoaderFunctions, ShaderLoaderFunctions>(Lifestyle.Singleton);
            container.Register<IShaderLoader, ShaderLoader>(Lifestyle.Singleton);
            container.Register<IFullNdcSpaceQuadVertexBuffer, FullNdcSpaceQuadVertices>(Lifestyle.Singleton);
            container.Register<IViewportManager, ViewportManager>(Lifestyle.Singleton);
            container.Register<IViewportFactory, ViewportFactory>(Lifestyle.Singleton);
            container.Register<IDrawStageRenderer, DrawStageRenderer>(Lifestyle.Singleton);
            container.Register<IColourEffectsStageRenderer, ColourEffectsStageRenderer>(Lifestyle.Singleton);
            container.Register<IBloomStageRenderer, BloomStageRenderer>(Lifestyle.Singleton);
            container.Register<IBloomSamplingRenderer, BloomSamplingRenderer>(Lifestyle.Singleton);
            container.Register<IBloomResultMixingRenderer, BloomResultMixingRenderer>(Lifestyle.Singleton);
            container.Register<IBlurStageRenderer, BlurStageRenderer>(Lifestyle.Singleton);
            container.Register<IBlur1DStageRenderer, Blur1DStageRenderer>(Lifestyle.Singleton);
            container.Register<IBlurResultMixingRenderer, BlurResultMixingRenderer>(Lifestyle.Singleton);
            container.Register<IDownSamplerWeightsAndOffsets, DownSamplerWeightsAndOffsets>(Lifestyle.Singleton);
            container.Register<IDownSamplingRenderer, DownSamplingRenderer>(Lifestyle.Singleton);
            container.Register<ISinglePassGaussianBlurRenderer, SinglePassGaussianBlurRenderer>(Lifestyle.Singleton);
            container.Register<IGaussianBlurWeightsAndOffsetsCache, GaussianBlurWeightsAndOffsetsCache>(Lifestyle.Singleton);
            container.Register<ICopyStageRenderer, CopyStageRenderer>(Lifestyle.Singleton);
            container.Register<IStyleEffectsStageRenderer, StyleEffectsStageRenderer>(Lifestyle.Singleton);
            container.Register<IMeshRenderStageRenderer, MeshRenderStageRenderer>(Lifestyle.Singleton);
            container.Register<IDistortionStageRenderer, DistortionStageRenderer>(Lifestyle.Singleton);
            container.Register<IDistortionHeightRenderer, DistortionHeightRenderer>(Lifestyle.Singleton);
            container.Register<IDistortionGraidentShiftRenderer, DistortionGradientShiftRenderer>(Lifestyle.Singleton);
            container.Register<IDistortionRenderer, DistortionRenderer>(Lifestyle.Singleton);
            container.Register<IMixStageRenderer, MixStageRenderer>(Lifestyle.Singleton);
            container.Register<ICustomShaderStageRenderer, CustomShaderStageRenderer>(Lifestyle.Singleton);
            container.Register<ICustomVeldridStageRenderer, CustomVeldridStageRenderer>(Lifestyle.Singleton);
            container.Register<ISurfaceCopyStageRenderer, SurfaceCopyStageRenderer>(Lifestyle.Singleton);
            container.Register<IImageSharpLoader, ImageSharpLoader>(Lifestyle.Singleton);
            container.Register<IFloatTextureBuilder, FloatTextureBuilder>(Lifestyle.Singleton);
            container.Register<IGpuSurfaceFactory, GpuSurfaceFactory>(Lifestyle.Singleton);
            container.Register<ICommonMeshBuilder, CommonMeshBuilder>(Lifestyle.Singleton);
            container.Register<IVertexLinearGridToTriangleListTool, VertexLinearGridToTriangleListTool>(Lifestyle.Singleton);
            container.Register<ICrtMeshBuilderFunctions, CrtMeshBuilderFunctions>(Lifestyle.Singleton);
            container.Register<ICrtMeshBuilder, CrtMeshBuilder>(Lifestyle.Singleton);
            container.Register<ISphericalMeshBuilder, SphericalMeshBuilder>(Lifestyle.Singleton);
            container.Register<IQuadMeshBuilder, QuadMeshBuilder>(Lifestyle.Singleton);
            container.Register<IRectangularCuboidMeshBuilder, RectangularCuboidMeshBuilder>(Lifestyle.Singleton);
            container.Register<ICommonOperations, CommonOperations>(Lifestyle.Singleton);
            container.Register<IDistortionHelper, DistortionHelper>(Lifestyle.Singleton);
            container.Register<IDistortionTextureGenerator, DistortionTextureGenerator>(Lifestyle.Singleton);
            container.Register<ISdl2EventProcessor, Sdl2EventProcessor>(Lifestyle.Singleton);
            container.Register<IInputMouseKeyboard, InputMouseKeyboard>(Lifestyle.Singleton);
            container.Register<IInputGameController, InputGameController>(Lifestyle.Singleton);
            container.Register<ICoordinateTransforms, CoordinateTransforms>(Lifestyle.Singleton);
            container.Register<IServices, Services>(Lifestyle.Singleton);
            container.Register<IBackend, Backend>(Lifestyle.Singleton);
            container.Register<IDrawing, Drawing>(Lifestyle.Singleton);
            container.Register<IDisplay, Display>(Lifestyle.Singleton);
            container.Register<IFps, Fps>(Lifestyle.Singleton);
            container.Register<IStages, Stages>(Lifestyle.Singleton);
            container.Register<ISurfaces, Surfaces>(Lifestyle.Singleton);
            container.Register<ICameras, Cameras>(Lifestyle.Singleton);
            container.Register<IRenderQueue, RenderQueue>(Lifestyle.Singleton);
            container.Register<IInput, Input.Input>(Lifestyle.Singleton);
            container.Register<IFonts, Fonts>(Lifestyle.Singleton);
            container.Register<IHelpers, Helpers>(Lifestyle.Singleton);
            container.Register<ICore, Core.Core>(Lifestyle.Singleton);

            container.Verify();

            var framework = container.GetInstance<ICore>();

            framework.Run();
        }

        private static Assembly ExtractApplicationAssembly(IApplication application)
        {
            return application.GetType().Assembly;
        }
    }
}