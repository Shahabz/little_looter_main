/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionsService : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private MissionConfigurationData[] _missions = default;

		#endregion

		#region Private properties

		private int _missionId = -1;
        private PlayerMissionTriggerDestruction _triggerDestruction = default;
        private PlayerMissionTriggerToolUpgrade _triggerToolUpgrade = default;
        private PlayerMissionTriggerExploration _triggerExploration = default;
        private PlayerMissionTriggerCrafting _triggerCrafting = default;
        private PlayerMissionTriggerDelivery _triggerDelivery = default;
        private PlayerMissionTriggerRepairing _triggerRepairing = default;

        #endregion

        #region Unity events

        private void Awake()
		{
            _triggerDestruction = new PlayerMissionTriggerDestruction();
            _triggerToolUpgrade = new PlayerMissionTriggerToolUpgrade();
            _triggerExploration = new PlayerMissionTriggerExploration();
            _triggerCrafting = new PlayerMissionTriggerCrafting();
            _triggerDelivery = new PlayerMissionTriggerDelivery();
            _triggerRepairing = new PlayerMissionTriggerRepairing();
		}

		private void OnDestroy()
		{
            _triggerDestruction.Teardown();
            _triggerToolUpgrade.Teardown();
            _triggerExploration.Teardown();
            _triggerCrafting.Teardown();
            _triggerDelivery.Teardown();
            _triggerRepairing.Teardown();
        }

		#endregion

		#region Public methods

        public void Init(PlayerEntryPoint playerEntryPoint, PlayerCraftingService playerCraftingService)
		{
            _triggerDestruction.Initialize(MissionCompleted);
            _triggerToolUpgrade.Initialize(MissionCompleted);
            _triggerExploration.Initialize(MissionCompleted);
            _triggerCrafting.Initialize(MissionCompleted, playerCraftingService, playerEntryPoint);
            _triggerDelivery.Initialize(MissionCompleted, playerEntryPoint);
            _triggerRepairing.Initialize(MissionCompleted);

            _missionId = 0;

            var mission = _missions[_missionId];

            StartTrigger(mission);

            PlayerMissionsEvents.OnInitialization?.Invoke(_missions[_missionId]);
        }

        public MissionConfigurationData GetCurrentMission()
		{
            var id = (_missionId >= _missions.Length) ? _missions.Length - 1 : _missionId;

            return _missions[id];
		}

		#endregion

		#region Private methods

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

        private void MissionCompleted()
        {
            _missionId++;

            NextMission();
        }

        private void StartTrigger(MissionConfigurationData data)
		{
            _triggerDestruction.ResetStatus(data.Type, data);
            _triggerToolUpgrade.ResetStatus(data.Type, data);
            _triggerExploration.ResetStatus(data.Type, data);
            _triggerCrafting.ResetStatus(data.Type, data);
            _triggerDelivery.ResetStatus(data.Type, data);
            _triggerRepairing.ResetStatus(data.Type, data);
		}

		#endregion
	}
}