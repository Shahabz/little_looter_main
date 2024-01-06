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
		/// Invoked when skip time panel is closed by player
		/// </summary>
		public static Action<UI.UI_ToolUpgradeSkipPanel.SkipPanelOpeningReason> OnCloseSkipTimePanel;

		public static Action<float> OnCameraSizeChanged;

		#region Weapon swapping events

		/// <summary>
		/// Triggered when UI slot weapon is selected and player should swap its current equipped weapon
		/// </summary>
		public static Action<int> OnWeaponSelection;

		#endregion

		#region Repairing events

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

		public static Action OnStartToolUpgrade;

		public static Action OnClaimToolUpgrade;

		public static Action OnSkipToolUpgrade;

		public static Action OnSpeedUpToolUpgrade;

		public static Action OnIncreaseToolDamage;

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

		public static Action<UpdateResourceByCheatArgs> OnGrantResourceByCheat;
		public static Action<UpdateResourceByCheatArgs> OnConsumeResourceByCheat;

		#endregion
	}
}