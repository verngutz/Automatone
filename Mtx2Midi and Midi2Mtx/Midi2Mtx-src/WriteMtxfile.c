/*
 * March 2006, Ying-Da Lee
 * -Contains functions for writing to the text file,
 *  extracted from mf2t.c, by Piet van Oostrum
 */

#include <stdio.h>
#include <ctype.h>
#include <fcntl.h>
#include <windows.h>
#include "Midi2Mtx.h"

static int TrkNr;
static int TrksToDo;
static int Measure;
static int M0;
static int Beat;
static int Clicks;
static long T0;

/* >>> Functions for writing the text file */

static void prtime(void)
{
	if (times) {
		long m = (Mf_currtime-T0)/Beat;
		fprintf(mtxFile,"%ld:%ld:%ld ",
			m/Measure+M0, m%Measure, (Mf_currtime-T0)%Beat);
	} else
		fprintf(mtxFile,"%ld ",Mf_currtime);
}

static void prtext(unsigned char *p, int leng)
{
	int n, c;
	int pos = 25;

	fprintf(mtxFile,"\"");
	for ( n=0; n<leng; n++ ) {
		c = *p++;
		if (fold && pos >= fold) {
			fprintf(mtxFile,"\\\n\t");
			pos = 13;	/* tab + \xab + \ */
			if (c == ' ' || c == '\t') {
				putc('\\', mtxFile);
				++pos;
			}
		}
		switch (c) {
		case '\\':
		case '"':
			fprintf(mtxFile,"\\%c", c);
			pos += 2;
			break;
		case '\r':
			fprintf(mtxFile, "\\r");
			pos += 2;
			break;
		case '\n':
			fprintf(mtxFile,"\\n");
			pos += 2;
			break;
		case '\0':
			fprintf(mtxFile, "\\0");
			pos += 2;
			break;
		default:
			if (isprint(c)) {
				putc(c, mtxFile);
				++pos;
			} else {
				fprintf(mtxFile,"\\x%02x" , c);
				pos += 4;
			}
		}
	}
	fprintf(mtxFile,"\"\n");
}

static void prhex(unsigned char *p, int leng)
{
	int n;
	int pos = 25;

	for ( n=0; n<leng; n++,p++ ) {
		if (fold && pos >= fold) {
			fprintf(mtxFile, "\\\n\t%02x" , *p);
			pos = 14;	/* tab + ab + " ab" + \ */
		}
		else {
			fprintf(mtxFile," %02x" , *p);
			pos += 3;
		}
	}
	fprintf(mtxFile,"\n");

}

/* This should be the first function call for writing the MTX file */
void Mf_header(int format, int ntrks, int division)
{
	if (division & 0x8000) { /* SMPTE */
	    times = 0;		 /* Can't do beats */
	    fprintf(mtxFile,"MFile %d %d %d %d\n",format,ntrks,
	    			-((-(division>>8))&0xff), division&0xff);
	} else
	    fprintf(mtxFile,"MFile %d %d %d\n",format,ntrks,division);
	if (format > 2) {
		fprintf(stderr, "Can't deal with format %d files\n", format);
		PauseAndGo(1); // exit(1);
	}
	TrkNr = 0;
	Beat = Clicks = division;
	TrksToDo = ntrks;
	Measure = 4;
	M0 = T0 = 0;
}


void Mf_error(char *s)
{
	if (TrksToDo <= 0)
		fprintf(stderr,"Error: Garbage at end\n",s);
	else
	    fprintf(stderr,"Error: %s\n",s);
}

	
static char *mknote(int pitch)
{
	static char * Notes [] =
		{ "c", "c#", "d", "d#", "e", "f", "f#", "g",
		  "g#", "a", "a#", "b" };
	static char buf[5];
	if ( notes )
		sprintf(buf, "%s%d", Notes[pitch % 12], pitch/12);
	else
		sprintf(buf, "%d", pitch);
	return buf;
}

void Mf_trackstart(void)
{
	fprintf(mtxFile,"MTrk\n");
	TrkNr++;
}

void Mf_trackend(void)
{
	fprintf(mtxFile,"TrkEnd\n");
	--TrksToDo;
}

void Mf_noteon(int chan, int pitch, int vol)
{
	prtime();
	fprintf(mtxFile,Onmsg,chan+1,mknote(pitch),vol);
}

void Mf_noteoff(int chan, int pitch, int vol)
{
	prtime();
	fprintf(mtxFile,Offmsg,chan+1,mknote(pitch),vol);
}

void Mf_pressure(int chan, int pitch, int press)
{
	prtime();
	fprintf(mtxFile,PoPrmsg,chan+1,mknote(pitch),press);
}

void Mf_parameter(int chan, int control, int value)
{
	prtime();
	fprintf(mtxFile,Parmsg,chan+1,control,value);
}

void Mf_pitchbend(int chan, int lsb, int msb)
{
	prtime();
	fprintf(mtxFile,Pbmsg,chan+1,128*msb+lsb);
}

void Mf_program(int chan, int program)
{
	prtime();
	fprintf(mtxFile,PrChmsg,chan+1,program);
}

void Mf_chanpressure(int chan, int press)
{
	prtime();
	fprintf(mtxFile,ChPrmsg,chan+1,press);
}

void Mf_sysex(int leng, char * mess)
{
	prtime();
	fprintf(mtxFile,"SysEx");
	prhex(mess, leng);
}

void Mf_metamisc(int type, int leng, char *mess)
{
	prtime();
	fprintf(mtxFile,"Meta 0x%02x",type);
	prhex (mess, leng);
}

void Mf_sqspecific(int leng, char *mess)
{
	prtime();
	fprintf(mtxFile,"SeqSpec");
	prhex (mess, leng);
}

void Mf_text(int type, int leng, char *mess)
{
	static char *ttype[] = {
		NULL,
		"Text",		/* type=0x01 */
		"Copyright",	/* type=0x02 */
		"TrkName",
		"InstrName",	/* ...       */
		"Lyric",
		"Marker",
		"Cue",		/* type=0x07 */
		"Unrec"
	};
	int unrecognized = (sizeof(ttype)/sizeof(char *)) - 1;

	prtime();
	if ( type < 1 || type > unrecognized )
		fprintf(mtxFile,"Meta 0x%02x ",type);
	else if (type == 3 && TrkNr == 1)
		fprintf(mtxFile,"Meta SeqName ");
	else
		fprintf(mtxFile,"Meta %s ",ttype[type]);
	prtext (mess, leng);
}

void Mf_seqnum(int num)
{
	prtime();
	fprintf(mtxFile,"SeqNr %d\n",num);
}

void Mf_eot(void)
{
	prtime();
	fprintf(mtxFile,"Meta TrkEnd\n");
}

void Mf_keysig(int sf,int mi)
{
	prtime();
	fprintf(mtxFile,"KeySig %d %s\n", (sf>127?sf-256:sf), (mi?"minor":"major"));
}

void Mf_tempo(long tempo)
{
	prtime();
	fprintf(mtxFile,"Tempo %ld\n",tempo);
}

void Mf_timesig(int nn, int dd, int cc, int bb)
{
	int denom = 1;
	while ( dd-- > 0 )
		denom *= 2;
	prtime();
	fprintf(mtxFile,"TimeSig %d/%d %d %d\n",
		nn,denom,cc,bb);
	M0 += (Mf_currtime-T0)/(Beat*Measure);
	T0 = Mf_currtime;
	Measure = nn;
	Beat = 4 * Clicks / denom;
}

void Mf_smpte(int hr, int mn, int se, int fr, int ff)
{
	prtime();
	fprintf(mtxFile,"SMPTE %d %d %d %d %d\n",
		hr,mn,se,fr,ff);
}

void Mf_arbitrary(int leng, char *mess)
{
	prtime();
	fprintf(mtxFile,"Arb",leng);
	prhex (mess, leng);
}

/* <<< Functions for writing the text file */


