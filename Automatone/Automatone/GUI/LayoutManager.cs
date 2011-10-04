using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Nuclex.UserInterface;

namespace Automatone.GUI
{
    public class LayoutManager
    {
        public const short DEFAULT_WINDOW_WIDTH = 800;
        public const short DEFAULT_WINDOW_HEIGHT = 600;

        public const byte CONTROLS_AND_GRID_DIVISION = 150;

        public const byte CELLHEIGHT = 20;
        public const byte CELLWIDTH = 20;

        public const int TOP_BORDER_THICKNESS = 25;
        public const int LEFT_BORDER_THICKNESS = 25;
        public const int RIGHT_BORDER_THICKNESS = 25;
        public const int BOTTOM_BORDER_THICKNESS = 25;

        public const byte BOTTOM_SCROLLBAR_THICKNESS = 50;
        public const byte RIGHT_SCROLLBAR_THICKNESS = 50;

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

        public static UniRectangle OpenButtonBounds
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

        public static UniRectangle AddButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static UniRectangle CopyButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static UniRectangle CutButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static UniRectangle PasteButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static UniRectangle RedoButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static UniRectangle RemoveButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static UniRectangle UndoButtonBounds
        {
            get
            {
                return new UniRectangle(10, 100, 43, 43);
            }
        }

        public static Rectangle GetCellsClickableArea(int songCellsWitdh, int songCellsHeight)
        {
            FramedBounds gridPanelBounds = GridPanelBounds;
            return new Rectangle
            (
                gridPanelBounds.InnerRectangle.X,
                gridPanelBounds.InnerRectangle.Y,
                Math.Min(gridPanelBounds.InnerRectangle.Width, songCellsWitdh * CELLWIDTH),
                Math.Min(gridPanelBounds.InnerRectangle.Height, songCellsHeight * CELLHEIGHT)
            );
        }

        public static FramedBounds GridPanelBounds
        {
            get
            {
                Rectangle outerRectangle = new Rectangle(0, CONTROLS_AND_GRID_DIVISION, Automatone.Instance.Window.ClientBounds.Width - RIGHT_SCROLLBAR_THICKNESS, Automatone.Instance.Window.ClientBounds.Height - CONTROLS_AND_GRID_DIVISION - BOTTOM_SCROLLBAR_THICKNESS);
                Rectangle innerRectangle = new Rectangle(outerRectangle.X + LEFT_BORDER_THICKNESS, outerRectangle.Y + TOP_BORDER_THICKNESS, outerRectangle.Width - LEFT_BORDER_THICKNESS - RIGHT_BORDER_THICKNESS, outerRectangle.Height - TOP_BORDER_THICKNESS - BOTTOM_BORDER_THICKNESS);
                return new FramedBounds(outerRectangle, innerRectangle);
            }
        }

        public static UniRectangle NavigatorBounds
        {
            get
            {
                Rectangle bounds = LayoutManager.GridPanelBounds.OuterRectangle;
                return new UniRectangle(bounds.X, bounds.Y, bounds.Width + RIGHT_SCROLLBAR_THICKNESS, bounds.Height + BOTTOM_SCROLLBAR_THICKNESS);
            }
        }

        public static UniRectangle NavHorizontalScrollBarBounds
        {
            get
            {
                FramedBounds gridPanelBounds = GridPanelBounds;
                UniRectangle a = new UniRectangle(gridPanelBounds.InnerRectangle.Left, gridPanelBounds.BottomRectangle.Bottom, gridPanelBounds.InnerRectangle.Width, BOTTOM_SCROLLBAR_THICKNESS);
                System.Console.WriteLine(a.ToString());
                return a;
            }
        }

        public static UniRectangle NavVerticalScrollBarBounds
        {
            get
            {
                FramedBounds gridPanelBounds = GridPanelBounds;
                return new UniRectangle(gridPanelBounds.RightRectangle.Right, gridPanelBounds.InnerRectangle.Top, RIGHT_SCROLLBAR_THICKNESS, gridPanelBounds.InnerRectangle.Height);
            }
        }

        public static float GetHorizontalSliderThumbSize(int songCellsWidth)
        {
            return GridPanelBounds.InnerRectangle.Width / songCellsWidth * CELLWIDTH;
        }
    }
}
