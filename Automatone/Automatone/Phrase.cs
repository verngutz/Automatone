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

            Harmony harm = new Harmony(theory, rand);
            harm.initializeHarmony(phraseLength);
            List<List<NoteName>> progression = harm.chordProgression;

            double[] rhythmCurve = rhythm.GetRhythmCurve(phraseLength);

            grid = new CellState[theory.PIANO_SIZE, phraseLength];
            double a = rand.NextDouble();
            for (int i = 0; i < phraseLength; i++)
            {
                int chordNumber = (i / phraseLength) * progression.Count;
                List<NoteName> chord = progression.ElementAt<List<NoteName>>(chordNumber);
                if (a < rhythmCurve[i])
                {
                    for (int j = 0; j < 5; j++)
                    {
                        int pitch = (int)(0 + rand.NextDouble() * 24);
                        while(!chord.Contains(theory.getNoteName(pitch)))
                            pitch = (int)(0 + rand.NextDouble() * 24);
                        grid[pitch, i] = CellState.START;
                        for(int k = 1; k < 4; k++)
                            grid[pitch, Math.Min(i + k, phraseLength - 1)] = CellState.HOLD;
                    }
                }
            }
        }
    }
}
