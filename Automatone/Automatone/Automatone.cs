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
using Duet.Render_System;
using Duet.Input_System;

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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Audio Objects
        Synthesizer synthesizer;
        Sequencer sequencer;

        // Services
        private IAudioSystemService audioService;
        private InputComponent input;

        // Random Shit
        private float tempoChangeSpeed = 0.0f;
        private float tempo;
        private bool buttonDown = false;
        private byte instrument = 0;

        //GUI
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

        //TXT2MIDI
        Process txt2midi;

        public Automatone()
        {
            graphics = new GraphicsDeviceManager(this);
            MSResolution.Init(ref graphics);
            MSResolution.SetVirtualResolution(50 * 16, 150 + 44 * 16);
            MSResolution.SetResolution(50 * 16, 150 + 44 * 16, false);

            content = new ContentManager(Services);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.Title = "Automatone";

            //create midi
            StreamWriter sw = new StreamWriter("Content\\sample.txt");
		    sw.WriteLine("mthd\n\tversion 1\n\tunit 192\nend mthd\n");
		    sw.WriteLine("mtrk\n\ttact 4 / 4 24 8\n\tbeats 140\n\tkey \"Cmaj\"\nend mtrk\n");
		    const int SEED = 40;
		    MSRandom random = new MSRandom(SEED);
		    Theory theory = new BasicWesternTheory(random);
		    SongGenerator sg = new SongGenerator(random);
            String song = sg.generateSong(theory);
            System.Console.WriteLine("Song Making Done");
		    sw.Write(song);
            System.Console.WriteLine("Song Writing Done");
		    sw.Close();

            Process.Start(Environment.CurrentDirectory + "\\Content\\TXT2MIDI.EXE", 
                Environment.CurrentDirectory + "\\Content\\sample.txt" + " " 
                + Environment.CurrentDirectory + "\\Content\\audio\\SAMPLE.MID");
            
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

            synthesizer = new Synthesizer(this);
            synthesizer.WaveBankPath = "Content\\audio\\Wave Bank.xwb";
            synthesizer.SoundBankPath = "Content\\audio\\Sound Bank.xsb";
            synthesizer.m_Patch = content.Load<XACTPatch>(@"content\audio\xact");

            sequencer = new Sequencer(this);

            // Setup MIDI routing
            sequencer.OutputDevice = synthesizer;

            // Load MIDI File
            sequencer.LoadMidi("Content\\audio\\SAMPLE.MID");
            sequencer.PlayMidi();

            tempo = 54.0f;
            audioService.BeatsPerMinute = (ulong)tempo;

            input = new InputComponent(this);
            this.Components.Add(input);

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

            slider = new MSImageHolder(new Rectangle(0, 0, 101, 13), Content.Load<Texture2D>("slider"), spriteBatch, this);
            sliderCursor = new MSImageHolder(new Rectangle(0, 0, 15, 15), Content.Load<Texture2D>("slidercursor"), spriteBatch, this);

            topLeftPanel = new MSPanel(null, new Rectangle(0, 0, 30 * 16, 150), null, Shape.RECTANGULAR, spriteBatch, this);

            MSPanel basicPanel = new MSPanel(Content.Load<Texture2D>("darkblue"), new Rectangle(0, 45, 30 * 16, 105), 6, 6, 25, 15, null, Shape.RECTANGULAR, spriteBatch, this);
            basicPanel.AddComponent(new MSFontScalingLabel("Mood:", new Rectangle(0, 0, 50, 25), Content.Load<SpriteFont>("Temp"), spriteBatch, this), Alignment.TOP_LEFT);
            basicPanel.AddComponent(new MSButton(
                new MSFontScalingLabel("Happy", new Rectangle(50, 0, 50, 25), Content.Load<SpriteFont>("Temp"), spriteBatch, this),
                null,
                new Rectangle(0, 0, 50, 35),
                Content.Load<Texture2D>("radiobutton"),
                Content.Load<Texture2D>("radiobutton"),
                Content.Load<Texture2D>("radiobutton"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this), Alignment.MIDDLE_LEFT);
            basicPanel.AddComponent(new MSButton(
                new MSFontScalingLabel("Sad", new Rectangle(50, 0, 50, 25), Content.Load<SpriteFont>("Temp"), spriteBatch, this),
                null,
                new Rectangle(0, 0, 33, 30),
                Content.Load<Texture2D>("radiobuttonempty"),
                Content.Load<Texture2D>("radiobuttonempty"),
                Content.Load<Texture2D>("radiobuttonempty"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this), Alignment.BOTTOM_LEFT);

            basicPanel.AddComponent(new MSFontScalingLabel("Speed:", new Rectangle(0, 0, 50, 25), Content.Load<SpriteFont>("Temp"), spriteBatch, this), Alignment.TOP_CENTER);
            basicPanel.AddComponent(new MSButton(
                new MSFontScalingLabel("Fast", new Rectangle(50, 0, 50, 25), Content.Load<SpriteFont>("Temp"), spriteBatch, this),
                null,
                new Rectangle(0, 0, 33, 30),
                Content.Load<Texture2D>("radiobuttonempty"),
                Content.Load<Texture2D>("radiobuttonempty"),
                Content.Load<Texture2D>("radiobuttonempty"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this), Alignment.MIDDLE_CENTER);
            basicPanel.AddComponent(new MSButton(
                new MSFontScalingLabel("Slow", new Rectangle(50, 0, 50, 25), Content.Load<SpriteFont>("Temp"), spriteBatch, this),
                null,
                new Rectangle(0, 0, 50, 35),
                Content.Load<Texture2D>("radiobutton"),
                Content.Load<Texture2D>("radiobutton"),
                Content.Load<Texture2D>("radiobutton"),
                null,
                Shape.RECTANGULAR,
                spriteBatch,
                this), Alignment.BOTTOM_CENTER);

            MSPanel expertPanel = new MSPanel(null, new Rectangle(), null, Shape.RECTANGULAR, spriteBatch, this);

            inputTabs = new MSTabbedPanel(topLeftPanel);
            inputTabs.AddTab(new MSTab(
                new MSButton(
                    new MSFontScalingLabel("Basic", new Rectangle(10, 10, 50, 25), Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, this),
                    null,
                    new Rectangle(0, 0, 117, 45),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    this),
                new MSButton(
                    new MSFontScalingLabel("Basic", new Rectangle(10, 10, 50, 25), Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, this),
                    null,
                    new Rectangle(0, 0, 117, 45),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    this),
                true,
                basicPanel));

            inputTabs.AddTab(new MSTab(
                new MSButton(
                    new MSFontScalingLabel("Expert", new Rectangle(10, 10, 50, 25), Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, this),
                    null,
                    new Rectangle(117, 0, 117, 45),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    this),
                new MSButton(
                    new MSFontScalingLabel("Expert", new Rectangle(10, 10, 50, 25), Content.Load<SpriteFont>("Temp"), Color.LightBlue, spriteBatch, this),
                    null,
                    new Rectangle(117, 0, 117, 45),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    Content.Load<Texture2D>("tab"),
                    null,
                    Shape.RECTANGULAR,
                    spriteBatch,
                    this),
                false,
                expertPanel));

            topLeftPanel.AddComponent(basicPanel);

            topRightPanel = new MSPanel(Content.Load<Texture2D>("darkblue"), new Rectangle(30 * 16, 0, 20 * 16, 150), 10, 5, 85, 85, null, Shape.RECTANGULAR, spriteBatch, this);
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
            content.Dispose();
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
            topPanel.Draw(gameTime);
            gridPanel.Draw(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
