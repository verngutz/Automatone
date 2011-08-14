using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoodSwingCoreComponents;
using Microsoft.Xna.Framework;
using System.IO;
using System.Diagnostics;

namespace Automatone
{
    public class SongRandomizer : MSAction
    {
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
            String song = sg.generateSong(theory);
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
        }
    }
}
