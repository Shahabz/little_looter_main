/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Model
{
	public class PlayerProgressEvents
	{
		public static int SKIP_TIME_SECS = 900;	// 15 mins

		public static Action<int, int> OnResourceHasChanged;

		#region Tool

		public static Action OnMeleeLevelUp;
		public static Action<MeleeUpgradeStartedArgs> OnMeleeUpgradeStarted;
		public static Action OnMeleeUpgradeCompleted;
		public static Action<MeleeUpgradeClaimedArgs> OnMeleeUpgradeClaimed;
		public static Action<ToolExtraDamageStartedArgs> OnToolDamageIncreaseStarted;
		public static Action OnToolDamageIncreaseCompleted;

		public struct MeleeUpgradeStartedArgs
		{
			public float duration;
			public float expiration;
			public int currentDamage;
			public int nextLevelDamage;
		}

		public struct MeleeUpgradeClaimedArgs
		{
			public int level;
			public int oldDamage;
			public int newDamage;
		}

		public struct ToolExtraDamageStartedArgs
		{
			public float expiration;
			public int duration;
		}

		#endregion

		#region Repair/Fix

		public static Action<RepairSlotArgs> OnSlotFixDone;

		public static Action<RepairStartActionArgs> OnStartRepairing;

		public static Action<int> OnCompleteRepairing;

		public static Action<RepairSpeedUpArgs> OnSpeedUpRepairing;

		public struct RepairSlotArgs
		{
			public int objectId;
			public int resourceId;
			public int currentAmount;
			public int totalAmount;
			public bool allSlotsDone;
		}

		public struct RepairStartActionArgs
		{
			public int id;
			public float expiration;
			public int duration;
		}

		public struct RepairSpeedUpArgs
		{
			public int id;
			public float expiration;
			public int duration;
		}

		#endregion

		#region Crafting

		public static Action<PlayerProgress_CraftingAreaData> OnCraftingAreaProcessStarted;
		public static Action<PlayerProgress_CraftingAreaData> OnCraftingAreaProcessCompleted;
		public static Action<PlayerProgress_CraftingAreaData> OnCraftingAreaProcessClaimed;
		public static Action<PlayerProgress_CraftingAreaData> OnCraftingAreaProcessSpeedUp;

		#endregion
	}
}