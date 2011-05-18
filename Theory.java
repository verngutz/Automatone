public abstract class Theory
{
	public abstract double getBeatResolution();
	public abstract void populate(CellState[][] songCells);
	public abstract void evolve(CellState[][] songCells);
	public abstract NoteName getNoteName(int pitchNumber);
	public abstract int getOctave(int pitchNumber);
}