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

		#endregion

		#region Inspector

		[Header("Muzzle")]
        [SerializeField] private Transform _muzzlePivot = default;
        [SerializeField] private GameObject _muzzlePrefab = default;

        [Header("Shells")]
        [SerializeField] private bool _hasShells = default;
        [SerializeField] private Transform _shellsPivot = default;
        [SerializeField] private GameObject _shellsPrefab = default;

        [Header("Projectile")]
        [SerializeField] private Transform _projectilePivot = default;
        [SerializeField] private GameObject _projectilePrefab = default;
        [SerializeField] private float _force = default;

        [Header("Configuration")]
        [SerializeField] private float _reloadingTime = 3;
		[Tooltip("Represents the total amount of ammo the weapon has")]
		[SerializeField] private int _ammo = 0;
		[Tooltip("Represents the weapon ammo capacity")]
		[SerializeField] private int _clipSize = 0;
        [SerializeField] private bool _isAutoFire = false;

        #endregion

        #region Private properties

        private bool _isReloading = false;
        private int _currentClipSize = 0;
        private int _currentAmmo = 0;

		#endregion

		#region Public properties

		public bool IsReloading => _isReloading;
        public bool IsAutoFire => _isAutoFire;
        public bool IsClipEmpty => _currentClipSize == 0;
        public bool HasAmmo => _currentAmmo > 0;

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
            Instantiate(_muzzlePrefab, _muzzlePivot.position, _muzzlePivot.rotation);

            // Shells
            if (_hasShells)
            {
                Instantiate(_shellsPrefab, _shellsPivot.position, _shellsPivot.rotation);
            }

            // Projectile
            var projectile = Instantiate(_projectilePrefab, _projectilePivot.position, _projectilePivot.rotation).GetComponent<ECExplodingProjectile>();

            var projectileRb = projectile.GetComponent<Rigidbody>();

            projectileRb.AddForce(_projectilePivot.forward * _force);

            projectile.DestroyInTime(5);
        }

        public void StartReloading()
		{
            _isReloading = true;

            Invoke(nameof(StopReloading), _reloadingTime);
		}

        public void ConsumeAmmo()
        {
            _currentClipSize -= 1;

            OnRefreshAmmo?.Invoke(_currentClipSize, _currentAmmo);
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
            var diff = _clipSize - _currentClipSize;

            var clip = (_currentAmmo > _clipSize) ? _clipSize : _currentAmmo;

            _currentClipSize = clip;

            _currentAmmo = Mathf.Clamp(_currentAmmo - diff, 0, _ammo);

            OnRefreshAmmo?.Invoke(_currentClipSize, _currentAmmo);
		}

        #endregion
    }
}