using System.Collections.Generic;

namespace Automatone
{
    public class ClassicalTheory : MusicTheory
    {
        //Song Constants
        public override int MIN_TEMPO { get { return 40; } }
        public override int MAX_TEMPO { get { return 160; } }
        public override int SONG_LENGTHINESS { get { return 10; } }
        public override double CHORUS_EXISTENCE { get { return 1; } }

        //Verse Constants
        public override double CADENCE_SMOOTHNESS { get { return 0.5; } }
        public override int VERSE_LENGTHINESS { get { return 8; } }

        //Phrase Constants
        public override int PHRASE_LENGTHINESS { get { return 8; } }

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
                return sample;
            }
        }
        
        //Melody
        public override double[] MELODY_BIAS_SAMPLE
        {
            get
            {
                return new double[]{0.7,1};
            }
        }
        public override int PITCH_CONTIGUITY { get { return 12; } }
    }
}
