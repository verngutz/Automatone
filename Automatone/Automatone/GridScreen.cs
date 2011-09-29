﻿using System;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Graphics.SpecialEffects.Particles;

namespace Automatone
{
    public class GridScreen : DrawableGameComponent
    {
        private Automatone automatone;

        private Rectangle clickableArea;

        public Vector2 gridOffset;
        public float playOffset;

        public const byte CELLHEIGHT = 20;
        public const byte CELLWIDTH = 20;

        private static Texture2D silentCell;
        private static Texture2D startCell;
        private static Texture2D holdCell;

        public bool ScrollWithMidi { set; get; }

        public const int GRID_Y_POSITION = 150;

        private float moveValX = 0;
        private float moveValY = 0;

        private const float INIT_GRID_MOVE_SPEED = 1;
        private const float GRID_MOVE_ACCELERATION = .02F;
        private const float MAX_GRID_MOVE_SPEED = 10;

        private MouseState oldMouseState;
        private int? manualGridChangeStartXIndex;
        private int? manualGridChangeEndXIndex;
        private int? manualGridChangeYIndex;

        private ParticleSystem<NoteHitParticle> particleSystem;
        private const int PARTICLE_SPAWN_DENSITY = 4;
        private const int PARTICLE_SPAWN_CYCLE = 50;
        private int currentCycle;

        private const int PITCH_TIME_LABEL_THICKNESS = 25;

        private static Texture2D pitchTimeLabelBackground;
        private static Rectangle pitchLabelRectangle;
        private static Rectangle timeLabelRectangle;

        private SpriteFont labelFont;

        public GridScreen(Automatone parent)
            : base(parent)
        {
            automatone = parent;
            clickableArea = new Rectangle(PITCH_TIME_LABEL_THICKNESS, Automatone.CONTROLS_AND_GRID_DIVISION + PITCH_TIME_LABEL_THICKNESS, automatone.Window.ClientBounds.Width - PITCH_TIME_LABEL_THICKNESS, automatone.Window.ClientBounds.Height - Automatone.CONTROLS_AND_GRID_DIVISION - PITCH_TIME_LABEL_THICKNESS);
            oldMouseState = Mouse.GetState();
            
            particleSystem = new ParticleSystem<NoteHitParticle>(8192);
            particleSystem.Affectors.Add(new MovementAffector<NoteHitParticle>(new NoteHitParticleModifier()));
            currentCycle = 0;

            SetPitchTimeLabelRectangles();

            gridOffset = new Vector2(PITCH_TIME_LABEL_THICKNESS, Automatone.CONTROLS_AND_GRID_DIVISION + PITCH_TIME_LABEL_THICKNESS);
            playOffset = 0;
            ManualGridChangeReset();
        }

        public void SetPitchTimeLabelRectangles()
        {
            pitchLabelRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, PITCH_TIME_LABEL_THICKNESS, automatone.Window.ClientBounds.Height - Automatone.CONTROLS_AND_GRID_DIVISION);
            timeLabelRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, automatone.Window.ClientBounds.Width, PITCH_TIME_LABEL_THICKNESS);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            silentCell = automatone.Content.Load<Texture2D>("darkbox");
            startCell = automatone.Content.Load<Texture2D>("lightbox");
            holdCell = automatone.Content.Load<Texture2D>("holdbox");
            pitchTimeLabelBackground = automatone.Content.Load<Texture2D>("BlackPixel");
            labelFont = automatone.Content.Load<SpriteFont>("Font");
        }

        protected override void UnloadContent()
        {
            if(silentCell != null) silentCell.Dispose();
            if(startCell != null) startCell.Dispose();
            if(holdCell != null) holdCell.Dispose();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (automatone.SongCells != null)
            {
                if (ScrollWithMidi)
                {
                    playOffset -= automatone.Tempo * Automatone.SUBBEATS_PER_WHOLE_NOTE / 14400.0f * CELLWIDTH;
                    gridOffset.X = Math.Min(playOffset + 100, 0);
                }
                else
                {
                    if (System.Windows.Forms.Form.FromHandle(automatone.Window.Handle).Focused)
                    {
                        HandleHorizontalScrolling();
                    }
                }

                if (System.Windows.Forms.Form.FromHandle(automatone.Window.Handle).Focused)
                {
                    HandleVerticalScrolling();
                    HandleManualGridInput();
                }

                gridOffset.X = MathHelper.Clamp(gridOffset.X, automatone.Window.ClientBounds.Width - (Math.Max(automatone.SongCells.GetLength(1), (automatone.Window.ClientBounds.Width - PITCH_TIME_LABEL_THICKNESS) / CELLWIDTH) * CELLWIDTH), PITCH_TIME_LABEL_THICKNESS);
                gridOffset.Y = MathHelper.Clamp(gridOffset.Y, automatone.Window.ClientBounds.Height - ((automatone.SongCells.GetLength(0)) * CELLHEIGHT), Automatone.CONTROLS_AND_GRID_DIVISION + PITCH_TIME_LABEL_THICKNESS);
                //ParticleSystemUpdate();
            }
            base.Update(gameTime);
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

                gridOffset.X += moveValX;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                moveValX += GRID_MOVE_ACCELERATION;
                if (moveValX > MAX_GRID_MOVE_SPEED)
                {
                    moveValX = MAX_GRID_MOVE_SPEED;
                }

                gridOffset.X -= moveValX;
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

                gridOffset.Y += moveValY;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                moveValY += GRID_MOVE_ACCELERATION;
                if (moveValY > MAX_GRID_MOVE_SPEED)
                {
                    moveValY = MAX_GRID_MOVE_SPEED;
                }

                gridOffset.Y -= moveValY;
            }
        }

        private void HandleManualGridInput()
        {
            MouseState newMouseState = Mouse.GetState();
            if (clickableArea.Contains(new Point(newMouseState.X, newMouseState.Y)) && automatone.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
            {
                if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    manualGridChangeStartXIndex = Math.Min(automatone.SongCells.GetLength(1) - 1, ScreenToGridCoordinatesX(Mouse.GetState().X));
                    manualGridChangeEndXIndex = Math.Min(automatone.SongCells.GetLength(1) - 1, ScreenToGridCoordinatesX(Mouse.GetState().X));
                    manualGridChangeYIndex = ScreenToGridCoordinatesY(Mouse.GetState().Y);
                    if (automatone.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] == CellState.SILENT)
                    {
                        automatone.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] = CellState.START;
                    }
                    else
                    {
                        automatone.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] = CellState.SILENT;
                    }
                }
                else if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    ManualGridChangeReset();
                }
                else if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    manualGridChangeEndXIndex = Math.Min(automatone.SongCells.GetLength(1) - 1, ScreenToGridCoordinatesX(Mouse.GetState().X));
                    int k = manualGridChangeStartXIndex.Value + 1;
                    while (k < automatone.SongCells.GetLength(1) && automatone.SongCells[manualGridChangeYIndex.Value, k] == CellState.HOLD)
                    {
                        automatone.SongCells[manualGridChangeYIndex.Value, k] = CellState.SILENT;
                        k++;
                    }
                    for (int i = manualGridChangeStartXIndex.Value + 1; i <= manualGridChangeEndXIndex; i++)
                    {
                        if (automatone.SongCells[manualGridChangeYIndex.Value, manualGridChangeStartXIndex.Value] != CellState.SILENT)
                        {
                            if (automatone.SongCells[manualGridChangeYIndex.Value, i] == CellState.SILENT)
                            {
                                automatone.SongCells[manualGridChangeYIndex.Value, i] = CellState.HOLD;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            oldMouseState = newMouseState;
        }

        public override void Draw(GameTime gameTime)
        {
            if (automatone.SongCells != null)
            {
                automatone.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                int startI = Math.Max(0, ScreenToGridCoordinatesY(automatone.Window.ClientBounds.Height));
                int endI = ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION + PITCH_TIME_LABEL_THICKNESS);

                int startJ = ScreenToGridCoordinatesX(PITCH_TIME_LABEL_THICKNESS);
                int endJ = Math.Min(ScreenToGridCoordinatesX(automatone.Window.ClientBounds.Width), automatone.SongCells.GetLength(1) - 1);

                for (int i = startI; i <= endI; i++)
                {
                    for (int j = startJ; j <= endJ; j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(j * CELLWIDTH + gridOffset.X), (int)((automatone.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT+ gridOffset.Y), CELLWIDTH, CELLHEIGHT);
                        Color drawColor = GetChromaticColor(i);
                        if (j * CELLWIDTH < -playOffset - CELLWIDTH)
                        {
                            automatone.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 32));
                        }
                        else if (j * CELLWIDTH < -playOffset)
                        {
                            automatone.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 255));
                        }
                        else
                        {
                            automatone.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, automatone.SongCells[i, j] == CellState.SILENT ? (automatone.Sequencer.State == Sequencer.MidiPlayerState.STOPPED ? 128 : 64) : 255));
                        }
                    }
                }

                ParticleSystemDraw();
                automatone.SpriteBatch.End();
            }

            automatone.SpriteBatch.Begin();
            DrawPitchLabel();
            DrawTimeLabel();
            automatone.SpriteBatch.End();

            base.Draw(gameTime);
        }

        private Texture2D GetCellTexture(int i, int j)
        {
            switch (automatone.SongCells[i, j])
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
        private void DrawPitchLabel()
        {
            automatone.SpriteBatch.Draw(pitchTimeLabelBackground, pitchLabelRectangle, Color.White);

            if (automatone.SongCells != null)
            {
                int startI = Math.Max(0, ScreenToGridCoordinatesY(automatone.Window.ClientBounds.Height));
                int endI = ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION + PITCH_TIME_LABEL_THICKNESS);

                for (int i = startI; i <= endI; i++)
                {
                    Vector2 loc = new Vector2(2, (int)((automatone.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT + gridOffset.Y));
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
                    automatone.SpriteBatch.DrawString(labelFont, letter, loc, Color.White);
                }
            }
        }

        private void DrawTimeLabel()
        {
            automatone.SpriteBatch.Draw(pitchTimeLabelBackground, timeLabelRectangle, Color.White);

            if (automatone.SongCells != null)
            {
                int startJ = ScreenToGridCoordinatesX(PITCH_TIME_LABEL_THICKNESS);
                int endJ = Math.Min(ScreenToGridCoordinatesX(automatone.Window.ClientBounds.Width), automatone.SongCells.GetLength(1) - 1);

                for (int j = startJ; j <= endJ; j++)
                {
                    Vector2 loc = new Vector2((int)(j * CELLWIDTH + gridOffset.X), 2 + Automatone.CONTROLS_AND_GRID_DIVISION);
                    if (j % automatone.MeasureLength == 0)
                    {
                        automatone.SpriteBatch.DrawString(labelFont, "" + (j / automatone.MeasureLength + 1), loc, Color.White);
                    }
                    for (int k = 1; k < automatone.MeasureLength / (Automatone.SUBBEATS_PER_WHOLE_NOTE / 4); k++)
                    {
                        if (j % automatone.MeasureLength == k * Automatone.SUBBEATS_PER_WHOLE_NOTE / 4)
                        {
                            automatone.SpriteBatch.DrawString(labelFont, "|", loc, Color.Navy);
                        }
                    }
                }
            }
        }

        public void ResetScrolling()
        {
            playOffset = 0;
            ScrollWithMidi = false;
        }

        private void ManualGridChangeReset()
        {
            manualGridChangeStartXIndex = null;
            manualGridChangeEndXIndex = null;
            manualGridChangeYIndex = null;
        }

        private int ScreenToGridCoordinatesX(int y)
        {
            return ((int)-gridOffset.X + y) / CELLWIDTH;
        }

        private int ScreenToGridCoordinatesY(int x)
        {
            return automatone.SongCells.GetLength(0) - 1 - ((int)-gridOffset.Y + x) / CELLHEIGHT;
        }
            
        public void SetScrollLocation(int noteNumber)
        {
            playOffset = -1 * noteNumber * CELLWIDTH;
        }

        private void ParticleSystemUpdate()
        {
            if (currentCycle++ > PARTICLE_SPAWN_CYCLE)
            {
                int startI = Math.Max(0, ScreenToGridCoordinatesY(automatone.Window.ClientBounds.Height));
                int endI = ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION);

                int j = ScreenToGridCoordinatesX((int)-playOffset);

                for (int i = startI; i < endI; i++)
                {
                    if (automatone.SongCells[i, j] != CellState.SILENT)
                    {
                        Vector2 particleSpawnPoint = new Vector2((int)(j * CELLWIDTH + gridOffset.X), (int)((automatone.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT + gridOffset.Y));
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

        private void ParticleSystemDraw()
        {
            foreach (NoteHitParticle particle in particleSystem.Particles.Array)
            {
                automatone.SpriteBatch.Draw(startCell, new Rectangle((int)particle.Position.X, (int)particle.Position.Y, 2, 2), particle.Color);
            }
        }
    }
}