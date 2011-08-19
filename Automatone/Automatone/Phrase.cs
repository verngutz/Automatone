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

        public Phrase(MusicTheory.CADENCE_NAMES c) 
        {
        }
    }
}
