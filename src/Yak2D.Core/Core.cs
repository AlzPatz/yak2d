using Yak2D.Internal;

namespace Yak2D.Core
{
    public class Core : ICore
    {
        private readonly IApplication _application;
        private readonly IApplicationMessenger _applicationMessenger;
        private readonly ICoreMessenger _coreMessenger;
        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ITimerFactory _timerFactory;
        private readonly IUpdatePeriodFactory _updatePeriodFactory;
        private readonly IFramesPerSecondMonitor _framesPerSecondMonitor;
        private readonly IVeldridWindowUpdater _windowUpdater;
        private readonly ISdl2EventProcessor _sdl2EventProcessor;
        private readonly ISystemComponents _systemComponents;
        private readonly IInputMouseKeyboard _inputMouseKeyboard;
        private readonly IInputGameController _inputGameController;
        private readonly IGraphics _graphics;
        private readonly IDrawing _drawing;
        private readonly IRenderQueue _renderQueue;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;
        private readonly IServices _services;
        private readonly IShutdownManager _shutdownManager;
        private readonly IStartupPropertiesCache _startUpPropertiesCache;

        private ITimer _timer;
        private IUpdatePeriod _updater;
        private LoopProperties _loopProperties;

        public Core(
                    IApplication application,
                    IApplicationMessenger applicationMessenger,
                    ICoreMessenger coreMessenger,
                    IFrameworkMessenger frameworkMessenger,
                    ITimerFactory timerFactory,
                    IUpdatePeriodFactory updatePeriodFactory,
                    IFramesPerSecondMonitor framesPerSecondMonitor,
                    ISystemComponents systemComponents,
                    IVeldridWindowUpdater windowUpdater,
                    IStartupPropertiesCache propertiesCache,
                    ISdl2EventProcessor sdl2EventProcessor,
                    IInputMouseKeyboard inputMouseKeyboard,
                    IInputGameController inputGameController,
                    IGraphics graphics,
                    IDrawing drawing,
                    IRenderQueue renderQueue,
                    IServices services,
                    IShutdownManager shutdownManager
                    )
        {
            _application = application;
            _applicationMessenger = applicationMessenger;
            _coreMessenger = coreMessenger;
            _frameworkMessenger = frameworkMessenger;
            _timerFactory = timerFactory;
            _updatePeriodFactory = updatePeriodFactory;
            _framesPerSecondMonitor = framesPerSecondMonitor;
            _windowUpdater = windowUpdater;
            _sdl2EventProcessor = sdl2EventProcessor;
            _systemComponents = systemComponents;
            _inputMouseKeyboard = inputMouseKeyboard;
            _inputGameController = inputGameController;
            _graphics = graphics;
            _drawing = drawing;
            _renderQueue = renderQueue;
            _services = services;
            _shutdownManager = shutdownManager;
            _startUpPropertiesCache = propertiesCache;
        }

        public void Run()
        {
            if (Initialise())
            {
                Loop();
            }

            Shutdown();
        }

        private bool Initialise()
        {
            _application.OnStartup();

            _frameworkMessenger.Report("Yak2D Framework Initialising...");

            InitialiseLoopProperties();

            _systemComponents.Window.Closed += () => { _loopProperties.Running = false; };

            _frameworkMessenger.Report("Application Initialising...");

            return InitialiseApplication();
        }

        private void InitialiseLoopProperties()
        {
            _timer = _timerFactory.Create();
            _timer.Reset();

            //For FIXED_ADAPTIVE update time steps, we do not start at the smallest possible timestep. Here is hardcorded 120FPS
            var timeStep = _startUpPropertiesCache.User.UpdatePeriodType == UpdatePeriod.Fixed_Adaptive&&
                            _startUpPropertiesCache.User.FixedOrSmallestUpdateTimeStepInSeconds < 1.0f / 120.0f ?
                            1.0f / 120.0f : _startUpPropertiesCache.User.FixedOrSmallestUpdateTimeStepInSeconds;

            _loopProperties = new LoopProperties
            {
                UpdatePeriodType = _startUpPropertiesCache.User.UpdatePeriodType,
                ProcessFractionalUpdatesBeforeDraw = _startUpPropertiesCache.User.ProcessFractionalUpdatesBeforeDraw,
                SmallestFixedUpdateTimeStepInSeconds = _startUpPropertiesCache.User.FixedOrSmallestUpdateTimeStepInSeconds,
                FixedUpdateTimeStepInSeconds = timeStep,
                DoNotDrawFrameUnlessThereHasBeenOneUpdate = _startUpPropertiesCache.User.RequireAtleastOneUpdatePerDraw,
                Running = false,
                TimeOfLastUpdate = 0.0,
                TimeOfLastDraw = 0.0,
                HasThereBeenAnUpdateSinceTheLastDraw = false
            };

            _updater = _updatePeriodFactory.Create(_loopProperties.UpdatePeriodType);
        }

        private bool InitialiseApplication()
        {
            return _application.CreateResources(_services);
        }

        private void Loop()
        {
            _frameworkMessenger.Report("Yak2D Framework entering core loops");

            _timer.Start();

            _loopProperties.Running = true;

            //The user is given a choice not to require each draw to follow at least one update
            //However, this can cause some tricky to debug issues if the first loop draws before any updates
            //have happened in the program before. So we guard for this and enforce at program start 
            //one update must occur
            var originalUserRequest = _loopProperties.DoNotDrawFrameUnlessThereHasBeenOneUpdate;
            _loopProperties.DoNotDrawFrameUnlessThereHasBeenOneUpdate = true;

            while (_loopProperties.Running)
            {
                var timeNow = _timer.Seconds;
                var timeSinceLastUpdate = timeNow - _loopProperties.TimeOfLastUpdate;

                _updater.ProcessRequiredUpdates(timeSinceLastUpdate, _loopProperties, _framesPerSecondMonitor, Update, _timer);

                if (!_loopProperties.DoNotDrawFrameUnlessThereHasBeenOneUpdate || _loopProperties.HasThereBeenAnUpdateSinceTheLastDraw)
                {
                    if (!originalUserRequest && _loopProperties.DoNotDrawFrameUnlessThereHasBeenOneUpdate)
                    {
                        //Unfortunately the above guard means we do the above bool check each iteration in order
                        //to flip back the user decision on updates before draws after enforcing it for the first
                        //time. Perhaps this can be refactored away some how
                        _loopProperties.DoNotDrawFrameUnlessThereHasBeenOneUpdate = false;
                    }

                    if (_graphics.RenderingComplete && !_systemComponents.CurrentlyReinitialisingDevices)
                    {
                        if (_loopProperties.ProcessFractionalUpdatesBeforeDraw)
                        {
                            timeSinceLastUpdate = timeNow - _loopProperties.TimeOfLastUpdate;
                            _updater.ProcessSingleUpdate(timeSinceLastUpdate, _loopProperties, _framesPerSecondMonitor, Update, _timer);
                        }

                        _updater.AnalysePeriod(_timer, _loopProperties);

                        Draw();

                        _updater.MarkStartOfAnalysisPeriod(_timer);
                    }
                }

                _framesPerSecondMonitor.Update();
            }
        }

        private bool Update(float timeStepSeconds)
        {
            if (!_windowUpdater.UpdateAndReturnIfWindowStillExists())
            {
                return false;
            }

            _sdl2EventProcessor.ProcessEvents();

            _inputGameController.Update(timeStepSeconds);
            _inputMouseKeyboard.UpdateVeldridInputSnapshot(_windowUpdater.LatestWindowInputSnapshot, timeStepSeconds);

            if (_coreMessenger.AreThereMessagesInQueue())
            {
                _coreMessenger.ProcessMessageQueue(_loopProperties);
            }

            if (_applicationMessenger.AreThereMessagesInQueue())
            {
                _applicationMessenger.ProcessMessageQueue(_services);
            }

            if (!_application.Update(_services, timeStepSeconds))
            {
                return false;
            }

            return true;
        }

        private void Draw()
        {
            var now = _timer.Seconds;
            var timeSinceLastDraw = now - _loopProperties.TimeOfLastDraw;
            var timeSinceLastUpdate = now - _loopProperties.TimeOfLastUpdate;

            _application.PreDrawing(_services,
                                    (float)timeSinceLastDraw,
                                    (float)timeSinceLastUpdate);

            _graphics.PrepareForDrawing();

            _application.Drawing(_drawing,
                                 _services.FPS,
                                 _services.Input,
                                 _services.Helpers.CoordinateTransforms,
                                 (float)timeSinceLastDraw,
                                 (float)timeSinceLastUpdate);

            _application.Rendering(_renderQueue,
                                   _services.Surfaces.ReturnMainWindowRenderTarget());

            _graphics.Render((float)timeSinceLastDraw);

            _framesPerSecondMonitor.RegisterDrawFrame();

            _loopProperties.TimeOfLastDraw = now;
            _loopProperties.HasThereBeenAnUpdateSinceTheLastDraw = false;
        }

        private void Shutdown()
        {
            _frameworkMessenger.Report("Shutdown Signal given to Application...");
            _application.Shutdown();
            _frameworkMessenger.Report("Yak2D Framework Shutting Down...");
            _shutdownManager.Shutdown();
            _frameworkMessenger.Report("Shutdown Complete - Have a nice day :)");
        }
    }
}