using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Automatone
{
    public class Verse
    {
        private List<Phrase> verse;

        private CellState[,] grid;
        public CellState[,] Grid { get { return grid; } }

        public Verse(MusicTheory theory, Random rand, Rhythm rhythm, List<double[]> rhythmSeeds)
        {
            //Calculate verse length
            int verseLength = (int)(theory.VERSE_LENGTHINESS * InputParameters.meanVerseLength);
            verseLength += (int)(verseLength * ((rand.NextDouble() - 0.5) * InputParameters.verseLengthVariance));
            
            //Select rhythms
            List<double[]> selectedRhythmSeeds = new List<double[]>();
            for (int i = 0; i < 1 + 2 * InputParameters.verseRhythmVariety * verseLength; i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count)));
            }

            //Build cadence curve
		    List<double> cadenceFractalCurve = Enumerable.Repeat<double>(1.0, verseLength).ToList<double>();
		    int x = verseLength;
		    for(int i = 2; i <= Math.Sqrt(verseLength); i++)
		    {
			    while(x % i == 0)
			    {
				    for(int j = 0; j < verseLength; j++)
				    {
					    if((j + 1) % x != 0)
					    {
						    cadenceFractalCurve[j] *= theory.CADENCE_SMOOTHNESS;
					    }
				    }
				    x /= i;
			    }
		    }

            //Create phrases
            verse = new List<Phrase>();
		    for(int i = 0; i < verseLength; i++)
		    {
			    int a = 0;
			    if(rand.NextDouble() > cadenceFractalCurve[i])
			    {
				    a = 1;
			    }
			    double sum = 0;
			    foreach(double j in MusicTheory.CADENCES[a])
			    {
				    sum += j;
			    }
			    double r = rand.NextDouble() * sum;
                bool addDefaultPhrase = true;
			    for(int j = 0; j < 4; j++)
			    {
				    if(r < MusicTheory.CADENCES[a][j])
				    {
                        verse.Add(new Phrase(theory, rand, (MusicTheory.CADENCE_NAMES)j, rhythm, selectedRhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count))));
                        addDefaultPhrase = false;
					    break;
				    }
				    else
				    {
					    r -= MusicTheory.CADENCES[a][j];
				    }
			    }
			    if(addDefaultPhrase)
			    {
                    verse.Add(new Phrase(theory, rand, MusicTheory.CADENCE_NAMES.SILENT, rhythm, selectedRhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count))));
			    }
		    }

            //build grid
            int gridSize = 0;
            foreach (Phrase phrase in verse)
            {
                gridSize += phrase.Grid.GetLength(1);
            }
            grid = new CellState[theory.PIANO_SIZE, gridSize];
            int gridStartPosition = 0;
            foreach (Phrase phrase in verse)
            {
                for (int i = 0; i < phrase.Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < phrase.Grid.GetLength(1); j++)
                    {
                        grid[i, j + gridStartPosition] = phrase.Grid[i, j];
                    }
                }
                gridStartPosition += phrase.Grid.GetLength(1);
            }
        }
    }
}
