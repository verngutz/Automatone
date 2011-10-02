using System.Collections.Generic;
using System.Linq;

namespace Automatone
{
    public class Rhythm
    {
        private List<double> rhythmCurve;

        public Rhythm(MusicTheory theory)
        {
            rhythmCurve = new List<double>();
            List<double> rhythmCurveSample = new List<double>(theory.RHYTHM_CURVE_SAMPLE);

            int oldCount = rhythmCurveSample.Count;
            int newCount = (int)(Automatone.SUBBEATS_PER_WHOLE_NOTE * 2 / (double)InputParameters.Instance.timeSignatureD);

            for (int i = 0; i < newCount; i++)
            {
                double sampleValue = rhythmCurveSample.ElementAt<double>(i * oldCount / newCount);
                sampleValue += (InputParameters.Instance.rhythmObedience - 1) * (sampleValue - 0.5);
                rhythmCurve.Add(sampleValue);
                rhythmCurveSample.RemoveAt(i * oldCount / newCount);
                rhythmCurveSample.Insert(i * oldCount / newCount, 0.0);
            }
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
