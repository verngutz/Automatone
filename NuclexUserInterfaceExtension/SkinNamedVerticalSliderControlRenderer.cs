using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat;

namespace NuclexUserInterfaceExtension
{
    /// <summary>Renders vertical sliders in a traditional flat style, with uniquely identified skin textures</summary>
    public class SkinNamedVerticalSliderControlRenderer : IFlatControlRenderer<SkinNamedVerticalSliderControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">Graphics interface that will be used to draw the control</param>
        public void Render(SkinNamedVerticalSliderControl control, IFlatGuiGraphics graphics)
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            float thumbHeight = controlBounds.Height * control.ThumbSize;
            float thumbY = (controlBounds.Height - thumbHeight) * control.ThumbPosition;

            graphics.DrawElement(control.SkinName + ".rail.vertical", controlBounds);

            RectangleF thumbBounds = new RectangleF(controlBounds.X, controlBounds.Y + thumbY, controlBounds.Width, thumbHeight);

            if (control.ThumbDepressed)
            {
                graphics.DrawElement(control.SkinName + ".slider.vertical.depressed", thumbBounds);
            }
            else if (control.MouseOverThumb)
            {
                graphics.DrawElement(control.SkinName + ".slider.vertical.highlighted", thumbBounds);
            }
            else
            {
                graphics.DrawElement(control.SkinName + ".slider.vertical.normal", thumbBounds);
            }
        }
    }
}
