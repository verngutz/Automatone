namespace Automatone
{
    public class Melody
    {
        private double[] melodyBias;
        private double pitchContiguity;
        private MusicTheory theory;

        public Melody(MusicTheory musicTheory, double chordalityObedience, double tonalityObedience, double pitchContiguityObedience)
        {
            melodyBias = new double[2];
            double[] melodyBiasSample = musicTheory.MELODY_BIAS_SAMPLE;
            melodyBias[0] = melodyBiasSample[0] + (chordalityObedience - 1) * (melodyBiasSample[0] - 0.5);
            melodyBias[1] = melodyBiasSample[1] + (tonalityObedience - 1) * (melodyBiasSample[1] - 0.5);
            pitchContiguity = musicTheory.PITCH_CONTIGUITY * pitchContiguityObedience;
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
