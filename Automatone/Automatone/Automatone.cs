using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nuclex.Input;
using Nuclex.UserInterface;
using NuclexUserInterfaceExtension;

namespace Automatone
{
    using Screen = Nuclex.UserInterface.Screen;

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

        //GUI
        private InputManager inputManager;
        private GuiManager gui;
        public GuiManager Gui { get { return gui; } }
        private MainScreen mainScreen;

        public const short CONTROLS_AND_GRID_DIVISION = 150;

        //Grid (Visual)
        private GridScreen gridScreen;
        public GridScreen GridScreen { get { return gridScreen; } }
        public const byte GRID_PADDING = 10;

        public const byte X_DIMENSION = 1;
        public const byte Y_DIMENSION = 0;

        //Grid (Logical)
        public String Song { set; get; }
        private CellState[,] songCells;
        public CellState[,] SongCells 
        {
            set
            {
                songCells = value;
            }
            get
            {
                return songCells;
            }
        }

        //Music Stuff
        private double timeSignatureN;
        public double TimeSignatureN { set { timeSignatureN = value; } get { return timeSignatureN; } }
        private double timeSignatureD;
        public double TimeSignatureD { set { timeSignatureD = value; } get { return timeSignatureD; } }
        private int measureLength;
        public int MeasureLength { set { measureLength = value; } get { return measureLength; } }
        private ushort tempo;
        public ushort Tempo 
        {
            set 
            {
                tempo = value;
                audioService.BeatsPerMinute = value;
            }

            get { return tempo; }
        }

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

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += delegate { gridScreen.SetPitchTimeLabelRectangles(); };
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
            previousSequencerState = Sequencer.MidiPlayerState.STOPPED;

            // Setup MIDI routing
            sequencer.OutputDevice = synthesizer;

            //Setup Gui Screen
            Screen screen = new Screen(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            gui.Screen = screen;
            mainScreen = new MainScreen(this);
            screen.Desktop.Children.Add(mainScreen);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

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
            sw.WriteLine("0 TimeSig " + timeSignatureN + "/" + timeSignatureD + " 24 8");
            //sw.WriteLine("0 TimeSig 4/4 24 8");
            sw.WriteLine("0 Tempo " + (TEMPO_DIVIDEND / Tempo));
            sw.WriteLine("0 KeySig 0 major");
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
            //txt2midi.Kill();//it now kills itself
        }

        public void StopSongPlaying()
        {
            Sequencer.StopMidi();
            GridScreen.ResetScrolling();
            mainScreen.ResetPlayButton();
        }
    }
}
