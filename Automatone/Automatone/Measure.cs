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

        public Measure(MusicTheory theory, Random rand, List<Part> parts, List<double[]> rhythmSeeds, List<double[]> melodySeeds, List<NoteName> chord, List<NoteName> diatonic)
        {
            notes = new List<List<Note>>();

            foreach (Part prt in parts)
            {
                notes.Add(prt.GenerateNotes(rhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count)), melodySeeds.ElementAt<double[]>((int)(rand.NextDouble() * melodySeeds.Count)), chord, diatonic));
            }
        }
    }
}
