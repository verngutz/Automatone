using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface;

namespace Automatone.GUI
{
    public class LayoutManager
    {
        public const short CONTROLS_AND_GRID_DIVISION = 150;

        public static UniRectangle ControlPanelBounds
        {
            get
            {
                return new UniRectangle(0, 0, Automatone.Instance.Window.ClientBounds.Width, LayoutManager.CONTROLS_AND_GRID_DIVISION);
            }
        }
    }
}
