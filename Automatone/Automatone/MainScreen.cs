#define USESEED

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;

namespace Automatone
{
    public class MainScreen : WindowControl
    {
        private Automatone automatone;

        private const int SEED = 40;
        private const int TEMPO_DIVIDEND = 60000000;

        public MainScreen(Automatone parent) : base()
        {
            automatone = parent;
            InitializeComponent();
        }

        private ButtonControl randomizeButton;
        private OptionControl playPauseButton;
        
        private void InitializeComponent()
        {
            Bounds = new UniRectangle(0, 0, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT);

            // Construct children
            randomizeButton = new ButtonControl();
            playPauseButton = new OptionControl();

            //
            // randomizeButton
            //
            randomizeButton.Bounds = new UniRectangle(10, 30, 117, 45);
            randomizeButton.Text = "Randomize";
            randomizeButton.Pressed += new EventHandler(RandomizeButtonPressed);

            //
            // playPauseButton
            //
            playPauseButton.Bounds = new UniRectangle(10, 75, 45, 42);
            playPauseButton.Changed += new EventHandler(PlayPauseButtonChanged);

            //
            // Add Children
            //
            Children.Add(randomizeButton);
            Children.Add(playPauseButton);
        }

        private void PlayPauseButtonChanged(object sender, EventArgs e)
        {
            if (playPauseButton.Selected)
            {
                automatone.Sequencer.PlayMidi();
            }
            else
            {
                automatone.Sequencer.PauseMidi();
            }

            automatone.GridScreen.ScrollWithMidi = playPauseButton.Selected;
        }

        private void RandomizeButtonPressed(object sender, EventArgs e)
        {
            automatone.Sequencer.StopMidi();
            automatone.GridScreen.ScrollWithMidi = false;

            StreamWriter sw = new StreamWriter("sample.mtx");
            sw.WriteLine("MFile 1 2 192");
            sw.WriteLine("MTrk");
            sw.WriteLine("0 TimeSig 4/4 24 8");
            sw.WriteLine("0 Tempo " + TEMPO_DIVIDEND / Automatone.TEMPO);
            sw.WriteLine("0 KeySig 0 major");
            sw.WriteLine("0 Meta TrkEnd");
            sw.WriteLine("TrkEnd");

            CellState[,] output;
#if USESEED
            String song = SongGenerator.GenerateSong(new Random(SEED), new ClassicalTheory(), out output);
#else
            String song = SongGenerator.GenerateSong(new Random(), new ClassicalTheory(), out automatone.SongCells);
#endif
            automatone.SongCells = output;

            sw.Write(song);
            sw.Close();

            if (File.Exists("sample.mid"))
                File.Delete("sample.mid");

            Process txt2midi = new Process();
            System.Console.WriteLine(Environment.CurrentDirectory);
            txt2midi.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            txt2midi.StartInfo.FileName = "Mtx2Midi.exe";
            txt2midi.StartInfo.Arguments = "sample.mtx";
            txt2midi.StartInfo.UseShellExecute = true;
            txt2midi.Start();
            automatone.Sequencer.LoadMidi("sample.mid");
            txt2midi.Kill();
        }
    }
}
