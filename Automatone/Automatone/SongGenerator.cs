using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoodSwingCoreComponents;

namespace Automatone
{
    public class SongGenerator
    {
	    public static String generateSong(Random random, MusicTheory theory, out CellState[,] song)
	    {
		    NoteThread thread = new NoteThread(theory.getBeatResolution());
            Song s = new Song(theory, random);
            song = s.Grid;

            for (int i = 0; i < song.GetLength(0); i++)
            {
                for (int j = 0; j < song.GetLength(1); j++)
                {
                    if (song[i, j] == CellState.START)
                    {
                        double startBeat = (j % (int)Math.Round(1 / theory.getBeatResolution()) * theory.getBeatResolution());

                        int integerDuration = 1;
                        for (int x = j + 1; x < song.GetLength(1) && song[i, x] == CellState.HOLD; x++)
                            integerDuration++;

                        thread.addNote(new Note(theory.getNoteName(i), theory.getOctave(i), integerDuration * theory.getBeatResolution(), (int)(j * theory.getBeatResolution()), startBeat));
                    }
                }
            }

		    return thread.ToString();
	    }
	
	    private int verseToThread(CellState[,] songCells, int globalStartMeasure)
	    {
		    int globalEndMeasure = globalStartMeasure;
		    
		    return globalEndMeasure;
	    }

	    private class NoteThread
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
