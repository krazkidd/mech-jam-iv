using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Player : CharacterBase
{

	[Signal]
	public delegate void ImmunityShieldActivatedEventHandler();
	[Signal]
	public delegate void ImmunityShieldDeactivatedEventHandler();

	#region Node references

	public Marker2D RobotMarker { get; private set; }
	public RemoteTransform2D RemoteTransform { get; private set; }
	public WeaponManager WeaponManager { get; private set; }

	private Timer immunityTimer;
	private GpuParticles2D immunityShield;

	#endregion

	#region Resources

	private PackedScene bloodSplatterResource = ResourceLoader.Load<PackedScene>("res://scenes/effects/blood_splatter.tscn");

	#endregion

    public override void _Ready()
    {
		base._Ready();

		RobotMarker = GetNode<Marker2D>("RobotMarker");
		RemoteTransform = GetNode<RemoteTransform2D>("RemoteTransform");

		immunityTimer = GetNode<Timer>("ImmunityTimer");
		immunityTimer.Timeout += () => DeactivateShield();

		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");

		WeaponManager = GetNode<WeaponManager>("WeaponManager");
    }

	protected override Vector2 GetMovementDirection()
	{
		Vector2 dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		return new Vector2(dir.X, 0.0f).Normalized();
	}

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	public void Fire(FireMode mode, Vector2 globalPos)
	{
		if (Health <= 0)
		{
			return;
		}

		WeaponManager.Fire(mode, globalPos);
	}

	public override void Hurt(int damage, Vector2 position, Vector2 normal)
	{
		if (immunityTimer.TimeLeft > 0)
		{
			return;
		}

		base.Hurt(damage, position, normal);

		if (Health <= 0)
		{
			return;
		}

		ActivateShield();

		immunityTimer.Start();
	}

	protected override async void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        GpuParticles2D splatter = bloodSplatterResource.Instantiate<GpuParticles2D>();
		splatter.GlobalPosition = position;
		splatter.Emitting = true;

		await GetTree().CurrentScene.AddChildDeferred(splatter);

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

	public void ActivateShield()
	{
		immunityShield.Visible = true;

		EmitSignal(SignalName.ImmunityShieldActivated);
	}

	public void DeactivateShield()
	{
		immunityShield.Visible = false;

		EmitSignal(SignalName.ImmunityShieldDeactivated);
	}

}
