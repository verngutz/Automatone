import java.util.*;

public class BasicWesternTheory extends Theory
{
	public static final NoteName NOTE_C              = new NoteName('c', ' ');
	public static final NoteName NOTE_C_SHARP        = new NoteName('c', '#');
	public static final NoteName NOTE_C_FLAT         = new NoteName('b', ' ');
	public static final NoteName NOTE_C_DOUBLE_SHARP = new NoteName('d', ' ');
	public static final NoteName NOTE_C_DOUBLE_FLAT  = new NoteName('b', 'b');
	
	public static final NoteName NOTE_D              = new NoteName('d', ' ');
	public static final NoteName NOTE_D_SHARP        = new NoteName('d', '#');
	public static final NoteName NOTE_D_FLAT         = new NoteName('d', 'b');
	public static final NoteName NOTE_D_DOUBLE_SHARP = new NoteName('e', ' ');
	public static final NoteName NOTE_D_DOUBLE_FLAT  = new NoteName('c', ' ');
	
	public static final NoteName NOTE_E              = new NoteName('e', ' ');
	public static final NoteName NOTE_E_SHARP        = new NoteName('f', ' ');
	public static final NoteName NOTE_E_FLAT         = new NoteName('e', 'b');
	public static final NoteName NOTE_E_DOUBLE_SHARP = new NoteName('f', '#');
	public static final NoteName NOTE_E_DOUBLE_FLAT  = new NoteName('d', ' ');
	
	public static final NoteName NOTE_F              = new NoteName('f', ' ');
	public static final NoteName NOTE_F_SHARP        = new NoteName('f', '#');
	public static final NoteName NOTE_F_FLAT         = new NoteName('e', ' ');
	public static final NoteName NOTE_F_DOUBLE_SHARP = new NoteName('g', ' ');
	public static final NoteName NOTE_F_DOUBLE_FLAT  = new NoteName('e', 'b');
	
	public static final NoteName NOTE_G              = new NoteName('g', ' ');
	public static final NoteName NOTE_G_SHARP        = new NoteName('g', '#');
	public static final NoteName NOTE_G_FLAT         = new NoteName('g', 'b');
	public static final NoteName NOTE_G_DOUBLE_SHARP = new NoteName('a', ' ');
	public static final NoteName NOTE_G_DOUBLE_FLAT  = new NoteName('f', ' ');
	
	public static final NoteName NOTE_A              = new NoteName('a', ' ');
	public static final NoteName NOTE_A_SHARP        = new NoteName('a', '#');
	public static final NoteName NOTE_A_FLAT         = new NoteName('a', 'b');
	public static final NoteName NOTE_A_DOUBLE_SHARP = new NoteName('b', ' ');
	public static final NoteName NOTE_A_DOUBLE_FLAT  = new NoteName('g', ' ');
	
	public static final NoteName NOTE_B              = new NoteName('b', ' ');
	public static final NoteName NOTE_B_SHARP        = new NoteName('c', ' ');
	public static final NoteName NOTE_B_FLAT         = new NoteName('b', 'b');
	public static final NoteName NOTE_B_DOUBLE_SHARP = new NoteName('c', '#');
	public static final NoteName NOTE_B_DOUBLE_FLAT  = new NoteName('a', ' ');
	
	public BasicWesternTheory()
	{
	}
	
	public double getBeatResolution()
	{
		return 1 / 16.0;
	}
}