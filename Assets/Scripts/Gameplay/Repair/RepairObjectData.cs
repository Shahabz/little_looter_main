/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [System.Serializable]
    public struct PartsData
	{
        public ResourceData resourceData;
        public int amount;
    }

    [CreateAssetMenu(fileName = "RepairObjectData", menuName = "ScriptableObjects/Gameplay/Repair/RepairObjectData", order = 1)]
    public class RepairObjectData : ScriptableObject
    {
        public int Id;
        public string DisplayName;
        [Tooltip("Parts needed to start repairing")]
        public PartsData[] Parts;
        [Tooltip("Time (in secs) to complete repairing")]
        public int Duration;
    }
}