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

        public static UniRectangle GenerateSongButtonBounds
        {
            get
            {
                return new UniRectangle(10, 10, 163, 48);
            }
        }

        public static UniRectangle PlayPauseButtonBounds
        {
            get
            {
                return new UniRectangle(10, 55, 43, 43);
            }
        }

        public static UniRectangle StopButtonBounds
        {
            get
            {
                return new UniRectangle(55, 55, 43, 43);
            }
        }

        public static UniRectangle SaveButtonBounds
        {
            get
            {
                return new UniRectangle(55, 100, 43, 43);
            }
        }

        public static UniRectangle LoadButtonBounds
        {
            get
            {
                return new UniRectangle(100, 100, 43, 43);
            }
        }

        public static UniRectangle NewButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }
    }
}
