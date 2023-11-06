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

		#endregion

		#region Melee events

		public static Action OnStartMeleeUpgrade;

		public static Action OnClaimMeleeUpgrade;

		#endregion
	}
}