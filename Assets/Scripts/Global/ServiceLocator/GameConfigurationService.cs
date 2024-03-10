/*
 * Date: January 20th, 2024
 * Author: Peche
 */

using LittleLooters.Gameplay;
using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Global.ServiceLocator
{
    public class GameConfigurationService : MonoBehaviour, IGameService
    {
        [Header("Resources configuration")]
        [SerializeField] private ResourceData[] _resources = default;

        [Header("Tool configuration")]
        [SerializeField] private int _toolExtraDamageDuration = 10;
        [SerializeField] private ConfigurationMeleeLevelData[] _toolLevels = default;

        [Header("Missions configuration")]
        [SerializeField] private float _timeToCheckCompletedMission = default;
        [SerializeField] private MissionConfigurationData[] _missions = default;

        public ResourceData[] Resources => _resources;
        public MissionConfigurationData[] Missions => _missions;
        public float TimeToCheckCompletedMission => _timeToCheckCompletedMission;
        public ConfigurationMeleeLevelData[] ToolLevels => _toolLevels;
        public int ToolExtraDamageDuration => _toolExtraDamageDuration;

        public (bool found, MissionConfigurationData mission) TryGetMission(int id)
		{
			for (int i = 0; i < _missions.Length; i++)
			{
                var mission = _missions[i];

                if (mission.Id == id) return (true, mission);
			}

            return (false, default);
		}

        public (bool found, ResourceData resource) TryGetResource(int id)
		{
			for (int i = 0; i < _resources.Length; i++)
			{
                var resource = _resources[i];

                if (resource.Id == id) return (true, resource);
			}

            return (false, default);
		}
    }
}