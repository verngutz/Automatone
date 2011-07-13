#region File Description
//-----------------------------------------------------------------------------
// XACTPatch.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework.Content;

namespace Duet.Audio_System
{
    public class _InstrumentInfo
    {
        //public byte PatchNumber;
        public bool Repeat;
        public bool StretchRange;
        public byte AttackSpeed;
        public byte ReleaseSpeed;
        public string Name;
    }

    public class XACTPatch
    {
        //Get our octaves
        public string[] Octaves = { "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9" };
        public string[] Notes = { "C", "CS", "D", "DS", "E", "F", "FS", "G", "GS", "A", "AS", "B" };

        private _InstrumentInfo[] m_Instruments = new _InstrumentInfo[128];

        public _InstrumentInfo this[int programNumber]
        {
            get 
            {
                return m_Instruments[programNumber]; 
            }
        }
                
        public XACTPatch(ContentReader input)
        {
            try
            {
                input.ReadByte();
                char[] header = input.ReadChars(4);
                if ( string.Compare( new string(header), "SLUT" ) != 0 )
                    throw new Exception( "Caught Exception: Binary not in SLUT format!" );
                int count = input.ReadInt32();
                if ( count > 128 ) count = 128; // Cap the maximum number of instruments
                for ( uint i = 0; i < count; ++i )
                {
                    _InstrumentInfo info = new _InstrumentInfo();
                    info.Name = input.ReadString();

                    byte flags = input.ReadByte();
                    info.Repeat = false; info.StretchRange = false;
                    if ((flags & 0x80) > 0) info.Repeat = true;      //if repeat
                    if ((flags & 0x40) > 0) info.StretchRange = true; //if range
                    int patchNumber = (int)(flags & 0x0F);
                    //info.PatchNumber = (byte)(flags & 0x0F);

                    info.AttackSpeed = input.ReadByte();
                    info.ReleaseSpeed = input.ReadByte();

                    m_Instruments[patchNumber] = info;
                }
            } 
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
