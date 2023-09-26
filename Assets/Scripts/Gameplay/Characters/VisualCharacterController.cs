/*
 * Date: September 2nd, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LittleLooters.Gameplay
{
    /// <summary>
    /// In charge of updating the Animator component state based on the character's state
    /// </summary>
    public class VisualCharacterController : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Animator _animator = default;
		[SerializeField] private RigBuilder _rigBuilder = default;
		[SerializeField] private ParticleSystem _damageVfx = default;

		#endregion

		#region Private properties

		private const string IS_AIMING = "isAiming";
		private const string RELOAD = "reload";
		private const string IS_RELOADING = "isReloading";
		private const string FIRE = "fire";
		private const string IS_FIRING = "isFiring";
		private const string DEAD = "dead";

		#endregion

		#region Public methods

		public void Init(WeaponController weaponController)
		{
			// Reloading events
			weaponController.OnStartReloading += StartReloading;
			weaponController.OnStopReloading += StopReloading;

			// Firing events
			weaponController.OnStartFiring += StartFiring;
			weaponController.OnStopFiring += StopFiring;

			var damageable = GetComponent<ITakeDamage>();
			damageable.OnTakeDamage += TakeDamage;
		}

		public void Teardown(WeaponController weaponController)
		{
			// Reloading events
			weaponController.OnStartReloading -= StartReloading;
			weaponController.OnStopReloading -= StopReloading;

			// Firing events
			weaponController.OnStartFiring += StartFiring;
			weaponController.OnStopFiring += StopFiring;

			var damageable = GetComponent<ITakeDamage>();
			damageable.OnTakeDamage -= TakeDamage;
		}

		public void RefreshStateByInput(StarterAssetsInputs input)
		{
			CheckAimingStatus(input);
		}

		public void Dead()
		{
			_animator.SetTrigger(DEAD);

			_rigBuilder.enabled = false;
		}

		#endregion

		#region Private methods

		private void CheckAimingStatus(StarterAssetsInputs input)
		{
			if (input.StartAiming)
			{
				_animator.SetBool(IS_AIMING, true);
				return;
			}

			if (input.StopAiming)
			{
				_animator.SetBool(IS_AIMING, false);
				return;
			}
		}

		private void StartReloading()
		{
			_animator.SetBool(IS_RELOADING, true);
			_animator.SetTrigger(RELOAD);
		}

		private void StopReloading()
		{
			_animator.SetBool(IS_RELOADING, false);
		}

		private void StartFiring()
		{
			_animator.SetBool(IS_FIRING, true);
			_animator.SetTrigger(FIRE);
		}

		private void StopFiring()
		{
			_animator.SetBool(IS_FIRING, false);
		}

		private void TakeDamage(float damage)
		{
			_damageVfx.Play();
		}

		#endregion
	}
}