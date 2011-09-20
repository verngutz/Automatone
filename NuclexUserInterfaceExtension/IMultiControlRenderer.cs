using System;
using System.Collections.Generic;

using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Visuals.Flat;

namespace NuclexUserInterfaceExtension
{

    /// <summary>Interface for a class that renders a control</summary>
    public interface IMultiControlRenderer { }

    /// <summary>
    ///   Interface for a class responsible to render a specific control type
    /// </summary>
    /// <typeparam name="ControlType">
    ///   Type of control the implementation class will render
    /// </typeparam>
    public interface IMultiControlRenderer<ControlType> : IMultiControlRenderer
      where ControlType : Control
    {

        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        void Render(ControlType control, IFlatGuiGraphics graphics);

    }

}