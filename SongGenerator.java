import java.util.*;

public class SongGenerator
{
	Random random;
	Theory theory;
	NoteThread thread;
	
	
	public SongGenerator(Random random)
	{
		this.random = random;
	}
	
	public String generateSong(Theory theory)
	{
		this.theory = theory;
		thread = new NoteThread(theory.getBeatResolution());
		VerseGenerator vg = new VerseGenerator();
		ArrayList<CellState[][]> generatedVerses = new ArrayList<CellState[][]>();
		CellState[][] songCells = vg.generateVerse(theory);
		generatedVerses.add( songCells );
		
		
		boolean done = false;
		double doneProb = 0;
		while(!done)
		{
			double randGS = random.nextDouble();
			if(randGS < ( 1.0 - doneProb ) / 2)
			{
				songCells = vg.generateVerse(theory);
				generatedVerses.add( songCells );
			}
			else if(randGS < 1.0 - doneProb)
			{
				songCells = generatedVerses.get(random.nextInt(generatedVerses.size()));
				generatedVerses.add( songCells );
			}
			else
				done = true;
			doneProb += 0.10;
		}
		
		int measureCounter = 0;
		for(CellState[][] cs : generatedVerses)
		{
			measureCounter = verseToThread(cs, measureCounter);
		}
		return thread.toString();
	}
	
	
	
	private int verseToThread(CellState[][] songCells, int globalStartMeasure)
	{
		int globalEndMeasure = globalStartMeasure;
		for(int i = 0; i < songCells.length; i++)
		{
			for(int j = 0; j < songCells[0].length; j++)
			{
				if(songCells[i][j] == CellState.START)
				{
					globalEndMeasure = globalStartMeasure + (int)(j * theory.getBeatResolution());
					double startBeat = (j % (int) Math.round(1 / theory.getBeatResolution()) * theory.getBeatResolution());
					
					int integerDuration = 1;
					for(int x = j + 1; x < songCells[0].length && songCells[i][x] == CellState.HOLD; x++)
						integerDuration++;
						
					thread.addNote(new Note(theory.getNoteName(i), theory.getOctave(i), integerDuration * theory.getBeatResolution(), globalEndMeasure, startBeat));
				}
			}
		}
		return globalEndMeasure;
	}
	
	class Note implements Comparable<Note>
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
		//accidental - should be '#', 'b', or ' '
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
	
	class NoteThread
	{
		private static final double EPSILON = 0.00001;
		
		private PriorityQueue<Note> notes;
		private double beatResolution;
		
		//Limited to 4/4 time as of now
		public NoteThread(double beat_resolution)
		{
			notes = new PriorityQueue<Note>();
			beatResolution = beat_resolution;
		}
		
		public void addNote(Note n)
		{
			notes.offer(n);
		}
		
		private String startTrack(int track, int port, int channel, String instr, int volume, int balance, int reverb)
		{
			return "mtrk(" + track + ")\n\tprefixport " + port + "\n\tprefixchannel " + channel + "\n\tprogram " + instr + "\n\tvolume " + volume + "\n\tbalance " + balance + "\n\treverb " + reverb + "\n";
		}
		
		public String toString()
		{
			String thread = startTrack(1, 0, 1, "GrandPno", 127, 64, 64);
			double timePassed = 0;
			ArrayList<Note> activeNotes = new ArrayList<Note>();
			ArrayList<Note> expiredNotes = new ArrayList<Note>();
			while(notes.size() > 0 || activeNotes.size() > 0)
			{
				while(notes.size() > 0 && Math.abs(notes.peek().getStartMeasure() + notes.peek().getStartBeat() - timePassed) <= EPSILON)
				{
					Note toActivate = notes.poll();
					activeNotes.add(toActivate);
					thread += "\t\t+" + toActivate.toString() + " $7F;\n";
				}
				thread += "\t1/8;\n";//beats per minute
				timePassed += beatResolution;
				for(Note n : activeNotes)
				{
					if(n.update(beatResolution) <= EPSILON)
					{
						expiredNotes.add(n);
						thread += "\t\t-" + n.toString() + " $00;\n";
					}
				}
				for(Note n : expiredNotes)
				{
					activeNotes.remove(n);
				}
				expiredNotes.clear();
			}
			thread += "end mtrk\n";
			return thread;
		}
	}
}