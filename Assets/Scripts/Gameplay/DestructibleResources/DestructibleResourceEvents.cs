/*
 * Date: October 29th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay
{
    public class DestructibleResourceEvents
    {
        public static Action OnApplyDamage;

        public static Action<int> OnDestroyed;

        public static Action<int, int> OnGrantRewardsByDamage;

        public static Action OnStopByPlayerMovement;
    }
}