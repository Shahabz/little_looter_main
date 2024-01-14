/*
 * Date: August 26th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
    /// <summary>
    /// In charge of doing all actions and data information related with a weapon:
    ///  - fire
    ///  - reloading
    ///  - ammo
    /// </summary>
    public class Weapon : MonoBehaviour
    {
		#region Events

		public System.Action OnStopReloading;
        public System.Action<int, int> OnRefreshAmmo;
        public System.Action OnCompleteFiring;

        #endregion

        #region Inspector

        [SerializeField] private AssaultWeaponData _data = default;

		[Header("Muzzle")]
        [SerializeField] private Transform _muzzlePivot = default;
        [SerializeField] private GameObject _muzzlePrefab = default;
        [SerializeField] private ParticleSystem _muzzle = default;

        [Header("Shells")]
        [SerializeField] private bool _hasShells = default;
        [SerializeField] private Transform _shellsPivot = default;
        [SerializeField] private GameObject _shellsPrefab = default;

        [Header("Projectile")]
        [SerializeField] private Transform[] _projectilePivots = default;
        [SerializeField] private GameObject _projectilePrefab = default;
        [SerializeField] private float _force = default;

        [Header("Configuration")]

		[Tooltip("Represents the total amount of ammo the weapon has")]
		[SerializeField] private int _ammo = 0;

        #endregion

        #region Private properties

        private bool _isReloading = false;
        private int _currentClipSize = 0;
        private int _currentAmmo = 0;
        private float _waitingFireRate = 0;

        #endregion

        #region Public properties

        public bool IsReloading => _isReloading;
        public bool IsAutoFire => _data.IsAutoFire;
        public bool IsClipEmpty => _currentClipSize == 0;
        public bool HasAmmo => _currentAmmo > 0;
        public float ReloadingTime => _data.ReloadingTime;
        public float FireRate => _data.FireRate;
        public bool WaitingFireRate => _waitingFireRate > 0;
        public int ClipSize => _data.ClipSize;
        public int Ammo => _currentAmmo;

		#endregion

		#region Unity events

		private void Update()
		{
            CheckFireRate(Time.deltaTime);
		}

		#endregion

		#region Public methods

		public void Init()
		{
            _currentAmmo = _ammo;

            FillClip();
		}

		public void Fire()
        {
            // Muzzle
            _muzzle.Play();

            // Shells
            if (_hasShells)
            {
                Instantiate(_data.Shells, _shellsPivot.position, _shellsPivot.rotation);
            }

			// Projectile/s
            if (_data.MultipleShots)
            {
                MultipleShots();
                return;
            }

            SimpleShot();
        }

        public void StartReloading()
		{
            _isReloading = true;

            Invoke(nameof(StopReloading), _data.ReloadingTime);
		}

        public void ConsumeAmmo()
        {
            _waitingFireRate = _data.FireRate;

            _currentClipSize -= 1;

            OnRefreshAmmo?.Invoke(_currentClipSize, _currentAmmo);
        }

        public void AddAmmo(int ammo)
		{
            _currentAmmo += ammo;

            OnRefreshAmmo?.Invoke(_currentClipSize, _currentAmmo);
        }

        public void RefreshData(AssaultWeaponData data)
		{
            _data = data;

            _muzzle = Instantiate(_data.Muzzle).GetComponent<ParticleSystem>();
            _muzzle.transform.parent = _muzzlePivot;
            _muzzle.transform.position = _muzzlePivot.position;
            _muzzle.transform.rotation = _muzzlePivot.rotation;

            if (_muzzle.TryGetComponent<ECdestroyMe>(out var destroyMe))
			{
                Destroy(destroyMe);
			}
        }

        public float GetRadiusDetection()
		{
            return _data.RadiusDetection;
		}

        #endregion

        #region Private methods

        private void StopReloading()
        {
            _isReloading = false;

            FillClip();

            OnStopReloading?.Invoke();
        }

        private void FillClip()
		{
            var diff = _data.ClipSize - _currentClipSize;

            var clip = (_currentAmmo > _data.ClipSize) ? _data.ClipSize : _currentAmmo;

            _currentClipSize = clip;

            _currentAmmo = Mathf.Clamp(_currentAmmo - diff, 0, _ammo);

            OnRefreshAmmo?.Invoke(_currentClipSize, _currentAmmo);
		}

        private void SimpleShot()
        {
            var projectilePivot = _projectilePivots[0];

            ProcessShot(projectilePivot);
        }

        private void MultipleShots()
		{
            for (int i = 0; i < _data.ShotsAmount; i++)
            {
                if (i >= _projectilePivots.Length) return;

                var projectilePivot = _projectilePivots[i];

                ProcessShot(projectilePivot);
            }
        }

        private void ProcessShot(Transform projectilePivot)
		{
            var projectile = Instantiate(_data.Projectile, projectilePivot.position, projectilePivot.rotation).GetComponent<ECExplodingProjectile>();

            projectile.SetDamageLimits(_data.MinDamage, _data.MaxDamage);

            var projectileRb = projectile.GetComponent<Rigidbody>();

            projectileRb.AddForce(projectilePivot.forward * _data.Force);

            projectile.DestroyInTime(_data.LifeTime);
        }

        private void CheckFireRate(float deltaTime)
		{
            if (_waitingFireRate <= 0) return;

            _waitingFireRate -= deltaTime;

            if (_waitingFireRate > 0) return;

            _waitingFireRate = 0;

            OnCompleteFiring?.Invoke();
        }

        #endregion
    }
}