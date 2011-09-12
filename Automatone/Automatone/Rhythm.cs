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
            int r = (int)(Automatone.SUBBEATS_PER_WHOLE_NOTE * 2 / Math.Round(InputParameters.timeSignatureD / 4.0) / 4.0);
            int lim = (Math.Round(InputParameters.timeSignatureN) > 1 ? r : (int)(r * Math.Round(InputParameters.timeSignatureN) / 2));
            for (int i = 0; i < lim ; i++)
            {
                int j = (l * i) / r;
                double c = ((1 - ((double)i / r) + ((double)j / l)) * (rhythmCurveSample.ElementAt<double>(j % l))) + ((((double)i / r) - ((double)j / l)) * (rhythmCurveSample.ElementAt<double>((j + 1) % l)));
                c = c + (InputParameters.rhythmObedience - 1) * (c - 0.5);
                rhythmCurve.Add(c);
            }
            for (int i = 2; i < Math.Round(InputParameters.timeSignatureN); i++)
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
