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

        public Song(MusicTheory theory, Random rand)
        {
            song = new List<Verse>();

            //Music stuff
            tempo = (ushort)(theory.MIN_TEMPO + ((InputParameters.songSpeed + (InputParameters.songSpeed * ((rand.NextDouble() - 0.5) * InputParameters.songSpeedVariance))) * (theory.MAX_TEMPO - theory.MIN_TEMPO)));
            timeSignatureN = (int)Math.Round(InputParameters.timeSignatureN);
            timeSignatureD = (int)Math.Pow(2, Math.Round(Math.Log(InputParameters.timeSignatureD) / Math.Log(2)));
            timeSignature = (double)timeSignatureN / (double)timeSignatureD;
            measureLength = (int)Math.Round(Automatone.SUBBEATS_PER_WHOLE_NOTE * timeSignature);
            key = new NoteName((byte)rand.Next(12));
            mode = (rand.NextDouble() > 0.5 ? MusicTheory.SCALE_MODE.MAJOR : MusicTheory.SCALE_MODE.NATURAL_MINOR);

            //Calculate song length
            int songLength = (int)(InputParameters.meanSongLength * theory.SONG_LENGTHINESS);
            songLength += (int)(songLength * ((rand.NextDouble() - 0.5) * InputParameters.songLengthVariance));
            songLength = Math.Max(1, songLength);

            //for rhythm and melody variance
            int lengthiness = (int)(songLength * theory.VERSE_LENGTHINESS * InputParameters.meanVerseLength * theory.PHRASE_LENGTHINESS * InputParameters.meanPhraseLength);

            //generate rhythms
            Rhythm rhythm = new Rhythm(theory, timeSignatureN, timeSignatureD, InputParameters.rhythmObedience);
            List<int> rhythmSeeds = new List<int>();
            for (int i = 0; i < 1 + InputParameters.songRhythmVariance * lengthiness; i++)
            {
                rhythmSeeds.Add(rand.Next());
            }

            //generate melodies
            Melody melody = new Melody(theory, InputParameters.chordalityObedience, InputParameters.tonalityObedience, InputParameters.meanPitchContiguity);
            List<int> melodySeeds = new List<int>();
            for (int i = 0; i < 1 + InputParameters.songMelodyVariance * lengthiness; i++)
            {
                melodySeeds.Add(rand.Next());
            }

            //generate harmony
            Harmony harmony = new Harmony(theory, rand, key, mode, InputParameters.seventhChordProbability);

            //generate parts
            List<Part> parts = new List<Part>();
            /*parts.Add(new Part(1, theory, rhythm, melody, measureLength, InputParameters.meanNoteLength, InputParameters.noteLengthVariance, 0.1, 0.5, 0, 36, 2, 1, false, false));
            parts.Add(new Part(1, theory, rhythm, melody, measureLength, InputParameters.meanNoteLength, InputParameters.noteLengthVariance, 0.2, 0.8, 0.4, 12, 1, 3, true, false));*/
            parts.Add(new Part(1, theory, rhythm, melody, measureLength, InputParameters.meanNoteLength, InputParameters.noteLengthVariance, 0.5, 0.5, 0, 48, 2, 1, false, false));
            parts.Add(new Part(2, theory, rhythm, melody, measureLength, InputParameters.meanNoteLength, InputParameters.noteLengthVariance, 0.2, 0.5, 0.1, 36, 1, 1, false, false));
            parts.Add(new Part(1, theory, rhythm, melody, measureLength, InputParameters.meanNoteLength, InputParameters.noteLengthVariance, 0.2, 0.8, 0.5, 24, 1, 3, true, false));
            
            //generate verses
		    List<Verse> verses = new List<Verse>();
		    for(int i = 0; i < 1 + 2 * InputParameters.structuralVariance * songLength; i++)
            {
                System.Console.Write("Verse " + i); //remove later
			    verses.Add(new Verse(theory, rand, parts, harmony, songLength, rhythmSeeds, melodySeeds,
                    InputParameters.meanPhraseLength, InputParameters.phraseLengthVariance, InputParameters.phraseRhythmVariance, InputParameters.phraseMelodyVariance,
                    InputParameters.meanVerseLength, InputParameters.verseLengthVariance, InputParameters.verseRhythmVariance, InputParameters.verseMelodyVariance));
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
