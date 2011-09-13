using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Part
    {
        private MusicTheory theory;
        private Rhythm rhythm;
        private Melody melody;
        private int measureLength;
        private double meanNoteLength;
        private double noteLengthVariance;
        private double rhythmCrowdedness;
        private double noteLengthAdjustment;
        private double regularity;
        private int lowerPitchLimit;
        private int pitchRange;
        private int multiplicity;
        private bool forceChord;
        private bool forceDiatonic;

        private double[] rhythmSeed;
        private double[] melodySeed;

        public Part(MusicTheory theory, Rhythm rhythm, Melody melody, int measureLength, double meanNoteLength, double noteLengthVariance, double rhythmCrowdedness, double noteLengthAdjustment, double regularity, int lowerPitchLimit, int pitchRange, int multiplicity, bool forceChord, bool forceDiatonic)
        {
            this.theory = theory;
            this.rhythm = rhythm;
            this.melody = melody;
            this.measureLength = measureLength;
            this.meanNoteLength = meanNoteLength;
            this.noteLengthVariance = noteLengthVariance;
            this.rhythmCrowdedness = rhythmCrowdedness;
            this.noteLengthAdjustment = noteLengthAdjustment;
            this.regularity = regularity;
            this.lowerPitchLimit = Math.Max(0,lowerPitchLimit);
            this.pitchRange = Math.Min(pitchRange, theory.PIANO_SIZE-lowerPitchLimit);
            this.multiplicity = multiplicity;
            this.forceChord = forceChord;
            this.forceDiatonic = forceDiatonic;
        }

        public List<Note> GenerateNotes(double[] rhythmSeed, double[] melodySeed, List<NoteName> chord, List<NoteName> diatonic)
        {
            List<Note> notes = new List<Note>();
            double[] rhythmCurve = rhythm.GetRhythmCurve(measureLength);
            double[] melodyBias = melody.GetMelodyBias();

            this.rhythmSeed = new double[rhythmSeed.Length];
            rhythmSeed.CopyTo(this.rhythmSeed, 0);
            this.melodySeed = new double[melodySeed.Length];
            melodySeed.CopyTo(this.melodySeed, 0);


            int f = 1;
            int mod = measureLength;
            List<int> regulars = new List<int>();
            regulars.Add(f);
            for(int i = 2; i <= measureLength/2; i++){
                while(mod % i == 0)
                {
                    mod /= i;
                    f *= i;
                    regulars.Add(f);
                }
            }
            mod = regulars.ElementAt<int>((int)Math.Round(regularity*(regulars.Count-1)));

            for (int i = 0; i < measureLength; i++)
            {
                double noteLength = theory.NOTE_LENGTHINESS * meanNoteLength;
                noteLength += noteLength * (noteLengthAdjustment - 0.5);
                noteLength += noteLength * ((NextRhythmValue() - 0.5) * noteLengthVariance);
                noteLength = Math.Pow(0.5, Math.Round(Math.Log(noteLength) / Math.Log(0.5)));

                for (int j = 0; j < multiplicity; j++)
                {
                    if (i % Math.Max(mod,1) == 0 && 1 - Math.Pow(NextRhythmValue(), 2 * (1 - rhythmCrowdedness)) < rhythmCurve[i])
                    {
                        int pitch = (int)(lowerPitchLimit + NextMelodyValue() * pitchRange);
                        if (forceChord || NextMelodyValue() < melodyBias[0])
                        {
                            while (!chord.Contains(new NoteName((byte)(pitch % 12))))
                                pitch = (int)(lowerPitchLimit + NextMelodyValue() * pitchRange);
                            notes.Add(new Note(new NoteName((byte)(pitch % 12)), (byte)(pitch / 12), noteLength, 0, Automatone.getBeatResolution() * i));
                        }
                        else if (forceDiatonic || NextMelodyValue() < melodyBias[1])
                        {
                            while (!diatonic.Contains(new NoteName((byte)(pitch % 12))))
                                pitch = (int)(lowerPitchLimit + NextMelodyValue() * pitchRange);
                            notes.Add(new Note(new NoteName((byte)(pitch % 12)), (byte)(pitch / 12), noteLength, 0, Automatone.getBeatResolution() * i));
                        }
                        else
                        {
                            notes.Add(new Note(new NoteName((byte)(pitch % 12)), (byte)(pitch / 12), noteLength, 0, Automatone.getBeatResolution() * i));
                        }
                    }
                }
            }
            return notes;
        }

        private double NextRhythmValue()
        {
            double temp = rhythmSeed[0];
            for (int i = 1; i < rhythmSeed.Length; i++)
            {
                rhythmSeed[i - 1] = rhythmSeed[i];
            }
            rhythmSeed[rhythmSeed.Length - 1] += temp;
            while (rhythmSeed[rhythmSeed.Length - 1] > 1)
            {
                rhythmSeed[rhythmSeed.Length - 1]--;
            }
            return rhythmSeed[rhythmSeed.Length - 1];
        }

        private double NextMelodyValue()
        {
            double temp = melodySeed[0];
            for (int i = 1; i < melodySeed.Length; i++)
            {
                melodySeed[i - 1] = melodySeed[i];
            }
            melodySeed[melodySeed.Length - 1] += temp;
            while (melodySeed[melodySeed.Length - 1] > 1)
            {
                melodySeed[melodySeed.Length - 1]--;
            }
            return melodySeed[melodySeed.Length - 1];
        }
    }
}
