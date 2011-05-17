import java.util.*;

public class PhraseGenerator
{
	private ArrayList<Phrase> phraseChromosomes;
	
	public PhraseGenerator() 
	{
		phraseChromosomes = new ArrayList<Phrase>();
	}
	
	public ArrayList<Note> generatePhrase()
	{
		Phrase p = new Phrase();
		phraseChromosomes.add(p);
		return p.toNoteList();
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
	private Chord getRandomChord()
	{
		return null;
	}
	
}