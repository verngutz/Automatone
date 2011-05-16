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
	public String randomVolume(double lbound, double ubound)
	{
		return " $" + (Math.random() * (ubound - lbound) + lbound) + ";\n";
	}
	public String toString()
	{
		String vol = randomVolume(100.0,127.0);
		String octave = "3";
		String time = "1/2";
		return strum(time, octave, vol) + strum(time, octave);
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