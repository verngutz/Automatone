using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Rhythm
    {
        private MusicTheory theory;
        private List<double> rhythmCurve;

        public Rhythm(MusicTheory musicTheory)
        {
            rhythmCurve = new List<double>();
            List<double> rhythmCurveSample = musicTheory.RHYTHM_CURVE_SAMPLE;
            int l = rhythmCurveSample.Count;
            int r = Automatone.SUBBEATS_PER_MEASURE;
            for (int i = 0; i < r; i++)
            {
                int j = (l * i) / r;
                double c = ((1 - ((double)i / r) + ((double)j / l)) * (rhythmCurveSample.ElementAt<double>(j % l))) + ((((double)i / r) - ((double)j / l)) * (rhythmCurveSample.ElementAt<double>((j + 1) % l)));
                c = c + (InputParameters.rhythmObedience - 1) * (c - 0.5);
                rhythmCurve.Add(c);
            }
            theory = musicTheory;
        }

        public double[] GetRhythmCurve(int measureLength)
        {
            double[] curve = new double[measureLength];
            for (int i = 0; i < measureLength; i++)
            {
                curve[i] = rhythmCurve.ElementAt<double>(i % Automatone.SUBBEATS_PER_MEASURE);
            }
            return curve;
        }


    }
}
