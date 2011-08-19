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

        public Verse(MusicTheory theory, Random rand)
        {
            verse = new List<Phrase>();
            int verseLength = (int)(theory.MEAN_VERSE_LENGTHINESS * InputParameters.meanVerseLength);
            verseLength += (int)(verseLength * ((rand.Next() - 0.5) * InputParameters.verseLengthVariance));
		    List<double> fractalCurve = Enumerable.Repeat<double>(1.0, verseLength).ToList<double>();


            
		    int x = verseLength;
		    for(int i = 2; i <= Math.Sqrt(verseLength); i++)
		    {
			    while(x % i == 0)
			    {
				    for(int j = 0; j < verseLength; j++)
				    {
					    if((j + 1) % x != 0)
					    {
						    fractalCurve[j] *= theory.CADENCE_SMOOTHNESS;
					    }
				    }
				    x /= i;
			    }
		    }
		    for(int i = 0; i < verseLength; i++)
		    {
			    int a = 0;
			    if(rand.NextDouble() > fractalCurve[i])
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
                        verse.Add(new Phrase((MusicTheory.CADENCE_NAMES)j));//change this to choose from random pre generated phrases
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
				    verse.Add(new Phrase(MusicTheory.CADENCE_NAMES.SILENT));//this too.
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
