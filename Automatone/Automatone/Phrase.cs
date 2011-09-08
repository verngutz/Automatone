using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Phrase
    {
        private List<Measure> phrase;

        private int phraseLength;
        public int PhraseLength { get { return phraseLength; } }
        private int measureCount;
        public int MeasureCount { get { return measureCount; } }

        private CellState[,] grid;
        public CellState[,] Grid { get { return grid; } }
        private List<Note> notes;
        public List<Note> Notes { get { return notes; } }

        public Phrase(MusicTheory theory, Random rand, MusicTheory.CADENCE_NAMES c, Rhythm rhythm, List<double[]> rhythmSeeds)
        {
            phrase = new List<Measure>();

            //Calculate phrase length
            phraseLength = (int)(theory.PHRASE_LENGTHINESS * InputParameters.meanPhraseLength);
            phraseLength += (int)(phraseLength * ((rand.NextDouble() - 0.5) * InputParameters.phraseLengthVariance));
            measureCount = phraseLength;

            System.Console.WriteLine(" length " + phraseLength); //remove later

            //Select rhythms
            List<double[]> selectedRhythmSeeds = new List<double[]>();
            for (int i = 0; i < 1 + 2 * InputParameters.phraseRhythmVariety * phraseLength; i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count)));
            }

            Harmony harm = new Harmony(theory, rand);
            harm.initializeHarmony(phraseLength * Automatone.SUBBEATS_PER_MEASURE);
            List<List<NoteName>> progression = harm.chordProgression;

            for (int i = 0; i < phraseLength; i++)
            {
                int chordNumber = (i * progression.Count / phraseLength);
                List<NoteName> chord = progression.ElementAt<List<NoteName>>(chordNumber);

                phrase.Add(new Measure(theory, rand, rhythm, rhythmSeeds.ElementAt<double[]>((int)(rand.NextDouble() * rhythmSeeds.Count)), chord));
            }

            notes = new List<Note>();
            for (int i = 0; i < phrase.Count; i++)
            {
                foreach (Note n in phrase.ElementAt<Measure>(i).Notes)
                {
                    Note n2 = new Note(n.GetNoteName(), n.GetOctave(), n.GetRemainingDuration(), n.GetStartMeasure() + i, n.GetStartBeat());
                    notes.Add(n2);
                }
            }

            //Build grid
            int gridSize = 0;
            foreach (Measure m in phrase)
            {
                gridSize += m.Grid.GetLength(1);
            }
            grid = new CellState[theory.PIANO_SIZE, gridSize];
            int gridStartPosition = 0;
            foreach (Measure m in phrase)
            {
                for (int i = 0; i < m.Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < m.Grid.GetLength(1); j++)
                    {
                        grid[i, j + gridStartPosition] = m.Grid[i, j];
                    }
                }
                gridStartPosition += m.Grid.GetLength(1);
            }
        }
    }
}
