/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionsService : MonoBehaviour
    {
        [SerializeField] private MissionConfigurationData[] _missions = default;

        private int _missionId = -1;
        private PlayerMissionTriggerDestruction _triggerDestruction = default;
        private PlayerMissionTriggerToolUpgrade _triggerToolUpgrade = default;

        private void Awake()
		{
            _triggerDestruction = new PlayerMissionTriggerDestruction();
            _triggerDestruction.Initialize(DestructionCompleted);

            _triggerToolUpgrade = new PlayerMissionTriggerToolUpgrade();
            _triggerToolUpgrade.Initialize(ToolUpgradeCompleted);

            FirstMission();
		}

        private void OnDestroy()
		{
            _triggerDestruction.Teardown();
            _triggerToolUpgrade.Teardown();
        }

        private void FirstMission()
        {
            _missionId = 0;

            var mission = _missions[_missionId];

            StartTrigger(mission);

            PlayerMissionsEvents.OnInitialization?.Invoke(_missions[_missionId]);
        }

        private void NextMission()
		{
            if (_missionId >= _missions.Length)
			{
                // TODO: reached last mission

                return;
			}

            var mission = _missions[_missionId];

            // Check triggers based on mission
            StartTrigger(mission);

            PlayerMissionsEvents.OnMoveToMission?.Invoke(_missions[_missionId]);
        }

        private void ToolUpgradeCompleted()
		{
            _missionId++;

            NextMission();
		}

        private void DestructionCompleted()
		{
            _missionId++;

            NextMission();
		}

        private void StartTrigger(MissionConfigurationData data)
		{
            _triggerDestruction.ResetStatus(data.Type, data);
            _triggerToolUpgrade.ResetStatus(data.Type, data);
		}
    }
}