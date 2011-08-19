using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class InputParameters
    {
        // Song Parameters
        public static double length = 0.1;
        public static double structuralVar = 0.2;

        //Verse Parameters
        public static double meanVerseLength = 0.5;
        public static double verseLengthVariance = 0.5;
        public static double phraseVariance = 0.5;

        //Rhythm
        public static double rhythmObedience = 0.5;
    }
}
