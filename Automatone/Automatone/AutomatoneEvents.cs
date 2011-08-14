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
        public SongRandomizer(List<CellState[,]> songCells)
        {
            this.songCells = songCells;
        }

        public void PerformAction(Game game)
        {
            System.Console.WriteLine("Create MIDI");

            (game as Automatone).sequencer.StopMidi();

            StreamWriter sw = new StreamWriter("sample.txt");
            sw.WriteLine("mthd\n\tversion 1\n\tunit 192\nend mthd\n");
            sw.WriteLine("mtrk\n\ttact 4 / 4 24 8\n\tbeats 140\n\tkey \"Cmaj\"\nend mtrk\n");
            const int SEED = 40;
            MSRandom random = new MSRandom();
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
            txt2midi.Start();

            (game as Automatone).gameScreen.RemoveComponent((game as Automatone).gameScreen.gridPanel);

            (game as Automatone).gameScreen.gridPanel = new MSPanel(null, new Rectangle(0, 150, songCells.ElementAt<CellState[,]>(0).GetLength(1) * MainScreen.CELLSIZE, songCells.ElementAt<CellState[,]>(0).GetLength(0) * MainScreen.CELLSIZE), null, Shape.RECTANGULAR, (game as Automatone).spriteBatch, game);

            int xOffset = 0;
            for (int x = 0; x < songCells.Count; x++)
            {
                for (int i = 0; i < songCells.ElementAt<CellState[,]>(x).GetLength(0); i++)
                {
                    for (int j = 0; j < songCells.ElementAt<CellState[,]>(x).GetLength(1); j++)
                    {
                        (game as Automatone).gameScreen.gridPanel.AddComponent(
                            new MSImageHolder(
                                new Rectangle(
                                    (game as Automatone).gameScreen.gridPanel.BoundingRectangle.X + j * MainScreen.CELLSIZE + xOffset,
                                    (game as Automatone).gameScreen.gridPanel.BoundingRectangle.Y + i * MainScreen.CELLSIZE,
                                    MainScreen.CELLSIZE, MainScreen.CELLSIZE),
                                (songCells.ElementAt<CellState[,]>(x)[i, j] != CellState.SILENT ? game.Content.Load<Texture2D>("lightbox") : game.Content.Load<Texture2D>("darkbox")),
                                (game as Automatone).spriteBatch,
                                game));
                    }
                }
                xOffset += songCells.ElementAt<CellState[,]>(x).GetLength(1) * MainScreen.CELLSIZE;
            }

            (game as Automatone).gameScreen.AddComponent((game as Automatone).gameScreen.gridPanel);

            (game as Automatone).graphics.PreferredBackBufferWidth = 800;
            (game as Automatone).graphics.PreferredBackBufferHeight = 150 + songCells.ElementAt<CellState[,]>(0).GetLength(0) * MainScreen.CELLSIZE;
            (game as Automatone).graphics.ApplyChanges();
        }
    }

    public class Play : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.LoadMidi("sample.mid");
            (game as Automatone).sequencer.PlayMidi();
        }
    }

    public class Stop : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.StopMidi();
        }
    }

    public class MoveLeft : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).gameScreen.GridMove = true;
            (game as Automatone).gameScreen.MoveDirection = false;
        }
    }

    public class MoveRight : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).gameScreen.GridMove = true;
            (game as Automatone).gameScreen.MoveDirection = true;
        }
    }
}
