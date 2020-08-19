using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class SdlWindow : IWindow
    {
        public Sdl2Window RawWindow { get; private set; }
        public Action Closed { get; set; }
        public Action Resized { get; set; }
        public Action FocusLost { get; set; }
        public Action FocusGained { get; set; }
        public Action Closing { get; set; }

        public bool Exists => RawWindow.Exists;

        public bool Focused => RawWindow.Focused;

        public int X { get { return RawWindow.X; } set { RawWindow.X = value; } }
        public int Y { get { return RawWindow.Y; } set { RawWindow.Y = value; } }
        public uint Width { get { return (uint)RawWindow.Width; } set { RawWindow.Width = (int)value; } }
        public uint Height { get { return (uint)RawWindow.Height; } set { RawWindow.Height = (int)value; } }
        public float Opacity { get { return RawWindow.Opacity; } set { RawWindow.Opacity = value; } }
        public bool Resizable { get { return RawWindow.Resizable; } set { RawWindow.Resizable = value; } }
        public bool BorderVisible { get { return RawWindow.BorderVisible; } set { RawWindow.BorderVisible = value; } }


        public WindowState WindowState { get { return RawWindow.WindowState; } set { RawWindow.WindowState = value; } }
        public string Title { get { return RawWindow.Title; } set { RawWindow.Title = value; } }
        public bool CursorVisible { get { return RawWindow.CursorVisible; } set { RawWindow.CursorVisible = value; } }

        public SdlWindow(Sdl2Window window)
        {
            RawWindow = window;

            RawWindow.Closed += () => { Closed?.Invoke(); };
            RawWindow.Resized += () => { Resized?.Invoke(); };
            RawWindow.FocusLost += () => { FocusLost?.Invoke(); };
            RawWindow.FocusGained += () => { FocusGained?.Invoke(); };
            RawWindow.Closing += () => { Closing?.Invoke(); };
        }

        public InputSnapshot PumpEvents() => RawWindow.PumpEvents();

        public void Close() => RawWindow.Close();
    }
}