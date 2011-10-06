//#define USESEED

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Duet.Audio_System;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using NuclexUserInterfaceExtension;
using Automatone.Theories;
using Microsoft.Xna.Framework;

namespace Automatone.GUI
{
    public class ControlPanel : SkinNamedWindowControl
    {
        private SkinNamedButtonControl arrowLeftButton;
        private SkinNamedButtonControl arrowRightButton;
        private SkinNamedButtonControl generateSongButton;
        private OptionControl playPauseButton;
        private SkinNamedButtonControl stopButton;
        private SkinNamedButtonControl newButton;
        private SkinNamedButtonControl openButton;
        private SkinNamedButtonControl saveButton;
        private SkinNamedButtonControl exportButton;
        private SkinNamedButtonControl cutButton;
        private SkinNamedButtonControl copyButton;
        private SkinNamedButtonControl pasteButton;
        private SkinNamedButtonControl undoButton;
        private SkinNamedButtonControl redoButton;
        private SkinNamedButtonControl addCellsButton;
        private SkinNamedButtonControl removeCellsButton;

        public UniRectangle ArrowLeftBounds { set { arrowLeftButton.Bounds = value; } }
        public UniRectangle ArrowRightBounds { set { arrowRightButton.Bounds = value; } }

        private List<UniRectangle> buttonsetBounds;

        private int firstButton;

        private ControlPanel() : base()
        {
            InitializeComponent();
        }

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

        private void InitializeComponent()
        {
            Bounds = LayoutManager.Instance.ControlPanelBounds;
            EnableDragging = false;
            SkinName = "control.panel";

            // Construct children
            arrowLeftButton = new SkinNamedButtonControl();
            arrowRightButton = new SkinNamedButtonControl();
            generateSongButton = new SkinNamedButtonControl();
            playPauseButton = new OptionControl();
            stopButton = new SkinNamedButtonControl();
            newButton = new SkinNamedButtonControl();
            openButton = new SkinNamedButtonControl();
            saveButton = new SkinNamedButtonControl();
            exportButton = new SkinNamedButtonControl();
            cutButton = new SkinNamedButtonControl();
            copyButton = new SkinNamedButtonControl();
            pasteButton = new SkinNamedButtonControl();
            undoButton = new SkinNamedButtonControl();
            redoButton = new SkinNamedButtonControl();
            addCellsButton = new SkinNamedButtonControl();
            removeCellsButton = new SkinNamedButtonControl();

            buttonsetBounds = LayoutManager.Instance.GetButtonsetBounds(14, 0);
            ResetButtonsetBounds();

            //
            // arrowLeftButton
            //
            arrowLeftButton.Bounds = LayoutManager.Instance.ControlArrowLeftBounds;
            arrowLeftButton.Pressed += new EventHandler(ArrowLeftPressed);
            arrowLeftButton.SkinName = "arrow.left";

            //
            // arrowRightButton
            //
            arrowRightButton.Bounds = LayoutManager.Instance.ControlArrowRightBounds;
            arrowRightButton.Pressed += new EventHandler(ArrowRightPressed);
            arrowRightButton.SkinName = "arrow.right";

            //
            // generateSongButton
            //
            generateSongButton.Pressed += new EventHandler(GenerateSongButtonPressed);
            generateSongButton.SkinName = "generate.song";

            //
            // playPauseButton
            //
            playPauseButton.Changed += new EventHandler(PlayPauseButtonToggled);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            stopButton.Pressed += new EventHandler(StopButtonPressed);
            stopButton.SkinName = "stop";

            //
            // newButton
            //
            newButton.Pressed += new EventHandler(NewButtonPressed);
            newButton.SkinName = "new";

            //
            // openButton
            //
            openButton.Pressed += new EventHandler(OpenButtonPressed);
            openButton.SkinName = "open";

            //
            // saveButton
            //
            saveButton.Pressed += new EventHandler(SaveButtonPressed);
            saveButton.SkinName = "save";

            //
            // exportButton
            //
            exportButton.Pressed += new EventHandler(ExportButtonPressed);
            exportButton.SkinName = "export";

            //
            // cutButton
            //
            cutButton.Pressed += new EventHandler(CutButtonPressed);
            cutButton.SkinName = "cut";
            
            //
            // copyButton
            //
            copyButton.Pressed += new EventHandler(CopyButtonPressed);
            copyButton.SkinName = "copy";
            
            //
            // pasteButton
            //
            pasteButton.Pressed += new EventHandler(PasteButtonPressed);
            pasteButton.SkinName = "paste";

            //
            // undoButton
            //
            undoButton.Pressed += new EventHandler(UndoButtonPressed);
            undoButton.SkinName = "undo";
            
            //
            // redoButton
            //
            redoButton.Pressed += new EventHandler(RedoButtonPressed);
            redoButton.SkinName = "redo";

            //
            // addButton
            //
            addCellsButton.Pressed += new EventHandler(AddCellsButtonPressed);
            addCellsButton.SkinName = "add.cells";

            //
            // removeButton
            //
            removeCellsButton.Pressed += new EventHandler(RemoveCellsButtonPressed);
            removeCellsButton.SkinName = "remove.cells";

            //
            // Add Children
            //
            Children.Add(arrowLeftButton);
            Children.Add(arrowRightButton);

            Children.Add(generateSongButton);
            Children.Add(playPauseButton);
            Children.Add(stopButton);
            Children.Add(newButton);
            Children.Add(openButton);
            Children.Add(saveButton);
            Children.Add(exportButton);
            Children.Add(cutButton);
            Children.Add(copyButton);
            Children.Add(pasteButton);
            Children.Add(undoButton);
            Children.Add(redoButton);
            Children.Add(addCellsButton);
            Children.Add(removeCellsButton);
        }

        public void ResetButtonsetBounds()
        {
            firstButton = (int)MathHelper.Clamp(firstButton, 0, buttonsetBounds.Count - LayoutManager.Instance.VisibleButtonCount);
            buttonsetBounds = LayoutManager.Instance.GetButtonsetBounds(buttonsetBounds.Count, firstButton);
            generateSongButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(0);
            playPauseButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(1);
            stopButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(2);
            newButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(3);
            openButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(4);
            saveButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(5);
            exportButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(6);
            cutButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(7);
            copyButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(8);
            pasteButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(9);
            undoButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(10);
            redoButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(11);
            addCellsButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(12);
            removeCellsButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(13);
            arrowLeftButton.Enabled = LayoutManager.Instance.ExcessButtonsLeft;
            arrowRightButton.Enabled = LayoutManager.Instance.ExcessButtonsRight;
        }

        public void StopSongPlaying()
        {
            Automatone.Instance.Sequencer.StopMidi();
            NavigatorPanel.Instance.ResetScrolling();
            playPauseButton.Selected = false;
        }

        public void PauseSongPlaying()
        {
            if (Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.PLAYING)
            {
                playPauseButton.Selected = false;
                PlayPauseButtonToggled(this, EventArgs.Empty);
            }
        }

        private void ArrowLeftPressed(object sender, EventArgs e)
        {
            firstButton--;
            ResetButtonsetBounds();
        }

        private void ArrowRightPressed(object sender, EventArgs e)
        {
            firstButton++;
            ResetButtonsetBounds();
        }

        private void GenerateSongButtonPressed(object sender, EventArgs e)
        {
            playPauseButton.Selected = false;
            PlayPauseButtonToggled(sender, e);
            ParametersPanel.Instance.Toggle();
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

            ParametersPanel.Instance.SlideUp();
        }

        private void StopButtonPressed(object sender, EventArgs e)
        {
            StopSongPlaying();
        }

        public DialogResult ShowSaveConfirmation()
        {
            DialogResult result = DialogResult.No;

            if (GridPanel.Instance.SongCells != null && GridPanel.Instance.HasUnsavedChanges)
            {
                System.Console.WriteLine(GridPanel.Instance.HasUnsavedChanges);
                PauseSongPlaying();
                result = MessageBox.Show(
                    "Save changes to current project?",
                    "Confirmation",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button3);
                if (result == DialogResult.Yes)
                {
                    SaveButtonPressed(this, EventArgs.Empty);
                }
            }

            return result;
        }

        public void NewButtonPressed(object sender, EventArgs e)
        {
            if (ShowSaveConfirmation() != DialogResult.Cancel)
            {
                StopSongPlaying();
                Automatone.Instance.MeasureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * InputParameters.Instance.TimeSignature);
                GridPanel.Instance.SongCells = new CellState[Automatone.PIANO_SIZE, Automatone.NEW_SONG_LENGTH];
                GridPanel.Instance.ResetCursors();
                NavigatorPanel.Instance.ResetGridDrawOffset();
                GridPanel.Instance.HasUnsavedChanges = false;
                ParametersPanel.Instance.SlideUp();
            }
        }

        private void OpenButtonPressed(object sender, EventArgs e)
        {
            if (ShowSaveConfirmation() != DialogResult.Cancel)
            {
                OpenFileDialog projectLoadDialog = new OpenFileDialog();
                projectLoadDialog.Filter = "Automatone Project (*.aut)|*.aut";
                projectLoadDialog.RestoreDirectory = true;

                Stream loadStream;

                if (projectLoadDialog.ShowDialog() == DialogResult.OK && (loadStream = projectLoadDialog.OpenFile()) != null)
                {
                    StopSongPlaying();
                    BinaryFormatter formatter = new BinaryFormatter();
                    InputParameters.LoadInstance((InputParameters)formatter.Deserialize(loadStream));
                    Automatone.Instance.MeasureLength = (int)formatter.Deserialize(loadStream);
                    GridPanel.Instance.SongCells = (CellState[,])formatter.Deserialize(loadStream);
                    loadStream.Close();
                    GridPanel.Instance.ResetCursors();
                    NavigatorPanel.Instance.ResetGridDrawOffset();
                    GridPanel.Instance.HasUnsavedChanges = false;
                    ParametersPanel.Instance.SlideUp();
                }
            }
        }

        private void SaveButtonPressed(object sender, EventArgs e)
        {
            if (GridPanel.Instance.SongCells != null)
            {
                SaveFileDialog projectSaveDialog = new SaveFileDialog();
                projectSaveDialog.Filter = "Automatone Project (*.aut)|*.aut";
                projectSaveDialog.RestoreDirectory = true;

                Stream saveStream;

                if (projectSaveDialog.ShowDialog() == DialogResult.OK && (saveStream = projectSaveDialog.OpenFile()) != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(saveStream, InputParameters.Instance);
                    formatter.Serialize(saveStream, Automatone.Instance.MeasureLength);
                    Automatone.Instance.RewriteSong();
                    formatter.Serialize(saveStream, GridPanel.Instance.SongCells);
                    saveStream.Close();
                    GridPanel.Instance.HasUnsavedChanges = false;
                }
            }
        }

        private void ExportButtonPressed(object sender, EventArgs e)
        {

        }

        private void CutButtonPressed(object sender, EventArgs e)
        {
            GridPanel.Instance.CopySelectedCells();
            GridPanel.Instance.DeleteSelectedCells();
        }

        private void CopyButtonPressed(object sender, EventArgs e)
        {
            GridPanel.Instance.CopySelectedCells();
        }

        private void PasteButtonPressed(object sender, EventArgs e)
        {
            GridPanel.Instance.PasteToSelectedCells();
        }

        private void UndoButtonPressed(object sender, EventArgs e)
        {

        }

        private void RedoButtonPressed(object sender, EventArgs e)
        {

        }
        
        private void AddCellsButtonPressed(object sender, EventArgs e)
        {
            GridPanel.Instance.InsertCells();
        }

        private void RemoveCellsButtonPressed(object sender, EventArgs e)
        {
            GridPanel.Instance.RemoveCells();
        }
    }
}
