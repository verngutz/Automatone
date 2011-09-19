using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Rhythm
    {
        private MusicTheory theory;
        private List<double> rhythmCurve;

        public Rhythm(MusicTheory musicTheory, int timeSignatureN, int timeSignatureD, double rhythmObedience)
        {
            rhythmCurve = new List<double>();
            List<double> rhythmCurveSample = musicTheory.RHYTHM_CURVE_SAMPLE;
            int l = rhythmCurveSample.Count;
            int r = (int)(Automatone.SUBBEATS_PER_WHOLE_NOTE * 2 / (double)timeSignatureD);
            int lim = (timeSignatureN > 1 ? r : r * timeSignatureN / 2);
            for (int i = 0; i < lim ; i++)
            {
                int j = (l * i) / r;
                double c = ((1 - ((double)i / r) + ((double)j / l)) * (rhythmCurveSample.ElementAt<double>(j % l))) + ((((double)i / r) - ((double)j / l)) * (rhythmCurveSample.ElementAt<double>((j + 1) % l)));
                c = c + (rhythmObedience - 1) * (c - 0.5);
                rhythmCurve.Add(c);
            }
            for (int i = 2; i < timeSignatureN; i++)
            {
                for (int j = r / 2; j < r; j++)
                {
                    rhythmCurve.Add(rhythmCurve.ElementAt<double>(j));
                }
            }
            theory = musicTheory;
        }

        public double[] GetRhythmCurve(int measureLength)
        {
            double[] curve = new double[measureLength];
            for (int i = 0; i < measureLength; i++)
            {
                curve[i] = rhythmCurve.ElementAt<double>(i % rhythmCurve.Count);
            }
            return curve;
        }
    }
}
