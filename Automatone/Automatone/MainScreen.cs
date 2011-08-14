using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoodSwingGUI;
using MoodSwingCoreComponents;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Automatone
{
    class MainScreen : MSScreen
    {
        MSButton randomButton;
        MSTabbedPanel inputTabs;
        MSButton playButton;
        MSButton rewindButton;
        MSButton forwardButton;
        MSImageHolder slider;
        MSImageHolder sliderCursor;
        MSPanel topLeftPanel;
        MSPanel topRightPanel;
        MSPanel topPanel;
        MSPanel gridPanel;

        const int CELLSIZE = 12;

        List<CellState[,]> songCells;

        public MainScreen(Texture2D background, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : this(background, 0, 0, 0, 0, Color.White, spriteBatch, game, graphics) { }
        
        public MainScreen(Texture2D background, Color highlight, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : this(background, 0, 0, 0, 0, highlight, spriteBatch, game, graphics) { }
        
        public MainScreen(Texture2D background, float topPadding, float bottomPadding, float leftPadding, float rightPadding, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : this(background, topPadding, bottomPadding, leftPadding, rightPadding, Color.White, spriteBatch, game, graphics) { }
        
        public MainScreen(Texture2D background, float topPadding, float bottomPadding, float leftPadding, float rightPadding, Color highlight, SpriteBatch spriteBatch, Game game, GraphicsDeviceManager graphics)
            : base(background, topPadding, bottomPadding, leftPadding, rightPadding, highlight, spriteBatch, game)
        {
            songCells = new List<CellState[,]>();
            songCells.Add(new CellState[10, 10]);
            randomButton = new MSButton(null, new SongRandomizer(songCells, graphics, CELLSIZE), new Rectangle(0, 0, 117, 45),
                game.Content.Load<Texture2D>("random"),
                game.Content.Load<Texture2D>("random"),
                game.Content.Load<Texture2D>("random"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game);

            playButton = new MSButton(null, null, new Rectangle(0, 0, 45, 42),
                game.Content.Load<Texture2D>("play"),
                game.Content.Load<Texture2D>("play"),
                game.Content.Load<Texture2D>("play"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game);

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

            topLeftPanel = new MSPanel(null, new Rectangle(0, 0, 30 * CELLSIZE, 150), null, Shape.RECTANGULAR, spriteBatch, game);

            MSPanel basicPanel = new MSPanel(game.Content.Load<Texture2D>("darkblue"), new Rectangle(0, 45, 30 * CELLSIZE, 105), 6, 6, 25, 15, null, Shape.RECTANGULAR, spriteBatch, game);
            basicPanel.AddComponent(new MSFontScalingLabel("Mood:", new Rectangle(0, 0, 50, 25), game.Content.Load<SpriteFont>("Temp"), spriteBatch, game), Alignment.TOP_LEFT);
            basicPanel.AddComponent(new MSButton(
                new MSFontScalingLabel("Happy", new Rectangle(50, 0, 50, 25), game.Content.Load<SpriteFont>("Temp"), spriteBatch, game),
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
                new MSFontScalingLabel("Sad", new Rectangle(50, 0, 50, 25), game.Content.Load<SpriteFont>("Temp"), spriteBatch, game),
                null,
                new Rectangle(0, 0, 33, 30),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                game.Content.Load<Texture2D>("radiobuttonempty"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                game), Alignment.BOTTOM_LEFT);

            basicPanel.AddComponent(new MSFontScalingLabel("Speed:", new Rectangle(0, 0, 50, 25), game.Content.Load<SpriteFont>("Temp"), spriteBatch, game), Alignment.TOP_CENTER);
            basicPanel.AddComponent(new MSButton(
                new MSFontScalingLabel("Fast", new Rectangle(50, 0, 50, 25), game.Content.Load<SpriteFont>("Temp"), spriteBatch, game),
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
                new MSFontScalingLabel("Slow", new Rectangle(50, 0, 50, 25), game.Content.Load<SpriteFont>("Temp"), spriteBatch, game),
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
                    new MSFontScalingLabel("Basic", new Rectangle(10, 10, 50, 25), game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, game),
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
                    new MSFontScalingLabel("Basic", new Rectangle(10, 10, 50, 25), game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, game),
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
                    new MSFontScalingLabel("Expert", new Rectangle(10, 10, 50, 25), game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, game),
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
                    new MSFontScalingLabel("Expert", new Rectangle(10, 10, 50, 25), game.Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, game),
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

            topRightPanel = new MSPanel(game.Content.Load<Texture2D>("darkblue"), new Rectangle(30 * CELLSIZE, 0, 20 * CELLSIZE, 150), 10, 5, 85, 85, null, Shape.RECTANGULAR, spriteBatch, game);
            topRightPanel.AddComponent(randomButton, Alignment.TOP_CENTER);
            topRightPanel.AddComponent(playButton, Alignment.MIDDLE_CENTER);
            topRightPanel.AddComponent(rewindButton, Alignment.MIDDLE_LEFT);
            topRightPanel.AddComponent(forwardButton, Alignment.MIDDLE_RIGHT);
            topRightPanel.AddComponent(slider, Alignment.BOTTOM_CENTER);
            topRightPanel.AddComponent(sliderCursor, Alignment.BOTTOM_CENTER);

            topPanel = new MSPanel(null, new Rectangle(0, 0, songCells.ElementAt<CellState[,]>(0).GetLength(1) * CELLSIZE, 150), null, Shape.RECTANGULAR, spriteBatch, game);
            topPanel.AddComponent(topLeftPanel);
            topPanel.AddComponent(topRightPanel);

            gridPanel = new MSPanel(null, new Rectangle(0, 150, songCells.ElementAt<CellState[,]>(0).GetLength(1) * CELLSIZE, songCells.ElementAt<CellState[,]>(0).GetLength(0) * CELLSIZE), null, Shape.RECTANGULAR, spriteBatch, game);

            for (int i = 0; i < songCells.ElementAt<CellState[,]>(0).GetLength(0); i++)
            {
                for (int j = 0; j < songCells.ElementAt<CellState[,]>(0).GetLength(1); j++)
                {
                    gridPanel.AddComponent(
                        new MSImageHolder(
                            new Rectangle(
                                gridPanel.BoundingRectangle.X + j * CELLSIZE,
                                gridPanel.BoundingRectangle.Y + i * CELLSIZE,
                                CELLSIZE, CELLSIZE),
                            (songCells.ElementAt<CellState[,]>(0)[i, j] != CellState.SILENT ? game.Content.Load<Texture2D>("lightbox") : game.Content.Load<Texture2D>("darkbox")),
                            spriteBatch,
                            game));
                }
            }
            AddComponent(topPanel);
            AddComponent(gridPanel);
            AddComponent(randomButton);
        }

        public override void Update(GameTime gameTime)
        {
            HandleMouseInput(false);
 	        base.Update(gameTime);
        }
    }
}
