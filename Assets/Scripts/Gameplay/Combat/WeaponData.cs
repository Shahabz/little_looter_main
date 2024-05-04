/*
 * Date: May 4th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
    public class WeaponData : ScriptableObject
    {
		public string Id;
		public WeaponType Type;
		public string DisplayName;
		public GameObject Art;
		public float FireRate;
		public int MinDamage;
		public int MaxDamage;
		public float ReloadingTime;
		public float RadiusDetection;
	}
}