#region File Description
//-----------------------------------------------------------------------------
// Program.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;

namespace MidiDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MidiGame game = new MidiGame())
            {
                game.Run();
            }
        }
    }
}

