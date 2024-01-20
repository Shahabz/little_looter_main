/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerUpgradeZoneService : MonoBehaviour
    {
		#region Private properties

		private const string _tag = "UpgradeZone";

		#endregion

		#region Unity events

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<UpgradeZoneController>(out var target)) return;

			target.ShowIndicator();
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<UpgradeZoneController>(out var target)) return;

			target.HideIndicator();
		}

		private void Update()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			if (!progressDataService.ProgressData.toolData.isUpgrading) return;

			if (Time.time < progressDataService.ProgressData.toolData.upgradeExpiration) return;

			progressDataService.Tool_CompleteUpgrade();
		}

		#endregion
	}
}