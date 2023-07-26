using Godot;
using System;
using System.Collections.Generic;

namespace MechJamIV {
    public static class PickupHelper
    {

        #region Resources

    	private static readonly PackedScene medkitPickup = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/medkit_pickup.tscn");

    	private static readonly PackedScene grenadePickup = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/grenade_pickup.tscn");

        #endregion

        public static PickupBase GenerateRandomPickup(float probability)
        {
            if (RandomHelper.GetSingle() <= probability)
            {
                switch ((PickupType)RandomHelper.GetInt(2))
                {
                    case PickupType.Medkit:
                        return medkitPickup.Instantiate<PickupBase>();

                    case PickupType.Grenade:
                        return grenadePickup.Instantiate<PickupBase>();
                }
            }

            return null;
        }

    }
}