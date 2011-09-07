#define USESEED

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
        private CellState[,] songCells;

        private const int SEED = 40;
        private const int TEMPO_DIVIDEND = 60000000;

        public SongRandomizer() { }

        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.StopMidi();
            (game as Automatone).gameScreen.ScrollWithMidi = false;

            StreamWriter sw = new StreamWriter("sample.mtx");
            sw.WriteLine("MFile 1 2 192");
            sw.WriteLine("MTrk");
            sw.WriteLine("0 TimeSig 4/4 24 8");
            sw.WriteLine("0 Tempo " + TEMPO_DIVIDEND / Automatone.TEMPO);
            sw.WriteLine("0 KeySig 0 major");
            sw.WriteLine("0 Meta TrkEnd");
            sw.WriteLine("TrkEnd");

#if USESEED
            String song = SongGenerator.GenerateSong(new Random(SEED), new ClassicalTheory(), out songCells);
#else
            String song = SongGenerator.GenerateSong(new Random(), new ClassicalTheory(), out songCells);
#endif
            sw.Write(song);
            sw.Close();

            if(File.Exists("sample.mid"))
                File.Delete("sample.mid");

            Process txt2midi = new Process();
            System.Console.WriteLine(Environment.CurrentDirectory);
            txt2midi.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            txt2midi.StartInfo.FileName = "Mtx2Midi.exe";
            txt2midi.StartInfo.Arguments = "sample.mtx";
            txt2midi.StartInfo.UseShellExecute = true;
            txt2midi.Start();

            (game as Automatone).gameScreen.DestroyGridButtons();

            (game as Automatone).gameScreen.gridHeight = songCells.GetLength(0);
            (game as Automatone).gameScreen.gridWidth = songCells.GetLength(1);
            (game as Automatone).gameScreen.gridOffset = Vector2.Zero;
            (game as Automatone).gameScreen.playOffset = 0;

            for (int i = 0; i < songCells.GetLength(0); i++)
            {
                for (int j = 0; j < songCells.GetLength(1); j++)
                {
                    Texture2D CellTexture = null;
                    switch (songCells[i, j])
                    {
                        case CellState.SILENT:
                            CellTexture = game.Content.Load<Texture2D>("darkbox");
                            break;
                        case CellState.START:
                            CellTexture = game.Content.Load<Texture2D>("lightbox");
                            break;
                        case CellState.HOLD:
                            CellTexture = game.Content.Load<Texture2D>("holdbox");
                            break;
                    }
                    (game as Automatone).gameScreen.AddGridButton(
                        new MSButton(null, null, new Rectangle(), CellTexture, CellTexture, CellTexture, null, Shape.RECTANGULAR, (game as Automatone).spriteBatch, game),
                            j, (songCells.GetLength(0) - i - 1));
                }
            }

            txt2midi.Kill();
            (game as Automatone).sequencer.LoadMidi("sample.mid");
        }
    }

    public class PlayMidi : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.PlayMidi();
            (game as Automatone).gameScreen.ScrollWithMidi = true;
        }
    }

    public class StopMidi : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.StopMidi();
            (game as Automatone).gameScreen.ScrollWithMidi = false;
        }
    }

    public class PauseMidi : MSAction
    {
        public void PerformAction(Game game)
        {
            (game as Automatone).sequencer.PauseMidi();
            (game as Automatone).gameScreen.ScrollWithMidi = false;
        }
    }
}
