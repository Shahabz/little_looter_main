/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "MissionData", menuName = "ScriptableObjects/Gameplay/MissionData", order = 1)]
    public class MissionConfigurationData : ScriptableObject
    {
        public int Id;
        public string Description;
        public MissionType Type;

        public virtual int GetProgressGoal() { return 1; }
    }
}