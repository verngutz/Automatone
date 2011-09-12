using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Measure
    {
        private int measureLength;
        public int MeasureLength { get { return measureLength; } }

        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Measure(MusicTheory theory, Random rand, List<Part> parts, List<double[]> rhythmSeeds, List<double[]> melodySeeds, List<NoteName> chord, List<NoteName> diatonic)
        {
            measureLength = (int)(Automatone.SUBBEATS_PER_WHOLE_NOTE * (Math.Round(InputParameters.timeSignatureN) / Math.Round(InputParameters.timeSignatureD / 4.0) / 4.0));
            System.Console.WriteLine("ml " + measureLength);
            notes = new List<List<Note>>();

            foreach (Part prt in parts)
            {
                notes.Add(prt.GenerateNotes(rhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count)), melodySeeds.ElementAt<double[]>((int)(rand.NextDouble() * melodySeeds.Count)), chord, diatonic, measureLength));
            }
        }
    }
}
