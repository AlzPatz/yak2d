using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;

namespace Yak2D.Internal
{
    public interface IWindow
    {
        Sdl2Window RawWindow { get; }
        bool Exists { get; }
        bool Focused { get; }
        int X { get; set; }
        int Y { get; set; }
        uint Width { get; set; }
        uint Height { get; set; }
        float Opacity { get; set;  }
        bool Resizable { get; set; }
        bool BorderVisible { get; set; }

        Action Closed { get; set; }
        Action Resized { get; set; }
        Action FocusLost { get; set; }
        Action FocusGained { get; set; }
        Action Closing { get; set; }
        WindowState WindowState { get; set; }
        string Title { get; set; }
        bool CursorVisible { get; set; }

        InputSnapshot PumpEvents();
        void Close();
    }
}
