using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat;

namespace NuclexUserInterfaceExtension
{

    /// <summary>Renders horizontal sliders in a traditional flat style</summary>
    public class SkinNamedHorizontalSliderControlRenderer :
      IFlatControlRenderer<SkinNamedHorizontalSliderControl>
    {

        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(
          SkinNamedHorizontalSliderControl control, IFlatGuiGraphics graphics
        )
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();

            float thumbWidth = controlBounds.Width * control.ThumbSize;
            float thumbX = (controlBounds.Width - thumbWidth) * control.ThumbPosition;

            graphics.DrawElement(control.SkinName + ".rail.horizontal", controlBounds);

            RectangleF thumbBounds = new RectangleF(
              controlBounds.X + thumbX, controlBounds.Y, thumbWidth, controlBounds.Height
            );

            if (control.ThumbDepressed)
            {
                graphics.DrawElement(control.SkinName + ".slider.horizontal.depressed", thumbBounds);
            }
            else if (control.MouseOverThumb)
            {
                graphics.DrawElement(control.SkinName + ".slider.horizontal.highlighted", thumbBounds);
            }
            else
            {
                graphics.DrawElement(control.SkinName + ".slider.horizontal.normal", thumbBounds);
            }

        }

    }

}
