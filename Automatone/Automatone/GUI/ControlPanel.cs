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
        private static ControlPanel instance;
        public static ControlPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = new ControlPanel();
                return instance;
            }
        }

        private ControlPanel() : base()
        {
            InitializeComponent();
            Automatone.Instance.Window.ClientSizeChanged += delegate { Bounds = LayoutManager.ControlPanelBounds; };
        }

        private SkinNamedButtonControl generateSongButton;
        private OptionControl playPauseButton;
        private SkinNamedButtonControl stopButton;
        private SkinNamedButtonControl saveButton;
        private SkinNamedButtonControl openButton;
        private SkinNamedButtonControl newButton;
        private SkinNamedButtonControl addCellsButton;
        private SkinNamedButtonControl copyButton;
        private SkinNamedButtonControl cutButton;
        private SkinNamedButtonControl pasteButton;
        private SkinNamedButtonControl redoButton;
        private SkinNamedButtonControl removeCellsButton;
        private SkinNamedButtonControl undoButton;
        
        private void InitializeComponent()
        {
            Bounds = LayoutManager.ControlPanelBounds;
            EnableDragging = false;

            // Construct children
            playPauseButton = new OptionControl();
            stopButton = new SkinNamedButtonControl();
            newButton = new SkinNamedButtonControl();
            openButton = new SkinNamedButtonControl();
            cutButton = new SkinNamedButtonControl();
            saveButton = new SkinNamedButtonControl();
            copyButton = new SkinNamedButtonControl();
            pasteButton = new SkinNamedButtonControl();
            undoButton = new SkinNamedButtonControl();
            redoButton = new SkinNamedButtonControl();
            addCellsButton = new SkinNamedButtonControl();
            removeCellsButton = new SkinNamedButtonControl();
            generateSongButton = new SkinNamedButtonControl();

            //
            // playPauseButton
            //
            playPauseButton.Bounds = LayoutManager.PlayPauseButtonBounds;
            playPauseButton.Changed += new EventHandler(PlayPauseButtonToggled);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            stopButton.Bounds = LayoutManager.StopButtonBounds;
            stopButton.Pressed += new EventHandler(StopButtonPressed);
            stopButton.SkinName = "stop";

            //
            // newButton
            //
            newButton.Bounds = LayoutManager.NewButtonBounds;
            newButton.Pressed += new EventHandler(NewButtonPressed);
            newButton.SkinName = "new";

            //
            // openButton
            //
            openButton.Bounds = LayoutManager.OpenButtonBounds;
            openButton.Pressed += new EventHandler(OpenButtonPressed);
            openButton.SkinName = "open";

            //
            // saveButton
            //
            saveButton.Bounds = LayoutManager.SaveButtonBounds;
            saveButton.Pressed += new EventHandler(SaveButtonPressed);
            saveButton.SkinName = "save";

            //
            // cutButton
            //
            cutButton.Bounds = LayoutManager.CutButtonBounds;
            cutButton.Pressed += new EventHandler(CutButtonPressed);
            cutButton.SkinName = "cut";
            
            //
            // copyButton
            //
            copyButton.Bounds = LayoutManager.CopyButtonBounds;
            copyButton.Pressed += new EventHandler(CopyButtonPressed);
            copyButton.SkinName = "copy";
            
            //
            // pasteButton
            //
            pasteButton.Bounds = LayoutManager.PasteButtonBounds;
            pasteButton.Pressed += new EventHandler(PasteButtonPressed);
            pasteButton.SkinName = "paste";

            //
            // undoButton
            //
            undoButton.Bounds = LayoutManager.UndoButtonBounds;
            undoButton.Pressed += new EventHandler(UndoButtonPressed);
            undoButton.SkinName = "undo";
            
            //
            // redoButton
            //
            redoButton.Bounds = LayoutManager.RedoButtonBounds;
            redoButton.Pressed += new EventHandler(RedoButtonPressed);
            redoButton.SkinName = "redo";

            //
            // addButton
            //
            addCellsButton.Bounds = LayoutManager.AddButtonBounds;
            addCellsButton.Pressed += new EventHandler(AddButtonPressed);
            addCellsButton.SkinName = "add.cells";

            //
            // removeButton
            //
            removeCellsButton.Bounds = LayoutManager.RemoveButtonBounds;
            removeCellsButton.Pressed += new EventHandler(RemoveButtonPressed);
            removeCellsButton.SkinName = "remove.cells";

            //
            // generateSongButton
            //
            generateSongButton.Bounds = LayoutManager.GenerateSongButtonBounds;
            generateSongButton.Pressed += new EventHandler(RandomizeButtonPressed);
            generateSongButton.SkinName = "generate.song";

            //
            // Add Children
            //
            Children.Add(playPauseButton);
            Children.Add(stopButton);
            Children.Add(newButton);
            Children.Add(openButton);
            Children.Add(saveButton);
            Children.Add(cutButton);
            Children.Add(copyButton);
            Children.Add(pasteButton);
            Children.Add(undoButton);
            Children.Add(redoButton);
            Children.Add(addCellsButton);
            Children.Add(removeCellsButton);
            Children.Add(generateSongButton);
        }

        public void ResetPlayButton()
        {
            playPauseButton.Selected = false;
        }

        private void PlayPauseButtonToggled(object sender, EventArgs e)
        {
            if (GridPanel.Instance.SongCells != null)
            {
                if (playPauseButton.Selected)
                {
                    if (Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED)
                        Automatone.Instance.RewriteSong();
                    Automatone.Instance.Sequencer.PlayMidi();
                }
                else
                {
                    Automatone.Instance.Sequencer.PauseMidi();
                }

                GridPanel.Instance.ScrollWithMidi = playPauseButton.Selected;
            }
            else
            {
                playPauseButton.Selected = false;
            }
        }

        private void StopButtonPressed(object sender, EventArgs e)
        {
            Automatone.Instance.StopSongPlaying();
        }

        private void NewButtonPressed(object sender, EventArgs e)
        {
            Automatone.Instance.StopSongPlaying();
            Automatone.Instance.MeasureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * InputParameters.Instance.TimeSignature);
            GridPanel.Instance.SongCells = new CellState[Automatone.PIANO_SIZE, Automatone.NEW_SONG_LENGTH];
            GridPanel.Instance.ResetGridView();
        }

        private void OpenButtonPressed(object sender, EventArgs e)
        {
            Automatone.Instance.StopSongPlaying();
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
                    Automatone.Instance.MeasureLength = (int)formatter.Deserialize(loadStream);
                    GridPanel.Instance.SongCells = (CellState[,])formatter.Deserialize(loadStream);
                    loadStream.Close();
                }
            }

            GridPanel.Instance.ResetGridView();
        }

        private void SaveButtonPressed(object sender, EventArgs e)
        {
            if (Automatone.Instance.Song != null)
            {
                Stream saveStream;
                SaveFileDialog projectSaveDialog = new SaveFileDialog();

                projectSaveDialog.Filter = "Automatone Project (*.aut)|*.aut";
                projectSaveDialog.RestoreDirectory = true;

                if (projectSaveDialog.ShowDialog() == DialogResult.OK && (saveStream = projectSaveDialog.OpenFile()) != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(saveStream, InputParameters.Instance);
                    formatter.Serialize(saveStream, Automatone.Instance.MeasureLength);
                    Automatone.Instance.RewriteSong();
                    formatter.Serialize(saveStream, GridPanel.Instance.SongCells);
                    saveStream.Close();
                }
            }
        }

        private void CutButtonPressed(object sender, EventArgs e)
        {

        }

        private void CopyButtonPressed(object sender, EventArgs e)
        {

        }

        private void PasteButtonPressed(object sender, EventArgs e)
        {

        }

        private void UndoButtonPressed(object sender, EventArgs e)
        {

        }

        private void RedoButtonPressed(object sender, EventArgs e)
        {

        }
        
        private void AddButtonPressed(object sender, EventArgs e)
        {

        }

        private void RemoveButtonPressed(object sender, EventArgs e)
        {

        }

        private void RandomizeButtonPressed(object sender, EventArgs e)
        {
            Automatone.Instance.StopSongPlaying();
            GetUserInput();

#if USESEED
            GridPanel.Instance.SongCells = SongGenerator.GenerateSong(Automatone.Instance, new Random(SEED), new ClassicalTheory());
#else
            GridPanel.Instance.SongCells = SongGenerator.GenerateSong(Automatone.Instance, new Random(), new ClassicalTheory());
#endif      
            GridPanel.Instance.ResetGridView();
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
