using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.UserInterface;
using Nuclex.Input;

using Duet.Audio_System;

namespace Automatone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Automatone : Microsoft.Xna.Framework.Game
    {
        // Content Objects
        private ContentManager content;

        // Graphics Objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch { get { return spriteBatch; } }

        // Audio Objects
        private Synthesizer synthesizer;
        private Sequencer sequencer;

        public Sequencer Sequencer { get { return sequencer; } }

        // Services
        private IAudioSystemService audioService;

        //GUI
        private InputManager inputManager;
        private GuiManager gui;

        public const short SCREEN_WIDTH = 800;
        public const short SCREEN_HEIGHT = 600;
        public const short CONTROLS_AND_GRID_DIVISION = 150;

        //Grid (Visual)
        private GridScreen gridScreen;
        public GridScreen GridScreen { get { return gridScreen; } }

        public const byte X_DIMENSION = 1;
        public const byte Y_DIMENSION = 0;

        //Grid (Logical)
        public CellState[,] SongCells { set; get; }

        //Music Stuff
        public const ushort TEMPO = 120;

        public const int SUBBEATS_PER_MEASURE = 16;
        public static double getBeatResolution()
        {
            return 1.0 / SUBBEATS_PER_MEASURE;
        }

        public Automatone()
        {
            graphics = new GraphicsDeviceManager(this);

            content = new ContentManager(Services);
            Content.RootDirectory = "Content";

            inputManager = new InputManager(Services);
            Components.Add(inputManager);

            gui = new GuiManager(Services);
            Components.Add(gui);

            gridScreen = new GridScreen(this);
            Components.Add(gridScreen);

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

            //Setup Tempo
            audioService.BeatsPerMinute = TEMPO;

            //Setup Gui Screen
            Screen screen = new Screen(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            gui.Screen = screen;
            screen.Desktop.Children.Add(new MainScreen(this));
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
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

            int frameRate = (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Temp"), "Frame Rate: " + frameRate + "fps", Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
