//#define USESEED

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
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

        private SkinNamedButtonControl randomizeButton;
        private OptionControl playPauseButton;
        private SkinNamedButtonControl stopButton;
        private SkinNamedButtonControl saveButton;
        private SkinNamedButtonControl loadButton;
        
        private void InitializeComponent()
        {
            Bounds = new UniRectangle(0, 0, Automatone.SCREEN_WIDTH, Automatone.SCREEN_HEIGHT);
            EnableDragging = false;

            // Construct children
            randomizeButton = new SkinNamedButtonControl();
            playPauseButton = new OptionControl();
            stopButton = new SkinNamedButtonControl();
            saveButton = new SkinNamedButtonControl();
            loadButton = new SkinNamedButtonControl();

            //
            // randomizeButton
            //
            randomizeButton.Bounds = new UniRectangle(10, 10, 163, 48);
            randomizeButton.Pressed += new EventHandler(RandomizeButtonPressed);
            randomizeButton.SkinName = "randomize";

            //
            // playPauseButton
            //
            playPauseButton.Bounds = new UniRectangle(10, 55, 43, 43);
            playPauseButton.Changed += new EventHandler(PlayPauseButtonChanged);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            stopButton.Bounds = new UniRectangle(55, 55, 43, 43);
            stopButton.Pressed += new EventHandler(stopButtonPressed);
            stopButton.SkinName = "stop";

            //
            // saveButton
            //
            saveButton.Bounds = new UniRectangle(55, 100, 43, 43);
            saveButton.Pressed += new EventHandler(SaveButtonPressed);
            saveButton.SkinName = "stop";

            //
            // loadButton
            //
            loadButton.Bounds = new UniRectangle(100, 100, 43, 43);
            loadButton.Pressed += new EventHandler(LoadButtonPressed);
            loadButton.SkinName = "stop";

            //
            // Add Children
            //
            Children.Add(randomizeButton);
            Children.Add(playPauseButton);
            Children.Add(stopButton);
            Children.Add(saveButton);
            Children.Add(loadButton);
        }

        private void LoadButtonPressed(object sender, EventArgs e)
        {
            Stream loadStream;
            OpenFileDialog projectLoadDialog = new OpenFileDialog();

            projectLoadDialog.Filter = "Automatone Project (*.aut)|*.aut";
            projectLoadDialog.RestoreDirectory = true;

            if (projectLoadDialog.ShowDialog() == DialogResult.OK)
            {
                if ((loadStream = projectLoadDialog.OpenFile()) != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    InputParameters.LoadInstance((InputParameters)formatter.Deserialize(loadStream));
                    automatone.MeasureLength = (int)formatter.Deserialize(loadStream);
                    automatone.Song = (string)formatter.Deserialize(loadStream);
                    automatone.SongCells = (CellState[,])formatter.Deserialize(loadStream);
                    automatone.Tempo = (ushort)formatter.Deserialize(loadStream);
                    automatone.TimeSignatureD = (double)formatter.Deserialize(loadStream);
                    automatone.TimeSignatureN = (double)formatter.Deserialize(loadStream);
                    loadStream.Close();
                }
            }
        }

        private void SaveButtonPressed(object sender, EventArgs e)
        {
            if (automatone.SongCells != null)
            {
                Stream saveStream;
                SaveFileDialog projectSaveDialog = new SaveFileDialog();

                projectSaveDialog.Filter = "Automatone Project (*.aut)|*.aut";
                projectSaveDialog.RestoreDirectory = true;

                if (projectSaveDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((saveStream = projectSaveDialog.OpenFile()) != null)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(saveStream, InputParameters.Instance);
                        formatter.Serialize(saveStream, automatone.MeasureLength);
                        automatone.RewriteSong();
                        formatter.Serialize(saveStream, automatone.Song);
                        formatter.Serialize(saveStream, automatone.SongCells);
                        formatter.Serialize(saveStream, automatone.Tempo);
                        formatter.Serialize(saveStream, automatone.TimeSignatureD);
                        formatter.Serialize(saveStream, automatone.TimeSignatureN);
                        saveStream.Close();
                    }
                }
            }
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
            InputParameters input = InputParameters.Instance;

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

            //Measure Parameters
            input.measureRhythmVariance = 0.5;
            input.measureMelodyVariance = 0.5;
            
            //Part Parameters
            input.homophony = 1;
            input.polyphony = 0.4;
            input.beatDefinition = 0.5;
            //Per-part Parameters
            input.meanPartRhythmCrowdedness = 0.5;
            input.partRhythmCrowdednessVariance = 0.5;
            input.partNoteLengthVariance = 0.5;
            input.meanPartOctaveRange = 0.5;
            input.partOctaveRangeVariance = 0.5;
            input.forceChordChance = 0;
            input.forceDiatonicChance = 0;

            //Note Parameters
            input.meanNoteLength = 0.5;
            input.noteLengthVariance = 0.5;

            //Rhythm
            input.rhythmObedience = 0.8;
        
            //Melody
            input.chordalityObedience = 0.95;
            input.tonalityObedience = 0.95;
            input.meanPitchContiguity = 0.2;

            //Harmony
            input.seventhChordProbability = 0.1;

            return input;
        }
    }
}
