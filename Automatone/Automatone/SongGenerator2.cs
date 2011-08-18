using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automatone
{
    public class SongGenerator2
    {
        Automatone aut;
        Random r;

        List<Verse> song = new List<Verse>();
        public SongGenerator2(Automatone aut)
        {
            this.aut = aut;
            r = new Random();
        }

        public void SongTest()
        {
            const int LENGTHINESS = 10;
		    const double CHORUS_EXISTENCE = 1;
		    double length = 0.1;
		    double structuralVar = 0.2;
		    List<Verse> verses = new List<Verse>();
		
		    for(int i = 0; i < 1 + 2 * structuralVar * length * LENGTHINESS; i++)
		    {
			    verses.Add(new Verse(aut, r));
		    }
		
		    double chorusProb = r.NextDouble();
            int? prev = null;
		    for(int i = 0; i < length * LENGTHINESS; i++)
		    {
			    int curr = (int)(r.NextDouble() * verses.Count);
			    if(chorusProb < CHORUS_EXISTENCE)
			    {
				    if (prev != null && ((prev == 0  && curr == 0) || prev != 0 && curr != 0))
				    {
                        curr = (int)(r.NextDouble() * verses.Count);
				    }
			    }
			    song.Add(verses.ElementAt<Verse>(curr));
                prev = curr;
		    }
		
		    
        }

        public void Play()
        {
            foreach (Verse v in song)
            {
                v.Play();
            }
        }
    }
}
