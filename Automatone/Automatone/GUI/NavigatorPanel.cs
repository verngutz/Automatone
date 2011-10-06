using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Duet.Audio_System;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using NuclexUserInterfaceExtension;

namespace Automatone.GUI
{
    /// <summary>
    /// This class handles the scrolling of the grid panel -- changing
    /// the section of the grid that the user is viewing and working on,
    /// and communicating with the layout manager to change which Cells should
    /// be drawn at the current time.
    /// </summary>
    public class NavigatorPanel : SkinNamedWindowControl
    {
        private Vector2 gridDrawOffset;

        public float GridDrawOffsetX
        {
            set
            {
                gridDrawOffset.X = value;
                horizontalSlider.ThumbPosition = (value - maxGridDrawOffset.X) / (minGridDrawOffset.X - maxGridDrawOffset.X);
            }
            get { return gridDrawOffset.X; }
        }

        public float GridDrawOffsetY
        {
            set
            {
                gridDrawOffset.Y = value;
                verticalSlider.ThumbPosition = (value - maxGridDrawOffset.Y) / (minGridDrawOffset.Y - maxGridDrawOffset.Y);
            }
            get { return gridDrawOffset.Y; }
        }

        private Point maxGridDrawOffset;
        private Point minGridDrawOffset;

        private float playOffset;
        public float PlayOffset { get { return playOffset; } }

        private KeyboardState oldKeyboardState;
        private KeyboardState newKeyboardState;

        private const int CURSOR_OFFSET = 200;
        private const float SCROLL_SPEED_DIVISOR = 14400f;

        private float gridMoveVelocityX = 0;
        private float gridMoveVelocityY = 0;

        private const float INIT_GRID_MOVE_SPEED = 1;
        private const float GRID_MOVE_ACCELERATION = .02F;
        private const float MAX_GRID_MOVE_SPEED = 10;
        private const float MAX_GRID_MOMENTUM = 5;

        private int verticalClippingStartIndex;
        private int verticalClippingEndIndex;
        private int horizontalClippingStartIndex;
        private int horizontalClippingEndIndex;

        public int VerticalClippingStartIndex { get { return verticalClippingStartIndex; } }
        public int VerticalClippingEndIndex { get { return verticalClippingEndIndex; } }
        public int HorizontalClippingStartIndex { get { return horizontalClippingStartIndex; } }
        public int HorizontalClippingEndIndex { get { return horizontalClippingEndIndex; } }

        private class ClampDelegateReturn { }
        private delegate ClampDelegateReturn ClampDelegate();

        public class RenewClippingDelegateReturn { }
        private delegate RenewClippingDelegateReturn RenewClippingDelegate();

        private OptionControl playPauseButton;
        private SkinNamedButtonControl stopButton;
        private SkinNamedHorizontalSliderControl horizontalSlider;
        private SkinNamedVerticalSliderControl verticalSlider;
        private SkinNamedLabelControl tempoLabel;
        private SkinNamedLabelControl timeSignatureNLabel;
        private SkinNamedLabelControl timeSignatureDLabel;

        public UniRectangle PlayPauseButtonBounds { set { playPauseButton.Bounds = value; } }
        public UniRectangle StopButtonBounds { set { stopButton.Bounds = value; } }
        public UniRectangle HorizontalScrollBarBounds { set { horizontalSlider.Bounds = value; } }
        public UniRectangle VerticalScrollBarBounds { set { verticalSlider.Bounds = value; } }
        public UniRectangle TempoLabelBounds { set { tempoLabel.Bounds = value; } }
        public UniRectangle TimeSignatureNLabelBounds { set { timeSignatureNLabel.Bounds = value; } }
        public UniRectangle TimeSignatureDLabelBounds { set { timeSignatureDLabel.Bounds = value; } }
        public float HorizontalScrollBarThumbSize { set { horizontalSlider.ThumbSize = value; } }
        public float VerticalScrollBarThumbSize { set { verticalSlider.ThumbSize = value; } }


        private NavigatorPanel() : base()
        {
            oldKeyboardState = Keyboard.GetState();
            playOffset = 0;
            InitializeComponent();
        }

        private static NavigatorPanel instance;
        public static NavigatorPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = new NavigatorPanel();
                return instance;
            }
        }

        private void InitializeComponent()
        {
            Bounds = LayoutManager.Instance.NavigatorPanelBounds;
            EnableDragging = false;
            SkinName = "navigator.panel";

            //Contruct Children
            horizontalSlider = new SkinNamedHorizontalSliderControl();
            verticalSlider = new SkinNamedVerticalSliderControl();
            playPauseButton = new OptionControl();
            stopButton = new SkinNamedButtonControl();
            tempoLabel = new SkinNamedLabelControl();
            timeSignatureNLabel = new SkinNamedLabelControl();
            timeSignatureDLabel = new SkinNamedLabelControl();

            //
            // playPauseButton
            //
            playPauseButton.Bounds = LayoutManager.Instance.PlayPauseButtonBounds;
            playPauseButton.Changed += new EventHandler(PlayPauseButtonToggled);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            stopButton.Bounds = LayoutManager.Instance.StopButtonBounds;
            stopButton.Pressed += new EventHandler(StopButtonPressed);
            stopButton.SkinName = "stop";

            //
            // horizontalSlider
            //
            horizontalSlider.Bounds = LayoutManager.Instance.HorizontalScrollBarBounds;
            horizontalSlider.ThumbPosition = LayoutManager.Instance.HorizontalSliderThumbSize;
            horizontalSlider.Moved += new EventHandler(HorizontalSliderMoved);
            horizontalSlider.SkinName = "navigator";

            //
            // verticalSlider
            //
            verticalSlider.Bounds = LayoutManager.Instance.VerticalScrollBarBounds;
            verticalSlider.ThumbPosition = LayoutManager.Instance.VerticalSliderThumbSize;
            verticalSlider.Moved += new EventHandler(VerticalSliderMoved);
            verticalSlider.SkinName = "navigator";

            //
            // tempoLabel
            //
            tempoLabel.Bounds = LayoutManager.Instance.TempoLabelBounds;
            tempoLabel.Text = "Tempo: " + InputParameters.Instance.Tempo.ToString();
            tempoLabel.SkinName = "settings";

            //
            // timeSignatureNLabel
            //
            timeSignatureNLabel.Bounds = LayoutManager.Instance.TimeSignatureNLabelBounds;
            timeSignatureNLabel.Text = "Time Signature: " + InputParameters.Instance.TimeSignatureN.ToString();
            timeSignatureNLabel.SkinName = "settings";

            //
            // timeSignatureDLabel
            //
            timeSignatureDLabel.Bounds = LayoutManager.Instance.TimeSignatureDLabelBounds;
            timeSignatureDLabel.Text = InputParameters.Instance.TimeSignatureD.ToString();
            timeSignatureDLabel.SkinName = "settings";

            //Add Children
            Children.Add(playPauseButton);
            Children.Add(stopButton);
            Children.Add(horizontalSlider);
            Children.Add(verticalSlider);
            Children.Add(tempoLabel);
            Children.Add(timeSignatureNLabel);
            Children.Add(timeSignatureDLabel);
        }

        public void StopSongPlaying()
        {
            Automatone.Instance.Sequencer.StopMidi();
            NavigatorPanel.Instance.ResetScrolling();
            playPauseButton.Selected = false;
        }

        public void PauseSongPlaying()
        {
            if (Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.PLAYING)
            {
                GridPanel.Instance.ScrollWithMidi = false;
                playPauseButton.Selected = false;
                PlayPauseButtonToggled(this, EventArgs.Empty);
            }
        }

        public void ResetPlayButton()
        {
            playPauseButton.Selected = false;
        }

        private void PlayPauseButtonToggled(object sender, EventArgs e)
        {
            if (GridPanel.Instance.SongCells != null)
            {
                if (playPauseButton.Selected)
                {
                    if (Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
                        Automatone.Instance.RewriteSong();
                    Automatone.Instance.Sequencer.PlayMidi();
                }
                else
                {
                    Automatone.Instance.Sequencer.PauseMidi();
                }

                GridPanel.Instance.ScrollWithMidi = playPauseButton.Selected;
            }
            else
            {
                playPauseButton.Selected = false;
            }

            ParametersPanel.Instance.SlideUp();
        }

        private void StopButtonPressed(object sender, EventArgs e)
        {
            StopSongPlaying();
        }

        private void HorizontalSliderMoved(object sender, EventArgs e)
        {
            gridMoveVelocityX = 0;
            GridDrawOffsetX = MathHelper.Lerp(maxGridDrawOffset.X, minGridDrawOffset.X, horizontalSlider.ThumbPosition);
        }

        private void VerticalSliderMoved(object sender, EventArgs e)
        {
            gridMoveVelocityY = 0;
            GridDrawOffsetY = MathHelper.Lerp(maxGridDrawOffset.Y, minGridDrawOffset.Y, verticalSlider.ThumbPosition);
        }

        public void CalculateGridOffsetBounds(int gridLengthX)
        {
            maxGridDrawOffset = LayoutManager.Instance.GridCellsArea.Location;
            minGridDrawOffset = new Point
            (
                Math.Min
                (
                    LayoutManager.Instance.GridCellsArea.Right - gridLengthX * LayoutManager.CELLWIDTH,
                    maxGridDrawOffset.X
                ),
                Math.Min
                (
                    LayoutManager.Instance.GridCellsArea.Bottom - Automatone.PIANO_SIZE * LayoutManager.CELLHEIGHT,
                    maxGridDrawOffset.Y
                )
            );
        }

        public void CalculateGridClipping()
        {
            if (GridPanel.Instance.SongCells != null)
            {
                CalculateHorizontalClipping();
                CalculateVerticalClipping();
            }
        }

        public RenewClippingDelegateReturn CalculateHorizontalClipping()
        {
            horizontalClippingStartIndex = Math.Max(GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridCellsArea.Left), 0);
            horizontalClippingEndIndex = Math.Min(GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridCellsArea.Right), GridPanel.Instance.CellsArrayLengthX - 1);
            return null;
        }

        public RenewClippingDelegateReturn CalculateVerticalClipping()
        {
            verticalClippingStartIndex = Math.Max(GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.Instance.GridCellsArea.Bottom), 0);
            verticalClippingEndIndex = Math.Min(GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.Instance.GridCellsArea.Top), Automatone.PIANO_SIZE - 1);
            return null;
        }

        public void ResetGridDrawOffset()
        {
            GridDrawOffsetX = maxGridDrawOffset.X;
            GridDrawOffsetY = maxGridDrawOffset.Y;
            gridMoveVelocityX = 0;
            gridMoveVelocityY = 0;
            CalculateGridClipping();
            horizontalSlider.ThumbPosition = 0;
            verticalSlider.ThumbPosition = 0;
        }

        public void ResetScrolling()
        {
            playOffset = 0;
            horizontalSlider.ThumbPosition = 0;
            GridPanel.Instance.ScrollWithMidi = false;
        }

        public void SetScrollLocation(int noteNumber)
        {
            playOffset = -1 * noteNumber * LayoutManager.CELLWIDTH;
        }

        public void Update(GameTime gameTime)
        {
            tempoLabel.Text = "Tempo: " + InputParameters.Instance.Tempo.ToString();
            timeSignatureNLabel.Text = "Time Signature: " + InputParameters.Instance.TimeSignatureN.ToString();
            timeSignatureDLabel.Text = InputParameters.Instance.TimeSignatureD.ToString();

            newKeyboardState = Keyboard.GetState();
            if (GridPanel.Instance.ScrollWithMidi)
            {
                HandlePlayScrolling();
            }
            else if (Automatone.Instance.IsActive)
            {
                HandleHorizontalScrolling();
            }

            if (Automatone.Instance.IsActive)
            {
                HandleVerticalScrolling();
            }
            oldKeyboardState = newKeyboardState;
        }

        private void HandlePlayScrolling()
        {
            gridMoveVelocityX = 0;
            playOffset -= InputParameters.Instance.Tempo * Automatone.SUBBEATS_PER_WHOLE_NOTE / SCROLL_SPEED_DIVISOR * LayoutManager.CELLWIDTH;
            GridDrawOffsetX = Math.Min(playOffset + CURSOR_OFFSET, 0);
            ClampGridOffsetX();
            CalculateHorizontalClipping();
        }

        private void HandleManualScrolling(ref float moveVal, ref float gridOffsetDimension, ClampDelegate clampDelegate, RenewClippingDelegate renewClippingDelegate, Keys positiveDirectionKey, Keys negativeDirectionKey)
        {
            if (Keyboard.GetState().IsKeyDown(negativeDirectionKey) && Keyboard.GetState().IsKeyDown(positiveDirectionKey))
            {
                moveVal = 0;
            }
            else if (Keyboard.GetState().IsKeyDown(negativeDirectionKey))
            {
                moveVal = MathHelper.Clamp(moveVal + GRID_MOVE_ACCELERATION, 0, MAX_GRID_MOVE_SPEED);
            }
            else if (Keyboard.GetState().IsKeyDown(positiveDirectionKey))
            {
                moveVal = MathHelper.Clamp(moveVal - GRID_MOVE_ACCELERATION, -MAX_GRID_MOVE_SPEED, 0);
            }
            else if (moveVal > 0)
            {
                moveVal = MathHelper.Clamp(moveVal - GRID_MOVE_ACCELERATION, 0, MAX_GRID_MOMENTUM);
            }
            else if (moveVal < 0)
            {
                moveVal = MathHelper.Clamp(moveVal + GRID_MOVE_ACCELERATION, -MAX_GRID_MOMENTUM, 0);
            }

            gridOffsetDimension += moveVal;
            renewClippingDelegate();
            clampDelegate();
        }

        private void HandleHorizontalScrolling()
        {
            HandleManualScrolling(ref gridMoveVelocityX, ref gridDrawOffset.X, ClampGridOffsetX, CalculateHorizontalClipping, Keys.Right, Keys.Left);
            if (newKeyboardState.IsKeyDown(Keys.Home) && oldKeyboardState.IsKeyUp(Keys.Home))
            {
                gridMoveVelocityX = 0;
                GridDrawOffsetX = maxGridDrawOffset.X;
            }
            if (newKeyboardState.IsKeyDown(Keys.End) && oldKeyboardState.IsKeyUp(Keys.End))
            {
                gridMoveVelocityX = 0;
                GridDrawOffsetX = minGridDrawOffset.X;
            }
        }

        private void HandleVerticalScrolling()
        {
            HandleManualScrolling(ref gridMoveVelocityY, ref gridDrawOffset.Y, ClampGridOffsetY, CalculateVerticalClipping, Keys.Down, Keys.Up);
        }

        private ClampDelegateReturn ClampGridOffsetX()
        {
            GridDrawOffsetX = MathHelper.Clamp(GridDrawOffsetX, minGridDrawOffset.X, maxGridDrawOffset.X);
            return null;
        }

        private ClampDelegateReturn ClampGridOffsetY()
        {
            GridDrawOffsetY = MathHelper.Clamp(GridDrawOffsetY, minGridDrawOffset.Y, maxGridDrawOffset.Y);
            return null;
        }
    }
}
