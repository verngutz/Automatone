public class NoteName
{
	private char letter;
	private char accidental;
	
	//note - should be 'c', 'd', 'e', 'f', 'g', 'a', or 'b' only
	//accidental - should be '#', 'b', or ' '
	public NoteName(char letter, char accidental)
	{
		this.letter = letter;
		this.accidental = accidental;
	}
	
	public String toString()
	{
		return "" + letter + (accidental == ' ' ? "" : accidental);
	}
	
	public char getLetter()
	{
		return letter;
	}
	
	public char getAccidental()
	{
		return accidental;
	}
	
	public boolean equals(Object o)
	{
		NoteName other = (NoteName)o;
		return other.letter == letter && other.accidental == accidental;
	}
	
	public int hashCode()
	{
		return letter * 10000 + accidental;
	}
}