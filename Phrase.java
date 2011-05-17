import java.util.*;

public class Phrase
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
	
	ArrayList<Chord> progression = new ArrayList<Chord>();
	public Phrase()
	{
		
	}
	
	public ArrayList<Note> toNoteList()
	{
		return new ArrayList<Note>();
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