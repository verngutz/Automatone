import java.util.*;

public class PhraseGenerator
{					
	public PhraseGenerator()
	{
		
	}
	
	public CellState[][] generatePhrase(Theory theory)
	{
		CellState[][] phrase;
		theory.populate(phrase);
		theory.evolve(phrase);
		return phrase;
	}
}