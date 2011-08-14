using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class VerseGenerator
    {
        public CellState[,] generateVerse(Theory theory)
        {
            PhraseGenerator pg = new PhraseGenerator();
            return pg.generatePhrase(theory);
        }
    }
}
