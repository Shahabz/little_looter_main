/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "DestructibleObjectData", menuName = "ScriptableObjects/Gameplay/DestructibleObjectData", order = 1)]
    public class DestructibleObjectData : ScriptableObject
    {
        public int Id;
        public DestructibleResourceType Type;
        public int Hp;
        public int LevelRequired;
        public DestructibleRewardData[] Rewards;
    }
}