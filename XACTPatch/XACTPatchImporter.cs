#region File Description
//-----------------------------------------------------------------------------
// XACTPatchImporter.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace XACTPatchPipeline
{
    [ContentImporterAttribute(".patchx", DefaultProcessor = "XACTPatchProcessor")]
    class XACTPatchImporter : ContentImporter<XACTPatchContent>
    {
        public override XACTPatchContent Import(string filename, ContentImporterContext context)
        {
            XmlSerializer xs = new XmlSerializer(typeof(XACTPatchContent));
            FileStream fs = new FileStream(filename, FileMode.Open);
            XACTPatchContent xpc = (XACTPatchContent)xs.Deserialize(fs);
            return xpc;
        }
    }
}
