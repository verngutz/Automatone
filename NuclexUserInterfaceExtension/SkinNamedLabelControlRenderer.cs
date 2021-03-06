﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Nuclex.UserInterface.Visuals.Flat;

namespace NuclexUserInterfaceExtension
{

    /// <summary>Renders label controls in a traditional flat style</summary>
    public class SkinNamedLabelControlRenderer : IFlatControlRenderer<SkinNamedLabelControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">Graphics interface that will be used to draw the control</param>
        public void Render(SkinNamedLabelControl control, IFlatGuiGraphics graphics)
        {
            graphics.DrawString(control.SkinName + ".label", control.GetAbsoluteBounds(), control.Text);
        }

    }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers
