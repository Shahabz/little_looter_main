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

		#endregion

		#region Tool events

		public static Action OnStartToolUpgrade;

		public static Action OnClaimToolUpgrade;

		public static Action OnSkipToolUpgrade;

		public static Action OnSpeedUpToolUpgrade;

		#endregion

		#region Mission events

		public static Action OnTriggerMissionAssistance;
		public static Action OnCancelMissionAssistance;
		public static Action OnMissionAssistanceFinished;
		public static Action OnStopMissionAssistance;

		#endregion
	}
}