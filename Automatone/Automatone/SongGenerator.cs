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
            NoteThread thread = new NoteThread(automatone.SongCells, automatone.MeasureLength, automatone.TimeSignature);
            automatone.Song = thread.ToString();
        }
	    public static CellState[,] GenerateSong(Automatone automatone, Random random, MusicTheory theory, InputParameters inputParameters)
        {
            Song s = new Song(theory, inputParameters, random);
		    NoteThread thread = new NoteThread(s.Notes, s.TimeSignature);
            int gridWidth = s.MeasureCount * s.MeasureLength;
            CellState[,] grid = new CellState[Automatone.PIANO_SIZE,gridWidth];
            foreach (List<Note> nts in s.Notes)
            {
                foreach (Note n in nts)
                {
                    grid[n.GetOctave() * 12 + n.GetNoteName().ChromaticIndex - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, (int)Math.Min(n.GetStartMeasure() * s.MeasureLength + n.GetStartBeat() * Automatone.SUBBEATS_PER_WHOLE_NOTE, gridWidth - 1)] = CellState.START;
                    for (int i = 1; i < n.GetRemainingDuration() * Automatone.SUBBEATS_PER_WHOLE_NOTE; i++)
                    {
                        if (grid[n.GetOctave() * 12 + n.GetNoteName().ChromaticIndex - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, (int)Math.Min(n.GetStartMeasure() * s.MeasureLength + n.GetStartBeat() * Automatone.SUBBEATS_PER_WHOLE_NOTE + i, gridWidth - 1)] == CellState.SILENT)
                        {
                            grid[n.GetOctave() * 12 + n.GetNoteName().ChromaticIndex - Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, (int)Math.Min(n.GetStartMeasure() * s.MeasureLength + n.GetStartBeat() * Automatone.SUBBEATS_PER_WHOLE_NOTE + i, gridWidth - 1)] = CellState.HOLD;
                        }
                    }
                }
            }
            automatone.MeasureLength = s.MeasureLength;
            automatone.TimeSignature = s.TimeSignature;
            automatone.TimeSignatureN = s.TimeSignatureN;
            automatone.TimeSignatureD = s.TimeSignatureD;
            automatone.Tempo = s.Tempo;
            //automatone.Song = thread.ToString(); //do we still need this?
            return grid;
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

            private double timeSignature;

            public NoteThread(CellState[,] notes, int measureLength, double timeSignature)
            {
                this.notes = new List<Note>();
                this.timeSignature = timeSignature;
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
                            this.notes.Add(new Note(new NoteName((byte)((i + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) % 12)), (byte)((i + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER) / 12), duration, (int)(j / measureLength), (j % measureLength) * Automatone.getBeatResolution()));
                        }
                    }
                }
            }

		    public NoteThread(List<List<Note>> notes, double timeSignature)
		    {
                this.notes = new List<Note>();
                foreach (List<Note> nts in notes)
                {
                    foreach (Note n in nts)
                    {
                        this.notes.Add(n);
                    }
                }
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
                    while (notes.Count > 0 && Math.Abs(notes.First<Note>().GetStartMeasure() * timeSignature + notes.First<Note>().GetStartBeat() - timePassed) <= EPSILON)
                    {
                        Note toActivate = notes.First<Note>();
                        notes.Remove(notes.First<Note>());
                        activeNotes.Add(toActivate);
                        thread.Append(TurnNoteOn(midiTimePassed, 1, toActivate.MidiNumber, 127));
                    }
                    timePassed += Automatone.getBeatResolution();
                    midiTimePassed += (ulong)(Automatone.getBeatResolution() * 768);
                    foreach (Note n in activeNotes)
                    {
                        if (n.Update(Automatone.getBeatResolution()) <= EPSILON)
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
