import java.util.*;

public class PhraseGenerator
{
	public static final Chord 	C = new Chord(new Notename('c', ' '), ),
								Dm = new Chord('d'),
								Em = new Chord('e'),
								F = new Chord('f'),
								G = new Chord('g'),
								Am = new Chord('a'),
								Bdim = new Chord('b');
								
	private Hashtable<Chord, Chord[]> progressionTheory;
	
	public PhraseGenerator()
	{
		progressionTheory = new Hashtable<Chord, Chord[]>();
		Chord[] eProg = {A};
		progressionTheory.put(E, eProg);
		Chord[] aProg = {D, F};
		progressionTheory.put(A, aProg);
		Chord[] dfProg = {G, B};
		progressionTheory.put(D, dfProg);
		progressionTheory.put(F, dfProg);
		Chord[] gProg = {C};
		progressionTheory.put(G, gProg);
		Chord[] bProg = {C, E};
		progressionTheory.put(B, bProg);
		Chord[] cProg = {C, D, E, F, G, A, B};
		progressionTheory.put(C, cProg);
	}
	
	public ArrayList<Note> generatePhrase()
	{
		return (new Phrase(progressionTheory)).getNoteList();
	}
		
	private Chord getRandomChord()
	{
		double rand = Math.random();
		if(rand < 1.0 / 7) return C;
		else if(rand < 2.0 / 7) return D;
		else if(rand < 3.0 / 7) return E;
		else if(rand < 4.0 / 7) return F;
		else if(rand < 5.0 / 7) return G;
		else if(rand < 6.0 / 7) return A;
		else return B;
	}
	
	public String generatePhrase2()
	{
		String phrase = startTrack(1, 0, 1, "GrandPno", 127, 64, 64);
		Chord curr = getRandomChord();
		int timingLimit = 32; //1/4notes * 8bars
		
		phrase += curr.toString("1/4", "3");
		for(int i=0; i<5; i++) //TODO: make this random
		{
			int length = progression.get(curr).length;
			int index = (int)(Math.random() * length);
			curr = progression.get(curr)[index];
			phrase += curr.toString("1/4", "3");
		}
		
		phrase += "end mtrk\n";
		return phrase;
	}
	
	private int[] generateRhythm(int numChords, int notes, int bars)
	{
		int[] partition = new int[numChords];
		ArrayList<Integer> range = new ArrayList<Integer>();
		range.add(1);
		range.add(numChords);
		
		int limit = notes * bars;
		int ubound = limit;
		int lbound = 1;
		for(int i=0; i<numChords; i++)
		{
			
			if(i!=0)
			{
				Collections.sort(range);
				for(int j=range.size(); j>1; j--)
				{
					if(range.get(j)-range.get(j-1) > maxDiff) 
						maxDiff = range.get(j)-range.get(j-1);
				}
			}
			else
			{
				partition[i] = (int) Math.random() * (ubound - lbound) + lbound;
				range.add(partition[i]);
			}
		}
	}
}