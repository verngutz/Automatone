using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class InputParameters
    {
        //Song Parameters
        public double songSpeed;
        public double songSpeedVariance;
        public double timeSignatureN;
        public double timeSignatureD;
        public double meanSongLength;
        public double structuralVariance;
        public double songRhythmVariance;
        public double songMelodyVariance;
        public double songLengthVariance;

        //Verse Parameters
        public double meanVerseLength;
        public double verseLengthVariance;
        public double verseRhythmVariance;
        public double verseMelodyVariance;

        //Phrase Parameters
        public double meanPhraseLength;
        public double phraseLengthVariance;
        public double phraseRhythmVariance;
        public double phraseMelodyVariance;
        //public double phraseDistinctiveness; //not yet using this?

        //Measure Parameters
        public double measureRhythmVariance;
        public double measureMelodyVariance;

        //Note Parameters
        public double meanNoteLength;
        public double noteLengthVariance;

        //Rhythm
        public double rhythmObedience;
        
        //Melody
        public double chordalityObedience;
        public double tonalityObedience;
        public double meanPitchContiguity;

        //Harmony
        public double seventhChordProbability;
        /*public double meanBeatharmonicCovariance;
        public double beatHarmonicCovarianceOffsetDivisor;
        public double randomModulationProbability;
        public double perfectFifthModulationProbability;
        public double perfectFourthModulationProbability;
        public double relativeModeModulationProbability;
        public double absoluteModeModulationProbability;*/

        public InputParameters()//Set Default Values
        {
            //Song Parameters
            songSpeed = 0.5;
            songSpeedVariance = 0.5;
            timeSignatureN = 4.0;
            timeSignatureD = 4.0;
            meanSongLength = 0.5;
            structuralVariance = 0.5;
            songRhythmVariance = 0.5;
            songMelodyVariance = 0.5;
            songLengthVariance = 0.5;

            //Verse Parameters
            meanVerseLength = 0.5;
            verseLengthVariance = 0.5;
            verseRhythmVariance = 0.5;
            verseMelodyVariance = 0.5;

            //Phrase Parameters
            meanPhraseLength = 0.5;
            phraseLengthVariance = 0.5;
            phraseRhythmVariance = 0.5;
            phraseMelodyVariance = 0.5;
            //phraseDistinctiveness = 0.5; //not yet using this?

            //Measure Parameters
            measureRhythmVariance = 0.5;
            measureMelodyVariance = 0.5;

            //Note Parameters
            meanNoteLength = 0.5;
            noteLengthVariance = 0.5;

            //Rhythm
            rhythmObedience = 0.9;
        
            //Melody
            chordalityObedience = 0.9;
            tonalityObedience = 0.9;
            meanPitchContiguity = 0.5;

            //Harmony
            seventhChordProbability = 0.1;
            /*meanBeatharmonicCovariance = 0.9;
            beatHarmonicCovarianceOffsetDivisor = 10;
            randomModulationProbability = 0.01;
            perfectFifthModulationProbability = 0.20;
            perfectFourthModulationProbability = 0.15;
            relativeModeModulationProbability = 0.1;
            absoluteModeModulationProbability = 0.04;*/
        }
    }
}
