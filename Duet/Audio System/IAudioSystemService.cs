#region File Description
//-----------------------------------------------------------------------------
// IAudioSystemService.cs
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

using Duet;

namespace Duet.Audio_System
{
    public interface IAudioSystemService : ISystem
    {
        AudioEngine XAudioEngine { get; }
        MidiDecoder Midi { get; }
        ulong ProgramMicrosecsPerMinute { get; }
        ulong MicrosecsPerQuarterNote { get; set; }
        ulong BeatsPerMinute { get; set; }
    }
}
