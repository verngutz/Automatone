using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class Phrase
    {
        public String midi = "";
        Automatone aut;
        public Phrase(Verse.CADENCE_NAMES c, Automatone aut) 
        {
            switch (c)
            {
                case Verse.CADENCE_NAMES.AUTHENTIC:
                    midi = "authentic.mid";
                    break;
                case Verse.CADENCE_NAMES.DECEPTIVE:
                    midi = "deceptive.mid";
                    break;
                case Verse.CADENCE_NAMES.HALF:
                    midi = "half.mid";
                    break;
                case Verse.CADENCE_NAMES.PLAGAL:
                    midi = "plagal.mid";
                    break;
                case Verse.CADENCE_NAMES.SILENT:
                    midi = "silent.mid";
                    break;
            }
            this.aut = aut;
        }


        public void Play()
        {
            aut.sequencer.StopMidi();
            aut.sequencer.LoadMidi(midi);
            aut.sequencer.PlayMidi();
        }
    }
}
