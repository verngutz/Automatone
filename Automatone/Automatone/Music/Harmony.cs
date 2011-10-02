using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone.Music
{
    public class Harmony
    {
        private Random random;

        private NoteName key;
        private MusicTheory.SCALE_MODE mode;

        private double seventhChordProbability;

        private List<NoteName> diatonicScale;
        public List<NoteName> DiatonicScale { get { return diatonicScale; } }

	    public Harmony (MusicTheory theory, Random random, NoteName key, MusicTheory.SCALE_MODE mode) 
        {
            this.random = random;
            this.key = key;
            this.mode = mode;
            this.seventhChordProbability = InputParameters.Instance.seventhChordProbability;
            diatonicScale = createDiatonicScale();
        }
	
	    private List<NoteName> createDiatonicScale()
	    {
		    List<NoteName> scale = new List<NoteName>();

            //Add white note
		    scale.Add(key);

            //Get note value of key
            int noteVal;
            MusicTheory.CHROMATIC_EQUIVALENTS.TryGetValue(key, out noteVal);

            //Create array of intervals
            int[] intervals;
            MusicTheory.SCALE_INTERVALS.TryGetValue(mode,out intervals);
            for (int i = 0; i < intervals.Length; i++)
            {
                noteVal += intervals[i];
                scale.Add(MusicTheory.CHROMATIC_SCALE[noteVal % MusicTheory.CHROMATIC_SCALE.Length]);
            }
		    return scale;
	    }
	
	    private List<NoteName> createChord(NoteName root, int[] otherNotes)
	    {
            List<NoteName> chord = new List<NoteName>();

            //Add root note
            chord.Add(root);
            
            //Add other notes
            for (int i = 1; i < MusicTheory.OCTAVE_SIZE; i++)
            {
                if (otherNotes.Contains<int>(i))
                {
                    chord.Add(diatonicScale[(diatonicScale.IndexOf(root) + i) % diatonicScale.Count]);
                }
            }
            return chord;
	    }

        public List<List<NoteName>> createChordProgression(int phraseLength, MusicTheory.CADENCE_NAMES cadence)
        {
            List<int> intervals = new List<int>();

            //Get circle progression
            int[] circle;
            MusicTheory.CIRCLE_PROGRESSION.TryGetValue(mode, out circle);

            //Select last chords of progression based on cadence
            switch (cadence)
            {
                case MusicTheory.CADENCE_NAMES.HALF:
                    intervals.Add(circle[random.Next(circle.Length)]);
                    intervals.Add(4);
                    break;
                case MusicTheory.CADENCE_NAMES.PERFECT_AUTHENTIC:
                    intervals.Add(4);
                    intervals.Add(0);
                    break;
                case MusicTheory.CADENCE_NAMES.IMPERFECT_AUTHENTIC:
                    intervals.Add(6);
                    intervals.Add(0);
                    break;
                case MusicTheory.CADENCE_NAMES.PLAGAL:
                    intervals.Add(3);
                    intervals.Add(0);
                    break;
                case MusicTheory.CADENCE_NAMES.DECEPTIVE:
                    intervals.Add(4);
                    intervals.Add(circle[random.Next(1, circle.Length)]);
                    break;
                default:
                    intervals.Add(0);
                    break;
            }

            //Add chords to progression to fill phrase
            while (intervals.Count < phraseLength)
            {
                intervals.Insert(0, circle[(circle.ToList<int>().IndexOf(intervals.First<int>()) + circle.Length - (random.NextDouble() < 0.1 ? 0 : 1)) % circle.Length]);
            }

            //Trim progression to fit phrase
            while (intervals.Count > phraseLength)
            {
                intervals.RemoveAt(0);
            }

            //Build chord progression
            List<List<NoteName>> progression = new List<List<NoteName>>();
            foreach (int i in intervals)
            {
                progression.Add(createChord(diatonicScale.ElementAt<NoteName>(i), (random.NextDouble() > seventhChordProbability ? new int[] { 2, 4 } : new int[] { 2, 4, 6 })));
            }

            return progression;
        }
    }
}
