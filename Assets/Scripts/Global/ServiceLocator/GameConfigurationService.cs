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
        [SerializeField] private MissionConfigurationData[] _missions = default;

        public ResourceData[] Resources => _resources;
        public MissionConfigurationData[] Missions => _missions;
        public ConfigurationMeleeLevelData[] ToolLevels => _toolLevels;
        public int ToolExtraDamageDuration => _toolExtraDamageDuration;
    }
}