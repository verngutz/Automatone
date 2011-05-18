import java.util.*;

public class Chord
{
	public enum ChordMode
	{
		MAJOR,
		MINOR,
		AUGMENTED,
		DIMINISHED
	}
	
	private ChordMode mode;
	private ArrayList<NoteName> chordComponents;
	
	//inversion - 0 means root position, 1 means first inversion, 2 means second inversion, and so on.
	public Chord(NoteName base, ChordMode mode, int inversion)
	{
		
	}
	
	public Chord(ArrayList<NoteName> chordComponents)
	{
		this.chordComponents = chordComponents;
	}
}