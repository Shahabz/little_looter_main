/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class MissionUpgradeToolAssistance : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Vector3 _verticalOffset = default;

		#endregion

		#region Private properties

		private bool _initialized = false;
        private UpgradeZoneController _upgradeToolZone = default;
        private bool _upgradeToolZoneFound = false;

		#endregion

		#region Unity events

		private void OnEnable()
		{
            if (_initialized) return;

            _upgradeToolZone = FindObjectOfType<UpgradeZoneController>();

            _initialized = true;

            _upgradeToolZoneFound = _upgradeToolZone != null;
		}

		#endregion

		#region Public methods

		public (Transform target, Vector3 targetOffset, bool found) Process(MissionConfigurationData mission, PlayerProgressData playerProgressData)
        {
            if (_canDebug) DebugProcess(mission);

            return (_upgradeToolZone.PivotAssistance, _verticalOffset, _upgradeToolZoneFound);
        }

		#endregion

		#region Debug

		private bool _canDebug = false;

        private void DebugProcess(MissionConfigurationData mission)
		{
            Debug.LogError($"<color=magenta>UPGRADE TOOL</color> -> Current mission <color=yellow>'{mission.Description}'</color>, type: <color=orange>{mission.Type}</color>, target: <color=magenta>'{_upgradeToolZone}'</color>");
        }

		#endregion
	}
}