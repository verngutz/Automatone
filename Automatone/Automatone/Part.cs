using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Part
    {
        private MusicTheory theory;
        private InputParameters inputParameters;
        private Rhythm rhythm;
        private int rhythmNumber;
        private Melody melody;
        private int melodyNumber;
        private int measureLength;
        private double rhythmCrowdedness;
        private double noteLengthAdjustment;
        private double regularity;
        private int lowerPitchLimit;
        private int octaveRange;
        private int multiplicity;
        private bool forceChord;
        private bool forceDiatonic;

        public Part(MusicTheory theory, InputParameters inputParameters, Rhythm rhythm, int rhythmNumber, Melody melody, int melodyNumber, int measureLength, double rhythmCrowdedness, double noteLengthAdjustment, double regularity, int lowerPitchLimit, int octaveRange, int multiplicity, bool forceChord, bool forceDiatonic)
        {
            this.theory = theory;
            this.inputParameters = inputParameters;
            this.rhythm = rhythm;
            this.rhythmNumber = rhythmNumber;
            this.melody = melody;
            this.melodyNumber = melodyNumber;
            this.measureLength = measureLength;
            this.rhythmCrowdedness = rhythmCrowdedness;
            this.noteLengthAdjustment = noteLengthAdjustment;
            this.regularity = regularity;
            this.lowerPitchLimit = lowerPitchLimit;
            this.octaveRange = octaveRange;
            this.multiplicity = multiplicity;
            this.forceChord = forceChord;
            this.forceDiatonic = forceDiatonic;
        }

        public List<Note> GenerateNotes(int rhythmSeed, int melodySeed, List<NoteName> chord, List<NoteName> diatonic)
        {
            List<Note> notes = new List<Note>();
            double[] rhythmCurve = rhythm.GetRhythmCurve(measureLength);
            double[] melodyBias = melody.GetMelodyBias();

            Random randomRhythm = new Random((rhythmSeed * rhythmNumber) % int.MaxValue);
            Random randomMelody = new Random((melodySeed * melodyNumber) % int.MaxValue);

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

            int pitch = randomMelody.Next(Math.Max(0,lowerPitchLimit), Math.Min(theory.PIANO_SIZE, octaveRange * 12 + lowerPitchLimit));

            for (int i = 0; i < measureLength; i++)
            {
                int change = (int)(randomMelody.Next((int)(2 * melody.GetPitchContiguity())) - melody.GetPitchContiguity());
                        
                double noteLength = theory.NOTE_LENGTHINESS * inputParameters.meanNoteLength;
                noteLength += noteLength * (noteLengthAdjustment - 0.5);
                noteLength += noteLength * ((randomRhythm.NextDouble() - 0.5) * inputParameters.noteLengthVariance);
                noteLength = Math.Pow(0.5, Math.Round(Math.Log(noteLength) / Math.Log(0.5)));

                for (int j = 0; j < multiplicity; j++)
                {
                    if (i % Math.Max(mod,1) == 0 && 1 - Math.Pow(randomRhythm.NextDouble(), 2 * (1 - rhythmCrowdedness)) < rhythmCurve[i])
                    {
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
                        while (pitch >= Math.Min(theory.PIANO_SIZE, lowerPitchLimit + octaveRange * 12))
                        {
                            pitch -= 12;
                        }
                        while (pitch < Math.Max(0, lowerPitchLimit))
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
