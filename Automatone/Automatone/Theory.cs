using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoodSwingCoreComponents;

namespace Automatone
{
    public abstract class Theory
    {
        protected static MSRandom random;
        public abstract double getBeatResolution();
        public abstract CellState[,] initialize();
        public abstract void Evolve(CellState[,] songCells);
        public abstract NoteName getNoteName(int pitchNumber);
        public abstract int getOctave(int pitchNumber);

        public Theory(MSRandom rand)
        {
            random = rand;
        }
    }
}
