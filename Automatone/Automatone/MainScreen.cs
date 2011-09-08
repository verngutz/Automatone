//#define USESEED

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

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

            CellState[,] output;
#if USESEED
            SongGenerator.GenerateSong(automatone, new Random(SEED), new ClassicalTheory(), out output);
#else
            SongGenerator.GenerateSong(automatone, new Random(), new ClassicalTheory(), out output);
#endif
            automatone.SongCells = output;
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
