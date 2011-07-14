using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using MoodSwingCoreComponents;
using MoodSwingGUI;

namespace Automatone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Automatone : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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

        public Automatone()
        {
            graphics = new GraphicsDeviceManager(this);
            MSResolution.Init(ref graphics);
            MSResolution.SetVirtualResolution(50 * 16, 150 + 44 * 16);
            MSResolution.SetResolution(50 * 16, 150 + 44 * 16, false);

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.Title = "Automatone";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
           
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            randomButton = new MSButton(null, null, new Rectangle(0, 0, 117, 45),
                Content.Load<Texture2D>("random"),
                Content.Load<Texture2D>("random"),
                Content.Load<Texture2D>("random"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this);

            playButton = new MSButton(null, null, new Rectangle(0, 0, 45, 42),
                Content.Load<Texture2D>("play"),
                Content.Load<Texture2D>("play"),
                Content.Load<Texture2D>("play"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this);

            rewindButton = new MSButton(null, null, new Rectangle(0, 0, 33, 36),
                Content.Load<Texture2D>("rev"),
                Content.Load<Texture2D>("rev"),
                Content.Load<Texture2D>("rev"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this);

            forwardButton = new MSButton(null, null, new Rectangle(0, 0, 33, 36),
                Content.Load<Texture2D>("fwd"),
                Content.Load<Texture2D>("fwd"),
                Content.Load<Texture2D>("fwd"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this);

            slider = new MSImageHolder(new Rectangle(0, 0, 143, 44), Content.Load<Texture2D>("slider"), spriteBatch, this);
            sliderCursor = new MSImageHolder(new Rectangle(0, 0, 15, 15), Content.Load<Texture2D>("slidercursor"), spriteBatch, this);

            topLeftPanel = new MSPanel(null, new Rectangle(0, 0, 30 * 16, 150), null, Shape.RECTANGULAR, spriteBatch, this);
            inputTabs = new MSTabbedPanel(topLeftPanel);

            topRightPanel = new MSPanel(null, new Rectangle(30 * 16, 0, 20 * 16, 150), null, Shape.RECTANGULAR, spriteBatch, this);
            topRightPanel.AddComponent(randomButton, Alignment.TOP_CENTER);
            topRightPanel.AddComponent(playButton, Alignment.MIDDLE_CENTER);
            topRightPanel.AddComponent(rewindButton, Alignment.MIDDLE_LEFT);
            topRightPanel.AddComponent(forwardButton, Alignment.MIDDLE_RIGHT);
            topRightPanel.AddComponent(slider, Alignment.BOTTOM_CENTER);
            topRightPanel.AddComponent(sliderCursor, Alignment.BOTTOM_CENTER);

            topPanel = new MSPanel(null, new Rectangle(0, 0, 50 * 16, 150), null, Shape.RECTANGULAR, spriteBatch, this);
            topPanel.AddComponent(topLeftPanel);
            topPanel.AddComponent(topRightPanel);

            gridPanel = new MSPanel(null, new Rectangle(0, 150, 50 * 16, 44 * 16), null, Shape.RECTANGULAR, spriteBatch, this);

            Random r = new Random();
            for (int i = 0; i < 44; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    gridPanel.AddComponent(
                        new MSImageHolder(
                            new Rectangle(
                                gridPanel.BoundingRectangle.X + j * 16,
                                gridPanel.BoundingRectangle.Y + i * 16,
                                16, 16),
                            (r.NextDouble() > 0.8 ? Content.Load<Texture2D>("lightbox") : Content.Load<Texture2D>("darkbox")),
                            spriteBatch,
                            this));
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            topPanel.Draw(gameTime);
            gridPanel.Draw(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}