using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class NoteName
    {
        public static NoteName NOTE_C = new NoteName('c', ' ');
        public static NoteName NOTE_C_SHARP = new NoteName('c', '#');
        public static NoteName NOTE_C_FLAT = new NoteName('b', ' ');
        public static NoteName NOTE_C_DOUBLE_SHARP = new NoteName('d', ' ');
        public static NoteName NOTE_C_DOUBLE_FLAT = new NoteName('a', '#');

        public static NoteName NOTE_D = new NoteName('d', ' ');
        public static NoteName NOTE_D_SHARP = new NoteName('d', '#');
        public static NoteName NOTE_D_FLAT = new NoteName('c', '#');
        public static NoteName NOTE_D_DOUBLE_SHARP = new NoteName('e', ' ');
        public static NoteName NOTE_D_DOUBLE_FLAT = new NoteName('c', ' ');

        public static NoteName NOTE_E = new NoteName('e', ' ');
        public static NoteName NOTE_E_SHARP = new NoteName('f', ' ');
        public static NoteName NOTE_E_FLAT = new NoteName('d', '#');
        public static NoteName NOTE_E_DOUBLE_SHARP = new NoteName('f', '#');
        public static NoteName NOTE_E_DOUBLE_FLAT = new NoteName('d', ' ');

        public static NoteName NOTE_F = new NoteName('f', ' ');
        public static NoteName NOTE_F_SHARP = new NoteName('f', '#');
        public static NoteName NOTE_F_FLAT = new NoteName('e', ' ');
        public static NoteName NOTE_F_DOUBLE_SHARP = new NoteName('g', ' ');
        public static NoteName NOTE_F_DOUBLE_FLAT = new NoteName('d', '#');

        public static NoteName NOTE_G = new NoteName('g', ' ');
        public static NoteName NOTE_G_SHARP = new NoteName('g', '#');
        public static NoteName NOTE_G_FLAT = new NoteName('f', '#');
        public static NoteName NOTE_G_DOUBLE_SHARP = new NoteName('a', ' ');
        public static NoteName NOTE_G_DOUBLE_FLAT = new NoteName('f', ' ');

        public static NoteName NOTE_A = new NoteName('a', ' ');
        public static NoteName NOTE_A_SHARP = new NoteName('a', '#');
        public static NoteName NOTE_A_FLAT = new NoteName('g', '#');
        public static NoteName NOTE_A_DOUBLE_SHARP = new NoteName('b', ' ');
        public static NoteName NOTE_A_DOUBLE_FLAT = new NoteName('g', ' ');

        public static NoteName NOTE_B = new NoteName('b', ' ');
        public static NoteName NOTE_B_SHARP = new NoteName('c', ' ');
        public static NoteName NOTE_B_FLAT = new NoteName('a', '#');
        public static NoteName NOTE_B_DOUBLE_SHARP = new NoteName('c', '#');
        public static NoteName NOTE_B_DOUBLE_FLAT = new NoteName('a', ' ');

        private char letter;
        private char accidental;

        private NoteName(char letter, char accidental)
        {
            this.letter = letter;
            this.accidental = accidental;
        }

        public override String ToString()
        {
            return "" + letter + (accidental == ' ' ? "" : accidental.ToString());
        }

        public char getLetter()
        {
            return letter;
        }

        public char getAccidental()
        {
            return accidental;
        }

        public override bool Equals(Object o)
        {
            NoteName other = (NoteName)o;
            return other.letter == letter && other.accidental == accidental;
        }

        public override int GetHashCode()
        {
            return letter * 10000 + accidental;
        }
    }
}
