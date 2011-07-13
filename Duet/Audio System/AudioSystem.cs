#region File Description
//-----------------------------------------------------------------------------
// AudioSystem.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion
#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Duet;
#endregion

namespace Duet.Audio_System
{
    public class AudioStartupOptions : Duet.StartupOptions
    {
        public String audioSettingsPath;
        public delegate bool BeatDetectCallback();
        public BeatDetectCallback beatDetectCallback;
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class AudioSystem : Microsoft.Xna.Framework.GameComponent, IAudioSystemService, ISystem
    {
        private AudioStartupOptions.BeatDetectCallback m_beatDetected;

#if (FREQ_DETECT)
        [DllImport("FrequencyLib2005.dll")]
        public static extern void InitializeXAudio();

        [DllImport("FrequencyLib2005.dll")]
        public static extern bool GetFrequencyBins(double[] pFreqBins, int nSize);

        [DllImport("FrequencyLib2005.dll")]
        public static extern bool GetDebugInputBuffer(double[] pFreqBins, int nSize);

        [DllImport("FrequencyLib2005.dll")]
        public static extern bool RegisterBeatDetect( AudioStartupOptions.BeatDetectCallback beatDetected );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetEnvironmentVariable(string lpName, string lpValue);
#endif

        private AudioEngine m_Audio;
        private IMidiDevice m_RootDevice;
        private MidiDecoder m_MidiDecoder = new MidiDecoder();
        private ulong m_ProgramMicrosecsPerMinute;

        #region Properties
        public AudioEngine XAudioEngine
        {
            get
            {
                return m_Audio;
            }
        }
        public MidiDecoder Midi
        {
            get
            {
                return m_MidiDecoder;
            }
        }
        public ulong ProgramMicrosecsPerMinute
        {
            get
            {
                return m_ProgramMicrosecsPerMinute;
            }
        }
        public ulong MicrosecsPerQuarterNote 
        { 
            get
            {
                return m_MidiDecoder.MicrosecsPerQuarterNote;
            }
            set
            {
                m_MidiDecoder.MicrosecsPerQuarterNote = value;
            }
        }
        public ulong BeatsPerMinute 
        {
            get
            {
                return m_MidiDecoder.BeatsPerMinute;
            }
            set
            {
                m_MidiDecoder.BeatsPerMinute = value;
            }
        }
        public AudioStartupOptions.BeatDetectCallback BeatDetected
        {
            set
            {
                m_beatDetected = value;
            }
        }

        public IMidiDevice RootDevice
        {
            get
            {
                return m_RootDevice;
            }
            set
            {
                m_RootDevice = value;
            }
        }
        #endregion

        public AudioSystem( Game game )
            : base( game )
        {
            // Register this object as a provider of the Audio Service
            game.Services.AddService( typeof( IAudioSystemService ), this );
        }


        /// <summary>
        /// Do additional system specific startup tasks.
        /// </summary>
        public void InsertDevice( IMidiDevice location, IMidiDevice device )
        {
            location.OutputDevice = device;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
#if (FREQ_DETECT)
            string newpath = Environment.GetEnvironmentVariable("Path") + ';' + 
                Path.GetFullPath(@"..\..\..\..\FrequencyLib\FrequencyLib2005\release") +
                ';' + Path.GetFullPath(@"..\..\..\..\FrequencyLib\fftw-3.1.2-dll");
            SetEnvironmentVariable("Path", newpath);
            

            m_PlaybackThread = new Thread(new ThreadStart(InitializeXAudio));
            m_PlaybackThread.Start();

            // The registration might fail if the internal object isn't created yet.
            if (m_beatDetected != null)
            {
                bool bRegistered = false;
                while (bRegistered == false)
                    bRegistered = RegisterBeatDetect(m_beatDetected);
            }
#endif

            base.Initialize();
        }


        /// <summary>
        /// Do additional system specific startup tasks.
        /// </summary>
        public void Startup( StartupOptions options )
        {
            m_Audio = new AudioEngine( ((AudioStartupOptions)options).audioSettingsPath );
            m_beatDetected = ((AudioStartupOptions)options).beatDetectCallback;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update( GameTime gameTime )
        {
            m_ProgramMicrosecsPerMinute = (ulong)((1.0 / gameTime.ElapsedGameTime.TotalSeconds) * gameTime.ElapsedGameTime.Ticks) * 60;
            m_Audio.Update();

            base.Update( gameTime );
        }


        /// <summary>
        /// Do system specific shutdown
        /// </summary>
        public void Shutdown()
        {
        }
    }
}


