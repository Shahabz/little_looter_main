/*
 * Date: April 1st, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// In charge of showing it when the tool upgrade requirements are ready
	/// </summary>
    public class UI_UpgradeTool_OffscreenIndicator_Upgrade : UI_TargetIndicator
    {
		#region Override methods

		protected override void Init()
		{
			PlayerProgressEvents.OnResourceHasChanged += HandlePlayerResourcesChanged;
			PlayerProgressEvents.OnMeleeUpgradeStarted += HandlePlayerToolUpgradeStarted;

			Hide();
		}

		protected override void Teardown()
		{
			PlayerProgressEvents.OnResourceHasChanged -= HandlePlayerResourcesChanged;
			PlayerProgressEvents.OnMeleeUpgradeStarted -= HandlePlayerToolUpgradeStarted;
		}

		#endregion

		#region Private methods

		private void HandlePlayerResourcesChanged(int resourceId, int upToDateAmount)
		{
			// Get player's progress data
			var service = ServiceLocator.Current.Get<PlayerProgressDataService>();
			var playerProgress = service.ProgressData;
			var playerToolData = playerProgress.toolData;

			// If is upgrading, skip it
			if (playerToolData.isUpgrading) return;

			// If it is ready to be claimed, skip it
			if (playerToolData.toClaim) return;

			var playerResourcesData = playerProgress.resourcesData;
			var playerToolNextLevelData = service.Tool_GetNextLevelData();

			// Check if current player's tool requirements are achieved
			var requirementsAchieved = playerToolData.CheckUpgradeRequirementsReady(playerResourcesData, playerToolNextLevelData);

			if (!requirementsAchieved)
			{
				Hide();
				return;
			}

			Show();
		}

		private void HandlePlayerToolUpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
		{
			Hide();
		}

		#endregion
	}
}