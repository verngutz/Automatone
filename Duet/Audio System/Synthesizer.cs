#region File Description
//-----------------------------------------------------------------------------
// Synthesizer.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Duet.Audio_System
{
    public class Synthesizer : IMidiDevice
    {
        public class _MidiNoteInfo
        {
            public Cue NoteCue;         //Note Cue of the midi
            public byte Channel;        //Channel that it's on
            public byte Note;           //Note value
            public byte Velocity;       //Note Velocity
            public double AttackVar;     //Tracker for the Attack Volume
            public double ReleaseVar;    //Tracker for the Release Volume
            public bool IsAttacking;    //Did note start playing
            public bool IsReleasing;    //Did note start releasing
        }

        //protected bool[] m_NoteStatus;
        protected List<_MidiNoteInfo> m_Notes = new List<_MidiNoteInfo>();
        protected _InstrumentInfo[] m_ChannelInfo = new _InstrumentInfo[16];

        private WaveBank m_WaveBank;
        private SoundBank m_SoundBank;
        public XACTPatch m_Patch;

        private IAudioSystemService m_AudioSystem;
        
        // Message Handlers
        MidiEventHandler noteOnHandler;

        // Overload routing
        protected override void OnSetOutput(IMidiDevice sender)
        {
            // HAHA!  This is so beautiful!
            sender.NoteOn += new MidiEventHandler(this.OnNoteOn);
            sender.NoteOff += new MidiEventHandler(this.OnNoteOff);
            sender.ProgramChange += new MidiEventHandler(this.OnProgramChange);

            base.OnSetOutput(sender);
        }

        #region Delegates
        // These correspond to the type of events that this class of MIDI devices can process
        // They can be implemented here or in a base class.  
        // If the later, then the delegate will be instantiated in the child.
        
        #endregion

        #region Properties
        public String WaveBankPath
        {
            get
            {
                return m_WaveBank.ToString();
            }
            set
            {
                m_WaveBank = new WaveBank(m_AudioSystem.XAudioEngine, value);
            }
        }

        public String SoundBankPath
        {
            get
            {
                return m_SoundBank.ToString();
            }
            set
            {
                m_SoundBank = new SoundBank(m_AudioSystem.XAudioEngine, value);
            }
        }

        public _InstrumentInfo[] ChannelInfo
        {
            get
            {
                return m_ChannelInfo;
            }
        }
        #endregion

        
        public Synthesizer(Game game)
            : base(game)
        {
            m_AudioSystem = (IAudioSystemService)game.Services.GetService(typeof(IAudioSystemService));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            noteOnHandler = new MidiEventHandler(this.OnNoteOn);

            base.Initialize();
        }

        /// <summary>
        /// Midi Timing Conversion Methods
        /// </summary>
        private double IncrementATKRELClock(long Ticks)
        {
            //Get the fixed Microsec per qn
            double mult = (m_AudioSystem.ProgramMicrosecsPerMinute / MICROSECONDS_PER_MINUTE);
            double mpqn = m_AudioSystem.MicrosecsPerQuarterNote * mult;
            //get the increment value
            double inc = 127 / (mpqn / (ulong)Ticks);
            return inc;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //At current tempo how much do we increment to get to 127 at the current quarter note
            double MidiATKRELInc = IncrementATKRELClock(gameTime.ElapsedGameTime.Ticks);

            #region Note Attack and release regions

            for (int i = 0; i < m_Notes.Count; ++i)
            {
                #region attack
                if (m_Notes[i].IsAttacking)
                {
                    m_Notes[i].NoteCue.Play();
                    m_Notes[i].IsAttacking = false;
                }
                #endregion
                if (!m_Notes[i].NoteCue.IsPlaying)
                {
                    m_Notes[i].NoteCue.Dispose();
                    m_Notes.RemoveAt(i); i--;
                    continue;
                }
                #region release
                if (m_Notes[i].IsReleasing)
                {
                    //Set up a work variable that houses the m_Patch attack speed
                    double atk = (double)m_ChannelInfo[(int)m_Notes[i].Channel].ReleaseSpeed;
                    //minimum rolloff
                    if (atk == 0) atk = 10;
                    //Get the true increment variable
                    double trueInc = 100 / (atk / MidiATKRELInc);
                    //increment our clocker
                    m_Notes[i].ReleaseVar += MidiATKRELInc;
                    float newVolume = (float)((double)m_Notes[i].NoteCue.GetVariable("Volume") - trueInc);
                    //Set the volume
                    m_Notes[i].NoteCue.SetVariable("Volume", newVolume);

                    //Have we reached the end of our attack
                    if (m_Notes[i].ReleaseVar >= (m_ChannelInfo[(int)m_Notes[i].Channel].ReleaseSpeed / 1.5))
                    {
                        m_Notes[i].NoteCue.Stop(AudioStopOptions.Immediate);
                        m_Notes[i].NoteCue.Dispose();
                        m_Notes.RemoveAt(i); i--;
                    }
                }

                #endregion

            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// Load all soundbank cues to save time later
        /// </summary>
        public void PrepareSamples()
        {
            // Not doable??

            //AudioStopOptions options = new AudioStopOptions();
            //for ( int i = 0; i < m_SlutObject.CodeTable.Length; ++i )
            //{
            //    m_Cues[ i ] = m_SoundBank.GetCue( m_SlutObject.CodeTable[ i ].index.ToString() );
            //    m_Cues[ i ].Play();
            //    m_Cues[ i ].Stop( options );
            //}
        }

        protected float VolumeToDB(int volume)
        {
            return (float)Math.Log10(128.0f / (float)(volume + 1)) * (-10.0f) * (-96.0f / -21.07f);
        }

        /// <summary>
        /// Handle MidiNoteOn event
        /// </summary>
        public override void MidiNoteOn(byte channel, byte noteNumber, byte velocity)
        {
            //now find the note
            decimal areaToLook = ((decimal)noteNumber / 12);
            //Get the whole val now
            areaToLook = Math.Round((decimal)areaToLook, 0);
            //Get a working variable
            int val = (int)areaToLook;

            string resourceName = "";

            try
            {
                _MidiNoteInfo mni = new _MidiNoteInfo();
                mni.Channel = channel;
                mni.Note = noteNumber;
                mni.Velocity = velocity;

                if (m_ChannelInfo[channel].StretchRange)
                {
                    resourceName = m_ChannelInfo[channel].Name + "_" + m_Patch.Octaves[val];
                    mni.NoteCue = m_SoundBank.GetCue(resourceName);
                    mni.NoteCue.SetVariable("Pitch", (float)-(((val + 1) * 12) - (noteNumber + 12)));
                }
                else
                {
                    int n = (int)noteNumber / 12;
                    resourceName = m_ChannelInfo[channel].Name + "_" + m_Patch.Notes[(12 * n) - noteNumber] + n.ToString();
                    mni.NoteCue = m_SoundBank.GetCue(resourceName);
                }
                mni.NoteCue.SetVariable("Volume", 64f + (velocity * 0.6f));
                mni.IsAttacking = true;
                m_Notes.Add(mni);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Attempting to create " + resourceName);
                //Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Handle MidiNoteOff event
        /// </summary>
        public override void MidiNoteOff(byte channel, byte noteNumber, byte velocity)
        {
            try
            {
                // TODO: status should update when m_Notes finish automatically
                //Cue cue = m_SoundBank.GetCue(m_SlutObject.CodeTable[noteNumber].ToString());
                //if (null != m_Cues[noteNumber])
                //{
                //    AudioStopOptions options = new AudioStopOptions();
                //    //cue.Stop( options );
                //    m_Cues[noteNumber].Stop(options);
                //    //m_Cues[ noteNumber ].Pause();
                //    m_NoteStatus[noteNumber] = false;

                //    //m_Cues[ noteNumber ].Dispose();
                //    m_Cues[noteNumber] = null;
                //    --m_CuePlayingCount;
                //}

                //all the notes
                for (int i = 0; i < m_Notes.Count; ++i)
                {
                    if (channel == m_Notes[i].Channel && noteNumber == m_Notes[i].Note)
                    {
                        m_Notes[i].IsReleasing = true; 
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Handle MidiProgramChange event
        /// </summary>
        public override void MidiProgramChange(byte channel, byte program)
        {
            m_ChannelInfo[channel] = m_Patch[program];
        }

        #region Message Handlers
        public override void OnNoteOn(object sender, MidiEventArgs e)
        {
            MidiNoteOn((byte)(e.Status & 0x0F), e.Data1, e.Data2);
        }

        public override void OnNoteOff(object sender, MidiEventArgs e)
        {
            MidiNoteOff((byte)(e.Status & 0x0F), e.Data1, e.Data2);
        }

        public override void OnProgramChange(object sender, MidiEventArgs e)
        {
            MidiProgramChange((byte)(e.Status & 0x0F), e.Data1);
        }
        #endregion
    }
}
