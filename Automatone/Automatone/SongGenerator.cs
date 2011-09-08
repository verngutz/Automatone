using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Automatone
{
    public class SongGenerator
    {
        private const byte START_OFFSET = 21;
        public static void RewriteSong(Automatone automatone)
        {
            NoteThread thread = new NoteThread(Automatone.getBeatResolution(), automatone.SongCells);
            automatone.Song = thread.ToString();
        }
	    public static void GenerateSong(Automatone automatone, Random random, MusicTheory theory, out CellState[,] song)
        {
            Song s = new Song(theory, random);
		    NoteThread thread = new NoteThread(Automatone.getBeatResolution(), s.Notes);
            CellState[,] grid = new CellState[theory.PIANO_SIZE,s.MeasureCount * Automatone.SUBBEATS_PER_MEASURE];
            foreach (List<Note> nts in s.Notes)
            {
                foreach (Note n in nts)
                {
                    grid[n.GetOctave() * 12 + n.GetNoteName().ChromaticIndex, Math.Min(n.GetStartMeasure() * Automatone.SUBBEATS_PER_MEASURE + (int)(n.GetStartBeat() * Automatone.SUBBEATS_PER_MEASURE), s.MeasureCount * Automatone.SUBBEATS_PER_MEASURE - 1)] = CellState.START;
                    for (int i = 1; i < n.GetRemainingDuration() * Automatone.SUBBEATS_PER_MEASURE; i++)
                    {
                        if (grid[n.GetOctave() * 12 + n.GetNoteName().ChromaticIndex, Math.Min(i + n.GetStartMeasure() * Automatone.SUBBEATS_PER_MEASURE + (int)(n.GetStartBeat() * Automatone.SUBBEATS_PER_MEASURE), s.MeasureCount * Automatone.SUBBEATS_PER_MEASURE - 1)] == CellState.SILENT)
                        {
                            grid[n.GetOctave() * 12 + n.GetNoteName().ChromaticIndex, Math.Min(i + n.GetStartMeasure() * Automatone.SUBBEATS_PER_MEASURE + (int)(n.GetStartBeat() * Automatone.SUBBEATS_PER_MEASURE), s.MeasureCount * Automatone.SUBBEATS_PER_MEASURE - 1)] = CellState.HOLD;
                        }
                    }
                }
            }
            song = grid;
            automatone.Song = thread.ToString();
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

            public NoteThread(double beat_resolution, CellState[,] notes)
            {
                this.notes = new List<Note>();
                for (int i = 0; i < notes.GetLength(0); i++)
                {
                    for (int j = 0; j < notes.GetLength(1); j++)
                    {
                        if (notes[i, j] == CellState.START)
                        {
                            double duration = Automatone.getBeatResolution();
                            int k = j + 1;
                            while (k < notes.GetLength(1) && notes[i, k] == CellState.HOLD)
                            {
                                duration += Automatone.getBeatResolution();
                                k++;
                            }
                            this.notes.Add(new Note(new NoteName((byte)(i % 12)), (byte)(i / 12), duration, (int)(j / Automatone.SUBBEATS_PER_MEASURE), (j % Automatone.SUBBEATS_PER_MEASURE) / (double)Automatone.SUBBEATS_PER_MEASURE));
                        }
                    }
                }
                beatResolution = beat_resolution;
            }

		    public NoteThread(double beat_resolution, List<List<Note>> notes)
		    {
                this.notes = new List<Note>();
                foreach (List<Note> nts in notes)
                {
                    foreach (Note n in nts)
                    {
                        this.notes.Add(n);
                    }
                }
			    beatResolution = beat_resolution;
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
                while (notes.Count > 0 || activeNotes.Count > 0)
                {
                    while (notes.Count > 0 && Math.Abs(notes.First<Note>().GetStartMeasure() + notes.First<Note>().GetStartBeat() - timePassed) <= EPSILON)
                    {
                        Note toActivate = notes.First<Note>();
                        notes.Remove(notes.First<Note>());
                        activeNotes.Add(toActivate);
                        thread.Append(TurnNoteOn(midiTimePassed, 1, toActivate.MidiNumber, 127));
                    }
                    timePassed += beatResolution;
                    midiTimePassed += (ulong)(beatResolution * 768);
                    foreach (Note n in activeNotes)
                    {
                        if (n.Update(beatResolution) <= EPSILON)
                        {
                            expiredNotes.Add(n);
                            thread.Append(TurnNoteOff(midiTimePassed, 1, n.MidiNumber, 0));
                        }
                    }
                    foreach (Note n in expiredNotes)
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
