﻿using System;
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
        private Vector2 gridOffset;

        public const int CELLSIZE = 12;

        private int moveVal = 0;

        private const int INIT_GRID_MOVE_SPEED = 1;
        private const int GRID_MOVE_ACCELERATION = 1;
        private const int MAX_GRID_MOVE_SPEED = 5;

        private KeyboardState oldKeyBoardState;

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
                    null, new Play(), new Rectangle(0, 0, 45, 42),
                    game.Content.Load<Texture2D>("play"),
                    game.Content.Load<Texture2D>("play"),
                    game.Content.Load<Texture2D>("play"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    game),
                new MSButton(
                    null, new Stop(), new Rectangle(0, 0, 45, 42),
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

            gridPanel = new MSPanel(null, new Rectangle(0, 150, 0, 0), null, Shape.RECTANGULAR, spriteBatch, game);
            gridOffset = new Vector2(0, 0);
            oldKeyBoardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            if(newKeyboardState.IsKeyDown(Keys.Left))
            {
                if (oldKeyBoardState.IsKeyDown(Keys.Left))
                {
                    moveVal += GRID_MOVE_ACCELERATION;
                    if (moveVal > MAX_GRID_MOVE_SPEED)
                    {
                        moveVal = MAX_GRID_MOVE_SPEED;
                    }
                }

                gridOffset.X += moveVal;
            }
            else if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                if (oldKeyBoardState.IsKeyDown(Keys.Right))
                {
                    moveVal += GRID_MOVE_ACCELERATION;
                    if (moveVal > MAX_GRID_MOVE_SPEED)
                    {
                        moveVal = MAX_GRID_MOVE_SPEED;
                    }
                }

                gridOffset.X -= moveVal;
            }
            else if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                if (oldKeyBoardState.IsKeyDown(Keys.Up))
                {
                    moveVal += GRID_MOVE_ACCELERATION;
                    if (moveVal > MAX_GRID_MOVE_SPEED)
                    {
                        moveVal = MAX_GRID_MOVE_SPEED;
                    }
                }

                gridOffset.Y += moveVal;
            }
            else if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                if (oldKeyBoardState.IsKeyDown(Keys.Down))
                {
                    moveVal += GRID_MOVE_ACCELERATION;
                    if (moveVal > MAX_GRID_MOVE_SPEED)
                    {
                        moveVal = MAX_GRID_MOVE_SPEED;
                    }
                }

                gridOffset.Y -= moveVal;
            }
            else
            {
                moveVal = INIT_GRID_MOVE_SPEED;
            }

            oldKeyBoardState = Keyboard.GetState();
            HandleMouseInput(false);
            gridPanel.Update(gameTime);
 	        base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (MSGUIClickable square in gridPanel.ClickableComponents)
            {
                Vector2 drawPosition = square.Position + gridOffset;
                if(gridPanel.BoundingRectangle.Intersects(new Rectangle((int)drawPosition.X, (int)drawPosition.Y, CELLSIZE, CELLSIZE)))
                    spriteBatch.Draw(square.CollisionTexture, drawPosition, Color.White);
            }

            base.Draw(gameTime);
        }
    }
}
