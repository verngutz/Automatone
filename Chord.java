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
	public String randomVolume(int lbound, int ubound)
	{
		return " $" + (int)(Math.random() * (ubound - lbound) + lbound) + ";\n";
		//return " $7F;\n";
	}
	public String toString(String time, String octave)
	{
		String vol = randomVolume(127,127);
		
		return strum(time, octave, vol);
	}
	public String strum(String time, String octave, String vol)
	{
		return 	"\t" + time + ";+" + base + octave + vol + 
				"\t+" + third + octave + vol + 
				"\t+" + fifth + octave + vol;
	}
	public String strum(String time, String octave)
	{
		return 	"\t" + time + ";-" + base + octave + " $00;\n" +
				"\t-" + third + octave + " $00;\n" +
				"\t-" + fifth + octave + " $00;\n";
	}
}