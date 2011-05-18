import java.util.*;

public class VerseGenerator
{
	public CellState[][] generateVerse(Theory theory)
	{
		PhraseGenerator pg = new PhraseGenerator();
		return pg.generatePhrase(theory);
	}
}