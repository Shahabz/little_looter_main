/*
 * Date: May 1st, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class WeaponEnemyController_Assault : WeaponEnemyController
    {
		public System.Action OnStartReloading;
		public System.Action OnStopReloading;

		[SerializeField] private ParticleSystem _muzzle = default;

		[Header("Projectile")]
		[SerializeField] private Transform _projectilePivot = default;
		[SerializeField] private GameObject _projectile = default;
		[SerializeField] private float _force = default;
		[SerializeField] private int _minDamage = default;
		[SerializeField] private int _maxDamage = default;
		[SerializeField] private float _lifeTime = default;

		private int _ammo = 0;
		private int _magazine = 0;
		private bool _isReloading = false;
		private bool _isFiring = false;
		private float _reloadingTime = 0;

		public override void Init(EnemyBehaviorData data, EnemyLevelConfiguration levelConfig, Transform target, System.Action callbackCompleted, System.Action callbackStarted)
		{
			_magazine = 4;	// data.Magazine;
			_ammo = _magazine;
			_reloadingTime = 3; //data.ReloadingTime;

			var destroyMe = _muzzle.GetComponent<ECdestroyMe>();
			destroyMe.enabled = false;

			_initialized = true;

			_enabled = true;
		}

		/// <summary>
		/// Invoked from the attack animation event
		/// </summary>
		public override void Process()
		{
			if (!_enabled) return;

			_ammo--;

			ProcessShot();
		}

		private void ProcessShot()
		{
			ShowMuzzleVfx();

			ShowProjectile();

			if (_ammo > 0) return;

			Invoke(nameof(StartReloading), 1f);
		}

		private void ShowMuzzleVfx()
		{
			_muzzle?.Play();
		}

		private void ShowProjectile()
		{
			var projectile = Instantiate(_projectile, _projectilePivot.position, _projectilePivot.rotation).GetComponent<ECExplodingProjectile>();

			projectile.SetDamageLimits(_minDamage, _maxDamage);

			var projectileRb = projectile.GetComponent<Rigidbody>();

			projectileRb.AddForce(_projectilePivot.forward * _force);

			projectile.DestroyInTime(_lifeTime);
		}

		private void StartReloading()
		{
			_isReloading = true;

			OnStartReloading?.Invoke();

			Invoke(nameof(StopReloading), _reloadingTime);
		}

		private void StopReloading()
		{
			_ammo = _magazine;

			_isReloading = false;

			OnStopReloading?.Invoke();
		}
	}
}