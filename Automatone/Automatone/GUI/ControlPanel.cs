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

        public UniRectangle ArrowLeftBounds { set { arrowLeftButton.Bounds = value; } }
        public UniRectangle ArrowRightBounds { set { arrowRightButton.Bounds = value; } }
        /*
        public UniRectangle GenerateSongButtonBounds { set { generateSongButton.Bounds = value; } }
        public UniRectangle PlayPauseButtonBounds { set { playPauseButton.Bounds = value; } }
        public UniRectangle StopButtonBounds { set { stopButton.Bounds = value; } }
        public UniRectangle SaveButtonBounds { set { saveButton.Bounds = value; } }
        public UniRectangle OpenButtonBounds { set { openButton.Bounds = value; } }
        public UniRectangle NewButtonBounds { set { newButton.Bounds = value; } }
        public UniRectangle AddCellsButtonBounds { set { addCellsButton.Bounds = value; } }
        public UniRectangle CopyButtonBounds { set { copyButton.Bounds = value; } }
        public UniRectangle CutButtonBounds { set { cutButton.Bounds = value; } }
        public UniRectangle PasteButtonBounds { set { pasteButton.Bounds = value; } }
        public UniRectangle RedoButtonBounds { set { redoButton.Bounds = value; } }
        public UniRectangle RemoveCellsButtonBounds { set { removeCellsButton.Bounds = value; } }
        public UniRectangle UndoButtonBounds { set { undoButton.Bounds = value; } }*/

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

            buttonsetBounds = LayoutManager.Instance.GetButtonsetBounds(13, 0);
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
            // playPauseButton
            //
            //playPauseButton.Bounds = LayoutManager.Instance.PlayPauseButtonBounds;
            playPauseButton.Changed += new EventHandler(PlayPauseButtonToggled);
            playPauseButton.Selected = false;

            //
            // stopButton
            //
            //stopButton.Bounds = LayoutManager.Instance.StopButtonBounds;
            stopButton.Pressed += new EventHandler(StopButtonPressed);
            stopButton.SkinName = "stop";

            //
            // newButton
            //
            //newButton.Bounds = LayoutManager.Instance.NewButtonBounds;
            newButton.Pressed += new EventHandler(NewButtonPressed);
            newButton.SkinName = "new";

            //
            // openButton
            //
            //openButton.Bounds = LayoutManager.Instance.OpenButtonBounds;
            openButton.Pressed += new EventHandler(OpenButtonPressed);
            openButton.SkinName = "open";

            //
            // saveButton
            //
            //saveButton.Bounds = LayoutManager.Instance.SaveButtonBounds;
            saveButton.Pressed += new EventHandler(SaveButtonPressed);
            saveButton.SkinName = "save";

            //
            // cutButton
            //
            //cutButton.Bounds = LayoutManager.Instance.CutButtonBounds;
            cutButton.Pressed += new EventHandler(CutButtonPressed);
            cutButton.SkinName = "cut";
            
            //
            // copyButton
            //
            //copyButton.Bounds = LayoutManager.Instance.CopyButtonBounds;
            copyButton.Pressed += new EventHandler(CopyButtonPressed);
            copyButton.SkinName = "copy";
            
            //
            // pasteButton
            //
            //pasteButton.Bounds = LayoutManager.Instance.PasteButtonBounds;
            pasteButton.Pressed += new EventHandler(PasteButtonPressed);
            pasteButton.SkinName = "paste";

            //
            // undoButton
            //
            //undoButton.Bounds = LayoutManager.Instance.UndoButtonBounds;
            undoButton.Pressed += new EventHandler(UndoButtonPressed);
            undoButton.SkinName = "undo";
            
            //
            // redoButton
            //
            //redoButton.Bounds = LayoutManager.Instance.RedoButtonBounds;
            redoButton.Pressed += new EventHandler(RedoButtonPressed);
            redoButton.SkinName = "redo";

            //
            // addButton
            //
            //addCellsButton.Bounds = LayoutManager.Instance.AddCellsButtonBounds;
            addCellsButton.Pressed += new EventHandler(AddCellsButtonPressed);
            addCellsButton.SkinName = "add.cells";

            //
            // removeButton
            //
            //removeCellsButton.Bounds = LayoutManager.Instance.RemoveCellsButtonBounds;
            removeCellsButton.Pressed += new EventHandler(RemoveCellsButtonPressed);
            removeCellsButton.SkinName = "remove.cells";

            //
            // generateSongButton
            //
            //generateSongButton.Bounds = LayoutManager.Instance.GenerateSongButtonBounds;
            generateSongButton.Pressed += new EventHandler(GenerateSongButtonPressed);
            generateSongButton.SkinName = "generate.song";

            //
            // Add Children
            //
            Children.Add(arrowLeftButton);
            Children.Add(arrowRightButton);

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
            cutButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(6);
            copyButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(7);
            pasteButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(8);
            undoButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(9);
            redoButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(10);
            addCellsButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(11);
            removeCellsButton.Bounds = buttonsetBounds.ElementAt<UniRectangle>(12);
            arrowLeftButton.Enabled = LayoutManager.Instance.ExcessButtonsLeft;
            arrowRightButton.Enabled = LayoutManager.Instance.ExcessButtonsRight;
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

        public void NewButtonPressed(object sender, EventArgs e)
        {
            Automatone.Instance.StopSongPlaying();
            Automatone.Instance.MeasureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * InputParameters.Instance.TimeSignature);
            GridPanel.Instance.SongCells = new CellState[Automatone.PIANO_SIZE, Automatone.NEW_SONG_LENGTH];
            GridPanel.Instance.ResetCursors();
            NavigatorPanel.Instance.ResetGridDrawOffset();
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

            GridPanel.Instance.ResetCursors();
            NavigatorPanel.Instance.ResetGridDrawOffset();
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
        
        private void AddCellsButtonPressed(object sender, EventArgs e)
        {

        }

        private void RemoveCellsButtonPressed(object sender, EventArgs e)
        {

        }

        private void GenerateSongButtonPressed(object sender, EventArgs e)
        {
            playPauseButton.Selected = false;
            PlayPauseButtonToggled(sender, e);
            ParametersPanel.Instance.Toggle();
        }
    }
}
