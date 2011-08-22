﻿using System;
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
        private CellState[,] songCells;

        private const int SEED = 40;

        public SongRandomizer(CellState[,] songCells)
        {
            this.songCells = songCells;
        }

        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.StopMidi();

            StreamWriter sw = new StreamWriter("sample.txt");
            sw.WriteLine("mthd\n\tversion 1\n\tunit 192\nend mthd\n");
            sw.WriteLine("mtrk\n\ttact 4 / 4 24 8\n\tbeats " + Automatone.TEMPO + "\n\tkey \"Cmaj\"\nend mtrk\n");

            String song = SongGenerator.generateSong(new Random(), new ClassicalTheory(), out songCells);
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

            (game as Automatone).gameScreen.gridPanel = new MSPanel(null, new Rectangle(0, 150, Math.Min(800, songCells.GetLength(1) * MainScreen.CELLSIZE), Math.Min(450, songCells.GetLength(0) * MainScreen.CELLSIZE)), null, Shape.RECTANGULAR, (game as Automatone).spriteBatch, game);

            for (int i = 0; i < songCells.GetLength(0); i++)
            {
                for (int j = 0; j < songCells.GetLength(1); j++)
                {
                    (game as Automatone).gameScreen.gridPanel.AddComponent(
                        new MSImageHolder(
                            new Rectangle(
                                (game as Automatone).gameScreen.gridPanel.BoundingRectangle.X + j * MainScreen.CELLSIZE,
                                (game as Automatone).gameScreen.gridPanel.BoundingRectangle.Y + (songCells.GetLength(0) - i - 1) * MainScreen.CELLSIZE,
                                MainScreen.CELLSIZE, MainScreen.CELLSIZE),
                            (songCells[i, j] != CellState.SILENT ? (songCells[i, j] == CellState.START ? game.Content.Load<Texture2D>("lightbox") : game.Content.Load<Texture2D>("holdbox")) : game.Content.Load<Texture2D>("darkbox")),
                            (game as Automatone).spriteBatch,
                            game));
                }
            }

            (game as Automatone).gameScreen.AddComponent((game as Automatone).gameScreen.gridPanel);

            (game as Automatone).graphics.PreferredBackBufferWidth = 800;
            (game as Automatone).graphics.PreferredBackBufferHeight = 600;
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
