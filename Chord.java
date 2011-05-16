public class Chord
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
		+ "\t1/2;-" + base + "3 $00;\n\t-" + third + "3 $00;\n\t-" + fifth + "3 $00;\n";
	}
}