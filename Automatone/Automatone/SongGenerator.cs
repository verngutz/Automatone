using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Automatone
{
    public class SongGenerator
    {
        private const byte START_OFFSET = 21;
	    public static String GenerateSong(Random random, MusicTheory theory, out CellState[,] song)
	    {
		    NoteThread thread = new NoteThread(Automatone.getBeatResolution());
            Song s = new Song(theory, random);
            song = s.Grid;

            for (int i = 0; i < song.GetLength(0); i++)
            {
                for (int j = 0; j < song.GetLength(1); j++)
                {
                    if (song[i, j] == CellState.START)
                    {
                        double startBeat = (j % (int)Math.Round(1 / Automatone.getBeatResolution()) * Automatone.getBeatResolution());

                        int integerDuration = 1;
                        for (int x = j + 1; x < song.GetLength(1) && song[i, x] == CellState.HOLD; x++)
                            integerDuration++;

                        thread.AddNote(new Note(new NoteName((byte)((i + START_OFFSET) % 12)), (byte)((i + START_OFFSET) / 12), integerDuration * Automatone.getBeatResolution(), (int)(j * Automatone.getBeatResolution()), startBeat));
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

            private const string START_TRACK = "MTrk\n";
            private const string PREFIX_PORT = "0 Meta 0x21 00\n";
            private const string PREFIX_CHANNEL = "0 Meta 0x20 00\n";
            private const string META_END_TRACK = " Meta TrkEnd\n";
            private const string END_TRACK = "TrkEnd\n";

		    private List<Note> notes;
		    private double beatResolution;

		    public NoteThread(double beat_resolution)
		    {
			    notes = new List<Note>();
			    beatResolution = beat_resolution;
		    }
		
		    public void AddNote(Note n)
		    {
			    notes.Add(n);
		    }
		
            private string ProgramChange(ulong time, byte channel, byte programNumber)
            {
                return time + " PrCh ch=" + channel + " p=" + programNumber + "\n";
            }

            private string SetVolume(ulong time, byte channel, byte volume)
            {
                return time + " Par ch=" + channel + " c=7 v=" + volume + "\n";
            }

            private string SetPanning(ulong time, byte channel, byte pan)
            {
                return time + " Par ch=" + channel + " c=10 v=" + pan + "\n";
            }

            private string SetReverb(ulong time, byte channel, byte reverb)
            {
                return time + " Par ch=" + channel + " c=91 v=" + reverb + "\n";
            }

            private string TurnNoteOn(ulong time, byte channel, byte noteMidiNumber, byte velocity)
            {
                return time + " On ch=" + channel + " n=" + noteMidiNumber + " v=" + velocity + "\n";
            }

            private string TurnNoteOff(ulong time, byte channel, byte noteMidiNumber, byte velocity)
            {
                return time + " Off ch=" + channel + " n=" + noteMidiNumber + " v=" + velocity + "\n";
            }
		
		    public override String ToString()
		    {
                notes.Sort();
                StringBuilder thread = new StringBuilder();
                thread.Append(START_TRACK);
                thread.Append(PREFIX_PORT);
                thread.Append(PREFIX_CHANNEL);
                thread.Append(ProgramChange(0, 1, 0));
                thread.Append(SetVolume(0, 1, 127));
                thread.Append(SetPanning(0, 1, 64));
                thread.Append(SetReverb(0, 1, 64));
			    double timePassed = 0;
                ulong midiTimePassed = 0;
			    List<Note> activeNotes = new List<Note>();
			    List<Note> expiredNotes = new List<Note>();
			    while(notes.Count > 0 || activeNotes.Count > 0)
			    {
				    while(notes.Count > 0 && Math.Abs(notes.First<Note>().GetStartMeasure() + notes.First<Note>().GetStartBeat() - timePassed) <= EPSILON)
				    {
					    Note toActivate = notes.First<Note>();
                        notes.Remove(notes.First<Note>());
					    activeNotes.Add(toActivate);
					    thread.Append(TurnNoteOn(midiTimePassed, 1, toActivate.MidiNumber, 127));
				    }
				    timePassed += beatResolution;
                    midiTimePassed += (ulong)(beatResolution * 768);
				    foreach(Note n in activeNotes)
				    {
					    if(n.Update(beatResolution) <= EPSILON)
					    {
						    expiredNotes.Add(n);
                            thread.Append(TurnNoteOff(midiTimePassed, 1, n.MidiNumber, 0));
					    }
				    }
				    foreach(Note n in expiredNotes)
				    {
					    activeNotes.Remove(n);
				    }
				    expiredNotes.Clear();
			    }
                thread.Append(midiTimePassed);
                thread.Append(META_END_TRACK);
                thread.Append(END_TRACK);
			    return thread.ToString();
		    }
	    }
    }
}
