/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionToolUpgradeData", menuName = "ScriptableObjects/Gameplay/Missions/ToolUpgradeData", order = 1)]
    public class MissionToolUpgradeData : MissionConfigurationData
    {
        public int ToolLevel;

        public override int GetProgressGoal()
        {
            return 1;
        }
    }
}