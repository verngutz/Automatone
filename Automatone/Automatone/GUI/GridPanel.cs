using System;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.Graphics.SpecialEffects.Particles;

namespace Automatone.GUI
{
    public class GridPanel : DrawableGameComponent
    {
        private Automatone parent;

        public const byte CELLHEIGHT = 20;
        public const byte CELLWIDTH = 20;

        public Vector2 gridOffset;
        public float playOffset;

        public bool ScrollWithMidi { set; get; }

        private MouseState oldMouseState;
        private MouseState newMouseState;

        private Rectangle clickableCursorArea;
        private int gridCursorStartIndex;
        private int gridCursorEndIndex;

        private const int TIME_LABEL_THICKNESS = 25;
        private const int PITCH_LABEL_THICKNESS = 25;

        private static Texture2D pitchTimeLabelBackground;
        private static Rectangle pitchLabelRectangle;
        private static Rectangle timeLabelRectangle;

        private static Texture2D silentCell;
        private static Texture2D startCell;
        private static Texture2D holdCell;

        private SpriteFont labelFont;

        private CellsPanel cellsPanel;

        public GridPanel(Automatone parent)
            : base(parent)
        {
            this.parent = parent;
            
            playOffset = 0;

            cellsPanel = new CellsPanel(this);
            oldMouseState = Mouse.GetState();
           
            gridCursorStartIndex = 0;
            gridCursorEndIndex = 0;

            UpdateLabelRectangles();
        }

        public void RespondToWindowResize()
        {
            UpdateLabelRectangles();
            cellsPanel.UpdateClickableArea();
        }

        public void UpdateLabelRectangles()
        {
            pitchLabelRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, PITCH_LABEL_THICKNESS, parent.Window.ClientBounds.Height - Automatone.CONTROLS_AND_GRID_DIVISION);
            timeLabelRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, parent.Window.ClientBounds.Width, TIME_LABEL_THICKNESS);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            silentCell = parent.Content.Load<Texture2D>("darkbox");
            startCell = parent.Content.Load<Texture2D>("lightbox");
            holdCell = parent.Content.Load<Texture2D>("holdbox");

            pitchTimeLabelBackground = parent.Content.Load<Texture2D>("BlackPixel");
            labelFont = parent.Content.Load<SpriteFont>("Font");
        }

        protected override void UnloadContent()
        {
            if (silentCell != null) silentCell.Dispose();
            if (startCell != null) startCell.Dispose();
            if (holdCell != null) holdCell.Dispose();
            if(pitchTimeLabelBackground != null) pitchTimeLabelBackground.Dispose();
            
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            newMouseState = Mouse.GetState();

            if (parent.SongCells != null)
            {
                cellsPanel.Update();
                if(parent.IsActive)
                {
                    HandleGridCursorMovement();
                }
            }

            oldMouseState = newMouseState;
            base.Update(gameTime);
        }

        
        private void HandleGridCursorMovement()
        {
            /**
            if (clickableCellsArea.Contains(new Point(newMouseState.X, newMouseState.Y)) && parent.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
            {
                if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    manualGridChangeStartXIndex = Math.Min(parent.SongCells.GetLength(1) - 1, ScreenToGridCoordinatesX(Mouse.GetState().X));
                    manualGridChangeEndXIndex = Math.Min(parent.SongCells.GetLength(1) - 1, ScreenToGridCoordinatesX(Mouse.GetState().X));
                    manualGridChangeYIndex = ScreenToGridCoordinatesY(Mouse.GetState().Y);
                    if (parent.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] == CellState.SILENT)
                    {
                        parent.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] = CellState.START;
                    }
                    else
                    {
                        parent.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] = CellState.SILENT;
                    }
                }
                else if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    ManualGridChangeReset();
                }
                else if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    manualGridChangeEndXIndex = Math.Min(parent.SongCells.GetLength(1) - 1, ScreenToGridCoordinatesX(Mouse.GetState().X));
                    int k = manualGridChangeStartXIndex.Value + 1;
                    while (k < parent.SongCells.GetLength(1) && parent.SongCells[manualGridChangeYIndex.Value, k] == CellState.HOLD)
                    {
                        parent.SongCells[manualGridChangeYIndex.Value, k] = CellState.SILENT;
                        k++;
                    }
                    for (int i = manualGridChangeStartXIndex.Value + 1; i <= manualGridChangeEndXIndex; i++)
                    {
                        if (parent.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] != CellState.SILENT)
                        {
                            if (parent.SongCells[manualGridChangeYIndex.Value, i] == CellState.SILENT)
                            {
                                parent.SongCells[manualGridChangeYIndex.Value, i] = CellState.HOLD;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
             */
        }

        public override void Draw(GameTime gameTime)
        {
            if (parent.SongCells != null)
            {
                cellsPanel.Draw();

                parent.SpriteBatch.Begin();
                DrawPitchLabel();
                DrawTimeLabel();
                DrawGridCursor();
                parent.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }
       

        private void DrawPitchLabel()
        {
            parent.SpriteBatch.Draw(pitchTimeLabelBackground, pitchLabelRectangle, Color.White);

            if (parent.SongCells != null)
            {
                int startI = Math.Max(0, ScreenToGridCoordinatesY(parent.Window.ClientBounds.Height));
                int endI = ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION + TIME_LABEL_THICKNESS);

                for (int i = startI; i <= endI; i++)
                {
                    Vector2 loc = new Vector2(2, (int)((parent.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT + gridOffset.Y));
                    string letter = "";
                    switch ((i - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER + 12) % 12)
                    {
                        case 0:
                            letter = "C";
                            break;
                        case 1:
                            letter = "C#";
                            break;
                        case 2:
                            letter = "D";
                            break;
                        case 3:
                            letter = "D#";
                            break;
                        case 4:
                            letter = "E";
                            break;
                        case 5:
                            letter = "F";
                            break;
                        case 6:
                            letter = "F#";
                            break;
                        case 7:
                            letter = "G";
                            break;
                        case 8:
                            letter = "G#";
                            break;
                        case 9:
                            letter = "A";
                            break;
                        case 10:
                            letter = "A#";
                            break;
                        case 11:
                            letter = "B";
                            break;
                    }
                    parent.SpriteBatch.DrawString(labelFont, letter, loc, Color.White);
                }
            }
        }

        private void DrawTimeLabel()
        {
            parent.SpriteBatch.Draw(pitchTimeLabelBackground, timeLabelRectangle, Color.White);

            if (parent.SongCells != null)
            {
                int startJ = ScreenToGridCoordinatesX(PITCH_LABEL_THICKNESS);
                int endJ = Math.Min(ScreenToGridCoordinatesX(parent.Window.ClientBounds.Width), parent.SongCells.GetLength(1) - 1);

                for (int j = startJ; j <= endJ; j++)
                {
                    Vector2 loc = new Vector2((int)(j * CELLWIDTH + gridOffset.X), 2 + Automatone.CONTROLS_AND_GRID_DIVISION);
                    if (j % parent.MeasureLength == 0)
                    {
                        parent.SpriteBatch.DrawString(labelFont, "" + (j / parent.MeasureLength + 1), loc, Color.White);
                    }
                    for (int k = 1; k < parent.MeasureLength / (Automatone.SUBBEATS_PER_WHOLE_NOTE / 4); k++)
                    {
                        if (j % parent.MeasureLength == k * Automatone.SUBBEATS_PER_WHOLE_NOTE / 4)
                        {
                            parent.SpriteBatch.DrawString(labelFont, "|", loc, Color.Navy);
                        }
                    }
                }
            }
        }

        private void DrawGridCursor()
        {
            
        }

        public void ResetScrolling()
        {
            playOffset = 0;
            ScrollWithMidi = false;
        }

        private int ScreenToGridCoordinatesX(int y)
        {
            return ((int)-gridOffset.X + y) / CELLWIDTH;
        }

        private int ScreenToGridCoordinatesY(int x)
        {
            return parent.SongCells.GetLength(0) - 1 - ((int)-gridOffset.Y + x) / CELLHEIGHT;
        }

        public void SetScrollLocation(int noteNumber)
        {
            playOffset = -1 * noteNumber * CELLWIDTH;
        }

        /// <summary>
        /// This class handles the updating and drawing of the cells 
        /// only (the visual representation of the current song project 
        /// as a 2D array, excluding the labels, cursors, and navigators)
        /// in the grid panel.
        /// </summary>
        internal class CellsPanel
        {
            private GridPanel parent;

            private float moveValX = 0;
            private float moveValY = 0;

            private const float INIT_GRID_MOVE_SPEED = 1;
            private const float GRID_MOVE_ACCELERATION = .02F;
            private const float MAX_GRID_MOVE_SPEED = 10;

            private Rectangle clickableArea;
            private int? gridInputStartXIndex;
            private int? gridInputEndXIndex;
            private int? gridInputYIndex;

            private ParticleSystem<NoteHitParticle> particleSystem;
            private const int PARTICLE_SPAWN_DENSITY = 4;
            private const int PARTICLE_SPAWN_CYCLE = 50;
            private int currentCycle;

            internal CellsPanel(GridPanel parent)
            {
                this.parent = parent;

                parent.gridOffset = new Vector2(PITCH_LABEL_THICKNESS, Automatone.CONTROLS_AND_GRID_DIVISION + TIME_LABEL_THICKNESS);
                UpdateClickableArea();

                particleSystem = new ParticleSystem<NoteHitParticle>(8192);
                particleSystem.Affectors.Add(new MovementAffector<NoteHitParticle>(new NoteHitParticleModifier()));
                currentCycle = 0;
            }

            internal void UpdateClickableArea()
            {
                clickableArea = new Rectangle
                (
                    PITCH_LABEL_THICKNESS,
                    Automatone.CONTROLS_AND_GRID_DIVISION + TIME_LABEL_THICKNESS,
                    parent.parent.Window.ClientBounds.Width - PITCH_LABEL_THICKNESS,
                    parent.parent.Window.ClientBounds.Height - Automatone.CONTROLS_AND_GRID_DIVISION - TIME_LABEL_THICKNESS
                );
            }

            internal void Update()
            {
                if (parent.ScrollWithMidi)
                {
                    parent.playOffset -= parent.parent.Tempo * Automatone.SUBBEATS_PER_WHOLE_NOTE / 14400.0f * CELLWIDTH;
                    parent.gridOffset.X = Math.Min(parent.playOffset + 100, 0);
                }
                else if (parent.parent.IsActive)
                {
                    HandleHorizontalScrolling();
                }

                if (parent.parent.IsActive)
                {
                    HandleVerticalScrolling();
                    HandleManualGridInput();
                }

                parent.gridOffset.X = MathHelper.Clamp(parent.gridOffset.X, parent.parent.Window.ClientBounds.Width - (Math.Max(parent.parent.SongCells.GetLength(1), (parent.parent.Window.ClientBounds.Width - PITCH_LABEL_THICKNESS) / CELLWIDTH) * CELLWIDTH), TIME_LABEL_THICKNESS);
                parent.gridOffset.Y = MathHelper.Clamp(parent.gridOffset.Y, parent.parent.Window.ClientBounds.Height - ((parent.parent.SongCells.GetLength(0)) * CELLHEIGHT), Automatone.CONTROLS_AND_GRID_DIVISION + TIME_LABEL_THICKNESS);

                //UpdateParticleSystem();
            }

            private void HandleHorizontalScrolling()
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left) == Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    moveValX = INIT_GRID_MOVE_SPEED;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    moveValX += GRID_MOVE_ACCELERATION;
                    if (moveValX > MAX_GRID_MOVE_SPEED)
                    {
                        moveValX = MAX_GRID_MOVE_SPEED;
                    }

                    parent.gridOffset.X += moveValX;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    moveValX += GRID_MOVE_ACCELERATION;
                    if (moveValX > MAX_GRID_MOVE_SPEED)
                    {
                        moveValX = MAX_GRID_MOVE_SPEED;
                    }

                    parent.gridOffset.X -= moveValX;
                }
            }

            private void HandleVerticalScrolling()
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up) == Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    moveValY = INIT_GRID_MOVE_SPEED;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    moveValY += GRID_MOVE_ACCELERATION;
                    if (moveValY > MAX_GRID_MOVE_SPEED)
                    {
                        moveValY = MAX_GRID_MOVE_SPEED;
                    }

                    parent.gridOffset.Y += moveValY;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    moveValY += GRID_MOVE_ACCELERATION;
                    if (moveValY > MAX_GRID_MOVE_SPEED)
                    {
                        moveValY = MAX_GRID_MOVE_SPEED;
                    }

                    parent.gridOffset.Y -= moveValY;
                }
            }

            private void HandleManualGridInput()
            {
                if (clickableArea.Contains(new Point(parent.newMouseState.X, parent.newMouseState.Y)) && parent.parent.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
                {
                    if (parent.newMouseState.LeftButton == ButtonState.Pressed && parent.oldMouseState.LeftButton == ButtonState.Released)
                    {
                        gridInputStartXIndex = Math.Min(parent.parent.SongCells.GetLength(1) - 1, parent.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputEndXIndex = Math.Min(parent.parent.SongCells.GetLength(1) - 1, parent.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputYIndex = parent.ScreenToGridCoordinatesY(Mouse.GetState().Y);
                        if (parent.parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] == CellState.SILENT)
                        {
                            parent.parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.START;
                        }
                        else
                        {
                            parent.parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.SILENT;
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
                            gridInputEndXIndex = Math.Min(parent.parent.SongCells.GetLength(1) - 1, parent.ScreenToGridCoordinatesX(Mouse.GetState().X));
                            int k = gridInputStartXIndex.Value + 1;
                            while (k < parent.parent.SongCells.GetLength(1) && parent.parent.SongCells[gridInputYIndex.Value, k] == CellState.HOLD)
                            {
                                parent.parent.SongCells[gridInputYIndex.Value, k] = CellState.SILENT;
                                k++;
                            }
                            for (int i = gridInputStartXIndex.Value + 1; i <= gridInputEndXIndex; i++)
                            {
                                if (parent.parent.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] != CellState.SILENT)
                                {
                                    if (parent.parent.SongCells[gridInputYIndex.Value, i] == CellState.SILENT)
                                    {
                                        parent.parent.SongCells[gridInputYIndex.Value, i] = CellState.HOLD;
                                    }
                                    else break;
                                }
                            }
                        }
                    }
                }
            }

            private void UpdateParticleSystem()
            {
                if (currentCycle++ > PARTICLE_SPAWN_CYCLE)
                {
                    int startI = Math.Max(0, parent.ScreenToGridCoordinatesY(parent.parent.Window.ClientBounds.Height));
                    int endI = parent.ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION);

                    int j = parent.ScreenToGridCoordinatesX((int)-parent.playOffset);

                    for (int i = startI; i < endI; i++)
                    {
                        if (parent.parent.SongCells[i, j] != CellState.SILENT)
                        {
                            Vector2 particleSpawnPoint = new Vector2((int)(j * CELLWIDTH + parent.gridOffset.X), (int)((parent.parent.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT + parent.gridOffset.Y));
                            for (float k = 0; k < PARTICLE_SPAWN_DENSITY; k++)
                            {
                                if (particleSystem.Particles.Count < particleSystem.Capacity)
                                    particleSystem.AddParticle(new NoteHitParticle(new Vector3(particleSpawnPoint + new Vector2(0, k / PARTICLE_SPAWN_DENSITY * CELLHEIGHT), 0), new Vector3(1, 0, 0), GetChromaticColor(i)));
                            }
                        }
                    }
                    currentCycle = 0;
                }

                IAsyncResult asyncResult = particleSystem.BeginUpdate(1, 4, null, null);
                particleSystem.EndUpdate(asyncResult);
                particleSystem.Prune(NoteHitParticle.IsAlive);
            }

            internal void Draw()
            {
                parent.parent.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                DrawGridCells();
                parent.parent.SpriteBatch.End();

                parent.parent.SpriteBatch.Begin();
                DrawParticleSystem();
                parent.parent.SpriteBatch.End();
            }

            private void DrawGridCells()
            {
                int startI = Math.Max(0, parent.ScreenToGridCoordinatesY(parent.parent.Window.ClientBounds.Height));
                int endI = parent.ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION + TIME_LABEL_THICKNESS);

                int startJ = parent.ScreenToGridCoordinatesX(PITCH_LABEL_THICKNESS);
                int endJ = Math.Min(parent.ScreenToGridCoordinatesX(parent.parent.Window.ClientBounds.Width), parent.parent.SongCells.GetLength(1) - 1);

                for (int i = startI; i <= endI; i++)
                {
                    for (int j = startJ; j <= endJ; j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(j * CELLWIDTH + parent.gridOffset.X), (int)((parent.parent.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT + parent.gridOffset.Y), CELLWIDTH, CELLHEIGHT);
                        Color drawColor = GetChromaticColor(i);
                        if (j * CELLWIDTH < -parent.playOffset - CELLWIDTH)
                        {
                            parent.parent.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 32));
                        }
                        else if (j * CELLWIDTH < -parent.playOffset)
                        {
                            parent.parent.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 255));
                        }
                        else
                        {
                            parent.parent.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, parent.parent.SongCells[i, j] == CellState.SILENT ? (parent.parent.Sequencer.State == Sequencer.MidiPlayerState.STOPPED ? 128 : 64) : 255));
                        }
                    }
                }
            }

            private Texture2D GetCellTexture(int i, int j)
            {
                switch (parent.parent.SongCells[i, j])
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

            private Color GetChromaticColor(int pitch)
            {
                int chromaticIndex = pitch % 12;
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

            private void DrawParticleSystem()
            {
                foreach (NoteHitParticle particle in particleSystem.Particles.Array)
                {
                    parent.parent.SpriteBatch.Draw(startCell, new Rectangle((int)particle.Position.X, (int)particle.Position.Y, 2, 2), particle.Color);
                }
            }
        }
    }
}