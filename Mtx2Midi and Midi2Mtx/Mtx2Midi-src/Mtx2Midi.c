/* $Id: t2mf.c,v 1.5 1995/12/14 21:58:36 piet Rel piet $ */
/*
 * Convert MTX file (.mtx) to a MIDI file.
 * Originally t2mf by Piet van Oostrum
 *
 * March 2006, Ying-Da Lee
 *
 * - Took out all indirected function references.
 * - Use MtxFile rather than stdout for output file.
 * - Changed the handling of input and output specifications in the 
 *	 command line.
 * - Reorganzed the code.
 *
 * Usage:
 *
 * Mtx2Midi [-r] inputfile [outputfile]
 *
 *  inputfile: The input text file (.mtx) to translate into MIDI file.
 *        It is always required.
 *  outputfile: Optional. If present, it specifies the name of the output
 *        file. if not, then the outputfile name is derived from the
 *        inputfile name: Given xyz.mtx as the inputfile, the output
 *        will be written into xyz.mid; the path will be preserved as
 *        well. In either case, if the outputfile already exists,
 *        the existing file will be renamed (by appending as many $'s
 *        as necessary).
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
 * -r   Use running status, which can reduce the
 *      size of the output MIDI file, but some
 *      some programs and MIDI instruments may
 *      have problems handling it.
 *	
 */


#include <malloc.h>
#include <stdlib.h>
#include <stdio.h>
#include <ctype.h>
#include <setjmp.h>
#include <string.h>
#include <io.h>
#include <fcntl.h>
#include <windows.h>
#include "Mtx2Midi.h"


extern int arg_index;
extern char *arg_option;
extern int Mf_RunStat;

FILE *mtxFile; /* for functions in t2mflex.c */
FILE *midiFile; /* for functions in WriteMidifile.c */

char *getLastc(char *s, int c)
/* returns pointer to the last occurrence of c in s, or its terminatig '\0' */
{
    char *Lastc;
    
    Lastc = strrchr(s, c);
    if (Lastc == NULL) Lastc = s + strlen(s);
    return Lastc;
}

int main(argc,argv)
int argc;
char **argv;
{
    extern int errno;
    
	int flg;
	char inName[FILENAME_MAX], outName[FILENAME_MAX];
	char tmpName[FILENAME_MAX];
        
	while (flg = crack (argc, argv, "Rr", 0)) {
		switch (flg) {
		case 'r':
		case 'R':
			Mf_RunStat = 1;
			break;
		case EOF:
            PauseAndGo(1);  //exit(1);
		}
	}

    if ( arg_index < argc ) {
        strcpy(inName, argv[arg_index++]);
        if((mtxFile = fopen (inName, "r")) == NULL) {
          fprintf(stderr,"***Error while opening input file %s:\n", inName);
          fprintf(stderr,"   %s\n", strerror(errno));
          PauseAndGo(1); //exit(1);
        }
    } else {
        fprintf(stderr, "***Error: No input file specified.\n" );
        PauseAndGo(1); //exit(1);
        }
    if (arg_index < argc ) {
        strcpy(outName, argv[arg_index]);
    } else {
        strcpy(outName, inName);
        strcpy(getLastc(outName, '.'), ".mid"); //replace or append ".mid" as file extension
    }
    
    /* if outName already exists, rename it. */
    strcpy(tmpName, outName);
    if (midiFile = fopen(tmpName, "r")) {
      do {
        fclose(midiFile);
        strcpy(getLastc(tmpName, '.'), "$.mid");
      } while (midiFile = fopen(tmpName, "r"));
      if (rename(outName, tmpName)) {
		fprintf(stderr, "***Error: Output file %s exists and cannot be renamed.\n",
			outName);
		PauseAndGo(1);  //exit(1);
      } else {
        fprintf(stderr, "Notice: Output file %s already exits.\n\tIt is renamed as %s\n",
          outName, tmpName);
      }
    }      
    if ((midiFile = fopen(outName,"wb")) == NULL) {
		fprintf(stderr, "***Error while opening output file %s:\n", outName);
		fprintf(stderr, "   %s\n", strerror(errno));
		PauseAndGo(1);  //exit(1);
    }    
    
    fprintf(stderr, "Writing output to %s ...  ", outName);    

    translate();
    
    fseek(midiFile, 0, SEEK_END); /* just to be sure */
    fprintf(stderr, "%ld bytes written.\n", ftell(midiFile));
    
    fclose(midiFile);
    fclose(mtxFile);
    PauseAndGo(0); //exit(0);
}

