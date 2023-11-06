/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Model
{
    public class PlayerProgressEvents
    {
        public static Action<int, int> OnResourceHasChanged;

		#region Melee

		public static Action OnMeleeLevelUp;
        public static Action<MeleeUpgradeStartedArgs> OnMeleeUpgradeStarted;
        public static Action OnMeleeUpgradeCompleted;
        public static Action OnMeleeUpgradeClaimed;

		public struct MeleeUpgradeStartedArgs
		{
			public float duration;
			public float expiration;
		}

		#endregion
	}
}