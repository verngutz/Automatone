using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Note : IComparable<Note>
    {
        
        private NoteName noteName;
        private byte octave;
        private double remainingDuration;
        private int startMeasure;
        private double startBeat;

        public byte MidiNumber { get { return (byte)(noteName.ChromaticIndex + (octave + 1) * 12); } }

        //lowest note is a0 = 21
        //highest note is c8 = 108
        //duration - 1 means whole note, 0.5 means half note, and so on
        //start_measure - start counting at 0
        //start_beat - 0 means start immediately once start_measure has been reached, 
        //             0.5 means start a half note after start_measure has been reached, and so on.
        public Note(NoteName note_name, byte octave, double duration, int start_measure, double start_beat)
        {
            this.noteName = note_name;
            this.octave = octave;
            this.remainingDuration = duration;
            this.startMeasure = start_measure;
            this.startBeat = start_beat;
        }

        public NoteName GetNoteName()
        {
            return noteName;
        }

        public byte GetOctave()
        {
            return octave;
        }

        public double GetRemainingDuration()
        {
            return remainingDuration;
        }

        public int GetStartMeasure()
        {
            return startMeasure;
        }

        public double GetStartBeat()
        {
            return startBeat;
        }

        public double Update(double beat_resolution)
        {
            remainingDuration -= beat_resolution;
            return remainingDuration;
        }

        public int CompareTo(Note n)
        {
            return (startMeasure == n.startMeasure ? (int)Math.Round((startBeat - n.startBeat) * 100) : startMeasure - n.startMeasure);
        }
    }
}
