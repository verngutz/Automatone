using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class InputParameters
    {
        // Song Parameters
        public static double songLength = 0.1;
        public static double structuralVar = 0.2;
        public static double songRhythmVariety = 0.5;

        //Verse Parameters
        public static double meanVerseLength = 0.5;
        public static double verseLengthVariance = 0.5;
        public static double verseRhythmVariety = 0.5;

        //Phrase Parameters
        public static double meanPhraseLength = 0.5;
        public static double phraseLengthVariance = 0.5;
        public static double phraseDistinctiveness = 0.5;
    }
}
