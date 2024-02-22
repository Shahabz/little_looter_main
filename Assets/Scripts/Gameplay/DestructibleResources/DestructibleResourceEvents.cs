/*
 * Date: October 29th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay
{
    public class DestructibleResourceEvents
    {
        public static Action<DestructibleResourceApplyDamageArgs> OnApplyDamage;

        public static Action<int> OnDestroyed;

        public static Action<int, int> OnGrantRewardsByDamage;

        public static Action OnStopByPlayerMovement;
	}

	public struct DestructibleResourceApplyDamageArgs
	{
        public int id;
        public DestructibleResourceType type;
    }

    public enum DestructibleResourceType 
    {
        NONE,
        SMALL_BOX,
        GRILL,
        DOOR,
    }
}