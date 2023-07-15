using Godot;
using System;
using MechJamIV;

public partial class Robot : CharacterBase
{

    public override Vector2 FaceDirection { get; set; } = Vector2.Zero;

	#region Node references

	private Player player;

	#endregion

    public override void _Ready()
    {
		base._Ready();

		player = (Player)GetTree().GetFirstNodeInGroup("player");
    }

	protected override Vector2 GetMovementDirection() => GlobalTransform.Origin.DirectionTo(player.RobotMarker.GlobalTransform.Origin);

    protected override bool IsJumping() => false;

    protected override void ProcessAction()
	{
		//TODO
	}

	protected override void AnimateInjury(int damage, Vector2 position, Vector2 normal)
	{
		//TODO
	}

    public override void Hurt(int damage, Vector2 position, Vector2 normal)
    {
		// ignore damage
        //base.Hurt(damage, position, normal);
    }

}
