namespace Automatone.Music
{
    public class Melody
    {
        private double[] melodyBias;
        public double[] MelodyBias { get { return melodyBias; } }
        private double pitchContiguity;
        public double PitchContiguity { get { return pitchContiguity; } }

        public Melody(MusicTheory theory)
        {
            melodyBias = new double[2];
            double[] melodyBiasSample = theory.MELODY_BIAS_SAMPLE;
            melodyBias[0] = melodyBiasSample[0] + (InputParameters.Instance.ChordalityObedience - 1) * (melodyBiasSample[0] - 0.5);
            melodyBias[1] = melodyBiasSample[1] + (InputParameters.Instance.TonalityObedience - 1) * (melodyBiasSample[1] - 0.5);
            pitchContiguity = theory.PITCH_CONTIGUITY * InputParameters.Instance.MeanPitchContiguity;
        }
    }
}
