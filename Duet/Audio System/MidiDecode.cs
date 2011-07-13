#region File Description
//-----------------------------------------------------------------------------
// MidiDecode.cs
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Duet.Audio_System
{
    //Enumeration for the Midi format style
    public enum MIDI_FORMAT
    {
        SINGLE_TRACK_FORMAT,    //Encompases a single track format
        MULTI_TRACK_FORMAT,     //Encompases a Multiple track format
        MULTI_SONG_FORMAT       //Encompases a Multiple Song format
    }
    //Midi Data Sent via midi event
    public enum MIDI_DATA_EVENTS
    {
        NONE,
        NOTE_ON,
        NOTE_OFF,
        NOTE_AFTERTOUCH,
        CONTROLLER,
        PROGRAM_CHANGE,
        CHANNEL_AFTERTOUCH,
        PITCH_BEND
    }
    //Midi Meta event data sent via midi event
    public enum MIDI_META_EVENTS
    {
        NONE,
        SEQUENCE_NUMBER,
     	TEXT_EVENT,
     	COPYRIGHT_NOTICE,
     	SEQUENCE_OR_TRACK_NAME,
     	INSTRUMENT_NAME,
     	LYRIC_TEXT,
     	MARKER_TEXT,
     	CUE_POINT,
        MIDI_CHANNEL_PREFIX_ASSIGNMENT,
        END_OF_TRACK,
        TEMPO_SETTING,
        SMPTE_OFFSET,
        TIME_SIGNATURE,
        KEY_SIGNATURE,
        SEQUENCER_SPECIFIC_EVENT
    }
    // Midi Timing type
    public enum MIDI_TIME_DIVISION
    {
        TICKS_PER_BEAT,
        FRAMES_PER_SECOND
    }

    //Structure for Midi Event data
    public class MIDI_EVENT_INFO
    {
        public UInt32 DeltaTime;
        public MIDI_META_EVENTS MetaEvents;
        public MIDI_DATA_EVENTS MidiDataEvents;
        public object[] Parameters = new object[5];
        //public object Parameters[0];
        //public object Parameters[1];
        //public object Parameters[2];
        //public object Parameters[3];
        //public object Parameters[4];
    }
    public class MIDI_EVENT_DATA
    {
        private double currTime;
        private ulong currEvent;
        //Midi Event Info
        private List<MIDI_EVENT_INFO> midiEvntInfo = new List<MIDI_EVENT_INFO>();

        public ulong NotesPlayed = 0;

        public double deltaTime = 0;
        public byte ChannelVolume;

        #region Properties
        public ulong CurrentEvent
        {
            get { return currEvent; }
            set { currEvent = value; }
        }
        public double CurrentTime
        {
            get { return currTime; }
            set { currTime = value; }
        }
        public List<MIDI_EVENT_INFO> MIDI_EVENT_INFO
        {
            get { return midiEvntInfo; }
        }

        #endregion

        public byte[] EventData     //Event Data
        {
            set
            {
                byte prevByte = 0;
                int index = 0;
                do
                {
                    MIDI_EVENT_INFO mei = new MIDI_EVENT_INFO();
                    UInt16 numofbytes = 0;
                    UInt32 ScrmbledDta = BitConverter.ToUInt32(value, index);
                    mei.DeltaTime = GetTime(ScrmbledDta, ref numofbytes); index += 4 - (4 - numofbytes);
                    byte statusByte = value[index];
                    if (statusByte < 0x80) { statusByte = prevByte; index--; }
                    if(statusByte != 0xFF) statusByte &= 0xF0;
                    
                    prevByte = statusByte;

                    switch (statusByte)
                    {
                        #region Midi Control Events

                        case 0x80:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_OFF; ++index;      //we have our midi event
                                mei.Parameters[0] = (byte)0;// value[index - 1] & 0x0F;
                                mei.Parameters[1] = value[index++];
                                mei.Parameters[2] = value[index++];
                            }
                            break;
                        case 0x90:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_ON; ++index;      //we have our midi event
                                mei.Parameters[0] = (byte)0;// (byte)(value[index - 1] & 0x0F);
                                mei.Parameters[1] = value[index++];
                                mei.Parameters[2] = value[index++];
                                if ((byte)mei.Parameters[2] == 0x00) mei.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_OFF;
                                NotesPlayed++;
                               /* int i = 0;
                                //Find out how big our data is
                                for (i = index; i < value.Length; i++) if (value[i] >= 0x80) break;

                                int SizeOfData = i - index + 1;
                                byte[] datStream = new byte[SizeOfData];
                                for (i = 1; i < datStream.Length; i++) datStream[i] = value[index + i - 1];
                                datStream[0] = value[index - 2];
                                mei.Parameters[1] = datStream;
                                index += datStream.Length - 1;

                                MIDI_EVENT_INFO mei2 = new MIDI_EVENT_INFO();
                                mei2.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_OFF;
                                //
                                numofbytes = 0;
                                ScrmbledDta = BitConverter.ToUInt32(value, index);
                                mei.DeltaTime = GetTime(ScrmbledDta, ref numofbytes); index += 4 - (4 - numofbytes);*/

                            }
                            break;
                        case 0xA0:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_AFTERTOUCH; ++index;      //we have our midi event
                                /* int i = 0;
                                 //Find out how big our data is
                                 for (i = index; i < value.Length; i++) if (value[i] >= 0x80) break;

                                 int SizeOfData = i - index + 1;
                                 byte[] datStream = new byte[SizeOfData];
                                 for (i = 1; i < datStream.Length; i++) datStream[i] = value[index + i - 1];
                                 datStream[0] = value[index - 2];
                                 mei.Parameters[1] = datStream;
                                 index += datStream.Length - 1;

                                 MIDI_EVENT_INFO mei2 = new MIDI_EVENT_INFO();
                                 mei2.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_OFF;
                                 //
                                 numofbytes = 0;
                                 ScrmbledDta = BitConverter.ToUInt32(value, index);
                                 mei.DeltaTime = GetTime(ScrmbledDta, ref numofbytes); index += 4 - (4 - numofbytes);*/

                            }
                            break;
                        case 0xB0:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.CONTROLLER;      //we have our midi event
                                mei.Parameters[0] = (byte)0; ++index;//(byte)(value[index++] & 0x0F);
                                mei.Parameters[1] = value[index++];
                                mei.Parameters[2] = value[index++];

                            }
                            break;
                        case 0xC0:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.PROGRAM_CHANGE;      //we have our midi event
                                mei.Parameters[0] = (byte)0; ++index;//(byte)(value[index++] & 0x0F);
                                mei.Parameters[1] = value[index++];
                            }
                            break;
                        case 0xD0:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.CHANNEL_AFTERTOUCH; ++index;      //we have our midi event
                               

                            }
                            break;
                        case 0xE0:
                            {
                                mei.MidiDataEvents = MIDI_DATA_EVENTS.PITCH_BEND; ++index;      //we have our midi event
                                /* int i = 0;
                                 //Find out how big our data is
                                 for (i = index; i < value.Length; i++) if (value[i] >= 0x80) break;

                                 int SizeOfData = i - index + 1;
                                 byte[] datStream = new byte[SizeOfData];
                                 for (i = 1; i < datStream.Length; i++) datStream[i] = value[index + i - 1];
                                 datStream[0] = value[index - 2];
                                 mei.Parameters[1] = datStream;
                                 index += datStream.Length - 1;

                                 MIDI_EVENT_INFO mei2 = new MIDI_EVENT_INFO();
                                 mei2.MidiDataEvents = MIDI_DATA_EVENTS.NOTE_OFF;
                                 //
                                 numofbytes = 0;
                                 ScrmbledDta = BitConverter.ToUInt32(value, index);
                                 mei.DeltaTime = GetTime(ScrmbledDta, ref numofbytes); index += 4 - (4 - numofbytes);*/

                            }
                            break;
                        #endregion
                        #region Midi Meta Events
                        //Meta Events
                        case 0xFF:
                            statusByte = value[++index];
                            switch (statusByte)
                            {
                                case 0x00: 
                                    mei.MetaEvents = MIDI_META_EVENTS.SEQUENCE_NUMBER; ++index; 
                                    break;
                                case 0x01:
                                    mei.MetaEvents = MIDI_META_EVENTS.TEXT_EVENT; ++index;
                                    //Get the length of the string
                                    mei.Parameters[0] = value[index++];
                                    //Set the Data to the correct variable
                                    mei.Parameters[1] = ASCIIEncoding.ASCII.GetString(value, index, ((int)value[index - 1])); index += (int)value[index - 1];
                                    break;
                                case 0x02:
                                    mei.MetaEvents = MIDI_META_EVENTS.COPYRIGHT_NOTICE; ++index; 
                                    //Get the length of the string
                                    mei.Parameters[0] = value[index++];
                                    //Set the Data to the correct variable
                                    mei.Parameters[1] = ASCIIEncoding.ASCII.GetString(value, index, ((int)value[index - 1])); index += (int)value[index - 1];
                                    break;
                                case 0x03:
                                    mei.MetaEvents = MIDI_META_EVENTS.SEQUENCE_OR_TRACK_NAME; ++index;
                                    //Get the length of the string
                                    mei.Parameters[0] = value[index++];
                                    //Set the Data to the correct variable
                                    mei.Parameters[1] = ASCIIEncoding.ASCII.GetString(value, index, ((int)value[index - 1])); index += (int)value[index - 1];

                                    break;
                                case 0x04:
                                    mei.MetaEvents = MIDI_META_EVENTS.INSTRUMENT_NAME; ++index;
                                    string IN = ASCIIEncoding.ASCII.GetString(value, index + 1, (int)value[index]); index += (int)value[index] + 1;

                                    //Get the Instrument Name
                                    mei.Parameters[0] = IN;

                                    break;
                                case 0x05:
                                    mei.MetaEvents = MIDI_META_EVENTS.LYRIC_TEXT; ++index;
                                    string LT = ASCIIEncoding.ASCII.GetString(value, index + 1, (int)value[index]); index += (int)value[index] + 1;

                                    //Get the Instrument Name
                                    mei.Parameters[0] = LT;

                                    break;
                                case 0x06:
                                    mei.MetaEvents = MIDI_META_EVENTS.MARKER_TEXT; ++index;
                                    string mT = ASCIIEncoding.ASCII.GetString(value, index + 1, (int)value[index]); index += (int)value[index] + 1;

                                    //Get the Instrument Name
                                    mei.Parameters[0] = mT;

                                    break;
                                case 0x20:
                                    mei.MetaEvents = MIDI_META_EVENTS.MIDI_CHANNEL_PREFIX_ASSIGNMENT; index++;
                                    //Get the length of the string
                                    mei.Parameters[0] = value[index++];
                                    //Set the Data to the correct variable
                                    mei.Parameters[1] = value[index++];

                                    break;
                                    //the end of the track
                                case 0x2F: mei.MetaEvents = MIDI_META_EVENTS.END_OF_TRACK; index += 2; break;

                                case 0x51:
                                    mei.MetaEvents = MIDI_META_EVENTS.TEMPO_SETTING; ++index;
                                    //Get the length of the data, don't see why it's so important tho :/
                                    mei.Parameters[4] = value[index++];
                                    //Get the data into an array
                                    byte[] mS = new byte[4]; for (int i = 0; i < 3; i++) mS[i + 1] = value[i + index]; index += 3;
                                    //Set it into a readable format
                                    byte[] mS2 = new byte[4]; for (int i = 0; i < 4; i++) mS2[3 - i] = mS[i];

                                    //Get the value from the array
                                    UInt32 Val = BitConverter.ToUInt32(mS2, 0);
                                    //Set the value
                                    mei.Parameters[0] = Val;
                                    break;

                                case 0x58:
                                    mei.MetaEvents = MIDI_META_EVENTS.TIME_SIGNATURE; ++index;
                                    //Get the length of the data, don't see why it's so important tho :/
                                    mei.Parameters[4] = value[index++];

                                    //First one is the time signature Numerator
                                    mei.Parameters[0] = value[index++];
                                    //Second one is the time signature Denominator
                                    mei.Parameters[1] = value[index++];
                                    //Third one is the time signature metranome
                                    mei.Parameters[2] = value[index++];
                                    //final one is the number of 32nd m_Notes per 24 midi clock signals
                                    mei.Parameters[3] = value[index++];

                                    break;
                                case 0x59:
                                    mei.MetaEvents = MIDI_META_EVENTS.KEY_SIGNATURE; ++index;
                                    
                                    //Get the length of the data, don't see why it's so important tho :/
                                    mei.Parameters[4] = value[index++];

                                    //Get Key
                                    mei.Parameters[0] = value[index++];
                                    //Get Scale
                                    mei.Parameters[1] = value[index++];
                                    break;
                                case 0x7F:
                                    //Sequencer specific events
                                    mei.MetaEvents = MIDI_META_EVENTS.SEQUENCER_SPECIFIC_EVENT; ++index;    //increment the indexer

                                    //Get the length of the data
                                    mei.Parameters[4] = value[index++];
                                    {
                                        //Get the byte length
                                        byte[] len = new byte[(int)value[index - 1]];
                                        //get the byte info
                                        for (int i = 0; i < len.Length; i++) { len[i] = value[index++]; }
                                    }
                                    break;
                                   
                            }

                            break;
                        #endregion
                    }
                    midiEvntInfo.Add(mei);
                } while (index < value.Length);
            }
        }
         //Get the delta time from the 32 Byte variable
        private UInt32 GetTime(UInt32 data,ref UInt16 numOfBytes)
        {
            //Setup a buffer to find data with
            byte[] buff = BitConverter.GetBytes(data); numOfBytes++;
            //Find the length of variable
            for (int i = 0; i < buff.Length; i++) { if ((buff[i] & 0x80) > 0) { numOfBytes++; } else { break; } }
            //Ignore the rest of the variables
            for (int i = numOfBytes; i < 4; i++) buff[i] = 0x00;
            buff = Cnvrt(buff);
            //Make a variable to to some bit shifting with
            data = BitConverter.ToUInt32(buff, 0);
            data >>= (32 - (numOfBytes * 8));
            UInt32 b = data;
            UInt32 bffr = (data & 0x7F);
            int c = 1;
            while ((data >>= 8) > 0)
            {
                bffr |= ((data & 0x7F) << (7 * c)); c++;
            }
           
            return bffr;
        }
        //Convert to the proper reading method
        private byte[] Cnvrt(byte[] data)
        {
            byte[] ret = new byte[data.Length]; //setup a return variable
            for (int i = 0; i < data.Length; i++) ret[i] = data[(data.Length - 1) - i];     //do our swap
            return ret; //return the var
        }

    }
    //Structure for Midi Header Data
    public class MIDI_HEADER_DATA
    {
        public Int32 Length;                    //Header Length
        public Int16 TrackCount;                //Track Count
        public Int16 DeltaTiming;               //Midi Track Timing
        public MIDI_FORMAT MidiFormat;          //Midi format
        public MIDI_TIME_DIVISION TimeFormat;   //Midi timing format
    }
    //Class for handling Midi Loading and decoding
    public class MidiDecoder
    {
        protected const ulong MICROSECONDS_PER_MINUTE = 60000000;
        //private ulong MICROSECS_PER_MIN;                            //Microsecs per minute
        private ulong m_BeatsPerMinute = 120;
        private ulong m_MicrosecsPerQuarterNote;
        //Initialize our private variables
        private MIDI_HEADER_DATA mhd = new MIDI_HEADER_DATA();      //Midi Header data structure initialized
        private MIDI_EVENT_DATA[] med = new MIDI_EVENT_DATA[0];      //Midi Event data structure initialized

        //Initialize our returnable Properties
        public MIDI_HEADER_DATA MidiHeaderData
        {
            get { return mhd; }
        }
        public MIDI_EVENT_DATA[] MidiEventData
        {
            get { return med; }
        }

        //Initialize some contructors
        public MidiDecoder() 
        {
            m_MicrosecsPerQuarterNote = MICROSECONDS_PER_MINUTE / m_BeatsPerMinute;
        }                                    //Just initialize some varables
        public MidiDecoder(string Midi) 
        {
            m_MicrosecsPerQuarterNote = MICROSECONDS_PER_MINUTE / m_BeatsPerMinute;
            LoadMidi(Midi); 
        }         //Initialize and load a midi

        //Microsecs
        public ulong MicrosecsPerQuarterNote
        {
            get { return m_MicrosecsPerQuarterNote; }
            set
            {
                m_MicrosecsPerQuarterNote = value;
                if (m_MicrosecsPerQuarterNote <= 0) m_MicrosecsPerQuarterNote = 1;
                m_BeatsPerMinute = MICROSECONDS_PER_MINUTE / m_MicrosecsPerQuarterNote;
            }
        }
        public ulong BeatsPerMinute
        {
            get { return m_BeatsPerMinute; }
            set
            {
                m_BeatsPerMinute = value;
                if (m_BeatsPerMinute <= 0) m_BeatsPerMinute = 1;
                m_MicrosecsPerQuarterNote = MICROSECONDS_PER_MINUTE / m_BeatsPerMinute;
            }
        }

        //Convert to the proper reading method
        private byte[] Cnvrt(byte[] data)
        {
            byte[] ret = new byte[data.Length]; //setup a return variable
            for (int i = 0; i < data.Length; i++) ret[i] = data[(data.Length - 1) - i];     //do our swap
            return ret; //return the var
        }
       
        //Some Public Functions
        public bool LoadMidi(string Midi)
        {
            //Load the midi for reading
            try
            {
                //Begin to read the file
                using (System.IO.FileStream fs = new System.IO.FileStream(Midi, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                {
                    byte[] tmp = new byte[4]; //Begin reading header

                    if (fs.Read(tmp, 0, 4) == -1) return false;     //read the block of data
                    if (ASCIIEncoding.ASCII.GetString(tmp) != "MThd") return false;     //if not a midi return false

                    if (fs.Read(tmp, 0, 4) == -1) return false;     //read the block of data
                    mhd.Length = BitConverter.ToInt32(Cnvrt(tmp), 0);      //Set the header length

                    tmp = new byte[2];                              //set a new buffer size
                    if (fs.Read(tmp, 0, 2) == -1) return false;     //read the block of data
                    int intTmp = BitConverter.ToInt16(Cnvrt(tmp), 0);      //Get the midi format type

                    //Set the new header data now
                    if (intTmp == 0) mhd.MidiFormat = MIDI_FORMAT.SINGLE_TRACK_FORMAT;
                    if (intTmp == 1) mhd.MidiFormat = MIDI_FORMAT.MULTI_TRACK_FORMAT;
                    if (intTmp == 2) mhd.MidiFormat = MIDI_FORMAT.MULTI_SONG_FORMAT;

                    if (fs.Read(tmp, 0, 2) == -1) return false;     //read the block of data
                    mhd.TrackCount = BitConverter.ToInt16(Cnvrt(tmp), 0);  //Set the track count
                    med = new MIDI_EVENT_DATA[mhd.TrackCount];      //Set up buffer space for the track event data

                    if (fs.Read(tmp, 0, 2) == -1) return false;     //read the block of data
                    short deltaTime = BitConverter.ToInt16(Cnvrt(tmp), 0);  //Set the Timing
                    mhd.DeltaTiming = (short)(deltaTime & 0x7FFF);
                    // Process Timing Info
                    mhd.TimeFormat = ((deltaTime & 0x8000)>0)?MIDI_TIME_DIVISION.FRAMES_PER_SECOND:MIDI_TIME_DIVISION.TICKS_PER_BEAT;

                    //Begin Looping through all the tracks
                    for (int i = 0; i < mhd.TrackCount; i++)
                    {
                        tmp = new byte[4];      //reset the size again
                        if (fs.Read(tmp, 0, 4) == -1) return false;     //if read error exit
                        if (ASCIIEncoding.ASCII.GetString(tmp) != "MTrk") return false; //make sure we haven't screwed up
                        med[i] = new MIDI_EVENT_DATA();                 //if we made it this far then lets intialize a new instance

                        if (fs.Read(tmp, 0, 4) == -1) return false;     //if read error exit
                        int byteLength = BitConverter.ToInt32(Cnvrt(tmp), 0);  //Set the length of the event data
                        
                        tmp = new byte[byteLength];                     //Reset the tmp variable
                        if (fs.Read(tmp, 0, byteLength) == -1) return false;
                        
                      /*  byte[] dt = new byte[4];
                        for (int c = 0; c < 4; c++) dt[c] = tmp[c];

                        UInt16 byteCount = 0;
                        med[i].DeltaTime = GetTime(dt, ref byteCount);*/

                        med[i].EventData = tmp;
                       // med[i].EventData = eventData;                         //Copy bytes into event data;

                    }
                    fs.Close();
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);         //note the error
                return false;                           //Exit
            }
            return true;    //if everythings ok then let's exit
        }
    }
}
