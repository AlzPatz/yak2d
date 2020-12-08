using System;
using Veldrid;
using Yak2D.Internal;
using Yak2D.Utility;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace Yak2D.Core
{
    public class SystemComponents : ISystemComponents
    {
        public GraphicsApi GraphicsApi { get => Device.BackendType; }
        public bool CurrentlyReinitialisingDevices { get; private set; }

        public IWindow Window { get; private set; }
        public IDevice Device { get; private set; }
        public IFactory Factory { get; private set; }

        public TexturePixelFormat SwapChainFramebufferPixelFormat { get; private set; }
        public Internal.TextureSampleCount SwapChainFramebufferSampleCount { get; private set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IApplicationMessenger _applicationMessenger;

        private readonly StartupConfig _userStartupProperties;

        private bool _vsync;

        public SystemComponents(IStartupPropertiesCache defaultPropertiesCache,
                                IFrameworkMessenger frameworkMessenger,
                                IApplicationMessenger applicationMessenger)
        {
            _frameworkMessenger = frameworkMessenger;
            _applicationMessenger = applicationMessenger;
            _frameworkMessenger.Report("Veldrid Components Initialising...");

            _userStartupProperties = defaultPropertiesCache.User;

            _vsync = _userStartupProperties.SyncToVerticalBlank;

            if (!AttemptToCreateGraphicsDevice(_userStartupProperties.PreferredGraphicsApi, _userStartupProperties.AvoidVulkanWherePossible))
            {
                throw new Yak2DException("Error -> Unable to create suitable graphics device for system. None of the framework backeneds are supported on this platform");
            }
        }

        private bool AttemptToCreateGraphicsDevice(GraphicsApi preferredGraphicsApi, bool avoidVulkanWherePossible)
        {
            //If Vulkan is specifically requested, it is not avoided even if the flag is set
            if (preferredGraphicsApi != GraphicsApi.SystemDefault)
            {
                var preferred = GraphicsApiConverter.ConvertApiToVeldridGraphicsBackend(preferredGraphicsApi);

                _frameworkMessenger.Report("Graphics API Requested: " + preferred.ToString());

                if (GraphicsApiConverter.AllCurrentlySupportedFrameworkBackends.Contains(preferred) && GraphicsDevice.IsBackendSupported(preferred))
                {
                    CreateGraphicsDevice(preferred);
                    return true;
                }
            }
            else
            {
                _frameworkMessenger.Report("Graphics API Requested: SystemDefault");
            }

            var defaultPlatformApi = VeldridStartup.GetPlatformDefaultBackend();

            if (avoidVulkanWherePossible && defaultPlatformApi == GraphicsBackend.Vulkan)
            {
                _frameworkMessenger.Report("System Default is Vulkan, but user has requested avoid Vulkan where possible...trying...");
            }
            else
            {
                if (GraphicsApiConverter.AllCurrentlySupportedFrameworkBackends.Contains(defaultPlatformApi) && GraphicsDevice.IsBackendSupported(defaultPlatformApi))
                {
                    CreateGraphicsDevice(defaultPlatformApi);
                    return true;
                }
            }

            var foundASuitableApi = false;

            var vulkanIsSuitableApi = false;

            GraphicsApiConverter.AllCurrentlySupportedFrameworkBackends.ForEach(api =>
            {
                var skip = false;
                if (api == GraphicsBackend.Vulkan)
                {
                    vulkanIsSuitableApi = true;
                    if (avoidVulkanWherePossible)
                    {
                        skip = true;
                    }
                }

                if (!skip && !foundASuitableApi && GraphicsDevice.IsBackendSupported(api))
                {
                    CreateGraphicsDevice(api);
                    foundASuitableApi = true;
                }
            });

            if (!foundASuitableApi && avoidVulkanWherePossible && vulkanIsSuitableApi)
            {
                if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
                {
                    _frameworkMessenger.Report("...unable to skip Vulkan as requested");
                    CreateGraphicsDevice(GraphicsBackend.Vulkan);
                    foundASuitableApi = true;
                }
            }

            return foundASuitableApi;
        }

        private void CreateGraphicsDevice(GraphicsBackend backend)
        {
            var windowInfo = new WindowCreateInfo();

            if (Window != null)
            {
                windowInfo.WindowTitle = Window.RawWindow.Title;
                windowInfo.X = Window.RawWindow.X;
                windowInfo.Y = Window.RawWindow.Bounds.Top; //RawWindow.Window.Y
                windowInfo.WindowWidth = Window.RawWindow.Width;
                windowInfo.WindowHeight = Window.RawWindow.Height;
                windowInfo.WindowInitialState = Window.RawWindow.WindowState;

                Window.Close();
            }
            else
            {
                windowInfo.WindowTitle = _userStartupProperties.WindowTitle;
                windowInfo.X = _userStartupProperties.WindowPositionX;
                windowInfo.Y = _userStartupProperties.WindowPositionY;
                windowInfo.WindowWidth = _userStartupProperties.WindowWidth;
                windowInfo.WindowHeight = _userStartupProperties.WindowHeight;
                windowInfo.WindowInitialState = WindowStateConverter.ConvertDisplayStateToVeldridWindowState(_userStartupProperties.WindowState);
            }

            Window = new SdlWindow(VeldridStartup.CreateWindow(ref windowInfo));

            Window.Resizable = _userStartupProperties.WindowIsResizable;

            var options = new GraphicsDeviceOptions(debug: false,
                                                    swapchainDepthFormat: PixelFormat.R16_UNorm,
                                                    syncToVerticalBlank: _vsync,
                                                    resourceBindingModel: ResourceBindingModel.Improved);

            Device = new VeldridDevice(VeldridStartup.CreateGraphicsDevice(Window.RawWindow, options, backend));

            Factory = new VeldridFactory(new DisposeCollectorResourceFactory(Device.RawVeldridDevice.ResourceFactory));

            RecordSwapChainBackBufferFormats();

            _frameworkMessenger.Report("Graphics API Chosen: " + Device.BackendType.ToString());
        }

        private void RecordSwapChainBackBufferFormats()
        {
            SwapChainFramebufferPixelFormat = TexturePixelFormatConverter.ConvertVeldridToYak(Device.SwapchainFramebufferOutputDescription.ColorAttachments[0].Format);
            SwapChainFramebufferSampleCount = TextureSampleCountConverter.ConvertVeldridToYak(Device.SwapchainFramebufferOutputDescription.SampleCount);
        }

        public bool IsGraphicsApiSupported(GraphicsApi api)
        {
            var veldridApi = GraphicsApiConverter.ConvertApiToVeldridGraphicsBackend(api);
            return GraphicsApiConverter.AllCurrentlySupportedFrameworkBackends.Contains(veldridApi) &&
                    GraphicsDevice.IsBackendSupported(veldridApi);
        }

        public void SetGraphicsApi(GraphicsApi api, Action systemPreAppReinitialisation)
        {
            if (api == GraphicsApi.SystemDefault)
            {
                api = GraphicsApiConverter.ConvertVeldridGraphicsBackendToApi(VeldridStartup.GetPlatformDefaultBackend());
            }

            if (api == GraphicsApi)
            {
                _frameworkMessenger.Report("Requested Graphics API is already in use. Not changing");
                return;
            }

            if (IsGraphicsApiSupported(api))
            {
                RecreateDeviceAndReinitialiseAllResources(systemPreAppReinitialisation, GraphicsApiConverter.ConvertApiToVeldridGraphicsBackend(api));
            }
            else
            {
                _frameworkMessenger.Report("Requested Graphics API is not supported: " + api);
            }
        }

        public void RecreateDeviceAndReinitialiseAllResources(Action systemPreAppReinitialisation)
        {
            RecreateDeviceAndReinitialiseAllResources(systemPreAppReinitialisation, GraphicsApiConverter.ConvertApiToVeldridGraphicsBackend(GraphicsApi));
        }

        private void RecreateDeviceAndReinitialiseAllResources(Action systemPreAppReinitialisation, GraphicsBackend graphicsBackend)
        {
            CurrentlyReinitialisingDevices = true;

            ReleaseResources();

            _vsync = Device.RawVeldridDevice.SyncToVerticalBlank;

            CreateGraphicsDevice(graphicsBackend);
            systemPreAppReinitialisation();

            CurrentlyReinitialisingDevices = false;

            _applicationMessenger.QueueMessage(FrameworkMessage.GraphicsDeviceRecreated);
        }

        public void ReleaseResources()
        {
            _frameworkMessenger.Report("Releasing all Veldrid Components...");

            Device.WaitForIdle();
            Factory.DisposeAll();
            Device.Dispose();
        }
    }
}