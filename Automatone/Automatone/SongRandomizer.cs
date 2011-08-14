using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MoodSwingCoreComponents;
using MoodSwingGUI;

namespace Automatone
{
    public class SongRandomizer : MSAction
    {
        private List<CellState[,]> songCells;
        private GraphicsDeviceManager graphics;
        private MSPanel gridPanel;
        private int cellsize;
        public SongRandomizer(List<CellState[,]> songCells, GraphicsDeviceManager graphics, int cellsize)
        {
            this.songCells = songCells;
            this.graphics = graphics;
            this.cellsize = cellsize;
        }

        public void PerformAction(Game game)
        {
            System.Console.WriteLine("Create MIDI");
            //Create MIDI

            StreamWriter sw = new StreamWriter("sample.txt");
            sw.WriteLine("mthd\n\tversion 1\n\tunit 192\nend mthd\n");
            sw.WriteLine("mtrk\n\ttact 4 / 4 24 8\n\tbeats 140\n\tkey \"Cmaj\"\nend mtrk\n");
            const int SEED = 40;
            MSRandom random = new MSRandom(SEED);
            Theory theory = new BasicWesternTheory(random);
            SongGenerator sg = new SongGenerator(random);
            String song = sg.generateSong(theory, out songCells);
            sw.Write(song);
            sw.Close();

            Process txt2midi = new Process();
            System.Console.WriteLine(Environment.CurrentDirectory);
            txt2midi.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            txt2midi.StartInfo.FileName = "TXT2MIDI.exe";
            txt2midi.StartInfo.Arguments = "sample.txt SAMPLE.MID";
            txt2midi.StartInfo.UseShellExecute = true;
            //txt2midi.Start();
            System.Console.WriteLine("txt2midi.Start()");

            gridPanel = new MSPanel(null, new Rectangle(0, 150, songCells.ElementAt<CellState[,]>(0).GetLength(1) * CELLSIZE, songCells.ElementAt<CellState[,]>(0).GetLength(0) * CELLSIZE), null, Shape.RECTANGULAR, spriteBatch, game);

            Random r = new Random();
            for (int i = 0; i < songCells.ElementAt<CellState[,]>(0).GetLength(0); i++)
            {
                for (int j = 0; j < songCells.ElementAt<CellState[,]>(0).GetLength(1); j++)
                {
                    gridPanel.AddComponent(
                        new MSImageHolder(
                            new Rectangle(
                                gridPanel.BoundingRectangle.X + j * cellsize,
                                gridPanel.BoundingRectangle.Y + i * cellsize,
                                cellsize, cellsize),
                            (songCells.ElementAt<CellState[,]>(0)[i, j] != CellState.SILENT ? game.Content.Load<Texture2D>("lightbox") : game.Content.Load<Texture2D>("darkbox")),
                            (game as Automatone).spriteBatch,
                            game));
                }
            }

            graphics.PreferredBackBufferWidth = songCells.ElementAt<CellState[,]>(0).GetLength(1) * cellsize;
            graphics.PreferredBackBufferHeight = 150 + songCells.ElementAt<CellState[,]>(0).GetLength(0) * cellsize;
        }
    }
}
