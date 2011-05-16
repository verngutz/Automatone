import java.util.*;

public class NoteThread
{
	private static final double BEAT_RESOLUTION = 1 / 16.0;
	
	private PriorityQueue<Note> notes;
	
	//Limited to 4/4 time as of now
	public NoteThread()
	{
		notes = new PriorityQueue<Note>();
	}
	
	public void addNote(Note n)
	{
		notes.offer(n);
	}
	
	public String toString()
	{
		String thread = "";
		int currentMeasure = 0;
		double currentBeat = 0;
		double timePassed = 0;
		ArrayList<Note> activeNotes = new ArrayList<Note>();
		while(notes.size() > 0)
		{
			//if(notes.peek().getStartMeasure() );
			timePassed += BEAT_RESOLUTION;
			currentMeasure = (int)Math.round(timePassed / 1);
			currentBeat = timePassed - currentMeasure;
		}
		return thread;
	}
}