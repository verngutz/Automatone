namespace Automatone
{
    public class NoteName
    {
        public static NoteName C = new NoteName(0);
        public static NoteName C_SHARP = new NoteName(1);
        public static NoteName C_FLAT = B;
        public static NoteName C_DOUBLE_SHARP = D;
        public static NoteName C_DOUBLE_FLAT = A_SHARP;

        public static NoteName D = new NoteName(2);
        public static NoteName D_SHARP = new NoteName(3);
        public static NoteName D_FLAT = C_SHARP;
        public static NoteName D_DOUBLE_SHARP = E;
        public static NoteName D_DOUBLE_FLAT = C;

        public static NoteName E = new NoteName(4);
        public static NoteName E_SHARP = F;
        public static NoteName E_FLAT = D_SHARP;
        public static NoteName E_DOUBLE_SHARP = F_SHARP;
        public static NoteName E_DOUBLE_FLAT = D;

        public static NoteName F = new NoteName(5);
        public static NoteName F_SHARP = new NoteName(6);
        public static NoteName F_FLAT = E;
        public static NoteName F_DOUBLE_SHARP = G;
        public static NoteName F_DOUBLE_FLAT = D_SHARP;

        public static NoteName G = new NoteName(7);
        public static NoteName G_SHARP = new NoteName(8);
        public static NoteName G_FLAT = F_SHARP;
        public static NoteName G_DOUBLE_SHARP = A;
        public static NoteName G_DOUBLE_FLAT = F;

        public static NoteName A = new NoteName(9);
        public static NoteName A_SHARP = new NoteName(10);
        public static NoteName A_FLAT = G_SHARP;
        public static NoteName A_DOUBLE_SHARP = B;
        public static NoteName A_DOUBLE_FLAT = G;

        public static NoteName B = new NoteName(11);
        public static NoteName B_SHARP = C;
        public static NoteName B_FLAT = A_SHARP;
        public static NoteName B_DOUBLE_SHARP = C_SHARP;
        public static NoteName B_DOUBLE_FLAT = A;

        private byte chromaticIndex;
        public byte ChromaticIndex { get { return chromaticIndex; } }

        public NoteName(byte chromaticIndex)
        {
            this.chromaticIndex = chromaticIndex;
        }

        public override bool Equals(object obj)
        {
 	        return obj is NoteName && ChromaticIndex == (obj as NoteName).ChromaticIndex;
        }

        public override int GetHashCode()
        {
            return chromaticIndex.GetHashCode();
        }
    }
}
