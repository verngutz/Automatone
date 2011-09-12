using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Song
    {
        private List<Verse> song;
        private int measureCount;
        public int MeasureCount { get { return measureCount; } }

        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Song(MusicTheory theory, Random rand)
        {
            song = new List<Verse>();

            //Calculate verse length
            int songLength = (int)(InputParameters.songLength * theory.SONG_LENGTHINESS);
            songLength += (int)(songLength * ((rand.NextDouble() - 0.5) * InputParameters.songLengthVariance));
            measureCount = 0;

            //generate rhythms
            Rhythm rhythm = new Rhythm(theory);
            List<double[]> rhythmSeeds = new List<double[]>();
            for (int i = 0; i < 1 + 2 * InputParameters.songRhythmVariety * songLength; i++)
            {
                rhythmSeeds.Add(new double[] { rand.NextDouble(), rand.NextDouble() });
            }
            //generate melodies
            Melody melody = new Melody(theory);
            List<double[]> melodySeeds = new List<double[]>();
            for (int i = 0; i < 1 + 2 * InputParameters.songMelodyVariety * songLength; i++)
            {
                melodySeeds.Add(new double[] { rand.NextDouble(), rand.NextDouble() });
            }

            //create harmony
            Harmony harmony = new Harmony(theory, rand, new NoteName((byte)rand.Next(12)), (rand.NextDouble() > 0.5 ? MusicTheory.SCALE_MODE.MAJOR : MusicTheory.SCALE_MODE.HARMONIC_MINOR));

            //generate parts
            List<Part> parts = new List<Part>();
            parts.Add(new Part(theory, rhythm, melody, 0.5, 0.5, 0, 45, 12, 1, false, false));
            parts.Add(new Part(theory, rhythm, melody, 0.1, 0.5, 0.3, 33, 12, 1, false, false));
            parts.Add(new Part(theory, rhythm, melody, 0.2, 1, 0.6, 20, 12, 3, true, false));

            //generate verses
		    List<Verse> verses = new List<Verse>();
		    for(int i = 0; i < 1 + 2 * InputParameters.structuralVar * songLength; i++)
            {
                System.Console.Write("Verse " + i); //remove later
			    verses.Add(new Verse(theory, rand, parts, harmony, rhythmSeeds, melodySeeds));
		    }

            System.Console.Write("Final song:"); //remove later
		
            //select verses to include in song
		    double chorusProb = rand.NextDouble();
            int? prev = null;
		    for(int i = 0; i < songLength; i++)
		    {
			    int curr = (int)(rand.NextDouble() * verses.Count);
                while (chorusProb < theory.CHORUS_EXISTENCE * songLength)
			    {
				    if (prev != null && ((prev == 0  && curr == 0) || prev != 0 && curr != 0))
				    {
                        curr = (int)(rand.NextDouble() * verses.Count);
				    }
                    chorusProb += chorusProb;
			    }
			    song.Add(verses.ElementAt<Verse>(curr));
                System.Console.Write(" " + curr); //remove later
                prev = curr;
            }

            //make note list
            notes = new List<List<Note>>();
            for (int i = 0; i < song.Count; i++)
            {
                for (int j = 0; j < song.ElementAt<Verse>(i).Notes.Count; j++)
                {
                    if (i == 0)
                    {
                        notes.Add(new List<Note>());
                    }
                    foreach (Note n in song.ElementAt<Verse>(i).Notes.ElementAt<List<Note>>(j))
                    {
                        Note n2 = new Note(n.GetNoteName(), n.GetOctave(), n.GetRemainingDuration(), n.GetStartMeasure() + measureCount, n.GetStartBeat());
                        notes.ElementAt<List<Note>>(j).Add(n2);
                    }
                }
                measureCount += song.ElementAt<Verse>(i).MeasureCount;
            }
        }
    }
}
