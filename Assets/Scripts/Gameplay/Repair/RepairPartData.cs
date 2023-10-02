/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [CreateAssetMenu(fileName = "RepairPartData", menuName = "ScriptableObjects/Gameplay/Repair/RepairData", order = 1)]

    public class RepairPartData : ScriptableObject
    {
        public int Id;
        public Sprite Icon;
    }
}