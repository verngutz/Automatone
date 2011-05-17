public class Chord
{
	public enum Mode
	{
		MAJOR,                   //C
		MINOR,                   //Cm
		AUGMENTED,               //Caug
		DIMINISHED,              //Cdim
		DOMINANT_SEVENTH,        //C7
		MAJOR_SEVENTH,           //CM7
		MINOR_SEVENTH,           //Cm7
		DIMINISHED_SEVENTH,      //Cdim7
		MINOR_MAJOR_SEVENTH,     //CmM7
		AUGMENTED_MAJOR_SEVENTH, //C+M7
		AUGMENTED_SEVENTH,       //C+7
		MANUAL
	}
	
	private ArrayList<Notename> chordConstituents;
	
	public Chord(Notename base, Mode mode)
	{
		chordConstituents = new ArrayList<Notename>();
	}
	
	public Chord(Notename base, int[] intervals)
	{
		chordConsituents = new ArrayList<Notename>();
		for(int i = 0; i < intervals.length; i++)
		{
			
		}
	}
	
	public String randomVolume(double lbound, double ubound)
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