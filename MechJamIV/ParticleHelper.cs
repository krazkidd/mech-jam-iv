using Godot;
using System;

namespace MechJamIV
{
    public static class ParticleHelper
    {

        public static async void EmitParticlesOnce(this Node node, GpuParticles2D particles, Vector2 globalPos)
        {
            particles.GlobalPosition = globalPos;

            particles.Emitting = true;

            await node.GetTree().CurrentScene.AddChildDeferred(particles);

            particles.TimedFree(particles.Lifetime + (particles.Lifetime * particles.Randomness));
        }

    }
}