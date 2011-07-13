#region File Description
//-----------------------------------------------------------------------------
// XACTPatchReader.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;

namespace Duet.Audio_System
{
    public class XACTPatchReader : ContentTypeReader< XACTPatch >
    {
        protected override XACTPatch Read(ContentReader input, XACTPatch existingInstance)
        {
            return new XACTPatch( input );
        }
    }
}
