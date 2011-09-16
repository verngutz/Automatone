using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Automatone
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

        public Verse(MusicTheory theory, Random rand, List<Part> parts, Harmony harmony, int songLength, List<int> rhythmSeeds, List<int> melodySeeds, double meanPhraseLength, double phraseLengthVariance, double phraseRhythmVariance, double phraseMelodyVariance, double meanVerseLength, double verseLengthVariance, double verseRhythmVariance, double verseMelodyVariance)
        {
            //Calculate verse length
            verseLength = (int)(theory.VERSE_LENGTHINESS * meanVerseLength);
            verseLength += (int)(verseLength * ((rand.NextDouble() - 0.5) * verseLengthVariance));
            verseLength = Math.Max(1, verseLength);

            System.Console.WriteLine(" length " + verseLength); //remove later

            //Select rhythms
            List<int> selectedRhythmSeeds = new List<int>();
            for (int i = 0; i < 1 + verseRhythmVariance * (rhythmSeeds.Count / songLength); i++)
            {
                selectedRhythmSeeds.Add(rhythmSeeds.ElementAt<int>(rand.Next(rhythmSeeds.Count)));
            }

            //Select melodies
            List<int> selectedMelodySeeds = new List<int>();
            for (int i = 0; i < 1 + verseMelodyVariance * (melodySeeds.Count / songLength); i++)
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
			    int a = 0;
			    if(rand.NextDouble() > cadenceFractalCurve[i])
			    {
				    a = 1;
			    }
			    double sum = 0;
			    foreach(double j in MusicTheory.CADENCES[a])
			    {
				    sum += j;
			    }
			    double r = rand.NextDouble() * sum;
                bool addDefaultPhrase = true;
			    for(int j = 0; j < 4; j++)
			    {
				    if(r < MusicTheory.CADENCES[a][j])
				    {
                        verse.Add(new Phrase(theory, rand, (MusicTheory.CADENCE_NAMES)j, parts, harmony, verseLength, selectedRhythmSeeds, selectedMelodySeeds,
                            meanPhraseLength, phraseLengthVariance, phraseRhythmVariance, phraseMelodyVariance));
                        addDefaultPhrase = false;
					    break;
				    }
				    else
				    {
					    r -= MusicTheory.CADENCES[a][j];
				    }
			    }
			    if(addDefaultPhrase)
			    {
                    verse.Add(new Phrase(theory, rand, MusicTheory.CADENCE_NAMES.SILENT, parts, harmony, verseLength, selectedRhythmSeeds, selectedMelodySeeds,
                        meanPhraseLength, phraseLengthVariance, phraseRhythmVariance, phraseMelodyVariance));
			    }
            }

            //make note lists
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
