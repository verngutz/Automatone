import java.util.*;

public class PhraseGenerator
{
	public enum Cadence
	{ 
		HALF,
		AUTHENTIC,
		PLAGAL,
		DECEPTIVE
	};
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
	public String startTrack(int track, int port, int channel, String instr, int volume, int balance, int reverb)
	{
		return "mtrk(" + track + ")\n\tprefixport " + port + "\n\tprefixchannel " + channel + "\n\tprogram " + instr + "\n\tvolume " + volume + "\n\tbalance " + balance + "\n\treverb " + reverb + "\n";
	}
	
	public String generatePhrase()
	{
		String phrase = startTrack(1, 0, 1, "GrandPno", 127, 64, 64);
		Chord curr = getRandomChord();
		phrase += "\t1/4;\n"; //generate random timing
		phrase += curr.toString();
		do
		{
			int length = progression.get(curr).length;
			int index = (int)(Math.random() * length);
			curr = progression.get(curr)[index];
			phrase += curr.toString();
		}
		while(curr != C);
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