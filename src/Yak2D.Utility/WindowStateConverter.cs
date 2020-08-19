using Veldrid;

namespace Yak2D.Utility
{
    public static class WindowStateConverter
    {
        public static WindowState ConvertDisplayStateToVeldridWindowState(DisplayState state)
        {
            switch (state)
            {
                case DisplayState.Normal:
                    return WindowState.Normal;
                case DisplayState.Minimised:
                    return WindowState.Minimized;
                case DisplayState.Maximised:
                    return WindowState.Maximized;
                case DisplayState.FullScreen:
                    return WindowState.FullScreen;
                case DisplayState.BorderlessFullScreen:
                    return WindowState.BorderlessFullScreen;
                case DisplayState.Hidden:
                    return WindowState.Hidden;
                default:
                    return WindowState.Normal;
            }
        }

        public static DisplayState ConvertVeldridWindowStateToDisplayState(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    return DisplayState.Normal;
                case WindowState.Minimized:
                    return DisplayState.Minimised;
                case WindowState.Maximized:
                    return DisplayState.Maximised;
                case WindowState.FullScreen:
                    return DisplayState.FullScreen;
                case WindowState.BorderlessFullScreen:
                    return DisplayState.BorderlessFullScreen;
                case WindowState.Hidden:
                    return DisplayState.Hidden;
                default:
                    return DisplayState.Normal;
            }
        }
    }
}