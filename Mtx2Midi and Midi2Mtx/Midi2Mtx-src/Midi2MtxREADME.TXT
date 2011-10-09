1. How to convert a MIDI file (.mid or .midi) to an MTX file (.mtx)

  RIGHT-click on an MIDI file and select 'Convert to MTX file' in the
  pop-up context menu. A file with the same name but having extension
  .mtx will be created in the same folder.

3. How to un-install?

  In folder where you installed the program (usually
  C:\Program Files\Mid2Mtx\ unless you selected a diffrent folder), 
  double-click on uninstall.exe. Or you can use the
  'Add or Remove Programs' applet in the Control Panel.

  On some systems (Windows 95, and perhaps Windows 98 also), 
  a window may pop up saying "System Error, The file you are trying 
  to open is already in use by another program. Quit the other program,
  and then try agian." In that case, just click on the 'Cancel' button to 
  close the pop-up window, and everything will proceed as it should.

-------------------------------------------------------------------------

This program is an adaptation of mf2t written by Piet van Oostrum, 
Dept of Computer Science, Utrecht University, Utrecht, The Netherlands.
I merely modified it to make it compile and run on Windows XP, and 
hook it into the Registry so you can right-click on an .MIDI file to create a 
corresponding .mtx file without having to run the program in a DOS window. 

Hope you find it useful.

Ying-Da Lee
April 2006

-------------------------------------------------------------------------

For people who want to run the program in a DOS window.

/*
 * Usage:
 *
 * Midi2Mtx [-mntv] [-f[n]] inputfile [outputfile]
 *
 *  inputfile: The input MIDI file to translate into text file.
 *              It is always required.
 *  outputfile: Optional. If present, it specifies the name of the output
 *              file. If not, then the outputfile name is derived from the
 *              inputfile name: Given xyz.mid as the inputfile, the output
 *              will be written into xyz.mtx; the path will be preserved as
 *              well. In either case, if the outputfile already exists,
 *              the existing file will be renamed (by appending as many $
 *              as necessary).
 *
 * Note that if the file names contain spaces, e.g., 
 *    C:\My Documents\My Music\song1.mtx
 * or
 *    Allegro ma non troppo.mid
 *
 * they MUST be in quotes, e.g
 *    "C:\My Documents\My Music\song1.mtx"
 * or
 *    "Allegro ma non troppo.mid"
 *
 * Options:
 *	-m	merge partial sysex into a single sysex message
 *	-n	write notes in symbolic rather than numeric form. A-C
 *		optionally followed by # (sharp) followed by octave number.
 *	-b	or
 *	-t	event times are written as bar:beat:click rather than a click number
 *	-v	use a slightly more verbose output
 *	-f<n>	fold long text and hex entries into more lines <n>=line length
 *		(default 80).
 *
 */