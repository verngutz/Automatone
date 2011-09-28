using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Measure
    {
        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Measure(MusicTheory theory, InputParameters inputParameters, Random rand, List<Part> parts, int phraseLength, List<int> rhythmSeeds, List<int> melodySeeds, List<NoteName> chord, List<NoteName> diatonic)
        {
            notes = new List<List<Note>>();

            //Select seeds
            List<int> selectedRhythmSeeds = new List<int>();
            for (int i = 0; i < 1 + inputParameters.measureRhythmVariance * parts.Count; i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<int>(rand.Next(rhythmSeeds.Count)));
            }
            List<int> selectedMelodySeeds = new List<int>();
            for (int i = 0; i < 1 + inputParameters.measureMelodyVariance * parts.Count; i++)
            {
                selectedMelodySeeds.Add(melodySeeds.ElementAt<int>(rand.Next(melodySeeds.Count)));
            }

            foreach (Part prt in parts)
            {
                notes.Add(prt.GenerateNotes(selectedRhythmSeeds.ElementAt<int>(rand.Next(selectedRhythmSeeds.Count)), selectedMelodySeeds.ElementAt<int>(rand.Next(selectedMelodySeeds.Count)), chord, diatonic));
            }
            System.Console.WriteLine("\t\t\t----------"); //remove later
        }
    }
}
