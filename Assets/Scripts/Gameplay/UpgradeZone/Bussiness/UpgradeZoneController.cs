/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.UI;
using LittleLooters.Global.ServiceLocator;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class UpgradeZoneController : MonoBehaviour
    {
		#region Inspector

        [SerializeField] private GameObject _indicator = default;
		[SerializeField] private UI_UpgradeZone_Panel _uiPanel = default;
		[SerializeField] private UI_UpgradeZone_ClaimPanel _uiClaimPanel = default;
		[SerializeField] private UI_UpgradeZone_InProgressPanel _uiInProgressPanel = default;
		[SerializeField] private Transform _pivotAssistance = default;

		#endregion

		#region Public properties

		public Transform PivotAssistance => _pivotAssistance;

		#endregion

		#region Public methods

		public void ShowIndicator()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			_uiClaimPanel.Hide();
			_uiPanel.Hide();
			//_uiInProgressPanel.Hide();

			_indicator.SetActive(true);

			var toolIsUpgrading = progressDataService.ProgressData.toolData.isUpgrading;

			if (toolIsUpgrading)
			{
				_uiInProgressPanel.Show();

				return;
			}

			var isMeleeClaimExpected = progressDataService.ProgressData.toolData.toClaim;

			if (isMeleeClaimExpected)
			{
				// Show claim panel
				_uiClaimPanel.Show();

				return;
			}

			var currentLevelData = progressDataService.Tool_GetCurrentLevelData();
			var nextLevelData = progressDataService.Tool_GetNextLevelData();

			_uiPanel.Show(progressDataService.ProgressData.resourcesData, currentLevelData, nextLevelData);
		}

		public void HideIndicator()
		{
			_indicator.SetActive(false);

			_uiPanel.Hide();
		}

		#endregion
	}
}