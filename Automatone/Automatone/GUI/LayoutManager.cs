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

        public const byte CONTROLS_AND_GRID_DIVISION = 116;

        public const byte CELLHEIGHT = 20;
        public const byte CELLWIDTH = 20;

        public const int TOP_BORDER_THICKNESS = 25;
        public const int LEFT_BORDER_THICKNESS = 25;
        public const int RIGHT_BORDER_THICKNESS = 25;
        public const int BOTTOM_BORDER_THICKNESS = 25;

        public const byte BOTTOM_SCROLLBAR_THICKNESS = 50;
        public const byte RIGHT_SCROLLBAR_THICKNESS = 50;

        private int cellsArrayLengthX;
        private int cellsArrayLengthY;

        private int windowWidth;
        private int windowHeight;

        private UniRectangle controlPanelBounds;
        private UniRectangle playPauseButtonBounds;
        private UniRectangle stopButtonBounds;
        private UniRectangle newButtonBounds;
        private UniRectangle openButtonBounds;
        private UniRectangle saveButtonBounds;
        private UniRectangle cutButtonBounds;
        private UniRectangle copyButtonBounds;
        private UniRectangle pasteButtonBounds;
        private UniRectangle undoButtonBounds;
        private UniRectangle redoButtonBounds;
        private UniRectangle addCellsButtonBounds;
        private UniRectangle removeCellsButtonBounds;
        private UniRectangle generateSongButtonBounds;

        public UniRectangle ControlPanelBounds { get { return controlPanelBounds; } }
        public UniRectangle PlayPauseButtonBounds { get { return playPauseButtonBounds; } }
        public UniRectangle StopButtonBounds { get { return stopButtonBounds; } }
        public UniRectangle NewButtonBounds { get { return newButtonBounds; } }
        public UniRectangle OpenButtonBounds { get { return openButtonBounds; } }
        public UniRectangle SaveButtonBounds { get { return saveButtonBounds; } }
        public UniRectangle CutButtonBounds { get { return cutButtonBounds; } }
        public UniRectangle CopyButtonBounds { get { return copyButtonBounds; } }
        public UniRectangle PasteButtonBounds { get { return pasteButtonBounds; } }
        public UniRectangle UndoButtonBounds { get { return undoButtonBounds; } }
        public UniRectangle RedoButtonBounds { get { return redoButtonBounds; } }
        public UniRectangle AddCellsButtonBounds { get { return addCellsButtonBounds; } }
        public UniRectangle RemoveCellsButtonBounds { get { return removeCellsButtonBounds; } }
        public UniRectangle GenerateSongButtonBounds { get { return generateSongButtonBounds; } }

        private FrameLayout gridPanelLayout;
        private Rectangle gridClickableArea;

        public Rectangle GridTopBorderBounds { get { return gridPanelLayout.TopRectangle; } }
        public Rectangle GridBottomBorderBounds { get { return gridPanelLayout.BottomRectangle; } }
        public Rectangle GridLeftBorderBounds { get { return gridPanelLayout.LeftRectangle; } }
        public Rectangle GridRightBorderBounds { get { return gridPanelLayout.RightRectangle; } }
        public Rectangle GridPanelBounds { get { return gridPanelLayout.OuterRectangle; } }
        public Rectangle GridCellsArea { get { return gridPanelLayout.InnerRectangle; } }
        public Rectangle GridCellsClickableArea { get { return gridClickableArea; } }

        private UniRectangle navigatorPanelBounds;
        private UniRectangle horizontalScrollBarBounds;
        private UniRectangle verticalScrollBarBounds;
        private float horizontalScrollBarThumbSize;
        private float verticalScrollBarThumbSize;

        public UniRectangle NavigatorPanelBounds { get { return navigatorPanelBounds; } }
        public UniRectangle HorizontalScrollBarBounds { get { return horizontalScrollBarBounds; } }
        public UniRectangle VerticalScrollBarBounds { get { return verticalScrollBarBounds; } }
        public float HorizontalScrollBarThumbSize { get { return horizontalScrollBarThumbSize; } }
        public float VerticalScrollBarThumbSize { get { return verticalScrollBarThumbSize; } }

        private LayoutManager() 
        {
            windowWidth = DEFAULT_WINDOW_WIDTH;
            windowHeight = DEFAULT_WINDOW_HEIGHT;
            cellsArrayLengthX = 0;
            cellsArrayLengthY = 0;
            CalculateBounds();

            Automatone.Instance.Window.ClientSizeChanged += delegate 
            {
                RespondToWindowResize(Automatone.Instance.Window.ClientBounds.Width, Automatone.Instance.Window.ClientBounds.Height);
            };
            GridPanel.Instance.SongCellsChanged += new GridPanel.GridLengthChangedEvent(RespondToGridResize);
        }

        private static LayoutManager instance;
        public static LayoutManager Instance
        {
            get 
            { 
                if (instance == null) 
                    instance = new LayoutManager();
                return instance;
            }
        }

        private void CalculateBounds()
        {
            controlPanelBounds = new UniRectangle(0, 0, windowWidth, CONTROLS_AND_GRID_DIVISION);
            playPauseButtonBounds = new UniRectangle(10, 26, 64, 64);
            stopButtonBounds = new UniRectangle(84, 26, 64, 64);
            newButtonBounds = new UniRectangle(158, 10, 64, 96);
            openButtonBounds = new UniRectangle(232, 10, 64, 96);
            saveButtonBounds = new UniRectangle(306, 10, 64, 96);
            cutButtonBounds = new UniRectangle(380, 10, 64, 96);
            copyButtonBounds = new UniRectangle(454, 10, 64, 96);
            pasteButtonBounds = new UniRectangle(528, 10, 64, 96);
            undoButtonBounds = new UniRectangle(602, 10, 64, 96);
            redoButtonBounds = new UniRectangle(676, 10, 64, 96);
            addCellsButtonBounds = new UniRectangle(750, 10, 64, 96);
            removeCellsButtonBounds = new UniRectangle(824, 10, 64, 96);
            generateSongButtonBounds = new UniRectangle(898, 10, 64, 96);

            Rectangle outerRectangle = new Rectangle(0, CONTROLS_AND_GRID_DIVISION, windowWidth - RIGHT_SCROLLBAR_THICKNESS, windowHeight - CONTROLS_AND_GRID_DIVISION - BOTTOM_SCROLLBAR_THICKNESS);
            Rectangle innerRectangle = new Rectangle(outerRectangle.X + LEFT_BORDER_THICKNESS, outerRectangle.Y + TOP_BORDER_THICKNESS, outerRectangle.Width - LEFT_BORDER_THICKNESS - RIGHT_BORDER_THICKNESS, outerRectangle.Height - TOP_BORDER_THICKNESS - BOTTOM_BORDER_THICKNESS);
            gridPanelLayout = new FrameLayout(outerRectangle, innerRectangle);
            RefreshGridClickableArea();

            navigatorPanelBounds = new UniRectangle(0, CONTROLS_AND_GRID_DIVISION, Automatone.Instance.Window.ClientBounds.Width, Automatone.Instance.Window.ClientBounds.Height);
            horizontalScrollBarBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left, gridPanelLayout.OuterRectangle.Height, gridPanelLayout.InnerRectangle.Width, BOTTOM_SCROLLBAR_THICKNESS);
            verticalScrollBarBounds = new UniRectangle(gridPanelLayout.OuterRectangle.Width, gridPanelLayout.InnerRectangle.Top - CONTROLS_AND_GRID_DIVISION, RIGHT_SCROLLBAR_THICKNESS, gridPanelLayout.InnerRectangle.Height);
        }

        private void RespondToWindowResize(int windowWidth, int windowHeight)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            CalculateBounds();

            ControlPanel.Instance.Bounds = ControlPanelBounds;
            ControlPanel.Instance.PlayPauseButtonBounds = PlayPauseButtonBounds;
            ControlPanel.Instance.StopButtonBounds = StopButtonBounds;
            ControlPanel.Instance.NewButtonBounds = NewButtonBounds;
            ControlPanel.Instance.OpenButtonBounds = OpenButtonBounds;
            ControlPanel.Instance.CutButtonBounds = CutButtonBounds;
            ControlPanel.Instance.CopyButtonBounds = CopyButtonBounds;
            ControlPanel.Instance.PasteButtonBounds = PasteButtonBounds;
            ControlPanel.Instance.UndoButtonBounds = UndoButtonBounds;
            ControlPanel.Instance.RedoButtonBounds = RedoButtonBounds;
            ControlPanel.Instance.AddCellsButtonBounds = AddCellsButtonBounds;
            ControlPanel.Instance.RemoveCellsButtonBounds = RemoveCellsButtonBounds;
            ControlPanel.Instance.GenerateSongButtonBounds = GenerateSongButtonBounds;

            NavigatorPanel.Instance.Bounds = NavigatorPanelBounds;
            NavigatorPanel.Instance.HorizontalScrollBarBounds = horizontalScrollBarBounds;
            NavigatorPanel.Instance.VerticalScrollBarBounds = verticalScrollBarBounds;
            NavigatorPanel.Instance.HorizontalScrollBarThumbSize = GetHorizontalSliderThumbSize();
            NavigatorPanel.Instance.VerticalScrollBarThumbSize = GetVerticalSliderThumbSize();
            NavigatorPanel.Instance.CalculateGridOffsetBounds(cellsArrayLengthX);
            NavigatorPanel.Instance.CalculateGridClipping();
        }

        private void RespondToGridResize(int cellsArrayLengthX, int cellsArrayLengthY)
        {
            this.cellsArrayLengthX = cellsArrayLengthX;
            this.cellsArrayLengthY = cellsArrayLengthY;
            RefreshGridClickableArea();

            NavigatorPanel.Instance.HorizontalScrollBarThumbSize = GetHorizontalSliderThumbSize();
            NavigatorPanel.Instance.VerticalScrollBarThumbSize = GetVerticalSliderThumbSize();
            NavigatorPanel.Instance.CalculateGridOffsetBounds(cellsArrayLengthX);
            NavigatorPanel.Instance.CalculateHorizontalClipping();
        }

        private void RefreshGridClickableArea()
        {
            gridClickableArea = new Rectangle
            (
                gridPanelLayout.InnerRectangle.X,
                gridPanelLayout.InnerRectangle.Y,
                Math.Min(gridPanelLayout.InnerRectangle.Width, cellsArrayLengthX * CELLWIDTH),
                Math.Min(gridPanelLayout.InnerRectangle.Height, Automatone.PIANO_SIZE * CELLHEIGHT)
            );
        }

        private float GetHorizontalSliderThumbSize()
        {
            return Math.Min(1, (float)GridCellsClickableArea.Width / (cellsArrayLengthX * CELLWIDTH));
        }

        private float GetVerticalSliderThumbSize()
        {
            return Math.Min(1, (float)GridCellsClickableArea.Height / (cellsArrayLengthY * CELLHEIGHT));
        }

        private class FrameLayout
        {
            private Rectangle topRectangle;
            public Rectangle TopRectangle { get { return topRectangle; } }

            private Rectangle bottomRectangle;
            public Rectangle BottomRectangle { get { return bottomRectangle; } }

            private Rectangle leftRectangle;
            public Rectangle LeftRectangle { get { return leftRectangle; } }

            private Rectangle rightRectangle;
            public Rectangle RightRectangle { get { return rightRectangle; } }

            private Rectangle centerRectangle;
            public Rectangle CenterRectangle { get { return centerRectangle; } }
            public Rectangle InnerRectangle { get { return centerRectangle; } }

            private Rectangle outerRectangle;
            public Rectangle OuterRectangle { get { return outerRectangle; } }

            public FrameLayout(Rectangle outerRectangle, Rectangle innerRectangle)
            {
                centerRectangle = innerRectangle;
                this.outerRectangle = outerRectangle;
                topRectangle = new Rectangle(outerRectangle.Left, outerRectangle.Top, outerRectangle.Width, innerRectangle.Top - outerRectangle.Top);
                bottomRectangle = new Rectangle(outerRectangle.Left, innerRectangle.Bottom, outerRectangle.Width, outerRectangle.Bottom - innerRectangle.Bottom);
                leftRectangle = new Rectangle(outerRectangle.Left, innerRectangle.Top, innerRectangle.Left - outerRectangle.Left, innerRectangle.Height);
                rightRectangle = new Rectangle(innerRectangle.Right, innerRectangle.Top, outerRectangle.Right - innerRectangle.Right, innerRectangle.Height);
            }
        }
    }
}
