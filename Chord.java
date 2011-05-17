public class Chord
{
	public enum Mode
	{
		MAJOR,
		MINOR,
		AUGMENTED,
		DIMINISHED
	}
	
	private NoteName base;
	private Mode mode;
	private ArrayList<NoteName> harmoniousNotes;
	private ArrayList<NoteName> neutralNotes;
	private ArrayList<NoteName> dissonantNotes;
	public Chord(NoteName base, Mode mode)
	{
		
	}
}