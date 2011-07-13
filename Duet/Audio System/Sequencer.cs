#region File Description
//-----------------------------------------------------------------------------
// Sequencer.cs
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
    public class Sequencer : IMidiDevice
    {
        enum MidiPlayerState { PLAYING, PAUSED, STOPPED }

        public override event MidiEventHandler NoteOn;
        public override event MidiEventHandler NoteOff;
        public override event MidiEventHandler ProgramChange;

        private IAudioSystemService m_AudioSystem;
        private MidiPlayerState m_mps = MidiPlayerState.STOPPED;

        private ulong m_LoopStart = ulong.MaxValue, m_LoopEnd = ulong.MaxValue, m_MidiPos = 0;
        
        //int note = 0;
        //TimeSpan timer = new TimeSpan();

        public Sequencer(Game game)
            : base( game )
        {
            m_AudioSystem = (IAudioSystemService)game.Services.GetService(typeof(IAudioSystemService));
        }

        /// <summary>
        /// Load a midi file from filename.
        /// </summary>
        public void LoadMidi(string filename)
        {
            if (!m_AudioSystem.Midi.LoadMidi(filename))
                Console.WriteLine("Failed to load MIDI file!");

        }

        /// <summary>
        /// Start midi playback
        /// </summary>
        public void PlayMidi()
        {
            m_mps = MidiPlayerState.PLAYING;
        }

        /// <summary>
        /// Start midi playback
        /// </summary>
        public void PlayMidi(ulong StartPoint)
        {
            m_MidiPos = StartPoint;
            PlayMidi();
        }

        /// <summary>
        /// Stop midi playback
        /// </summary>
        public void StopMidi()
        {
            m_mps = MidiPlayerState.STOPPED;
            m_MidiPos = 0;
        }

        /// <summary>
        /// Pause midi playback
        /// </summary>
        public void PauseMidi()
        {
            m_mps = MidiPlayerState.PAUSED;
        }

        /// <summary>
        /// Midi Timing Conversion Methods
        /// </summary>
        private double IncrementMidiClock(long Ticks)
        {
            //Get the fixed Microsec per qn
            double mult = (m_AudioSystem.ProgramMicrosecsPerMinute / MICROSECONDS_PER_MINUTE);
            double mpqn = m_AudioSystem.MicrosecsPerQuarterNote * mult;
            //get the increment value
            double inc = (double)m_AudioSystem.Midi.MidiHeaderData.DeltaTiming / (mpqn / (double)Ticks);
            return inc;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //increment the midi clock for our purposes
            double MidiClockInc = IncrementMidiClock(gameTime.ElapsedGameTime.Ticks);
            int curEvent = 0;
            
            if (m_mps == MidiPlayerState.PLAYING && gameTime.ElapsedGameTime.Ticks > 0)
            {
                bool amPlaying = false;

                //ulong bpm = MIDI_MICROSECS_PER_MINUTE / MIDI_EVENT_DATA.MICROSECS_PER_QUARTER_NOTES;
                for (int i = 0; i < m_AudioSystem.Midi.MidiEventData.Length; ++i)
                {
                    curEvent = (int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent;
                    
                    if ((ulong)curEvent >= (ulong)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO.Count) continue;
                    else amPlaying = true;
                    //increment our values to what we need
                    if (m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent].DeltaTime > 0)
                    {
                        m_AudioSystem.Midi.MidiEventData[i].deltaTime += MidiClockInc;
                        m_AudioSystem.Midi.MidiEventData[i].CurrentTime += MidiClockInc;
                    }
                    if (m_LoopStart != ulong.MaxValue && m_LoopEnd != ulong.MaxValue && m_AudioSystem.Midi.MidiEventData[i].CurrentTime >= m_LoopEnd)
                    {
                        m_AudioSystem.Midi.MidiEventData[i].CurrentTime = m_LoopStart;
                    }

                    //if we encountered an event let's handle it or else continue
                    if (m_AudioSystem.Midi.MidiEventData[i].deltaTime >= m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent].DeltaTime)
                    {
                        m_AudioSystem.Midi.MidiEventData[i].deltaTime -= m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent].DeltaTime;
                        curEvent = (int)++m_AudioSystem.Midi.MidiEventData[i].CurrentEvent;
                    }
                    else continue;

                    #region Midi data

                    switch (m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].MidiDataEvents)
                    {
                        case MIDI_DATA_EVENTS.NOTE_ON:
                            //Play the note supplied on the channel supplied
                            NoteOn(this, new MidiEventArgs((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[0],
                                        (byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[1],
                                        (byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[2]));
                            // Console.WriteLine("Note ON: Ch" + ((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[0]).ToString() + " Note: " + ((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[1]).ToString() + " Vel: " + ((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[2]).ToString());



                            break;
                        case MIDI_DATA_EVENTS.NOTE_OFF:
                            //Stop the first occuring note with these properties
                            NoteOff(this, new MidiEventArgs((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[0],
                                        (byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[1],
                                        (byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[2]));
                            // Console.WriteLine("Note OFF: Ch" + ((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[0]).ToString() + " Note: " + ((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[1]).ToString() + " Vel: " + ((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[2]).ToString());
                            break;
                        case MIDI_DATA_EVENTS.CONTROLLER:
                            break;
                        case MIDI_DATA_EVENTS.PROGRAM_CHANGE:
                            try
                            {
                                //byte foo = (byte)(m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[0]);
                                //byte bar = (byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[1];
                                ProgramChange(this, new MidiEventArgs((byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[0],
                                        (byte)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[curEvent - 1].Parameters[1],
                                        0x00));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        case MIDI_DATA_EVENTS.PITCH_BEND: 
                            break;
                    }

                    #endregion
                    #region Midi Event Data

                    switch (m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].MetaEvents)
                    {
                        case MIDI_META_EVENTS.TEMPO_SETTING:
                            UInt32 val = (UInt32)m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent - 1].Parameters[0];
                            m_AudioSystem.MicrosecsPerQuarterNote = (ulong)val;    
                            break;
                        case MIDI_META_EVENTS.MARKER_TEXT: break;
                    }

                    #endregion
                    if ((int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent < m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO.Count)
                        if (m_AudioSystem.Midi.MidiEventData[i].MIDI_EVENT_INFO[(int)m_AudioSystem.Midi.MidiEventData[i].CurrentEvent].DeltaTime == 0) i--;
                }
                //if we are done then stop
                if (!amPlaying) m_mps = MidiPlayerState.STOPPED;

                base.Update(gameTime);
            }
            
        }

    }
}
