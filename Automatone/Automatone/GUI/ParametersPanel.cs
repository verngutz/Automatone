using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using NuclexUserInterfaceExtension;

using Automatone.Theories;

namespace Automatone.GUI
{
    public class ParametersPanel : SkinNamedWindowControl
    {
        private const byte SLIDE_MOVESPEED = 5;
        private short slideMoveVelocity;

        private SkinNamedButtonControl globalRandomizeButton;
        private SkinNamedButtonControl okButton;
        private SkinNamedButtonControl cancelButton;

        private Dictionary<SkinNamedHorizontalSliderControl, PropertyInfo> parametersSliders;

        public UniRectangle GlobalRandomizeButtonBounds { set { globalRandomizeButton.Bounds = value; } }
        public UniRectangle OkButtonBounds { set { okButton.Bounds = value; } }
        public UniRectangle CancelButtonBounds { set { cancelButton.Bounds = value; } }

        private ParametersPanel() : base()
        {
            slideMoveVelocity = 0;
            parametersSliders = new Dictionary<SkinNamedHorizontalSliderControl, PropertyInfo>();
            InitializeComponent();
        }

        private static ParametersPanel instance;
        public static ParametersPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = new ParametersPanel();
                return instance;
            }
        }

        private void InitializeComponent()
        {
            Bounds = LayoutManager.Instance.ParametersPanelBounds;
            EnableDragging = false;
            SkinName = "parameters.panel";

            //
            // Contruct Children
            //
            globalRandomizeButton = new SkinNamedButtonControl();
            okButton = new SkinNamedButtonControl();
            cancelButton = new SkinNamedButtonControl();

            //
            // globalRandomizeButton
            //
            globalRandomizeButton.Bounds = LayoutManager.Instance.GlobalRandomizeButtonBounds;
            globalRandomizeButton.Pressed += new EventHandler(GlobalRandomizeButtonPressed);
            globalRandomizeButton.SkinName = "global.randomize";

            //
            // okButton
            //
            okButton.Bounds = LayoutManager.Instance.OkButtonBounds;
            okButton.Pressed += new EventHandler(OkButtonPressed);
            okButton.SkinName = "ok";

            //
            // cancelButton
            //
            cancelButton.Bounds = LayoutManager.Instance.CancelButtonBounds;
            cancelButton.Pressed += new EventHandler(CancelButtonPressed);
            cancelButton.SkinName = "cancel";

            //
            // Add Children
            //
            Children.Add(globalRandomizeButton);
            Children.Add(okButton);
            Children.Add(cancelButton);

            //
            // Dynamically create sliders for each of the input parameters that needs to be hooked up to a slider
            //
            foreach (PropertyInfo p in typeof(InputParameters).GetProperties())
            {
                // Do nothing if the current parameter isn't a ZeroOneParameter<T>, where T implements ZeroOneParameter
                if(!p.PropertyType.IsGenericType
                    || p.PropertyType.GetGenericArguments()[0].GetInterfaces().Length == 0
                    || p.PropertyType.GetGenericArguments()[0].GetInterfaces()[0] != typeof(ZeroOneParameter))
                {
                    continue;
                }

                SkinNamedHorizontalSliderControl parameterSlider = new SkinNamedHorizontalSliderControl();

                if (p.PropertyType == typeof(ParameterWrapper<SongParameter>))
                {
                    double d = (ParameterWrapper<SongParameter>)p.GetValue(InputParameters.Instance, null);
                    System.Console.WriteLine(d);
                }

                parametersSliders.Add(parameterSlider, p);
            }
        }

        public void Update()
        {
            Bounds.Location.Y = new UniScalar(MathHelper.Clamp(Bounds.Location.Y.Offset + slideMoveVelocity, LayoutManager.CONTROLS_AND_GRID_DIVISION - LayoutManager.PARAMETERS_PANEL_HEIGHT, LayoutManager.CONTROLS_AND_GRID_DIVISION));
        }

        public void Toggle()
        {
            if (slideMoveVelocity <= 0)
                SlideDown();
            else
                SlideUp();
        }

        public void SlideDown()
        {
            slideMoveVelocity = SLIDE_MOVESPEED;
        }

        public void SlideUp()
        {
            slideMoveVelocity = -SLIDE_MOVESPEED;
        }

        private void GlobalRandomizeButtonPressed(object sender, EventArgs e)
        {
            Random rand = new Random();
            foreach (PropertyInfo p in parametersSliders.Values)
            {
                MethodInfo method = typeof(ParameterWrapperFactory).GetMethod(ParameterWrapperFactory.WrapperMethodName).MakeGenericMethod(p.PropertyType.GetGenericArguments()[0]);
                p.GetSetMethod().Invoke(InputParameters.Instance, new object[] { method.Invoke(null, new object[]{ rand.NextDouble() }) });
            }
        }

        private void OkButtonPressed(object sender, EventArgs e)
        {
            Automatone.Instance.StopSongPlaying();

#if USESEED
            GridPanel.Instance.SongCells = SongGenerator.GenerateSong(Automatone.Instance, new Random(SEED), new ClassicalTheory());
#else
            GridPanel.Instance.SongCells = SongGenerator.GenerateSong(Automatone.Instance, new Random(), new ClassicalTheory());
#endif
            GridPanel.Instance.ResetCursors();
            NavigatorPanel.Instance.ResetGridDrawOffset();
            SlideUp();

            foreach (PropertyInfo p in parametersSliders.Values)
            {
                MethodInfo method = typeof(ParameterWrapperFactory).GetMethod(ParameterWrapperFactory.DoubleMethodName).MakeGenericMethod(p.PropertyType.GetGenericArguments()[0]);
                System.Console.WriteLine((double)method.Invoke(null, new object[] { p.GetGetMethod().Invoke(InputParameters.Instance, null) }));
            }
        }

        private void CancelButtonPressed(object sender, EventArgs e)
        {
            SlideUp();
        }
    }
}
