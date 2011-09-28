namespace Automatone
{
    public class InputParameters
    {
        //Song Parameters
        public double songSpeed = 0.5;
        public double songSpeedVariance = 0.5;
        public double timeSignatureN = 4.0;
        public double timeSignatureD = 4.0;
        public double meanSongLength = 0.5;
        public double structuralVariance = 0.5;
        public double songRhythmVariance = 0.5;
        public double songMelodyVariance = 0.5;
        public double songLengthVariance = 0.5;

        //Verse Parameters
        public double meanVerseLength = 0.5;
        public double verseLengthVariance = 0.5;
        public double verseRhythmVariance = 0.5;
        public double verseMelodyVariance = 0.5;

        //Phrase Parameters
        public double meanPhraseLength = 0.5;
        public double phraseLengthVariance = 0.5;
        public double phraseRhythmVariance = 0.5;
        public double phraseMelodyVariance = 0.5;
        //public double phraseDistinctiveness = 0.5; //not yet using this?

        //Measure Parameters
        public double measureRhythmVariance = 0.5;
        public double measureMelodyVariance = 0.5;

        //Part Parameters
        public double homophony = 0.5;
        public double polyphony = 0.5;
        public double beatDefinition = 0.5;
        //Per-part Parameters
        public double meanPartRhythmCrowdedness = 0.5;
        public double partRhythmCrowdednessVariance = 0.5;
        public double partNoteLengthVariance = 0.5;
        public double meanPartOctaveRange = 0.5;
        public double partOctaveRangeVariance = 0.5;
        public double forceChordChance = 0;
        public double forceDiatonicChance = 0;

        //Note Parameters
        public double meanNoteLength = 0.5;
        public double noteLengthVariance = 0.5;

        //Rhythm
        public double rhythmObedience = 0.9;
        
        //Melody
        public double chordalityObedience = 0.9;
        public double tonalityObedience = 0.9;
        public double meanPitchContiguity = 0.5;

        //Harmony
        public double seventhChordProbability = 0.1;
        /*public double meanBeatharmonicCovariance = 0.9;
        public double beatHarmonicCovarianceOffsetDivisor = 10;
        public double randomModulationProbability = 0.01;
        public double perfectFifthModulationProbability = 0.20;
        public double perfectFourthModulationProbability = 0.15;
        public double relativeModeModulationProbability = 0.1;
        public double absoluteModeModulationProbability = 0.04;*/

        private InputParameters() { }

        private static InputParameters instance = null;

        public static InputParameters Instantiate()
        {
            if (instance == null)
            {
                instance = new InputParameters();
            }
            return instance;
        }
    }
}
