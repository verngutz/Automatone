//#define USESEED

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;

using Duet.Audio_System;

namespace Automatone
{
    public class MainScreen : WindowControl
    {
        private Automatone automatone;

        public MainScreen(Automatone parent) : base()
        {
            automatone = parent;
            InitializeComponent();
        }

        private ButtonControl randomizeButton;
        private OptionControl playPauseButton;
        private ChoiceControl stopButton;
        
        private void InitializeComponent()
        {
            Bounds = new UniRectangle(0, 0, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT);
            EnableDragging = false;

            // Construct children
            randomizeButton = new ButtonControl();
            playPauseButton = new OptionControl();
            stopButton = new ChoiceControl();

            //
            // randomizeButton
            //
            randomizeButton.Bounds = new UniRectangle(10, 30, 163, 48);
            randomizeButton.Text = "Randomize";
            randomizeButton.Pressed += new EventHandler(RandomizeButtonPressed);

            //
            // playPauseButton
            //
            playPauseButton.Bounds = new UniRectangle(10, 75, 43, 43);
            playPauseButton.Changed += new EventHandler(PlayPauseButtonChanged);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            stopButton.Bounds = new UniRectangle(55, 75, 43, 43);
            stopButton.Changed += new EventHandler(StopButtonChanged);
            stopButton.Selected = false;

            //
            // Add Children
            //
            Children.Add(randomizeButton);
            Children.Add(playPauseButton);
            Children.Add(stopButton);
        }

        private void PlayPauseButtonChanged(object sender, EventArgs e)
        {
            if (playPauseButton.Selected)
            {
                if (automatone.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
                    automatone.RewriteSong();
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
            StopButtonChanged(sender, e);

#if USESEED
            automatone.SongCells = SongGenerator.GenerateSong(automatone, new Random(SEED), new ClassicalTheory());
#else
            automatone.SongCells = SongGenerator.GenerateSong(automatone, new Random(), new ClassicalTheory());
#endif
            //automatone.Tempo = (ushort)(40 + InputParameters.songSpeed * 120);

            /**
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    myStream.Close();
                }
            }
             */
        }

        private void StopButtonChanged(object sender, EventArgs e)
        {
            automatone.Sequencer.StopMidi();
            automatone.GridScreen.Reset();
            automatone.GridScreen.ScrollWithMidi = false;
            playPauseButton.Selected = false;
            stopButton.Selected = false;
        }
    }
}
