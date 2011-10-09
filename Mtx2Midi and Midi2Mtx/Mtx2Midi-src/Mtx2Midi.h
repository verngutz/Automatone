extern void WriteMidifile(int format,int ntracks,int division);
extern int Mf_writetrack(void);
extern void translate(void);


#define PauseAndGo(x)	\
	do { \
		fprintf(stderr, "Press <Enter> key to continue . . . "); \
		getchar(); \
		exit(x); \
	} while(0)


#define MTHD	256
#define MTRK	257
#define TRKEND	258

#define ON	note_on
#define OFF	note_off
#define POPR	poly_aftertouch
#define PAR	control_change
#define PB	pitch_wheel
#define PRCH	program_chng
#define CHPR	channel_aftertouch
#define SYSEX	system_exclusive

#define ARB	259
#define MINOR	260
#define MAJOR	261
	
#define CH	262
#define NOTE	263
#define VAL	264
#define CON	265
#define PROG	266

#define INT	267
#define STRING	268
#define STRESC	269
#define ERR	270
#define NOTEVAL 271
#define EOL	272

#define META	273
#define SEQSPEC	(META+1+sequencer_specific)
#define MTEXT	(META+1+text_event)
#define COPYRIGHT	(META+1+copyright_notice)
#define SEQNAME	(META+1+sequence_name)
#define INSTRNAME	(META+1+instrument_name)
#define LYRIC	(META+1+lyric)
#define MARKER	(META+1+marker)
#define CUE	(META+1+cue_point)
#define SEQNR	(META+1+sequence_number)
#define KEYSIG	(META+1+key_signature)
#define TEMPO	(META+1+set_tempo)
#define TIMESIG	(META+1+time_signature)
#define SMPTE	(META+1+smpte_offset)

/* MIDI status commands most significant bit is 1 */
#define note_off         	0x80
#define note_on          	0x90
#define poly_aftertouch  	0xa0
#define control_change    	0xb0
#define program_chng     	0xc0
#define channel_aftertouch      0xd0
#define pitch_wheel      	0xe0
#define system_exclusive      	0xf0
#define delay_packet	 	(1111)

/* 7 bit controllers */
#define damper_pedal            0x40
#define portamento	        0x41 	
#define sostenuto	        0x42
#define soft_pedal	        0x43
#define general_4               0x44
#define	hold_2		        0x45
#define	general_5	        0x50
#define	general_6	        0x51
#define general_7	        0x52
#define general_8	        0x53
#define tremolo_depth	        0x5c
#define chorus_depth	        0x5d
#define	detune		        0x5e
#define phaser_depth	        0x5f

/* parameter values */
#define data_inc	        0x60
#define data_dec	        0x61

/* parameter selection */
#define non_reg_lsb	        0x62
#define non_reg_msb	        0x63
#define reg_lsb		        0x64
#define reg_msb		        0x65

/* Standard MIDI Files meta event definitions */
#define	meta_event		0xFF
#define	sequence_number 	0x00
#define	text_event		0x01
#define copyright_notice 	0x02
#define sequence_name    	0x03
#define instrument_name 	0x04
#define lyric	        	0x05
#define marker			0x06
#define	cue_point		0x07
#define channel_prefix		0x20
#define	end_of_track		0x2f
#define	set_tempo		0x51
#define	smpte_offset		0x54
#define	time_signature		0x58
#define	key_signature		0x59
#define	sequencer_specific	0x7f

/* Manufacturer's ID number */
#define Seq_Circuits (0x01) /* Sequential Circuits Inc. */
#define Big_Briar    (0x02) /* Big Briar Inc.           */
#define Octave       (0x03) /* Octave/Plateau           */
#define Moog         (0x04) /* Moog Music               */
#define Passport     (0x05) /* Passport Designs         */
#define Lexicon      (0x06) /* Lexicon 			*/
#define Tempi        (0x20) /* Bon Tempi                */
#define Siel         (0x21) /* S.I.E.L.                 */
#define Kawai        (0x41) 
#define Roland       (0x42)
#define Korg         (0x42)
#define Yamaha       (0x43)

/* miscellaneous definitions */
#define MThd 0x4d546864L
#define MTrk 0x4d54726bL
#define lowerbyte(x) ((unsigned char)(x & 0xff))
#define upperbyte(x) ((unsigned char)((x & 0xff00)>>8))
