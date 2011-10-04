using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Automatone.GUI
{
    public class FramedBounds
    {
        private Rectangle topRectangle;
        public Rectangle TopRectangle { get { return topRectangle; } }

        private Rectangle bottomRectangle;
        public Rectangle BottomRectangle { get { return bottomRectangle; } }

        private Rectangle leftRectangle;
        public Rectangle LeftRectangle { get { return leftRectangle; } }

        private Rectangle rightRectangle;
        public Rectangle RightRectangle { get { return rightRectangle; } }

        private Rectangle centerRectangle;
        public Rectangle CenterRectangle { get { return centerRectangle; } }
        public Rectangle InnerRectangle { get { return centerRectangle; } }

        private Rectangle outerRectangle;
        public Rectangle OuterRectangle { get { return outerRectangle; } }

        public FramedBounds(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            centerRectangle = innerRectangle;
            this.outerRectangle = outerRectangle;
            topRectangle = new Rectangle(outerRectangle.Left, outerRectangle.Top, outerRectangle.Width, innerRectangle.Top - outerRectangle.Top);
            bottomRectangle = new Rectangle(outerRectangle.Left, innerRectangle.Bottom, outerRectangle.Width, outerRectangle.Bottom - innerRectangle.Bottom);
            leftRectangle = new Rectangle(outerRectangle.Left, innerRectangle.Top, innerRectangle.Left - outerRectangle.Left, innerRectangle.Height);
            rightRectangle = new Rectangle(innerRectangle.Right, innerRectangle.Top, outerRectangle.Right - innerRectangle.Right, innerRectangle.Height);
        }
    }
}
