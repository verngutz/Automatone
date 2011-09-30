using System;

using Microsoft.Xna.Framework;

using Nuclex.Graphics.SpecialEffects.Particles;

namespace Automatone.GUI
{
    public class NoteHitParticleModifier : IParticleModifier<NoteHitParticle>
    {
        public bool HasVelocity { get { return true; } }
        public bool HasWeight { get { return false; } }

        public void GetPosition(ref NoteHitParticle particle, out Vector3 position)
        {
            position = particle.Position;
        }

        public void GetVelocity(ref NoteHitParticle particle, out Vector3 velocity)
        {
            velocity = particle.Velocity;
        }

        public float GetWeight(ref NoteHitParticle particle)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(ref NoteHitParticle particle, ref Vector3 position)
        {
            position = particle.Position;
        }

        public void SetVelocity(ref NoteHitParticle particle, ref Vector3 velocity)
        {
            velocity = particle.Velocity;
        }

        public void SetWeight(ref NoteHitParticle particle, float weight)
        {
            throw new NotImplementedException();
        }

        public void AddScaledVelocityToPosition(NoteHitParticle[] particles, int start, int count, float scale)
        {
            int end = start + count;
            for (int i = start; i < end; i++)
            {
                particles[i].Position += particles[i].Velocity * scale;
            }
        }

        public void AddToVelocity(NoteHitParticle[] particles, int start, int count, ref Vector3 velocityAdjustment)
        {
            int end = start + count;
            for (int i = start; i < end; i++)
            {
                particles[i].Velocity += velocityAdjustment;
            }
        }

        public void AddVelocityToPosition(NoteHitParticle[] particles, int start, int count)
        {
            int end = start + count;
            for (int i = start; i < end; i++)
            {
                particles[i].Position += particles[i].Velocity;
            }
        }
    }
}
