using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoodSwingCoreComponents;

namespace Automatone
{
    public class BasicWesternTheory : Theory
    {
	    public BasicWesternTheory(MSRandom random) : base(random) { }
	
	    //AUTOMATON CONSTANTS
	    private const int NUM_GENERATIONS = 100;
	    private const int CROWDEDNESS_TOLERANCE = 1;
	
	    //KEYBOARD CONSTANTS
	    public static NoteName NOTE_C              = new NoteName('c', ' ');
	    public static NoteName NOTE_C_SHARP        = new NoteName('c', '#');
	    public static NoteName NOTE_C_FLAT         = new NoteName('b', ' ');
	    public static NoteName NOTE_C_DOUBLE_SHARP = new NoteName('d', ' ');
	    public static NoteName NOTE_C_DOUBLE_FLAT  = new NoteName('a', '#');
	
	    public static NoteName NOTE_D              = new NoteName('d', ' ');
	    public static NoteName NOTE_D_SHARP        = new NoteName('d', '#');
	    public static NoteName NOTE_D_FLAT         = new NoteName('c', '#');
	    public static NoteName NOTE_D_DOUBLE_SHARP = new NoteName('e', ' ');
	    public static NoteName NOTE_D_DOUBLE_FLAT  = new NoteName('c', ' ');
	
	    public static NoteName NOTE_E              = new NoteName('e', ' ');
	    public static NoteName NOTE_E_SHARP        = new NoteName('f', ' ');
	    public static NoteName NOTE_E_FLAT         = new NoteName('d', '#');
	    public static NoteName NOTE_E_DOUBLE_SHARP = new NoteName('f', '#');
	    public static NoteName NOTE_E_DOUBLE_FLAT  = new NoteName('d', ' ');
	
	    public static NoteName NOTE_F              = new NoteName('f', ' ');
	    public static NoteName NOTE_F_SHARP        = new NoteName('f', '#');
	    public static NoteName NOTE_F_FLAT         = new NoteName('e', ' ');
	    public static NoteName NOTE_F_DOUBLE_SHARP = new NoteName('g', ' ');
	    public static NoteName NOTE_F_DOUBLE_FLAT  = new NoteName('d', '#');
	
	    public static NoteName NOTE_G              = new NoteName('g', ' ');
	    public static NoteName NOTE_G_SHARP        = new NoteName('g', '#');
	    public static NoteName NOTE_G_FLAT         = new NoteName('f', '#');
	    public static NoteName NOTE_G_DOUBLE_SHARP = new NoteName('a', ' ');
	    public static NoteName NOTE_G_DOUBLE_FLAT  = new NoteName('f', ' ');
	
	    public static NoteName NOTE_A              = new NoteName('a', ' ');
	    public static NoteName NOTE_A_SHARP        = new NoteName('a', '#');
	    public static NoteName NOTE_A_FLAT         = new NoteName('g', '#');
	    public static NoteName NOTE_A_DOUBLE_SHARP = new NoteName('b', ' ');
	    public static NoteName NOTE_A_DOUBLE_FLAT  = new NoteName('g', ' ');
	
	    public static NoteName NOTE_B              = new NoteName('b', ' ');
	    public static NoteName NOTE_B_SHARP        = new NoteName('c', ' ');
	    public static NoteName NOTE_B_FLAT         = new NoteName('a', '#');
	    public static NoteName NOTE_B_DOUBLE_SHARP = new NoteName('c', '#');
	    public static NoteName NOTE_B_DOUBLE_FLAT  = new NoteName('a', ' ');

	    private const int PIANO_SIZE = 60;
	    private const int NOTENAME_OFFSET = 7;
	    private const int OCTAVE_OFFSET = 9;
	
	    //CHROMATIC SCALE
	    private static NoteName[] CHROMATIC_SCALE = 
	    {
		    NOTE_C,
		    NOTE_C_SHARP,
		    NOTE_D,
		    NOTE_D_SHARP,
		    NOTE_E,
		    NOTE_F,
		    NOTE_F_SHARP,
		    NOTE_G,
		    NOTE_G_SHARP,
		    NOTE_A,
		    NOTE_A_SHARP,
		    NOTE_B
	    };

	    private static Dictionary<NoteName, int> CHROMATIC_EQUIVALENTS = createChromaticEquivalents();
	    private static Dictionary<NoteName, int> createChromaticEquivalents()
	    {
		    Dictionary<NoteName, int> equivalents = new Dictionary<NoteName, int>();
		
		    equivalents.Add(NOTE_C, 0);
		    equivalents.Add(NOTE_C_SHARP, 1);
		    equivalents.Add(NOTE_D, 2);
		    equivalents.Add(NOTE_D_SHARP, 3);
		    equivalents.Add(NOTE_E, 4);
		    equivalents.Add(NOTE_F, 5);
		    equivalents.Add(NOTE_F_SHARP, 6);
		    equivalents.Add(NOTE_G, 7);
		    equivalents.Add(NOTE_G_SHARP, 8);
		    equivalents.Add(NOTE_A, 9);
		    equivalents.Add(NOTE_A_SHARP, 10);
		    equivalents.Add(NOTE_B, 11);
		
		    return equivalents;
	    }
	
	    //INTERVALS
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
	
	    //MELODY
	    private void initializeMelody()
	    {
		    tonality = 0.9;
	    }
	
	    private double tonality; //how much the melody notes conform to the diatonic scale of a given key versus the chromatic scale
	
	    private enum ScaleMode { MAJOR, NATURAL_MINOR, HARMONIC_MINOR, MELODIC_MINOR, GENERIC_MINOR }
	
	    private static List<NoteName> createDiatonicScale(NoteName key, ScaleMode mode)
	    {
		    List<NoteName> scale = new List<NoteName>();
		    scale.Add(key);
		    NoteName curr = key;
            int noteVal;
            CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
		    switch(mode)
		    {
			    case ScaleMode.MAJOR:
				    for(int i = 0; i < 2; i++)
				    {
					    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 3; i++)
				    {
					    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    break;
			    case ScaleMode.NATURAL_MINOR:
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 2; i++)
				    {
					    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    break;
			    case ScaleMode.HARMONIC_MINOR:
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 2; i++)
				    {
					    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    break;
			    case ScaleMode.MELODIC_MINOR:
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 4; i++)
				    {
					    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    break;
			    case ScaleMode.GENERIC_MINOR:
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 2; i++)
				    {
					    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = CHROMATIC_SCALE[(noteVal + HALF_STEP) % CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    break;
		    }
		    return scale;
	    }
	
	    //HARMONY
	    private const double MEAN_BEATHARMONIC_COVARIANCE = 0.9;
	    private const double BEAT_HARMONIC_COVARIANCE_OFFSET_DIVISOR = 10;
	    private const double RANDOM_MODULATION_PROBABILITY = 0.01;
	    private const double PERFECT_FIFTH_MODULATION_PROBABILITY = 0.20;
	    private const double PERFECT_FOURTH_MODULATION_PROBABILITY = 0.15;
	    private const double RELATIVE_MODE_MODULATION_PROBABILITY = 0.1;
	    private const double ABSOLUTE_MODE_MODULATION_PROBABILITY = 0.04;
	
	    private void initializeHarmony(int phraseLength)
	    {
		    jazzyness = 0;
		    beatHarmonicCovariance = MEAN_BEATHARMONIC_COVARIANCE;
		    double offset = random.GetUniform() / BEAT_HARMONIC_COVARIANCE_OFFSET_DIVISOR;
		
		    if(random.GetUniform() < 0.5)
			    beatHarmonicCovariance += offset;
		    else
			    beatHarmonicCovariance -= offset;
			
		    NoteName originalKey;
		    ScaleMode originalMode;
		    if(key == null)
		    {
			    key = CHROMATIC_SCALE[(int)(random.GetUniform() * CHROMATIC_SCALE.Length)];
			    mode = (random.GetUniform() < 0.5 ? ScaleMode.MAJOR : ScaleMode.GENERIC_MINOR);
			    originalKey = key;
			    originalMode = mode;
		    }
		    else
		    {
			    originalKey = new NoteName(key.getLetter(), key.getAccidental());
			    originalMode = mode;
			    double rand = random.GetUniform();
			    double cdf = RANDOM_MODULATION_PROBABILITY;
			    if(rand < cdf)
			    {
				    key = CHROMATIC_SCALE[(int)(random.GetUniform() * CHROMATIC_SCALE.Length)];
			    }
			    else 
			    {
				    cdf += PERFECT_FIFTH_MODULATION_PROBABILITY;
                    int originalKeyValue;
                    CHROMATIC_EQUIVALENTS.TryGetValue(originalKey, out originalKeyValue);
				    if(rand < cdf)
				    {
                        key = CHROMATIC_SCALE[(originalKeyValue + PERFECT_FIFTH_INTERVAL) % CHROMATIC_SCALE.Length];
				    }
				    else
				    {
					    cdf += PERFECT_FOURTH_MODULATION_PROBABILITY;
					    if(rand < cdf)
					    {
                            key = CHROMATIC_SCALE[(originalKeyValue + PERFECT_FOURTH_INTERVAL) % CHROMATIC_SCALE.Length];
					    }
					    else
					    {
						    cdf += RELATIVE_MODE_MODULATION_PROBABILITY;
						    if(rand < cdf)
						    {
							    if(mode == ScaleMode.MAJOR)
							    {
                                    key = CHROMATIC_SCALE[(originalKeyValue + MAJOR_SIXTH_INTERVAL) % CHROMATIC_SCALE.Length];
								    mode = ScaleMode.GENERIC_MINOR;
							    }
							    else
							    {
                                    key = CHROMATIC_SCALE[(originalKeyValue + MINOR_THIRD_INTERVAL) % CHROMATIC_SCALE.Length];
								    mode = ScaleMode.MAJOR;
							    }
						    }
						    else 
						    {
							    cdf += ABSOLUTE_MODE_MODULATION_PROBABILITY;
							    if(rand < cdf)
							    {
								    if(mode == ScaleMode.MAJOR)
									    mode = ScaleMode.GENERIC_MINOR;
								    else
									    mode = ScaleMode.MAJOR;
							    }
						    }
					    }
				    }
			    }
		    }
		    chordProgression = createChordProgression
		    (
			    key, 
			    mode,
			    phraseLength
		    );
		    key = originalKey;
		    mode = originalMode;
	    }
	
	    private NoteName key;
	    private ScaleMode mode;
	    private double jazzyness; //how much blue notes and sixth, seventh, ninth, etc. chords will be used.
	    private double beatHarmonicCovariance; //how much chord changes are likely to fall on strong beats.
	    private List<List<NoteName>> chordProgression;
	
	    private enum ChordMode { MAJOR, MINOR, AUGMENTED, DIMINISHED }
	
	    private static List<NoteName> createChord(NoteName root, ChordMode mode)
	    {
		    List<NoteName> chord = new List<NoteName>();
		    chord.Add(root);
            int noteVal;
            CHROMATIC_EQUIVALENTS.TryGetValue(root, out noteVal);
		    switch(mode)
		    {
			    case ChordMode.MAJOR:
				    chord.Add(CHROMATIC_SCALE[(noteVal + MAJOR_THIRD_INTERVAL) % CHROMATIC_SCALE.Length]);
				    chord.Add(CHROMATIC_SCALE[(noteVal + PERFECT_FIFTH_INTERVAL) % CHROMATIC_SCALE.Length]);
				    break;
			    case ChordMode.MINOR:
				    chord.Add(CHROMATIC_SCALE[(noteVal + MINOR_THIRD_INTERVAL) % CHROMATIC_SCALE.Length]);
				    chord.Add(CHROMATIC_SCALE[(noteVal + PERFECT_FIFTH_INTERVAL) % CHROMATIC_SCALE.Length]);
				    break;
			    case ChordMode.AUGMENTED:
				    chord.Add(CHROMATIC_SCALE[(noteVal + MAJOR_THIRD_INTERVAL) % CHROMATIC_SCALE.Length]);
				    chord.Add(CHROMATIC_SCALE[(noteVal + AUGMENTED_FIFTH_INTERVAL) % CHROMATIC_SCALE.Length]);
				    break;
			    case ChordMode.DIMINISHED:
				    chord.Add(CHROMATIC_SCALE[(noteVal + MINOR_THIRD_INTERVAL) % CHROMATIC_SCALE.Length]);
				    chord.Add(CHROMATIC_SCALE[(noteVal + DIMINISHED_FIFTH_INTERVAL) % CHROMATIC_SCALE.Length]);
				    break;
		    }
		    return chord;
	    }
	
	    private List<List<NoteName>> createChordProgression(NoteName key, ScaleMode mode, int phraseLength)
	    {
		    Dictionary<DiatonicNumeral, List<NoteName>> diatonicTriads = createDiatonicTriads(key, mode);
		    List<List<NoteName>> rawProgression = new List<List<NoteName>>();
		    DiatonicNumeral goalTriad;
		    switch(mode)
		    {
			    case ScaleMode.MAJOR:
				    goalTriad = DiatonicNumeral.I;
				    break;
			    case ScaleMode.NATURAL_MINOR:
			    case ScaleMode.HARMONIC_MINOR:
			    case ScaleMode.MELODIC_MINOR:
			    case ScaleMode.GENERIC_MINOR:
				    goalTriad = DiatonicNumeral.i;
				    break;
			    default:
				    goalTriad = DiatonicNumeral.I;
				    break;
		    }
		    DiatonicNumeral curr = goalTriad;
            List<NoteName> progressionIterator;
		    do
		    {
                diatonicTriads.TryGetValue(curr, out progressionIterator);
			    rawProgression.Add(progressionIterator);
			    curr = getNextDiatonicTriad(curr, mode);
		    }
		    while(curr != goalTriad);
            diatonicTriads.TryGetValue(curr, out progressionIterator);
		    rawProgression.Add(progressionIterator);
		    List<int> progressionChangeIndices = new List<int>();
		    List<int> rands = new List<int>();
		    int randsSize = phraseLength;
		    int increment = SUBBEATS_PER_MEASURE;
		    while(randsSize / increment < rawProgression.Count)
		    {
			    randsSize *= 4;
			    increment /= 2;
		    }
		    for(int i = 0; i < randsSize; i+= increment)
		    {
			    int deviation = (int)((1 - beatHarmonicCovariance) * SUBBEATS_PER_MEASURE / 2 * random.GetUniform());
			    rands.Add((random.GetUniform() < 0.5 ? i + deviation : Math.Max(i - deviation, 0)));
		    }
		    for(int i = 0; i < rawProgression.Count; i++)
		    {
			    int randomIndex = random.GetUniformInt(rands.Count);
                progressionChangeIndices.Add(rands.ElementAt<int>(randomIndex));
			    rands.Remove(randomIndex);
		    }
            progressionChangeIndices.Sort();
		
		    List<List<NoteName>> organizedProgression = new List<List<NoteName>>();
		    int index = 0;
		    for(int i = 0; i < phraseLength; i++)
		    {
			    if(rawProgression.Count == 0)
				    organizedProgression.Add(organizedProgression.ElementAt<List<NoteName>>(i - 1));
			    else
			    {
				    organizedProgression.Add(rawProgression.ElementAt<List<NoteName>>(0));
				    if(i == progressionChangeIndices[index])
				    {
					    index++;
					    rawProgression.RemoveAt(0);
				    }
			    }
		    }
		    return organizedProgression;
	    }
	
	    private static DiatonicNumeral getNextDiatonicTriad(DiatonicNumeral currentTriad, ScaleMode mode)
	    {
		    double rand = random.GetUniform();
		    switch(mode)
		    {
			    case ScaleMode.MAJOR:
				    switch(currentTriad)
				    {
                        case DiatonicNumeral.iii:
						    return DiatonicNumeral.vi;
                        case DiatonicNumeral.vi:
						    if(rand < 0.5)
							    return DiatonicNumeral.ii;
						    else
							    return DiatonicNumeral.IV;
                        case DiatonicNumeral.ii:
                        case DiatonicNumeral.IV:
                        case DiatonicNumeral.N:
						    if(rand < 0.5)
							    return DiatonicNumeral.V;
						    else
							    return DiatonicNumeral.viio;
                        case DiatonicNumeral.viio:
						    if(rand < 0.5)
							    return DiatonicNumeral.iii;
                            return DiatonicNumeral.I;
                        case DiatonicNumeral.V:
						    return DiatonicNumeral.I;
                        case DiatonicNumeral.I:
						    return getRandomMajor();
				    }
				    break;
			    case ScaleMode.NATURAL_MINOR:
                case ScaleMode.HARMONIC_MINOR:
                case ScaleMode.MELODIC_MINOR:
                case ScaleMode.GENERIC_MINOR:
				    switch(currentTriad)
				    {
                        case DiatonicNumeral.VII:
						    return DiatonicNumeral.III;
                        case DiatonicNumeral.III:
						    return DiatonicNumeral.VI;
                        case DiatonicNumeral.VI:
						    if(rand < 0.33)
							    return DiatonicNumeral.iio;
						    else if(rand < 0.66)
							    return DiatonicNumeral.iv;
						    else
							    return DiatonicNumeral.N;
                        case DiatonicNumeral.iio:
                        case DiatonicNumeral.iv:
						    if(rand < 0.33)
							    return DiatonicNumeral.VII;
                            else if (rand < 0.66)
                                return DiatonicNumeral.V;
                            else
                                return DiatonicNumeral.viio;
                        case DiatonicNumeral.N:
						    if(rand < 0.5)
							    return DiatonicNumeral.V;
						    else
							    return DiatonicNumeral.viio;
                        case DiatonicNumeral.viio:
                        case DiatonicNumeral.V:
						    return DiatonicNumeral.i;
                        case DiatonicNumeral.i:
						    return getRandomMinor();
				    }
				    break;
		    }
            return DiatonicNumeral.INVALID;
	    }
	
	    private enum DiatonicNumeral { I, i, ii, iio, III, iii, IV, iv, V, v, VI, vi, VII, viio, N, INVALID }

        private static DiatonicNumeral getRandomMajor()
		{
			double rand = random.GetUniform();
			if(rand < 1.0 / 7)
				return DiatonicNumeral.I;
			else if(rand < 2.0 / 7)
                return DiatonicNumeral.ii;
			else if(rand < 3.0 / 7)
                return DiatonicNumeral.iii;
			else if(rand < 4.0 / 7)
                return DiatonicNumeral.IV;
			else if(rand < 5.0 / 7)
                return DiatonicNumeral.V;
			else if(rand < 6.0 / 7)
                return DiatonicNumeral.vi;
			else
                return DiatonicNumeral.viio;
		}
		private static DiatonicNumeral getRandomMinor()
		{
			double rand = random.GetUniform();
			if(rand < 1.0 / 8)
                return DiatonicNumeral.i;
			else if(rand < 2.0 / 8)
                return DiatonicNumeral.iio;
			else if(rand < 3.0 / 8)
                return DiatonicNumeral.III;
			else if(rand < 4.0 / 8)
                return DiatonicNumeral.iv;
			else if(rand < 5.0 / 8)
                return DiatonicNumeral.V;
			else if(rand < 6.0 / 8)
                return DiatonicNumeral.VI;
			else if(rand < 7.0 / 8)
                return DiatonicNumeral.VII;
			else
                return DiatonicNumeral.viio;
		}
	
	    private static Dictionary<DiatonicNumeral, List<NoteName>> createDiatonicTriads(NoteName key, ScaleMode mode)
	    {
		    Dictionary<DiatonicNumeral, List<NoteName>> diatonicTriads = new Dictionary<DiatonicNumeral, List<NoteName>>();
		
		    switch(mode)
		    {
			    case ScaleMode.MAJOR:
				    List<NoteName> majorScale = createDiatonicScale(key, ScaleMode.MAJOR);
				    diatonicTriads.Add(DiatonicNumeral.I, createChord(majorScale.ElementAt<NoteName>(0), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.ii, createChord(majorScale.ElementAt<NoteName>(1), ChordMode.MINOR));
                    diatonicTriads.Add(DiatonicNumeral.iii, createChord(majorScale.ElementAt<NoteName>(2), ChordMode.MINOR));
                    diatonicTriads.Add(DiatonicNumeral.IV, createChord(majorScale.ElementAt<NoteName>(3), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.V, createChord(majorScale.ElementAt<NoteName>(4), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.vi, createChord(majorScale.ElementAt<NoteName>(5), ChordMode.MINOR));
                    diatonicTriads.Add(DiatonicNumeral.viio, createChord(majorScale.ElementAt<NoteName>(6), ChordMode.DIMINISHED));
				    break;
			    case ScaleMode.NATURAL_MINOR:
			    case ScaleMode.HARMONIC_MINOR:
			    case ScaleMode.MELODIC_MINOR:
			    case ScaleMode.GENERIC_MINOR:
				    List<NoteName> minorScale = createDiatonicScale(key, ScaleMode.GENERIC_MINOR);
                    diatonicTriads.Add(DiatonicNumeral.i, createChord(minorScale.ElementAt<NoteName>(0), ChordMode.MINOR));
                    diatonicTriads.Add(DiatonicNumeral.iio, createChord(minorScale.ElementAt<NoteName>(1), ChordMode.DIMINISHED));
                    diatonicTriads.Add(DiatonicNumeral.III, createChord(minorScale.ElementAt<NoteName>(2), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.iv, createChord(minorScale.ElementAt<NoteName>(3), ChordMode.MINOR));
                    diatonicTriads.Add(DiatonicNumeral.v, createChord(minorScale.ElementAt<NoteName>(4), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.V, createChord(minorScale.ElementAt<NoteName>(4), ChordMode.MINOR));
                    diatonicTriads.Add(DiatonicNumeral.VI, createChord(minorScale.ElementAt<NoteName>(5), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.VII, createChord(minorScale.ElementAt<NoteName>(6), ChordMode.MAJOR));
                    diatonicTriads.Add(DiatonicNumeral.viio, createChord(minorScale.ElementAt<NoteName>(7), ChordMode.DIMINISHED));
				    break;
		    }

            int rootKeyValue;
            CHROMATIC_EQUIVALENTS.TryGetValue(key, out rootKeyValue);
		    diatonicTriads.Add
		    (
			    DiatonicNumeral.N, 
			    createChord(CHROMATIC_SCALE[(rootKeyValue + HALF_STEP) % CHROMATIC_SCALE.Length], ChordMode.MAJOR)
		    );
		
		    return diatonicTriads;
	    }
	
	    //RHYTHM
	    private void initializeRhythm(double notesMean, double beatsLoyalty)
	    {
		    this.notesMean = notesMean;
		    this.beatsLoyalty = beatsLoyalty;
		    double[] beats = new double[SUBBEATS_PER_MEASURE];
		    beatsProb = new double[SUBBEATS_PER_MEASURE];
		
		    for(int i=0; i<SUBBEATS_PER_MEASURE; i++)
			    beats[i] = i;
		    for(int i=0; i<SUBBEATS_PER_MEASURE; i++)
		    {
			    beatsProb[i] = ( i == 0 ?  0 : getProbability( beats, beats[i] ) );
		    }
	    }
	
	    private double[] beatsProb;
	
	    private double getProbability( double[] beats, double beat )
	    {
		    return getProbability( beats, beat, 1.0, 1, beats.Length - 1 );
	    }
	
	    private double getProbability( double[] beats, double beat, double prob, int low, int high )
	    {
		    prob /= 2;
		    int mid = ( low + high ) / 2;
		    if( beats[mid] > beat )
			    return getProbability( beats, beat, prob, low, mid - 1 );
		    else if( beats[mid] < beat )
			    return getProbability( beats, beat, prob, mid + 1, high );
		    else
			    return 1 - prob;
	    }
	
	    private const int SUBBEATS_PER_MEASURE = 8;
	    private double notesMean;
	    private double beatsLoyalty;
	
	    //FORM
	    private void initializeForm()
	    {
	    }
	
	    private const int NUM_MEASURES_LBOUND = 2;
	    private const int NUM_MEASURES_UBOUND = 8;
	
	
	    public override double getBeatResolution()
	    {
		    return 1.0 / SUBBEATS_PER_MEASURE;
	    }
	
	    public override CellState[,] initialize()
	    {
		    int numMeasures = (int)Math.Round(NUM_MEASURES_LBOUND + Math.Abs(random.GetNormal()) * (NUM_MEASURES_UBOUND - NUM_MEASURES_LBOUND));
		    int phraseLength = SUBBEATS_PER_MEASURE * numMeasures;
		    CellState[,] songCells = new CellState[PIANO_SIZE, phraseLength];
		    for(int i = 0; i < PIANO_SIZE; i++)
		    {
			    for(int j = 0; j < phraseLength; j++)
			    {
				    switch(random.GetUniformInt(3))
				    {
					    case 0:
						    songCells[i, j] = CellState.START;
						    break;
					    case 1:
						    songCells[i, j] = CellState.HOLD;
						    break;
					    case 2:
						    songCells[i, j] = CellState.SILENT;
						    break;
				    }
			    }
		    }
		
		    return songCells;
	    }
	
	    public override void Evolve(CellState[,] songCells)
	    {
		    initializeMelody();
		    initializeHarmony(songCells.GetLength(1));
		    initializeRhythm(0.375, Math.Min(1, random.GetUniform() + 0.5f)); //@param ( notesmean, beatsloyalty )
		    initializeForm();
		    CellState[,] previousState = new CellState[songCells.GetLength(0), songCells.GetLength(1)];
		
		
		    for(int x = 0; x < NUM_GENERATIONS; x++)
		    {
			    for(int i = 0; i < songCells.GetLength(0); i++)
				    for(int j = 0; j < songCells.GetLength(1); j++)
					    previousState[i, j] = songCells[i, j];
					
			    //GAME OF LIFE
                int midsectionStart = 10;
                int midsectionEnd = 10;
                int[] crowd = { 3, 0, 3 };
                for (int section = 0; section < 3; section++)
                {
                    for (int i = midsectionStart*(section%2) + (songCells.GetLength(0)-midsectionEnd)*(section/2);
                        (i < midsectionStart && section == 0) || (i < songCells.GetLength(0) - midsectionEnd && section == 1) || i < songCells.GetLength(0);
                        i++)
                    {
                        for (int j = 0; j < songCells.GetLength(1); j++)
                        {
                            int crowdedness = crowd[section];
                            for (int k = Math.Max(0, i - 2); k < Math.Min(songCells.GetLength(0), i + 2); k++)
                            {
                                switch (previousState[k, j])
                                {
                                    case CellState.START:
                                        crowdedness += 2;
                                        break;
                                    case CellState.HOLD:
                                        crowdedness++;
                                        break;
                                }
                            }
                            if (crowdedness > CROWDEDNESS_TOLERANCE)
                            {
                                songCells[i, j] = CellState.SILENT;
                            }
                            else
                            {
                                songCells[i, j] = CellState.START;
                            }
                        }
                    }
                }
                

			    //MELODY AND RANGE
			    for(int i = 0; i < songCells.GetLength(0); i++)
			    {
				    for(int j = 0; j < songCells.GetLength(1); j++)
				    {
					    if(j > songCells.GetLength(1) - SUBBEATS_PER_MEASURE)
						    songCells[i, j] = CellState.HOLD;
				    }
			    }
			
			    //HARMONY
			    for(int i = 0; i < songCells.GetLength(0); i++)
			    {
				    for(int j = 0; j < songCells.GetLength(1); j++)
				    {
					    if(!(chordProgression.ElementAt<List<NoteName>>(j)).Contains(getNoteName(i)))
					    {
						    if(random.GetUniform() * (i / PIANO_SIZE) < tonality)
						    {
							    songCells[i, j] = CellState.SILENT;
						    }
					    }
				    }
			    }
			
			    //RHYTHM
			    for(int i = 0; i < songCells.GetLength(0); i++)
			    {
				    for(int j = 0; j < songCells.GetLength(1); j++)
				    {
					    if(random.GetUniform() < beatsProb[j%SUBBEATS_PER_MEASURE] * beatsLoyalty + ( 1 - notesMean ) * ( 1 - beatsLoyalty ))
						    songCells[i, j] = CellState.SILENT;
				    }
			    }
			    for(int i = 0; i < songCells.GetLength(0); i++)//pitch
			    {
				    for(int j = 0; j < songCells.GetLength(1) - 1; j++)//time
				    {
					    if(songCells[i, j] == CellState.START)
					    {
						    int distCounter = 0;
						    while(j < songCells.GetLength(1) - 1 && songCells[i, j+1] != CellState.START && songCells[i, j] != CellState.HOLD)
						    {
							    distCounter++;
							    j++;
							    if(random.GetUniform() < 1.0 / distCounter)
								    songCells[i, j] = CellState.HOLD;
							    else 
								    break;
						    }
					    }
				    }
			    }
			    //END RHYTHM	
		    }
	    }
	
	    public override NoteName getNoteName(int pitchNumber)
	    {
		    return CHROMATIC_SCALE[(pitchNumber + NOTENAME_OFFSET) % CHROMATIC_SCALE.Length];
	    }
	
	    public override int getOctave(int pitchNumber)
	    {
		    return (pitchNumber + OCTAVE_OFFSET) / CHROMATIC_SCALE.Length + 1;
	    }
	
    }
}
