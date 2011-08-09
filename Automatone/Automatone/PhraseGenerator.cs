using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class PhraseGenerator
    {
        public CellState[,] generatePhrase(Theory theory)
        {
            CellState[,] phrase = theory.initialize();
            theory.Evolve(phrase);
            return phrase;
        }
    }
}
