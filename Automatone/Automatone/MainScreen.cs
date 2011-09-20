//#define USESEED

using System;
using Duet.Audio_System;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;

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

        private NamedButtonControl randomizeButton;
        private OptionControl playPauseButton;
        private NamedButtonControl stopButton;
        
        private void InitializeComponent()
        {
            Bounds = new UniRectangle(0, 0, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT);
            EnableDragging = false;

            // Construct children
            randomizeButton = new NamedButtonControl();
            playPauseButton = new OptionControl();
            stopButton = new NamedButtonControl();

            //
            // randomizeButton
            //
            randomizeButton.Bounds = new UniRectangle(10, 30, 163, 48);
            randomizeButton.Pressed += new EventHandler(RandomizeButtonPressed);
            randomizeButton.Name = "randomize";

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
            stopButton.Pressed += new EventHandler(stopButtonPressed);
            stopButton.Name = "stop";

            //
            // Add Children
            //
            Children.Add(randomizeButton);
            Children.Add(playPauseButton);
            Children.Add(stopButton);
        }

        private void PlayPauseButtonChanged(object sender, EventArgs e)
        {
            if (automatone.SongCells != null)
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
            else
            {
                playPauseButton.Selected = false;
            }
        }

        private void RandomizeButtonPressed(object sender, EventArgs e)
        {
            stopButtonPressed(sender, e);

#if USESEED
            automatone.SongCells = SongGenerator.GenerateSong(automatone, new Random(SEED), new ClassicalTheory(), GetUserInput());
#else
            automatone.SongCells = SongGenerator.GenerateSong(automatone, new Random(), new ClassicalTheory(), GetUserInput());
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

        private void stopButtonPressed(object sender, EventArgs e)
        {
            automatone.Sequencer.StopMidi();
            automatone.GridScreen.Reset();
            automatone.GridScreen.ScrollWithMidi = false;
            playPauseButton.Selected = false;
        }

        private InputParameters GetUserInput()
        {
            InputParameters input = new InputParameters();

            //Song Parameters
            input.songSpeed = 0.4;
            input.songSpeedVariance = 0.5;
            input.timeSignatureN = 4.0;
            input.timeSignatureD = 4.0;
            input.meanSongLength = 0.5;
            input.structuralVariance = 0.2;
            input.songRhythmVariance = 0.5;
            input.songMelodyVariance = 0.5;
            input.songLengthVariance = 0.5;

            //Verse Parameters
            input.meanVerseLength = 0.5;
            input.verseLengthVariance = 0.5;
            input.verseRhythmVariance = 0.5;
            input.verseMelodyVariance = 0.5;

            //Phrase Parameters
            input.meanPhraseLength = 0.5;
            input.phraseLengthVariance = 0.5;
            input.phraseRhythmVariance = 0.5;
            input.phraseMelodyVariance = 0.5;
            //input.phraseDistinctiveness = 0.5; //not yet using this?

            //Measure Parameters
            input.measureRhythmVariance = 0;
            input.measureMelodyVariance = 0;

            //Note Parameters
            input.meanNoteLength = 0.5;
            input.noteLengthVariance = 0.5;

            //Rhythm
            input.rhythmObedience = 0.8;
        
            //Melody
            input.chordalityObedience = 0.9;
            input.tonalityObedience = 0.9;
            input.meanPitchContiguity = 0.5;

            //Harmony
            input.seventhChordProbability = 0.1;
            /*input.meanBeatharmonicCovariance = 0.9;
            input.beatHarmonicCovarianceOffsetDivisor = 10;
            input.randomModulationProbability = 0.01;
            input.perfectFifthModulationProbability = 0.20;
            input.perfectFourthModulationProbability = 0.15;
            input.relativeModeModulationProbability = 0.1;
            input.absoluteModeModulationProbability = 0.04;*/

            return input;
        }
    }
}
