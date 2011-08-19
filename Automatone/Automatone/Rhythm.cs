using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Rhythm
    {
        private List<double> rhythmCurve;
        public List<double> RhythmCurve { get { return rhythmCurve; } }

        public Rhythm(double seed, MusicTheory musicTheory)
        {
            rhythmCurve = new List<double>();
            List<double> rhythmCurveSample = musicTheory.RHYTHM_CURVE_SAMPLE;
            int l = rhythmCurveSample.Count;
            int r = musicTheory.SUBBEATS_PER_MEASURE;
            for (int i = 0; i < r; i++)
            {
                int j = (l * i) / r;
                double c = ((1 - ((double)i / r) + ((double)j / l)) * (rhythmCurveSample.ElementAt<double>(j % r))) + ((((double)i / r) - ((double)j / l)) * (rhythmCurveSample.ElementAt<double>((j + 1) % r)));
                c = c + (InputParameters.rhythmObedience - 1) * (c - 0.5);
                rhythmCurve.Add(c);
            }
        }
    }
}
