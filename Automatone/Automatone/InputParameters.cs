using System;

namespace Automatone
{
    [Serializable]
    public class InputParameters
    {
        //Global Song Parameters
        public ushort tempo = 120;
        public int timeSignatureN = 4;
        public int timeSignatureD = 4;
        public double TimeSignature { get { return (timeSignatureN / (double)timeSignatureD); } }
        
        //Song Parameters
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

        private InputParameters() { }

        private static InputParameters instance = null;

        public static InputParameters Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputParameters();
                }
                return instance;
            }
        }

        public static void LoadInstance(InputParameters instance)
        {
            InputParameters.instance = instance;
        }
    }
}
