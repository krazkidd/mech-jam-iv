using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class World : Node2D
{

	[Export]
	public PackedScene NextScene { get; set; } = null;

	#region Node references

	private Player player;
	private PlayerCamera playerCamera;

	private IList<Spawn> spawns;
	private Spawn activeSpawn;

	private PauseScreen pauseScreen;

	#endregion

	public override void _Ready()
	{
		InitPauseScreen();

		InitSpawns();
		InitPickups();
		InitEnemies();
		InitObjectives();

		player = (Player)GetTree().GetFirstNodeInGroup("player");
		player.GlobalTransform = activeSpawn.SpawnPointMarker.GlobalTransform;

		playerCamera = GetNode<PlayerCamera>("PlayerCamera");
		playerCamera.TrackPlayer(player);
	}

	private void InitPauseScreen()
	{
		pauseScreen = ResourceLoader.Load<PackedScene>("res://scenes/pause_screen.tscn").Instantiate<PauseScreen>();

		pauseScreen.Visible = false;

		pauseScreen.ContinueClicked += () =>
		{
			pauseScreen.Visible = false;

			GetTree().Paused = false;
		};
		pauseScreen.RestartClicked += () =>
		{
			// if (player.Health <= 0)
			// {
			GetTree().ReloadCurrentScene();

			GetTree().Paused = false;
			// }
			// else
			// {
			// 	player.GlobalTransform = activeSpawn.SpawnPointMarker.GlobalTransform;
			// }
		};
		pauseScreen.QuitClicked += () =>
		{
			GetTree().Quit();
		};

		AddChild(pauseScreen);
	}

	private void InitSpawns()
	{
		spawns = new List<Spawn>();
		foreach (Spawn spawn in GetTree().GetNodesInGroup("spawn").OfType<Spawn>())
		{
			spawns.Add(spawn);

			if (activeSpawn == null || spawn.IsWorldSpawn)
			{
				activeSpawn = spawn;
			}

			spawn.SpawnReached += (player) =>
			{
				activeSpawn = spawn;
			};
		}
	}

	private void InitPickups()
	{
		foreach (PickupBase pickup in GetTree().GetNodesInGroup("pickup").OfType<PickupBase>())
		{
			pickup.PickedUp += () => Pickup(pickup);
		}
	}

	private void InitEnemies()
	{
		foreach (EnemyBase enemy in GetTree().GetNodesInGroup("enemy").OfType<EnemyBase>())
		{
			enemy.PickupDropped += (pickup) =>
			{
				GetTree().CurrentScene.AddChild(pickup);

				pickup.PickedUp += () => Pickup(pickup);
			};
		}
	}

	private void InitObjectives()
	{
		foreach (Objective objective in GetTree().GetNodesInGroup("objective").OfType<Objective>())
		{
			objective.ObjectiveReached += () =>
			{
				//TODO
				GetTree().ChangeSceneToPacked(NextScene);
			};
		}
	}

    public override void _Process(double delta)
    {
#if DEBUG
		QueueRedraw();
#endif
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("quit"))
		{
			GetTree().Paused = true;

			pauseScreen.Visible = true;
		}
		else if (Input.IsActionJustPressed("throw_grenade"))
		{
			player.ThrowGrenade(playerCamera.GetGlobalMousePosition());
		}
		//TODO?
		// else if (Input.IsActionJustPressed("fire"))
		// {
		// 	player.FireGun(playerCamera.GetGlobalMousePosition());
		// }
		else if (Input.IsActionPressed("fire"))
		{
			player.FireGun(playerCamera.GetGlobalMousePosition());
		}
    }

	private void Pickup(PickupBase pickup)
	{
		switch (pickup.PickupType)
		{
			case PickupType.Medkit:
				player.Heal(50);

				break;
			case PickupType.Grenade:
				player.GrenadeCount++;

				break;
		}
	}

}
