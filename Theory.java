import java.util.*;

public abstract class Theory
{
	protected static Random random;
	private static final long SEED = 1;
	
	public abstract double getBeatResolution();
	public abstract CellState[][] initialize();
	public abstract void evolve(CellState[][] songCells);
	public abstract NoteName getNoteName(int pitchNumber);
	public abstract int getOctave(int pitchNumber);
	
	public Theory()
	{
		random = new Random(SEED);
	}
}