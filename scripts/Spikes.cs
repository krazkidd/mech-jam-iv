using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Spikes : Area2D
{

	[Export]
	public int Damage { get; set; } = 10;

	private Dictionary<Rid, Player> collidingBodies = new ();

	public override void _Ready()
	{
		BodyEntered += async (body) =>
		{
			if (body is Player player)
			{
				collidingBodies.Add(player.GetRid(), player);

				await player.HurtAsync(Damage, Vector2.Zero);
			}
		};
		BodyExited += (body) =>
		{
			if (body is Player player)
			{
				collidingBodies.Remove(player.GetRid());
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		if (collidingBodies.Any())
		{
			foreach (KeyValuePair<Rid, Player> kvp in collidingBodies)
			{
				kvp.Value.HurtAsync(Damage, Vector2.Zero);
			}
		}
	}

}
