using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Melody
    {
        private double[] melodyBias;
        private MusicTheory theory;

        public Melody(MusicTheory musicTheory, double chordalityObedience, double tonalityObedience)
        {
            melodyBias = new double[2];
            double[] melodyBiasSample = musicTheory.MELODY_BIAS_SAMPLE;
            melodyBias[0] = melodyBiasSample[0] + (chordalityObedience - 1) * (melodyBiasSample[0] - 0.5);
            melodyBias[1] = melodyBiasSample[1] + (tonalityObedience - 1) * (melodyBiasSample[1] - 0.5);
        }

        public double[] GetMelodyBias()
        {
            return melodyBias;
        }
    }
}
