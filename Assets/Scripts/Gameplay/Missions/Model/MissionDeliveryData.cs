/*
 * Date: December 28th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionDeliveryData", menuName = "ScriptableObjects/Gameplay/Missions/DeliveryData", order = 1)]
    public class MissionDeliveryData : MissionConfigurationData
    {
        public RepairObjectData RepairObjectData;
        public ResourceData ResourceData;
        public int Amount;

        public override int GetProgressGoal()
        {
            return Amount;
        }
    }
}