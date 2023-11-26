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
		public static Action<MeleeUpgradeClaimedArgs> OnMeleeUpgradeClaimed;

		public struct MeleeUpgradeStartedArgs
		{
			public float duration;
			public float expiration;
		}

		public struct MeleeUpgradeClaimedArgs
		{
			public int level;
			public int oldDamage;
			public int newDamage;
		}

		#endregion

		#region Repair/Fix

		public static Action<RepairSlotArgs> OnSlotFixDone;

		public struct RepairSlotArgs
		{
			public int objectId;
			public int resourceId;
			public int currentAmount;
			public int totalAmount;
			public bool allSlotsDone;
		}

		#endregion
	}
}