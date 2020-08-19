using Yak2D.Internal;
using Yak2D.Utility;

namespace Yak2D.Core
{
    public class Display : IDisplay
    {
        public bool WindowIsInFocus { get => _systemComponents.Window.Focused; }

        public int WindowPositionX { get => _systemComponents.Window.X; set => _systemComponents.Window.X = value; }
        public int WindowPositionY { get => _systemComponents.Window.Y; set => _systemComponents.Window.Y = value; }

        public uint WindowResolutionWidth { get => (uint)_systemComponents.Window.Width; }
        public uint WindowResolutionHeight { get => (uint)_systemComponents.Window.Height; }

        public float WindowOpacity { get => _systemComponents.Window.Opacity; set => _systemComponents.Window.Opacity = value; }

        public bool WindowResizable { get => _systemComponents.Window.Resizable; set => _systemComponents.Window.Resizable = value; }
        public bool WindowBorderVisible { get => _systemComponents.Window.BorderVisible; set => _systemComponents.Window.BorderVisible = value; }

        public DisplayState DisplayState { get => ReturnDisplayState(); }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IApplicationMessenger _applicationMessenger;
        private readonly ISystemComponents _systemComponents;
        private readonly IGpuSurfaceManager _gpuSurfaceManager;

        public Display(IFrameworkMessenger frameworkMessenger,
                        IApplicationMessenger applicationMessenger,
                        ISystemComponents systemComponents,
                        IGpuSurfaceManager gpuSurfaceManager)
        {
            _frameworkMessenger = frameworkMessenger;
            _applicationMessenger = applicationMessenger;
            _systemComponents = systemComponents;
            _gpuSurfaceManager = gpuSurfaceManager;
            _systemComponents.Window.Resized += ActionWindowResize;
            _systemComponents.Window.FocusLost += ActionWindowLostFocus;
            _systemComponents.Window.FocusGained += ActionWindowGainedFocus;
            _systemComponents.Window.Closing += ActionWindowClosing;
        }

        private void ActionWindowResize()
        {
            ReSizeWindowRenderSurface();
            _applicationMessenger.QueueMessage(FrameworkMessage.WindowWasResized);
        }

        private void ActionWindowGainedFocus()
        {
            _applicationMessenger.QueueMessage(FrameworkMessage.WindowGainedFocus);
        }

        private void ActionWindowLostFocus()
        {
            _applicationMessenger.QueueMessage(FrameworkMessage.WindowLostFocus);
        }

        private void ActionWindowClosing()
        {
            _applicationMessenger.QueueMessage(FrameworkMessage.ApplicationWindowClosing);
        }

        private void ReSizeWindowRenderSurface()
        {
            _systemComponents.Device.ResizeMainWindow(WindowResolutionWidth, WindowResolutionHeight);
            _gpuSurfaceManager.RegisterSwapChainOutput(_systemComponents.Device.SwapchainFramebuffer, true);
            _applicationMessenger.QueueMessage(FrameworkMessage.SwapChainFramebufferReCreated);
            _frameworkMessenger.Report(string.Concat("Window Resized: ", WindowResolutionWidth, " by ", WindowResolutionHeight));
        }

        public void SetWindowResolution(uint resolutionX, uint resolutionY)
        {
            _systemComponents.Window.Width = resolutionX;
            _systemComponents.Window.Height = resolutionY;
        }

        public void SetVsync(bool vsync)
        {
            _systemComponents.Device.SyncToVerticalBlank = vsync;
        }

        public void SetDisplayState(DisplayState state)
        {
            _systemComponents.Window.WindowState = WindowStateConverter.ConvertDisplayStateToVeldridWindowState(state);
            _frameworkMessenger.Report(string.Concat("Window State Set: ", state));
        }

        private DisplayState ReturnDisplayState()
        {
            return WindowStateConverter.ConvertVeldridWindowStateToDisplayState(_systemComponents.Window.WindowState);
        }

        public void SetWindowTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new Yak2DException("Invalid string attempted to be set as window title");
            }

            _systemComponents.Window.Title = title;
        }

        public void SetCursorVisible(bool visible)
        {
            _systemComponents.Window.CursorVisible = visible;
        }
    }
}