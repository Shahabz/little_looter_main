/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "AllMissionsData", menuName = "ScriptableObjects/Gameplay/Missions/AllMissions", order = 1)]
    public class AllMissionsConfiguration : ScriptableObject
    {
        public MissionConfigurationData[] Missions;
    }
}