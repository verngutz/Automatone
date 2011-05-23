import java.util.*;

public abstract class Theory
{
	protected Random random;
	public abstract double getBeatResolution();
	public abstract CellState[][] initialize();
	public abstract void evolve(CellState[][] songCells);
	public abstract NoteName getNoteName(int pitchNumber);
	public abstract int getOctave(int pitchNumber);
	
	public Theory(Random random)
	{
		this.random = random;
	}
}