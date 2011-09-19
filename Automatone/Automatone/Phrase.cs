using System;
using System.Collections.Generic;
using System.Linq;

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

        public Phrase(MusicTheory theory, Random rand, MusicTheory.CADENCE_NAMES c, List<Part> parts, Harmony harmony, int verseLength, List<int> rhythmSeeds, List<int> melodySeeds, double meanPhraseLength, double phraseLengthVariance, double phraseRhythmVariance, double phraseMelodyVariance)
        {
            phrase = new List<Measure>();

            //Calculate phrase length
            phraseLength = (int)(theory.PHRASE_LENGTHINESS * meanPhraseLength);
            phraseLength += (int)(phraseLength * ((rand.NextDouble() - 0.5) * phraseLengthVariance));
            phraseLength = Math.Max(1, phraseLength);
            measureCount = phraseLength;

            System.Console.WriteLine(" length " + phraseLength); //remove later

            //Select rhythms
            List<int> selectedRhythmSeeds = new List<int>();
            for (int i = 0; i < 1 + phraseRhythmVariance * (rhythmSeeds.Count / verseLength); i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<int>(rand.Next(rhythmSeeds.Count)));
            }
            //Select melodies
            List<int> selectedMelodySeeds = new List<int>();
            for (int i = 0; i < 1 + phraseMelodyVariance * (melodySeeds.Count / verseLength); i++)
            {
                selectedMelodySeeds.Add(melodySeeds.ElementAt<int>(rand.Next(melodySeeds.Count)));
            }

            List<List<NoteName>> progression = harmony.createChordProgression(phraseLength, c);

            List<NoteName> diatonic = harmony.GetDiatonicScale();

            for (int i = 0; i < phraseLength; i++)
            {
                phrase.Add(new Measure(theory, rand, parts, selectedRhythmSeeds, selectedMelodySeeds, progression.ElementAt<List<NoteName>>(i), diatonic));
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
