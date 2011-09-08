using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Song
    {
        private List<Verse> song;

        private CellState[,] grid;
        public CellState[,] Grid { get { return grid; } }

        public Song(MusicTheory theory, Random rand)
        {
            song = new List<Verse>();

            //Calculate verse length
            int songLength = (int)(InputParameters.songLength * theory.SONG_LENGTHINESS);
            songLength += (int)(songLength * ((rand.NextDouble() - 0.5) * InputParameters.songLengthVariance));

            //generate rhythms
            Rhythm rhythm = new Rhythm(theory);
            List<double[]> rhythmSeeds = new List<double[]>();
            for (int i = 0; i < 1 + 2 * InputParameters.songRhythmVariety * songLength; i++)
            {
                rhythmSeeds.Add(new double[] {rand.NextDouble(),rand.NextDouble()});
            }

            //generate verses
		    List<Verse> verses = new List<Verse>();
		    for(int i = 0; i < 1 + 2 * InputParameters.structuralVar * songLength; i++)
            {
                System.Console.Write("Verse " + i); //remove later
			    verses.Add(new Verse(theory, rand, rhythm, rhythmSeeds));
		    }

            System.Console.Write("Final song:"); //remove later
		
            //select verses to include in song
		    double chorusProb = rand.NextDouble();
            int? prev = null;
		    for(int i = 0; i < songLength; i++)
		    {
			    int curr = (int)(rand.NextDouble() * verses.Count);
                while (chorusProb < theory.CHORUS_EXISTENCE * songLength)
			    {
				    if (prev != null && ((prev == 0  && curr == 0) || prev != 0 && curr != 0))
				    {
                        curr = (int)(rand.NextDouble() * verses.Count);
				    }
                    chorusProb += chorusProb;
			    }
			    song.Add(verses.ElementAt<Verse>(curr));
                System.Console.Write(" " + curr); //remove later
                prev = curr;
		    }

            //build grid
            int gridSize = 0;
            foreach(Verse verse in song)
            {
                gridSize += verse.Grid.GetLength(1);
            }
            grid = new CellState[theory.PIANO_SIZE, gridSize];
            int gridStartPosition = 0;
            foreach (Verse verse in song)
            {
                for(int i = 0; i < verse.Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < verse.Grid.GetLength(1); j++)
                    {
                        grid[i, j + gridStartPosition] = verse.Grid[i, j];
                    }
                }
                gridStartPosition += verse.Grid.GetLength(1);
            }
        }
    }
}
