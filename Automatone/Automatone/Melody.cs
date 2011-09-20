namespace Automatone
{
    public class Melody
    {
        private double[] melodyBias;
        private double pitchContiguity;
        private MusicTheory theory;

        public Melody(MusicTheory theory, double chordalityObedience, double tonalityObedience, double pitchContiguityObedience)
        {
            melodyBias = new double[2];
            double[] melodyBiasSample = theory.MELODY_BIAS_SAMPLE;
            melodyBias[0] = melodyBiasSample[0] + (chordalityObedience - 1) * (melodyBiasSample[0] - 0.5);
            melodyBias[1] = melodyBiasSample[1] + (tonalityObedience - 1) * (melodyBiasSample[1] - 0.5);
            pitchContiguity = theory.PITCH_CONTIGUITY * pitchContiguityObedience;
            this.theory = theory;
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
