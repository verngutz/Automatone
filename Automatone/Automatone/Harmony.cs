using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Harmony
    {
        private class Random2
        {
            public double NextDouble()
            {
                return 0.5;
            }
            public int Next(int i)
            {
                return i / 2;
            }
        }
        private Random2 random;
        private MusicTheory theory;


	    public Harmony (MusicTheory theory, Random random) 
        {
            this.random = new Random2();//random;
            this.theory = theory;
        }

	    private static Dictionary<NoteName, int> CHROMATIC_EQUIVALENTS = createChromaticEquivalents();
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
	
	    public enum ScaleMode { MAJOR, NATURAL_MINOR, HARMONIC_MINOR, MELODIC_MINOR, GENERIC_MINOR }
	
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
                        curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
                    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 3; i++)
				    {
                        curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    break;
			    case ScaleMode.NATURAL_MINOR:
                    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
                    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 2; i++)
				    {
                        curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
                    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    break;
			    case ScaleMode.HARMONIC_MINOR:
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 2; i++)
				    {
					    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    break;
			    case ScaleMode.MELODIC_MINOR:
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 4; i++)
				    {
					    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    break;
			    case ScaleMode.GENERIC_MINOR:
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    for(int i = 0; i < 2; i++)
				    {
					    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                        CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
					    scale.Add(curr);
				    }
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + WHOLE_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
                    CHROMATIC_EQUIVALENTS.TryGetValue(curr, out noteVal);
				    scale.Add(curr);
				    curr = ClassicalTheory.CHROMATIC_SCALE[(noteVal + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length];
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
	
	    public void initializeHarmony(int phraseLength)
	    {
		    jazzyness = 0;
		    beatHarmonicCovariance = MEAN_BEATHARMONIC_COVARIANCE;
		    double offset = random.NextDouble() / BEAT_HARMONIC_COVARIANCE_OFFSET_DIVISOR;

            if (random.NextDouble() < 0.5)
			    beatHarmonicCovariance += offset;
		    else
			    beatHarmonicCovariance -= offset;
			
		    NoteName originalKey;
		    ScaleMode originalMode;
		    if(key == null)
		    {
                key = ClassicalTheory.CHROMATIC_SCALE[(int)(random.NextDouble() * ClassicalTheory.CHROMATIC_SCALE.Length)];
                mode = (random.NextDouble() < 0.5 ? ScaleMode.MAJOR : ScaleMode.GENERIC_MINOR);
			    originalKey = key;
			    originalMode = mode;
		    }
		    else
		    {
                originalKey = key;
			    originalMode = mode;
			    double rand = random.NextDouble();
			    double cdf = RANDOM_MODULATION_PROBABILITY;
			    if(rand < cdf)
			    {
				    key = ClassicalTheory.CHROMATIC_SCALE[(int)(random.NextDouble() * ClassicalTheory.CHROMATIC_SCALE.Length)];
			    }
			    else 
			    {
				    cdf += PERFECT_FIFTH_MODULATION_PROBABILITY;
                    int originalKeyValue;
                    CHROMATIC_EQUIVALENTS.TryGetValue(originalKey, out originalKeyValue);
				    if(rand < cdf)
				    {
                        key = ClassicalTheory.CHROMATIC_SCALE[(originalKeyValue + PERFECT_FIFTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length];
				    }
				    else
				    {
					    cdf += PERFECT_FOURTH_MODULATION_PROBABILITY;
					    if(rand < cdf)
					    {
                            key = ClassicalTheory.CHROMATIC_SCALE[(originalKeyValue + PERFECT_FOURTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length];
					    }
					    else
					    {
						    cdf += RELATIVE_MODE_MODULATION_PROBABILITY;
						    if(rand < cdf)
						    {
							    if(mode == ScaleMode.MAJOR)
							    {
                                    key = ClassicalTheory.CHROMATIC_SCALE[(originalKeyValue + MAJOR_SIXTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length];
								    mode = ScaleMode.GENERIC_MINOR;
							    }
							    else
							    {
                                    key = ClassicalTheory.CHROMATIC_SCALE[(originalKeyValue + MINOR_THIRD_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length];
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
	    public List<List<NoteName>> chordProgression;
	
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
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + MAJOR_THIRD_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + PERFECT_FIFTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    break;
			    case ChordMode.MINOR:
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + MINOR_THIRD_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + PERFECT_FIFTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    break;
			    case ChordMode.AUGMENTED:
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + MAJOR_THIRD_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + AUGMENTED_FIFTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    break;
			    case ChordMode.DIMINISHED:
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + MINOR_THIRD_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    chord.Add(ClassicalTheory.CHROMATIC_SCALE[(noteVal + DIMINISHED_FIFTH_INTERVAL) % ClassicalTheory.CHROMATIC_SCALE.Length]);
				    break;
		    }
		    return chord;
	    }
	
	    public List<List<NoteName>> createChordProgression(NoteName key, ScaleMode mode, int phraseLength)
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
            int increment = Automatone.SUBBEATS_PER_MEASURE;
		    while(randsSize / increment < rawProgression.Count)
		    {
			    randsSize *= 4;
			    increment /= 2;
		    }
		    for(int i = 0; i < randsSize; i+= increment)
		    {
                int deviation = (int)((1 - beatHarmonicCovariance) * Automatone.SUBBEATS_PER_MEASURE / 2 * random.NextDouble());
			    rands.Add((random.NextDouble() < 0.5 ? i + deviation : Math.Max(i - deviation, 0)));
		    }
		    for(int i = 0; i < rawProgression.Count; i++)
		    {
			    int randomIndex = random.Next(rands.Count);
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
	
	    private DiatonicNumeral getNextDiatonicTriad(DiatonicNumeral currentTriad, ScaleMode mode)
	    {
		    double rand = random.NextDouble();
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

        private DiatonicNumeral getRandomMajor()
		{
			double rand = random.NextDouble();
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
		private DiatonicNumeral getRandomMinor()
		{
			double rand = random.NextDouble();
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
			    createChord(ClassicalTheory.CHROMATIC_SCALE[(rootKeyValue + HALF_STEP) % ClassicalTheory.CHROMATIC_SCALE.Length], ChordMode.MAJOR)
		    );
		
		    return diatonicTriads;
	    }
    }
}
