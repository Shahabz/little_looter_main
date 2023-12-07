/*
 * Date: September 2nd, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using StarterAssets;
using System.Collections;
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

		[Header("Weapons")]
		[SerializeField] private GameObject _assaultWeaponContainer = default;
		[SerializeField] private GameObject _meleeWeaponContainer = default;

		#endregion

		#region Private properties

		private float _dampTime = 0.1f;
		private const string IS_AIMING = "isAiming";
		private const string RELOAD = "reload";
		private const string IS_RELOADING = "isReloading";
		private const string FIRE = "fire";
		private const string IS_FIRING = "isFiring";
		private const string DEAD = "dead";
		private const string VELOCITY_X = "velocityX";
		private const string VELOCITY_Z = "velocityZ";

		#endregion

		#region Public properties

		public RuntimeAnimatorController AnimatorController => _animator.runtimeAnimatorController;

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

		public void SetAutoAiming(bool status)
		{
			_animator.SetBool(IS_AIMING, status);
		}

		public void OverrideAnimatorController(RuntimeAnimatorController animatorController)
		{
			_animator.runtimeAnimatorController = animatorController;
		}

		public void ApplyMelee()
		{
			_animator.SetTrigger(FIRE);
		}

		public void SetAssaultWeapon()
		{
			_assaultWeaponContainer.SetActive(true);
			_meleeWeaponContainer.SetActive(false);
		}

		public void SetMeleeWeapon()
		{
			_assaultWeaponContainer.SetActive(false);
			_meleeWeaponContainer.SetActive(true);
		}

		public void EnableRig()
		{
			var rig = _rigBuilder.layers[0].rig;

			StartCoroutine(RefreshRigWithDelay(rig, 1));
		}

		public void DisableRig()
		{
			var rig = _rigBuilder.layers[0].rig;

			StartCoroutine(RefreshRigWithDelay(rig, 0));
		}

		public void RefreshLocomotionDirection(Vector3 movementDirection)
		{
			var velocityZ = Vector3.Dot(movementDirection, transform.forward);
			var velocityX = Vector3.Dot(movementDirection, transform.right);

			_animator.SetFloat(VELOCITY_X, velocityX, _dampTime, Time.deltaTime);
			_animator.SetFloat(VELOCITY_Z, velocityZ, _dampTime, Time.deltaTime);
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

		private void StartReloading(float reloadingTime)
		{
			_animator.SetBool(IS_RELOADING, true);
			_animator.SetTrigger(RELOAD);
		}

		private void StopReloading()
		{
			_animator.SetBool(IS_RELOADING, false);
		}

		private void StartFiring(float fireRateTime)
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

		private IEnumerator RefreshRigWithDelay(Rig rig, float weight)
		{
			yield return new WaitForEndOfFrame();

			rig.weight = weight;
		}

		#endregion
	}
}