using System;

using Microsoft.Xna.Framework;

namespace Automatone
{
    public struct NoteHitParticle
    {
        private static Random rand = new Random();

        private Vector3 position;
        public Vector3 Position
        {
            set { position = value; }
            get { return position; }
        }

        private Vector3 velocity;
        public Vector3 Velocity
        {
            set { velocity = value; }
            get { return velocity; }
        }

        private const int MAX_TIMER = 10000;
        private int timer;
        public int Timer 
        {
            set { timer = value; }
            get { return timer; }
        }

        private Color color;
        public Color Color
        {
            set { color = value; }
            get { return color; }
        }

        public NoteHitParticle(Vector3 position, Vector3 velocity, Color color)
        {
            this.position = position;
            this.velocity = velocity;
            this.timer = NewTime();
            this.color = color;
        }

        public static bool IsAlive(ref NoteHitParticle particle)
        {
            if (particle.Timer-- < 0)
            {
                particle.Timer = NewTime();
                return true;
            }
            return false;
        }

        private static int NewTime()
        {
            return (int)(rand.NextDouble() * MAX_TIMER);
        }
    }
}
