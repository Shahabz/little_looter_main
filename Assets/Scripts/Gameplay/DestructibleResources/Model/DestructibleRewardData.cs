/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using LittleLooters.General;

namespace LittleLooters.Gameplay
{
    [System.Serializable]
    public struct DestructibleRewardData
    {
        public float percentage;
        public ResourceData resource;
        public int amount;
    }
}