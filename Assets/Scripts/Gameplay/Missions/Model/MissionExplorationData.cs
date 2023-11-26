/*
 * Date: November 25th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionExplorationData", menuName = "ScriptableObjects/Gameplay/Missions/ExplorationData", order = 1)]
    public class MissionExplorationData : MissionConfigurationData
    {
        public ExplorableObjectType explorableType;
    }
}