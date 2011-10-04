using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat;

namespace NuclexUserInterfaceExtension
{

    /// <summary>Renders horizontal sliders in a traditional flat style</summary>
    public class SkinNamedVerticalSliderControlRenderer :
      IFlatControlRenderer<SkinNamedVerticalSliderControl>
    {

        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(
          SkinNamedVerticalSliderControl control, IFlatGuiGraphics graphics
        )
        {
        }

    }

}
