using Microsoft.Xna.Framework.Input;
using Nuclex.UserInterface.Controls.Desktop;

namespace NuclexUserInterfaceExtension
{
    public class SkinNamedButtonControl : ButtonControl
    {
        public string SkinName { set; get; }
        protected override bool OnKeyPressed(Keys keyCode)
        {
            if (keyCode == Keys.Space)
                return false;
            return base.OnKeyPressed(keyCode);
        }
    }
}
