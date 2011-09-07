using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Phrase
    {
        private List<Measure> phrase;

        private CellState[,] grid;
        public CellState[,] Grid { get { return grid; } }

        public Phrase(MusicTheory theory, Random rand, MusicTheory.CADENCE_NAMES c, Rhythm rhythm, double[] rhythmSeed)
        {
            phrase = new List<Measure>();

            //Calculate phrase length
            int phraseLength = (int)(theory.PHRASE_LENGTHINESS * InputParameters.meanPhraseLength);
            phraseLength += (int)(phraseLength * ((rand.NextDouble() - 0.5) * InputParameters.phraseLengthVariance));

            Harmony harm = new Harmony(theory, rand);
            harm.initializeHarmony(phraseLength * Automatone.SUBBEATS_PER_MEASURE);
            List<List<NoteName>> progression = harm.chordProgression;

            grid = new CellState[theory.PIANO_SIZE, phraseLength * Automatone.SUBBEATS_PER_MEASURE];

            for (int i = 0; i < phraseLength; i++)
            {
                int chordNumber = (i / phraseLength) * progression.Count;
                List<NoteName> chord = progression.ElementAt<List<NoteName>>(chordNumber);

                phrase.Add(new Measure(theory, rand, rhythm, rhythmSeed, chord));
            }

            int gridSize = 0;
            foreach (Measure m in phrase)
            {
                gridSize += m.Grid.GetLength(1);
            }
            grid = new CellState[theory.PIANO_SIZE, gridSize];
            int gridStartPosition = 0;
            foreach (Measure m in phrase)
            {
                for (int i = 0; i < m.Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < m.Grid.GetLength(1); j++)
                    {
                        grid[i, j + gridStartPosition] = m.Grid[i, j];
                    }
                }
                gridStartPosition += m.Grid.GetLength(1);
            }
        }
    }
}
