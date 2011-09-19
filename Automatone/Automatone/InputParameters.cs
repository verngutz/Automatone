namespace Automatone
{
    public class InputParameters
    {
        //Song Parameters
        public static double songSpeed = 0.3;
        public static double songSpeedVariance = 0.5;
        public static double timeSignatureN = 4.0;
        public static double timeSignatureD = 4.0;
        public static double meanSongLength = 0.5;
        public static double structuralVariance = 0.2;
        public static double songRhythmVariance = 0.5;
        public static double songMelodyVariance = 0.5;
        public static double songLengthVariance = 0.5;

        //Verse Parameters
        public static double meanVerseLength = 0.5;
        public static double verseLengthVariance = 0.5;
        public static double verseRhythmVariance = 0.5;
        public static double verseMelodyVariance = 0.5;

        //Phrase Parameters
        public static double meanPhraseLength = 0.5;
        public static double phraseLengthVariance = 0.5;
        public static double phraseRhythmVariance = 0.5;
        public static double phraseMelodyVariance = 0.5;
        //public static double phraseDistinctiveness = 0.5; //not yet using this?

        //Note Parameters
        public static double meanNoteLength = 0.5;
        public static double noteLengthVariance = 0.5;

        //Rhythm
        public static double rhythmObedience = 0.8;
        
        //Melody
        public static double chordalityObedience = 0.9;
        public static double tonalityObedience = 0.9;
        public static double meanPitchContiguity = 0.5;

        //Harmony
        public static double seventhChordProbability = 0.1;
        /*public static double meanBeatharmonicCovariance = 0.9;
        public static double beatHarmonicCovarianceOffsetDivisor = 10;
        public static double randomModulationProbability = 0.01;
        public static double perfectFifthModulationProbability = 0.20;
        public static double perfectFourthModulationProbability = 0.15;
        public static double relativeModeModulationProbability = 0.1;
        public static double absoluteModeModulationProbability = 0.04;*/
    }
}
