using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Phrase
    {
        private CellState[,] grid;
        public CellState[,] Grid { get { return grid; } }

        public Phrase(MusicTheory theory, Random rand, MusicTheory.CADENCE_NAMES c, Rhythm rhythm)
        {
            //Calculate phrase length
            int phraseLength = (int)(theory.PHRASE_LENGTHINESS * InputParameters.meanPhraseLength);
            phraseLength += (int)(phraseLength * ((rand.NextDouble() - 0.5) * InputParameters.phraseLengthVariance));
            phraseLength *= theory.SUBBEATS_PER_MEASURE;

            double[] rhythmCurve = rhythm.GetRhythmCurve(phraseLength);


            grid = new CellState[theory.PIANO_SIZE, phraseLength];
            for (int i = 0; i < phraseLength; i++)
            {
                if (rand.NextDouble() < rhythmCurve[i])
                {
                    grid[30 + (int)(rand.NextDouble() * 10), i] = CellState.START;
                }
            }
        }
    }
}
