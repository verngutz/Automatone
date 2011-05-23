import java.util.*;

public class PhraseGenerator
{
	public CellState[][] generatePhrase(Theory theory)
	{
		CellState[][] phrase = theory.initialize();
		theory.evolve(phrase);
		return phrase;
	}
}