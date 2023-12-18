/*
 * Date: December 14th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.General
{
    [CreateAssetMenu(fileName = "CraftingData", menuName = "ScriptableObjects/Gameplay/CraftingData", order = 1)]
    public class CraftingConfigurationData : ScriptableObject
    {
        public int Id;
        public ResourceData ResourceRequired;
        public int AmountRequired;
        public ResourceData ResourceGenerated;
    }
}