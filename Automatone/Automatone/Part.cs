using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Part
    {
        private MusicTheory theory;
        private Random rand;
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
        private bool forceChord;
        private bool forceDiatonic;

        public Part(MusicTheory theory, Random rand, InputParameters inputParameters, Rhythm rhythm, int rhythmNumber, Melody melody, int melodyNumber, int measureLength)
        {
            this.theory = theory;
            this.rand = rand;
            this.inputParameters = inputParameters;
            this.rhythm = rhythm;
            this.rhythmNumber = rhythmNumber;
            this.melody = melody;
            this.melodyNumber = melodyNumber;
            this.measureLength = measureLength;
            rhythmCrowdedness = inputParameters.meanPartRhythmCrowdedness + (inputParameters.meanPartRhythmCrowdedness * (rand.NextDouble() - 0.5) * inputParameters.partRhythmCrowdednessVariance);
            noteLengthAdjustment = 0.5 + (0.5 * (rand.NextDouble() - 0.5) * inputParameters.partNoteLengthVariance);
            octaveRange = (int)Math.Round((theory.PART_OCTAVE_RANGE * inputParameters.meanPartOctaveRange) + ((theory.PART_OCTAVE_RANGE * inputParameters.meanPartOctaveRange) * (rand.NextDouble() - 0.5) * inputParameters.partOctaveRangeVariance));
            lowerPitchLimit = rand.Next(Automatone.PIANO_SIZE - (octaveRange * 12)) + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER;
            forceChord = rand.NextDouble() < inputParameters.forceChordChance;
            forceDiatonic = rand.NextDouble() < inputParameters.forceDiatonicChance;
            regularity = 0;
            if (rand.NextDouble() < inputParameters.beatDefinition)
            {
                forceChord = true;
                regularity = rand.NextDouble();
            }
        }

        public Part(MusicTheory theory, Random rand, InputParameters inputParameters, Rhythm rhythm, int rhythmNumber, Melody melody, int melodyNumber, int measureLength, double rhythmCrowdedness, double noteLengthAdjustment, double regularity, int lowerPitchLimit, int octaveRange, bool forceChord, bool forceDiatonic)
            : this(theory, rand, inputParameters, rhythm, rhythmNumber, melody, melodyNumber, measureLength)
        {
            this.rhythmCrowdedness = rhythmCrowdedness;
            this.noteLengthAdjustment = noteLengthAdjustment;
            this.regularity = regularity;
            this.lowerPitchLimit = lowerPitchLimit;
            this.octaveRange = octaveRange;
            this.forceChord = forceChord;
            this.forceDiatonic = forceDiatonic;
        }

        public List<Note> GenerateNotes(List<int> rhythmSeeds, List<int> melodySeeds, List<NoteName> chord, List<NoteName> diatonic)
        {
            List<Note> notes = new List<Note>();
            double[] rhythmCurve = rhythm.GetRhythmCurve(measureLength);
            double[] melodyBias = melody.GetMelodyBias();

            Random randomRhythm = new Random(rhythmSeeds.ElementAt<int>(rhythmNumber % rhythmSeeds.Count));
            Random randomMelody = new Random(melodySeeds.ElementAt<int>(melodyNumber % melodySeeds.Count));
            Random randomPitch = new Random(melodySeeds.ElementAt<int>(melodyNumber % melodySeeds.Count));

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

            int pitch = randomPitch.Next(Math.Max(Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, lowerPitchLimit), Math.Min(Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, octaveRange * 12 + lowerPitchLimit));

            double noteLength = theory.NOTE_LENGTHINESS * inputParameters.meanNoteLength;
            noteLength += noteLength * (noteLengthAdjustment - 0.5);
            noteLength += noteLength * (regularity - 0.5) * 0.2;

            for (int i = 0; i < measureLength; i++)
            {
                int change = (int)(randomPitch.Next((int)(2 * melody.GetPitchContiguity())) - melody.GetPitchContiguity());
                pitch += change;
                    
                double currNoteLength = noteLength;
                currNoteLength += currNoteLength * ((randomRhythm.NextDouble() - 0.5) * inputParameters.noteLengthVariance);
                currNoteLength = Math.Pow(0.5, Math.Round(Math.Log(currNoteLength) / Math.Log(0.5)));

                if (chord.Count > 0 && (forceChord || randomMelody.NextDouble() < melodyBias[0]))
                {
                    while (!chord.Contains(new NoteName((byte)((pitch + 12) % 12))))
                    {
                        pitch = (change > 0 ? pitch + 1 : pitch - 1);
                    }
                }
                else if (diatonic.Count > 0 && (forceDiatonic || randomMelody.NextDouble() < melodyBias[1]))
                {
                    while (!diatonic.Contains(new NoteName((byte)((pitch + 12) % 12))))
                    {
                        pitch = (change > 0 ? pitch + 1 : pitch - 1);
                    }
                }
                while (pitch >= Math.Min(Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, lowerPitchLimit + octaveRange * 12))
                {
                    pitch -= 12;
                }
                while (pitch < Math.Max(Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, lowerPitchLimit))
                {
                    pitch += 12;
                }

                if (1 - Math.Pow(randomRhythm.NextDouble(), 2 * (1 - regularity) * (1 - rhythmCrowdedness)) < rhythmCurve[i] && i % Math.Max(mod, 1) == 0)
                {
                    notes.Add(new Note(new NoteName((byte)(pitch % 12)), (byte)(pitch / 12), currNoteLength, 0, Automatone.getBeatResolution() * i));
                }
            }
            return notes;
        }
    }
}
