using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone.Music
{
    public class Verse
    {
        private List<Phrase> verse;

        private int verseLength;
        public int VerseLength { get { return verseLength; } }
        private int measureCount;
        public int MeasureCount { get { return measureCount; } }

        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Verse(MusicTheory theory, Random rand, List<Part> parts, Harmony harmony, int songLength, List<int> rhythmSeeds, List<int> melodySeeds)
        {
            //Get instance of InputParameters
            InputParameters inputParameters = InputParameters.Instance;

            //Calculate verse length
            verseLength = (int)(theory.VERSE_LENGTHINESS * inputParameters.MeanVerseLength);
            verseLength += (int)(verseLength * ((rand.NextDouble() - 0.5) * inputParameters.VerseLengthVariance));
            verseLength = Math.Max(1, verseLength);

            System.Console.WriteLine(" length " + verseLength); //remove later

            //Select seeds for rhythm and melody
            double rhythmSeedLength = 1 + inputParameters.MeasureRhythmVariance * (parts.Count);
            rhythmSeedLength += inputParameters.PhraseRhythmVariance * (theory.PHRASE_LENGTHINESS * inputParameters.MeanPhraseLength * rhythmSeedLength);
            rhythmSeedLength += inputParameters.VerseRhythmVariance * (verseLength * rhythmSeedLength);
            List<int> selectedRhythmSeeds = new List<int>();
            for (int i = 0; i < rhythmSeedLength; i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<int>(rand.Next(rhythmSeeds.Count)));
            }
            double melodySeedLength = 1 + inputParameters.MeasureMelodyVariance * (parts.Count);
            melodySeedLength += inputParameters.PhraseMelodyVariance * (theory.PHRASE_LENGTHINESS * inputParameters.MeanPhraseLength * melodySeedLength);
            melodySeedLength += inputParameters.VerseMelodyVariance * (verseLength * melodySeedLength);
            List<int> selectedMelodySeeds = new List<int>();
            for (int i = 0; i < melodySeedLength; i++)
            {
                selectedMelodySeeds.Add(melodySeeds.ElementAt<int>(rand.Next(melodySeeds.Count)));
            }

            //Build cadence curve
		    List<double> cadenceFractalCurve = Enumerable.Repeat<double>(1.0, verseLength).ToList<double>();
		    int x = verseLength;
		    for(int i = 2; i <= Math.Sqrt(verseLength); i++)
		    {
			    while(x % i == 0)
			    {
				    for(int j = 0; j < verseLength; j++)
				    {
					    if((j + 1) % x != 0)
					    {
						    cadenceFractalCurve[j] *= theory.CADENCE_SMOOTHNESS;
					    }
				    }
				    x /= i;
			    }
		    }

            //Create phrases
            verse = new List<Phrase>();
		    for(int i = 0; i < verseLength; i++)
		    {
                System.Console.Write("\tPhrase " + i); //remove later
			    int aboveCurve = 0;
			    if(rand.NextDouble() > cadenceFractalCurve[i])
			    {
				    aboveCurve = 1;
			    }
			    double sum = 0;
			    foreach(double j in MusicTheory.CADENCES[aboveCurve])
			    {
				    sum += j;
			    }
			    double r = rand.NextDouble() * sum;
                bool addDefaultPhrase = true;
                for (int j = 0; j < 4; j++)
                {
                    if (r < MusicTheory.CADENCES[aboveCurve][j])
                    {
                        verse.Add(new Phrase(theory, rand, (MusicTheory.CADENCE_NAMES)j, parts, harmony, verseLength, selectedRhythmSeeds, selectedMelodySeeds));
                        addDefaultPhrase = false;
                        break;
                    }
                    else
                    {
                        r -= MusicTheory.CADENCES[aboveCurve][j];
                    }
                }
                if (addDefaultPhrase)
			    {
                    verse.Add(new Phrase(theory, rand, MusicTheory.CADENCE_NAMES.SILENT, parts, harmony, verseLength, selectedRhythmSeeds, selectedMelodySeeds));
			    }
            }

            //Build notes from phrases
            measureCount = 0;
            notes = new List<List<Note>>();
            for (int i = 0; i < verse.Count; i++)
            {
                for (int j = 0; j < verse.ElementAt<Phrase>(i).Notes.Count; j++ )
                {
                    if (i == 0)
                    {
                        notes.Add(new List<Note>());
                    }
                    foreach (Note n in verse.ElementAt<Phrase>(i).Notes.ElementAt<List<Note>>(j))
                    {
                        Note n2 = new Note(n.GetNoteName(), n.GetOctave(), n.GetRemainingDuration(), n.GetStartMeasure() + measureCount, n.GetStartBeat());
                        notes.ElementAt<List<Note>>(j).Add(n2);
                    }
                }
                measureCount += verse.ElementAt<Phrase>(i).MeasureCount;
            }
        }
    }
}
