/* $Id: midifile.c,v 1.4 1991/11/17 21:57:26 piet Rel piet $ */

/*
 * ReadMidifle.c - March 2006, Ying-Da Lee
 * -Contains only functions needed for reading Midi files.
 * -All indirect references of functions are eleiminated.
 */
 
 /*
 * midifile 1.11
 * 
 * Read and write a MIDI file.  Externally-assigned function pointers are 
 * called upon recognizing things in the file.
 *
 * Original release by Tim Thompson, tjt@twitch.att.com
 *
 * June 1989 - Added writing capability, M. Czeiszperger.
 *
 * Oct 1991 - Modifications by Piet van Oostrum <piet@cs.ruu.nl>:
 *	Changed identifiers to be 7 char unique.
 *	Added sysex write capability (mf_w_sysex_event)
 *	Corrected a bug in writing of tempo track
 *	Added code to implement running status on write
 *	Added check for meta end of track insertion
 *	Added a couple of include files to get proper int=short compilation
 *
 * Nov 1991 - Piet van Oostrum <piet@cs.ruu.nl>
 *	mf_w_tempo needs a delta time parameter otherwise the tempo cannot
 *      be changed during the piece.
 *
 * Apr 1993 - Piet van Oostrum <piet@cs.ruu.nl>
 *	decl of malloc replaced by #include <malloc.h>
 *	readheader() declared void.
 *
 * Aug 1993 - Piet van Oostrum <piet@cs.ruu.nl>
 *	sequencer_specific in midifile.h was wrong
 *
 *          The file format implemented here is called
 *          Standard MIDI Files, and is part of the Musical
 *          instrument Digital Interface specification.
 *          The spec is avaiable from:
 *
 *               International MIDI Association
 *               5316 West 57th Street
 *               Los Angeles, CA 90056
 *
 *          An in-depth description of the spec can also be found
 *          in the article "Introducing Standard MIDI Files", published
 *          in Electronic Musician magazine, April, 1989.
 *
 */
 
 
#include <stdlib.h>
#include <stdio.h>
#include <malloc.h>

#include "Midi2Mtx.h"
/*
 * See Midi2Mtx.h for functions (all having prefix Mf_) that
 * must be aupplied before calling ReadMidifile.
 */

#define NULLFUNC 0

int Mf_nomerge = 0;		/* 1 => continue'ed system exclusives are */
				            /* not collapsed. */
long Mf_currtime = 0L;		/* current time in delta-time units */

/* private stuff */
static long Mf_toberead = 0L;
static long Mf_numbyteswritten = 0L;

static long readvarinum(void);
static long read32bit(void);
static long to32bit(int c1, int c2, int c3, int c4);
static int read16bit(void);
static int to16bit(int c1, int c2);
static char *msg(void);
static void readheader(void);
static int readtrack(void);
static void badbyte(int c);
static void metaevent(int type);
static void chanmessage(int status, int c1, int c2);
static void msginit(void);
static int msgleng(void);
static void msgadd(int c);
static void biggermsg(void);
       
void ReadMidifile(void)
/*
 * Read MIDI contents from MidiFile and write to MTX file using
 * Mf_... function.
 * See Midi2Mtx.h for functions (all having prefix Mf_) that
 * must be supplied before calling ReadMidifile.
 */
{

	readheader();
	while ( readtrack() )
		;
}

static int mferror(char *s)
{
	Mf_error(s);
	PauseAndGo(1); //exit(1)
}


static int readmt(char *s) /* read through the "MThd" or "MTrk" header string */
{
	int n = 0;
	char *p = s;
	int c;

	while ( n++<4 && (c=fgetc(midiFile)) != EOF ) {
		if ( c != *p++ ) {
			char buff[32];
			(void) strcpy(buff,"expecting ");
			(void) strcat(buff,s);
			mferror(buff);
		}
	}
	return(c);
}

static int egetc(void)	/* read a single character and abort on EOF */
{
	int c = fgetc(midiFile);

	if ( c == EOF )
		mferror("premature EOF");
	Mf_toberead--;
	return(c);
}

static void readheader(void)		/* read a header chunk */
{
	int format, ntrks, division;

	if ( readmt("MThd") == EOF )
		return;

	Mf_toberead = read32bit();
	format = read16bit();
	ntrks = read16bit();
	division = read16bit();
	Mf_header(format,ntrks,division);

	/* flush any extra stuff, in case the length of header is not 6 */
	while ( Mf_toberead > 0 )
		(void) egetc();
}

static int readtrack(void)		 /* read a track chunk */
{
	/* This array is indexed by the high half of a status byte.  It's */
	/* value is either the number of bytes needed (1 or 2) for a channel */
	/* message, or 0 (meaning it's not  a channel message). */
	static int chantype[] = {
		0, 0, 0, 0, 0, 0, 0, 0,		/* 0x00 through 0x70 */
		2, 2, 2, 2, 1, 1, 2, 0		/* 0x80 through 0xf0 */
	};
	long lookfor;
	int c, c1, type;
	int sysexcontinue = 0;	/* 1 if last message was an unfinished sysex */
	int running = 0;	/* 1 when running status used */
	int status = 0;		/* status value (e.g. 0x90==note-on) */
	int needed;

	if ( readmt("MTrk") == EOF )
		return(0);

	Mf_toberead = read32bit();
	Mf_currtime = 0;
	Mf_trackstart();
	while ( Mf_toberead > 0 ) {

		Mf_currtime += readvarinum();	/* delta time */

		c = egetc();

		if ( sysexcontinue && c != 0xf7 )
			mferror("didn't find expected continuation of a sysex");

		if ( (c & 0x80) == 0 ) {	 /* running status? */
			if ( status == 0 )
				mferror("unexpected running status");
			running = 1;
			c1 = c;
			c = status;
		}
		else if (c < 0xf0) {
			status = c;
			running = 0;
		}

		needed = chantype[ (c>>4) & 0xf ];

		if ( needed ) {		/* ie. is it a channel message? */

			if ( !running )
				c1 = egetc();
			chanmessage( status, c1, (needed>1) ? egetc() : 0 );
			continue;;
		}

		switch ( c ) {

		case 0xff:			/* meta event */

			type = egetc();
			lookfor = Mf_toberead - readvarinum();
			msginit();

			while ( Mf_toberead > lookfor )
				msgadd(egetc());

			metaevent(type);
			break;

		case 0xf0:		/* start of system exclusive */

			lookfor = Mf_toberead - readvarinum();
			msginit();
			msgadd(0xf0);

			while ( Mf_toberead > lookfor )
				msgadd(c=egetc());

			if ( c==0xf7 || Mf_nomerge==0 )
				Mf_sysex(msgleng(),msg());
			else
				sysexcontinue = 1;  /* merge into next msg */
			break;

		case 0xf7:	/* sysex continuation or arbitrary stuff */

			lookfor = Mf_toberead - readvarinum();

			if ( ! sysexcontinue )
				msginit();

			while ( Mf_toberead > lookfor )
				msgadd(c=egetc());

			if ( ! sysexcontinue ) {
				Mf_arbitrary(msgleng(),msg());
			}
			else if ( c == 0xf7 ) {
				Mf_sysex(msgleng(),msg());
				sysexcontinue = 0;
			}
			break;
		default:
			badbyte(c);
			break;
		}
	}
	Mf_trackend();
	return(1);
}

static void badbyte(int c)
{
	char buff[32];

	(void) sprintf(buff,"unexpected byte: 0x%02x",c);
	mferror(buff);
}

static void metaevent(int type)
{
	int leng = msgleng();
	char *m = msg();

	switch  ( type ) {
	case 0x00:
		Mf_seqnum(to16bit(m[0],m[1]));
		break;
	case 0x01:	/* Text event */
	case 0x02:	/* Copyright notice */
	case 0x03:	/* Sequence/Track name */
	case 0x04:	/* Instrument name */
	case 0x05:	/* Lyric */
	case 0x06:	/* Marker */
	case 0x07:	/* Cue point */
	case 0x08:
	case 0x09:
	case 0x0a:
	case 0x0b:
	case 0x0c:
	case 0x0d:
	case 0x0e:
	case 0x0f:
		/* These are all text events */
		Mf_text(type,leng,m);
		break;
	case 0x2f:	/* End of Track */
		Mf_eot();
		break;
	case 0x51:	/* Set tempo */
		Mf_tempo(to32bit(0,m[0],m[1],m[2]));
		break;
	case 0x54:
		Mf_smpte(m[0],m[1],m[2],m[3],m[4]);
		break;
	case 0x58:
		Mf_timesig(m[0],m[1],m[2],m[3]);
		break;
	case 0x59:
		Mf_keysig(m[0],m[1]);
		break;
	case 0x7f:
		Mf_sqspecific(leng,m);
		break;
	default:
		Mf_metamisc(type,leng,m);
	}
}

/*
static sysex()
{
	Mf_sysex(msgleng(),msg());
}
*/

static void chanmessage(int status, int c1, int c2)
{
	int chan = status & 0xf;

	switch ( status & 0xf0 ) {
	case 0x80:
		Mf_noteoff(chan,c1,c2);
		break;
	case 0x90:
		Mf_noteon(chan,c1,c2);
		break;
	case 0xa0:
		Mf_pressure(chan,c1,c2);
		break;
	case 0xb0:
		Mf_parameter(chan,c1,c2);
		break;
	case 0xe0:
		Mf_pitchbend(chan,c1,c2);
		break;
	case 0xc0:
		Mf_program(chan,c1);
		break;
	case 0xd0:
		Mf_chanpressure(chan,c1);
		break;
	}
}

/* readvarinum - read a varying-length number, and return the */
/* number of characters it took. */

static long readvarinum(void)
{
	long value;
	int c;

	c = egetc();
	value = c;
	if ( c & 0x80 ) {
		value &= 0x7f;
		do {
			c = egetc();
			value = (value << 7) + (c & 0x7f);
		} while (c & 0x80);
	}
	return (value);
}

static long to32bit(int c1, int c2, int c3, int c4)
{
	long value = 0L;

	value = (c1 & 0xff);
	value = (value<<8) + (c2 & 0xff);
	value = (value<<8) + (c3 & 0xff);
	value = (value<<8) + (c4 & 0xff);
	return (value);
}

static int to16bit(int c1, int c2)
{
	return ((c1 & 0xff ) << 8) + (c2 & 0xff);
}

static long read32bit(void)
{
	int c1, c2, c3, c4;

	c1 = egetc();
	c2 = egetc();
	c3 = egetc();
	c4 = egetc();
	return to32bit(c1,c2,c3,c4);
}

static int read16bit(void)
{
	int c1, c2;
	c1 = egetc();
	c2 = egetc();
	return to16bit(c1,c2);
}


/* The code below allows collection of a system exclusive message of */
/* arbitrary length.  The Msgbuff is expanded as necessary.  The only */
/* visible data/routines are msginit(), msgadd(), msg(), msgleng(). */

#define MSGINCREMENT 128
static char *Msgbuff = NULL;	/* message buffer */
static int Msgsize = 0;		/* Size of currently allocated Msg */
static int Msgindex = 0;	/* index of next available location in Msg */

static void msginit(void)
{
	Msgindex = 0;
}

static char *msg(void)
{
	return(Msgbuff);
}

static int msgleng(void)
{
	return(Msgindex);
}

static void msgadd(int c)
{
	/* If necessary, allocate larger message buffer. */
	if ( Msgindex >= Msgsize )
		biggermsg();
	Msgbuff[Msgindex++] = c;
}

static void biggermsg(void)
{
	char *newmess;
	char *oldmess = Msgbuff;
	int oldleng = Msgsize;

	Msgsize += MSGINCREMENT;
	newmess = (char *) malloc( (unsigned)(sizeof(char)*Msgsize) );

	if(newmess == NULL)
		mferror("malloc error!");
		
	/* copy old message into larger new one */
	if ( oldmess != NULL ) {
		register char *p = newmess;
		register char *q = oldmess;
		register char *endq = &oldmess[oldleng];

		for ( ; q!=endq ; p++,q++ )
			*p = *q;
		free(oldmess);
	}
	Msgbuff = newmess;
}

