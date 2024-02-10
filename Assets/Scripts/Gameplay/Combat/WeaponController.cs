/*
 * Date: August 26th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using StarterAssets;
using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
	public class WeaponController : MonoBehaviour
	{
		#region Events

		public System.Action<float> OnStartReloading;
		public System.Action OnStopReloading;
		public System.Action<float> OnStartFiring;
		public System.Action OnStopFiring;
		public System.Action OnCompleteFiring;
		public System.Action<int, int> OnRefreshAmmo;

		#endregion

		#region Inspector

		[SerializeField] private PlayerWeaponInfo[] _weapons = default;

		[SerializeField] private Weapon _weapon = default;

		[Header("Inventory weapons")]
		[SerializeField] private GameObject _weapon0 = default;
		[SerializeField] private AssaultWeaponData _weaponData0 = default;

		[SerializeField] private GameObject _weapon1 = default;
		[SerializeField] private AssaultWeaponData _weaponData1 = default;

		[Header("Configuration")]
		[SerializeField] AssaultWeaponData[] _allAssaultWeapons = default;

		#endregion

		#region Private properties

		private PlayerWeaponInfo _currentWeapon = default;
		private bool _isFiring = default;
		private bool _gameHasStarted = false;
		private PlayerAimingAssistance _aimingAssistance = default;

		#endregion

		#region Public properties

		public bool IsReloading => _currentWeapon.isReloading;	//_weapon.IsReloading;
		public int ClipSize => _weapon.ClipSize;
		public int Ammo => _currentWeapon.ammo;	//_weapon.Ammo;
		public float WeaponRadiusDetection => _weapon.GetRadiusDetection();
		public string CurrentWeaponId => _currentWeapon.id;

		#endregion

		#region Unity events

		private void Update()
		{
			if (!_gameHasStarted) return;

			ProcessWeaponsReloading(Time.time);
		}

		#endregion

		#region Public methods

		public void Init(PlayerAimingAssistance aimingAssistance)
		{
			_aimingAssistance = aimingAssistance;

			_weapon.OnStopReloading += ProcessStopReloading;
			_weapon.OnRefreshAmmo += RefreshAmmo;
			_weapon.OnCompleteFiring += CompleteFiring;

			_weapon.Init();

			InitWeapons();

			UI_GameplayEvents.OnWeaponSelection += SwapWeapon;
			UI_GameplayEvents.OnWeaponStartReloading += HandleWeaponStartReloadingFromSlot;
			UI_GameplayEvents.OnStartGame += HandleStartGame;
		}

		public void Teardown()
		{
			UI_GameplayEvents.OnWeaponSelection -= SwapWeapon;
			UI_GameplayEvents.OnWeaponStartReloading -= HandleWeaponStartReloadingFromSlot;
			UI_GameplayEvents.OnStartGame -= HandleStartGame;

			_weapon.OnStopReloading -= ProcessStopReloading;
			_weapon.OnRefreshAmmo -= RefreshAmmo;
			_weapon.OnCompleteFiring -= CompleteFiring;
		}

		public void CheckInput(StarterAssetsInputs input)
		{
			if (!_gameHasStarted) return;

			var reloading = CheckReloading(input);

			if (reloading) return;

			CheckAttack(input);
		}

		/// <summary>
		/// Invoked from fire animation event at keyframe
		/// </summary>
		public void FireWeapon()
		{
			_aimingAssistance.Process(transform.forward);
			_aimingAssistance.RotateToTarget(true);

			_weapon.Fire();
		}

		public void PickupAmmo(int ammo)
		{
			_weapon.AddAmmo(ammo);
		}

		public PlayerWeaponInfo GetWeaponInfo(int index)
		{
			index = Mathf.Clamp(index, 0, _weapons.Length - 1);

			return _weapons[index];
		}

		#endregion

		#region Private methods

		private void HandleStartGame()
		{
			_gameHasStarted = true;
		}

		private bool CheckReloading(StarterAssetsInputs input)
		{
			if (_currentWeapon.ammo == 0) return false;

			// Check if input start reloading action
			if (!input.reload) return false;

			// Check if it is already reloading
			if (_currentWeapon.isReloading) return false;

			input.reload = false;

			ProcessStartReloading();

			return true;
		}

		private void CheckAttack(StarterAssetsInputs input)
		{
			var isClipEmpty = _currentWeapon.ammo == 0;

			var nonFiring = _currentWeapon.isReloading || !input.attack || isClipEmpty;

			if (nonFiring)
			{
				// Check if it was previously firing
				if (_isFiring)
				{
					_isFiring = false;

					OnStopFiring?.Invoke();
				}

				return;
			}

			// Check waiting fire rate
			if (_weapon.WaitingFireRate) return;

			// Check if it is trying to fire more than once with pressing button when weapon is not autofire
			if (_isFiring && !_weapon.IsAutoFire) return;

			ProcessFiring(input.attack);
		}

		private void ProcessFiring(bool inputAttackActive)
		{
			// Check if previously was firing
			if (!_isFiring)
			{
				OnStartFiring?.Invoke(_weapon.FireRate);
			}

			_isFiring = true;

			// NOTE: this method should be invoked from firing animation
			FireWeapon();

			ConsumeAmmo();
		}

		private void ProcessStartReloading()
		{
			_weapon.StartReloading();

			OnStartReloading?.Invoke(_weapon.ReloadingTime);

			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				if (weapon.isSelected)
				{
					var expiration = Time.time + _weapon.ReloadingTime;
					weapon.StartReloading(expiration, _weapon.ReloadingTime);
					break;
				}
			}
		}

		private void ProcessStopReloading()
		{
			OnStopReloading?.Invoke();
		}

		private void RefreshAmmo(int clipSize, int ammo)
		{
			OnRefreshAmmo?.Invoke(clipSize, ammo);
		}

		private void SwapWeapon(string id)
		{
			RefreshSelectedWeapon(id);

			var weaponIndex = GetWeaponIndex(id);

			var weaponData = (weaponIndex == 0) ? _weaponData0 : _weaponData1;

			_weapon0.SetActive(weaponIndex == 0);
			_weapon1.SetActive(weaponIndex == 1);

			_weapon.RefreshData(weaponData);

			PlayerProgressEvents.OnWeaponChanged?.Invoke(weaponData);
		}

		private void CompleteFiring()
		{
			OnCompleteFiring?.Invoke();
		}

		private void ConsumeAmmo()
		{
			_weapon.ConsumeAmmo();

			var hasAmmo = true;

			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				if (!weapon.id.Equals(_weapon.Id)) continue;

				weapon.ConsumeAmmo();

				hasAmmo = weapon.ammo > 0;

				break;
			}

			if (hasAmmo) return;

			ProcessStartReloading();
		}

		private void RefreshSelectedWeapon(string id)
		{
			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				weapon.RefreshSelection(weapon.id.Equals(id));

				if (!weapon.id.Equals(id)) continue;

				_currentWeapon = weapon;
			}
		}

		private void InitWeapons()
		{
			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				if (weapon.isLocked)
				{
					weapon.Init(0, false);
					continue;
				}

				var initialAmmo = GetInitialAmmo(weapon.id);

				weapon.Init(initialAmmo, i == 0);
			}

			_currentWeapon = _weapons[0];
		}

		private int GetInitialAmmo(string id)
		{
			var ammo = 0;

			for (int i = 0; i < _allAssaultWeapons.Length; i++)
			{
				var weaponData = _allAssaultWeapons[i];

				if (weaponData.Id.Equals(id))
				{
					ammo = weaponData.ClipSize;

					break;
				}
			}

			return ammo;
		}

		private int GetWeaponIndex(string id)
		{
			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				if (weapon.id.Equals(id)) return i;
			}

			return 0;
		}

		private void ProcessWeaponsReloading(float time)
		{
			var invokeEvent = false;

			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				if (!weapon.isReloading) continue;

				if (weapon.reloadingExpiration > time) continue;

				var index = GetWeaponIndex(weapon.id);

				var ammo = _allAssaultWeapons[index].ClipSize;

				weapon.StopReloading(ammo);

				invokeEvent = true;
			}

			if (!invokeEvent) return;

			OnStopReloading?.Invoke();
		}

		private void HandleWeaponStartReloadingFromSlot(string weaponId)
		{
			if (weaponId.Equals(_currentWeapon.id))
			{
				ProcessStartReloading();
				return;
			}

			for (int i = 0; i < _weapons.Length; i++)
			{
				var weapon = _weapons[i];

				if (!weapon.id.Equals(weaponId)) continue;

				var index = GetWeaponIndex(weaponId);
				var weaponData = _allAssaultWeapons[index];
				var duration = weaponData.ReloadingTime;
				var expiration = Time.time + duration;

				weapon.StartReloading(expiration, duration);

				break;
			}
		}

		#endregion
	}
}