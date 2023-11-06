/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerUpgradeZoneService : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;

		#endregion

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
			if (!_playerEntryPoint.ProgressData.meleeData.isUpgrading) return;

			if (Time.time < _playerEntryPoint.ProgressData.meleeData.upgradeExpiration) return;

			_playerEntryPoint.CompleteMeleeUpgrade();
		}

		#endregion
	}
}