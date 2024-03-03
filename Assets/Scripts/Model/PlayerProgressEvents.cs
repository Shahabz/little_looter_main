/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay;
using System;

namespace LittleLooters.Model
{
	public class PlayerProgressEvents
	{
		#region Resources

		public static Action<int, int> OnResourceHasChanged;

		#endregion

		#region Weapons

		public struct WeaponAmmoChangeArgs
		{
			public string id;
			public int ammo;
		}

		public struct WeaponStartReloadingArgs
		{
			public string id;
			public float expiration;
			public float duration;
		}

		public struct WeaponStopReloadingArgs
		{
			public string id;
		}

		public struct WeaponSelectionArgs
		{
			public string id;
			public bool isSelected;
		}

		public static Action<AssaultWeaponData> OnWeaponChanged;

		public static Action<WeaponAmmoChangeArgs> OnWeaponAmmoChanged;

		public static Action<WeaponStartReloadingArgs> OnWeaponStartReloading;

		public static Action<WeaponStopReloadingArgs> OnWeaponStopReloading;

		public static Action<WeaponSelectionArgs> OnWeaponSelectionChanged;

		#endregion

		#region Tool

		public static Action OnMeleeLevelUp;
		public static Action<MeleeUpgradeStartedArgs> OnMeleeUpgradeStarted;
		public static Action OnMeleeUpgradeCompleted;
		public static Action<MeleeUpgradeClaimedArgs> OnMeleeUpgradeClaimed;
		public static Action<ToolExtraDamageStartedArgs> OnToolDamageIncreaseStarted;
		public static Action OnToolDamageIncreaseCompleted;
		public static Action<ToolUpgradeExpirationChangedArgs> OnToolUpgradeExpirationHasChanged;

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

		public struct ToolUpgradeExpirationChangedArgs
		{
			public float duration;
			public float expiration;
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

		#region Missions

		/// <summary>
		/// Invoke when player's mission is moved to the next one
		/// </summary>
		public static Action OnMoveToNextMission;

		#endregion
	}
}