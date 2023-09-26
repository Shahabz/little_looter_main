/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	[CreateAssetMenu(fileName = "EnemyBehaviorData", menuName = "ScriptableObjects/Gameplay/Enemies/BehaviorData", order = 1)]
	public class EnemyBehaviorData : ScriptableObject
	{
		#region Inspector

		public float WalkingSpeed;
		public float RunningSpeed;
		public float RadiusDetection;
		public float RadiusAttack;
		public float AttackRate;
		public float AttackAngle;
		public float AttackDistance;
		public float AngleFieldOfView;
		public Weapon Weapon;
		public int MinDamage;
		public int MaxDamage;

		#endregion
	}
}