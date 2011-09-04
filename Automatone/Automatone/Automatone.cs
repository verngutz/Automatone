using System;
using System.Diagnostics;
using System.IO;
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

using Duet;
using Duet.Audio_System;

namespace Automatone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Automatone : Microsoft.Xna.Framework.Game
    {
        // Content Objects
        ContentManager content;

        // Graphics Objects
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        // Audio Objects
        Synthesizer synthesizer;
        public Sequencer sequencer;

        // Services
        private IAudioSystemService audioService;

        //GUI
        public MainScreen gameScreen;

        public const ushort TEMPO = 120;

        public Automatone()
        {
            graphics = new GraphicsDeviceManager(this);
            

            content = new ContentManager(Services);
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
            // Start up core DUET services
            Duet.Global.Initialize(this);

            // Service Providers
            audioService = (IAudioSystemService)Services.GetService(typeof(IAudioSystemService));

            // Initialize Audio System with game specific settings
            AudioStartupOptions options = new AudioStartupOptions();
            options.audioSettingsPath = "Content\\audio\\WavetablePrograms.xgs";
            audioService.Startup(options);

            //Setup Synthesizer
            synthesizer = new Synthesizer(this);
            synthesizer.WaveBankPath = "Content\\audio\\Wave Bank.xwb";
            synthesizer.SoundBankPath = "Content\\audio\\Sound Bank.xsb";
            synthesizer.m_Patch = content.Load<XACTPatch>(@"content\audio\xact");

            //Construct Sequencer
            sequencer = new Sequencer(this);

            // Setup MIDI routing
            sequencer.OutputDevice = synthesizer;

            audioService.BeatsPerMinute = TEMPO;

            base.Initialize();


           
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameScreen = new MainScreen(null, spriteBatch, this, graphics);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            content.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            sequencer.Update(gameTime);
            synthesizer.Update(gameTime);
            gameScreen.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            gameScreen.Draw(gameTime);


            int frameRate = (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Temp"), "Frame Rate: " + frameRate + "fps", Vector2.Zero, Color.White);

            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
