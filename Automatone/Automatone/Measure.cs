using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Measure
    {
        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Measure(MusicTheory theory, Random rand, List<Part> parts, List<int> rhythmSeeds, List<int> melodySeeds, List<NoteName> chord, List<NoteName> diatonic)
        {
            notes = new List<List<Note>>();

            foreach (Part prt in parts)
            {
                notes.Add(prt.GenerateNotes(rhythmSeeds.ElementAt<int>(rand.Next(rhythmSeeds.Count)), melodySeeds.ElementAt<int>(rand.Next(melodySeeds.Count)), chord, diatonic));
            }
        }
    }
}
