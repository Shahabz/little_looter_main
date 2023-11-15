/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionResourceDestructionData", menuName = "ScriptableObjects/Gameplay/Missions/ResourceDestructionData", order = 1)]
    public class MissionResourceDestructionData : MissionConfigurationData
    {
        public DestructibleObjectData Destructible;
        public int Amount;

		public override int GetProgressGoal()
		{
			return Amount;
		}
	}
}