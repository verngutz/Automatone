using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class NoteName
    {
        private char letter;
        private char accidental;

        //note - should be 'c', 'd', 'e', 'f', 'g', 'a', or 'b' only
        //accidental - should be '#', 'b', or ' '
        public NoteName(char letter, char accidental)
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
