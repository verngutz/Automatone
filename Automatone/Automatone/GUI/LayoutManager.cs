using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface;

namespace Automatone.GUI
{
    public class LayoutManager
    {
        public const short CONTROLS_AND_GRID_DIVISION = 116;

        public static UniRectangle ControlPanelBounds
        {
            get
            {
                return new UniRectangle(0, 0, Automatone.Instance.Window.ClientBounds.Width, LayoutManager.CONTROLS_AND_GRID_DIVISION);
            }
        }

        public static UniRectangle PlayPauseButtonBounds
        {
            get
            {
                return new UniRectangle(10, 26, 64, 64);
            }
        }

        public static UniRectangle StopButtonBounds
        {
            get
            {
                return new UniRectangle(84, 26, 64, 64);
            }
        }

        public static UniRectangle NewButtonBounds
        {
            get
            {
                return new UniRectangle(158, 10, 64, 96);
            }
        }

        public static UniRectangle OpenButtonBounds
        {
            get
            {
                return new UniRectangle(232, 10, 64, 96);
            }
        }

        public static UniRectangle SaveButtonBounds
        {
            get
            {
                return new UniRectangle(306, 10, 64, 96);
            }
        }

        public static UniRectangle CutButtonBounds
        {
            get
            {
                return new UniRectangle(380, 10, 64, 96);
            }
        }

        public static UniRectangle CopyButtonBounds
        {
            get
            {
                return new UniRectangle(454, 10, 64, 96);
            }
        }

        public static UniRectangle PasteButtonBounds
        {
            get
            {
                return new UniRectangle(528, 10, 64, 96);
            }
        }

        public static UniRectangle UndoButtonBounds
        {
            get
            {
                return new UniRectangle(602, 10, 64, 96);
            }
        }

        public static UniRectangle RedoButtonBounds
        {
            get
            {
                return new UniRectangle(676, 10, 64, 96);
            }
        }

        public static UniRectangle AddButtonBounds
        {
            get
            {
                return new UniRectangle(750, 10, 64, 96);
            }
        }

        public static UniRectangle RemoveButtonBounds
        {
            get
            {
                return new UniRectangle(824, 10, 64, 96);
            }
        }

        public static UniRectangle GenerateSongButtonBounds
        {
            get
            {
                return new UniRectangle(898, 10, 64, 96);
            }
        }
    }
}
