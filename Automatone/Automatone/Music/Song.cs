using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone.Music
{
    public class Song
    {
        private List<Verse> song;
        private int measureCount;
        public int MeasureCount { get { return measureCount; } }
        private int measureLength;
        public int MeasureLength { get { return measureLength; } }

        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Song(MusicTheory theory, Random rand)
        {
            //Get instance of InputParameters
            InputParameters inputParameters = InputParameters.Instance;

            //Set music properties
            measureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * inputParameters.TimeSignature);
            NoteName key = new NoteName((byte)rand.Next(MusicTheory.OCTAVE_SIZE));
            MusicTheory.SCALE_MODE mode = (rand.NextDouble() > 0.4 ? MusicTheory.SCALE_MODE.MAJOR : MusicTheory.SCALE_MODE.NATURAL_MINOR);

            //Calculate song length
            int songLength = (int)(inputParameters.MeanSongLength * theory.SONG_LENGTHINESS);
            songLength += (int)(songLength * ((rand.NextDouble() - 0.5) * inputParameters.SongLengthVariance));
            songLength = Math.Max(1, songLength);

            //Generate rhythm
            Rhythm rhythm = new Rhythm(theory, measureLength);

            //Generate melody
            Melody melody = new Melody(theory);
            
            //Generate harmony
            Harmony harmony = new Harmony(theory, rand, key, mode);

            //Generate parts
            List<Part> parts = new List<Part>();
            int partCount = 1 + (int)(inputParameters.Polyphony * theory.PART_COUNT);
            int rhythmNumber = rand.Next(1, partCount);
            int melodyNumber = rand.Next(1, partCount);
            for (int i = 0; i < 1 + inputParameters.Polyphony * theory.PART_COUNT; i++)
            {
                rhythmNumber = (rand.NextDouble() < inputParameters.Homophony ? rhythmNumber : rand.Next(1, partCount));
                melodyNumber = (rand.NextDouble() < inputParameters.Homophony ? melodyNumber : rand.Next(1, partCount));
                parts.Add(new Part(theory, rand, rhythm, rhythmNumber, melody, melodyNumber, measureLength));
            }

            //Generate seeds for rhythm and melody
            double rhythmSeedLength = 1 + inputParameters.MeasureRhythmVariance * (parts.Count);
            rhythmSeedLength += inputParameters.PhraseRhythmVariance * (theory.PHRASE_LENGTHINESS * inputParameters.MeanPhraseLength * rhythmSeedLength);
            rhythmSeedLength += inputParameters.VerseRhythmVariance * (theory.VERSE_LENGTHINESS * inputParameters.MeanVerseLength * rhythmSeedLength);
            rhythmSeedLength += inputParameters.SongRhythmVariance * (songLength * rhythmSeedLength);
            List<int> rhythmSeeds = new List<int>();
            for (int i = 0; i < rhythmSeedLength; i++)
            {
                rhythmSeeds.Add(rand.Next());
            }
            double melodySeedLength = 1 + inputParameters.MeasureMelodyVariance * (parts.Count);
            melodySeedLength += inputParameters.PhraseMelodyVariance * (theory.PHRASE_LENGTHINESS * inputParameters.MeanPhraseLength * melodySeedLength);
            melodySeedLength += inputParameters.VerseMelodyVariance * (theory.VERSE_LENGTHINESS * inputParameters.MeanVerseLength * melodySeedLength);
            melodySeedLength += inputParameters.SongMelodyVariance * (songLength * melodySeedLength);
            List<int> melodySeeds = new List<int>();
            for (int i = 0; i < melodySeedLength; i++)
            {
                melodySeeds.Add(rand.Next());
            }

            //Generate verses
		    List<Verse> verses = new List<Verse>();
		    for(int i = 0; i < 1 + 2 * inputParameters.StructuralVariance * songLength; i++)
            {
                System.Console.Write("Verse " + i); //remove later
                verses.Add(new Verse(theory, rand, parts, harmony, songLength, rhythmSeeds, melodySeeds));
		    }

            System.Console.Write("Final song:"); //remove later
		
            //Select verses to include in song
            List<int> verseArrangement = new List<int>();
            for (int i = 0; i < songLength; i++)
            {
                int choice = rand.Next(verses.Count);
                int curr = verseArrangement.Count;
                while (curr > 0 && rand.NextDouble() < theory.CHORUS_EXISTENCE)
                {
                    if ((verseArrangement.ElementAt<int>(curr - 1) == 0 && verseArrangement.Last<int>() == 0 && choice == 0) || (verseArrangement.ElementAt<int>(curr - 1) != 0 && verseArrangement.Last<int>() != 0 && choice != 0))
                    {
                        choice = rand.Next(verses.Count);
                        curr--;
                    }
                    else
                    {
                        curr = 0;
                    }
                }
                verseArrangement.Add(choice);
            }
            song = new List<Verse>();
            foreach (int choice in verseArrangement)
            {
                song.Add(verses.ElementAt<Verse>(choice));
                System.Console.Write(" " + choice); //remove later
            }

            //Build notes from verses
            notes = new List<List<Note>>();
            measureCount = 0;
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
