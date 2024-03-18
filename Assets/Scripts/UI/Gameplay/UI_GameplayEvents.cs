/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay
{
    public static class UI_GameplayEvents
    {
		/// <summary>
		/// Triggered when player was initialized;
		/// </summary>
		public static Action OnPlayerInitialization;

		/// <summary>
		/// Triggered when player was initialized;
		/// </summary>
		public static Action OnPlayerHasDie;

		/// <summary>
		/// Invoked when player starts the game from Title screen
		/// </summary>
		public static Action OnStartGame;

		/// <summary>
		/// Invoked when skip time panel is closed by player
		/// </summary>
		public static Action<UI.UI_ToolUpgradeSkipPanel.SkipPanelOpeningReason> OnCloseSkipTimePanel;

		public static Action<float> OnCameraSizeChanged;

		public static Action<float> OnStartRolling;

		#region Weapon events

		/// <summary>
		/// Triggered when UI slot weapon is selected and player should swap its current equipped weapon
		/// </summary>
		public static Action<string> OnWeaponSelection;

		/// <summary>
		/// Invoked when weapon radius detection has changed
		/// </summary>
		public static Action<float> OnWeaponRadiusChanged;

		/// <summary>
		/// Invoked from weapon slots
		/// </summary>
		public static Action<string> OnWeaponStartReloading;

		#endregion

		#region Repairing events

		/// <summary>
		/// Invoked when player reaches a repairable object area
		/// </summary>
		public static Action<RepairObjectData> OnShowRepairPanel;

		/// <summary>
		/// Invoked when player is going out from a repairable object area
		/// </summary>
		public static Action OnHideRepairPanel;

		/// <summary>
		/// Invoked when player pickups a repairing part
		/// </summary>
		public static Action<RepairPartData> OnPickupedRepairPart;

        /// <summary>
        /// Invoked when the repairing phase that involves a repair part was completed
        /// </summary>
        public static Action<RepairPartData> OnConsumedRepairPart;

		/// <summary>
		/// Invoked when player tries to fix a slot (second param) from an object (first param)
		/// </summary>
		public static Action<int, int> OnFixSlot;

		/// <summary>
		/// Invoked when player tries to start the object repairing
		/// </summary>
		public static Action<int> OnStartRepairing;

		/// <summary>
		/// Invoked by player when it wants to skip the progress
		/// </summary>
		public static Action<int, float, int> OnSkipRepairing;

		/// <summary>
		/// Invoked when player wants to skip the repairing in progress
		/// </summary>
		public static Action<int> OnSpeedUpRepairing;

		#endregion

		#region Tool events

		public static Action OnShowUpgradeToolInformation;

		public static Action OnShowUpgradeToolProgress;

		public static Action OnShowUpgradeToolClaim;

		public static Action OnHideUpgradeTool;

		public static Action OnStartToolUpgrade;

		public static Action OnClaimToolUpgrade;

		public static Action OnSkipToolUpgrade;

		public static Action OnSpeedUpToolUpgrade;

		public static Action OnIncreaseToolDamage;

		public static Action<MeleeUpgradeRequirementData[]> OnConsumeResourcesByToolUpgrade;

		public static Action OnTriggerToolAreaAssistance;

		public static Action OnUpgradeToolAreaInteracion;

		#endregion

		#region Mission events

		public static Action OnTriggerMissionAssistance;
		public static Action OnCancelMissionAssistance;
		public static Action OnMissionAssistanceFinished;
		public static Action OnStopMissionAssistance;

		#endregion

		#region Crafting events

		/// <summary>
		/// Invoked when player starts a crafting process. [int areaId, int amount]
		/// </summary>
		public static Action<int, int> OnCraftingStarted;

		/// <summary>
		/// Invoked when player claims a completed crafting process. [int areaId]
		/// </summary>
		public static Action<int> OnCraftingClaimed;

		/// <summary>
		/// Invoked when player tries to skip the crafting in progress. [int areaId]
		/// </summary>
		public static Action<int> OnCraftingSkipped;

		/// <summary>
		/// Invoked when player skips the crafting area progress.
		/// </summary>
		public static Action<int, int> OnCraftingSpeedUp;

		#endregion

		#region Cheat events

		public struct UpdateResourceByCheatArgs
		{
			public int resourceId;
			public int amount;
		}

		public struct AutofireByCheatArgs
		{
			public bool enabled;
			public bool movementFree;
		}

		public static Action<UpdateResourceByCheatArgs> OnGrantResourceByCheat;
		public static Action<UpdateResourceByCheatArgs> OnConsumeResourceByCheat;
		public static Action<AutofireByCheatArgs> OnAutofireChangedByCheat;

		#endregion
	}
}