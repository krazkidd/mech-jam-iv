using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MechJamIV;

public partial class Missile : Grenade
{

	[Export]
	public float ThrustForce { get; set; } = 5_000.0f;

	[Export]
	public float TurnSpeed { get; set; } = 3_000f;

	#region Node references

	public CharacterTracker CharacterTracker { get; private set;}

	private GpuParticles2D gpuParticles2D;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		CharacterTracker = GetNodeOrNull<CharacterTracker>("CharacterTracker");

		gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

		BodyEntered += (body) => Hurt(Health, GlobalTransform.Origin, Vector2.Zero);
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Health <= 0)
		{
			return;
		}

		if (CharacterTracker.Target != null)
		{
			Vector2 directionToTarget = CharacterTracker.GetDirectionToTarget();

			float angleDiff = Mathf.RadToDeg(CharacterAnimator.SpriteFaceDirection.Rotated(Rotation).AngleTo(directionToTarget));
			int turnDirection = Mathf.Sign(angleDiff);

			float rotation = TurnSpeed * (float)delta;

			if (Mathf.Abs(angleDiff) < rotation)
			{
				Rotate(Mathf.DegToRad(angleDiff));
			}
			else
			{
				Rotate(Mathf.DegToRad(rotation) * turnDirection);
			}
		}

		ApplyForce(CharacterAnimator.SpriteFaceDirection.Rotated(Rotation) * ThrustForce * (float)delta);
    }

	protected override void AnimateDeath()
	{
		base.AnimateDeath();

		// allow emitted particles to decay
		gpuParticles2D.Emitting = false;
	}

	#region IDestructible

	public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		base.Hurt(damage, globalPos, normal);

		if (Health <= 0)
		{
			CharacterTracker.Untrack();
		}
	}

	#endregion

}
