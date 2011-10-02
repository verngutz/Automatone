using System;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.Graphics.SpecialEffects.Particles;

using Automatone.Music;

namespace Automatone.GUI
{
    public class GridPanel : DrawableGameComponent
    {
        private Automatone parent;

        public bool ScrollWithMidi { set; get; }

        private MouseState oldMouseState;
        private MouseState newMouseState;

        private CellState[,] songCells;
        public CellState[,] SongCells
        {
            set
            {
                songCells = value;
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
                    instance = new GridPanel(Automatone.Instance);
                return instance;
            }
        }

        private GridPanel(Automatone parent)
            : base(parent)
        {
            this.parent = parent;
            oldMouseState = Mouse.GetState();
            cells = new Cells(this);
            labels = new Labels(this);
            navigators = new Navigators(this);
            parent.Gui.Screen.Desktop.Children.Add(navigators);
            parent.Window.ClientSizeChanged += delegate { RespondToWindowResize(); };
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
            labels.RefreshBoundaries();
            cells.RefreshBoundaries();
            navigators.RenewHorizontalClipping();
            navigators.RenewVerticalClipping();
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

        public int ScreenToGridCoordinatesX(int y)
        {
            return ((int)-navigators.GridDrawOffsetX + y) / Cells.CELLWIDTH;
        }

        public int ScreenToGridCoordinatesY(int x)
        {
            return SongCells.GetLength(0) - 1 - ((int)-navigators.GridDrawOffsetY + x) / Cells.CELLHEIGHT;
        }

        /// <summary>
        /// This class handles the updating and drawing of the cells 
        /// only (the visual representation of the current song project 
        /// as a 2D array, excluding the labels, cursors, and navigators)
        /// in the grid panel.
        /// </summary>
        private class Cells
        {
            private GridPanel parent;

            public const byte CELLHEIGHT = 20;
            public const byte CELLWIDTH = 20;
            private static Texture2D silentCell;
            private static Texture2D startCell;
            private static Texture2D holdCell;

            private Rectangle clickableArea;
            private int? gridInputStartXIndex;
            private int? gridInputEndXIndex;
            private int? gridInputYIndex;

            public Cells(GridPanel parent)
            {
                this.parent = parent;
                RefreshBoundaries();
            }

            public void RefreshBoundaries()
            {
                clickableArea = new Rectangle
                (
                    Labels.PITCH_LABEL_THICKNESS,
                    Automatone.CONTROLS_AND_GRID_DIVISION + Labels.TIME_LABEL_THICKNESS,
                    parent.parent.Window.ClientBounds.Width - Labels.PITCH_LABEL_THICKNESS,
                    parent.parent.Window.ClientBounds.Height - Automatone.CONTROLS_AND_GRID_DIVISION - Labels.TIME_LABEL_THICKNESS
                );
            }

            public void LoadContent()
            {
                silentCell = parent.parent.Content.Load<Texture2D>("darkbox");
                startCell = parent.parent.Content.Load<Texture2D>("lightbox");
                holdCell = parent.parent.Content.Load<Texture2D>("holdbox");
            }

            public void Dispose()
            {
                if (silentCell != null) silentCell.Dispose();
                if (startCell != null) startCell.Dispose();
                if (holdCell != null) holdCell.Dispose();
            }

            public void Update(GameTime gameTime)
            {
                if (parent.parent.IsActive
                    && parent.parent.Sequencer.State == Sequencer.MidiPlayerState.STOPPED
                    && (parent.newMouseState.LeftButton != ButtonState.Released 
                        || parent.newMouseState.LeftButton != ButtonState.Released)
                    && clickableArea.Contains(new Point(parent.newMouseState.X, parent.newMouseState.Y)))
                {
                    if (parent.newMouseState.LeftButton == ButtonState.Pressed && parent.oldMouseState.LeftButton == ButtonState.Released)
                    {
                        gridInputStartXIndex = Math.Min(parent.SongCells.GetLength(1) - 1, parent.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputEndXIndex = Math.Min(parent.SongCells.GetLength(1) - 1, parent.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputYIndex = parent.ScreenToGridCoordinatesY(Mouse.GetState().Y);
                        if (parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] == CellState.SILENT)
                        {
                            parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.START;
                        }
                        else
                        {
                            parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.SILENT;
                        }
                    }
                    else if (parent.newMouseState.LeftButton == ButtonState.Released && parent.oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        gridInputStartXIndex = null;
                        gridInputEndXIndex = null;
                        gridInputYIndex = null;
                    }
                    else if (parent.newMouseState.LeftButton == ButtonState.Pressed && parent.oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (gridInputStartXIndex.HasValue)
                        {
                            gridInputEndXIndex = Math.Min(parent.SongCells.GetLength(1) - 1, parent.ScreenToGridCoordinatesX(Mouse.GetState().X));
                            int k = gridInputStartXIndex.Value + 1;
                            while (k < parent.SongCells.GetLength(1) && parent.SongCells[gridInputYIndex.Value, k] == CellState.HOLD)
                            {
                                parent.SongCells[gridInputYIndex.Value, k] = CellState.SILENT;
                                k++;
                            }
                            for (int i = gridInputStartXIndex.Value + 1; i <= gridInputEndXIndex; i++)
                            {
                                if (parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] != CellState.SILENT)
                                {
                                    if (parent.SongCells[gridInputYIndex.Value, i] == CellState.SILENT)
                                    {
                                        parent.SongCells[gridInputYIndex.Value, i] = CellState.HOLD;
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
                parent.parent.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                for (int i = parent.navigators.VerticalClippingStartIndex; i <= parent.navigators.VerticalClippingEndIndex; i++)
                {
                    for (int j = parent.navigators.HorizontalClippingStartIndex; j <= parent.navigators.HorizontalClippingEndIndex; j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(j * CELLWIDTH + parent.navigators.GridDrawOffsetX), (int)((parent.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT + parent.navigators.GridDrawOffsetY), CELLWIDTH, CELLHEIGHT);
                        Color drawColor = parent.GetChromaticColor(i);
                        if (j * CELLWIDTH < -parent.navigators.PlayOffset - CELLWIDTH)
                        {
                            parent.parent.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 32));
                        }
                        else if (j * CELLWIDTH < -parent.navigators.PlayOffset)
                        {
                            parent.parent.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 255));
                        }
                        else
                        {
                            parent.parent.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, parent.SongCells[i, j] == CellState.SILENT ? (parent.parent.Sequencer.State == Sequencer.MidiPlayerState.STOPPED ? 128 : 64) : 255));
                        }
                    }
                }
                parent.parent.SpriteBatch.End();
            }

            private Texture2D GetCellTexture(int i, int j)
            {
                switch (parent.SongCells[i, j])
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
            private GridPanel parent;

            public const int TIME_LABEL_THICKNESS = 25;
            public const int PITCH_LABEL_THICKNESS = 25;
            private Texture2D pitchTimeLabelBackground;
            private SpriteFont labelFont;

            private Rectangle pitchLabelRectangle;
            private Rectangle timeLabelRectangle;

            public Labels(GridPanel parent)
            {
                this.parent = parent;
                RefreshBoundaries();
            }

            public void RefreshBoundaries()
            {
                pitchLabelRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, PITCH_LABEL_THICKNESS, parent.parent.Window.ClientBounds.Height - Automatone.CONTROLS_AND_GRID_DIVISION);
                timeLabelRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, parent.parent.Window.ClientBounds.Width, TIME_LABEL_THICKNESS);
            }

            public void LoadContent()
            {
                pitchTimeLabelBackground = parent.parent.Content.Load<Texture2D>("BlackPixel");
                labelFont = parent.parent.Content.Load<SpriteFont>("LabelFont");
            }

            public void Dispose()
            {
                if (pitchTimeLabelBackground != null) 
                    pitchTimeLabelBackground.Dispose();
            }

            public void Draw(GameTime gameTime)
            {
                parent.parent.SpriteBatch.Begin();
                DrawPitchLabel();
                DrawTimeLabel();
                parent.parent.SpriteBatch.End();
            }

            private void DrawPitchLabel()
            {
                bool sharpLabels = true;

                parent.parent.SpriteBatch.Draw(pitchTimeLabelBackground, pitchLabelRectangle, Color.White);

                for (int i = parent.navigators.VerticalClippingStartIndex; i <= parent.navigators.VerticalClippingEndIndex; i++)
                {
                    Vector2 loc = new Vector2(2, (int)((parent.SongCells.GetLength(0) - 1 - i) * Cells.CELLHEIGHT + parent.navigators.GridDrawOffsetY));
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
                    parent.parent.SpriteBatch.DrawString(labelFont, letter, loc, Color.White);
                }
            }

            private void DrawTimeLabel()
            {
                parent.parent.SpriteBatch.Draw(pitchTimeLabelBackground, timeLabelRectangle, Color.White);

                for (int j = parent.navigators.HorizontalClippingStartIndex; j <= parent.navigators.HorizontalClippingEndIndex; j++)
                {
                    Vector2 loc = new Vector2((int)(j * Cells.CELLWIDTH + parent.navigators.GridDrawOffsetX), 2 + Automatone.CONTROLS_AND_GRID_DIVISION);
                    if (j % parent.parent.MeasureLength == 0)
                    {
                        parent.parent.SpriteBatch.DrawString(labelFont, "" + (j / parent.parent.MeasureLength + 1), loc, Color.White);
                    }
                    for (int k = 1; k < parent.parent.MeasureLength / (Automatone.SUBBEATS_PER_WHOLE_NOTE / 4); k++)
                    {
                        if (j % parent.parent.MeasureLength == k * Automatone.SUBBEATS_PER_WHOLE_NOTE / 4)
                        {
                            parent.parent.SpriteBatch.DrawString(labelFont, "♩", loc, Color.Navy);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This class handles the updating and drawing of the particle
        /// system (used to create cool visual effects while notes are played).
        /// </summary>
        private class Particles
        {
            private GridPanel parent;

            private ParticleSystem<NoteHitParticle> particleSystem;
            private const int PARTICLE_SPAWN_DENSITY = 4;
            private const int PARTICLE_SPAWN_CYCLE = 50;
            private int currentCycle;

            public Particles(GridPanel parent)
            {
                this.parent = parent;

                particleSystem = new ParticleSystem<NoteHitParticle>(8192);
                particleSystem.Affectors.Add(new MovementAffector<NoteHitParticle>(new NoteHitParticleModifier()));
                currentCycle = 0;
            }

            private void Update()
            {
                if (currentCycle++ > PARTICLE_SPAWN_CYCLE)
                {
                    int j = parent.ScreenToGridCoordinatesX((int)-parent.navigators.PlayOffset);

                    for (int i = parent.navigators.VerticalClippingStartIndex; i < parent.navigators.VerticalClippingEndIndex; i++)
                    {
                        if (parent.SongCells[i, j] != CellState.SILENT)
                        {
                            Vector2 particleSpawnPoint = new Vector2((int)(j * Cells.CELLWIDTH + parent.navigators.GridDrawOffsetX), (int)((parent.SongCells.GetLength(0) - 1 - i) * Cells.CELLHEIGHT + parent.navigators.GridDrawOffsetY));
                            for (float k = 0; k < PARTICLE_SPAWN_DENSITY; k++)
                            {
                                if (particleSystem.Particles.Count < particleSystem.Capacity)
                                    particleSystem.AddParticle(new NoteHitParticle(new Vector3(particleSpawnPoint + new Vector2(0, k / PARTICLE_SPAWN_DENSITY * Cells.CELLHEIGHT), 0), new Vector3(1, 0, 0), parent.GetChromaticColor(i)));
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
                    //parent.parent.SpriteBatch.Draw(startCell, new Rectangle((int)particle.Position.X, (int)particle.Position.Y, 2, 2), particle.Color);
                }
            }
        }

        /// <summary>
        /// This class handles the scrolling of the grid panel -- changing
        /// the section of the grid that the user is viewing and working on.
        /// the movement 
        /// </summary>
        private class Navigators : WindowControl
        {
            private GridPanel parent;

            private Vector2 gridDrawOffset;
            public float GridDrawOffsetX { get { return gridDrawOffset.X; } }
            public float GridDrawOffsetY { get { return gridDrawOffset.Y; } }

            private float playOffset;
            public float PlayOffset { get { return playOffset; } }

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

            private delegate void VoidDelegate();

            public Navigators(GridPanel parent)
                : base()
            {
                this.parent = parent;
                ResetScrolling();
                InitializeComponent();
            }

            private void InitializeComponent()
            {
            }

            public void RenewHorizontalClipping()
            {
                horizontalClippingStartIndex = parent.ScreenToGridCoordinatesX(Labels.PITCH_LABEL_THICKNESS);
                horizontalClippingEndIndex = Math.Min(parent.ScreenToGridCoordinatesX(parent.parent.Window.ClientBounds.Width), parent.SongCells.GetLength(1) - 1);
            }

            public void RenewVerticalClipping()
            {
                verticalClippingStartIndex = Math.Max(0, parent.ScreenToGridCoordinatesY(parent.parent.Window.ClientBounds.Height));
                verticalClippingEndIndex = parent.ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION + Labels.TIME_LABEL_THICKNESS);
            }

            public void ResetGridDrawOffset()
            {
                gridDrawOffset = new Vector2(Labels.PITCH_LABEL_THICKNESS, Automatone.CONTROLS_AND_GRID_DIVISION + Labels.TIME_LABEL_THICKNESS);
                RenewHorizontalClipping();
                RenewVerticalClipping();
            }

            public void ResetScrolling()
            {
                playOffset = 0;
                parent.ScrollWithMidi = false;
            }

            public void SetScrollLocation(int noteNumber)
            {
                playOffset = -1 * noteNumber * Cells.CELLWIDTH;
            }

            public void Update(GameTime gameTime)
            {
                if (parent.ScrollWithMidi)
                {
                    HandlePlayScrolling();
                }
                else if (parent.parent.IsActive)
                {
                    HandleHorizontalScrolling();
                }

                if (parent.parent.IsActive)
                {
                    HandleVerticalScrolling();
                }
            }

            private void HandlePlayScrolling()
            {
                playOffset -= InputParameters.Instance.tempo * Automatone.SUBBEATS_PER_WHOLE_NOTE / SCROLL_SPEED_DIVISOR * Cells.CELLWIDTH;
                gridDrawOffset.X = Math.Min(playOffset + CURSOR_OFFSET, 0);
                ClampGridOffsetX();
                RenewHorizontalClipping();
            }

            private void HandleManualScrolling(ref float moveVal, ref float gridOffsetDimension, VoidDelegate clampDelegate, VoidDelegate renewClippingDelegate, Keys positiveDirectionKey, Keys negativeDirectionKey)
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
                clampDelegate();
                renewClippingDelegate();
            }

            private void HandleHorizontalScrolling()
            {
                HandleManualScrolling(ref moveValX, ref gridDrawOffset.X, ClampGridOffsetX, RenewHorizontalClipping, Keys.Right, Keys.Left);
            }

            private void HandleVerticalScrolling()
            {
                HandleManualScrolling(ref moveValY, ref gridDrawOffset.Y, ClampGridOffsetY, RenewVerticalClipping, Keys.Down, Keys.Up);
            }

            private void ClampGridOffsetX()
            {
                gridDrawOffset.X = MathHelper.Clamp(gridDrawOffset.X, parent.parent.Window.ClientBounds.Width - (Math.Max(parent.SongCells.GetLength(1), (parent.parent.Window.ClientBounds.Width - Labels.PITCH_LABEL_THICKNESS) / Cells.CELLWIDTH) * Cells.CELLWIDTH), Labels.TIME_LABEL_THICKNESS);
            }

            private void ClampGridOffsetY()
            {
                gridDrawOffset.Y = MathHelper.Clamp(gridDrawOffset.Y, parent.parent.Window.ClientBounds.Height - ((parent.SongCells.GetLength(0)) * Cells.CELLHEIGHT), Automatone.CONTROLS_AND_GRID_DIVISION + Labels.TIME_LABEL_THICKNESS);
            }
        }
    }
}