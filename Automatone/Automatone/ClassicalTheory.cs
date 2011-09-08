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
                sample.Add(0);
                sample.Add(0);
                sample.Add(1);
                sample.Add(0);
                sample.Add(0);
                sample.Add(1);
                sample.Add(0);
                sample.Add(1);
                sample.Add(.25);
                sample.Add(.5);
                sample.Add(.25);
                sample.Add(1);
                sample.Add(.25);
                sample.Add(.5);
                sample.Add(.25);
                return sample;
            }
        }

        //Pitch Range and Offset
        public override int PIANO_SIZE { get { return 60; } }
        public const int NOTENAME_OFFSET = 7;
        public const int OCTAVE_OFFSET = 5;

        //CHROMATIC SCALE
        public static NoteName[] CHROMATIC_SCALE = 
	    {
		    NoteName.C,
		    NoteName.C_SHARP,
		    NoteName.D,
		    NoteName.D_SHARP,
		    NoteName.E,
		    NoteName.F,
		    NoteName.F_SHARP,
		    NoteName.G,
		    NoteName.G_SHARP,
		    NoteName.A,
		    NoteName.A_SHARP,
		    NoteName.B
	    };
    }
}
