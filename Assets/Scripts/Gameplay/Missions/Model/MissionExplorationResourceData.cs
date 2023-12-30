/*
 * Date: November 25th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionExplorationResourceData", menuName = "ScriptableObjects/Gameplay/Missions/ExplorationResourceData", order = 1)]
    public class MissionExplorationResourceData : MissionExplorationData
    {
        public ResourceData Data;

		public override int Id => Data.Id;
	}
}