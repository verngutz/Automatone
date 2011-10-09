
#include <stdio.h>
#include <malloc.h>

#include "Mtx2midi.h"

#define NULLFUNC 0

/* Functions to implement in order to write a MIDI file */
extern int Mf_writetrack(void);

extern FILE *midiFile; 

long Mf_numbyteswritten;

/*
 * WriteMidifile() - The only fuction you'll need to call to write out
 *             a midi file.
 *
 * format      0 - Single multi-channel track
 *             1 - Multiple simultaneous tracks
 *             2 - One or more sequentially independent
 *                 single track patterns                
 * ntracks     The number of tracks in the file.
 * division    This is kind of tricky, it can represent two
 *             things, depending on whether it is positive or negative
 *             (bit 15 set or not).  If  bit  15  of division  is zero,
 *             bits 14 through 0 represent the number of delta-time
 *             "ticks" which make up a quarter note.  If bit  15 of
 *             division  is  a one, delta-times in a file correspond to
 *             subdivisions of a second similiar to  SMPTE  and  MIDI
 *             time code.  In  this format bits 14 through 8 contain
 *             one of four values - 24, -25, -29, or -30,
 *             corresponding  to  the  four standard  SMPTE and MIDI
 *             time code frame per second formats, where  -29
 *             represents  30  drop  frame.   The  second  byte
 *             consisting  of  bits 7 through 0 corresponds the the
 *             resolution within a frame.  Refer the Standard MIDI
 *             Files 1.0 spec for more details.
 *
 * Global variables:
 * midiFile: type (FILE *) of the output file
 * Mf_RunStat  if nonzero use running status.
 */ 
extern FILE *midiFile;
int Mf_RunStat = 0;
static int laststat;		/* last status code */
static int lastmeta;		/* last meta event type */

void 
WriteMidifile(format,ntracks,division) 
int format,ntracks,division; 

{
    int i; void mf_w_track_chunk(), mf_w_header_chunk();


    /* every MIDI file starts with a header */
    mf_w_header_chunk(format,ntracks,division);

    /* In format 1 files, the first track is a tempo map */
/*
    if(format == 1 && ( Mf_wtempotrack ))
    {
        mf_w_track_chunk(-1,Mf_wtempotrack);
	ntracks--;
    }
*/
    /* The rest of the file is a series of tracks */
    for(i = 0; i < ntracks; i++)
        mf_w_track_chunk(i);
}

void 
mf_w_track_chunk(which_track)
int which_track;
{
	unsigned long trkhdr,trklength;
	long offset, place_marker;
	void write16bit(),write32bit();
	
	trkhdr = MTrk;
	trklength = 0;

	/* Remember where the length was written, because we don't
	   know how long it will be until we've finished writing */
	offset = ftell(midiFile); 

#ifdef DEBUG
        printf("offset = %d\n",(int) offset);
#endif

	/* Write the track chunk header */
	write32bit(trkhdr);
	write32bit(trklength);

	Mf_numbyteswritten = 0L; /* the header's length doesn't count */
	laststat = 0;
	
	/* Note: this calls Mf_writetempotrack with an unused parameter (-1)
	   But this is innocent */

	Mf_writetrack();

	if (laststat != meta_event || lastmeta != end_of_track) {
	    /* mf_write End of track meta event */
	    eputc(0);
	    eputc(meta_event);
	    eputc(end_of_track);
	    eputc(0);
	}

	laststat = 0;
	 
	/* It's impossible to know how long the track chunk will be beforehand,
           so the position of the track length data is kept so that it can
           be written after the chunk has been generated */
	place_marker = ftell(midiFile);
	
	/* This method turned out not to be portable because the
           parameter returned from ftell is not guaranteed to be
           in bytes on every machine */
 	/* track.length = place_marker - offset - (long) sizeof(track); */

#ifdef DEBUG
printf("length = %d\n",(int) trklength);
#endif

 	if(fseek(midiFile,offset,0) < 0)
	    fprintf(stderr,"error seeking during final stage of write");

	trklength = Mf_numbyteswritten;

	/* Re-mf_write the track chunk header with right length */
	write32bit(trkhdr);
	write32bit(trklength);

	fseek(midiFile,place_marker,0);
} /* End gen_track_chunk() */


void 
mf_w_header_chunk(format,ntracks,division)
int format,ntracks,division;
{
    unsigned long ident,length;
    void write16bit(),write32bit();
    
    ident = MThd;           /* Head chunk identifier                    */
    length = 6;             /* Chunk length                             */

    /* individual bytes of the header must be written separately
       to preserve byte order across cpu types :-( */
    write32bit(ident);
    write32bit(length);
    write16bit(format);
    write16bit(ntracks);
    write16bit(division);
} /* end gen_header_chunk() */

void WriteVarLen();

/*
 * mf_w_midi_event()
 * 
 * Library routine to mf_write a single MIDI track event in the standard MIDI
 * file format. The format is:
 *
 *                    <delta-time><event>
 *
 * In this case, event can be any multi-byte midi message, such as
 * "note on", "note off", etc.      
 *
 * delta_time - the time in ticks since the last event.
 * type - the type of event.
 * chan - The midi channel.
 * data - A pointer to a block of chars containing the META EVENT,
 *        data.
 * size - The length of the midi-event data.
 */
int mf_w_midi_event(delta_time, type, chan, data, size)
unsigned long delta_time;
unsigned int chan,type;
unsigned long size;
unsigned char *data;
{
    int i;
    unsigned char c;

    WriteVarLen(delta_time);

    /* all MIDI events start with the type in the first four bits,
       and the channel in the lower four bits */
    c = type | chan;

    if(chan > 15)
        perror("error: MIDI channel greater than 16\n");

    if (!Mf_RunStat || laststat != c)
    	eputc(c);
	
    laststat = c;

    /* write out the data bytes */
    for(i = 0; i < size; i++)
        eputc(data[i]);

    return(size);
} /* end mf_write MIDI event */

/*
 * mf_w_meta_event()
 *
 * Library routine to mf_write a single meta event in the standard MIDI
 * file format. The format of a meta event is:
 *
 *          <delta-time><FF><type><length><bytes>
 *
 * delta_time - the time in ticks since the last event.
 * type - the type of meta event.
 * data - A pointer to a block of chars containing the META EVENT,
 *        data.
 * size - The length of the meta-event data.
 */
int
mf_w_meta_event(delta_time, type, data, size)
unsigned long delta_time;
unsigned char *data,type;
unsigned long size;
{
    int i;

    WriteVarLen(delta_time);
    
    /* This marks the fact we're writing a meta-event */
    eputc(meta_event);
    laststat = meta_event;

    /* The type of meta event */
    eputc(type);
    lastmeta = type;

    /* The length of the data bytes to follow */
    WriteVarLen(size); 

    for(i = 0; i < size; i++)
    {
	if(eputc(data[i]) != data[i])
	    return(-1); 
    }
    return(size);
} /* end mf_w_meta_event */

/*
 * mf_w_sysex_event()
 *
 * Library routine to mf_write a single sysex (or arbitrary)
 * event in the standard MIDI file format. The format of the event is:
 *
 *          <delta-time><type><length><bytes>
 *
 * delta_time - the time in ticks since the last event.
 * data - A pointer to a block of chars containing the EVENT data.
 *        The first byte is the type (0xf0 for sysex, 0xf7 otherwise)
 * size - The length of the sysex-event data.
 */
int
mf_w_sysex_event(delta_time, data, size)
unsigned long delta_time;
unsigned char *data;
unsigned long size;
{
    int i;

    WriteVarLen(delta_time);
    
    /* The type of sysex event */
    eputc(*data);
    laststat = 0;

    /* The length of the data bytes to follow */
    WriteVarLen(size-1); 

    for(i = 1; i < size; i++)
    {
	if(eputc(data[i]) != data[i])
	    return(-1); 
    }
    return(size);
} /* end mf_w_sysex_event */

void 
mf_w_tempo(delta_time, tempo)
unsigned long delta_time;
unsigned long tempo;
{
    /* Write tempo */
    /* all tempos are written as 120 beats/minute, */
    /* expressed in microseconds/quarter note     */

    WriteVarLen(delta_time);

    eputc(meta_event);
    laststat = meta_event;
    eputc(set_tempo);

    eputc(3);
    eputc((unsigned)(0xff & (tempo >> 16)));
    eputc((unsigned)(0xff & (tempo >> 8)));
    eputc((unsigned)(0xff & tempo));
}

/*
unsigned long 
mf_sec2ticks(secs,division,tempo)
int division;
unsigned int tempo;
float secs;
{    
     return (long)(((secs * 1000.0) / 4.0 * division) / tempo);
}
*/

/*
 * Write multi-length bytes to MIDI format files
 */
void 
WriteVarLen(value)
unsigned long value;
{
  unsigned long buffer;

  buffer = value & 0x7f;
  while((value >>= 7) > 0)
  {
	buffer <<= 8;
	buffer |= 0x80;
	buffer += (value & 0x7f);
  }
  while(1){
       eputc((unsigned)(buffer & 0xff));
       
	if(buffer & 0x80)
		buffer >>= 8;
	else
		return;
	}
}/* end of WriteVarLen */

/* 
 * This routine converts delta times in ticks into seconds. The
 * else statement is needed because the formula is different for tracks
 * based on notes and tracks based on SMPTE times.
 *
 */
/* Don't know why it's here: It's never called. */

/*
float 
mf_ticks2sec(ticks,division,tempo)
int division;
unsigned int tempo;
unsigned long ticks;
{
    float smpte_format, smpte_resolution;

    if(division > 0)
        return ((float) (((float)(ticks) * (float)(tempo)) / ((float)(division) * 1000000.0)));
    else
    {
       smpte_format = upperbyte(division);
       smpte_resolution = lowerbyte(division);
       return (float) ((float) ticks / (smpte_format * smpte_resolution * 1000000.0));
    }
} // end of ticks2sec()
*/

/*
 * write32bit()
 * write16bit()
 *
 * These routines are used to make sure that the byte order of
 * the various data types remains constant between machines. This
 * helps make sure that the code will be portable from one system
 * to the next.  It is slightly dangerous that it assumes that longs
 * have at least 32 bits and ints have at least 16 bits, but this
 * has been true at least on PCs, UNIX machines, and Macintosh's.
 *
 */
void 
write32bit(data)
unsigned long data;
{
    eputc((unsigned)((data >> 24) & 0xff));
    eputc((unsigned)((data >> 16) & 0xff));
    eputc((unsigned)((data >> 8 ) & 0xff));
    eputc((unsigned)(data & 0xff));
}

void 
write16bit(data)
int data;
{
    eputc((unsigned)((data & 0xff00) >> 8));
    eputc((unsigned)(data & 0xff));
}

/* write a single character and abort on error */
eputc(c)			
unsigned char c;
{
	int return_val;
	
	return_val = fputc(c, midiFile);

	if ( return_val == EOF ) {
       fprintf(stderr,"error writing");
		PauseAndGo(1);  //exit(1);
    }
		
	Mf_numbyteswritten++;
	return(return_val);
}
