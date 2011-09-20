using System;
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

        private Rectangle boundingRectangle;

        public Vector2 gridOffset;
        public float playOffset;

        public const byte CELLHEIGHT = 15;
        public const byte CELLWIDTH = 15;

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

        public GridScreen(Automatone parent)
            : base(parent)
        {
            automatone = parent;
            boundingRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT - Automatone.CONTROLS_AND_GRID_DIVISION);
            oldMouseState = Mouse.GetState();
            
            particleSystem = new ParticleSystem<NoteHitParticle>(8192);
            particleSystem.Affectors.Add(new MovementAffector<NoteHitParticle>(new NoteHitParticleModifier()));
            currentCycle = 0;
        }

        public override void Initialize()
        {
            base.Initialize();
            Reset();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            silentCell = automatone.Content.Load<Texture2D>("darkbox");
            startCell = automatone.Content.Load<Texture2D>("lightbox");
            holdCell = automatone.Content.Load<Texture2D>("holdbox");
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
                    KeyboardState newKeyboardState = Keyboard.GetState();
                    MouseState newMouseState = Mouse.GetState();

                    if (ScrollWithMidi)
                    {
                        playOffset -= automatone.Tempo * Automatone.SUBBEATS_PER_WHOLE_NOTE / 14400.0f * CELLWIDTH;
                        gridOffset.X = Math.Min(playOffset + 100, 0);
                    }
                    else
                    {
                        if (System.Windows.Forms.Form.FromHandle(automatone.Window.Handle).Focused)
                        {
                            if (newKeyboardState.IsKeyDown(Keys.Left) == newKeyboardState.IsKeyDown(Keys.Right))
                            {
                                moveValX = INIT_GRID_MOVE_SPEED;
                            }
                            else if (newKeyboardState.IsKeyDown(Keys.Left))
                            {
                                moveValX += GRID_MOVE_ACCELERATION;
                                if (moveValX > MAX_GRID_MOVE_SPEED)
                                {
                                    moveValX = MAX_GRID_MOVE_SPEED;
                                }

                                gridOffset.X += moveValX;
                            }
                            else if (newKeyboardState.IsKeyDown(Keys.Right))
                            {
                                moveValX += GRID_MOVE_ACCELERATION;
                                if (moveValX > MAX_GRID_MOVE_SPEED)
                                {
                                    moveValX = MAX_GRID_MOVE_SPEED;
                                }

                                gridOffset.X -= moveValX;
                            }
                        }
                    }
                    if (System.Windows.Forms.Form.FromHandle(automatone.Window.Handle).Focused)
                    {
                        if (newKeyboardState.IsKeyDown(Keys.Up) == newKeyboardState.IsKeyDown(Keys.Down))
                        {
                            moveValY = INIT_GRID_MOVE_SPEED;
                        }
                        else if (newKeyboardState.IsKeyDown(Keys.Up))
                        {
                            moveValY += GRID_MOVE_ACCELERATION;
                            if (moveValY > MAX_GRID_MOVE_SPEED)
                            {
                                moveValY = MAX_GRID_MOVE_SPEED;
                            }

                            gridOffset.Y += moveValY;
                        }
                        else if (newKeyboardState.IsKeyDown(Keys.Down))
                        {
                            moveValY += GRID_MOVE_ACCELERATION;
                            if (moveValY > MAX_GRID_MOVE_SPEED)
                            {
                                moveValY = MAX_GRID_MOVE_SPEED;
                            }

                            gridOffset.Y -= moveValY;
                        }
                           
                        if (boundingRectangle.Contains(new Point(newMouseState.X, newMouseState.Y)) && automatone.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
                        {
                            if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                            {
                                manualGridChangeStartXIndex = ScreenToGridCoordinatesX(Mouse.GetState().X);
                                manualGridChangeEndXIndex = ScreenToGridCoordinatesX(Mouse.GetState().X);
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
                                manualGridChangeEndXIndex = ScreenToGridCoordinatesX(Mouse.GetState().X);
                                int k = manualGridChangeStartXIndex.Value + 1;
                                while (automatone.SongCells[manualGridChangeYIndex.Value, k] == CellState.HOLD)
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
                    }
                    gridOffset.X = MathHelper.Clamp(gridOffset.X, Automatone.SCREEN_WIDTH - ((automatone.SongCells.GetLength(1)) * CELLWIDTH), 0);
                    gridOffset.Y = MathHelper.Clamp(gridOffset.Y, Automatone.SCREEN_HEIGHT - ((automatone.SongCells.GetLength(0)) * CELLHEIGHT), Automatone.CONTROLS_AND_GRID_DIVISION);
                    
                    oldMouseState = newMouseState;
                    //ParticleSystemUpdate();
                }
                base.Update(gameTime);
            
        }

        public override void Draw(GameTime gameTime)
        {
            if (automatone.SongCells != null)
            {
                automatone.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                int startI = Math.Max(0, ScreenToGridCoordinatesY(Automatone.SCREEN_HEIGHT));
                int endI = ScreenToGridCoordinatesY(Automatone.CONTROLS_AND_GRID_DIVISION);

                int startJ = ScreenToGridCoordinatesX(0);
                int endJ = Math.Min(ScreenToGridCoordinatesX(Automatone.SCREEN_WIDTH), automatone.SongCells.GetLength(1) - 1);

                for (int i = startI; i < endI; i++)
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
                            automatone.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, automatone.SongCells[i, j] == CellState.SILENT ? (ScrollWithMidi ? 64 : 128) : 255));
                        }
                    }
                }

                ParticleSystemDraw();
                automatone.SpriteBatch.End();
            }

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

        public void Reset()
        {
            gridOffset = new Vector2(0, Automatone.CONTROLS_AND_GRID_DIVISION);
            playOffset = 0;
            ManualGridChangeReset();
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

        private Color GetChromaticColor(int pitch)
        {
            int chromaticIndex = pitch % 12;
            switch(chromaticIndex)
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

        private void ParticleSystemUpdate()
        {
            if (currentCycle++ > PARTICLE_SPAWN_CYCLE)
            {
                int startI = Math.Max(0, ScreenToGridCoordinatesY(Automatone.SCREEN_HEIGHT));
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