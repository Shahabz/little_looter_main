/*
 * Date: August 26th, 2023
 * Author: Peche
 */

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

		[SerializeField] private Weapon _weapon = default;

		[Header("Inventory weapons")]
		[SerializeField] private GameObject _weapon0 = default;
		[SerializeField] private AssaultWeaponData _weaponData0 = default;

		[SerializeField] private GameObject _weapon1 = default;
		[SerializeField] private AssaultWeaponData _weaponData1 = default;

		#endregion

		#region Private properties

		private bool _isFiring = default;
		private PlayerAimingAssistance _aimingAssistance = default;

		#endregion

		#region Public properties

		public bool IsReloading => _weapon.IsReloading;
		public int ClipSize => _weapon.ClipSize;
		public int Ammo => _weapon.Ammo;

		#endregion

		#region Public methods

		public void Init(PlayerAimingAssistance aimingAssistance)
		{
			_aimingAssistance = aimingAssistance;

			_weapon.OnStopReloading += ProcessStopReloading;
			_weapon.OnRefreshAmmo += RefreshAmmo;
			_weapon.OnCompleteFiring += CompleteFiring;

			_weapon.Init();

			UI_GameplayEvents.OnWeaponSelection += SwapWeapon;
		}

		public void Teardown()
		{
			UI_GameplayEvents.OnWeaponSelection -= SwapWeapon;

			_weapon.OnStopReloading -= ProcessStopReloading;
			_weapon.OnRefreshAmmo -= RefreshAmmo;
			_weapon.OnCompleteFiring -= CompleteFiring;
		}

		public void CheckInput(StarterAssetsInputs input)
		{
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

		#endregion

		#region Private methods

		private bool CheckReloading(StarterAssetsInputs input)
		{
			if (!_weapon.HasAmmo) return false;

			// Check if input start reloading action
			if (!input.reload) return false;

			// Check if it is already reloading
			if (_weapon.IsReloading) return false;

			input.reload = false;

			ProcessStartReloading();

			return true;
		}

		private void CheckAttack(StarterAssetsInputs input)
		{
			var isClipEmpty = _weapon.IsClipEmpty;

			var nonFiring = _weapon.IsReloading || _weapon.WaitingFireRate || !input.attack || isClipEmpty;

			if (!_weapon.IsAutoFire)
			{
				input.attack = false;
			}

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

			ProcessFiring();
		}

		private void ProcessFiring()
		{
			// Check if previously was firing
			if (!_isFiring)
			{
				OnStartFiring?.Invoke(_weapon.FireRate);
			}

			_isFiring = true;

			// NOTE: this method should be invoked from firing animation
			FireWeapon();

			_weapon.ConsumeAmmo();
		}

		private void ProcessStartReloading()
		{
			_weapon.StartReloading();

			OnStartReloading?.Invoke(_weapon.ReloadingTime);
		}

		private void ProcessStopReloading()
		{
			OnStopReloading?.Invoke();
		}

		private void RefreshAmmo(int clipSize, int ammo)
		{
			OnRefreshAmmo?.Invoke(clipSize, ammo);
		}

		private void SwapWeapon(int id)
		{
			var weaponData = (id == 0) ? _weaponData0 : _weaponData1;

			_weapon0.SetActive(id == 0);
			_weapon1.SetActive(id == 1);

			_weapon.RefreshData(weaponData);
		}

		private void CompleteFiring()
		{
			OnCompleteFiring?.Invoke();
		}

		#endregion
	}
}