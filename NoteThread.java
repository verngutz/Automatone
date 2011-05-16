public class NoteThread
{
	private static final double BEAT_RESOLUTION = 1 / 16.0;
	
	private ArrayList<Note> notes;
	private ArrayList<Note> activeNotes;
	private ArrayList<Note> expiredNotes;
	private int currentMeasure;
	private double currentBeat;
	private double timePassed;
	private String thread;
	
	//Limited to 4/4 time as of now
	public NoteThread()
	{
		notes = new ArrayList<Note>();
		currentMeasure = 0;
		currentBeat = 0;
		thread = "";
	}
	
	//increment time and deallocate expired notes
	public void update()
	{
		timePassed += BEAT_RESOLUTION;
		currentMeasure = (int)Math.round(timePassed / 1);
		currentBeat = timePassed - currentMeasure;
	}
	
	//add a new note at the current time
	public void addNote(char note, char accidental, int octave, double duration)
	{
	}
	
	public String toString()
	{
		return thread;
	}
}