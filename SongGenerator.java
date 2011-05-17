import java.util.*;

public class SongGenerator
{
	public ArrayList<Note> generateSong()
	{
		VerseGenerator vg = new VerseGenerator();
		return vg.generateVerse();
	}
}