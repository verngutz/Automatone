import java.util.*;

public class PhraseGenerator
{
	private static Hashtable<Chord, Chord[]> progression;
	private static void Init()
	{
		progression = new Hashtable<Chord, Chord[]>();
		Chord[] eProg = {Chord.A};
		progression.put(Chord.E, eProg);
		Chord[] aProg = {Chord.D, Chord.F};
		progression.put(Chord.A, aProg);
		Chord[] dfProg = {Chord.G, Chord.B};
		progression.put(Chord.D, dfProg);
		progression.put(Chord.F, dfProg);
		Chord[] gProg = {Chord.C};
		progression.put(Chord.G, gProg);
		Chord[] bProg = {Chord.C, Chord.E};
		progression.put(Chord.B, bProg);
		Chord[] cProg = {Chord.C, Chord.D, Chord.E, Chord.F, Chord.G, Chord.A, Chord.B};
		progression.put(Chord.C, cProg);
	}
	public PhraseGenerator()
	{
		Init();
	}
	
	public String generatePhrase()
	{
		Chord curr = getRandomChord();
		String phrase = curr.toString();
		while(curr != Chord.C)
		{
			int length = progression.get(curr).length;
			int index = (int)(Math.rand() * length);
			curr = progression.get(index);
			phrase.add(curr.toString());
		}
		return phrase;
	}
	
	private Chord getRandomChord()
	{
		double rand = Math.rand();
		if(rand < 1.0 / 7) return Chord.C;
		else if(rand < 2.0 / 7) return Chord.D;
		else if(rand < 3.0 / 7) return Chord.E;
		else if(rand < 4.0 / 7) return Chord.F;
		else if(rand < 5.0 / 7) return Chord.G;
		else if(rand < 6.0 / 7) return Chord.A;
		else if(rand < 1) return Chord.B;
	}
}

class Chord
{
	public static final Chord 	C = new Chord('c'),
								D = new Chord('d'),
								E = new Chord('e'),
								F = new Chord('f'),
								G = new Chord('g'),
								A = new Chord('a'),
								B = new Chord('b');
								
	private static char[] keyboard = {'a', 'b', 'c', 'd', 'e', 'f', 'g'};
	private char base;
	private char third;
	private char fifth;
	private Chord(char base)
	{
		this.base = keyboard[base - 'a'];
		third = keyboard[(base - 'a' + 2) % 7];
		fifth = keyboard[(base - 'a' + 4) % 7];
	}
	public String toString()
	{
		return "";
	}
}