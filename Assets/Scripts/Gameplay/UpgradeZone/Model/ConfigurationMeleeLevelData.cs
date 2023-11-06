/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [System.Serializable]
    public struct MeleeUpgradeRequirementData
	{
        public ResourceData resource;
        public int amount;
    }

    [CreateAssetMenu(fileName = "ConfigurationMeleeLevelData", menuName = "ScriptableObjects/Gameplay/ConfigurationMeleeLevelData", order = 1)]
    public class ConfigurationMeleeLevelData : ScriptableObject
    {
        public int level;
        public float upgradeTime;
        public int damage;
        public MeleeUpgradeRequirementData[] requirements;
    }
}