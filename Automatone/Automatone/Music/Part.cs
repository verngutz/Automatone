using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone.Music
{
    public class Part
    {
        private MusicTheory theory;
        private Random rand;
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

        public Part(MusicTheory theory, Random rand, Rhythm rhythm, int rhythmNumber, Melody melody, int melodyNumber, int measureLength)
        {
            //Get instance of InputParameters
            InputParameters inputParameters = InputParameters.Instance;

            this.theory = theory;
            this.rand = rand;
            this.rhythm = rhythm;
            this.rhythmNumber = rhythmNumber;
            this.melody = melody;
            this.melodyNumber = melodyNumber;
            this.measureLength = measureLength;

            //Set rhythm crowdedness based on input parameters
            rhythmCrowdedness = inputParameters.meanPartRhythmCrowdedness + (inputParameters.meanPartRhythmCrowdedness * (rand.NextDouble() - 0.5) * inputParameters.partRhythmCrowdednessVariance);

            //Set note length adjustment based on input parameters
            noteLengthAdjustment = 0.5 + (0.5 * (rand.NextDouble() - 0.5) * inputParameters.partNoteLengthVariance);

            //Set octave range based on input parameters
            octaveRange = (int)Math.Round((theory.PART_OCTAVE_RANGE * inputParameters.meanPartOctaveRange) + ((theory.PART_OCTAVE_RANGE * inputParameters.meanPartOctaveRange) * (rand.NextDouble() - 0.5) * inputParameters.partOctaveRangeVariance));
            
            //Set random lower pitch limit
            lowerPitchLimit = rand.Next(Automatone.PIANO_SIZE - (octaveRange * MusicTheory.OCTAVE_SIZE)) + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER;

            forceChord = false;
            forceDiatonic = false;

            //Set regularity based on input parameters
            regularity = 0;
            if (rand.NextDouble() < inputParameters.beatDefinition)
            {
                forceChord = true;
                regularity = rand.NextDouble();
            }
        }

        public Part(MusicTheory theory, Random rand, Rhythm rhythm, int rhythmNumber, Melody melody, int melodyNumber, int measureLength, double rhythmCrowdedness, double noteLengthAdjustment, double regularity, int lowerPitchLimit, int octaveRange, bool forceChord, bool forceDiatonic)
            : this(theory, rand, rhythm, rhythmNumber, melody, melodyNumber, measureLength)
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
            //Get instance of InputParameters
            InputParameters inputParameters = InputParameters.Instance;

            //Create seeded random number sequences for rhythm, melody, and pitch change
            Random randomRhythm = new Random(rhythmSeeds.ElementAt<int>(rhythmNumber % rhythmSeeds.Count));
            Random randomMelody = new Random(melodySeeds.ElementAt<int>(melodyNumber % melodySeeds.Count));
            Random randomPitch = new Random(melodySeeds.ElementAt<int>(melodyNumber % melodySeeds.Count));

            //Get rhythm curve and melody bias
            double[] rhythmCurve = rhythm.RhythmCurve;
            double[] melodyBias = melody.MelodyBias;

            //Create sorted list of rhythm curve values to be used in song regularity
            List<double> regulars = new List<double>();
            foreach (double val in rhythmCurve)
            {
                if (!regulars.Contains(val))
                {
                    regulars.Add(val);
                }
            }
            regulars.Sort();

            //set mean note length
            double noteLength = theory.NOTE_LENGTHINESS * inputParameters.meanNoteLength;
            noteLength += noteLength * (noteLengthAdjustment - 0.5);
            noteLength += noteLength * (regularity - 0.5) * 0.2;

            //Select random starting pitch
            int pitch = randomPitch.Next(Math.Max(Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, lowerPitchLimit), Math.Min(Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, octaveRange * MusicTheory.OCTAVE_SIZE + lowerPitchLimit));

            //Build notes
            List<Note> notes = new List<Note>();
            for (int i = 0; i < measureLength; i++)
            {
                //Change pitch
                int change = (int)(randomPitch.Next((int)(2 * melody.PitchContiguity)) - melody.PitchContiguity);
                pitch += change;
                
                //Adjust note length
                double currNoteLength = noteLength;
                currNoteLength += currNoteLength * ((randomRhythm.NextDouble() - 0.5) * inputParameters.noteLengthVariance);
                currNoteLength = Math.Pow(0.5, Math.Round(Math.Log(currNoteLength) / Math.Log(0.5)));

                //Adjust pitch based on melody bias
                if (chord.Count > 0 && (forceChord || randomMelody.NextDouble() < melodyBias[0]))
                {
                    while (!chord.Contains(new NoteName((byte)((pitch + MusicTheory.OCTAVE_SIZE) % MusicTheory.OCTAVE_SIZE))))
                    {
                        pitch = (change > 0 ? pitch + 1 : pitch - 1);
                    }
                }
                else if (diatonic.Count > 0 && (forceDiatonic || randomMelody.NextDouble() < melodyBias[1]))
                {
                    while (!diatonic.Contains(new NoteName((byte)((pitch + MusicTheory.OCTAVE_SIZE) % MusicTheory.OCTAVE_SIZE))))
                    {
                        pitch = (change > 0 ? pitch + 1 : pitch - 1);
                    }
                }

                //Adjust pitch octave if out of range
                while (pitch >= Math.Min(Automatone.PIANO_SIZE + Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, lowerPitchLimit + octaveRange * MusicTheory.OCTAVE_SIZE))
                {
                    pitch -= MusicTheory.OCTAVE_SIZE;
                }
                while (pitch < Math.Max(Automatone.LOWEST_NOTE_CHROMATIC_NUMBER, lowerPitchLimit))
                {
                    pitch += MusicTheory.OCTAVE_SIZE;
                }

                //Add note based on rhythm, rhythm crowdedness, and regularity
                if (1 - Math.Pow(randomRhythm.NextDouble(), 2 * (1 - regularity) * (1 - rhythmCrowdedness)) < rhythmCurve[i] && rhythmCurve[i] >= regulars.ElementAt<double>((int)Math.Round(regularity * (regulars.Count - 1))))
                {
                    notes.Add(new Note(new NoteName((byte)(pitch % MusicTheory.OCTAVE_SIZE)), (byte)(pitch / MusicTheory.OCTAVE_SIZE), currNoteLength, 0, Automatone.getBeatResolution() * i));
                }
            }
            return notes;
        }
    }
}
