using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class HitScanBulletEmitter : Node2D
{

	[Export]
	public float HitScanDistance { get; set; } = 10000.0f;

	private Godot.Collections.Array<Rid> _bodiesToExclude = new ();

	public int Damage { get; set; } = 1;

	// (not) used in Fire()
	//private Vector2 gravity = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	#region Resources

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/shrapnel_splatter.tscn");

	#endregion

	public void SetBodiesToExclude(IEnumerable<Rid> resourceIds)
	{
		_bodiesToExclude = new Godot.Collections.Array<Rid>(resourceIds);
	}

	public void Fire(Vector2 mousePos)
	{
		Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
		{
			From = GlobalTransform.Origin,
			To = mousePos.Normalized() * HitScanDistance,
			Exclude = _bodiesToExclude,
			CollideWithBodies = true,
			CollideWithAreas = true,
			CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Environment | CollisionLayerMask.Hitbox)
		});

		if (collision.ContainsKey("collider"))
		{
			Vector2 position = collision["position"].AsVector2();
			Vector2 normal = collision["normal"].AsVector2();

			if (collision["collider"].Obj is Hitbox hitbox)
			{
				hitbox.Hurt(Damage, position, normal);
			}
			else if (collision["collider"].Obj is Barrel barrel)
			{
				barrel.Hurt(Damage, position,  normal);
			}
			//BUG: Grenades are not currently in the Environment layer,
			//     so this doesn't work. (See kanban task.)
			else if (collision["collider"].Obj is Grenade grenade)
			{
				grenade.Hurt(Damage, position,  normal);
			}
			else if (collision["collider"].Obj is GrenadePickup grenadePickup)
			{
				grenadePickup.Hurt(Damage, position, normal);
			}
			else
			{
				// world or environment hit

				GpuParticles2D splatter = shrapnelSplatter.Instantiate<GpuParticles2D>();

				GetTree().CurrentScene.AddChild(splatter);

				splatter.GlobalPosition = position;

				// if (Mathf.IsZeroApprox(normal.AngleTo(-gravity)))
				// {
				// 	//TODO why do we return here? this causes ground shots to not have the effect
				// 	return;
				// }
				// else if (Mathf.IsZeroApprox(normal.AngleTo(gravity)))
				// {
				// 	splatter.Rotate(Mathf.Pi);

				// 	//TODO why do we return here? presumably this would cause ceiling shots to not have an effect
				// 	return;
				// }

 				splatter.Emitting = true;

				splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
			}
		}
	}

}
