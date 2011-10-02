using System.Collections.Generic;

namespace Automatone.Music
{
    public abstract class MusicTheory
    {
        //Song Constants
        public abstract int MIN_TEMPO { get; }
        public abstract int MAX_TEMPO { get; }
        public abstract int SONG_LENGTHINESS { get; }
        public abstract double CHORUS_EXISTENCE { get; }

        //Verse Constants
        public abstract double CADENCE_SMOOTHNESS { get; }
        public abstract int VERSE_LENGTHINESS { get; }

        //Phrase Constants
        public abstract int PHRASE_LENGTHINESS { get; }

        //Part Constants
        public abstract int PART_COUNT { get; }
        public abstract int PART_OCTAVE_RANGE { get; }

        //Note Constants
        public abstract double NOTE_LENGTHINESS { get; }

        //Cadences
        public enum CADENCE_NAMES { HALF, PERFECT_AUTHENTIC, IMPERFECT_AUTHENTIC, PLAGAL, DECEPTIVE, SILENT };
        public static double[][] CADENCES = new double[][] { new double[] { 0, 1, 0.8, 0.5, 0.01 }, new double[] { 0.8, 0.8, 0.6, 0.2, 0.2 } };

        //Rhythm
        public abstract List<double> RHYTHM_CURVE_SAMPLE { get; }

        //Melody
        public abstract double[] MELODY_BIAS_SAMPLE { get; }
        public abstract int PITCH_CONTIGUITY { get; }

        //Music
        public static Dictionary<NoteName, int> CHROMATIC_EQUIVALENTS = createChromaticEquivalents();
        private static Dictionary<NoteName, int> createChromaticEquivalents()
        {
            Dictionary<NoteName, int> equivalents = new Dictionary<NoteName, int>();

            equivalents.Add(NoteName.C, 0);
            equivalents.Add(NoteName.C_SHARP, 1);
            equivalents.Add(NoteName.D, 2);
            equivalents.Add(NoteName.D_SHARP, 3);
            equivalents.Add(NoteName.E, 4);
            equivalents.Add(NoteName.F, 5);
            equivalents.Add(NoteName.F_SHARP, 6);
            equivalents.Add(NoteName.G, 7);
            equivalents.Add(NoteName.G_SHARP, 8);
            equivalents.Add(NoteName.A, 9);
            equivalents.Add(NoteName.A_SHARP, 10);
            equivalents.Add(NoteName.B, 11);

            return equivalents;
        }
        public const int DIMINISHED_UNISON_INTERVAL = -1;
        public const int PERFECT_UNISON_INTERVAL = 0;
        public const int AUGMENTED_UNISON_INTERVAL = 1;
        public const int DIMINISHED_SECOND_INTERVAL = 0;
        public const int MINOR_SECOND_INTERVAL = 1;
        public const int MAJOR_SECOND_INTERVAL = 2;
        public const int AUGMENTED_SECOND_INTERVAL = 3;
        public const int DIMINISHED_THIRD_INTERVAL = 2;
        public const int MINOR_THIRD_INTERVAL = 3;
        public const int MAJOR_THIRD_INTERVAL = 4;
        public const int AUGMENTED_THIRD_INTERVAL = 5;
        public const int DIMINISHED_FOURTH_INTERVAL = 4;
        public const int PERFECT_FOURTH_INTERVAL = 5;
        public const int AUGMENTED_FOURTH_INTERVAL = 6;
        public const int DIMINISHED_FIFTH_INTERVAL = 6;
        public const int PERFECT_FIFTH_INTERVAL = 7;
        public const int AUGMENTED_FIFTH_INTERVAL = 8;
        public const int DIMINIHED_SIXTH_INTERVAL = 7;
        public const int MINOR_SIXTH_INTERVAL = 8;
        public const int MAJOR_SIXTH_INTERVAL = 9;
        public const int AUGMENTED_SIXTH_INTERVAL = 10;
        public const int DIMINISHED_SEVENTH_INTERVAL = 9;
        public const int MINOR_SEVENTH_INTERVAL = 10;
        public const int MAJOR_SEVENTH_INTERVAL = 11;
        public const int AUGMENTED_SEVENTH_INTERVAL = 12;
        public const int DIMINISHED_OCTAVE_INTERVAL = 11;
        public const int PERFECT_OCTAVE_INTERVAL = 12;
        public const int AUGMENTED_OCTAVE_INTERVAL = 13;

        public const int WHOLE_STEP = 2;
        public const int HALF_STEP = 1;

        public enum SCALE_MODE { MAJOR, NATURAL_MINOR, HARMONIC_MINOR, MELODIC_MINOR, GENERIC_MINOR }
        public static Dictionary<SCALE_MODE, int[]> SCALE_INTERVALS = createScaleIntervals();
        private static Dictionary<SCALE_MODE, int[]> createScaleIntervals()
        {
            Dictionary<SCALE_MODE, int[]> intervals = new Dictionary<SCALE_MODE, int[]>();

            intervals.Add(SCALE_MODE.MAJOR, new int[] { 2, 2, 1, 2, 2, 2 });
            intervals.Add(SCALE_MODE.NATURAL_MINOR, new int[] { 2, 1, 2, 2, 1, 2 });
            intervals.Add(SCALE_MODE.HARMONIC_MINOR, new int[] { 2, 1, 2, 2, 1, 3 });
            intervals.Add(SCALE_MODE.MELODIC_MINOR, new int[] { 2, 1, 2, 2, 2, 2 });
            intervals.Add(SCALE_MODE.GENERIC_MINOR, new int[] { 2, 1, 2, 2, 1, 2, 1 });

            return intervals;
        }
        public static Dictionary<SCALE_MODE, int[]> CIRCLE_PROGRESSION = createCircleProgression();
        private static Dictionary<SCALE_MODE, int[]> createCircleProgression()
        {
            Dictionary<SCALE_MODE, int[]> intervals = new Dictionary<SCALE_MODE, int[]>();

            intervals.Add(SCALE_MODE.MAJOR, new int[] { 0, 3, 6, 2, 5, 1, 4 });
            intervals.Add(SCALE_MODE.NATURAL_MINOR, new int[] { 0, 3, 6, 2, 5, 1, 4 });
            intervals.Add(SCALE_MODE.HARMONIC_MINOR, new int[] { 0, 3, 6, 2, 5, 1, 4 });
            intervals.Add(SCALE_MODE.MELODIC_MINOR, new int[] { 0, 3, 6, 2, 5, 1, 4 });
            intervals.Add(SCALE_MODE.GENERIC_MINOR, new int[] { 0, 3, 6, 2, 5, 1, 4 });

            return intervals;
        }


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
