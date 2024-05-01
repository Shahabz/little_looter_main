/*
 * Date: September 25th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class WeaponEnemyController : MonoBehaviour
	{
		#region Private properties

		private EnemyBehaviorData _data = default;
		private EnemyLevelConfiguration _levelConfiguration = default;
		protected bool _initialized = false;
		private Action _meleeAttackCompleted = default;
		private Action _meleeAttackStarted = default;
		private Transform _target = default;
		private PlayerHealth _targetHealth = default;
		protected bool _enabled = false;

		#endregion

		#region Public methods

		public virtual void Init(EnemyBehaviorData data, EnemyLevelConfiguration levelConfig, Transform target, Action callbackCompleted, Action callbackStarted)
		{
			_data = data;

			_levelConfiguration = levelConfig;

			_target = target;
			_targetHealth = _target.GetComponent<PlayerHealth>();

			_meleeAttackStarted = callbackStarted;
			_meleeAttackCompleted = callbackCompleted;

			_initialized = true;

			_enabled = true;
		}

		public virtual void Process()
		{

		}

		public void Disable()
		{
			_enabled = false;
		}

		#endregion

		#region Animation events

		public void CheckMeleeDamageArea()
		{
			if (!_initialized) return;

			//Debug.LogError($"<color=orange>{name}</color> performed a melee attack! angle: <color=yellow>{_data.AttackAngle}</color>,  distance: <color=yellow>{_data.AttackDistance}</color>");

			var playerInsideAttackArea = CheckInsideAttackArea();

			if (!playerInsideAttackArea) return;

			var damage = UnityEngine.Random.Range(_levelConfiguration.minDamage, _levelConfiguration.maxDamage);

			_targetHealth.TakeDamage(damage);
		}

		public void MeleeAttackStarted()
		{
			_meleeAttackStarted?.Invoke();
		}

		public void MeleeAttackCompleted()
		{
			_meleeAttackCompleted?.Invoke();
		}

		#endregion

		#region Private methods

		private bool CheckInsideAttackArea()
		{
			Vector3 currentPosition = transform.position;

			Vector3 directionToTarget = _target.position - currentPosition;

			// Checks if the distance is inside the area radius detection
			var insideRange = CheckInsideRange(directionToTarget);

			if (!insideRange) return false;

			// Check dot product to detect if the target entity is inside of field of view
			var insideFOV = GetAngle(directionToTarget.normalized);

			if (!insideFOV) return false;

			return true;
		}

		private bool CheckInsideRange(Vector3 directionToTarget)
		{
			float dSqrToTarget = directionToTarget.magnitude;

			var insideRange = dSqrToTarget <= _data.AttackDistance;

			return insideRange;
		}

		private bool GetAngle(Vector3 direction)
		{
			var angleToTarget = Mathf.Abs(Vector3.Angle(direction, this.transform.forward));

			if (angleToTarget > _data.AttackAngle / 2) return false;

			return true;
		}

		#endregion
	}
}