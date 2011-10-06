using System;
using System.Collections.Generic;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat;

namespace NuclexUserInterfaceExtension
{
    /// <summary>Renders window controls in a traditional flat style, with uniquely identified skin textures</summary>
    public class SkinNamedWindowControlRenderer : IFlatControlRenderer<SkinNamedWindowControl>
    {

        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">Graphics interface that will be used to draw the control</param>
        public void Render(SkinNamedWindowControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();
            graphics.DrawElement(control.SkinName + ".window", controlBounds);

            if (control.Title != null)
            {
                graphics.DrawString(control.SkinName + ".window", controlBounds, control.Title);
            }
        }
    }
}
