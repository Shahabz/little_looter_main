/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.UI;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class UpgradeZoneController : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
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
			_uiClaimPanel.Hide();
			_uiPanel.Hide();
			//_uiInProgressPanel.Hide();

			_indicator.SetActive(true);

			var meleeIsUpgrading = _playerEntryPoint.ProgressData.meleeData.isUpgrading;

			if (meleeIsUpgrading)
			{
				// Show upgrade in progress panel
				//var duration = _playerEntryPoint.GetMeleeNextLevelData().upgradeTime;
				//var expiration = _playerEntryPoint.ProgressData.meleeData.upgradeExpiration;

				_uiInProgressPanel.Show();	// duration, expiration);

				return;
			}

			var isMeleeClaimExpected = _playerEntryPoint.ProgressData.meleeData.toClaim;

			if (isMeleeClaimExpected)
			{
				// Show claim panel
				_uiClaimPanel.Show();

				return;
			}

			var currentLevelData = _playerEntryPoint.GetCurrentToolLevelData();
			var nextLevelData = _playerEntryPoint.GetMeleeNextLevelData();

			_uiPanel.Show(_playerEntryPoint.ProgressData.resourcesData, currentLevelData, nextLevelData);
		}

		public void HideIndicator()
		{
			_indicator.SetActive(false);

			_uiPanel.Hide();
		}

		#endregion
	}
}