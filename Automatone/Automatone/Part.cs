using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Part
    {
        private int partNumber;
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
        private int octaveRange;
        private int multiplicity;
        private bool forceChord;
        private bool forceDiatonic;

        public Part(int partNumber, MusicTheory theory, Rhythm rhythm, Melody melody, int measureLength, double meanNoteLength, double noteLengthVariance, double rhythmCrowdedness, double noteLengthAdjustment, double regularity, int lowerPitchLimit, int octaveRange, int multiplicity, bool forceChord, bool forceDiatonic)
        {
            this.partNumber = partNumber;
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
            this.octaveRange = Math.Min(octaveRange * 12, theory.PIANO_SIZE - lowerPitchLimit)/12;
            this.multiplicity = multiplicity;
            this.forceChord = forceChord;
            this.forceDiatonic = forceDiatonic;
        }

        public List<Note> GenerateNotes(int rhythmSeed, int melodySeed, List<NoteName> chord, List<NoteName> diatonic)
        {
            List<Note> notes = new List<Note>();
            double[] rhythmCurve = rhythm.GetRhythmCurve(measureLength);
            double[] melodyBias = melody.GetMelodyBias();

            Random randomRhythm = new Random((rhythmSeed * partNumber) % int.MaxValue);
            Random randomMelody = new Random((melodySeed * partNumber) % int.MaxValue);

            System.Console.WriteLine("\t\t\t" + rhythmSeed + ":" + melodySeed);

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

            int pitch = randomMelody.Next(lowerPitchLimit, octaveRange * 12 + lowerPitchLimit);

            for (int i = 0; i < measureLength; i++)
            {
                double noteLength = theory.NOTE_LENGTHINESS * meanNoteLength;
                noteLength += noteLength * (noteLengthAdjustment - 0.5);
                noteLength += noteLength * ((randomRhythm.NextDouble() - 0.5) * noteLengthVariance);
                noteLength = Math.Pow(0.5, Math.Round(Math.Log(noteLength) / Math.Log(0.5)));

                for (int j = 0; j < multiplicity; j++)
                {
                    if (i % Math.Max(mod,1) == 0 && 1 - Math.Pow(randomRhythm.NextDouble(), 2 * (1 - rhythmCrowdedness)) < rhythmCurve[i])
                    {
                        int change = (int)(randomMelody.Next((int)(2 * melody.GetPitchContiguity())) - melody.GetPitchContiguity());
                        pitch += change;
                        if (forceChord || randomMelody.NextDouble() < melodyBias[0])
                        {
                            while (!chord.Contains(new NoteName((byte)(pitch % 12))))
                                pitch = (change > 0 ? pitch + 1 : pitch - 1);
                        }
                        else if (forceDiatonic || randomMelody.NextDouble() < melodyBias[1])
                        {
                            while (!diatonic.Contains(new NoteName((byte)(pitch % 12))))
                                pitch = (change > 0 ? pitch + 1 : pitch - 1);
                        }
                        while (pitch >= lowerPitchLimit + octaveRange * 12)
                        {
                            pitch -= 12;
                        }
                        while (pitch < lowerPitchLimit)
                        {
                            pitch += 12;
                        }
                        notes.Add(new Note(new NoteName((byte)(pitch % 12)), (byte)(pitch / 12), noteLength, 0, Automatone.getBeatResolution() * i));
                    }
                }
            }
            return notes;
        }
    }
}
