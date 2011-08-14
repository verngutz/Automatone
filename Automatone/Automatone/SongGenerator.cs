using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoodSwingCoreComponents;

namespace Automatone
{
    public class SongGenerator
    {
        private const double INIT_DONE_PROBABILITY = 0.4;
	    private const double INIT_REPEAT_PROBABILITY = 0.5;
	    private const double INIT_MAKENEW_PROBABILITY = 0.5;
	    private const double PROBABILITY_ADJUSTOR = 0.1;
	
	    MSRandom random;
	    Theory theory;
	    NoteThread thread;
	
	    public SongGenerator(MSRandom random)
	    {
		    this.random = random;
	    }
	
	    public String generateSong(Theory theory, out List<CellState[,]> generatedVerses)
	    {
		    this.theory = theory;
		    thread = new NoteThread(theory.getBeatResolution());
		    VerseGenerator vg = new VerseGenerator();
		    generatedVerses = new List<CellState[,]>();
		    CellState[,] songCells = vg.generateVerse(theory);
		    generatedVerses.Add( songCells );
		
		    bool done = false;
		    double doneProb = INIT_DONE_PROBABILITY;
		    double repeatProb = INIT_REPEAT_PROBABILITY;
		    double makeNewProb = INIT_MAKENEW_PROBABILITY;
		    while(!done)
		    {
			    if(random.GetUniform() < makeNewProb)
			    {
				    songCells = vg.generateVerse(theory);
				    generatedVerses.Add( songCells );
				    makeNewProb -= PROBABILITY_ADJUSTOR;
				    repeatProb += PROBABILITY_ADJUSTOR;
				    doneProb += PROBABILITY_ADJUSTOR;
                    
			    }
			    else
			    {
				    if(random.GetUniform() < repeatProb)
				    {
					    songCells = generatedVerses.ElementAt<CellState[,]>(random.GetUniformInt(generatedVerses.Count));
					    generatedVerses.Add( songCells );
					    repeatProb -= PROBABILITY_ADJUSTOR;
					    makeNewProb += PROBABILITY_ADJUSTOR;
					    doneProb += PROBABILITY_ADJUSTOR;
				    }
				    else if(random.GetUniform() < doneProb)
				    {
					    done = true;
				    }
				
			    }
		    }
		
		    int measureCounter = 0;
		    foreach(CellState[,] cs in generatedVerses)
		    {
			    measureCounter = verseToThread(cs, measureCounter);
		    }
		    return thread.ToString();
	    }
	
	
	
	    private int verseToThread(CellState[,] songCells, int globalStartMeasure)
	    {
		    int globalEndMeasure = globalStartMeasure;
		    for(int i = 0; i < songCells.GetLength(0); i++)
		    {
			    for(int j = 0; j < songCells.GetLength(1); j++)
			    {
				    if(songCells[i, j] == CellState.START)
				    {
					    globalEndMeasure = globalStartMeasure + (int)(j * theory.getBeatResolution());
					    double startBeat = (j % (int) Math.Round(1 / theory.getBeatResolution()) * theory.getBeatResolution());
					
					    int integerDuration = 1;
					    for(int x = j + 1; x < songCells.GetLength(1) && songCells[i, x] == CellState.HOLD; x++)
						    integerDuration++;
						
					    thread.addNote(new Note(theory.getNoteName(i), theory.getOctave(i), integerDuration * theory.getBeatResolution(), globalEndMeasure, startBeat));
				    }
			    }
		    }
		    return globalEndMeasure;
	    }
	
	    class Note : IComparable<Note>
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
		
		    public override String ToString()
		    {
			    return noteName.ToString() + octave;
		    }

		    public int CompareTo(Note n)
		    {
			    return (int) Math.Round((startMeasure + startBeat) * 100 - (n.startMeasure + n.startBeat) * 100);
		    }
	    }
	
	    class NoteThread
	    {
		    private const double EPSILON = 0.00001;

		    private List<Note> notes;
		    private double beatResolution;
		
		    //Limited to 4/4 time as of now
		    public NoteThread(double beat_resolution)
		    {
			    notes = new List<Note>();
			    beatResolution = beat_resolution;
		    }
		
		    public void addNote(Note n)
		    {
			    notes.Add(n);
		    }
		
		    private String startTrack(int track, int port, int channel, String instr, int volume, int balance, int reverb)
		    {
			    return "mtrk(" + track + ")\n\tprefixport " + port + "\n\tprefixchannel " + channel + "\n\tprogram " + instr + "\n\tvolume " + volume + "\n\tbalance " + balance + "\n\treverb " + reverb + "\n";
		    }
		
		    public override String ToString()
		    {
                notes.Sort();
			    String thread = startTrack(1, 0, 1, "GrandPno", 127, 64, 64);
			    double timePassed = 0;
			    List<Note> activeNotes = new List<Note>();
			    List<Note> expiredNotes = new List<Note>();
			    while(notes.Count > 0 || activeNotes.Count > 0)
			    {
				    while(notes.Count > 0 && Math.Abs(notes.First<Note>().getStartMeasure() + notes.First<Note>().getStartBeat() - timePassed) <= EPSILON)
				    {
					    Note toActivate = notes.First<Note>();
                        notes.Remove(notes.First<Note>());
					    activeNotes.Add(toActivate);
					    thread += "\t\t+" + toActivate.ToString() + " $7F;\n";
				    }
				    thread += "\t1/8;\n";//beats per minute
				    timePassed += beatResolution;
				    foreach(Note n in activeNotes)
				    {
					    if(n.update(beatResolution) <= EPSILON)
					    {
						    expiredNotes.Add(n);
						    thread += "\t\t-" + n.ToString() + " $00;\n";
					    }
				    }
				    foreach(Note n in expiredNotes)
				    {
					    activeNotes.Remove(n);
				    }
				    expiredNotes.Clear();
			    }
			    thread += "end mtrk\n";
			    return thread;
		    }
	    }
    }
}
