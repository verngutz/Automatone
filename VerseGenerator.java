import java.util.*;

public class VerseGenerator
{
	public ArrayList<Note> generateVerse()
	{
		PhraseGenerator pg = new PhraseGenerator();
		return pg.generatePhrase();
	}
}