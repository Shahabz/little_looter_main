/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionsService : MonoBehaviour
    {
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

        public void Init(PlayerCraftingService playerCraftingService)
		{
            _triggerDestruction.Initialize(MissionCompleted);
            _triggerToolUpgrade.Initialize(MissionCompleted);
            _triggerExploration.Initialize(MissionCompleted);
            _triggerCrafting.Initialize(MissionCompleted, playerCraftingService);
            _triggerDelivery.Initialize(MissionCompleted);
            _triggerRepairing.Initialize(MissionCompleted);

            _missionId = 0;

            var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

            var mission = gameConfiguration.Missions[_missionId];

            StartTrigger(mission);

            PlayerMissionsEvents.OnInitialization?.Invoke(gameConfiguration.Missions[_missionId]);
        }

        public MissionConfigurationData GetCurrentMission()
		{
            var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

            var id = (_missionId >= gameConfiguration.Missions.Length) ? gameConfiguration.Missions.Length - 1 : _missionId;

            return gameConfiguration.Missions[id];
		}

        public MissionConfigurationData GetFirstMissionData()
		{
            var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

            return gameConfiguration.Missions[0];
        }

        #endregion

        #region Private methods

        private void NextMission()
		{
            var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

            if (_missionId >= gameConfiguration.Missions.Length)
			{
                // TODO: reached last mission
                Debug.LogError("Last mission was completed!");

                return;
			}

            var mission = gameConfiguration.Missions[_missionId];

            // Check triggers based on mission
            StartTrigger(mission);

            PlayerMissionsEvents.OnMoveToMission?.Invoke(gameConfiguration.Missions[_missionId]);
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