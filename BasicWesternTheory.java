import java.util.*;

public class BasicWesternTheory extends Theory
{
	public BasicWesternTheory()
	{
		super();
	}
	
	//MELODY
	public static final NoteName NOTE_C              = new NoteName('c', ' ');
	public static final NoteName NOTE_C_SHARP        = new NoteName('c', '#');
	public static final NoteName NOTE_C_FLAT         = new NoteName('b', ' ');
	public static final NoteName NOTE_C_DOUBLE_SHARP = new NoteName('d', ' ');
	public static final NoteName NOTE_C_DOUBLE_FLAT  = new NoteName('b', 'b');
	
	public static final NoteName NOTE_D              = new NoteName('d', ' ');
	public static final NoteName NOTE_D_SHARP        = new NoteName('d', '#');
	public static final NoteName NOTE_D_FLAT         = new NoteName('d', 'b');
	public static final NoteName NOTE_D_DOUBLE_SHARP = new NoteName('e', ' ');
	public static final NoteName NOTE_D_DOUBLE_FLAT  = new NoteName('c', ' ');
	
	public static final NoteName NOTE_E              = new NoteName('e', ' ');
	public static final NoteName NOTE_E_SHARP        = new NoteName('f', ' ');
	public static final NoteName NOTE_E_FLAT         = new NoteName('e', 'b');
	public static final NoteName NOTE_E_DOUBLE_SHARP = new NoteName('f', '#');
	public static final NoteName NOTE_E_DOUBLE_FLAT  = new NoteName('d', ' ');
	
	public static final NoteName NOTE_F              = new NoteName('f', ' ');
	public static final NoteName NOTE_F_SHARP        = new NoteName('f', '#');
	public static final NoteName NOTE_F_FLAT         = new NoteName('e', ' ');
	public static final NoteName NOTE_F_DOUBLE_SHARP = new NoteName('g', ' ');
	public static final NoteName NOTE_F_DOUBLE_FLAT  = new NoteName('e', 'b');
	
	public static final NoteName NOTE_G              = new NoteName('g', ' ');
	public static final NoteName NOTE_G_SHARP        = new NoteName('g', '#');
	public static final NoteName NOTE_G_FLAT         = new NoteName('g', 'b');
	public static final NoteName NOTE_G_DOUBLE_SHARP = new NoteName('a', ' ');
	public static final NoteName NOTE_G_DOUBLE_FLAT  = new NoteName('f', ' ');
	
	public static final NoteName NOTE_A              = new NoteName('a', ' ');
	public static final NoteName NOTE_A_SHARP        = new NoteName('a', '#');
	public static final NoteName NOTE_A_FLAT         = new NoteName('a', 'b');
	public static final NoteName NOTE_A_DOUBLE_SHARP = new NoteName('b', ' ');
	public static final NoteName NOTE_A_DOUBLE_FLAT  = new NoteName('g', ' ');
	
	public static final NoteName NOTE_B              = new NoteName('b', ' ');
	public static final NoteName NOTE_B_SHARP        = new NoteName('c', ' ');
	public static final NoteName NOTE_B_FLAT         = new NoteName('b', 'b');
	public static final NoteName NOTE_B_DOUBLE_SHARP = new NoteName('c', '#');
	public static final NoteName NOTE_B_DOUBLE_FLAT  = new NoteName('a', ' ');
	
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
	
	private static final int PIANO_SIZE = 88;
	
	//HARMONY
	
	//RHYTHM
	private static final double SUBBEATS_PER_MEASURE = 16.0;
	
	//FORM
	private static final int NUM_MEASURES_LBOUND = 2;
	private static final int NUM_MEASURES_UBOUND = 14;
	
	
	public double getBeatResolution()
	{
		return 1 / SUBBEATS_PER_MEASURE;
	}
	
	public CellState[][] initialize()
	{
		int numMeasures = (int)Math.round(NUM_MEASURES_LBOUND + random.nextDouble() * (NUM_MEASURES_UBOUND - NUM_MEASURES_LBOUND));
		System.out.println(numMeasures);
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
		
	}
	
	public NoteName getNoteName(int pitchNumber)
	{
		return CHROMATIC_SCALE[(pitchNumber % 12 + 7) % 12];
	}
	
	public int getOctave(int pitchNumber)
	{
		return (pitchNumber - 3) / 12 + 1;
	}
}