import java.util.*;

public class NoteThread
{
	private static final double BEAT_RESOLUTION = 1 / 16.0;
	private static final double EPSILON = 0.00001;
	
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
		double timePassed = 0;
		ArrayList<Note> activeNotes = new ArrayList<Note>();
		ArrayList<Note> expiredNotes = new ArrayList<Note>();
		while(notes.size() > 0 || activeNotes.size() > 0 || expiredNotes.size() > 0)
		{
			while(Math.abs(notes.peek().getStartMeasure() + notes.peek().getStartBeat() - timePassed) <= EPSILON)
			{
				Note toActivate = notes.poll();
				activeNotes.add(toActivate);
				thread += "\t\t+" + toActivate.toString() + " $7F;\n";
			}
			thread += "\t1/16;\n";
			timePassed += BEAT_RESOLUTION;
			for(Note n : activeNotes)
			{
				if(n.update(BEAT_RESOLUTION) <= EPSILON)
				{
					expiredNotes.add(n);
					thread += "\t\t-" + n.toString() + " $00;\n";
				}
			}
			for(Note n : expiredNotes)
			{
				activeNotes.remove(n);
			}
		}
		return thread;
	}
}