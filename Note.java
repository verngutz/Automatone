public class Note implements Comparable<Note>
{
	private NoteName noteName;
	private int octave;
	private double remainingDuration;
	private int startMeasure;
	private double startBeat;
	
	//octave - should be between ?
	//duration - 1 means whole note, 0.5 means half note, and so on
	//start_measure - start counting at 0
	//start_beat - 0 means start immediately once start_measure has been reached, 
	//             0.5 means start a half note after start_measure has been reached, and so on.
	public Note(NoteName note_name, int octave, double duration, int start_measure, double start_beat)
	{
		this.noteName = note_name;
		this.octave = octave;
		this.remainingDuration = duration;
		this.startMeasure = start_measure;
		this.startBeat = start_beat;
	}
	
	//letter - should be 'c', 'd', 'e', 'f', 'g', 'a', or 'b' only
	//accidental - should be '#', 'b', or ''
	//octave - should be between ?
	//duration - 1 means whole note, 0.5 means half note, and so on
	//start_measure - start counting at 0
	//start_beat - 0 means start immediately once start_measure has been reached, 
	//             0.5 means start a half note after start_measure has been reached, and so on.
	public Note(char letter, char accidental, int octave, double duration, int start_measure, double start_beat)
	{
		this.noteName = new NoteName(letter, accidental);
		this.octave = octave;
		this.remainingDuration = duration;
		this.startMeasure = start_measure;
		this.startBeat = start_beat;
	}
	
	public int getStartMeasure()
	{
		return startMeasure;
	}
	
	public double getStartBeat()
	{
		return startBeat;
	}
	
	//beat_resolution will be passed by NoteThread and should ideally be less than the initial duration of any Note
	public double update(double beat_resolution)
	{
		remainingDuration -= beat_resolution;
		return remainingDuration;
	}
	
	public String toString()
	{
		return noteName.toString() + octave;
	}
	
	public int compareTo(Note n)
	{
		return (int) Math.round((startMeasure + startBeat) * 100 - (n.startMeasure + n.startBeat) * 100);
	}
}

