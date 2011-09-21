using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Rhythm
    {
        private MusicTheory theory;
        private List<double> rhythmCurve;

        public Rhythm(MusicTheory theory, int timeSignatureN, int timeSignatureD, double rhythmObedience)
        {
            rhythmCurve = new List<double>();
            List<double> rhythmCurveSample = new List<double>(theory.RHYTHM_CURVE_SAMPLE);

            int l = rhythmCurveSample.Count;
            int r = (int)(Automatone.SUBBEATS_PER_WHOLE_NOTE * 2 / (double)timeSignatureD);
            int lim = (timeSignatureN > 1 ? r : r * timeSignatureN / 2);

            for (int i = 0; i < lim; i++)
            {
                rhythmCurve.Add(rhythmCurveSample.ElementAt<double>(i*l/r));
                rhythmCurveSample.RemoveAt(i * l / r);
                rhythmCurveSample.Insert(i * l / r, 0.0);
            }

            this.theory = theory;
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
