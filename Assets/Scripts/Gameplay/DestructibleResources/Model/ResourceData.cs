/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.General
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "ScriptableObjects/General/ResourceData", order = 1)]
    public class ResourceData : ScriptableObject
    {
        public int Id;
        public string DisplayName;
        public Sprite Icon;
    }
}