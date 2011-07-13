#region File Description
//-----------------------------------------------------------------------------
// IMidiDevice.cs
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
    public abstract class IMidiDevice : Microsoft.Xna.Framework.GameComponent
    {
        protected const ulong MICROSECONDS_PER_MINUTE = 60000000;

        private IMidiDevice m_OutputDevice;
        
        protected IMidiDevice( Game game)
            : base( game )
        {
        }

        // Overload this routing function to provide event handlers in child classes
        public IMidiDevice OutputDevice
        {
            get
            {
                return m_OutputDevice;
            }
            set
            {
                m_OutputDevice = value;
                value.OnSetOutput(this);
            }
        }

        public virtual void MidiNoteOn(byte channel, byte noteNumber, byte velocity) { }
        public virtual void MidiNoteOff(byte channel, byte noteNumber, byte velocity) { }
        public virtual void MidiProgramChange(byte channel, byte program) { }

        // Message Generating Events
        public virtual event MidiEventHandler NoteOn;
        public virtual event MidiEventHandler NoteOff;
        public virtual event MidiEventHandler ProgramChange;

        // Message Handlers
        protected virtual void OnSetOutput(IMidiDevice sender) { }

        public virtual void OnNoteOn(object sender, MidiEventArgs e) { }
        public virtual void OnNoteOff(object sender, MidiEventArgs e) { }
        public virtual void OnProgramChange(object sender, MidiEventArgs e) { }
    }

    // Helper classes for MIDI Event Handling
    // This is a single-use object
    public class MidiEventArgs : System.EventArgs
    {
        // Hopefully the compiler can optimize this?
        private readonly byte[] m_header = new byte[3] { 0x00, 0x00, 0x00 };

        public MidiEventArgs(byte status, byte data1, byte data2)
        {
            m_header[0] = status; m_header[1] = data1; m_header[2] = data2;
        }

        public byte Status
        {
            get
            {
                return m_header[0];
            }
        }

        public byte Data1
        {
            get
            {
                return m_header[1];
            }
        }

        public byte Data2
        {
            get
            {
                return m_header[2];
            }
        }
    }

    // MIDI message handler delegate
    public delegate void MidiEventHandler(object sender, MidiEventArgs e);
}
