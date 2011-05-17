import java.util.*;

public class Phrase
{	
	ArrayList<Note> noteList;
	ArrayList<Chord> progression = new ArrayList<Chord>();
	public Phrase()
	{
		noteList = new ArrayList<Note>();
		progression = new ArrayList<Chord>();
	}
	
	public ArrayList<Note> getNoteList()
	{
		return noteList;
	}
}