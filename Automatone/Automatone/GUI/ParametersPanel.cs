using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using NuclexUserInterfaceExtension;

using Automatone.Theories;

namespace Automatone.GUI
{
    public class ParametersPanel : SkinNamedWindowControl
    {
        private const byte SLIDE_MOVESPEED = 10;
        private short slideMoveVelocity;

        private SkinNamedButtonControl globalRandomizeButton;
        private SkinNamedButtonControl okButton;
        private SkinNamedButtonControl cancelButton;

        private Dictionary<SkinNamedHorizontalSliderControl, PropertyInfo> slidersHookUp;

        public UniRectangle GlobalRandomizeButtonBounds { set { globalRandomizeButton.Bounds = value; } }
        public UniRectangle OkButtonBounds { set { okButton.Bounds = value; } }
        public UniRectangle CancelButtonBounds { set { cancelButton.Bounds = value; } }

        private bool slidersHaveBeenMoved;

        private ParametersPanel() : base()
        {
            slideMoveVelocity = 0;
            slidersHookUp = new Dictionary<SkinNamedHorizontalSliderControl, PropertyInfo>();
            slidersHaveBeenMoved = false;
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

            int x = 0;
            //
            // Dynamically create sliders for each of the input parameters that needs to be hooked up to a slider
            //
            foreach (PropertyInfo inputParameterInfo in typeof(InputParameters).GetProperties())
            {
                // Do something only if the current parameter is a ParameterWrapper<T>, where T implements ZeroOneParameter
                if (inputParameterInfo.PropertyType.IsGenericType
                    && inputParameterInfo.PropertyType.GetGenericArguments()[0].GetInterfaces().Length != 0
                    && inputParameterInfo.PropertyType.GetGenericArguments()[0].GetInterfaces()[0] == typeof(ZeroOneParameter))
                {
                    MethodInfo method = typeof(ParameterWrapperFactory).GetMethod(ParameterWrapperFactory.DoubleMethodName).MakeGenericMethod(inputParameterInfo.PropertyType.GetGenericArguments()[0]);

                    SkinNamedHorizontalSliderControl parameterSlider = new SkinNamedHorizontalSliderControl();
                    parameterSlider.Bounds = new UniRectangle(0, x * 50, 100, 30);
                    parameterSlider.ThumbSize = 0.1f;
                    parameterSlider.ThumbPosition = (float)(double)method.Invoke(null, new object[] { inputParameterInfo.GetGetMethod().Invoke(InputParameters.Instance, null) });
                    parameterSlider.Moved += new EventHandler(ParameterSliderMoved);
                    parameterSlider.SkinName = "navigator";
                    Children.Add(parameterSlider);
                    slidersHookUp.Add(parameterSlider, inputParameterInfo);
                    x++;
                }
            }
        }

        private void ParameterSliderMoved(object sender, EventArgs e)
        {
            PropertyInfo inputParameterInfo;
            if (slidersHookUp.TryGetValue((SkinNamedHorizontalSliderControl)sender, out inputParameterInfo))
            {
                
            }
        }

        public void Update()
        {
            Bounds.Location.Y = new UniScalar(MathHelper.Clamp(Bounds.Location.Y.Offset + slideMoveVelocity, LayoutManager.CONTROLS_AND_GRID_DIVISION - Automatone.Instance.Window.ClientBounds.Height, LayoutManager.CONTROLS_AND_GRID_DIVISION));
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

            // Randomize sliders only, don't change underlying input parameter yet
            foreach (SkinNamedHorizontalSliderControl slider in slidersHookUp.Keys)
            {
                slider.ThumbPosition = (float)rand.NextDouble();
            }

            slidersHaveBeenMoved = true;
        }

        private void OkButtonPressed(object sender, EventArgs e)
        {
            if (ControlPanel.Instance.ShowSaveConfirmation() != DialogResult.Cancel)
            {
                ControlPanel.Instance.StopSongPlaying();

                //synchronize all the input parameters based on the slider parameters
                foreach (KeyValuePair<SkinNamedHorizontalSliderControl, PropertyInfo> sliderParamPair in slidersHookUp)
                {
                    PropertyInfo inputParameterInfo = sliderParamPair.Value;
                    MethodInfo method = typeof(ParameterWrapperFactory).GetMethod(ParameterWrapperFactory.WrapperMethodName).MakeGenericMethod(inputParameterInfo.PropertyType.GetGenericArguments()[0]);
                    inputParameterInfo.GetSetMethod().Invoke(InputParameters.Instance, new object[] { method.Invoke(null, new object[] { sliderParamPair.Key.ThumbPosition }) });
                }

#if USESEED
                GridPanel.Instance.SongCells = SongGenerator.GenerateSong(Automatone.Instance, new Random(SEED), new ClassicalTheory());
#else
                GridPanel.Instance.SongCells = SongGenerator.GenerateSong(Automatone.Instance, new Random(), new ClassicalTheory());
#endif
                GridPanel.Instance.HasUnsavedChanges = true;
                GridPanel.Instance.ResetCursors();
                NavigatorPanel.Instance.ResetGridDrawOffset();
                SlideUp();
            }
        }

        private void CancelButtonPressed(object sender, EventArgs e)
        {
            if (slidersHaveBeenMoved)
            {
                if (MessageBox.Show(
                    "Sliders will be reverted back to their old values. Proceed?",
                    "Confirmation",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2)
                    == DialogResult.OK)
                {
                    //reset all the sliders to the previous (unchanged) values of the underlying input parameters
                    foreach (KeyValuePair<SkinNamedHorizontalSliderControl, PropertyInfo> sliderParamPair in slidersHookUp)
                    {
                        PropertyInfo inputParameterInfo = sliderParamPair.Value;
                        MethodInfo method = typeof(ParameterWrapperFactory).GetMethod(ParameterWrapperFactory.DoubleMethodName).MakeGenericMethod(inputParameterInfo.PropertyType.GetGenericArguments()[0]);
                        sliderParamPair.Key.ThumbPosition = (float)(double)method.Invoke(null, new object[] { inputParameterInfo.GetGetMethod().Invoke(InputParameters.Instance, null) });
                    }
                    slidersHaveBeenMoved = false;
                }
                else
                {
                    return;
                }
            }

            SlideUp();
        }
    }
}
