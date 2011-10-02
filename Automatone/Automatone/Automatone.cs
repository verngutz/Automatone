using System;
using System.Diagnostics;
using System.IO;

using Duet.Audio_System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Input;
using Nuclex.UserInterface;
using NuclexUserInterfaceExtension;

using Automatone.GUI;

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
        private Sequencer.MidiPlayerState previousSequencerState;

        // Services
        private IAudioSystemService audioService;
        public IAudioSystemService AudioService { get { return audioService; } }

        //GUI
        private InputManager inputManager;
        private GuiManager gui;
        public GuiManager Gui { get { return gui; } }
        private ControlPanel controlPanel;

        public const short CONTROLS_AND_GRID_DIVISION = 150;

        //Grid (Visual)
        private GridPanel gridPanel;
        public GridPanel GridPanel { get { return gridPanel; } }
        public const byte GRID_PADDING = 10;

        public const byte X_DIMENSION = 1;
        public const byte Y_DIMENSION = 0;

        //Grid (Logical)
        public String Song { set; get; }
        

        //Music Stuff
        private int measureLength;
        public int MeasureLength { set { measureLength = value; } get { return measureLength; } }

        public const int SUBBEATS_PER_WHOLE_NOTE = 16;
        public static double getBeatResolution()
        {
            return 1.0 / SUBBEATS_PER_WHOLE_NOTE;
        }

        private const int SEED = 40;
        private const int TEMPO_DIVIDEND = 60000000;

        //Pitch Range and Offset
        public const int PIANO_SIZE = 61;
        public const int LOWEST_NOTE_CHROMATIC_NUMBER = 6;

        private static Automatone instance;
        public static Automatone Instance
        {
            get
            {
                if (instance == null)
                    instance = new Automatone();
                return instance;
            }
        }

        private Automatone()
        {
            graphics = new GraphicsDeviceManager(this);

            content = new ContentManager(Services);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.Title = "Automatone";
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set Preferred Resolution
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            // Initialize Input Handler
            inputManager = new InputManager(Services);
            Components.Add(inputManager);

            // Initialize GUI
            gui = new GuiManager(Services);
            Components.Add(gui);
            Screen screen = new Screen(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            gui.Screen = screen;

            // Create control panel
            controlPanel = new ControlPanel(this);
            screen.Desktop.Children.Add(controlPanel);

            // Create grid panel
            gridPanel = GridPanel.Instance;
            Components.Add(gridPanel);

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
            previousSequencerState = Sequencer.MidiPlayerState.STOPPED;

            // Setup MIDI routing
            sequencer.OutputDevice = synthesizer;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gui.Visualizer = MultiGuiVisualizer.FromFile(Services, "Content\\Automatone.skin.xml");
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
            if (previousSequencerState == Sequencer.MidiPlayerState.PLAYING && sequencer.State == Sequencer.MidiPlayerState.STOPPED)
            {
                StopSongPlaying();
            }
            previousSequencerState = sequencer.State;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        public void RewriteSong()
        {
            Song = SongGenerator.WriteSong(this);

            StreamWriter sw = new StreamWriter("sample.mtx");
            sw.WriteLine("MFile 1 2 192");
            sw.WriteLine("MTrk");
            sw.WriteLine("0 TimeSig " + InputParameters.Instance.timeSignatureN + "/" + InputParameters.Instance.timeSignatureD + " 24 8");
            //sw.WriteLine("0 TimeSig 4/4 24 8");
            sw.WriteLine("0 Tempo " + (TEMPO_DIVIDEND / InputParameters.Instance.tempo));
            //sw.WriteLine("0 KeySig 0 major");
            sw.WriteLine("0 Meta TrkEnd");
            sw.WriteLine("TrkEnd");
            sw.Write(Song);
            sw.Close();

            if (File.Exists("sample.mid"))
                File.Delete("sample.mid");

            Process txt2midi = new Process();
            System.Console.WriteLine(Environment.CurrentDirectory);
            txt2midi.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            txt2midi.StartInfo.FileName = "Mtx2Midi.exe";
            txt2midi.StartInfo.Arguments = "sample.mtx";
            txt2midi.StartInfo.CreateNoWindow = true;
            txt2midi.Start();
            while(!Sequencer.LoadMidi("sample.mid"));
        }

        public void StopSongPlaying()
        {
            Sequencer.StopMidi();
            GridPanel.ResetScrolling();
            controlPanel.ResetPlayButton();
        }
    }
}
