#region File Description
//-----------------------------------------------------------------------------
// XACTPatchWriter.cs
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
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace XACTPatchPipeline
{
    [ContentTypeWriter]
    public class XACTPatchWriter : ContentTypeWriter<XACTPatchContent>
    {
        protected override void Write(ContentWriter output, XACTPatchContent value)
        {
            output.Write("SLUT");
            output.Write(value.Count);
            for ( uint i = 0; i < value.Instrument.Length; ++i )
            {
                output.Write(value.Instrument[i].Name);
                int flags = 0x00 | 
                    ((value.Instrument[i].Repeat ? 1 : 0) << 7) |
                    ((value.Instrument[i].StretchRange ? 1 : 0) << 6) |
                    (value.Instrument[i].PatchNumber & 0x0F);
                output.Write((byte)flags);
                output.Write(value.Instrument[i].AttackSpeed);
                output.Write(value.Instrument[i].ReleaseSpeed);
            }
        }
  
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            //return typeof(XACTPatchContent).AssemblyQualifiedName;
            return "Duet.Audio_System.XACTPatch, Duet";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Duet.Audio_System.XACTPatchReader, Duet";
        }
    }
}
