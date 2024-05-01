/*
 * Date: May 1st, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class EnemyController_Assault : EnemyController
    {
		private bool _targetDetected = false;

		public override void Initialization(int id)
		{
			base.Initialization(id);

			var weaponController = _weaponController as WeaponEnemyController_Assault;
			weaponController.OnStartReloading += HandleStartReloading;
			weaponController.OnStopReloading += HandleStopReloading;
		}

		public override void Teardown()
		{
			base.Teardown();

			var weaponController = _weaponController as WeaponEnemyController_Assault;
			weaponController.OnStartReloading -= HandleStartReloading;
			weaponController.OnStopReloading -= HandleStopReloading;
		}

		protected override void OnStartDetection()
		{
			if (!_enabled) return;

			if (_state == EnemyState.RELOAD) return;

			if (_targetDetected) return;

			_targetDetected = true;

			StopMovement();

			_state = EnemyState.AIM;
		}

		protected override void OnStopDetection()
		{
			if (!_enabled) return;

			if (!_targetDetected) return;

			_targetDetected = false;

			StopMovement();

			if (_state == EnemyState.RELOAD) return;

			_state = EnemyState.IDLE;
		}

		protected override bool CheckInAttackRange()
		{
			if (!_targetDetected) return false;

			var inRange = base.CheckInAttackRange();

			//var message = (inRange) ? "<color=green>" : "<color=red>";
			//Debug.LogError($"Player is in range: {message}{inRange}</color>");

			return inRange;
		}

		protected override void Attack()
		{
			if (Time.time < _nextAttackTime) return;

			_nextAttackTime = Time.time + _data.AttackRate;

			_visualController.Attack();
		}

		private void HandleStartReloading()
		{
			_state = EnemyState.RELOAD;

			_visualController.Refresh(_state, false);
		}

		private void HandleStopReloading()
		{
			_state = EnemyState.IDLE;
		}
	}
}