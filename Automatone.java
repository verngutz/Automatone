import java.util.*;
import java.io.*;

public class Automatone
{
	public static void main(String args[]) throws IOException
	{
		FileWriter fw = new FileWriter("sample.txt");
		fw.write("mthd\n\tversion 1\n\tunit 192\nend mthd\n\n");
		fw.write("mtrk\n\ttact 4 / 4 24 8\n\tbeats 140\n\tkey \"Cmaj\"\nend mtrk\n\n");
		final long SEED = 4;
		Random random = new Random(SEED);
		Theory theory = new BasicWesternTheory(random);
		SongGenerator sg = new SongGenerator(random);
		fw.write(sg.generateSong(theory));
		fw.close();
	}
}