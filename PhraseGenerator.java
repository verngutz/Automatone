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
}