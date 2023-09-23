/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using StarterAssets;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerEntryPoint : MonoBehaviour
    {
		#region Inspector

		[Header("Config")]
		[SerializeField] private float _initialHp = default;
		[SerializeField] private float _maxHp = default;

		#endregion

		#region Private properties

		private PlayerHealth _health = default;
		private WeaponController _weaponController = default;
		private VisualCharacterController _visualController = default;
		private ThirdPersonController _controller = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			// Health
			_health = GetComponent<PlayerHealth>();

			// Weapon controller
			_weaponController = GetComponent<WeaponController>();
			_weaponController.Init();

			// Visual controller
			_visualController = GetComponent<VisualCharacterController>();
			_visualController.Init(_weaponController);

			// Controller
			_controller = GetComponent<ThirdPersonController>();
		}

		private void Start()
		{
			_health.Init(_initialHp, _maxHp);
			_health.OnTakeDamage += TakeDamage;
			_health.OnDead += Dead;
		}

		#endregion

		#region Private methods

		private void TakeDamage(float damage)
		{
			_controller.TakeDamage(damage);
		}

		private void Dead()
		{
			_controller.Dead();
		}

		#endregion
	}
}