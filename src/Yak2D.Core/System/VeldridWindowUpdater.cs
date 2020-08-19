using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Core
{
    public class VeldridWindowUpdater : IVeldridWindowUpdater
    {
        public InputSnapshot LatestWindowInputSnapshot { get; private set; }

        private readonly ISystemComponents _components;
        public VeldridWindowUpdater(ISystemComponents components)
        {
            _components = components;
        }

        public bool UpdateAndReturnIfWindowStillExists()
        {
            if (_components.Window.Exists)
            {
                LatestWindowInputSnapshot = _components.Window.PumpEvents();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}