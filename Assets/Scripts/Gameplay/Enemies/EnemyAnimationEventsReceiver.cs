/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    /// <summary>
    /// Class in charge to receive the enemy's animation events triggered from animation frames
    /// </summary>
    public class EnemyAnimationEventsReceiver : MonoBehaviour
    {
		[SerializeField] private WeaponEnemyController _weaponController = default;

		public void CheckMeleeDamageArea()
		{
			_weaponController.CheckMeleeDamageArea();
		}

		public void MeleeAttackStarted()
		{
			_weaponController.MeleeAttackStarted();
		}

		public void MeleeAttackCompleted()
		{
			_weaponController.MeleeAttackCompleted();
		}
	}
}