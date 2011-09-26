using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Song
    {
        private List<Verse> song;
        private int measureCount;
        public int MeasureCount { get { return measureCount; } }

        private ushort tempo;
        public ushort Tempo { get { return tempo; } }
        private int timeSignatureN;
        public int TimeSignatureN { get { return timeSignatureN; } }
        private int timeSignatureD;
        public int TimeSignatureD { get { return timeSignatureD; } }
        private double timeSignature;
        public double TimeSignature { get { return timeSignature; } }
        private int measureLength;
        public int MeasureLength { get { return measureLength; } }
        private NoteName key;
        public NoteName Key { get { return key; } }
        private MusicTheory.SCALE_MODE mode;
        public MusicTheory.SCALE_MODE Mode { get { return mode; } }

        private List<List<Note>> notes;
        public List<List<Note>> Notes { get { return notes; } }

        public Song(MusicTheory theory, InputParameters inputParameters, Random rand)
        {
            song = new List<Verse>();

            //Music stuff
            tempo = (ushort)(theory.MIN_TEMPO + ((inputParameters.songSpeed + (inputParameters.songSpeed * ((rand.NextDouble() - 0.5) * inputParameters.songSpeedVariance))) * (theory.MAX_TEMPO - theory.MIN_TEMPO)));
            timeSignatureN = (int)Math.Round(inputParameters.timeSignatureN);
            timeSignatureD = (int)Math.Pow(2, Math.Round(Math.Log(inputParameters.timeSignatureD) / Math.Log(2)));
            timeSignature = (double)timeSignatureN / (double)timeSignatureD;
            measureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * timeSignature);
            key = new NoteName((byte)rand.Next(12));
            mode = (rand.NextDouble() > 0.4 ? MusicTheory.SCALE_MODE.MAJOR : MusicTheory.SCALE_MODE.NATURAL_MINOR);

            //Calculate song length
            int songLength = (int)(inputParameters.meanSongLength * theory.SONG_LENGTHINESS);
            songLength += (int)(songLength * ((rand.NextDouble() - 0.5) * inputParameters.songLengthVariance));
            songLength = Math.Max(1, songLength);

            //generate rhythm
            Rhythm rhythm = new Rhythm(theory, timeSignatureN, timeSignatureD, inputParameters.rhythmObedience);

            //generate melody
            Melody melody = new Melody(theory, inputParameters.chordalityObedience, inputParameters.tonalityObedience, inputParameters.meanPitchContiguity);
            
            //generate harmony
            Harmony harmony = new Harmony(theory, rand, key, mode, inputParameters.seventhChordProbability);

            //generate parts
            List<Part> parts = new List<Part>();

            //Random part creation
            int partCount = 1 + (int)(inputParameters.polyphony * theory.PART_COUNT);
            int rhythmNumber = rand.Next(1, partCount);
            int melodyNumber = rand.Next(1, partCount);
            for (int i = 0; i < 1 + inputParameters.polyphony * theory.PART_COUNT; i++)
            {
                rhythmNumber = (rand.NextDouble() < inputParameters.homophony ? rhythmNumber : rand.Next(1, partCount));
                melodyNumber = (rand.NextDouble() < inputParameters.homophony ? melodyNumber : rand.Next(1, partCount));
                parts.Add(new Part(theory, rand, inputParameters, rhythm, rhythmNumber, melody, melodyNumber, measureLength));
            }

            //Manual part creation
            /*
            parts.Add(new Part(theory, rand, inputParameters, rhythm, 1, melody, 1, measureLength, 0.5, 0.5, 0, 48, 2, false, false));
            parts.Add(new Part(theory, rand, inputParameters, rhythm, 2, melody, 1, measureLength, 0.5, 0.5, 0, 36, 2, false, false));
            parts.Add(new Part(theory, rand, inputParameters, rhythm, 1, melody, 1, measureLength, 0.5, 0.9, 0.5, 12, 2, true, false));
            parts.Add(new Part(theory, rand, inputParameters, rhythm, 2, melody, 2, measureLength, 0.5, 0.9, 0.5, 12, 2, true, false));
            parts.Add(new Part(theory, rand, inputParameters, rhythm, 3, melody, 3, measureLength, 0.5, 0.9, 0.5, 12, 2, true, false));
            */

            //generate seeds
            int lengthiness = (int)(songLength * theory.VERSE_LENGTHINESS * inputParameters.meanVerseLength * theory.PHRASE_LENGTHINESS * inputParameters.meanPhraseLength * parts.Count);
            List<int> rhythmSeeds = new List<int>();
            for (int i = 0; i < 1 + inputParameters.songRhythmVariance * lengthiness; i++)
            {
                rhythmSeeds.Add(rand.Next());
            }
            List<int> melodySeeds = new List<int>();
            for (int i = 0; i < 1 + inputParameters.songMelodyVariance * lengthiness; i++)
            {
                melodySeeds.Add(rand.Next());
            }
            
            //generate verses
		    List<Verse> verses = new List<Verse>();
		    for(int i = 0; i < 1 + 2 * inputParameters.structuralVariance * songLength; i++)
            {
                System.Console.Write("Verse " + i); //remove later
                verses.Add(new Verse(theory, inputParameters, rand, parts, harmony, songLength, rhythmSeeds, melodySeeds));
		    }

            System.Console.Write("Final song:"); //remove later
		
            //select verses to include in song
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
            foreach (int choice in verseArrangement)
            {
                song.Add(verses.ElementAt<Verse>(choice));
                System.Console.Write(" " + choice); //remove later
            }

            //make note list
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
