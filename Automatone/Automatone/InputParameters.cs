using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class InputParameters
    {
        //Song Parameters
        public static double songLength = 0.5;
        public static double structuralVar = 0.2;
        public static double songRhythmVariety = 0.5;
        public static double songMelodyVariety = 0.5;
        public static double songLengthVariance = 0.5;

        //Verse Parameters
        public static double meanVerseLength = 0.5;
        public static double verseLengthVariance = 0.5;
        public static double verseRhythmVariety = 0.5;
        public static double verseMelodyVariety = 0.5;

        //Phrase Parameters
        public static double meanPhraseLength = 0.5;
        public static double phraseLengthVariance = 0.5;
        //public static double phraseDistinctiveness = 0.5; //not yet using this?
        public static double phraseRhythmVariety = 0.5;
        public static double phraseMelodyVariety = 0.5;

        //Note Parameters
        public static double meanNoteLength = 0.5;
        public static double noteLengthVariance = 0.5;

        //Rhythm
        public static double rhythmObedience = 0.5;
        
        //Melody
        public static double chordalityObedience = 0.9;
        public static double tonalityObedience = 0.9;
    }
}
