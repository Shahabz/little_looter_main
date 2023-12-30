/*
 * Date: December 29th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionRepairingData", menuName = "ScriptableObjects/Gameplay/Missions/RepairingData", order = 1)]
    public class MissionRepairingData : MissionConfigurationData
    {
        public RepairObjectData RepairObjectData;

        public override int GetProgressGoal()
        {
            return 1;
        }
    }
}