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

        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Phrase(MusicTheory theory, Random rand, MusicTheory.CADENCE_NAMES c, List<Part> parts, List<double[]> rhythmSeeds, List<double[]> melodySeeds)
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
            //Select melodies
            List<double[]> selectedMelodySeeds = new List<double[]>();
            for (int i = 0; i < 1 + 2 * InputParameters.phraseMelodyVariety * phraseLength; i++)
            {
                selectedMelodySeeds.Add(melodySeeds.ElementAt<double[]>((int)(rand.NextDouble() * melodySeeds.Count)));
            }

            Harmony harm = new Harmony(theory, rand);
            harm.initializeHarmony(phraseLength * Automatone.SUBBEATS_PER_MEASURE);
            List<List<NoteName>> progression = harm.chordProgression;

            List<NoteName> diatonic = new List<NoteName>();

            for (int i = 0; i < phraseLength; i++)
            {
                int chordNumber = (i * progression.Count / phraseLength);
                List<NoteName> chord = progression.ElementAt<List<NoteName>>(chordNumber);

                phrase.Add(new Measure(theory, rand, parts, selectedRhythmSeeds, selectedMelodySeeds, chord, diatonic));
            }

            notes = new List<List<Note>>();
            for (int i = 0; i < phrase.Count; i++)
            {
                for (int j = 0; j < phrase.ElementAt<Measure>(i).Notes.Count; j++ )
                {
                    if (i == 0)
                    {
                        notes.Add(new List<Note>());
                    }
                    foreach (Note n in phrase.ElementAt<Measure>(i).Notes.ElementAt<List<Note>>(j))
                    {
                        Note n2 = new Note(n.GetNoteName(), n.GetOctave(), n.GetRemainingDuration(), n.GetStartMeasure() + i, n.GetStartBeat());
                        notes.ElementAt<List<Note>>(j).Add(n2);
                    }
                }
            }
        }
    }
}
