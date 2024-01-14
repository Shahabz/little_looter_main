/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
	[CreateAssetMenu(fileName = "AsssaultWeaponData", menuName = "ScriptableObjects/Gameplay/Weapons/Assault/data", order = 1)]
	public class AssaultWeaponData : ScriptableObject
	{
		#region Inspector

		public int Id;
		public string DisplayName;
		public GameObject Art;
		public float FireRate;
		public int MinDamage;
		public int MaxDamage;
		public bool MultipleShots;
		public int ShotsAmount;
		public bool IsAutoFire;
		public bool HasShell;
		public int ClipSize;
		public float ReloadingTime;
		public GameObject Muzzle;
		public GameObject Shells = default;
		public GameObject Projectile = default;
		public float LifeTime;
		public float Force;
		public float RadiusDetection;

		#endregion
	}
}