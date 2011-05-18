import java.util.*;
import java.io.*;

public class Automatone
{
	public static void main(String args[]) throws IOException
	{
		FileWriter fw = new FileWriter("sample.txt");
		fw.write("mthd\n\tversion 1\n\tunit 192\nend mthd\n\n");
		fw.write("mtrk\n\ttact 4 / 4 24 8\n\tbeats 120\n\tkey \"Cmaj\"\nend mtrk\n\n");
		Theory theory = new BasicWesternTheory();
		SongGenerator sg = new SongGenerator();
		fw.write(sg.generateSong(theory));
		fw.close();
	}
}