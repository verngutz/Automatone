﻿using System;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.Graphics.SpecialEffects.Particles;
using NuclexUserInterfaceExtension;

using Automatone.Music;

namespace Automatone.GUI
{
    public class GridPanel : DrawableGameComponent
    {
        public bool ScrollWithMidi { set; get; }

        private MouseState oldMouseState;
        private MouseState newMouseState;

        private const byte DimensionX = 1;
        private const byte DimensionY = 0;

        private CellState[,] songCells;
        public CellState[,] SongCells
        {
            set
            {
                if (value.GetLength(DimensionY) != Automatone.PIANO_SIZE)
                    throw new ArgumentOutOfRangeException("Incorrect value for dimension: " + DimensionY);
                songCells = value;
                cells.CalculateBounds();
                labels.CalculateBounds();
                navigators.CalculateGridOffsetBounds();
                navigators.RenewHorizontalClipping();
            }
            get
            {
                return songCells;
            }
        }

        private Cells cells;
        private Labels labels;
        private Navigators navigators;

        private static GridPanel instance;
        public static GridPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = new GridPanel();
                return instance;
            }
        }

        private GridPanel()
            : base(Automatone.Instance)
        {
            oldMouseState = Mouse.GetState();
            cells = new Cells();
            labels = new Labels();
            navigators = new Navigators();
            Automatone.Instance.Gui.Screen.Desktop.Children.Add(navigators);
            Automatone.Instance.Window.ClientSizeChanged += delegate { RespondToWindowResize(); };
        }

        public void ResetGridView()
        {
            navigators.ResetGridDrawOffset();
        }

        public void ResetScrolling()
        {
            navigators.ResetScrolling();
        }

        private void RespondToWindowResize()
        {
            if (SongCells != null)
            {
                labels.CalculateBounds();
                cells.CalculateBounds();
                navigators.CalculateGridOffsetBounds();
                navigators.CalculateClippingBounds();
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            cells.LoadContent();
            labels.LoadContent();
        }

        protected override void UnloadContent()
        {
            cells.Dispose();
            labels.Dispose();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            newMouseState = Mouse.GetState();

            if (SongCells != null)
            {
                cells.Update(gameTime);
                navigators.Update(gameTime);
            }

            oldMouseState = newMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (SongCells != null)
            {
                cells.Draw(gameTime);
                labels.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        private Color GetChromaticColor(int pitch)
        {
            int chromaticIndex = pitch % MusicTheory.OCTAVE_SIZE;
            switch (chromaticIndex)
            {
                case 0:
                    return Color.MediumVioletRed;
                case 1:
                    return Color.Red;
                case 2:
                    return Color.OrangeRed;
                case 3:
                    return Color.Orange;
                case 4:
                    return Color.Yellow;
                case 5:
                    return Color.GreenYellow;
                case 6:
                    return Color.Green;
                case 7:
                    return Color.SeaGreen;
                case 8:
                    return Color.Blue;
                case 9:
                    return Color.Indigo;
                case 10:
                    return Color.Violet;
                case 11:
                    return Color.Silver;
            }
            return Color.White;
        }

        public int ScreenToGridCoordinatesX(int x)
        {
            return ((int)-navigators.GridDrawOffsetX + x) / LayoutManager.CELLWIDTH;
        }

        public int ScreenToGridCoordinatesY(int y)
        {
            return SongCells.GetLength(DimensionY) - 1 - ((int)-navigators.GridDrawOffsetY + y) / LayoutManager.CELLHEIGHT;
        }

        /// <summary>
        /// This class handles the updating and drawing of the cells 
        /// only (the visual representation of the current song project 
        /// as a 2D array, excluding the labels, cursors, and navigators)
        /// in the grid panel.
        /// </summary>
        private class Cells
        {
            private static Texture2D silentCell;
            private static Texture2D startCell;
            private static Texture2D holdCell;

            private Rectangle Bounds { set; get; }
            private int? gridInputStartXIndex;
            private int? gridInputEndXIndex;
            private int? gridInputYIndex;

            public void CalculateBounds()
            {
                Bounds = LayoutManager.GetCellsClickableArea(GridPanel.Instance.SongCells.GetLength(DimensionX), GridPanel.Instance.SongCells.GetLength(DimensionY));
            }

            public void LoadContent()
            {
                silentCell = Automatone.Instance.Content.Load<Texture2D>("darkbox");
                startCell = Automatone.Instance.Content.Load<Texture2D>("lightbox");
                holdCell = Automatone.Instance.Content.Load<Texture2D>("holdbox");
            }

            public void Dispose()
            {
                if (silentCell != null) silentCell.Dispose();
                if (startCell != null) startCell.Dispose();
                if (holdCell != null) holdCell.Dispose();
            }

            public void Update(GameTime gameTime)
            {
                if (Automatone.Instance.IsActive
                    && Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED
                    && (GridPanel.Instance.newMouseState.LeftButton != ButtonState.Released 
                        || GridPanel.Instance.newMouseState.LeftButton != ButtonState.Released)
                    && Bounds.Contains(new Point(GridPanel.Instance.newMouseState.X, GridPanel.Instance.newMouseState.Y)))
                {
                    if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Released)
                    {
                        gridInputStartXIndex = Math.Min(GridPanel.Instance.SongCells.GetLength(DimensionX) - 1, GridPanel.Instance.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputEndXIndex = Math.Min(GridPanel.Instance.SongCells.GetLength(DimensionX) - 1, GridPanel.Instance.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputYIndex = GridPanel.Instance.ScreenToGridCoordinatesY(Mouse.GetState().Y);
                        if (GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] == CellState.SILENT)
                        {
                            GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.START;
                        }
                        else
                        {
                            GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.SILENT;
                        }
                    }
                    else if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Released && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        gridInputStartXIndex = null;
                        gridInputEndXIndex = null;
                        gridInputYIndex = null;
                    }
                    else if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (gridInputStartXIndex.HasValue)
                        {
                            gridInputEndXIndex = Math.Min(GridPanel.Instance.SongCells.GetLength(DimensionX) - 1, GridPanel.Instance.ScreenToGridCoordinatesX(Mouse.GetState().X));
                            int k = gridInputStartXIndex.Value + 1;
                            while (k < GridPanel.Instance.SongCells.GetLength(DimensionX) && GridPanel.Instance.SongCells[gridInputYIndex.Value, k] == CellState.HOLD)
                            {
                                GridPanel.Instance.SongCells[gridInputYIndex.Value, k] = CellState.SILENT;
                                k++;
                            }
                            for (int i = gridInputStartXIndex.Value + 1; i <= gridInputEndXIndex; i++)
                            {
                                if (GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] != CellState.SILENT)
                                {
                                    if (GridPanel.Instance.SongCells[gridInputYIndex.Value, i] == CellState.SILENT)
                                    {
                                        GridPanel.Instance.SongCells[gridInputYIndex.Value, i] = CellState.HOLD;
                                    }
                                    else break;
                                }
                            }
                        }
                    }
                }
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                for (int i = GridPanel.Instance.navigators.VerticalClippingStartIndex; i <= GridPanel.Instance.navigators.VerticalClippingEndIndex; i++)
                {
                    for (int j = GridPanel.Instance.navigators.HorizontalClippingStartIndex; j <= GridPanel.Instance.navigators.HorizontalClippingEndIndex; j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(j * LayoutManager.CELLWIDTH + GridPanel.Instance.navigators.GridDrawOffsetX), (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + GridPanel.Instance.navigators.GridDrawOffsetY), LayoutManager.CELLWIDTH, LayoutManager.CELLHEIGHT);
                        Color drawColor = GridPanel.Instance.GetChromaticColor(i);
                        if (j * LayoutManager.CELLWIDTH < -GridPanel.Instance.navigators.PlayOffset - LayoutManager.CELLWIDTH)
                        {
                            Automatone.Instance.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 32));
                        }
                        else if (j * LayoutManager.CELLWIDTH < -GridPanel.Instance.navigators.PlayOffset)
                        {
                            Automatone.Instance.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 255));
                        }
                        else
                        {
                            Automatone.Instance.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, GridPanel.Instance.SongCells[i, j] == CellState.SILENT ? (Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED ? 128 : 64) : 255));
                        }
                    }
                }
                Automatone.Instance.SpriteBatch.End();
            }

            private Texture2D GetCellTexture(int i, int j)
            {
                switch (GridPanel.Instance.SongCells[i, j])
                {
                    case CellState.SILENT:
                        return silentCell;
                    case CellState.START:
                        return startCell;
                    case CellState.HOLD:
                        return holdCell;
                }
                return null;
            }
        }
        
        /// <summary>
        /// This class handles the updating and drawing of the
        /// pitch ("C3", "C#3", "D3", etc.) and time (measure number and beats)
        /// labels of the grid, which automatically moves along with the
        /// navigators to allow the user to know which section of the grid
        /// he/she is viewing and working on.
        /// </summary>
        private class Labels
        {
            private Texture2D labelsBackground;
            private SpriteFont labelFont;

            private Rectangle leftBorderBounds;
            private Rectangle topBorderBounds;
            private Rectangle rightBorderBounds;
            private Rectangle bottomBorderBounds;

            public void CalculateBounds()
            {
                leftBorderBounds = LayoutManager.GridPanelBounds.LeftRectangle;
                topBorderBounds = LayoutManager.GridPanelBounds.TopRectangle;
                rightBorderBounds = LayoutManager.GridPanelBounds.RightRectangle;
                bottomBorderBounds = LayoutManager.GridPanelBounds.BottomRectangle;
            }

            public void LoadContent()
            {
                labelsBackground = Automatone.Instance.Content.Load<Texture2D>("BlackPixel");
                labelFont = Automatone.Instance.Content.Load<SpriteFont>("LabelFont");
            }

            public void Dispose()
            {
                if (labelsBackground != null) 
                    labelsBackground.Dispose();
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin();
                DrawPitchLabel();
                DrawTimeLabel();
                DrawRightBorder();
                DrawBottomBorder();
                Automatone.Instance.SpriteBatch.End();
            }

            private void DrawPitchLabel()
            {
                bool sharpLabels = true;

                Automatone.Instance.SpriteBatch.Draw(labelsBackground, leftBorderBounds, Color.White);

                for (int i = GridPanel.Instance.navigators.VerticalClippingStartIndex; i <= GridPanel.Instance.navigators.VerticalClippingEndIndex; i++)
                {
                    Vector2 loc = new Vector2(2, (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + GridPanel.Instance.navigators.GridDrawOffsetY));
                    string letter = "";
                    switch ((i - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER + MusicTheory.OCTAVE_SIZE) % MusicTheory.OCTAVE_SIZE)
                    {
                        case 0:
                            letter = "C";
                            break;
                        case 1:
                            letter = (sharpLabels ? "C♯" : "D♭");
                            break;
                        case 2:
                            letter = "D";
                            break;
                        case 3:
                            letter = (sharpLabels ? "D♯" : "E♭");
                            break;
                        case 4:
                            letter = "E";
                            break;
                        case 5:
                            letter = "F";
                            break;
                        case 6:
                            letter = (sharpLabels ? "F♯" : "G♭");
                            break;
                        case 7:
                            letter = "G";
                            break;
                        case 8:
                            letter = (sharpLabels ? "G♯" : "A♭");
                            break;
                        case 9:
                            letter = "A";
                            break;
                        case 10:
                            letter = (sharpLabels ? "A♯" : "B♭");
                            break;
                        case 11:
                            letter = "B";
                            break;
                    }
                    Automatone.Instance.SpriteBatch.DrawString(labelFont, letter, loc, Color.White);
                }
            }

            private void DrawTimeLabel()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, topBorderBounds, Color.White);

                for (int j = GridPanel.Instance.navigators.HorizontalClippingStartIndex; j <= GridPanel.Instance.navigators.HorizontalClippingEndIndex; j++)
                {
                    Vector2 loc = new Vector2((int)(j * LayoutManager.CELLWIDTH + GridPanel.Instance.navigators.GridDrawOffsetX), 2 + LayoutManager.CONTROLS_AND_GRID_DIVISION);
                    if (j % Automatone.Instance.MeasureLength == 0)
                    {
                        Automatone.Instance.SpriteBatch.DrawString(labelFont, "" + (j / Automatone.Instance.MeasureLength + 1), loc, Color.White);
                    }
                    for (int k = 1; k < Automatone.Instance.MeasureLength / (Automatone.SUBBEATS_PER_WHOLE_NOTE / 4); k++)
                    {
                        if (j % Automatone.Instance.MeasureLength == k * Automatone.SUBBEATS_PER_WHOLE_NOTE / 4)
                        {
                            Automatone.Instance.SpriteBatch.DrawString(labelFont, "♩", loc, Color.Navy);
                        }
                    }
                }
            }

            private void DrawRightBorder()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, rightBorderBounds, Color.White);
            }

            private void DrawBottomBorder()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, bottomBorderBounds, Color.White);
            }
        }

        /// <summary>
        /// This class handles the updating and drawing of the particle
        /// system (used to create cool visual effects while notes are played).
        /// </summary>
        private class Particles
        {

            private ParticleSystem<NoteHitParticle> particleSystem;
            private const int PARTICLE_SPAWN_DENSITY = 4;
            private const int PARTICLE_SPAWN_CYCLE = 50;
            private int currentCycle;

            public Particles()
            {
                particleSystem = new ParticleSystem<NoteHitParticle>(8192);
                particleSystem.Affectors.Add(new MovementAffector<NoteHitParticle>(new NoteHitParticleModifier()));
                currentCycle = 0;
            }

            private void Update()
            {
                if (currentCycle++ > PARTICLE_SPAWN_CYCLE)
                {
                    int j = GridPanel.Instance.ScreenToGridCoordinatesX((int)-GridPanel.Instance.navigators.PlayOffset);

                    for (int i = GridPanel.Instance.navigators.VerticalClippingStartIndex; i < GridPanel.Instance.navigators.VerticalClippingEndIndex; i++)
                    {
                        if (GridPanel.Instance.SongCells[i, j] != CellState.SILENT)
                        {
                            Vector2 particleSpawnPoint = new Vector2((int)(j * LayoutManager.CELLWIDTH + GridPanel.Instance.navigators.GridDrawOffsetX), (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + GridPanel.Instance.navigators.GridDrawOffsetY));
                            for (float k = 0; k < PARTICLE_SPAWN_DENSITY; k++)
                            {
                                if (particleSystem.Particles.Count < particleSystem.Capacity)
                                    particleSystem.AddParticle(new NoteHitParticle(new Vector3(particleSpawnPoint + new Vector2(0, k / PARTICLE_SPAWN_DENSITY * LayoutManager.CELLHEIGHT), 0), new Vector3(1, 0, 0), GridPanel.Instance.GetChromaticColor(i)));
                            }
                        }
                    }
                    currentCycle = 0;
                }

                IAsyncResult asyncResult = particleSystem.BeginUpdate(1, 4, null, null);
                particleSystem.EndUpdate(asyncResult);
                particleSystem.Prune(NoteHitParticle.IsAlive);
            }

            private void Draw()
            {
                foreach (NoteHitParticle particle in particleSystem.Particles.Array)
                {
                    //Automatone.Instance.SpriteBatch.Draw(startCell, new Rectangle((int)particle.Position.X, (int)particle.Position.Y, 2, 2), particle.Color);
                }
            }
        }

        /// <summary>
        /// This class handles the scrolling of the grid panel -- changing
        /// the section of the grid that the user is viewing and working on,
        /// and communicating with the layout manager to change which Cells should
        /// be drawn at the current time.
        /// </summary>
        private class Navigators : WindowControl
        {
            private Vector2 gridDrawOffset;
            public float GridDrawOffsetX { get { return gridDrawOffset.X; } }
            public float GridDrawOffsetY { get { return gridDrawOffset.Y; } }

            private Point maxGridDrawOffset;
            private Point minGridDrawOffset;

            private float playOffset;
            public float PlayOffset { get { return playOffset; } }

            private KeyboardState oldKeyboardState;
            private KeyboardState newKeyboardState;

            private const int CURSOR_OFFSET = 100;
            private const float SCROLL_SPEED_DIVISOR = 14400f;

            private float moveValX = 0;
            private float moveValY = 0;

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

            private SkinNamedHorizontalSliderControl horizonalSlider;

            public Navigators()
                : base()
            {
                oldKeyboardState = Keyboard.GetState();
                playOffset = 0;
                InitializeComponent();
            }

            private void InitializeComponent()
            {
            }

            public void CalculateGridOffsetBounds()
            {
                maxGridDrawOffset = LayoutManager.GridPanelBounds.InnerRectangle.Location;
                minGridDrawOffset = new Point
                (
                    Math.Min
                    (
                        LayoutManager.GridPanelBounds.InnerRectangle.Right - GridPanel.Instance.SongCells.GetLength(DimensionX) * LayoutManager.CELLWIDTH,
                        maxGridDrawOffset.X
                    ),
                    Math.Min
                    (
                        LayoutManager.GridPanelBounds.InnerRectangle.Bottom - GridPanel.Instance.SongCells.GetLength(DimensionY) * LayoutManager.CELLHEIGHT,
                        maxGridDrawOffset.Y
                    )
                );
            }

            public void CalculateClippingBounds()
            {
                RenewHorizontalClipping();
                RenewVerticalClipping();
            }

            public RenewClippingDelegateReturn RenewHorizontalClipping()
            {
                horizontalClippingStartIndex = Math.Max(GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.GridPanelBounds.InnerRectangle.Left), 0);
                horizontalClippingEndIndex = Math.Min(GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.GridPanelBounds.InnerRectangle.Right), GridPanel.Instance.SongCells.GetLength(DimensionX) - 1);
                return null;
            }

            public RenewClippingDelegateReturn RenewVerticalClipping()
            {
                verticalClippingStartIndex = Math.Max(GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.GridPanelBounds.InnerRectangle.Bottom), 0);
                verticalClippingEndIndex = Math.Min(GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.GridPanelBounds.InnerRectangle.Top), GridPanel.Instance.SongCells.GetLength(DimensionY) - 1);
                return null;
            }

            public void ResetGridDrawOffset()
            {
                gridDrawOffset = new Vector2(maxGridDrawOffset.X, maxGridDrawOffset.Y);
                moveValX = 0;
                moveValY = 0;
                CalculateClippingBounds();
            }

            public void ResetScrolling()
            {
                playOffset = 0;
                GridPanel.Instance.ScrollWithMidi = false;
            }

            public void SetScrollLocation(int noteNumber)
            {
                playOffset = -1 * noteNumber * LayoutManager.CELLWIDTH;
            }

            public void Update(GameTime gameTime)
            {
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
                playOffset -= InputParameters.Instance.tempo * Automatone.SUBBEATS_PER_WHOLE_NOTE / SCROLL_SPEED_DIVISOR * LayoutManager.CELLWIDTH;
                gridDrawOffset.X = Math.Min(playOffset + CURSOR_OFFSET, 0);
                ClampGridOffsetX();
                RenewHorizontalClipping();
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
                else if(moveVal > 0)
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
                HandleManualScrolling(ref moveValX, ref gridDrawOffset.X, ClampGridOffsetX, RenewHorizontalClipping, Keys.Right, Keys.Left);
                if (newKeyboardState.IsKeyDown(Keys.Home) && oldKeyboardState.IsKeyUp(Keys.Home))
                {
                    gridDrawOffset.X = minGridDrawOffset.X;
                }
                if (newKeyboardState.IsKeyDown(Keys.End) && oldKeyboardState.IsKeyUp(Keys.End))
                {
                    gridDrawOffset.X = maxGridDrawOffset.X;
                }
            }

            private void HandleVerticalScrolling()
            {
                HandleManualScrolling(ref moveValY, ref gridDrawOffset.Y, ClampGridOffsetY, RenewVerticalClipping, Keys.Down, Keys.Up);
            }

            private ClampDelegateReturn ClampGridOffsetX()
            {
                gridDrawOffset.X = MathHelper.Clamp(gridDrawOffset.X, minGridDrawOffset.X, maxGridDrawOffset.X);
                return null;
            }

            private ClampDelegateReturn ClampGridOffsetY()
            {
                gridDrawOffset.Y = MathHelper.Clamp(gridDrawOffset.Y, minGridDrawOffset.Y, maxGridDrawOffset.Y);
                return null;
            }
        }

        /// <summary>
        /// This class handles the cursors which are used to select
        /// sections of the grid for editing.
        /// </summary>
        private class Cursors
        {
        }
    }
}