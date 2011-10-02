namespace Automatone
{
    public class Melody
    {
        private double[] melodyBias;
        private double pitchContiguity;

        public Melody(MusicTheory theory)
        {
            melodyBias = new double[2];
            double[] melodyBiasSample = theory.MELODY_BIAS_SAMPLE;
            melodyBias[0] = melodyBiasSample[0] + (InputParameters.Instance.chordalityObedience - 1) * (melodyBiasSample[0] - 0.5);
            melodyBias[1] = melodyBiasSample[1] + (InputParameters.Instance.tonalityObedience - 1) * (melodyBiasSample[1] - 0.5);
            pitchContiguity = theory.PITCH_CONTIGUITY * InputParameters.Instance.meanPitchContiguity;
        }

        public double GetPitchContiguity()
        {
            return pitchContiguity;
        }

        public double[] GetMelodyBias()
        {
            return melodyBias;
        }
    }
}
