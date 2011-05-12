import java.util.*;

public class PhraseGenerator
{
	public static final Chord 	C = new Chord('c'),
								D = new Chord('d'),
								E = new Chord('e'),
								F = new Chord('f'),
								G = new Chord('g'),
								A = new Chord('a'),
								B = new Chord('b');
								
	private static Hashtable<Chord, Chord[]> progression;
	private static void Init()
	{
		progression = new Hashtable<Chord, Chord[]>();
		Chord[] eProg = {A};
		progression.put(E, eProg);
		Chord[] aProg = {D, F};
		progression.put(A, aProg);
		Chord[] dfProg = {G, B};
		progression.put(D, dfProg);
		progression.put(F, dfProg);
		Chord[] gProg = {C};
		progression.put(G, gProg);
		Chord[] bProg = {C, E};
		progression.put(B, bProg);
		Chord[] cProg = {C, D, E, F, G, A, B};
		progression.put(C, cProg);
	}
	public PhraseGenerator()
	{
		Init();
	}
	
	public String generatePhrase()
	{
		String phrase = "mtrk(1)\n\tprefixport 0\n\tprefixchannel 1\n\tprogram GrandPno\n\tvolume 127\n\tbalance 64\n\treverb 64\n\t1/4;\n";
		Chord curr = getRandomChord();
		phrase += curr.toString();
		while(curr != C)
		{
			int length = progression.get(curr).length;
			int index = (int)(Math.random() * length);
			curr = progression.get(curr)[index];
			phrase += curr.toString();
		}
		phrase += "end mtrk\n";
		return phrase;
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
}

class Chord
{					
	private static char[] keyboard = {'a', 'b', 'c', 'd', 'e', 'f', 'g'};
	private char base;
	private char third;
	private char fifth;
	public Chord(char base)
	{
		this.base = keyboard[base - 'a'];
		third = keyboard[(base - 'a' + 2) % 7];
		fifth = keyboard[(base - 'a' + 4) % 7];
	}
	public String toString()
	{
		return "\t+" + base + "3 $7F;\n\t+" + third + "3 $7F;\n\t+" + fifth + "3 $7F;\n" 
		+ "\t1/4;-" + base + "3 $00;\n\t-" + third + "3 $00;\n\t-" + fifth + "3 $00;\n";
	}
}