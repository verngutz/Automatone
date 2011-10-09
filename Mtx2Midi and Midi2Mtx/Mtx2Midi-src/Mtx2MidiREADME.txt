1. How to convert an MTX file (.mtx) to a MIDI file (.mid)

  Just double-click on an MTX file. A MIDI file with the same filename 
  but having extension .mid will be created in the same folder.
  Alternatively, you can RIGHT-click on an MTX file and select 
  'Convert to MIDI file'.

2. How to create an MTX file?

  a) In MIDI-OX, enable logging, select 'MIDI to Text' for Log Format, 
      and use .mtx as the extension of the log File Name.
  b) Run Midi2Mtx.exe, Mf2TXP.exe, or Mf2T.exe with a MIDI file as 
      input and use .mtx as extension of the output file.
  c) If you already have text files produced from MIDI-OX or Mf2T.exe, 
      or Mf2TXP.exe, just change the extension from .txt to .mtx.

3. How to un-install?

  In the folder where you installed the program (usually 
  C:\Program Files\Mtx2Midi\ unless you selected a different folder),
  double-click on uninstall.exe. Or you can use the
  'Add or Remove Programs' applet in Control Panel.

  On some systems (Windows 95, and perhaps Windows 98 also), 
  a window may pop up saying "System Error, The file you are trying 
  to open is already in use by another program. Quit the other program,
  and then try agian." In that case, just click on the 'Cancel' button to 
  close the pop-up window, and everything will proceed as it should.

-------------------------------------------------------------------------

This program is an adaptation of t2mf written by Piet van Oostrum, 
Dept of Computer Science, Utrecht University, Utrecht, The Netherlands.
I merely modified it to make it compile and run on Windows XP, and 
hook it into the Registry so you can double-click or right-click on an
.mtx file to create a corresponding .mid file without having to run the
program in a DOS window. 

Hope you find it useful.

Ying-Da Lee
April 2006

-------------------------------------------------------------------------

For people who want to run the program in a DOS window.

/*
 * Usage:
 *
 *  Mtx2Midi [-r] inputfile [outputfile]
 *
 *  inputfile: The input text file (.mtx) to translate into MIDI file.
 *              It is always required.
 *  outputfile: Optional. If present, it specifies the name of the output
 *              file. if not, then the outputfile name is derived from the
 *              inputfile name: Given xyz.mtx as the inputfile, the output
 *              will be written into xyz.mid; the path will be preserved as
 *              well. In either case, if the outputfile already exists,
 *              the existing file will be renamed (by appending as many $'s
 *              as necessary).
 * Note that if the file names contain spaces, e.g., 
 *    C:\My Documents\My Music\song1.mtx
 * or
 *    Allegro ma non troppo.mid
 * they MUST be in quotes, e.g
 *    "C:\My Documents\My Music\song1.mtx"
 * or
 *    "Allegro ma non troppo.mid"
 *
 * Option:
 * -r	 Use running status, which can reduce the size of the MIDI file
 *      produced, but some programs/instruments may have problems
 *      handling it.
 */
