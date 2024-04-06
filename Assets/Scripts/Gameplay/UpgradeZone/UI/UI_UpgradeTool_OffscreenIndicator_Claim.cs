/*
 * Date: April 1st, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using System;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
	/// In charge of showing it when the tool upgrade was completed and it is ready to be claimed
	/// </summary>
    public class UI_UpgradeTool_OffscreenIndicator_Claim : UI_TargetIndicator
    {
		#region Override methods

		protected override void Init()
		{
			PlayerProgressEvents.OnMeleeUpgradeCompleted += HandlePlayerToolUpgradeCompleted;
			PlayerProgressEvents.OnMeleeUpgradeClaimed += HandlePlayerToolUpgradeClaimed;

			Hide();
		}

		protected override void Teardown()
		{
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= HandlePlayerToolUpgradeCompleted;
			PlayerProgressEvents.OnMeleeUpgradeClaimed -= HandlePlayerToolUpgradeClaimed;
		}

		#endregion

		#region Private methods

		private void HandlePlayerToolUpgradeCompleted()
		{
			Show();
		}

		private void HandlePlayerToolUpgradeClaimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
			Hide();
		}

		#endregion
	}
}