using System;
using Duet.Audio_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.Graphics.SpecialEffects.Particles;
using NuclexUserInterfaceExtension;

using Automatone.Music;

namespace Automatone.GUI
{
    public class GridPanel : DrawableGameComponent
    {
        public bool ScrollWithMidi { set; get; }

        private MouseState oldMouseState;
        private MouseState newMouseState;

        private const byte DimensionX = 1;
        private const byte DimensionY = 0;

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
            private static Texture2D holdCell;
            private static Texture2D holdCellEnd;
            private static Texture2D gridPanelBackground;

            private int? gridInputStartXIndex;
            private int? gridInputEndXIndex;
            private int? gridInputYIndex;

            public void LoadContent()
            {
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
                    if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed && GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Released)
                    {
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
            private Texture2D labelsBackground;
            private SpriteFont labelFont;
            private const bool sharpLabels = true;

            public void LoadContent()
            {
                labelsBackground = Automatone.Instance.Content.Load<Texture2D>("BlackPixel");
                labelFont = Automatone.Instance.Content.Load<SpriteFont>("LabelFont");
            }

            public void Dispose()
            {
                if (labelsBackground != null) 
                    labelsBackground.Dispose();
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin();
                DrawPitchLabel();
                DrawTimeLabel();
                DrawRightBorder();
                DrawBottomBorder();
                Automatone.Instance.SpriteBatch.End();
            }

            private void DrawPitchLabel()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, LayoutManager.Instance.GridLeftBorderBounds, Color.White);

                for (int i = NavigatorPanel.Instance.VerticalClippingStartIndex; i <= NavigatorPanel.Instance.VerticalClippingEndIndex; i++)
                {
                    Vector2 loc = new Vector2(2, (int)((GridPanel.Instance.SongCells.GetLength(DimensionY) - 1 - i) * LayoutManager.CELLHEIGHT + NavigatorPanel.Instance.GridDrawOffsetY));
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
                    Automatone.Instance.SpriteBatch.DrawString(labelFont, letter, loc, Color.White);
                }
            }

            private void DrawTimeLabel()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, LayoutManager.Instance.GridTopBorderBounds, Color.White);

                for (int j = NavigatorPanel.Instance.HorizontalClippingStartIndex; j <= NavigatorPanel.Instance.HorizontalClippingEndIndex; j++)
                {
                    Vector2 loc = new Vector2((int)(j * LayoutManager.CELLWIDTH + NavigatorPanel.Instance.GridDrawOffsetX), 2 + LayoutManager.CONTROLS_AND_GRID_DIVISION);
                    if (j % Automatone.Instance.MeasureLength == 0)
                    {
                        Automatone.Instance.SpriteBatch.DrawString(labelFont, "" + (j / Automatone.Instance.MeasureLength + 1), loc, Color.White);
                    }
                    for (int k = 1; k < Automatone.Instance.MeasureLength / (Automatone.SUBBEATS_PER_WHOLE_NOTE / 4); k++)
                    {
                        if (j % Automatone.Instance.MeasureLength == k * Automatone.SUBBEATS_PER_WHOLE_NOTE / 4)
                        {
                            Automatone.Instance.SpriteBatch.DrawString(labelFont, "♩", loc, Color.Navy);
                        }
                    }
                }
            }

            private void DrawRightBorder()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, LayoutManager.Instance.GridRightBorderBounds, Color.White);
                for (int i = 0; i < (Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) / MusicTheory.OCTAVE_SIZE + 1; i++)
                {
                    if ((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER > NavigatorPanel.Instance.VerticalClippingStartIndex && i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER < NavigatorPanel.Instance.VerticalClippingEndIndex)
                    {
                        int upperBound = Math.Max(LayoutManager.Instance.GridTopBorderBounds.Bottom, GridPanel.Instance.GridToScreenCoordinatesY((i + 1) * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER));
                        int lowerBound = Math.Min(LayoutManager.Instance.GridBottomBorderBounds.Top, GridPanel.Instance.GridToScreenCoordinatesY(i * MusicTheory.OCTAVE_SIZE - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER));
                        Vector2 loc = new Vector2(LayoutManager.Instance.GridRightBorderBounds.Left + 2, (upperBound + lowerBound) / 2);
                        Automatone.Instance.SpriteBatch.DrawString(labelFont, i + "", loc, Color.White);
                    }
                }
            }

            private void DrawBottomBorder()
            {
                Automatone.Instance.SpriteBatch.Draw(labelsBackground, LayoutManager.Instance.GridBottomBorderBounds, Color.White);
            }
        }

        /// <summary>
        /// This class handles the updating and drawing of the particle
        /// system (used to create cool visual effects while notes are played).
        /// </summary>
        private class Particles
        {

            private ParticleSystem<NoteHitParticle> particleSystem;
            private const int PARTICLE_SPAWN_DENSITY = 4;
            private const int PARTICLE_SPAWN_CYCLE = 50;
            private int currentCycle;

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
        }

        /// <summary>
        /// This class handles the cursors which are used to select
        /// sections of the grid for editing.
        /// </summary>
        private class Cursors
        {
            private Texture2D startCursorHead;
            private Texture2D endCursorHead;
            private Texture2D playCursor;

            private const int CURSORHEIGHT = 20;
            private const int CURSORWIDTH = 20;

            private int startIndex;
            private int endIndex;

            public int StartIndex { get { return Math.Min(startIndex, endIndex); } }
            public int EndIndex { get { return Math.Max(endIndex, startIndex); } }

            private Rectangle startDrawRectangle;
            private Rectangle endDrawRectangle;

            public void LoadContent()
            {
                startCursorHead = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_CursorHead");
                endCursorHead = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_CursorHead");
                playCursor = Automatone.Instance.Content.Load<Texture2D>("Grid Panel/Itm_Cursor");
            }

            public void Dispose()
            {
                if (startCursorHead != null) startCursorHead.Dispose();
                if (endCursorHead != null) endCursorHead.Dispose();
                if (playCursor != null) playCursor.Dispose();
            }

            public void Update(GameTime gameTime)
            {
                if (Automatone.Instance.IsActive
                    && Automatone.Instance.Sequencer.State == Sequencer.MidiPlayerState.STOPPED
                    && (GridPanel.Instance.newMouseState.LeftButton != ButtonState.Released
                        || GridPanel.Instance.oldMouseState.LeftButton != ButtonState.Released)
                    && LayoutManager.Instance.GridCursorsClickableArea.Contains(new Point(GridPanel.Instance.newMouseState.X, GridPanel.Instance.newMouseState.Y)))
                {
                    if (GridPanel.Instance.newMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (GridPanel.Instance.oldMouseState.LeftButton == ButtonState.Released)
                        {
                            startIndex = GridPanel.Instance.ScreenToGridCoordinatesX(GridPanel.Instance.newMouseState.X + LayoutManager.CELLWIDTH / 2);
                        }
                        endIndex = GridPanel.Instance.ScreenToGridCoordinatesX(GridPanel.Instance.newMouseState.X + LayoutManager.CELLWIDTH / 2);
                        System.Console.WriteLine(StartIndex + " " + EndIndex);
                    }
                }
            }

            public void Draw(GameTime gameTime)
            {
                Automatone.Instance.SpriteBatch.Begin();
                if (StartIndex != EndIndex 
                    && EndIndex <= GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridRightBorderBounds.Left) 
                    && EndIndex >= GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridLeftBorderBounds.Right))
                {
                    Automatone.Instance.SpriteBatch.Draw(endCursorHead, new Rectangle(GridPanel.Instance.GridToScreenCoordinatesX(EndIndex) - CURSORWIDTH / 2, LayoutManager.Instance.GridCursorsClickableArea.Y, CURSORWIDTH, CURSORHEIGHT), Color.Red);
                }
                if(StartIndex <= GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridRightBorderBounds.Left)
                    && StartIndex >= GridPanel.Instance.ScreenToGridCoordinatesX(LayoutManager.Instance.GridLeftBorderBounds.Right))
                {
                    Automatone.Instance.SpriteBatch.Draw(startCursorHead, new Rectangle(GridPanel.Instance.GridToScreenCoordinatesX(StartIndex) - CURSORWIDTH / 2, LayoutManager.Instance.GridCursorsClickableArea.Y, CURSORWIDTH, CURSORHEIGHT), Color.Green);
                }
                
                if(Automatone.Instance.Sequencer.State != Sequencer.MidiPlayerState.STOPPED)
                {
                    Automatone.Instance.SpriteBatch.Draw(playCursor, new Rectangle((int)(NavigatorPanel.Instance.GridDrawOffsetX - NavigatorPanel.Instance.PlayOffset - 32), LayoutManager.Instance.GridCellsClickableArea.Y, 32, LayoutManager.Instance.GridCellsClickableArea.Height), Color.AliceBlue);
                }
                Automatone.Instance.SpriteBatch.End();
            }
        }
    }
}