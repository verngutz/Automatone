using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls;
using NuclexUserInterfaceExtension;

namespace Automatone.GUI
{
    public class LayoutManager
    {
        public const short DEFAULT_WINDOW_WIDTH = 800;
        public const short DEFAULT_WINDOW_HEIGHT = 600;

        public const short MINIMUM_WINDOW_WIDTH = 640;
        public const short MINIMUM_WINDOW_HEIGHT = 480;

        public const byte CONTROLS_AND_GRID_DIVISION = 116;

        public const byte CELLHEIGHT = 20;
        public const byte CELLWIDTH = 20;

        public const byte TOP_BORDER_THICKNESS = 35;
        public const byte LEFT_BORDER_THICKNESS = 35;
        public const byte RIGHT_BORDER_THICKNESS = 35;
        public const byte BOTTOM_BORDER_THICKNESS = 35;

        public const byte TOP_CURSOR_HEAD_HEIGHT = 20;
        public const byte TOP_CURSOR_HEAD_WIDTH = 20;
        public const byte LEFT_CURSOR_HEAD_HEIGHT = 20;
        public const byte LEFT_CURSOR_HEAD_WIDTH = 20;
        public const byte HORIZONTAL_CURSOR_HEIGHT = 20;
        public const byte VERTICAL_CURSOR_WIDTH = 20;

        public const byte BOTTOM_SCROLLBAR_THICKNESS = 50;
        public const byte RIGHT_SCROLLBAR_THICKNESS = 50;

        public const byte CONTROL_ARROW_WIDTH = 32;
        public const byte CONTROL_ARROW_HEIGHT = 96;
        public const byte CONTROL_BUTTON_WIDTH = 64;
        public const byte CONTROL_BUTTON_HEIGHT = 96;
        public const byte CONTROL_BUTTON_SPACING = 10;

        public const byte MEDIA_BUTTON_HEIGHT = 64;

        public const byte PARAMETERS_BUTTON_WIDTH = 64;
        public const byte PARAMETERS_BUTTON_HEIGHT = 64;
        public const byte PARAMETERS_BUTTON_SPACING = 10;

        public const byte PARAMETERS_PANEL_TOP_MARGIN = 10;
        public const byte PARAMETERS_PANEL_BOTTOM_MARGIN = 80;
        public const byte PARAMETERS_PANEL_LEFT_MARGIN = 10;
        public const byte PARAMETERS_PANEL_RIGHT_MARGIN = 10;

        public const byte PARAMETER_SLIDER_WIDTH = 180;
        public const byte PARAMETER_SLIDER_HEIGHT = 15;
        public const byte PARAMETER_SLIDER_SPACING = 8;

        public const byte PARAMETER_LABEL_WIDTH = 180;
        public const byte PARAMETER_LABEL_HEIGHT = 10;

        public const byte MISC_MUSIC_SETTINGS_PADDING = 5;
        public const byte MISC_MUSIC_SETTINGS_LABEL_HEIGHT = 30;

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
        private UniRectangle playPauseButtonBounds;
        private UniRectangle stopButtonBounds;
        private UniRectangle horizontalScrollBarBounds;
        private UniRectangle verticalScrollBarBounds;
        private UniRectangle tempoLabelBounds;
        private UniRectangle timeSignatureNLabelBounds;
        private UniRectangle timeSignatureDLabelBounds;

        public UniRectangle NavigatorPanelBounds { get { return navigatorPanelBounds; } }
        public UniRectangle PlayPauseButtonBounds { get { return playPauseButtonBounds; } }
        public UniRectangle StopButtonBounds { get { return stopButtonBounds; } }
        public UniRectangle HorizontalScrollBarBounds { get { return horizontalScrollBarBounds; } }
        public UniRectangle VerticalScrollBarBounds { get { return verticalScrollBarBounds; } }
        public UniRectangle TempoLabelBounds { get { return tempoLabelBounds; } }
        public UniRectangle TimeSignatureNLabelBounds { get { return timeSignatureNLabelBounds; } }
        public UniRectangle TimeSignatureDLabelBounds { get { return timeSignatureDLabelBounds; } }

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

        private List<SkinNamedHorizontalSliderControl> parametersSliders;
        private List<SkinNamedLabelControl> parametersLabels;

        public int VisibleButtonCount
        {
            get
            {
                return ((windowWidth - CONTROL_BUTTON_SPACING * 3 - CONTROL_ARROW_WIDTH * 2) / (CONTROL_BUTTON_WIDTH + CONTROL_BUTTON_SPACING));
            }
        }

        private LayoutManager() 
        {
            parametersSliders = new List<SkinNamedHorizontalSliderControl>();
            parametersLabels = new List<SkinNamedLabelControl>();

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

        public void Register(SkinNamedHorizontalSliderControl parameterSlider)
        {
            parameterSlider.Bounds = GetParameterSliderBounds(parametersSliders.Count);
            parametersSliders.Add(parameterSlider);
        }

        public void Register(SkinNamedLabelControl sliderLabel)
        {
            sliderLabel.Bounds = GetParameterLabelBounds(parametersLabels.Count);
            parametersLabels.Add(sliderLabel);
        }

        private UniRectangle GetParameterSliderBounds(int index)
        {
            float boundsX;
            float boundsY;
            if (index == 0)
            {
                boundsX = PARAMETERS_PANEL_LEFT_MARGIN;
                boundsY = PARAMETERS_PANEL_TOP_MARGIN + PARAMETER_LABEL_HEIGHT + PARAMETER_SLIDER_SPACING;
            }
            else
            {
                boundsX = parametersSliders.ElementAt<SkinNamedHorizontalSliderControl>(index - 1).Bounds.Location.X.Offset;
                boundsY = parametersSliders.ElementAt<SkinNamedHorizontalSliderControl>(index - 1).Bounds.Location.Y.Offset
                    + PARAMETER_SLIDER_HEIGHT
                    + PARAMETER_SLIDER_SPACING
                    + PARAMETER_LABEL_HEIGHT
                    + PARAMETER_SLIDER_SPACING;

                if (boundsY + PARAMETER_SLIDER_HEIGHT + PARAMETERS_PANEL_BOTTOM_MARGIN > ParametersPanelBounds.Size.Y.Offset)
                {
                    boundsX += PARAMETER_SLIDER_SPACING + PARAMETER_SLIDER_WIDTH;
                    boundsY = PARAMETERS_PANEL_TOP_MARGIN + PARAMETER_LABEL_HEIGHT + PARAMETER_SLIDER_SPACING;
                }
            }

            return new UniRectangle(boundsX, boundsY, PARAMETER_SLIDER_WIDTH, PARAMETER_SLIDER_HEIGHT);
        }

        private UniRectangle GetParameterLabelBounds(int index)
        {
            float boundsX;
            float boundsY;
            if (index == 0)
            {
                boundsX = PARAMETERS_PANEL_LEFT_MARGIN;
                boundsY = PARAMETERS_PANEL_TOP_MARGIN;
            }
            else
            {
                boundsX = parametersLabels.ElementAt<LabelControl>(index - 1).Bounds.Location.X.Offset;
                boundsY = parametersLabels.ElementAt<LabelControl>(index - 1).Bounds.Location.Y.Offset
                    + PARAMETER_LABEL_HEIGHT
                    + PARAMETER_SLIDER_SPACING
                    + PARAMETER_SLIDER_HEIGHT
                    + PARAMETER_SLIDER_SPACING;

                if (boundsY 
                    + PARAMETER_LABEL_HEIGHT 
                    + PARAMETER_SLIDER_SPACING 
                    + PARAMETER_SLIDER_HEIGHT 
                    + PARAMETERS_PANEL_BOTTOM_MARGIN 
                    > ParametersPanelBounds.Size.Y.Offset)
                {
                    boundsX += PARAMETER_SLIDER_SPACING + PARAMETER_LABEL_WIDTH;
                    boundsY = PARAMETERS_PANEL_TOP_MARGIN;
                }
            }

            return new UniRectangle(boundsX, boundsY, PARAMETER_LABEL_WIDTH, PARAMETER_LABEL_HEIGHT);
        }

        private void CalculateBounds()
        {
            controlPanelBounds = new UniRectangle(0, 0, windowWidth, CONTROLS_AND_GRID_DIVISION);

            controlArrowLeftBounds = new UniRectangle(CONTROL_BUTTON_SPACING, (CONTROLS_AND_GRID_DIVISION - CONTROL_ARROW_HEIGHT) / 2, CONTROL_ARROW_WIDTH, CONTROL_ARROW_HEIGHT);
            controlArrowRightBounds = new UniRectangle(windowWidth - CONTROL_BUTTON_SPACING - CONTROL_ARROW_WIDTH, (CONTROLS_AND_GRID_DIVISION - CONTROL_ARROW_HEIGHT) / 2, CONTROL_ARROW_WIDTH, CONTROL_ARROW_HEIGHT);

            Rectangle outerRectangle = new Rectangle(0, CONTROLS_AND_GRID_DIVISION, windowWidth - RIGHT_SCROLLBAR_THICKNESS, windowHeight - CONTROLS_AND_GRID_DIVISION - BOTTOM_SCROLLBAR_THICKNESS - MEDIA_BUTTON_HEIGHT);
            Rectangle innerRectangle = new Rectangle(outerRectangle.X + LEFT_BORDER_THICKNESS, outerRectangle.Y + TOP_BORDER_THICKNESS, outerRectangle.Width - LEFT_BORDER_THICKNESS - RIGHT_BORDER_THICKNESS, outerRectangle.Height - TOP_BORDER_THICKNESS - BOTTOM_BORDER_THICKNESS);
            gridPanelLayout = new FrameLayout(outerRectangle, innerRectangle);
            RefreshGridCellsClickableArea();
            RefreshGridTopCursorsClickableArea();
            RefreshGridLeftCursorsClickableArea();

            navigatorPanelBounds = new UniRectangle(0, CONTROLS_AND_GRID_DIVISION, windowWidth, windowHeight);
            playPauseButtonBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left, gridPanelLayout.OuterRectangle.Height + BOTTOM_SCROLLBAR_THICKNESS, CONTROL_BUTTON_WIDTH, MEDIA_BUTTON_HEIGHT);
            stopButtonBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left + CONTROL_BUTTON_WIDTH + CONTROL_BUTTON_SPACING, gridPanelLayout.OuterRectangle.Height + BOTTOM_SCROLLBAR_THICKNESS, CONTROL_BUTTON_WIDTH, MEDIA_BUTTON_HEIGHT);
            horizontalScrollBarBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left, gridPanelLayout.OuterRectangle.Height, gridPanelLayout.InnerRectangle.Width, BOTTOM_SCROLLBAR_THICKNESS);
            verticalScrollBarBounds = new UniRectangle(gridPanelLayout.OuterRectangle.Width, gridPanelLayout.InnerRectangle.Top - CONTROLS_AND_GRID_DIVISION, RIGHT_SCROLLBAR_THICKNESS, gridPanelLayout.InnerRectangle.Height);
            tempoLabelBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left + 4 * (CONTROL_BUTTON_WIDTH + CONTROL_BUTTON_SPACING), gridPanelLayout.OuterRectangle.Height + BOTTOM_SCROLLBAR_THICKNESS + MISC_MUSIC_SETTINGS_PADDING, CONTROL_BUTTON_WIDTH, MEDIA_BUTTON_HEIGHT);
            timeSignatureNLabelBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left + 7 * (CONTROL_BUTTON_WIDTH + CONTROL_BUTTON_SPACING), gridPanelLayout.OuterRectangle.Height + BOTTOM_SCROLLBAR_THICKNESS + MISC_MUSIC_SETTINGS_PADDING, CONTROL_BUTTON_WIDTH, MEDIA_BUTTON_HEIGHT);
            timeSignatureDLabelBounds = new UniRectangle(gridPanelLayout.InnerRectangle.Left + 7 * (CONTROL_BUTTON_WIDTH + CONTROL_BUTTON_SPACING), gridPanelLayout.OuterRectangle.Height + BOTTOM_SCROLLBAR_THICKNESS + MISC_MUSIC_SETTINGS_PADDING + MISC_MUSIC_SETTINGS_LABEL_HEIGHT, CONTROL_BUTTON_WIDTH, MEDIA_BUTTON_HEIGHT);

            if (parametersPanelBounds.Top == 0)
                parametersPanelBounds = new UniRectangle(0, CONTROLS_AND_GRID_DIVISION - windowHeight, windowWidth, windowHeight - CONTROLS_AND_GRID_DIVISION - MEDIA_BUTTON_HEIGHT);
            else
                parametersPanelBounds = new UniRectangle(0, ParametersPanel.Instance.Bounds.Top, windowWidth, windowHeight - CONTROLS_AND_GRID_DIVISION - MEDIA_BUTTON_HEIGHT);

            globalRandomizeButtonBounds = new UniRectangle(new UniScalar(1, -PARAMETERS_BUTTON_SPACING * 3 - PARAMETERS_BUTTON_WIDTH * 3), new UniScalar(1, -PARAMETERS_BUTTON_SPACING - PARAMETERS_BUTTON_HEIGHT), PARAMETERS_BUTTON_WIDTH, PARAMETERS_BUTTON_HEIGHT);
            okButtonBounds = new UniRectangle(new UniScalar(1, -PARAMETERS_BUTTON_SPACING * 2 - PARAMETERS_BUTTON_WIDTH * 2), new UniScalar(1, -PARAMETERS_BUTTON_SPACING - PARAMETERS_BUTTON_HEIGHT), PARAMETERS_BUTTON_WIDTH, PARAMETERS_BUTTON_HEIGHT);
            cancelButtonBounds = new UniRectangle(new UniScalar(1, -PARAMETERS_BUTTON_SPACING - PARAMETERS_BUTTON_WIDTH), new UniScalar(1, -PARAMETERS_BUTTON_SPACING - PARAMETERS_BUTTON_HEIGHT), PARAMETERS_BUTTON_WIDTH, PARAMETERS_BUTTON_HEIGHT);

            for (int i = 0; i < parametersSliders.Count; i++)
            {
                parametersSliders.ElementAt<SkinNamedHorizontalSliderControl>(i).Bounds = GetParameterSliderBounds(i);
            }

            for (int i = 0; i < parametersLabels.Count; i++)
            {
                parametersLabels.ElementAt<LabelControl>(i).Bounds = GetParameterLabelBounds(i);
            }
        }

        private void RespondToWindowResize(int windowWidth, int windowHeight)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            CalculateBounds();
            RefreshControlPanel();
            RefreshNavigatorsPanel();
            RefreshParametersPanel();
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

        private void RefreshControlPanel()
        {
            ControlPanel.Instance.Bounds = ControlPanelBounds;
            ControlPanel.Instance.ArrowLeftBounds = ControlArrowLeftBounds;
            ControlPanel.Instance.ArrowRightBounds = controlArrowRightBounds;
            ControlPanel.Instance.ResetButtonsetBounds();
        }

        private void RefreshNavigatorsPanel()
        {
            NavigatorPanel.Instance.Bounds = NavigatorPanelBounds;
            NavigatorPanel.Instance.PlayPauseButtonBounds = PlayPauseButtonBounds;
            NavigatorPanel.Instance.StopButtonBounds = StopButtonBounds;
            NavigatorPanel.Instance.HorizontalScrollBarBounds = HorizontalScrollBarBounds;
            NavigatorPanel.Instance.VerticalScrollBarBounds = VerticalScrollBarBounds;
            NavigatorPanel.Instance.HorizontalScrollBarThumbSize = HorizontalSliderThumbSize;
            NavigatorPanel.Instance.VerticalScrollBarThumbSize = VerticalSliderThumbSize;
            NavigatorPanel.Instance.TempoLabelBounds = TempoLabelBounds;
            NavigatorPanel.Instance.TimeSignatureNLabelBounds = TimeSignatureNLabelBounds;
            NavigatorPanel.Instance.TimeSignatureDLabelBounds = TimeSignatureDLabelBounds;
            NavigatorPanel.Instance.CalculateGridOffsetBounds(cellsArrayLengthX);
            NavigatorPanel.Instance.CalculateGridClipping();
        }

        private void RefreshParametersPanel()
        {
            ParametersPanel.Instance.Bounds = ParametersPanelBounds;
            ParametersPanel.Instance.GlobalRandomizeButtonBounds = GlobalRandomizeButtonBounds;
            ParametersPanel.Instance.OkButtonBounds = OkButtonBounds;
            ParametersPanel.Instance.CancelButtonBounds = CancelButtonBounds;
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
