using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone.Music
{
    public class Measure
    {
        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Measure(MusicTheory theory, Random rand, List<Part> parts, int phraseLength, List<int> rhythmSeeds, List<int> melodySeeds, List<NoteName> chord, List<NoteName> diatonic)
        {
            //Get instance of InputParameters
            InputParameters inputParameters = InputParameters.Instance;

            notes = new List<List<Note>>();

            //Select seeds for rhythm and melody
            List<int> selectedRhythmSeeds = new List<int>();
            for (int i = 0; i < 1 + inputParameters.MeasureRhythmVariance * (parts.Count); i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<int>(rand.Next(rhythmSeeds.Count)));
            }
            List<int> selectedMelodySeeds = new List<int>();
            for (int i = 0; i < 1 + inputParameters.MeasureMelodyVariance * (parts.Count); i++)
            {
                selectedMelodySeeds.Add(melodySeeds.ElementAt<int>(rand.Next(melodySeeds.Count)));
            }

            //Build notes from parts
            foreach (Part prt in parts)
            {
                notes.Add(prt.GenerateNotes(selectedRhythmSeeds, selectedMelodySeeds, chord, diatonic));
            }
        }
    }
}
