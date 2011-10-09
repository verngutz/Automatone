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

static char data[5];
static int chan;
static err_continue =0;
static int Measure, Beat, Clicks, M0;
static long T0;

static char* buffer = 0;
static int bufsiz = 0, buflen;

extern long yyval;
extern int yyleng;
extern int lineno;
extern char *yytext;
extern int do_hex;
extern int eol_seen;

#ifdef NO_YYLENG_VAR
#define	yyleng yylength
#endif

static jmp_buf erjump;
int Mf_writetrack(void);
static void checkchan();
static void checknote();
static void checkval();
static void splitval();
static void get16val();
static void checkcon();
static void checkprog();
static void checkeol();
static void gethex();


static void prs_error(s)
char *s;
{
    int c;
    int count;
    int ln = (eol_seen? lineno-1 : lineno);
    fprintf (stderr, "%d: %s\n", ln, s);
    if (yyleng > 0 && *yytext != '\n')
        fprintf (stderr, "*** %*s ***\n", yyleng, yytext);
    count = 0;
    while (count < 100 && 
	   (c=yylex()) != EOL && c != EOF) count++/* skip rest of line */;
    if (c == EOF) {
        PauseAndGo(1);  //exit(1);
    }       
    if (err_continue)
        longjmp(erjump,1);
}


int Mf_writetrack(void)
{
    int opcode, c;
    long currtime = 0;
    long newtime, delta;
    int i, k;
    
    while ((opcode = yylex()) == EOL) 
		;
    if (opcode != MTRK)
        prs_error("Missing MTrk");
	    checkeol();
    while (1) {
		err_continue = 1;
		setjmp (erjump);
        switch (yylex()) {
        case MTRK:
            prs_error("Unexpected MTrk");
		case EOF:
			err_continue = 0;
			fprintf(stderr, "Unexpected EOF");
			return -1;
		case TRKEND:
			err_continue = 0;
			checkeol();
            return 1;
		case INT:
			newtime = yyval;
			if ((opcode=yylex())=='/') {
				if (yylex()!=INT) 
					prs_error("Illegal time value");
				newtime = (newtime-M0)*Measure+yyval;
				if (yylex()!='/'||yylex()!=INT) 
					prs_error("Illegal time value");
				newtime = T0 + newtime*Beat + yyval;
				opcode = yylex();
			} 
			delta = newtime - currtime;
            switch (opcode) {
            case ON:
            case OFF:
            case POPR:
				checkchan();
				checknote();
				checkval();
				mf_w_midi_event (delta, opcode, chan, data, 2L);
				break;

            case PAR:
				checkchan();
				checkcon();
				checkval();
				mf_w_midi_event (delta, opcode, chan, data, 2L);
				break;
		
            case PB:
				checkchan();
				splitval();
				mf_w_midi_event (delta, opcode, chan, data, 2L);
				break;

            case PRCH:
				checkchan();
				checkprog();
				mf_w_midi_event (delta, opcode, chan, data, 1L);
				break;
                
			case CHPR:
				checkchan();
				checkval();
				data[0] = data[1];
				mf_w_midi_event (delta, opcode, chan, data, 1L);
				break;
	    	
            case SYSEX:
            case ARB:
				gethex();
				mf_w_sysex_event (delta, buffer, (long)buflen);
				break;
		
			case TEMPO:
				if (yylex() != INT)
					prs_error("Syntax error");
				mf_w_tempo (delta, yyval);
				break;
		
            case TIMESIG: {
				int nn, denom, cc, bb;
				if (yylex() != INT || yylex() != '/') 
					prs_error("Syntax error");
				nn = yyval;
				denom = getbyte("Denom");
				cc = getbyte("clocks per click");
				bb = getbyte("32nd notes per 24 clocks");
				for (i=0, k=1 ; k<denom; i++, k<<=1);
				if (k!=denom) 
					fprintf(stderr,"Illegal TimeSig");
				data[0] = nn;
				data[1] = i;
				data[2] = cc;
				data[3] = bb;
				M0 += (newtime-T0)/(Beat*Measure);
				T0 = newtime;
				Measure = nn;
				Beat = 4 * Clicks / denom;
				mf_w_meta_event (delta, time_signature, data, 4L);
				} //case TIMESIG
				break;
		
            case SMPTE:
				for (i=0; i<5; i++) {
					data[i] = getbyte("SMPTE");
				}
				mf_w_meta_event (delta, smpte_offset, data, 5L);
				break;
		
            case KEYSIG:
				data[0] = i = getint ("Keysig");
				if (i<-7 || i>7)
					fprintf(stderr, "Key Sig must be between -7 and 7");
				if ((c=yylex()) != MINOR && c != MAJOR)
					prs_error("Syntax error");
				data[1] = (c == MINOR);
				mf_w_meta_event (delta, key_signature, data, 2L);
				break;
		
            case SEQNR:
				get16val ("SeqNr");
				mf_w_meta_event (delta, sequence_number, data, 2L);
				break;

			case META: {
				int type = yylex();
				switch (type) {
				case TRKEND:
					type = end_of_track;
					break;
				case MTEXT:
				case COPYRIGHT:
				case SEQNAME:
				case INSTRNAME:
				case LYRIC:
				case MARKER:
				case CUE:
					type -= (META+1);
					break;
				case INT:
					type = yyval;
					break;
				default:
					prs_error ("Illegal Meta type");
				} // switch (type)
				if (type == end_of_track)
					buflen = 0;
				else
					gethex();
				mf_w_meta_event (delta, type, buffer, (long)buflen);
				break;
				} // META
            case SEQSPEC:
				gethex();
				mf_w_meta_event (delta, sequencer_specific, buffer, (long)buflen);
				break;
			default:
				prs_error ("Unknown input");
				break;
			} // switch(opcode)
			// still in case INT
			currtime = newtime;
		case EOL:
			break;
		default:
			prs_error ("Unknown input");
			break;
		} // switch(yylex())
		checkeol();
    } // while(1)
}

getbyte(mess)
char *mess;
{
    getint (mess);
    if (yyval < 0 || yyval > 127) {
		fprintf(stderr, "Wrong value (%ld) for %s", yyval, mess);
		yyval = 0;
    }
    return yyval;
}

getint(mess)
char *mess;
{
    char ermesg[100];
    if (yylex() != INT) {
		fprintf(stderr, "Integer expected for %s", mess);
		yyval = 0;
    }
    return yyval;
}

static void checkchan()
{
    if (yylex() != CH || yylex() != INT) 
		prs_error("Syntax error");
    if (yyval < 1 || yyval > 16)
		fprintf(stderr,"Chan must be between 1 and 16");
    chan = yyval-1;
}

static void checknote()
{
    int c;
    if (yylex() != NOTE || ((c=yylex()) != INT && c != NOTEVAL))
		prs_error("Syntax error");
    if (c == NOTEVAL) {
        static int notes[] = {
	    9,		/* a */
	    11,		/* b */
	    0,		/* c */
	    2,		/* d */
	    4,		/* e */
	    5,		/* f */
	    7		/* g */
        };
		char *p = yytext;
        c = *p++;
        if (isupper(c)) 
			c = tolower(c);
        yyval = notes[c-'a'];
        switch (*p) {
        case '#':
        case '+':
       	    yyval++;
			p++;
			break;
		case 'b':
		case 'B':
		case '-':
			yyval--;
			p++;
			break;
		} // switch (*p)	
		yyval += 12 * atoi(p);
    } // if (c == NOTEVAL)
    if (yyval < 0 || yyval > 127)
		fprintf(stderr, "Note must be between 0 and 127");
    data[0] = yyval;
}

static void checkval()
{
    if (yylex() != VAL || yylex() != INT)
		prs_error("Syntax error");
    if (yyval < 0 || yyval > 127)
		fprintf(stderr,"Value must be between 0 and 127");
    data[1] = yyval;
}

static void splitval()
{
    if (yylex() != VAL || yylex() != INT)
		prs_error("Syntax error");
    if (yyval < 0 || yyval > 16383)
		fprintf(stderr, "Value must be between 0 and 16383");
    data[0] = yyval%128;
    data[1] = yyval/128;
}

static void get16val()
{
    if (yylex() != VAL || yylex() != INT)
		prs_error("Syntax error");
    if (yyval < 0 || yyval > 65535)
		fprintf(stderr,"Value must be between 0 and 65535");
    data[0] = (yyval>>8)&0xff;
    data[1] = yyval&0xff;
}

static void checkcon()
{
    if (yylex() != CON || yylex() != INT)
		prs_error("Syntax error");
    if (yyval < 0 || yyval > 127)
		fprintf(stderr,"Controller must be between 0 and 127");
    data[0] = yyval;
}

static void checkprog()
{
    if (yylex() != PROG || yylex() != INT) 
		prs_error("Syntax error");
    if (yyval < 0 || yyval > 127)
		fprintf(stderr,"Program number must be between 0 and 127");
    data[0] = yyval;
}

static void checkeol()
{
    if (eol_seen) return;
    if (yylex() != EOL) {
    	prs_error ("Garbage deleted");
	while (!eol_seen) yylex();	 /* skip rest of line */
    }
}

static void gethex()
{
    int c;
    buflen = 0;
    do_hex = 1;
    c = yylex();
    if (c==STRING) {
	/* Note: yytext includes the trailing, but not the starting quote */
		int i = 0;
    	if (yyleng-1 > bufsiz) {
			bufsiz = yyleng-1;
			if (buffer)
				buffer = realloc (buffer, bufsiz);
			else
				buffer = malloc (bufsiz);
			if (! buffer) 
				fprintf(stderr,"Out of memory");
		}
		while (i<yyleng-1) {
			c = yytext[i++];
rescan:
			if (c == '\\') {
				switch (c = yytext[i++]) {
				case '0':
					c = '\0';
					break;
				case 'n':
					c = '\n';
					break;
				case 'r':
					c = '\r';
					break;
				case 't':
					c = '\t';
					break;
				case 'x':
					if (sscanf (yytext+i, "%2x", &c) != 1)
						prs_error ("Illegal \\x in string");
					i += 2;
					break;
				case '\r':
				case '\n':
					while ((c=yytext[i++])==' '||c=='\t'||c=='\r'||c=='\n')
					/* skip whitespace */;
					goto rescan; /* sorry EWD :=) */
				} // switch (c = yytext[i++])
			} // if (c =='\\')
			buffer[buflen++] = c;
		} // while (i<yyleng-1)	    
    } // if (c==STRING)
    else if (c == INT) {
		do {
    	    if (buflen >= bufsiz) {
				bufsiz += 128;
				if (buffer)
					buffer = realloc (buffer, bufsiz);
				else
					buffer = malloc (bufsiz);
				if (! buffer) fprintf(stderr,"Out of memory");
			} //if (buflen >= bufsiz)
/* This test not applicable for sysex
			if (yyval < 0 || yyval > 127)
				error ("Illegal hex value");
*/
			buffer[buflen++] = yyval;
			c = yylex();
		} while (c == INT);
		if (c != EOL) prs_error ("Unknown hex input");
    } // else if (c == INT)
    else 
		prs_error ("String or hex input expected");
} // static void gethex()

long bankno (s, n)
char *s; int n;
{
    long res = 0;
    int c;
    while (n-- > 0) {
        c = (*s++);
		if (islower(c))
			c -= 'a';
		else if (isupper(c))
			c -= 'A';
		else
			c -= '1';
			res = res * 8 + c;
    }
    return res;
}

void translate(void)
{
	int Format, Ntrks;

//    TrkNr = 0;
    Measure = 4;
    Beat = 96;
    Clicks = 96;
    M0 = 0;
    T0 = 0;

    if (yylex()==MTHD) {
        Format = getint("MFile format");
        Ntrks = getint("MFile #tracks");
        Clicks = getint("MFile Clicks");

		if (Clicks < 0)
			Clicks = (Clicks&0xff)<<8|getint("MFile SMPTE division");

		checkeol();
	    WriteMidifile(Format, Ntrks, Clicks);
    } else {
        fprintf (stderr, "Missing MFile - can't continue\n");
        PauseAndGo(1);  //exit(1);
    }
}
