using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Duet.Audio_System;

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

        public GridScreen(Automatone parent)
            : base(parent)
        {
            automatone = parent;
            boundingRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT - Automatone.CONTROLS_AND_GRID_DIVISION);
            oldMouseState = Mouse.GetState();
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
            silentCell.Dispose();
            startCell.Dispose();
            holdCell.Dispose();
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
                    playOffset -= automatone.Tempo * Automatone.SUBBEATS_PER_MEASURE / 14400.0f * CELLWIDTH;
                    gridOffset.X = Math.Min(playOffset + 100, 0);
                }
                else
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

                gridOffset.X = MathHelper.Clamp(gridOffset.X, Automatone.SCREEN_WIDTH - ((automatone.SongCells.GetLength(1)) * CELLWIDTH), 0);
                gridOffset.Y = MathHelper.Clamp(gridOffset.Y, Automatone.SCREEN_HEIGHT - ((automatone.SongCells.GetLength(0)) * CELLHEIGHT), Automatone.CONTROLS_AND_GRID_DIVISION);

                if (boundingRectangle.Contains(new Point(newMouseState.X, newMouseState.Y)) && automatone.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
                {
                    if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                    {
                        manualGridChangeStartXIndex = ScreenToGridCoordinatesX(Mouse.GetState().X);
                        manualGridChangeEndXIndex = ScreenToGridCoordinatesX(Mouse.GetState().X);
                        manualGridChangeYIndex = ScreenToGridCoordinatesY(Mouse.GetState().Y);
                    }
                    else if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        ManualGridChangeReset();
                    }
                    else if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        manualGridChangeEndXIndex = ScreenToGridCoordinatesX(Mouse.GetState().X);
                        for (int i = manualGridChangeStartXIndex.Value; i <= manualGridChangeEndXIndex; i++, manualGridChangeStartXIndex++)
                        {
                            if (automatone.SongCells[manualGridChangeYIndex.Value, i] == CellState.SILENT)
                            {
                                if (i == 0 || automatone.SongCells[manualGridChangeYIndex.Value, i - 1] == CellState.SILENT)
                                {
                                    automatone.SongCells[manualGridChangeYIndex.Value, i] = CellState.START;
                                }
                                else
                                {
                                    automatone.SongCells[manualGridChangeYIndex.Value, i] = CellState.HOLD;
                                }
                            }
                            else
                            {
                                automatone.SongCells[manualGridChangeYIndex.Value, i] = CellState.SILENT;
                            }
                        }
                    }
                }
                oldMouseState = newMouseState;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (automatone.SongCells != null)
            {
                automatone.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                for (int i = 0; i < automatone.SongCells.GetLength(0); i++)
                {
                    for (int j = 0; j < automatone.SongCells.GetLength(1); j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(j * CELLWIDTH + gridOffset.X), (int)((automatone.SongCells.GetLength(0) - 1 - i) * CELLHEIGHT+ gridOffset.Y), CELLWIDTH, CELLHEIGHT);
                        if (boundingRectangle.Intersects(drawRectangle))
                        {
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
                }
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
            
        public void setScrollLocation(int noteNumber)
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
    }
}
