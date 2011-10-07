using System;
using System.Windows.Forms;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.Graphics.SpecialEffects.Particles;
using NuclexUserInterfaceExtension;

using Automatone.Music;

using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Automatone.GUI
{
    public class GridPanel : DrawableGameComponent
    {
        public bool HasUnsavedChanges { set; get; }
        public bool ScrollWithMidi { set; get; }

        private MouseState oldMouseState;
        private MouseState newMouseState;

        private const byte DimensionX = 1;
        private const byte DimensionY = 0;

        public const string SONG_CELLS_FORMAT = "SONG_CELLS_FORMAT";

        public delegate void GridLengthChangedEvent(int newX, int newY);
        public event GridLengthChangedEvent SongCellsChanged;
        private CellState[,] songCells;
        public CellState[,] SongCells
        {
            set
            {
                if (value.GetLength(DimensionY) != Automatone.PIANO_SIZE)
                    throw new ArgumentOutOfRangeException("Incorrect value for dimension: " + DimensionY);
                songCells = value;
                SongCellsChanged.Invoke(value.GetLength(DimensionX), value.GetLength(DimensionY));
            }
            get
            {
                return songCells;
            }
        }

        public int CellsArrayLengthX 
        { 
            get 
            {
                if (songCells == null)
                    return 0;
                return songCells.GetLength(DimensionX); 
            } 
        }

        private Cells cells;
        private Labels labels;
        private Cursors cursors;

        private static GridPanel instance;
        public static GridPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = new GridPanel();
                return instance;
            }
        }

        private GridPanel()
            : base(Automatone.Instance)
        {
            oldMouseState = Mouse.GetState();
            cells = new Cells();
            labels = new Labels();
            cursors = new Cursors();
            HasUnsavedChanges = true;
        }

        public void ResetCursors()
        {
            cursors.ResetIndices();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            cells.LoadContent();
            labels.LoadContent();
            cursors.LoadContent();
        }

        protected override void UnloadContent()
        {
            cells.Dispose();
            labels.Dispose();
            cursors.Dispose();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            newMouseState = Mouse.GetState();

            if (SongCells != null)
            {
                cells.Update(gameTime);
                cursors.Update(gameTime);

                NavigatorPanel.Instance.Update(gameTime);
            }

            oldMouseState = newMouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (SongCells != null)
            {
                cells.Draw(gameTime);
                labels.Draw(gameTime);
                cursors.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        private Color GetChromaticColor(int pitch)
        {
            int chromaticIndex = pitch % MusicTheory.OCTAVE_SIZE;
            switch (chromaticIndex)
            {
                case 0:
                    return Color.MediumVioletRed;
                case 1:
                    return Color.Red;
                case 2:
                    return Color.OrangeRed;
                case 3:
                    return Color.Orange;
                case 4:
                    return Color.Yellow;
                case 5:
                    return Color.GreenYellow;
                case 6:
                    return Color.Green;
                case 7:
                    return Color.SeaGreen;
                case 8:
                    return Color.Blue;
                case 9:
                    return Color.Indigo;
                case 10:
                    return Color.Violet;
                case 11:
                    return Color.Silver;
            }
            return Color.White;
        }

        public int GridToScreenCoordinatesX(int x)
        {
            return (x * LayoutManager.CELLWIDTH + (int)NavigatorPanel.Instance.GridDrawOffsetX);
        }

        public int GridToScreenCoordinatesY(int y)
        {
            return ((int)NavigatorPanel.Instance.GridDrawOffsetY - (LayoutManager.CELLHEIGHT * (y - SongCells.GetLength(DimensionY) + 1)));
        }

        public int ScreenToGridCoordinatesX(int x)
        {
            return ((int)-NavigatorPanel.Instance.GridDrawOffsetX + x) / LayoutManager.CELLWIDTH;
        }

        public int ScreenToGridCoordinatesY(int y)
        {
            return SongCells.GetLength(DimensionY) - 1 - ((int)-NavigatorPanel.Instance.GridDrawOffsetY + y) / LayoutManager.CELLHEIGHT;
        }

        public State getCurrentState()
        {
            return new State(songCells, cursors.TopStartIndex, cursors.TopEndIndex, cursors.LeftStartIndex, cursors.LeftEndIndex);
        }

        public void setCurrentState(State state)
        {
            songCells = state.SongCells;
            cursors.TopStartIndex = state.TopStartCursorIndex;
            cursors.TopEndIndex = state.TopEndCursorIndex;
            cursors.LeftStartIndex = state.LeftStartCursorIndex;
            cursors.LeftEndIndex = state.LeftEndCursorIndex;
        }

        public void AddCells(int columns)
        {
            if (columns > 0)
            {
                CellState[,] newSongCells = new CellState[songCells.GetLength(0), songCells.GetLength(1) + columns];
                for (int i = 0; i < songCells.GetLength(0); i++)
                {
                    for (int j = 0; j < songCells.GetLength(1); j++)
                    {
                        newSongCells[i, j] = songCells[i, j];
                    }
                }
                Memento.Instance.DoAction(getCurrentState());
                ControlPanel.Instance.ResetButtonsetStatuses();
                SongCells = newSongCells;
            }
        }

        public void InsertCells()
        {
            int endIndex = Math.Max(cursors.TopEndIndex, cursors.TopStartIndex + 1);
            CellState[,] newSongCells = new CellState[songCells.GetLength(0), songCells.GetLength(1) + endIndex - cursors.TopStartIndex];
            for (int i = 0; i < newSongCells.GetLength(0); i++)
            {
                for (int j = 0; j < cursors.TopStartIndex; j++)
                {
                    newSongCells[i, j] = songCells[i, j];
                }
                int tail = cursors.TopStartIndex;
                while (tail < songCells.GetLength(1) && songCells[i, tail] == CellState.HOLD)
                {
                    songCells[i, tail] = CellState.SILENT;
                    tail++;
                }
                for (int j = endIndex; j < newSongCells.GetLength(1); j++)
                {
                    newSongCells[i, j] = songCells[i, j - endIndex + cursors.TopStartIndex];
                }
            }
            Memento.Instance.DoAction(getCurrentState());
            ControlPanel.Instance.ResetButtonsetStatuses();
            SongCells = newSongCells;
        }

        public void RemoveCells()
        {
            CellState[,] newSongCells = new CellState[songCells.GetLength(0), songCells.GetLength(1) - (cursors.TopEndIndex - cursors.TopStartIndex)];
            for (int i = 0; i < newSongCells.GetLength(0); i++)
            {
                for (int j = 0; j < cursors.TopStartIndex; j++)
                {
                    newSongCells[i, j] = songCells[i, j];
                }
                int tail = cursors.TopEndIndex;
                while (tail < songCells.GetLength(1) && songCells[i, tail] == CellState.HOLD)
                {
                    songCells[i, tail] = CellState.SILENT;
                    tail++;
                }
                for (int j = cursors.TopStartIndex; j < newSongCells.GetLength(1); j++)
                {
                    newSongCells[i, j] = songCells[i, j - cursors.TopStartIndex + cursors.TopEndIndex];
                }
            }
            Memento.Instance.DoAction(getCurrentState());
            ControlPanel.Instance.ResetButtonsetStatuses();
            SongCells = newSongCells;
            cursors.TopEndIndex = cursors.TopStartIndex;
        }

        public void CopySelectedCells()
        {
            if (cursors.TopStartIndex != cursors.TopEndIndex && cursors.LeftStartIndex != cursors.LeftEndIndex)
            {
                CellState[,] selectedCells = new CellState[cursors.LeftEndIndex - cursors.LeftStartIndex, cursors.TopEndIndex - cursors.TopStartIndex];
                for (int i = cursors.LeftStartIndex + 1; i <= cursors.LeftEndIndex; i++)
                {
                    for (int j = cursors.TopStartIndex; j < cursors.TopEndIndex; j++)
                    {
                        selectedCells[i - (cursors.LeftStartIndex + 1), j - cursors.TopStartIndex] = songCells[i, j];
                    }
                }
                for (int i = 0; i < selectedCells.GetLength(0); i++)
                {
                    int tail = 0;
                    while (tail < selectedCells.GetLength(1) && selectedCells[i, tail] == CellState.HOLD)
                    {
                        selectedCells[i, tail] = CellState.SILENT;
                        tail++;
                    }
                }
                CopyCellsToClipboard(selectedCells);
            }
        }

        public void DeleteSelectedCells()
        {
            if (cursors.TopStartIndex != cursors.TopEndIndex && cursors.LeftStartIndex != cursors.LeftEndIndex)
            {
                Memento.Instance.DoAction(getCurrentState());
                ControlPanel.Instance.ResetButtonsetStatuses();
                for (int i = cursors.LeftStartIndex + 1; i <= cursors.LeftEndIndex; i++)
                {
                    for (int j = cursors.TopStartIndex; j < cursors.TopEndIndex; j++)
                    {
                        songCells[i, j] = CellState.SILENT;
                    }
                    int tail = cursors.TopEndIndex;
                    while (tail < songCells.GetLength(1) && songCells[i, tail] == CellState.HOLD)
                    {
                        songCells[i, tail] = CellState.SILENT;
                        tail++;
                    }
                }
            }
        }

        public void PasteToSelectedCells()
        {
            CellState[,] cellsToPaste = GetCellsFromClipboard();
            if (cellsToPaste != null)
            {
                Memento.Instance.DoAction(getCurrentState());
                ControlPanel.Instance.ResetButtonsetStatuses();
                AddCells(cellsToPaste.GetLength(1) - (songCells.GetLength(1) - cursors.TopStartIndex));
                for (int i = cursors.LeftEndIndex + 1 - cellsToPaste.GetLength(0); i <= cursors.LeftEndIndex; i++)
                {
                    if (i >= 0)
                    {
                        for (int j = cursors.TopStartIndex; j < cursors.TopStartIndex + cellsToPaste.GetLength(1); j++)
                        {
                            songCells[i, j] = cellsToPaste[i - (cursors.LeftEndIndex + 1 - cellsToPaste.GetLength(0)), j - cursors.TopStartIndex];
                        }
                        int tail = cursors.TopStartIndex + cellsToPaste.GetLength(1);
                        while (tail < songCells.GetLength(1) && songCells[i, tail] == CellState.HOLD)
                        {
                            songCells[i, tail] = CellState.SILENT;
                            tail++;
                        }
                    }
                }
                cursors.LeftStartIndex = cursors.LeftEndIndex - cellsToPaste.GetLength(0);
                cursors.TopEndIndex = cursors.TopStartIndex + cellsToPaste.GetLength(1);
            }
        }

        private void CopyCellsToClipboard(CellState[,] cellsToBeCopied)
        {
            if (cellsToBeCopied != null)
            {
                Clipboard.SetData(SONG_CELLS_FORMAT, cellsToBeCopied);
            }
        }

        private CellState[,] GetCellsFromClipboard()
        {
            if (Clipboard.ContainsData(SONG_CELLS_FORMAT))
            {
                return (CellState[,])Clipboard.GetData(SONG_CELLS_FORMAT);
            }
            return null;
        }


        /// <summary>
        /// This class handles the updating and drawing of the cells 
        /// only (the visual representation of the current song project 
        /// as a 2D array, excluding the labels, cursors, and navigators)
        /// in the grid panel.
        /// </summary>
        private class Cells
        {
            private static Texture2D silentCell;
            private static Texture2D startCell;
            private static Texture2D startCellEnd;
            private static Texture2D holdCell;
            private static Texture2D holdCellEnd;
            private static Texture2D gridPanelBackground;

            private int? gridInputStartXIndex;
            private int? gridInputEndXIndex;
            private int? gridInputYIndex;

            private bool storeAction = true;

            public void LoadContent()
            {
                startCellEnd = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Cell_Lightboxend");
                startCell = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Cell_Lightbox");
                holdCell = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Cell_Holdbox");
                holdCellEnd = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Cell_Holdboxend");
                silentCell = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Cell_Darkbox");
                gridPanelBackground = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Bg_Gridpanel");
            }

            public void Dispose()
            {
                if (silentCell != null) silentCell.Dispose();
                if (startCell != null) startCell.Dispose();
                if (startCellEnd != null) startCellEnd.Dispose();
                if (holdCell != null) holdCell.Dispose();
                if (holdCellEnd != null) holdCellEnd.Dispose();
                if (gridPanelBackground != null) gridPanelBackground.Dispose();
            }

            public void Update(GameTime gameTime)
            {
                if (Automatone.Instance.IsActive
                    && Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED
                    && (GridPanel.Instance.newMouseState.LeftButton != ButtonState.Released
                        || GridPanel.Instance.oldMouseState.LeftButton != ButtonState.Released)
                    && LayoutManager.Instance.GridCellsClickableArea.Contains(new Point(GridPanel.Instance.newMouseState.X, GridPanel.Instance.newMouseState.Y)))
                {
                    if (storeAction)
                    {
                        Memento.Instance.DoAction(GridPanel.Instance.getCurrentState());
                        ControlPanel.Instance.ResetButtonsetStatuses();
                        storeAction = false;
                    }
                    if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Released)
                    {
                        GridPanel.Instance.HasUnsavedChanges = true;
                        gridInputStartXIndex = Math.Min(GridPanel.Instance.SongCells.GetLength(DimensionX) - 1, GridPanel.Instance.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputEndXIndex = Math.Min(GridPanel.Instance.SongCells.GetLength(DimensionX) - 1, GridPanel.Instance.ScreenToGridCoordinatesX(Mouse.GetState().X));
                        gridInputYIndex = GridPanel.Instance.ScreenToGridCoordinatesY(Mouse.GetState().Y);
                        if (GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] == CellState.SILENT)
                        {
                            GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.START;
                        }
                        else
                        {
                            GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] = CellState.SILENT;
                        }

                    }
                    else if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Released && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        gridInputStartXIndex = null;
                        gridInputEndXIndex = null;
                        gridInputYIndex = null;
                        storeAction = true;
                    }
                    else if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (gridInputStartXIndex.HasValue)
                        {
                            gridInputEndXIndex = Math.Min(GridPanel.Instance.SongCells.GetLength(DimensionX) - 1, GridPanel.Instance.ScreenToGridCoordinatesX(Mouse.GetState().X));
                            int k = gridInputStartXIndex.Value + 1;
                            while (k < GridPanel.Instance.SongCells.GetLength(DimensionX) && GridPanel.Instance.SongCells[gridInputYIndex.Value, k] == CellState.HOLD)
                            {
                                GridPanel.Instance.SongCells[gridInputYIndex.Value, k] = CellState.SILENT;
                                k++;
                            }
                            for (int i = gridInputStartXIndex.Value + 1; i <= gridInputEndXIndex; i++)
                            {
                                if (GridPanel.Instance.SongCells[gridInputYIndex.Value, gridInputStartXIndex.Value] != CellState.SILENT)
                                {
                                    if (GridPanel.Instance.SongCells[gridInputYIndex.Value, i] == CellState.SILENT)
                                    {
                                        GridPanel.Instance.SongCells[gridInputYIndex.Value, i] = CellState.HOLD;
                                    }
                                    else break;
                                }
                            }
                        }
                    }
                }
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin();
                
                Automatone.Instance.SpriteBatch.Draw(gridPanelBackground, LayoutManager.Instance.GridPanelBounds, Color.White);
                for (int i = NavigatorPanel.Instance.VerticalClippingStartIndex; i <= NavigatorPanel.Instance.VerticalClippingEndIndex; i++)
                {
                    for (int j = NavigatorPanel.Instance.HorizontalClippingStartIndex; j <= NavigatorPanel.Instance.HorizontalClippingEndIndex; j++)
                    {
                        Rectangle drawRectangle = new Rectangle((int)(j * LayoutManager.CELLWIDTH + NavigatorPanel.Instance.GridDrawOffsetX), (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + NavigatorPanel.Instance.GridDrawOffsetY), LayoutManager.CELLWIDTH, LayoutManager.CELLHEIGHT);
                        Color drawColor = GridPanel.Instance.GetChromaticColor(i);

                        if (((i + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) / 12) % 2 == 0)
                        {
                            Automatone.Instance.SpriteBatch.Draw(silentCell, drawRectangle, Color.White);
                        }
                        else
                        {
                            Automatone.Instance.SpriteBatch.Draw(silentCell, drawRectangle, Color.Black);
                        }

                        if (GridPanel.Instance.SongCells[i, j] != CellState.SILENT && j * LayoutManager.CELLWIDTH < -NavigatorPanel.Instance.PlayOffset - LayoutManager.CELLWIDTH)
                        {
                            Automatone.Instance.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 32));
                        }
                        else if (GridPanel.Instance.SongCells[i, j] != CellState.SILENT)
                        {
                            Automatone.Instance.SpriteBatch.Draw(GetCellTexture(i, j), drawRectangle, new Color(drawColor.R, drawColor.G, drawColor.B, 255));
                        }
                    }
                }
                Automatone.Instance.SpriteBatch.End();
            }

            private Texture2D GetCellTexture(int i, int j)
            {
                switch (GridPanel.Instance.SongCells[i, j])
                {
                    case CellState.START:
                        if (j + 1 >= GridPanel.Instance.SongCells.GetLength(DimensionX) || GridPanel.Instance.SongCells[i, j + 1] != CellState.HOLD)
                            return startCellEnd;
                        return startCell;
                    case CellState.HOLD:
                        if (j + 1 >= GridPanel.Instance.SongCells.GetLength(DimensionX) || GridPanel.Instance.SongCells[i, j+1] != CellState.HOLD)
                            return holdCellEnd;
                        return holdCell;
                }
                return null;
            }
        }
        
        /// <summary>
        /// This class handles the updating and drawing of the
        /// pitch ("C3", "C#3", "D3", etc.) and time (measure number and beats)
        /// labels of the grid, which automatically moves along with the
        /// navigators to allow the user to know which section of the grid
        /// he/she is viewing and working on.
        /// </summary>
        private class Labels
        {
            private Texture2D labelsBackgroundVert;
            private Texture2D labelsBackgroundHori;
            private SpriteFont labelFont;
            private const bool sharpLabels = true;

            public void LoadContent()
            {
                labelsBackgroundVert = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Bar_Border_Vert");
                labelsBackgroundHori = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Bar_Border_Hori");
                labelFont = Automatone.Instance.Content.Load<SpriteFont>("LabelFont");
            }

            public void Dispose()
            {
                if (labelsBackgroundVert != null)
                    labelsBackgroundVert.Dispose();
                if (labelsBackgroundHori != null)
                    labelsBackgroundHori.Dispose();
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin();
                DrawPitchLabel();
                DrawRightBorder();
                DrawTimeLabel();
                DrawBottomBorder();
                Automatone.Instance.SpriteBatch.End();
            }

            private void DrawPitchLabel()
            {
                for (int i = 0; i < (Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) / MusicTheory.OCTAVE_SIZE + 1; i++)
                {
                    if ((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER >= NavigatorPanel.Instance.VerticalClippingStartIndex && i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER <= NavigatorPanel.Instance.VerticalClippingEndIndex)
                    {
                        int upperBound = Math.Max(LayoutManager.Instance.GridTopBorderBounds.Bottom, GridPanel.Instance.GridToScreenCoordinatesY((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER - 1));
                        int lowerBound = Math.Min(LayoutManager.Instance.GridBottomBorderBounds.Top, GridPanel.Instance.GridToScreenCoordinatesY(i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER - 1));
                        Automatone.Instance.SpriteBatch.Draw(labelsBackgroundVert, new Rectangle(LayoutManager.Instance.GridLeftBorderBounds.Left, upperBound, LayoutManager.Instance.GridRightBorderBounds.Width, lowerBound - upperBound), (i % 2 == 0 ? Color.White : Color.Gray));
                    }
                }

                for (int i = NavigatorPanel.Instance.VerticalClippingStartIndex; i <= NavigatorPanel.Instance.VerticalClippingEndIndex; i++)
                {
                    Vector2 loc = new Vector2(5, (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + NavigatorPanel.Instance.GridDrawOffsetY));
                    string letter = "";
                    switch ((i - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER + MusicTheory.OCTAVE_SIZE) % MusicTheory.OCTAVE_SIZE)
                    {
                        case 0:
                            letter = "C";
                            break;
                        case 1:
                            letter = (sharpLabels ? "C♯" : "D♭");
                            break;
                        case 2:
                            letter = "D";
                            break;
                        case 3:
                            letter = (sharpLabels ? "D♯" : "E♭");
                            break;
                        case 4:
                            letter = "E";
                            break;
                        case 5:
                            letter = "F";
                            break;
                        case 6:
                            letter = (sharpLabels ? "F♯" : "G♭");
                            break;
                        case 7:
                            letter = "G";
                            break;
                        case 8:
                            letter = (sharpLabels ? "G♯" : "A♭");
                            break;
                        case 9:
                            letter = "A";
                            break;
                        case 10:
                            letter = (sharpLabels ? "A♯" : "B♭");
                            break;
                        case 11:
                            letter = "B";
                            break;
                    }
                    Automatone.Instance.SpriteBatch.DrawString(labelFont, letter, loc, Color.LightGray);
                }
            }

            private void DrawTimeLabel()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackgroundHori, LayoutManager.Instance.GridTopBorderBounds, Color.White);
                for (int j = NavigatorPanel.Instance.HorizontalClippingStartIndex; j <= NavigatorPanel.Instance.HorizontalClippingEndIndex; j++)
                {
                    Vector2 loc = new Vector2((int)(j * LayoutManager.CELLWIDTH + NavigatorPanel.Instance.GridDrawOffsetX - 2), 5 + LayoutManager.CONTROLS_AND_GRID_DIVISION);
                    if (j % Automatone.Instance.MeasureLength == 0)
                    {
                        Automatone.Instance.SpriteBatch.DrawString(labelFont, "" + (j / Automatone.Instance.MeasureLength + 1), loc, Color.LightGray);
                    }
                    for (int k = 1; k < Automatone.Instance.MeasureLength / (Automatone.SUBBEATS_PER_WHOLE_NOTE / 4); k++)
                    {
                        if (j % Automatone.Instance.MeasureLength == k * Automatone.SUBBEATS_PER_WHOLE_NOTE / 4)
                        {
                            Automatone.Instance.SpriteBatch.DrawString(labelFont, "♩", loc, Color.Gray);
                        }
                    }
                }
            }

            private void DrawRightBorder()
            {
                for (int i = 0; i < (Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) / MusicTheory.OCTAVE_SIZE + 1; i++)
                {
                    if ((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER >= NavigatorPanel.Instance.VerticalClippingStartIndex && i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER <= NavigatorPanel.Instance.VerticalClippingEndIndex)
                    {
                        int upperBound = Math.Max(LayoutManager.Instance.GridTopBorderBounds.Bottom, GridPanel.Instance.GridToScreenCoordinatesY((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER - 1));
                        int lowerBound = Math.Min(LayoutManager.Instance.GridBottomBorderBounds.Top, GridPanel.Instance.GridToScreenCoordinatesY(i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER - 1));
                        Automatone.Instance.SpriteBatch.Draw(labelsBackgroundVert, new Rectangle(LayoutManager.Instance.GridRightBorderBounds.Left, upperBound, LayoutManager.Instance.GridRightBorderBounds.Width, lowerBound - upperBound), (i % 2 == 0 ? Color.White : Color.Gray));
                    }
                }
                for (int i = 0; i < (Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) / MusicTheory.OCTAVE_SIZE + 1; i++)
                {
                    if ((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER > NavigatorPanel.Instance.VerticalClippingStartIndex && i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER - 1 < NavigatorPanel.Instance.VerticalClippingEndIndex)
                    {
                        int upperBound = Math.Max(LayoutManager.Instance.GridTopBorderBounds.Bottom, GridPanel.Instance.GridToScreenCoordinatesY((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER));
                        int lowerBound = Math.Min(LayoutManager.Instance.GridBottomBorderBounds.Top, GridPanel.Instance.GridToScreenCoordinatesY(i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER));
                        Vector2 loc = new Vector2(LayoutManager.Instance.GridRightBorderBounds.Left + 10, (upperBound + lowerBound) / 2);
                        Automatone.Instance.SpriteBatch.DrawString(labelFont, i + "", loc, Color.LightGray);
                    }
                }
            }

            private void DrawBottomBorder()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackgroundHori, LayoutManager.Instance.GridBottomBorderBounds, Color.White);
            }
        }

        /// <summary>
        /// This class handles the updating and drawing of the particle
        /// system (used to create cool visual effects while notes are played).
        /// </summary>
        private class Particles
        {

            private ParticleSystem<NoteHitParticle> particleSystem;
            private const byte PARTICLE_SPAWN_DENSITY = 4;
            private const byte PARTICLE_SPAWN_CYCLE = 50;
            private byte currentCycle;

            public Particles()
            {
                particleSystem = new ParticleSystem<NoteHitParticle>(8192);
                particleSystem.Affectors.Add(new MovementAffector<NoteHitParticle>(new NoteHitParticleModifier()));
                currentCycle = 0;
            }

            private void Update()
            {
                if (currentCycle++ > PARTICLE_SPAWN_CYCLE)
                {
                    int j = GridPanel.Instance.ScreenToGridCoordinatesX((int)-NavigatorPanel.Instance.PlayOffset);

                    for (int i = NavigatorPanel.Instance.VerticalClippingStartIndex; i < NavigatorPanel.Instance.VerticalClippingEndIndex; i++)
                    {
                        if (GridPanel.Instance.SongCells[i, j] != CellState.SILENT)
                        {
                            Vector2 particleSpawnPoint = new Vector2((int)(j * LayoutManager.CELLWIDTH + NavigatorPanel.Instance.GridDrawOffsetX), (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + NavigatorPanel.Instance.GridDrawOffsetY));
                            for (float k = 0; k < PARTICLE_SPAWN_DENSITY; k++)
                            {
                                if (particleSystem.Particles.Count < particleSystem.Capacity)
                                    particleSystem.AddParticle(new NoteHitParticle(new Vector3(particleSpawnPoint + new Vector2(0, k / PARTICLE_SPAWN_DENSITY * LayoutManager.CELLHEIGHT), 0), new Vector3(1, 0, 0), GridPanel.Instance.GetChromaticColor(i)));
                            }
                        }
                    }
                    currentCycle = 0;
                }

                IAsyncResult asyncResult = particleSystem.BeginUpdate(1, 4, null, null);
                particleSystem.EndUpdate(asyncResult);
                particleSystem.Prune(NoteHitParticle.IsAlive);
            }

            private void Draw()
            {
                foreach (NoteHitParticle particle in particleSystem.Particles.Array)
                {
                    //Automatone.Instance.SpriteBatch.Draw(startCell, new Rectangle((int)particle.Position.X, (int)particle.Position.Y, 2, 2), particle.Color);
                }
            }

            private struct NoteHitParticle
            {
                private static Random rand = new Random();

                private Vector3 position;
                public Vector3 Position
                {
                    set { position = value; }
                    get { return position; }
                }

                private Vector3 velocity;
                public Vector3 Velocity
                {
                    set { velocity = value; }
                    get { return velocity; }
                }

                private const int MAX_TIMER = 3;
                private int timer;
                public int Timer
                {
                    set { timer = value; }
                    get { return timer; }
                }

                private Color color;
                public Color Color
                {
                    set { color = value; }
                    get { return color; }
                }

                public NoteHitParticle(Vector3 position, Vector3 velocity, Color color)
                {
                    this.position = position;
                    this.velocity = velocity;
                    this.timer = NewTime();
                    this.color = color;
                }

                public static bool IsAlive(ref NoteHitParticle particle)
                {
                    if (particle.Timer-- < 0)
                    {
                        particle.Timer = NewTime();
                        return false;
                    }
                    return true;
                }

                private static int NewTime()
                {
                    return (int)(rand.NextDouble() * MAX_TIMER);
                }
            }

            private class NoteHitParticleModifier : IParticleModifier<NoteHitParticle>
            {
                public bool HasVelocity { get { return true; } }
                public bool HasWeight { get { return false; } }

                public void GetPosition(ref NoteHitParticle particle, out Vector3 position)
                {
                    position = particle.Position;
                }

                public void GetVelocity(ref NoteHitParticle particle, out Vector3 velocity)
                {
                    velocity = particle.Velocity;
                }

                public float GetWeight(ref NoteHitParticle particle)
                {
                    throw new NotImplementedException();
                }

                public void SetPosition(ref NoteHitParticle particle, ref Vector3 position)
                {
                    position = particle.Position;
                }

                public void SetVelocity(ref NoteHitParticle particle, ref Vector3 velocity)
                {
                    velocity = particle.Velocity;
                }

                public void SetWeight(ref NoteHitParticle particle, float weight)
                {
                    throw new NotImplementedException();
                }

                public void AddScaledVelocityToPosition(NoteHitParticle[] particles, int start, int count, float scale)
                {
                    int end = start + count;
                    for (int i = start; i < end; i++)
                    {
                        particles[i].Position += particles[i].Velocity * scale;
                    }
                }

                public void AddToVelocity(NoteHitParticle[] particles, int start, int count, ref Vector3 velocityAdjustment)
                {
                    int end = start + count;
                    for (int i = start; i < end; i++)
                    {
                        particles[i].Velocity += velocityAdjustment;
                    }
                }

                public void AddVelocityToPosition(NoteHitParticle[] particles, int start, int count)
                {
                    int end = start + count;
                    for (int i = start; i < end; i++)
                    {
                        particles[i].Position += particles[i].Velocity;
                    }
                }
            }
        }

        /// <summary>
        /// This class handles the cursors which are used to select
        /// sections of the grid for editing.
        /// </summary>
        private class Cursors
        {
            private Texture2D topCursorHead;
            private Texture2D leftCursorHead;
            private Texture2D playCursor;
            private Texture2D cursorHori;
            private Texture2D cursorVert;
            private Texture2D cursorHighlight;

            private bool storeAction = true;

            private int topStartIndex;
            private int topEndIndex;
            private int leftStartIndex;
            private int leftEndIndex;

            public int TopStartIndex
            {
                get
                {
                    return Math.Min(topStartIndex, topEndIndex);
                }
                set
                {
                    topEndIndex = TopEndIndex;
                    topStartIndex = (int)MathHelper.Clamp(value, 0, GridPanel.Instance.SongCells.GetLength(DimensionX) + 1);
                }
            }
            public int TopEndIndex
            {
                get
                {
                    return Math.Max(topEndIndex, topStartIndex);
                }
                set
                {
                    topStartIndex = TopStartIndex;
                    topEndIndex = (int)MathHelper.Clamp(value, 0, GridPanel.Instance.SongCells.GetLength(DimensionX) + 1);
                }
            }

            public int LeftStartIndex
            {
                get
                {
                    return Math.Min(leftStartIndex, leftEndIndex);
                }
                set
                {
                    leftEndIndex = LeftEndIndex;
                    leftStartIndex = (int)MathHelper.Clamp(value, -1, GridPanel.Instance.SongCells.GetLength(DimensionY));
                }
            }
            public int LeftEndIndex
            {
                get
                {
                    return Math.Max(leftEndIndex, leftStartIndex);
                }
                set
                {
                    leftStartIndex = LeftStartIndex;
                    leftEndIndex = (int)MathHelper.Clamp(value, -1, GridPanel.Instance.SongCells.GetLength(DimensionY));
                }
            }

            public void ResetIndices()
            {
                topStartIndex = 0;
                topEndIndex = 0;
                leftStartIndex = -1;
                leftEndIndex = -1;
            }

            public void LoadContent()
            {
                topCursorHead = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_TopCursorHead");
                leftCursorHead = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_LeftCursorHead");
                playCursor = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_Play_Cursor");
                cursorHori = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_Cursor_Hori");
                cursorVert = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_Cursor_Vert");
                cursorHighlight = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_Cursor_Highlight");
            }

            public void Dispose()
            {
                if (topCursorHead != null) topCursorHead.Dispose();
                if (leftCursorHead != null) leftCursorHead.Dispose();
                if (playCursor != null) playCursor.Dispose();
                if (cursorHori != null) cursorHori.Dispose();
                if (cursorVert != null) cursorVert.Dispose();
                if (cursorHighlight != null) cursorHighlight.Dispose();
            }

            public void Update(GameTime gameTime)
            {
                if (Automatone.Instance.IsActive
                    && Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED
                    && (GridPanel.Instance.newMouseState.LeftButton != ButtonState.Released
                        || GridPanel.Instance.oldMouseState.LeftButton != ButtonState.Released)
                    && LayoutManager.Instance.GridTopCursorsClickableArea.Contains(new Point(GridPanel.Instance.newMouseState.X, GridPanel.Instance.newMouseState.Y)))
                {
                    if (storeAction)
                    {
                        Memento.Instance.DoAction(GridPanel.Instance.getCurrentState());
                        ControlPanel.Instance.ResetButtonsetStatuses();
                        storeAction = false;
                    }
                    if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Released)
                        {
                            topStartIndex = GridPanel.Instance.ScreenToGridCoordinatesX(GridPanel.Instance.newMouseState.X + LayoutManager.CELLWIDTH / 2);
                            storeAction = true;
                        }
                        topEndIndex = GridPanel.Instance.ScreenToGridCoordinatesX(GridPanel.Instance.newMouseState.X + LayoutManager.CELLWIDTH / 2);
                    }
                }
                if (Automatone.Instance.IsActive
                    && Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED
                    && (GridPanel.Instance.newMouseState.LeftButton != ButtonState.Released
                        || GridPanel.Instance.oldMouseState.LeftButton != ButtonState.Released)
                    && LayoutManager.Instance.GridLeftCursorsClickableArea.Contains(new Point(GridPanel.Instance.newMouseState.X, GridPanel.Instance.newMouseState.Y)))
                {
                    if (storeAction)
                    {
                        Memento.Instance.DoAction(GridPanel.Instance.getCurrentState());
                        ControlPanel.Instance.ResetButtonsetStatuses();
                        storeAction = false;
                    }
                    if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Released)
                        {
                            leftStartIndex = GridPanel.Instance.ScreenToGridCoordinatesY(GridPanel.Instance.newMouseState.Y + LayoutManager.CELLHEIGHT / 2);
                            storeAction = true;
                        }
                        leftEndIndex = GridPanel.Instance.ScreenToGridCoordinatesY(GridPanel.Instance.newMouseState.Y + LayoutManager.CELLHEIGHT / 2);
                    }
                }
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin();
                if (TopStartIndex != TopEndIndex)
                {
                    Automatone.Instance.SpriteBatch.Draw(cursorHighlight,
                        new Rectangle(Math.Max(LayoutManager.Instance.GridLeftBorderBounds.Right, GridPanel.Instance.GridToScreenCoordinatesX(TopStartIndex)),
                            LayoutManager.Instance.GridTopBorderBounds.Bottom,
                            Math.Min(LayoutManager.Instance.GridRightBorderBounds.Left, GridPanel.Instance.GridToScreenCoordinatesX(TopEndIndex)) - Math.Max(LayoutManager.Instance.GridLeftBorderBounds.Right, GridPanel.Instance.GridToScreenCoordinatesX(TopStartIndex)),
                            LayoutManager.Instance.GridCellsClickableArea.Height),
                        Color.White);
                }
                if (LeftStartIndex != LeftEndIndex)
                {
                    Automatone.Instance.SpriteBatch.Draw(cursorHighlight,
                        new Rectangle(LayoutManager.Instance.GridLeftBorderBounds.Right,
                            Math.Max(LayoutManager.Instance.GridTopBorderBounds.Bottom, GridPanel.Instance.GridToScreenCoordinatesY(LeftEndIndex)),
                            LayoutManager.Instance.GridCellsClickableArea.Width,
                            Math.Min(LayoutManager.Instance.GridBottomBorderBounds.Top, GridPanel.Instance.GridToScreenCoordinatesY(LeftStartIndex)) - Math.Max(LayoutManager.Instance.GridTopBorderBounds.Bottom, GridPanel.Instance.GridToScreenCoordinatesY(LeftEndIndex))),
                        Color.White);
                }
                if (TopStartIndex != TopEndIndex
                    && TopEndIndex <= GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridRightBorderBounds.Left)
                    && TopEndIndex > GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridLeftBorderBounds.Right))
                {
                    Automatone.Instance.SpriteBatch.Draw(cursorVert, new Rectangle(GridPanel.Instance.GridToScreenCoordinatesX(TopEndIndex) - LayoutManager.VERTICAL_CURSOR_WIDTH / 2, LayoutManager.Instance.GridTopBorderBounds.Bottom, LayoutManager.VERTICAL_CURSOR_WIDTH, LayoutManager.Instance.GridCellsClickableArea.Height), Color.White);
                    Automatone.Instance.SpriteBatch.Draw(topCursorHead, new Rectangle(GridPanel.Instance.GridToScreenCoordinatesX(TopEndIndex) - LayoutManager.TOP_CURSOR_HEAD_WIDTH / 2, LayoutManager.Instance.GridTopCursorsClickableArea.Y, LayoutManager.TOP_CURSOR_HEAD_WIDTH, LayoutManager.TOP_CURSOR_HEAD_HEIGHT), Color.White);
                }
                if (TopStartIndex <= GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridRightBorderBounds.Left)
                    && TopStartIndex > GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridLeftBorderBounds.Right))
                {
                    Automatone.Instance.SpriteBatch.Draw(cursorVert, new Rectangle(GridPanel.Instance.GridToScreenCoordinatesX(TopStartIndex) - LayoutManager.VERTICAL_CURSOR_WIDTH / 2, LayoutManager.Instance.GridTopBorderBounds.Bottom, LayoutManager.VERTICAL_CURSOR_WIDTH, LayoutManager.Instance.GridCellsClickableArea.Height), Color.White);
                    Automatone.Instance.SpriteBatch.Draw(topCursorHead, new Rectangle(GridPanel.Instance.GridToScreenCoordinatesX(TopStartIndex) - LayoutManager.TOP_CURSOR_HEAD_WIDTH / 2, LayoutManager.Instance.GridTopCursorsClickableArea.Y, LayoutManager.TOP_CURSOR_HEAD_WIDTH, LayoutManager.TOP_CURSOR_HEAD_HEIGHT), Color.White);
                }

                if (LeftStartIndex != LeftEndIndex
                    && LeftEndIndex < GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.Instance.GridTopBorderBounds.Bottom)
                    && LeftEndIndex >= GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.Instance.GridBottomBorderBounds.Top))
                {
                    Automatone.Instance.SpriteBatch.Draw(cursorHori, new Rectangle(LayoutManager.Instance.GridLeftBorderBounds.Right, GridPanel.Instance.GridToScreenCoordinatesY(LeftEndIndex) - LayoutManager.HORIZONTAL_CURSOR_HEIGHT / 2, LayoutManager.Instance.GridCellsClickableArea.Width, LayoutManager.HORIZONTAL_CURSOR_HEIGHT), Color.White);
                    Automatone.Instance.SpriteBatch.Draw(leftCursorHead, new Rectangle(LayoutManager.Instance.GridLeftCursorsClickableArea.X, GridPanel.Instance.GridToScreenCoordinatesY(LeftEndIndex) - LayoutManager.LEFT_CURSOR_HEAD_HEIGHT / 2, LayoutManager.LEFT_CURSOR_HEAD_WIDTH, LayoutManager.LEFT_CURSOR_HEAD_HEIGHT), Color.White);
                }
                if (LeftStartIndex < GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.Instance.GridTopBorderBounds.Bottom)
                    && LeftStartIndex >= GridPanel.Instance.ScreenToGridCoordinatesY(LayoutManager.Instance.GridBottomBorderBounds.Top))
                {
                    Automatone.Instance.SpriteBatch.Draw(cursorHori, new Rectangle(LayoutManager.Instance.GridLeftBorderBounds.Right, GridPanel.Instance.GridToScreenCoordinatesY(LeftStartIndex) - LayoutManager.HORIZONTAL_CURSOR_HEIGHT / 2, LayoutManager.Instance.GridCellsClickableArea.Width, LayoutManager.HORIZONTAL_CURSOR_HEIGHT), Color.White);
                    Automatone.Instance.SpriteBatch.Draw(leftCursorHead, new Rectangle(LayoutManager.Instance.GridLeftCursorsClickableArea.X, GridPanel.Instance.GridToScreenCoordinatesY(LeftStartIndex) - LayoutManager.LEFT_CURSOR_HEAD_HEIGHT / 2, LayoutManager.LEFT_CURSOR_HEAD_WIDTH, LayoutManager.LEFT_CURSOR_HEAD_HEIGHT), Color.White);
                }
                
                if(Automatone.Instance.Sequencer.State != Sequencer.MidiPlayerState.STOPPED)
                {
                    Automatone.Instance.SpriteBatch.Draw(playCursor, new Rectangle((int)(NavigatorPanel.Instance.GridDrawOffsetX - NavigatorPanel.Instance.PlayOffset - 32), LayoutManager.Instance.GridCellsClickableArea.Y, 32, LayoutManager.Instance.GridCellsClickableArea.Height), Color.White);
                }
                Automatone.Instance.SpriteBatch.End();
            }
        }
    }
}