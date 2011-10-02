using System;
using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Harmony
    {
        private Random random;

        private NoteName key;
        private MusicTheory.SCALE_MODE mode;

        private double seventhChordProbability;

        private List<NoteName> diatonicScale;

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
		    scale.Add(key);
            int noteVal;
            MusicTheory.CHROMATIC_EQUIVALENTS.TryGetValue(key, out noteVal);
            int[] intervals;
            MusicTheory.SCALE_INTERVALS.TryGetValue(mode,out intervals);
            for (int i = 0; i < intervals.Length; i++)
            {
                noteVal += intervals[i];
                scale.Add(MusicTheory.CHROMATIC_SCALE[noteVal % MusicTheory.CHROMATIC_SCALE.Length]);
            }
		    return scale;
	    }
	
	    private List<NoteName> createChord(NoteName root, int[] intervals)
	    {
		    List<NoteName> chord = new List<NoteName>();
		    chord.Add(root);
            for (int i = 1; i < 12; i++)
            {
                if (intervals.Contains<int>(i))
                {
                    chord.Add(diatonicScale[(diatonicScale.IndexOf(root) + i) % diatonicScale.Count]);
                }
            }
            return chord;
	    }

        public List<List<NoteName>> createChordProgression(int phraseLength, MusicTheory.CADENCE_NAMES cadence)
        {
            List<int> intervals = new List<int>();

            int[] circle;
            MusicTheory.CIRCLE_PROGRESSION.TryGetValue(mode, out circle);

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
            while (intervals.Count < phraseLength)
            {
                intervals.Insert(0, circle[(circle.ToList<int>().IndexOf(intervals.First<int>()) + circle.Length - (random.NextDouble() < 0.1 ? 0 : 1)) % circle.Length]);
            }
            while (intervals.Count > phraseLength)
            {
                intervals.RemoveAt(0);
            }

            List<List<NoteName>> progression = new List<List<NoteName>>();
            foreach (int i in intervals)
            {
                progression.Add(createChord(diatonicScale.ElementAt<NoteName>(i), (random.NextDouble() > seventhChordProbability ? new int[] { 2, 4 } : new int[] { 2, 4, 6 })));
            }

            return progression;
        }

        public List<NoteName> GetDiatonicScale()
        {
            return diatonicScale;
        }
    }
}
