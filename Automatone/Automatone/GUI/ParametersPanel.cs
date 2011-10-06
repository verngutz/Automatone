using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Xna.Framework;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Controls.Desktop;
using NuclexUserInterfaceExtension;

namespace Automatone.GUI
{
    public class ParametersPanel : SkinNamedWindowControl
    {
        private const byte SLIDE_MOVESPEED = 5;
        private short slideMoveVelocity;

        private ParametersPanel() : base()
        {
            slideMoveVelocity = 0;
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

            foreach (PropertyInfo p in typeof(InputParameters).GetProperties())
            {
                if (p.PropertyType == typeof(MusicParameter<SongParameter>))
                {
                    double d = (MusicParameter<SongParameter>)p.GetValue(InputParameters.Instance, null);
                    System.Console.WriteLine(d);
                }
            }
        }

        public void Update()
        {
            Bounds.Location.Y = new UniScalar(MathHelper.Clamp(Bounds.Location.Y.Offset + slideMoveVelocity, LayoutManager.CONTROLS_AND_GRID_DIVISION - LayoutManager.PARAMETERS_PANEL_HEIGHT, LayoutManager.CONTROLS_AND_GRID_DIVISION));
        }

        public void Toggle()
        {
            if (slideMoveVelocity <= 0)
                StartSlideDown();
            else
                StartSlideUp();
        }

        public void StartSlideDown()
        {
            slideMoveVelocity = SLIDE_MOVESPEED;
        }

        public void StartSlideUp()
        {
            slideMoveVelocity = -SLIDE_MOVESPEED;   
        }
    }
}
