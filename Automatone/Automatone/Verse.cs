using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Automatone
{
    public class Verse
    {
        List<Phrase> phrases;
        public enum CADENCE_NAMES { HALF, AUTHENTIC, PLAGAL, DECEPTIVE, SILENT };
		static double[][] CADENCES = new double[][] { new double[] {0, 1, 0.3, 0.6}, new double[] {1, 0, 0.5, 0.2} };
		const double CADENCE_SMOOTHNESS = 0.5;
        Automatone aut;
        public Verse(Automatone aut, Random rand)
        {
            phrases = new List<Phrase>();
            int verseLength = 8;
		    List<double> fractalCurve = Enumerable.Repeat<double>(1.0, verseLength).ToList<double>();
            
		    int x = verseLength;
		    for(int i = 2; i <= Math.Sqrt(verseLength); i++)
		    {
			    while(x % i == 0)
			    {
				    for(int j = 0; j < verseLength; j++)
				    {
					    if((j + 1) % x != 0)
					    {
						    fractalCurve[j] *= CADENCE_SMOOTHNESS;
					    }
				    }
				    x /= i;
			    }
		    }
		    for(int i = 0; i < verseLength; i++)
		    {
			    int a = 0;
			    if(rand.NextDouble() > fractalCurve[i])
			    {
				    a = 1;
			    }
			    double sum = 0;
			    foreach(double j in CADENCES[a])
			    {
				    sum += j;
			    }
			    double r = rand.NextDouble() * sum;
                bool addDefaultPhrase = true;
			    for(int j = 0; j < 4; j++)
			    {
				    if(r < CADENCES[a][j])
				    {
                        phrases.Add(new Phrase((CADENCE_NAMES)j, aut));
                        addDefaultPhrase = false;
					    break;
				    }
				    else
				    {
					    r -= CADENCES[a][j];
				    }
			    }
			    if(addDefaultPhrase)
			    {
				    phrases.Add(new Phrase(CADENCE_NAMES.SILENT, aut));
			    }
		    }
            this.aut = aut;
            old = new KeyboardState();
            foreach (Phrase p in phrases)
            {
                System.Console.WriteLine(p.midi);
            }
        }
        KeyboardState old;
        public void Play()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P) && old.IsKeyUp(Keys.P))
            {
                phrases.First<Phrase>().Play();
                phrases.RemoveAt(0);
            }
            old = Keyboard.GetState();
        }
    }
}
