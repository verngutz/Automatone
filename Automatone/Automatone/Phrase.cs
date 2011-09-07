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

        public Phrase(MusicTheory theory, Random rand, MusicTheory.CADENCE_NAMES c, Rhythm rhythm, double[] rhythmSeed)
        {
            //Calculate phrase length
            int phraseLength = (int)(theory.PHRASE_LENGTHINESS * InputParameters.meanPhraseLength);
            phraseLength += (int)(phraseLength * ((rand.NextDouble() - 0.5) * InputParameters.phraseLengthVariance));
            phraseLength *= Automatone.SUBBEATS_PER_MEASURE;

            Harmony harm = new Harmony(theory, rand);
            harm.initializeHarmony(phraseLength);
            List<List<NoteName>> progression = harm.chordProgression;

            double[] rhythmCurve = rhythm.GetRhythmCurve(phraseLength);

            double[] thisSeed = new double[rhythmSeed.Length];
            rhythmSeed.CopyTo(thisSeed, 0);

            grid = new CellState[theory.PIANO_SIZE, phraseLength];
            for (int i = 0; i < phraseLength; i++)
            {
                int chordNumber = (i / phraseLength) * progression.Count;
                List<NoteName> chord = progression.ElementAt<List<NoteName>>(chordNumber);
                
                //get next seedvalue
                for (int j = 0; j < 2 && i % 2 == 0; j++)
                {
                    thisSeed[i % thisSeed.Length] += thisSeed[(i + 1) % thisSeed.Length];
                    while (thisSeed[i % thisSeed.Length] > 1)
                    {
                        thisSeed[i % thisSeed.Length]--;
                    }

                    if (thisSeed[i % thisSeed.Length] < rhythmCurve[i])
                    {
                    
                        int pitch = (int)(25 + rand.NextDouble() * 16);
                        while (!chord.Contains(new NoteName((byte)(pitch % 12))))
                            pitch = (int)(25 + rand.NextDouble() * 16);
                        grid[pitch, i] = CellState.START;
                        for (int k = 1; k < 4; k++)
                            grid[pitch, Math.Min(i + k, phraseLength - 1)] = CellState.HOLD;
                    }
                }

                //madaya to. left hand. :))
                for (int j = 0; j < 4 && i % 8 == 0; j++)
                {
                    thisSeed[i % thisSeed.Length] += thisSeed[(i + 1) % thisSeed.Length];
                    while (thisSeed[i % thisSeed.Length] > 1)
                    {
                        thisSeed[i % thisSeed.Length]--;
                    }
                    if (thisSeed[i % thisSeed.Length] < rhythmCurve[i])
                    {
                        int pitch = (int)(10 + rand.NextDouble() * 12);
                        while (!chord.Contains(new NoteName((byte)(pitch % 12))))
                            pitch = (int)(10 + rand.NextDouble() * 12);
                        grid[pitch, i] = CellState.START;
                        for (int k = 1; k < 8; k++)
                            grid[pitch, Math.Min(i + k, phraseLength - 1)] = CellState.HOLD;
                    }
                }
            }
        }
    }
}
