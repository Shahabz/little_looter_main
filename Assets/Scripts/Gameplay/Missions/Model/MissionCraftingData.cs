/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionCraftingData", menuName = "ScriptableObjects/Gameplay/Missions/CraftingData", order = 1)]
    public class MissionCraftingData : MissionConfigurationData
    {
        public CraftingConfigurationData AreaData;
        public int Amount;

        public override int GetProgressGoal()
        {
            return Amount;
        }

        public ResourceData ResourceData => AreaData.ResourceGenerated;
    }
}