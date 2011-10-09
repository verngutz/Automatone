/* $Id: mf2t.c,v 1.5 1995/12/14 20:20:10 piet Rel $ */
/*
 * 
 * 
 * Convert a MIDI file to MTX file (.mtx).
 *
 * Originally mf2t, by Piet van Oostrum <piet@cs.ruu.nl>
 *
 * March 2006, Ying-Da Lee
 * - Took out all indirected function references.
 * - Use MtxFile rather than stdout for output file.
 * - Changed the handling of input and output specifications in the command line.
 * - Reorganzed the code.
 *   
 * Usage:
 *
 * Midi2Mtx [-mntv] [-f[n]] inputfile [outputfile]
 *
 *  inputfile: The input MIDI file to translate into text file.
 *        It is always required.
 *  outputfile: Optional. If present, it specifies the name of the output
 *        file. if not, then the outputfile name is derived from the
 *        inputfile name: Given xyz.mid as the inputfile, the output
 *        will be written into xyz.mtx; the path will be preserved as
 *        well. In either case, if the outputfile already exists,
 *        the existing file will be renamed (by appending as many $
 *        as necessary).
 *
 * Note that if the file names contain spaces, e.g., 
 *    C:\My Documents\My Music\song1.mtx
 * or
 *    Allegro ma non troppo.mid
 * they MUST be in quotes, e.g
 *    "C:\My Documents\My Music\song1.mtx"
 * or
 *    "Allegro ma non troppo.mid"
 *
 * Options:
 *  -m	merge partial sysex into a single sysex message
 *  -n	write notes in symbolic rather than numeric form. A-C
 *      optionally followed by # (sharp) followed by octave number.
 *  -b  or
 *  -t  event times are written as bar:beat:click rather than a click number
 *  -v  use a slightly more verbose output
 *  -f<n>  fold long text and hex entries into more lines <n>=line length
 *      (default 80).
 *
 */


#include <stdio.h>
#include <ctype.h>
#include <fcntl.h>
#include <windows.h>
#include "Midi2Mtx.h"

extern int arg_index;
extern char *arg_option;

/* options */

int fold = 0;		/* fold long lines */
int notes = 0;		/* print notes as a-g */
int times = 0;		/* print times as Measure/beat/click */

char *Onmsg  = "On ch=%d n=%s v=%d\n";
char *Offmsg = "Off ch=%d n=%s v=%d\n";
char *PoPrmsg = "PoPr ch=%d n=%s v=%d\n";
char *Parmsg = "Par ch=%d c=%d v=%d\n";
char *Pbmsg  = "Pb ch=%d v=%d\n";
char *PrChmsg = "PrCh ch=%d p=%d\n";
char *ChPrmsg = "ChPr ch=%d v=%d\n";

FILE *midiFile = NULL;
FILE *mtxFile = NULL;


FILE *efopen(char *name, char *mode)
{
    FILE *f;
    extern int errno;
    if ( (f=fopen(name,mode)) == NULL ) {
        (void) fprintf(stderr,"***Error: Cannot open '%s'!\n",name);
        fprintf(stderr, strerror(errno));
    }
    return(f);
}


char *getLastc(char *s, int c)
/* returns pointer to the last occurrence of c in s, or its terminatig '\0' */
{
    char *Lastc;
    
    Lastc = strrchr(s, c);
    if (Lastc == NULL) Lastc = s + strlen(s);
    return Lastc;
}

int main(int argc, char **argv)
{
	
	int flg;
	char inName[FILENAME_MAX], outName[FILENAME_MAX];
    char tmpName[FILENAME_MAX];
    FILE *outF;
	
	Mf_nomerge = 1;
	while (flg = crack (argc, argv, "F|f|BbNnTtVvMm", 0)) {
		switch (flg) {
		case 'f':
		case 'F':
			if (*arg_option)
				fold = atoi(arg_option);
			else
				fold = 80;
			break;
		case 'm':
		case 'M':
			Mf_nomerge = 0;
			break;
		case 'n':
		case 'N':
			notes++;
			break;
		case 't':
		case 'T':
		case 'b':
		case 'B':
			times++;
			break;
		case 'v':
		case 'V':
			Onmsg  = "On ch=%d note=%s vol=%d\n";
			Offmsg = "Off ch=%d note=%s vol=%d\n";
			PoPrmsg = "PolyPr ch=%d note=%s val=%d\n";
			Parmsg = "Param ch=%d con=%d val=%d\n";
			Pbmsg  = "Pb ch=%d val=%d\n";
			PrChmsg = "ProgCh ch=%d prog=%d\n";
			ChPrmsg = "ChanPr ch=%d val=%d\n";
			break;
		case EOF:
			PauseAndGo(1); //exit(1);
		}
	}
    if ( arg_index < argc ) {
        strcpy(inName, argv[arg_index++]);
        if((midiFile = efopen (inName, "rb")) == NULL)
          PauseAndGo(1); //exit(1);
    } else {
        fprintf(stderr, "***Error: No input file specified.\n" );
        PauseAndGo(1); //exit(1);
    }
    if (arg_index < argc ) {
        strcpy(outName, argv[arg_index]);
    } else {
        strcpy(outName, inName);
        strcpy(getLastc(outName, '.'), ".mtx"); /* replace or append ".mtx" as file extension */
    }
    
    /* if outName already exists, rename it. */
    strcpy(tmpName, outName);
    if (mtxFile = fopen(tmpName, "r")) {
      do {
        fclose(mtxFile);
        strcpy(getLastc(tmpName, '.'), "$.mtx");
      } while (mtxFile = fopen(tmpName, "r"));
      if (rename(outName, tmpName)) {
        fprintf(stderr, "***Error: Output file %s exists and cannot be renamed.\n",
            outName);
        PauseAndGo(1); //exit(1);
      } else {
        fprintf(stderr, "Notice: Output file %s already exits.\n\tIt is renamed as %s\n",
          outName, tmpName);
      }
    }      

    if ((mtxFile = fopen(outName, "w")) == NULL) {
      fprintf(stderr, "***Error: Can't open outputfile %s\n", outName);
      fprintf(stderr, "%s\n", strerror(errno));
      PauseAndGo(1); //exit(1);
    }  

    fprintf(stderr, "Writing output to %s ...  ", outName);
    
	ReadMidifile();
    if (ferror(mtxFile)) {
      fprintf(stderr, "***Error while writing to output file %s\n", outName);
      fprintf(stderr, "%s\n", strerror(errno));
      PauseAndGo(1); //exit(1);
    }  
    fseek(mtxFile, 0, SEEK_END); /* just to be sure */
    fprintf(stderr, "%ld bytes written.\n", ftell(mtxFile));

	fclose(midiFile);
	fclose(mtxFile);
	PauseAndGo(0); //exit(0);
}

