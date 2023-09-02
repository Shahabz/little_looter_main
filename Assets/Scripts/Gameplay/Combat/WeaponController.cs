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

		public System.Action OnStartReloading;
		public System.Action OnStopReloading;
		public System.Action OnStartFiring;
		public System.Action OnStopFiring;
		public System.Action<int, int> OnRefreshAmmo;

		#endregion

		#region Inspector

		[SerializeField] private Weapon _weapon = default;

		#endregion

		#region Private properties

		private bool _isFiring = default;

		#endregion

		#region Public properties

		public bool IsReloading => _weapon.IsReloading;

		#endregion

		#region Public methods

		public void Init()
		{
			_weapon.OnStopReloading += ProcessStopReloading;
			_weapon.OnRefreshAmmo += RefreshAmmo;

			_weapon.Init();
		}

		public void Teardown()
		{
			_weapon.OnStopReloading -= ProcessStopReloading;
			_weapon.OnRefreshAmmo -= RefreshAmmo;
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
			_weapon.Fire();
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

			var nonFiring = _weapon.IsReloading || !input.attack || isClipEmpty;

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
				OnStartFiring?.Invoke();
			}

			_isFiring = true;

			_weapon.ConsumeAmmo();
		}

		private void ProcessStartReloading()
		{
			_weapon.StartReloading();

			OnStartReloading?.Invoke();
		}

		private void ProcessStopReloading()
		{
			OnStopReloading?.Invoke();
		}

		private void RefreshAmmo(int clipSize, int ammo)
		{
			OnRefreshAmmo?.Invoke(clipSize, ammo);
		}

		#endregion
	}
}