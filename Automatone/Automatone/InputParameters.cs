using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class InputParameters
    {
        //Song Parameters
        public static double timeSignatureN = 4.0;
        public static double timeSignatureD = 4.0;
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
        public static double rhythmObedience = 0.8;
        
        //Melody
        public static double chordalityObedience = 1;
        public static double tonalityObedience = 1;

        //Harmony
        public static double seventhChordProbablility = 0.2;
        /*public static double meanBeatharmonicCovariance = 0.9;
        public static double beatHarmonicCovarianceOffsetDivisor = 10;
        public static double randomModulationProbability = 0.01;
        public static double perfectFifthModulationProbability = 0.20;
        public static double perfectFourthModulationProbability = 0.15;
        public static double relativeModeModulationProbability = 0.1;
        public static double absoluteModeModulationProbability = 0.04;*/
    }
}
