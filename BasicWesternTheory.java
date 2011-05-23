import java.util.*;

public class BasicWesternTheory extends Theory
{
	public BasicWesternTheory()
	{
		super();
	}
	
	private static final int NUM_GENERATIONS = 100;
	
	//KEYBOARD CONSTANTS
	public static final NoteName NOTE_C              = new NoteName('c', ' ');
	public static final NoteName NOTE_C_SHARP        = new NoteName('c', '#');
	public static final NoteName NOTE_C_FLAT         = new NoteName('b', ' ');
	public static final NoteName NOTE_C_DOUBLE_SHARP = new NoteName('d', ' ');
	public static final NoteName NOTE_C_DOUBLE_FLAT  = new NoteName('a', '#');
	
	public static final NoteName NOTE_D              = new NoteName('d', ' ');
	public static final NoteName NOTE_D_SHARP        = new NoteName('d', '#');
	public static final NoteName NOTE_D_FLAT         = new NoteName('c', '#');
	public static final NoteName NOTE_D_DOUBLE_SHARP = new NoteName('e', ' ');
	public static final NoteName NOTE_D_DOUBLE_FLAT  = new NoteName('c', ' ');
	
	public static final NoteName NOTE_E              = new NoteName('e', ' ');
	public static final NoteName NOTE_E_SHARP        = new NoteName('f', ' ');
	public static final NoteName NOTE_E_FLAT         = new NoteName('d', '#');
	public static final NoteName NOTE_E_DOUBLE_SHARP = new NoteName('f', '#');
	public static final NoteName NOTE_E_DOUBLE_FLAT  = new NoteName('d', ' ');
	
	public static final NoteName NOTE_F              = new NoteName('f', ' ');
	public static final NoteName NOTE_F_SHARP        = new NoteName('f', '#');
	public static final NoteName NOTE_F_FLAT         = new NoteName('e', ' ');
	public static final NoteName NOTE_F_DOUBLE_SHARP = new NoteName('g', ' ');
	public static final NoteName NOTE_F_DOUBLE_FLAT  = new NoteName('d', '#');
	
	public static final NoteName NOTE_G              = new NoteName('g', ' ');
	public static final NoteName NOTE_G_SHARP        = new NoteName('g', '#');
	public static final NoteName NOTE_G_FLAT         = new NoteName('f', '#');
	public static final NoteName NOTE_G_DOUBLE_SHARP = new NoteName('a', ' ');
	public static final NoteName NOTE_G_DOUBLE_FLAT  = new NoteName('f', ' ');
	
	public static final NoteName NOTE_A              = new NoteName('a', ' ');
	public static final NoteName NOTE_A_SHARP        = new NoteName('a', '#');
	public static final NoteName NOTE_A_FLAT         = new NoteName('g', '#');
	public static final NoteName NOTE_A_DOUBLE_SHARP = new NoteName('b', ' ');
	public static final NoteName NOTE_A_DOUBLE_FLAT  = new NoteName('g', ' ');
	
	public static final NoteName NOTE_B              = new NoteName('b', ' ');
	public static final NoteName NOTE_B_SHARP        = new NoteName('c', ' ');
	public static final NoteName NOTE_B_FLAT         = new NoteName('a', '#');
	public static final NoteName NOTE_B_DOUBLE_SHARP = new NoteName('c', '#');
	public static final NoteName NOTE_B_DOUBLE_FLAT  = new NoteName('a', ' ');

	private static final int PIANO_SIZE = 88;
	private static final int NOTENAME_OFFSET = 7;
	private static final int OCTAVE_OFFSET = 9;
	
	//CHROMATIC SCALE
	private static final NoteName[] CHROMATIC_SCALE = 
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
	
	private static final Hashtable<NoteName, Integer> CHROMATIC_EQUIVALENTS = createChromaticEquivalents();
	private static Hashtable<NoteName, Integer> createChromaticEquivalents()
	{
		Hashtable<NoteName, Integer> equivalents = new Hashtable<NoteName, Integer>();
		
		equivalents.put(NOTE_C, 0);
		equivalents.put(NOTE_C_SHARP, 1);
		equivalents.put(NOTE_C_FLAT, 11);
		equivalents.put(NOTE_C_DOUBLE_SHARP, 2);
		equivalents.put(NOTE_C_DOUBLE_FLAT, 10);
		
		equivalents.put(NOTE_D, 2);
		equivalents.put(NOTE_D_SHARP, 3);
		equivalents.put(NOTE_D_FLAT, 1);
		equivalents.put(NOTE_D_DOUBLE_SHARP, 4);
		equivalents.put(NOTE_D_DOUBLE_FLAT, 0);
		
		equivalents.put(NOTE_E, 4);
		equivalents.put(NOTE_E_SHARP, 5);
		equivalents.put(NOTE_E_FLAT, 3);
		equivalents.put(NOTE_E_DOUBLE_SHARP, 6);
		equivalents.put(NOTE_E_DOUBLE_FLAT, 2);
		
		equivalents.put(NOTE_F, 5);
		equivalents.put(NOTE_F_SHARP, 6);
		equivalents.put(NOTE_F_FLAT, 4);
		equivalents.put(NOTE_F_DOUBLE_SHARP, 7);
		equivalents.put(NOTE_F_DOUBLE_FLAT, 3);
		
		equivalents.put(NOTE_G, 7);
		equivalents.put(NOTE_G_SHARP, 8);
		equivalents.put(NOTE_G_FLAT, 6);
		equivalents.put(NOTE_G_DOUBLE_SHARP, 9);
		equivalents.put(NOTE_G_DOUBLE_FLAT, 5);
		
		equivalents.put(NOTE_A, 9);
		equivalents.put(NOTE_A_SHARP, 10);
		equivalents.put(NOTE_A_FLAT, 8);
		equivalents.put(NOTE_A_DOUBLE_SHARP, 11);
		equivalents.put(NOTE_A_DOUBLE_FLAT, 7);
		
		equivalents.put(NOTE_B, 11);
		equivalents.put(NOTE_B_SHARP, 0);
		equivalents.put(NOTE_B_FLAT, 10);
		equivalents.put(NOTE_B_DOUBLE_SHARP, 1);
		equivalents.put(NOTE_B_DOUBLE_FLAT, 9);
		
		return equivalents;
	}
	
	//INTERVALS
	public static final int DIMINISHED_UNISON_INTERVAL = -1;
	public static final int PERFECT_UNISON_INTERVAL = 0;
	public static final int AUGMENTED_UNISON_INTERVAL = 1;
	public static final int DIMINISHED_SECOND_INTERVAL = 0;
	public static final int MINOR_SECOND_INTERVAL = 1;
	public static final int MAJOR_SECOND_INTERVAL = 2;
	public static final int AUGMENTED_SECOND_INTERVAL = 3;
	public static final int DIMINISHED_THIRD_INTERVAL = 2;
	public static final int MINOR_THIRD_INTERVAL = 3;
	public static final int MAJOR_THIRD_INTERVAL = 4;
	public static final int AUGMENTED_THIRD_INTERVAL = 5;
	public static final int DIMINISHED_FOURTH_INTERVAL = 4;
	public static final int PERFECT_FOURTH_INTERVAL = 5;
	public static final int AUGMENTED_FOURTH_INTERVAL = 6;
	public static final int DIMINISHED_FIFTH_INTERVAL = 6;
	public static final int PERFECT_FIFTH_INTERVAL = 7;
	public static final int AUGMENTED_FIFTH_INTERVAL = 8;
	public static final int DIMINIHED_SIXTH_INTERVAL = 7;
	public static final int MINOR_SIXTH_INTERVAL = 8;
	public static final int MAJOR_SIXTH_INTERVAL = 9;
	public static final int AUGMENTED_SIXTH_INTERVAL = 10;
	public static final int DIMINISHED_SEVENTH_INTERVAL = 9;
	public static final int MINOR_SEVENTH_INTERVAL = 10;
	public static final int MAJOR_SEVENTH_INTERVAL = 11;
	public static final int AUGMENTED_SEVENTH_INTERVAL = 12;
	public static final int DIMINISHED_OCTAVE_INTERVAL = 11;
	public static final int PERFECT_OCTAVE_INTERVAL = 12;
	public static final int AUGMENTED_OCTAVE_INTERVAL = 13;
	
	public static final int WHOLE_STEP = 2;
	public static final int HALF_STEP = 1;
	
	//MELODY
	private void initializeMelody()
	{
		tonality = 0.8;
	}
	
	private double tonality; //how much the melody notes conform to the diatonic scale of a given key versus the chromatic scale
	
	private enum ScaleMode { MAJOR, NATURAL_MINOR, HARMONIC_MINOR, MELODIC_MINOR, GENERIC_MINOR }
	
	private static ArrayList<NoteName> createDiatonicScale(NoteName key, ScaleMode mode)
	{
		ArrayList<NoteName> scale = new ArrayList<NoteName>();
		scale.add(key);
		NoteName curr = key;
		switch(mode)
		{
			case MAJOR:
				for(int i = 0; i < 2; i++)
				{
					curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
					scale.add(curr);
				}
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				for(int i = 0; i < 3; i++)
				{
					curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
					scale.add(curr);
				}
				break;
			case NATURAL_MINOR:
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				for(int i = 0; i < 2; i++)
				{
					curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
					scale.add(curr);
				}
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				break;
			case HARMONIC_MINOR:
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				for(int i = 0; i < 2; i++)
				{
					curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
					scale.add(curr);
				}
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				break;
			case MELODIC_MINOR:
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				for(int i = 0; i < 4; i++)
				{
					curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
					scale.add(curr);
				}
				break;
			case GENERIC_MINOR:
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				for(int i = 0; i < 2; i++)
				{
					curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
					scale.add(curr);
				}
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + WHOLE_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				curr = CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(curr) + HALF_STEP) % CHROMATIC_SCALE.length];
				scale.add(curr);
				break;
		}
		return scale;
	}
	
	//HARMONY
	private void initializeHarmony(int phraseLength)
	{
		jazzyness = 0;
		beatHarmonicCovariance = 0.9;
		chordProgression = createChordProgression
		(
			CHROMATIC_SCALE[(int)(random.nextDouble() * CHROMATIC_SCALE.length)], 
			(random.nextDouble() < 0.5 ? ScaleMode.MAJOR : ScaleMode.GENERIC_MINOR),
			phraseLength
		);
	}
	
	private double jazzyness; //how much blue notes and sixth, seventh, ninth, etc. chords will be used.
	private double beatHarmonicCovariance; //how much chord changes are likely to fall on strong beats.
	private ArrayList<ArrayList<NoteName>> chordProgression;
	
	private enum ChordMode { MAJOR, MINOR, AUGMENTED, DIMINISHED }
	
	private static ArrayList<NoteName> createChord(NoteName base, ChordMode mode)
	{
		ArrayList<NoteName> chord = new ArrayList<NoteName>();
		chord.add(base);
		switch(mode)
		{
			case MAJOR:
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + MAJOR_THIRD_INTERVAL) % CHROMATIC_SCALE.length]);
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + PERFECT_FIFTH_INTERVAL) % CHROMATIC_SCALE.length]);
				break;
			case MINOR:
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + MINOR_THIRD_INTERVAL) % CHROMATIC_SCALE.length]);
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + PERFECT_FIFTH_INTERVAL) % CHROMATIC_SCALE.length]);
				break;
			case AUGMENTED:
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + MAJOR_THIRD_INTERVAL) % CHROMATIC_SCALE.length]);
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + AUGMENTED_FIFTH_INTERVAL) % CHROMATIC_SCALE.length]);
				break;
			case DIMINISHED:
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + MINOR_THIRD_INTERVAL) % CHROMATIC_SCALE.length]);
				chord.add(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(base) + DIMINISHED_FIFTH_INTERVAL) % CHROMATIC_SCALE.length]);
				break;
		}
		return chord;
	}
	
	private static ArrayList<ArrayList<NoteName>> createChordProgression(NoteName key, ScaleMode mode, int phraseLength)
	{
		Hashtable<DiatonicNumeral, ArrayList<NoteName>> diatonicTriads = createDiatonicTriads(key, mode);
		ArrayList<ArrayList<NoteName>> rawProgression = new ArrayList<ArrayList<NoteName>>();
		DiatonicNumeral goalTriad;
		DiatonicNumeral curr;
		switch(mode)
		{
			case MAJOR:
				goalTriad = DiatonicNumeral.I;
				curr = DiatonicNumeral.getRandomMajor();
				break;
			case NATURAL_MINOR:
			case HARMONIC_MINOR:
			case MELODIC_MINOR:
			case GENERIC_MINOR:
				goalTriad = DiatonicNumeral.i;
				curr = DiatonicNumeral.getRandomMinor();
				break;
			default:
				goalTriad = DiatonicNumeral.I;
				curr = DiatonicNumeral.getRandomMajor();
				break;
		}
		do
		{
			rawProgression.add(diatonicTriads.get(curr));
			curr = getNextDiatonicTriad(curr, mode);
		}
		while(curr != goalTriad);
		rawProgression.add(diatonicTriads.get(curr));
		
		ArrayList<ArrayList<NoteName>> organizedProgression = new ArrayList<ArrayList<NoteName>>();
		int[] absoluteNoteDensity = new int[phraseLength];
		int[] relativeNoteDensity = new int[phraseLength];
		int maxAbsoluteNoteDensity = 0;
		for(int i = 0; i < phraseLength; i++)
		{
			if(rawProgression.size() == 0)
				organizedProgression.add(organizedProgression.get(i - 1));
			else
				organizedProgression.add(rawProgression.get(0));
			double rand = random.nextDouble();
			if(rand < 1.0 / phraseLength && rawProgression.size() != 0)
				rawProgression.remove(0);
		}
		return organizedProgression;
	}
	
	private static DiatonicNumeral getNextDiatonicTriad(DiatonicNumeral currentTriad, ScaleMode mode)
	{
		double rand = random.nextDouble();
		switch(mode)
		{
			case MAJOR:
				switch(currentTriad)
				{
					case iii:
						return DiatonicNumeral.vi;
					case vi:
						if(rand < 0.33)
							return DiatonicNumeral.ii;
						else if(rand < 0.66)
							return DiatonicNumeral.IV;
						else
							return DiatonicNumeral.N;
					case ii:
					case IV:
					case N:
						if(rand < 0.5)
							return DiatonicNumeral.V;
						else
							return DiatonicNumeral.viio;
					case viio:
						if(rand < 0.5)
							return DiatonicNumeral.iii;
					case V:
						return DiatonicNumeral.I;
					case I:
						return DiatonicNumeral.getRandomMajor();
				}
				break;
			case NATURAL_MINOR:
			case HARMONIC_MINOR:
			case MELODIC_MINOR:
			case GENERIC_MINOR:
				switch(currentTriad)
				{
					case VII:
						return DiatonicNumeral.III;
					case III:
						return DiatonicNumeral.VI;
					case VI:
						if(rand < 0.33)
							return DiatonicNumeral.iio;
						else if(rand < 0.66)
							return DiatonicNumeral.iv;
						else
							return DiatonicNumeral.N;
					case iio:
					case iv:
						if(rand < 0.5)
							return DiatonicNumeral.VII;
					case N:
						if(rand < 0.5)
							return DiatonicNumeral.V;
						else
							return DiatonicNumeral.viio;
					case viio:
						return DiatonicNumeral.III;
					case i:
						return DiatonicNumeral.getRandomMinor();
				}
				break;
		}
		return null;
	}
	
	private enum DiatonicNumeral 
	{ 
		I, i, ii, iio, III, iii, IV, iv, V, v, VI, vi, VII, viio, N;
		public static DiatonicNumeral getRandomMajor()
		{
			double rand = random.nextDouble();
			if(rand < 1.0 / 7)
				return I;
			else if(rand < 2.0 / 7)
				return ii;
			else if(rand < 3.0 / 7)
				return iii;
			else if(rand < 4.0 / 7)
				return IV;
			else if(rand < 5.0 / 7)
				return V;
			else if(rand < 6.0 / 7)
				return vi;
			else
				return viio;
		}
		public static DiatonicNumeral getRandomMinor()
		{
			double rand = random.nextDouble();
			if(rand < 1.0 / 8)
				return i;
			else if(rand < 2.0 / 8)
				return iio;
			else if(rand < 3.0 / 8)
				return III;
			else if(rand < 4.0 / 8)
				return iv;
			else if(rand < 5.0 / 8)
				return V;
			else if(rand < 6.0 / 8)
				return VI;
			else if(rand < 7.0 / 8)
				return VII;
			else
				return viio;
		}
	}
	
	private static Hashtable<DiatonicNumeral, ArrayList<NoteName>> createDiatonicTriads(NoteName key, ScaleMode mode)
	{
		Hashtable<DiatonicNumeral, ArrayList<NoteName>> diatonicTriads = new Hashtable<DiatonicNumeral, ArrayList<NoteName>>();
		
		switch(mode)
		{
			case MAJOR:
				ArrayList<NoteName> majorScale = createDiatonicScale(key, ScaleMode.MAJOR);
				diatonicTriads.put(DiatonicNumeral.I, createChord(majorScale.get(0), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.ii, createChord(majorScale.get(1), ChordMode.MINOR));
				diatonicTriads.put(DiatonicNumeral.iii, createChord(majorScale.get(2), ChordMode.MINOR));
				diatonicTriads.put(DiatonicNumeral.IV, createChord(majorScale.get(3), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.V, createChord(majorScale.get(4), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.vi, createChord(majorScale.get(5), ChordMode.MINOR));
				diatonicTriads.put(DiatonicNumeral.viio, createChord(majorScale.get(6), ChordMode.DIMINISHED));
				break;
			case NATURAL_MINOR:
			case HARMONIC_MINOR:
			case MELODIC_MINOR:
			case GENERIC_MINOR:
				ArrayList<NoteName> minorScale = createDiatonicScale(key, ScaleMode.GENERIC_MINOR);
				diatonicTriads.put(DiatonicNumeral.i, createChord(minorScale.get(0), ChordMode.MINOR));
				diatonicTriads.put(DiatonicNumeral.iio, createChord(minorScale.get(1), ChordMode.DIMINISHED));
				diatonicTriads.put(DiatonicNumeral.III, createChord(minorScale.get(2), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.iv, createChord(minorScale.get(3), ChordMode.MINOR));
				diatonicTriads.put(DiatonicNumeral.v, createChord(minorScale.get(4), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.V, createChord(minorScale.get(4), ChordMode.MINOR));
				diatonicTriads.put(DiatonicNumeral.VI, createChord(minorScale.get(5), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.VII, createChord(minorScale.get(6), ChordMode.MAJOR));
				diatonicTriads.put(DiatonicNumeral.viio, createChord(minorScale.get(7), ChordMode.DIMINISHED));
				break;
		}
		
		diatonicTriads.put
		(
			DiatonicNumeral.N, 
			createChord(CHROMATIC_SCALE[(CHROMATIC_EQUIVALENTS.get(key) + HALF_STEP) % CHROMATIC_SCALE.length], ChordMode.MAJOR)
		);
		
		return diatonicTriads;
	}
	
	//RHYTHM
	private void initializeRhythm()
	{
	}
	
	private static final double SUBBEATS_PER_MEASURE = 16.0;
	
	//FORM
	private void initializeForm()
	{
	}
	
	private static final int NUM_MEASURES_LBOUND = 2;
	private static final int NUM_MEASURES_UBOUND = 14;
	
	
	public double getBeatResolution()
	{
		return 1 / SUBBEATS_PER_MEASURE;
	}
	
	public CellState[][] initialize()
	{
		int numMeasures = (int)Math.round(NUM_MEASURES_LBOUND + random.nextDouble() * (NUM_MEASURES_UBOUND - NUM_MEASURES_LBOUND));
		int phraseLength = (int)SUBBEATS_PER_MEASURE * numMeasures;
		CellState[][] songCells = new CellState[PIANO_SIZE][phraseLength];
		for(int i = 0; i < PIANO_SIZE; i++)
		{
			for(int j = 0; j < phraseLength; j++)
			{
				switch(random.nextInt(3))
				{
					case 0:
						songCells[i][j] = CellState.START;
						break;
					case 1:
						songCells[i][j] = CellState.HOLD;
						break;
					case 2:
						songCells[i][j] = CellState.SILENT;
						break;
				}
			}
		}
		
		return songCells;
	}
	
	public void evolve(CellState[][] songCells)
	{
		initializeMelody();
		initializeHarmony(songCells[0].length);
		initializeRhythm();
		initializeForm();
		for(int x = 0; x < NUM_GENERATIONS; x++)
		{
			for(int i = 0; i < songCells.length; i++)
			{
				for(int j = 0; j < songCells[0].length; j++)
				{
					if(!(chordProgression.get(j)).contains(getNoteName(i)))
					{
						if(random.nextDouble() > 0.7)
						{
							songCells[i][j] = CellState.SILENT;
						}
					}
				}
			}
		}
	}
	
	public NoteName getNoteName(int pitchNumber)
	{
		return CHROMATIC_SCALE[(pitchNumber + NOTENAME_OFFSET) % CHROMATIC_SCALE.length];
	}
	
	public int getOctave(int pitchNumber)
	{
		return (pitchNumber + OCTAVE_OFFSET) / CHROMATIC_SCALE.length;
	}
	
}