using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoodSwingGUI;
using MoodSwingCoreComponents;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Automatone
{
    public class MainScreen : MSScreen
    {
        private MSButton randomButton;
        private MSTabbedPanel inputTabs;
        private MSCheckbox playButton;
        private MSButton rewindButton;
        private MSButton forwardButton;
        private MSImageHolder slider;
        private MSImageHolder sliderCursor;
        private MSPanel topLeftPanel;
        private MSPanel topRightPanel;
        private MSPanel topPanel;
        public MSPanel gridPanel;
        public Vector2 gridOffset;

        public float playOffset;

        public int gridWidth = 0;
        public int gridHeight = 0;

        public const int CELLSIZE = 12;
        public const int GRID_Y_POSITION = 150;

        private float moveValX = 0;
        private float moveValY = 0;

        private const float INIT_GRID_MOVE_SPEED = 1;
        private const float GRID_MOVE_ACCELERATION = .02F;
        private const float MAX_GRID_MOVE_SPEED = 10;

        public bool ScrollWithMidi { set; get; }

        private Dictionary<Vector2, MSButton> gridAccessor;

        public MainScreen(Texture2D background, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : this(background, 0, 0, 0, 0, Color.White, spriteBatch, game, graphics) { }
        
        public MainScreen(Texture2D background, Color highlight, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : this(background, 0, 0, 0, 0, highlight, spriteBatch, game, graphics) { }
        
        public MainScreen(Texture2D background, float topPadding, float bottomPadding, float leftPadding, float rightPadding, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : this(background, topPadding, bottomPadding, leftPadding, rightPadding, Color.White, spriteBatch, game, graphics) { }
        
        public MainScreen(Texture2D background, float topPadding, float bottomPadding, float leftPadding, float rightPadding, Color highlight, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : base(background, topPadding, bottomPadding, leftPadding, rightPadding, highlight, spriteBatch, game)
        {
            randomButton = new MSButton(null, new SongRandomizer(), new Rectangle(0, 0, 117, 45),
                game.Content.Load<Texture2D>("random"),
                game.Content.Load<Texture2D>("random"),
                game.Content.Load<Texture2D>("random"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game);

            playButton = new MSCheckbox(
                new MSButton(
                    null, new PlayMidi(), new Rectangle(0, 0, 45, 42),
                    game.Content.Load<Texture2D>("play"),
                    game.Content.Load<Texture2D>("play"),
                    game.Content.Load<Texture2D>("play"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                new MSButton(
                    null, new PauseMidi(), new Rectangle(0, 0, 45, 42),
                    game.Content.Load<Texture2D>("stop"),
                    game.Content.Load<Texture2D>("stop"),
                    game.Content.Load<Texture2D>("stop"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                false);

            rewindButton = new MSButton(null, null, new Rectangle(0, 0, 33, 36),
                game.Content.Load<Texture2D>("rev"),
                game.Content.Load<Texture2D>("rev"),
                game.Content.Load<Texture2D>("rev"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game);

            forwardButton = new MSButton(null, null, new Rectangle(0, 0, 33, 36),
                game.Content.Load<Texture2D>("fwd"),
                game.Content.Load<Texture2D>("fwd"),
                game.Content.Load<Texture2D>("fwd"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game);

            slider = new MSImageHolder(new Rectangle(0, 0, 101, 13), game.Content.Load<Texture2D>("slider"), spriteBatch, game);
            sliderCursor = new MSImageHolder(new Rectangle(0, 0, 15, 15), game.Content.Load<Texture2D>("slidercursor"), spriteBatch, game);

            topLeftPanel = new MSPanel(null, new Rectangle(0, 0, 400, 150), null, Shape.RECTANGULAR, spriteBatch, game);

            MSPanel basicPanel = new MSPanel(null, new Rectangle(0, 45, 30 * CELLSIZE, 105), 6, 6, 25, 15, null, Shape.RECTANGULAR, spriteBatch, game);
            basicPanel.AddComponent(new MSWrappingLabel(new Point(0, 0), "Mood:",  game.Content.Load<SpriteFont>("Temp"), Color.White, null, null, null, spriteBatch, game), Alignment.TOP_LEFT);
            basicPanel.AddComponent(new MSButton(
                new MSWrappingLabel(new Point(50, 0), "Happy", game.Content.Load<SpriteFont>("Temp"), Color.White, null, null, null, spriteBatch, game),
                null,
                new Rectangle(0, 0, 50, 35),
                game.Content.Load<Texture2D>("radiobutton"),
                game.Content.Load<Texture2D>("radiobutton"),
                game.Content.Load<Texture2D>("radiobutton"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game), Alignment.MIDDLE_LEFT);
            basicPanel.AddComponent(new MSButton(
                new MSWrappingLabel(new Point(50, 0), "Sad", game.Content.Load<SpriteFont>("Temp"), Color.White, null, null, null, spriteBatch, game),
                null,
                new Rectangle(0, 0, 33, 30),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game), Alignment.BOTTOM_LEFT);

            basicPanel.AddComponent(new MSWrappingLabel(new Point(0, 0), "Speed:", game.Content.Load<SpriteFont>("Temp"), Color.White, null, null, null, spriteBatch, game), Alignment.TOP_CENTER);
            basicPanel.AddComponent(new MSButton(
                new MSWrappingLabel(new Point(50, 0), "Fast", game.Content.Load<SpriteFont>("Temp"), Color.White, null, null, null, spriteBatch, game),
                null,
                new Rectangle(0, 0, 33, 30),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game), Alignment.MIDDLE_CENTER);
            basicPanel.AddComponent(new MSButton(
                new MSWrappingLabel(new Point(50, 0), "Slow", game.Content.Load<SpriteFont>("Temp"), Color.White, null, null, null, spriteBatch, game),
                null,
                new Rectangle(0, 0, 50, 35),
                game.Content.Load<Texture2D>("radiobutton"),
                game.Content.Load<Texture2D>("radiobutton"),
                game.Content.Load<Texture2D>("radiobutton"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game), Alignment.BOTTOM_CENTER);

            MSPanel expertPanel = new MSPanel(null, new Rectangle(), null, Shape.RECTANGULAR, spriteBatch, game);

            inputTabs = new MSTabbedPanel(topLeftPanel);
            inputTabs.AddTab(new MSTab(
                new MSButton(
                    new MSWrappingLabel(new Point(10, 10), "Basic", game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, null, null, null, spriteBatch, game),
                    null,
                    new Rectangle(0, 0, 117, 45),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                new MSButton(
                    new MSWrappingLabel(new Point(10, 10), "Basic", game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, null, null, null, spriteBatch, game),
                    null,
                    new Rectangle(0, 0, 117, 45),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                true,
                basicPanel));

            inputTabs.AddTab(new MSTab(
                new MSButton(
                    new MSWrappingLabel(new Point(10, 10), "Expert", game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, null, null, null, spriteBatch, game),
                    null,
                    new Rectangle(117, 0, 117, 45),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                new MSButton(
                    new MSWrappingLabel(new Point(10, 10), "Expert", game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, null, null, null, spriteBatch, game),
                    null,
                    new Rectangle(117, 0, 117, 45),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    game.Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                false,
                expertPanel));

            topLeftPanel.AddComponent(basicPanel);


            topRightPanel = new MSPanel(null, new Rectangle(400, 0, 400, 150), 10, 5, 85, 85, null, Shape.RECTANGULAR, spriteBatch, game);

            topRightPanel.AddComponent(randomButton, Alignment.TOP_CENTER);
            topRightPanel.AddComponent(playButton, Alignment.MIDDLE_CENTER);
            topRightPanel.AddComponent(rewindButton, Alignment.MIDDLE_LEFT);
            topRightPanel.AddComponent(forwardButton, Alignment.MIDDLE_RIGHT);
            topRightPanel.AddComponent(slider, Alignment.BOTTOM_CENTER);
            topRightPanel.AddComponent(sliderCursor, Alignment.BOTTOM_CENTER);

            topPanel = new MSPanel(null, new Rectangle(0, 0, 800, 150), null, Shape.RECTANGULAR, spriteBatch, game);
            topPanel.AddComponent(topLeftPanel);
            topPanel.AddComponent(topRightPanel);
            
            AddComponent(topPanel);

            gridPanel = new MSPanel(null, new Rectangle(0, GRID_Y_POSITION, 800, 600), null, Shape.RECTANGULAR, spriteBatch, game);
            gridOffset = new Vector2(0, 0);
            gridAccessor = new Dictionary<Vector2, MSButton>();
        }

        public override void Update(GameTime gameTime)
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
            
            gridOffset.X = MathHelper.Clamp(gridOffset.X, gridPanel.BoundingRectangle.Width - 10 - CELLSIZE - (gridWidth * CELLSIZE), 0);
            gridOffset.Y = MathHelper.Clamp(gridOffset.Y, gridPanel.BoundingRectangle.Height - 160 - CELLSIZE - (gridHeight * CELLSIZE), 0);

            HandleMouseInput(false);
            gridPanel.Update(gameTime);
 	        base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (MSGUIClickable cell in gridPanel.ClickableComponents)
            {
                Vector2 drawPosition = cell.Position + gridOffset;
                if (gridPanel.BoundingRectangle.Intersects(new Rectangle((int)drawPosition.X, (int)drawPosition.Y, CELLSIZE, CELLSIZE)))
                {
                    if (Math.Abs(cell.Position.X) < -1 * playOffset - CELLSIZE)
                    {
                        spriteBatch.Draw(cell.CollisionTexture, drawPosition, Color.Gray);
                    }
                    else if (Math.Abs(cell.Position.X) < -1 * playOffset)
                    {
                        spriteBatch.Draw(cell.CollisionTexture, drawPosition, Color.SpringGreen);
                    }
                    else
                    {
                        spriteBatch.Draw(cell.CollisionTexture, drawPosition, Color.White);
                    }
                }
            }

            base.Draw(gameTime);
        }

        public void AddGridButton(MSButton button, int horizontalIndex, int verticalIndex)
        {
            gridPanel.AddComponent(button);
            int buttonXPosition = gridPanel.BoundingRectangle.X + horizontalIndex * MainScreen.CELLSIZE;
            int buttonYPosition = gridPanel.BoundingRectangle.Y + verticalIndex * MainScreen.CELLSIZE;
            button.BoundingRectangle = new Rectangle(
                                buttonXPosition,
                                buttonYPosition,
                                MainScreen.CELLSIZE, MainScreen.CELLSIZE);
            gridAccessor.Add(new Vector2(buttonXPosition, buttonYPosition), button);
        }

        public void DestroyGridButtons()
        {
            gridPanel.ClickableComponents.Clear();
            gridAccessor.Clear();
        }
    }
}
