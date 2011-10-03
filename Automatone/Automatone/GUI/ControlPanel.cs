//#define USESEED

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Duet.Audio_System;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using NuclexUserInterfaceExtension;
using Automatone.Theories;

namespace Automatone.GUI
{
    public class ControlPanel : WindowControl
    {
        private Automatone automatone;

        public ControlPanel(Automatone parent) : base()
        {
            automatone = parent;
            InitializeComponent();
        }

        private SkinNamedButtonControl randomizeButton;
        private OptionControl playPauseButton;
        private SkinNamedButtonControl stopButton;
        private SkinNamedButtonControl saveButton;
        private SkinNamedButtonControl loadButton;
        private SkinNamedButtonControl newButton;
        
        private void InitializeComponent()
        {
            Bounds = LayoutManager.ControlPanelBounds;
            EnableDragging = false;

            // Construct children
            randomizeButton = new SkinNamedButtonControl();
            playPauseButton = new OptionControl();
            stopButton = new SkinNamedButtonControl();
            saveButton = new SkinNamedButtonControl();
            loadButton = new SkinNamedButtonControl();
            newButton = new SkinNamedButtonControl();

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
            playPauseButton.Changed += new EventHandler(PlayPauseButtonToggled);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            stopButton.Bounds = new UniRectangle(55, 55, 43, 43);
            stopButton.Pressed += new EventHandler(StopButtonPressed);
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
            // newButton
            //
            newButton.Bounds = new UniRectangle(10, 100, 43, 43);
            newButton.Pressed += new EventHandler(NewButtonPressed);
            newButton.SkinName = "stop";

            //
            // Add Children
            //
            Children.Add(randomizeButton);
            Children.Add(playPauseButton);
            Children.Add(stopButton);
            Children.Add(saveButton);
            Children.Add(loadButton);
            Children.Add(newButton);
        }

        public void ResetPlayButton()
        {
            playPauseButton.Selected = false;
        }

        private void NewButtonPressed(object sender, EventArgs e)
        {
            automatone.StopSongPlaying();
            automatone.MeasureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * InputParameters.Instance.TimeSignature);
            automatone.GridPanel.SongCells = new CellState[Automatone.PIANO_SIZE, 45];
            GridPanel.Instance.ResetGridView();
        }

        private void LoadButtonPressed(object sender, EventArgs e)
        {
            automatone.StopSongPlaying();
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
                    automatone.GridPanel.SongCells = (CellState[,])formatter.Deserialize(loadStream);
                    loadStream.Close();
                }
            }

            GridPanel.Instance.ResetGridView();
        }

        private void SaveButtonPressed(object sender, EventArgs e)
        {
            if (automatone.Song != null)
            {
                Stream saveStream;
                SaveFileDialog projectSaveDialog = new SaveFileDialog();

                projectSaveDialog.Filter = "Automatone Project (*.aut)|*.aut";
                projectSaveDialog.RestoreDirectory = true;

                if (projectSaveDialog.ShowDialog() == DialogResult.OK && (saveStream = projectSaveDialog.OpenFile()) != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(saveStream, InputParameters.Instance);
                    formatter.Serialize(saveStream, automatone.MeasureLength);
                    automatone.RewriteSong();
                    formatter.Serialize(saveStream, automatone.Song);
                    formatter.Serialize(saveStream, automatone.GridPanel.SongCells);
                    saveStream.Close();
                }
            }
        }

        private void PlayPauseButtonToggled(object sender, EventArgs e)
        {
            if (automatone.GridPanel.SongCells != null)
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

                automatone.GridPanel.ScrollWithMidi = playPauseButton.Selected;
            }
            else
            {
                playPauseButton.Selected = false;
            }
        }

        private void RandomizeButtonPressed(object sender, EventArgs e)
        {
            automatone.StopSongPlaying();
            GetUserInput();

#if USESEED
            automatone.SongCells = SongGenerator.GenerateSong(automatone, new Random(SEED), new ClassicalTheory());
#else
            automatone.GridPanel.SongCells = SongGenerator.GenerateSong(automatone, new Random(), new ClassicalTheory());
#endif      
            GridPanel.Instance.ResetGridView();
        }

        private void StopButtonPressed(object sender, EventArgs e)
        {
            automatone.StopSongPlaying();
        }

        private void GetUserInput()
        {
            InputParameters inputParameters = InputParameters.Instance;

            //Song Parameters
            inputParameters.tempo = 120;
            inputParameters.timeSignatureN = 4;
            inputParameters.timeSignatureD = 4;

            //Song Parameters
            inputParameters.meanSongLength = 0.5;
            inputParameters.structuralVariance = 0.5;
            inputParameters.songRhythmVariance = 0.5;
            inputParameters.songMelodyVariance = 0.5;
            inputParameters.songLengthVariance = 0.5;

            //Verse Parameters
            inputParameters.meanVerseLength = 0.5;
            inputParameters.verseLengthVariance = 0.5;
            inputParameters.verseRhythmVariance = 0.5;
            inputParameters.verseMelodyVariance = 0.5;

            //Phrase Parameters
            inputParameters.meanPhraseLength = 0.5;
            inputParameters.phraseLengthVariance = 0.5;
            inputParameters.phraseRhythmVariance = 0.5;
            inputParameters.phraseMelodyVariance = 0.5;

            //Measure Parameters
            inputParameters.measureRhythmVariance = 0.5;
            inputParameters.measureMelodyVariance = 0.5;
            
            //Part Parameters
            inputParameters.homophony = 1;
            inputParameters.polyphony = 0.4;
            inputParameters.beatDefinition = 0.5;
            //Per-part Parameters
            inputParameters.meanPartRhythmCrowdedness = 0.5;
            inputParameters.partRhythmCrowdednessVariance = 0.5;
            inputParameters.partNoteLengthVariance = 0.5;
            inputParameters.meanPartOctaveRange = 0.5;
            inputParameters.partOctaveRangeVariance = 0.5;

            //Note Parameters
            inputParameters.meanNoteLength = 0.5;
            inputParameters.noteLengthVariance = 0.5;

            //Rhythm
            inputParameters.rhythmObedience = 0.8;
        
            //Melody
            inputParameters.chordalityObedience = 0.95;
            inputParameters.tonalityObedience = 0.95;
            inputParameters.meanPitchContiguity = 0.5;

            //Harmony
            inputParameters.seventhChordProbability = 0.1;
        }
    }
}
