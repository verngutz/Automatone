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

        //Note Constants
        public override double NOTE_LENGTHINESS { get { return 0.5; } }

        //Rhythm
        public override List<double> RHYTHM_CURVE_SAMPLE
        {
            get
            {
                List<double> sample = new List<double>();
                sample.Add(1);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(0.36);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(0.6);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(0.36);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(1);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(0.36);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(0.6);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                sample.Add(0.36);
                sample.Add(0.1296);
                sample.Add(0.216);
                sample.Add(0.1296);
                return sample;
            }
        }
        
        //Melody
        public override double[] MELODY_BIAS_SAMPLE
        {
            get
            {
                return new double[]{0.5,1};
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
