using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Measure
    {
        private CellState[,] grid;
        public CellState[,] Grid { get { return grid; } }

        public Measure(MusicTheory theory, Random rand, Rhythm rhythm, double[] rhythmSeed, List<NoteName> chord)
        {
            System.Console.WriteLine("\t\t" + (int)(rhythmSeed[0]*1000) + ":" + (int)(rhythmSeed[1]*1000)); //remove later

            int measureLength = Automatone.SUBBEATS_PER_MEASURE;

            double[] rhythmCurve = rhythm.GetRhythmCurve(measureLength);

            double[] thisSeed = new double[rhythmSeed.Length];
            rhythmSeed.CopyTo(thisSeed, 0);

            grid = new CellState[theory.PIANO_SIZE, measureLength];
            for (int i = 0; i < measureLength; i++)
            {
                //get next seedvalue
                for (int j = 0; j < 2; j++)
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
                            grid[pitch, Math.Min(i + k, measureLength - 1)] = CellState.HOLD;
                    }
                }

                //madaya to. left hand. :))
                for (int j = 0; j < 4 && i % 4 == 0; j++)
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
                            grid[pitch, Math.Min(i + k, measureLength - 1)] = CellState.HOLD;
                    }
                }
            }
        }
    }
}
