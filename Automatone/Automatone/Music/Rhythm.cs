using System.Collections.Generic;
using System.Linq;

namespace Automatone.Music
{
    public class Rhythm
    {
        private int measureLength;
        private double[] rhythmCurve;
        public double[] RhythmCurve
        {
            get
            {
                double[] curve = new double[measureLength];
                for (int i = 0; i < measureLength; i++)
                {
                    curve[i] = rhythmCurve[i % rhythmCurve.Length];
                }
                return curve;
            }
        }

        public Rhythm(MusicTheory theory, int measureLength)
        {
            //Get rhythm curve sample
            List<double> rhythmCurveSample = new List<double>(theory.RHYTHM_CURVE_SAMPLE);

            //Determine curve sizes
            int oldCount = rhythmCurveSample.Count;
            int newCount = (int)(Automatone.SUBBEATS_PER_WHOLE_NOTE * 2 / (double)InputParameters.Instance.timeSignatureD);

            //Create adjusted rhythm curve
            rhythmCurve = new double[newCount];
            for (int i = 0; i < newCount; i++)
            {
                double sampleValue = rhythmCurveSample.ElementAt<double>(i * oldCount / newCount);
                sampleValue += (InputParameters.Instance.rhythmObedience - 1) * (sampleValue - 0.5);
                rhythmCurve[i] = sampleValue;
                rhythmCurveSample.RemoveAt(i * oldCount / newCount);
                rhythmCurveSample.Insert(i * oldCount / newCount, 0.0);
            }

            this.measureLength = measureLength;
        }
    }
}
