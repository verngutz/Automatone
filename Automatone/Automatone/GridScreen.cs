using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Automatone
{
    public class GridScreen : DrawableGameComponent
    {
        private Automatone automatone;

        private Rectangle boundingRectangle;

        public Vector2 gridOffset;
        public float playOffset;

        public const byte CELLSIZE = 12;

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

        public GridScreen(Automatone parent)
            : base(parent)
        {
            automatone = parent;
            boundingRectangle = new Rectangle(0, Automatone.CONTROLS_AND_GRID_DIVISION, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT - Automatone.CONTROLS_AND_GRID_DIVISION);
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

                if (ScrollWithMidi)
                {
                    playOffset -= Automatone.TEMPO * Automatone.SUBBEATS_PER_MEASURE / 14400.0f * CELLSIZE;
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

                gridOffset.X = MathHelper.Clamp(gridOffset.X, Automatone.SCREEN_WIDTH - ((automatone.SongCells.GetLength(1) + 1) * CELLSIZE), 0);
                gridOffset.Y = MathHelper.Clamp(gridOffset.Y, Automatone.SCREEN_HEIGHT + Automatone.CONTROLS_AND_GRID_DIVISION - ((automatone.SongCells.GetLength(0) + 1) * CELLSIZE), 0);

            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (automatone.SongCells != null)
            {
                automatone.SpriteBatch.Begin();
                for (int i = 0; i < automatone.SongCells.GetLength(0); i++)
                {
                    for (int j = 0; j < automatone.SongCells.GetLength(1); j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(i * CELLSIZE + gridOffset.X), (int)(j * CELLSIZE + gridOffset.Y), CELLSIZE, CELLSIZE);
                        if (boundingRectangle.Intersects(drawRectangle))
                        {
                            Texture2D cellTexture = null;
                            switch (automatone.SongCells[i, j])
                            {
                                case CellState.SILENT:
                                    cellTexture = silentCell;
                                    break;
                                case CellState.START:
                                    cellTexture = startCell;
                                    break;
                                case CellState.HOLD:
                                    cellTexture = holdCell;
                                    break;
                            }

                            if (Math.Abs(drawRectangle.X) < -playOffset - CELLSIZE)
                            {
                                automatone.SpriteBatch.Draw(cellTexture, drawRectangle, Color.Gray);
                            }
                            else if (Math.Abs(drawRectangle.X) < -playOffset)
                            {
                                automatone.SpriteBatch.Draw(cellTexture, drawRectangle, Color.SpringGreen);
                            }
                            else
                            {
                                automatone.SpriteBatch.Draw(cellTexture, drawRectangle, Color.White);
                            }
                        }
                    }
                }
                automatone.SpriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void Reset()
        {
            gridOffset = Vector2.Zero;
            playOffset = 0;
        }
    }
}
