/*
 * Date: February 29th, 2024
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	public class UI_UpgradeTool_Panel : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private UI_UpgradeTool_InfoPanel _infoPanel = default;
		[SerializeField] private UI_UpgradeTool_ProgressPanel _progressPanel = default;
		[SerializeField] private UI_UpgradeTool_ClaimPanel _claimPanel = default;

		#endregion

		#region Private properties

		private bool _isShowing = false;

		#endregion

		#region Unity events

		private void Awake()
		{
			SubscribeEvents();
		}

		private void OnDestroy()
		{
			UnsubscribeEvents();
		}

		#endregion

		#region Private methods

		private void SubscribeEvents()
		{
			UI_GameplayEvents.OnShowUpgradeToolInformation += HandleShowInformationPanel;
			UI_GameplayEvents.OnShowUpgradeToolProgress += HandleShowProgressPanel;
			UI_GameplayEvents.OnShowUpgradeToolClaim += HandleShowClaimPanel;
			UI_GameplayEvents.OnHideUpgradeTool += HandleHidePanel;
			PlayerProgressEvents.OnMeleeUpgradeStarted += HandleToolUpgradeStarted;
			PlayerProgressEvents.OnMeleeUpgradeCompleted += HandleToolUpgradeCompleted;
		}

		private void UnsubscribeEvents()
		{
			UI_GameplayEvents.OnShowUpgradeToolInformation -= HandleShowInformationPanel;
			UI_GameplayEvents.OnShowUpgradeToolProgress -= HandleShowProgressPanel;
			UI_GameplayEvents.OnShowUpgradeToolClaim -= HandleShowClaimPanel;
			UI_GameplayEvents.OnHideUpgradeTool -= HandleHidePanel;
			PlayerProgressEvents.OnMeleeUpgradeStarted -= HandleToolUpgradeStarted;
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= HandleToolUpgradeCompleted;
		}

		private void HandleShowInformationPanel()
		{
			_isShowing = true;

			HideAllPanels();

			_infoPanel.Show();
		}

		private void HandleShowProgressPanel()
		{
			_isShowing = true;

			HideAllPanels();

			_progressPanel.Show();
		}

		private void HandleShowClaimPanel()
		{
			_isShowing = true;

			HideAllPanels();

			_claimPanel.Show();
		}

		private void HandleHidePanel()
		{
			_isShowing = false;

			HideAllPanels();
		}

		private void HideAllPanels()
		{
			_infoPanel.Hide();
			_progressPanel.Hide();
			_claimPanel.Hide();
		}

		private void HandleToolUpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
		{
			HideAllPanels();

			_progressPanel.Show();
		}

		private void HandleToolUpgradeCompleted()
		{
			if (!_isShowing) return;

			HideAllPanels();

			_claimPanel.Show();
		}

		#endregion
	}
}