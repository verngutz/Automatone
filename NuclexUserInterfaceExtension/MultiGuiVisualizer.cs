﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;

using Microsoft.Xna.Framework.Content;

using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Visuals;
using Nuclex.Support.Plugins;

namespace NuclexUserInterfaceExtension
{
    // TODO: This implements the Drawable class but doesn't override Draw()
    //   Having two overloads of the Draw() method, one doing nothing, is confusing
    //   and should be avoided. Find a better solution. Perhaps we can rely completely
    //   on the virtualized graphics device here.

    /// <summary>Draws traditional flat GUIs using 2D bitmaps</summary>
    public class MultiGuiVisualizer : IGuiVisualizer, IDisposable
    {
        /// <summary>Container for a control and its absolute boundaries</summary>
        struct ControlWithBounds
        {

            /// <summary>Initializes a new control and absolute boundary container</summary>
            /// <param name="control">Control being store in the container</param>
            /// <param name="bounds">Absolute boundaries the control lives in</param>
            public ControlWithBounds(Control control, RectangleF bounds)
            {
                this.Control = control;
                this.Bounds = bounds;
            }

            /// <summary>
            ///   Builds an absolute boundary container from the provided control
            /// </summary>
            /// <param name="control">Control from which a container will be created</param>
            /// <param name="containerBounds">Absolute boundaries of the control's parent</param>
            /// <returns>A new container with the control</returns>
            public static ControlWithBounds FromControl(Control control, RectangleF containerBounds)
            {
                containerBounds.X += control.Bounds.Location.X.Fraction * containerBounds.Width;
                containerBounds.X += control.Bounds.Location.X.Offset;
                containerBounds.Y += control.Bounds.Location.Y.Fraction * containerBounds.Height;
                containerBounds.Y += control.Bounds.Location.Y.Offset;

                containerBounds.Width = control.Bounds.Size.X.ToOffset(containerBounds.Width);
                containerBounds.Height = control.Bounds.Size.Y.ToOffset(containerBounds.Height);

                return new ControlWithBounds(control, containerBounds);
            }

            /// <summary>
            ///   Builds a control and absolute boundary container from a screen
            /// </summary>
            /// <param name="screen">Screen whose desktop control and absolute boundaries are used to construct the container</param>
            /// <returns>A new container with the screen's desktop control</returns>
            public static ControlWithBounds FromScreen(Screen screen)
            {
                return new ControlWithBounds(screen.Desktop, screen.Desktop.Bounds.ToOffset(screen.Width, screen.Height));
            }

            /// <summary>Control stored in the container</summary>
            public Control Control;
            /// <summary>Absolute boundaries of the stored control</summary>
            public RectangleF Bounds;
        }

        /// <summary>Interface for a generic (typeless) control renderer</summary>
        internal interface IControlRendererAdapter
        {
            /// <summary>
            ///   Renders the specified control using the provided graphics interface
            /// </summary>
            /// <param name="controlToRender">Control that will be rendered</param>
            /// <param name="graphics">Graphics interface that will be used to render the control</param>
            void Render(Control controlToRender, IFlatGuiGraphics graphics);

            /// <summary>The type of the control renderer being adapted</summary>
            Type AdaptedType { get; }
        }

        /// <summary>
        ///   Adapter that automatically casts a control down to the renderer's supported control type
        /// </summary>
        /// <typeparam name="ControlType">Type of control the control renderer casts down to</typeparam>
        /// <remarks>
        ///   This is simply an optimization to avoid invoking the control renderer
        ///   by reflection (using the Invoke() method) which would require us to construct
        ///   an object[] array on the heap to pass its arguments.
        /// </remarks>
        private class ControlRendererAdapter<ControlType> : IControlRendererAdapter where ControlType : Control
        {
            /// <summary>Initializes a new control renderer adapter</summary>
            /// <param name="controlRenderer">Control renderer the adapter is used for</param>
            public ControlRendererAdapter(IFlatControlRenderer<ControlType> controlRenderer)
            {
                this.controlRenderer = controlRenderer;
            }

            /// <summary>
            ///   Renders the specified control using the provided graphics interface
            /// </summary>
            /// <param name="controlToRender">Control that will be rendered</param>
            /// <param name="graphics">Graphics interface that will be used to render the control</param>
            public void Render(Control controlToRender, IFlatGuiGraphics graphics)
            {
                this.controlRenderer.Render((ControlType)controlToRender, graphics);
            }

            /// <summary>The type of the control renderer being adapted</summary>
            public Type AdaptedType
            {
                get { return this.controlRenderer.GetType(); }
            }

            /// <summary>Control renderer this adapter is performing the downcast for</summary>
            private IFlatControlRenderer<ControlType> controlRenderer;
        }

        /// <summary>
        ///   Employs concrete types implementing IFlatGuiControlRenderer&lt;&gt;
        /// </summary>
        /// <remarks>
        ///   This employer actually looks for concrete implementations using a variant
        ///   of the IFlatGuiControlRenderer&lt;&gt; interface, regardless of the
        ///   type it has been realized for.
        /// </remarks>
        internal class ControlRendererEmployer : Employer
        {
            /// <summary>Initializes a new control renderer employer</summary>
            public ControlRendererEmployer()
            {
                this.renderers = new Dictionary<Type, IControlRendererAdapter>();
                renderers.Add(typeof(SkinNamedButtonControl), new ControlRendererAdapter<SkinNamedButtonControl>(new SkinNamedButtonControlRenderer()));
                renderers.Add(typeof(SkinNamedHorizontalSliderControl), new ControlRendererAdapter<SkinNamedHorizontalSliderControl>(new SkinNamedHorizontalSliderControlRenderer()));
                renderers.Add(typeof(SkinNamedVerticalSliderControl), new ControlRendererAdapter<SkinNamedVerticalSliderControl>(new SkinNamedVerticalSliderControlRenderer()));
                renderers.Add(typeof(SkinNamedWindowControl), new ControlRendererAdapter<SkinNamedWindowControl>(new SkinNamedWindowControlRenderer()));
                renderers.Add(typeof(SkinNamedLabelControl), new ControlRendererAdapter<SkinNamedLabelControl>(new SkinNamedLabelControlRenderer()));
            }

            /// <summary>Determines whether the type suites the employer's requirements</summary>
            /// <param name="type">Type that is checked for employability</param>
            /// <returns>True if the type can be employed</returns>
            public override bool CanEmploy(Type type)
            {
                // If the type doesn't implement the IFlatcontrolRenderer interface, there's
                // no chance that it will implement one of the generic control drawers
                if (!typeof(IFlatControlRenderer).IsAssignableFrom(type))
                {
                    return false;
                }

                // We also need a default constructor in order to be able to create an
                // instance of this renderer
                if (!PluginHelper.HasDefaultConstructor(type))
                {
                    return false;
                }

                // Look for the IFlatControlRenderer<> interface in all interfaces implemented by this type
                Type[] implementedInterfaces = type.GetInterfaces();
                for (int index = 0; index < implementedInterfaces.Length; ++index)
                {

                    // Only perform further checks if this interface is actually generic
                    if (implementedInterfaces[index].IsGenericType)
                    {
                        Type genericType = implementedInterfaces[index].GetGenericTypeDefinition();
                        if (genericType == typeof(IFlatControlRenderer<>))
                        {
                            return true;
                        }
                    }

                }

                // The interface we were looking for was not found, therefore, this is not an employable type
                return false;
            }

            /// <summary>Employs the specified plugin type</summary>
            /// <param name="type">Type to be employed</param>
            public override void Employ(Type type)
            {
                // Obtain all the interfaces of the employed type and search them one by one.
                // We need to take this route because there's no method that would allow us to
                // look up the generic interface in its unspecialized form with a simple call.
                Type[] implementedInterfaces = type.GetInterfaces();
                for (int index = 0; index < implementedInterfaces.Length; ++index)
                {
                    // Only perform further checks if this interface is actually a generic one
                    if (implementedInterfaces[index].IsGenericType)
                    {
                        // Get the (unspecialized) generic form of this interface and see if it's the interface we're looking for
                        Type genericType = implementedInterfaces[index].GetGenericTypeDefinition();
                        if (genericType == typeof(IFlatControlRenderer<>))
                        {
                            // Find out which control type the renderer is specialized for
                            Type[] controlType = implementedInterfaces[index].GetGenericArguments();

                            // Do we already have a renderer for this control type?
                            if (this.renderers.ContainsKey(controlType[0]))
                            {
#if !(XBOX360 || WINDOWS_PHONE)
                                // We found another renderer for a control type that already has
                                // a renderer. At least print out a warning to the debug log about this.
                                string message = string.Format
                                (
                                    "Warning: Control type '{0}' already using renderer '{1}'.\n" +
                                    "         Second renderer '{2}' will be ignored!",
                                    controlType[0].FullName.ToString(),
                                    this.renderers[controlType[0]].AdaptedType.FullName.ToString(),
                                    type.FullName.ToString()
                                );
                                System.Diagnostics.Trace.WriteLine(message);
#endif
                            }
                            else
                            { // No, this is the first renderer we found for this control type

                                // Type of the downcast adapter we need to bring to life
                                Type adapterType = typeof(ControlRendererAdapter<>).MakeGenericType(controlType[0]);
                                // Look up the constructor of the downcast adapter
                                ConstructorInfo adapterConstructor = adapterType.GetConstructor(new Type[] { implementedInterfaces[index] });

                                // Now use that constructor to create an instance
                                object adapterInstance = adapterConstructor.Invoke(new object[] { Activator.CreateInstance(type) });

                                // Employ the new adapter and thereby the control renderer it adapts
                                this.renderers.Add(controlType[0], (IControlRendererAdapter)adapterInstance);
                            }
                        }
                    }
                }
            }

            /// <summary>Renderers that were employed to the plugin host</summary>
            public Dictionary<Type, IControlRendererAdapter> Renderers
            {
                get { return this.renderers; }
            }

            /// <summary>Employed renderers</summary>
            private Dictionary<Type, IControlRendererAdapter> renderers;
        }


        /// <summary>Initializes a new gui visualizer from a skin stored in a file</summary>
        /// <param name="serviceProvider">Game service provider containing the graphics device service</param>
        /// <param name="skinPath">Path to the skin description this GUI visualizer will load</param>
        public static MultiGuiVisualizer FromFile(IServiceProvider serviceProvider, string skinPath)
        {
            using (FileStream skinStream = new FileStream(skinPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ContentManager contentManager = new ContentManager(serviceProvider, Path.GetDirectoryName(skinPath));
                //try
                //{
                    return new MultiGuiVisualizer(contentManager, skinStream);
                //}
                //catch (Exception e)
                //{
                //    System.Console.Write(e.StackTrace);
                //    contentManager.Dispose();
                //    throw;
                //}
            }
        }

        /// <summary>Initializes a new gui visualizer from a skin stored as a resource</summary>
        /// <param name="serviceProvider">Game service provider containing the graphics device service</param>
        /// <param name="resourceManager">Resource manager containing the resources used in the skin</param>
        /// <param name="skinResource">Name of the resource containing the skin description</param>
        public static MultiGuiVisualizer FromResource(IServiceProvider serviceProvider, ResourceManager resourceManager, string skinResource)
        {
            byte[] resourceData = (byte[])resourceManager.GetObject(skinResource);
            if (resourceData == null)
            {
                throw new ArgumentException("Resource '" + skinResource + "' not found", "skinResource");
            }

            // This sucks! I cannot use ResourceManager.GetStream() on a resource that's
            // stored as a byte array!
            using (MemoryStream skinStream = new MemoryStream(resourceData, false))
            {
                ResourceContentManager contentManager = new ResourceContentManager(serviceProvider, resourceManager);
                try
                {
                    return new MultiGuiVisualizer(contentManager, skinStream);
                }
                catch (Exception)
                {
                    contentManager.Dispose();
                    throw;
                }
            }
        }

        /// <summary>Initializes a new gui painter for traditional GUIs</summary>
        /// <param name="contentManager">Content manager that will be used to load the skin resources</param>
        /// <param name="skinStream">Stream from which the GUI Visualizer will read the skin description</param>
        protected MultiGuiVisualizer(ContentManager contentManager, Stream skinStream)
        {
            this.employer = new ControlRendererEmployer();
            this.pluginHost = new PluginHost(this.employer);

            // Employ our own assembly in order to obtain the default GUI renderers
            this.pluginHost.Repository.AddAssembly(Self);

            this.flatGuiGraphics = new MultiGuiGraphics(contentManager, skinStream);
            this.controlStack = new Stack<ControlWithBounds>();
        }

        /// <summary>Immediately releases all resources owned by the instance</summary>
        public void Dispose()
        {
            if (this.flatGuiGraphics != null)
            {
                this.flatGuiGraphics.Dispose();
                this.flatGuiGraphics = null;
            }
        }

        /// <summary>Draws an entire GUI hierarchy</summary>
        /// <param name="screen">Screen containing the GUI that will be drawn</param>
        public void Draw(Screen screen)
        {
            this.flatGuiGraphics.BeginDrawing();
            try
            {
                this.controlStack.Push(ControlWithBounds.FromScreen(screen));

                while (controlStack.Count > 0)
                {
                    ControlWithBounds controlWithBounds = this.controlStack.Pop();

                    Control currentControl = controlWithBounds.Control;
                    RectangleF currentBounds = controlWithBounds.Bounds;

                    // Add the controls in normal order, so the first control in the collection will
                    // be taken off the stack last, ensuring it's rendered on top of all others.
                    for (int index = 0; index < currentControl.Children.Count; ++index)
                    {
                        this.controlStack.Push(ControlWithBounds.FromControl(currentControl.Children[index], currentBounds));
                    }

                    renderControl(currentControl);
                }
            }
            finally
            {
                this.flatGuiGraphics.EndDrawing();
            }
        }

        /// <summary>
        ///   Plugin repository from which renderers for GUI controls are taken
        /// </summary>
        public PluginRepository RendererRepository
        {
            get { return this.pluginHost.Repository; }
        }

        /// <summary>Renders a single control</summary>
        /// <param name="controlToRender">Control that will be rendered</param>
        private void renderControl(Control controlToRender)
        {
            IControlRendererAdapter renderer = null;

            Type controlType = controlToRender.GetType();

            // If this is an actual instance of the 'Control' class, don't render it.
            // Such instances can be used to construct invisible containers, and are most
            // prominently embodied in the 'desktop' control that hosts the whole GUI.
            if ((controlType == typeof(Control) || (controlType.FullName.ToString().Equals("Nuclex.UserInterface.Controls.DesktopControl"))))
            {
                return;
            }

            // Find a renderer for this control. If no renderer for the control itself can
            // be found, look for a renderer then can render its base class. This allows
            // controls to inherit from existing controls, remaining renderable (but also
            // gaining the ability to accept a specialized renderer for the new derived
            // control class!). Normally, this loop will finish without any repetitions.
            while (controlType != typeof(object))
            {
                bool found = this.employer.Renderers.TryGetValue(controlType, out renderer);
                if (found)
                {
                    break;
                }

                // Next, try the base class of this type
                controlType = controlType.BaseType;
            }

            // If we found a renderer, use it to render the control
            if (renderer != null)
            {
                renderer.Render(controlToRender, this.flatGuiGraphics);
            }
            else
            { // No renderer found, output a warning
#if WINDOWS
                Trace.WriteLine
                (
                    string.Format
                    (
                        "Warning: No renderer found for control '{0}' or any of its base classes.\n" +
                        "         Control will not be rendered.",
                        controlToRender.GetType().FullName.ToString()
                    )
                );
#endif
            }
        }

        /// <summary>Returns the assembly containing the GUI visualizer</summary>
        private static Assembly Self
        {
            get { return typeof(FlatGuiVisualizer).Assembly; }
        }

        /// <summary>Holds the assemblies we have employed for our cause</summary>
        private PluginHost pluginHost;

        /// <summary>Carries the employed control renderers</summary>
        private ControlRendererEmployer employer;

        /// <summary>Used to draw the individual building elements of the GUI</summary>
        private MultiGuiGraphics flatGuiGraphics;

        /// <summary>Helps draw the GUI controls in the hierarchically correct order</summary>
        /// <remarks>
        ///   This is a field and not a local variable because the stack allocates
        ///   heap memory and we don't want that to happen in a frame-by-frame basis on
        ///   the compact framework. By reusing the same stack over and over, the amount
        ///   of heap allocations required will amortize itself.
        /// </remarks>
        private Stack<ControlWithBounds> controlStack;
    }
}