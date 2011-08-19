using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class ClassicalTheory : MusicTheory
    {
        //Song Constants
        public override int SONG_LENGTHINESS { get { return 10; } }
        public override double CHORUS_EXISTENCE { get { return 1; } }

        //Verse Constants
        public override double CADENCE_SMOOTHNESS { get { return 0.5; } }
        public override int VERSE_LENGTHINESS { get { return 8; } }

        //Phrase Constants
        public override double PHRASE_LENGTHINESS { get { return 8; } }

        //Rhythm
        public override List<double> RHYTHM_CURVE_SAMPLE 
        {
            get
            {
                List<double> sample = new List<double>();
                sample.Add(1);
                sample.Add(0.125);
                sample.Add(0.25);
                sample.Add(0.125);
                sample.Add(0.5);
                sample.Add(0.125);
                sample.Add(0.25);
                sample.Add(0.125);
                return sample;
            }
        }

        //Pitch Range and Offset
        public override int PIANO_SIZE { get { return 60; } }
        public const int NOTENAME_OFFSET = 7;
        public const int OCTAVE_OFFSET = 5;

        public override NoteName getNoteName(int pitchNumber)
        {
            return CHROMATIC_SCALE[(pitchNumber + NOTENAME_OFFSET) % CHROMATIC_SCALE.Length];
        }

        public override int getOctave(int pitchNumber)
        {
            return (pitchNumber + OCTAVE_OFFSET) / CHROMATIC_SCALE.Length + 1;
        }

        //CHROMATIC SCALE
        public static NoteName[] CHROMATIC_SCALE = 
	    {
		    NoteName.NOTE_C,
		    NoteName.NOTE_C_SHARP,
		    NoteName.NOTE_D,
		    NoteName.NOTE_D_SHARP,
		    NoteName.NOTE_E,
		    NoteName.NOTE_F,
		    NoteName.NOTE_F_SHARP,
		    NoteName.NOTE_G,
		    NoteName.NOTE_G_SHARP,
		    NoteName.NOTE_A,
		    NoteName.NOTE_A_SHARP,
		    NoteName.NOTE_B
	    };

        //Beat Resolution
        public override int SUBBEATS_PER_MEASURE { get { return 16; } }
        public override double getBeatResolution()
        {
            return 1.0 / SUBBEATS_PER_MEASURE;
        }
    }
}
