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

        public const short PARAMETERS_PANEL_HEIGHT = 400;

        public const int CONTROL_ARROW_WIDTH = 32;
        public const int CONTROL_ARROW_HEIGHT = 96;
        public const int CONTROL_BUTTON_WIDTH = 64;
        public const int CONTROL_BUTTON_HEIGHT = 96;
        public const int CONTROL_BUTTON_SPACING = 10;

        private int cellsArrayLengthX;
        private int cellsArrayLengthY;

        private int windowWidth;
        private int windowHeight;

        private UniRectangle controlPanelBounds;

        public UniRectangle ControlPanelBounds { get { return controlPanelBounds; } }

        private UniRectangle controlArrowLeftBounds;
        private UniRectangle controlArrowRightBounds;

        public UniRectangle ControlArrowLeftBounds { get { return controlArrowLeftBounds; } }
        public UniRectangle ControlArrowRightBounds { get { return controlArrowRightBounds; } }

        private bool excessButtonsLeft;
        private bool excessButtonsRight;

        public bool ExcessButtonsLeft { get { return excessButtonsLeft; } }
        public bool ExcessButtonsRight { get { return excessButtonsRight; } }

        private FrameLayout gridPanelLayout;
        private Rectangle gridCellsClickableArea;
        private Rectangle gridTopCursorsClickableArea;
        private Rectangle gridLeftCursorsClickableArea;

        public Rectangle GridTopBorderBounds { get { return gridPanelLayout.TopRectangle; } }
        public Rectangle GridBottomBorderBounds { get { return gridPanelLayout.BottomRectangle; } }
        public Rectangle GridLeftBorderBounds { get { return gridPanelLayout.LeftRectangle; } }
        public Rectangle GridRightBorderBounds { get { return gridPanelLayout.RightRectangle; } }
        public Rectangle GridPanelBounds { get { return gridPanelLayout.OuterRectangle; } }
        public Rectangle GridCellsArea { get { return gridPanelLayout.InnerRectangle; } }
        public Rectangle GridCellsClickableArea { get { return gridCellsClickableArea; } }
        public Rectangle GridTopCursorsClickableArea { get { return gridTopCursorsClickableArea; } }
        public Rectangle GridLeftCursorsClickableArea { get { return gridLeftCursorsClickableArea; } }

        private UniRectangle navigatorPanelBounds;
        private UniRectangle horizontalScrollBarBounds;
        private UniRectangle verticalScrollBarBounds;

        public UniRectangle NavigatorPanelBounds { get { return navigatorPanelBounds; } }
        public UniRectangle HorizontalScrollBarBounds { get { return horizontalScrollBarBounds; } }
        public UniRectangle VerticalScrollBarBounds { get { return verticalScrollBarBounds; } }

        public float HorizontalSliderThumbSize
        {
            get
            {
                return Math.Min(1, (float)GridCellsClickableArea.Width / (cellsArrayLengthX * CELLWIDTH));
            }
        }

        public float VerticalSliderThumbSize
        {
            get
            {
                return Math.Min(1, (float)GridCellsClickableArea.Height / (cellsArrayLengthY * CELLHEIGHT));
            }
        }

        private UniRectangle parametersPanelBounds;
        private UniRectangle globalRandomizeButtonBounds;
        private UniRectangle okButtonBounds;
        private UniRectangle cancelButtonBounds;

        public UniRectangle ParametersPanelBounds { get { return parametersPanelBounds; } }
        public UniRectangle GlobalRandomizeButtonBounds { get { return globalRandomizeButtonBounds; } }
        public UniRectangle OkButtonBounds { get { return okButtonBounds; } }
        public UniRectangle CancelButtonBounds { get { return cancelButtonBounds; } }

        public int VisibleButtonCount
        {
            get
            {
                return ((windowWidth - CONTROL_BUTTON_SPACING * 3 - CONTROL_ARROW_WIDTH * 2) / (CONTROL_BUTTON_WIDTH + CONTROL_BUTTON_SPACING));
            }
        }

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

        public List<UniRectangle> GetButtonsetBounds(int buttonCount, int firstButton)
        {
            List<UniRectangle> buttonsetBounds = new List<UniRectangle>();
            excessButtonsLeft = false;
            excessButtonsRight = false;
            for (int i = 0; i < buttonCount; i++)
            {
                if (i < firstButton)
                {
                    buttonsetBounds.Add(new UniRectangle(windowWidth + CONTROL_BUTTON_WIDTH, (CONTROLS_AND_GRID_DIVISION - CONTROL_BUTTON_HEIGHT) / 2, CONTROL_BUTTON_WIDTH, CONTROL_BUTTON_HEIGHT));
                    excessButtonsLeft = true;
                }
                else if ( i < firstButton + VisibleButtonCount)
                {
                    buttonsetBounds.Add(new UniRectangle(CONTROL_ARROW_WIDTH + CONTROL_BUTTON_SPACING * (2 + i - firstButton) + CONTROL_BUTTON_WIDTH * (i - firstButton), (CONTROLS_AND_GRID_DIVISION - CONTROL_BUTTON_HEIGHT) / 2, CONTROL_BUTTON_WIDTH, CONTROL_BUTTON_HEIGHT));
                }
                else
                {
                    buttonsetBounds.Add(new UniRectangle(windowWidth + CONTROL_BUTTON_WIDTH, (CONTROLS_AND_GRID_DIVISION - CONTROL_BUTTON_HEIGHT) / 2, CONTROL_BUTTON_WIDTH, CONTROL_BUTTON_HEIGHT));
                    excessButtonsRight = true;
                }
            }
            return buttonsetBounds;
        }

        private void CalculateBounds()
        {
            controlPanelBounds = new UniRectangle(0, 0, windowWidth, CONTROLS_AND_GRID_DIVISION);

            controlArrowLeftBounds = new UniRectangle(CONTROL_BUTTON_SPACING, (CONTROLS_AND_GRID_DIVISION - CONTROL_ARROW_HEIGHT) / 2, CONTROL_ARROW_WIDTH, CONTROL_ARROW_HEIGHT);
            controlArrowRightBounds = new UniRectangle(windowWidth - CONTROL_BUTTON_SPACING - CONTROL_ARROW_WIDTH, (CONTROLS_AND_GRID_DIVISION - CONTROL_ARROW_HEIGHT) / 2, CONTROL_ARROW_WIDTH, CONTROL_ARROW_HEIGHT);

            Rectangle outerRectangle = new Rectangle(0, CONTROLS_AND_GRID_DIVISION, windowWidth - RIGHT_SCROLLBAR_THICKNESS, windowHeight - CONTROLS_AND_GRID_DIVISION - BOTTOM_SCROLLBAR_THICKNESS);
            Rectangle innerRectangle = new Rectangle(outerRectangle.X + LEFT_BORDER_THICKNESS, outerRectangle.Y + TOP_BORDER_THICKNESS, outerRectangle.Width - LEFT_BORDER_THICKNESS - RIGHT_BORDER_THICKNESS, outerRectangle.Height - TOP_BORDER_THICKNESS - BOTTOM_BORDER_THICKNESS);
            gridPanelLayout = new FrameLayout(outerRectangle, innerRectangle);
            RefreshGridCellsClickableArea();
            RefreshGridTopCursorsClickableArea();
            RefreshGridLeftCursorsClickableArea();

            navigatorPanelBounds = new UniRectangle(0, CONTROLS_AND_GRID_DIVISION, windowWidth, windowHeight);
            horizontalScrollBarBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left, gridPanelLayout.OuterRectangle.Height, gridPanelLayout.InnerRectangle.Width, BOTTOM_SCROLLBAR_THICKNESS);
            verticalScrollBarBounds = new UniRectangle(gridPanelLayout.OuterRectangle.Width, gridPanelLayout.InnerRectangle.Top - CONTROLS_AND_GRID_DIVISION, RIGHT_SCROLLBAR_THICKNESS, gridPanelLayout.InnerRectangle.Height);

            if (parametersPanelBounds.Top == 0)
                parametersPanelBounds = new UniRectangle(CONTROL_BUTTON_SPACING, CONTROLS_AND_GRID_DIVISION - PARAMETERS_PANEL_HEIGHT, windowWidth - 2 * CONTROL_BUTTON_SPACING, PARAMETERS_PANEL_HEIGHT);
            else
                parametersPanelBounds = new UniRectangle(CONTROL_BUTTON_SPACING, ParametersPanel.Instance.Bounds.Top, windowWidth - 2 * CONTROL_BUTTON_SPACING, PARAMETERS_PANEL_HEIGHT);

            globalRandomizeButtonBounds = new UniRectangle(new UniScalar(0.5f, -32), 10, 64, 64);
            okButtonBounds = new UniRectangle(new UniScalar(0.33f, -32), new UniScalar(1, -74), 64, 64);
            cancelButtonBounds = new UniRectangle(new UniScalar(0.66f, -32), new UniScalar(1, -74), 64, 64);
        }

        private void RespondToWindowResize(int windowWidth, int windowHeight)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            CalculateBounds();

            ControlPanel.Instance.Bounds = ControlPanelBounds;

            ControlPanel.Instance.ArrowLeftBounds = ControlArrowLeftBounds;
            ControlPanel.Instance.ArrowRightBounds = controlArrowRightBounds;
            ControlPanel.Instance.ResetButtonsetBounds();

            NavigatorPanel.Instance.Bounds = NavigatorPanelBounds;
            NavigatorPanel.Instance.HorizontalScrollBarBounds = HorizontalScrollBarBounds;
            NavigatorPanel.Instance.VerticalScrollBarBounds = VerticalScrollBarBounds;
            NavigatorPanel.Instance.HorizontalScrollBarThumbSize = HorizontalSliderThumbSize;
            NavigatorPanel.Instance.VerticalScrollBarThumbSize = VerticalSliderThumbSize;
            NavigatorPanel.Instance.CalculateGridOffsetBounds(cellsArrayLengthX);
            NavigatorPanel.Instance.CalculateGridClipping();

            ParametersPanel.Instance.Bounds = ParametersPanelBounds;
            ParametersPanel.Instance.GlobalRandomizeButtonBounds = GlobalRandomizeButtonBounds;
            ParametersPanel.Instance.OkButtonBounds = OkButtonBounds;
            ParametersPanel.Instance.CancelButtonBounds = CancelButtonBounds;
        }

        private void RespondToGridResize(int cellsArrayLengthX, int cellsArrayLengthY)
        {
            this.cellsArrayLengthX = cellsArrayLengthX;
            this.cellsArrayLengthY = cellsArrayLengthY;
            RefreshGridCellsClickableArea();
            RefreshGridTopCursorsClickableArea();
            RefreshGridLeftCursorsClickableArea();

            NavigatorPanel.Instance.HorizontalScrollBarThumbSize = HorizontalSliderThumbSize;
            NavigatorPanel.Instance.VerticalScrollBarThumbSize = VerticalSliderThumbSize;
            NavigatorPanel.Instance.CalculateGridOffsetBounds(cellsArrayLengthX);
            NavigatorPanel.Instance.CalculateHorizontalClipping();
        }

        private void RefreshGridCellsClickableArea()
        {
            gridCellsClickableArea = new Rectangle
            (
                gridPanelLayout.InnerRectangle.X,
                gridPanelLayout.InnerRectangle.Y,
                Math.Min(gridPanelLayout.InnerRectangle.Width, cellsArrayLengthX * CELLWIDTH),
                Math.Min(gridPanelLayout.InnerRectangle.Height, Automatone.PIANO_SIZE * CELLHEIGHT)
            );
        }

        private void RefreshGridTopCursorsClickableArea()
        {
            gridTopCursorsClickableArea = new Rectangle
            (
                gridPanelLayout.InnerRectangle.X,
                gridPanelLayout.OuterRectangle.Y,
                Math.Min(gridPanelLayout.InnerRectangle.Width, cellsArrayLengthX * CELLWIDTH),
                TOP_BORDER_THICKNESS
            );
        }

        private void RefreshGridLeftCursorsClickableArea()
        {
            gridLeftCursorsClickableArea = new Rectangle
            (
                0,
                gridPanelLayout.InnerRectangle.Y,
                LEFT_BORDER_THICKNESS,
                Math.Min(gridPanelLayout.InnerRectangle.Height, cellsArrayLengthY * CELLHEIGHT)
            );
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
