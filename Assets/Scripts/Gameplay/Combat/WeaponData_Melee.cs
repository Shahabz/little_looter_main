/*
 * Date: May 4th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
    [CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ScriptableObjects/Gameplay/Weapons/Melee/data", order = 1)]
    public class WeaponData_Melee : WeaponData
    {
        public bool Pushback;
        public float PushbackForce;
    }
}