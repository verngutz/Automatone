#region File Description
//-----------------------------------------------------------------------------
// ISystem.cs
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

using Duet.Audio_System;

namespace Duet
{
    public class StartupOptions
    {
        //public String startupString;
    }

    public interface ISystem
    {
        void Startup( StartupOptions options );
        void Shutdown();
    }

    static public class Global
    {
        private static AudioSystem g_AudioSystem;

        public static void Initialize( Game game )
        {
            // Create essential system instances
            g_AudioSystem = new AudioSystem( game );

            // Register systems
            game.Components.Add(g_AudioSystem);
        }
    }
}
