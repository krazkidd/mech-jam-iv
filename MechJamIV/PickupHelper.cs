using Godot;
using System;

namespace MechJamIV
{
    public static class PickupHelper
    {

        #region Resources

        private static readonly PackedScene medkit = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/medkit.tscn");
        private static readonly PackedScene grenade = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade.tscn");
        private static readonly PackedScene missile = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile.tscn");

        private static readonly PackedScene hitScanBulletEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/hit_scan_bullet_emitter.tscn");
        private static readonly PackedScene grenadeEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade_emitter.tscn");
        private static readonly PackedScene missileEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile_emitter.tscn");

        #endregion

        public static PickupType? GenerateRandomPickup(float probability)
        {
            if (GD.Randf() <= probability)
            {
                switch (GD.Randi() % 3)
                {
                    case 0:
                        return PickupType.Medkit;

                    case 1:
                        return PickupType.Grenade;

                    case 2:
                        return PickupType.Missile;
                }
            }

            return null;
        }

        public static Projectile GenerateProjectile(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Medkit:
                    return medkit.Instantiate<Projectile>();

                case PickupType.Grenade:
                    return grenade.Instantiate<Projectile>();

                case PickupType.Missile:
                    return missile.Instantiate<Missile>();

                default:
                    // this should never happen
                    System.Diagnostics.Debug.Assert(false, "Invalid switch case");

                    break;
            }

            throw new ArgumentException($"Pickup type {Enum.GetName(pickupType)} is not a projectile type");
        }

        public static WeaponBase GenerateWeapon(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Rifle:
                    return hitScanBulletEmitter.Instantiate<HitScanBulletEmitter>();

                case PickupType.Grenade:
                    return grenadeEmitter.Instantiate<ProjectileEmitter>();

                case PickupType.Missile:
                    return missileEmitter.Instantiate<ProjectileEmitter>();

                default:
                    // this should never happen
                    System.Diagnostics.Debug.Assert(false, "Invalid switch case");

                    break;
            }

            throw new ArgumentException($"Pickup type {Enum.GetName(pickupType)} is not a weapon type");
        }

    }
}