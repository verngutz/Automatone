using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public abstract class MusicTheory
    {
        //Song Constants
        public abstract int SONG_LENGTHINESS { get; }
        public abstract double CHORUS_EXISTENCE { get; }

        //Verse Constants
        public abstract double CADENCE_SMOOTHNESS { get; }
        public abstract int VERSE_LENGTHINESS { get; }

        //Cadences
        public enum CADENCE_NAMES { HALF, AUTHENTIC, PLAGAL, DECEPTIVE, SILENT };
        public static double[][] CADENCES = new double[][] { new double[] { 0, 1, 0.3, 0.6 }, new double[] { 1, 0, 0.5, 0.2 } };

        //Phrase Constants
        public abstract double PHRASE_LENGTHINESS { get; }

        //Pitch Range and Offset
        public abstract int PIANO_SIZE { get; }
        public abstract NoteName getNoteName(int pitchNumber);
        public abstract int getOctave(int pitchNumber);

        //Beat Resolution
        public abstract int SUBBEATS_PER_MEASURE { get; }
        public abstract double getBeatResolution();

        //Rhythm
        public abstract List<double> RHYTHM_CURVE_SAMPLE { get; }
    }
}
