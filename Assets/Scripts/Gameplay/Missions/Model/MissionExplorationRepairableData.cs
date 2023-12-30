/*
 * Date: November 25th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionExplorationRepairableData", menuName = "ScriptableObjects/Gameplay/Missions/ExplorationRepairableData", order = 1)]
    public class MissionExplorationRepairableData : MissionExplorationData
    {
        public RepairObjectData Data;

        public override int Id => Data.Id;
	}
}