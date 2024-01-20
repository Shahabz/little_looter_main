/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using StarterAssets;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerEntryPoint : MonoBehaviour
    {
		#region Inspector

		[Header("Level")]
		[SerializeField] private LevelEnemies _levelEnemies = default;

		[Header("Config")]
		[SerializeField] private float _initialHp = default;
		[SerializeField] private float _maxHp = default;
		[SerializeField] private bool _godMode = default;

		[Header("Aiming assistance")]
		[SerializeField] private float _aimingAssistanceAngle = default;
		[SerializeField] private float _aimingAssistanceRadius = default;

		[Header("Repairing")]
		[SerializeField] private float _repairingSpeed = 0.1f;

		#endregion

		#region Private properties

		private PlayerHealth _health = default;
		private WeaponController _weaponController = default;
		private VisualCharacterController _visualController = default;
		private ThirdPersonController _controller = default;
		private PlayerMissionsService _missionsService = default;
		private PlayerAimingAssistance _aimingAssistance = default;

		#endregion

		#region Public properties

		public PlayerHealth Health => _health;

		#endregion

		#region Unity events

		private void Awake()
		{
			// Health
			_health = GetComponent<PlayerHealth>();

			// Weapon controller
			_weaponController = GetComponent<WeaponController>();

			// Aiming assistance
			_aimingAssistance = new PlayerAimingAssistance();
			_aimingAssistance.Init(transform, _aimingAssistanceAngle, _weaponController.WeaponRadiusDetection, _levelEnemies);

			_weaponController.Init(_aimingAssistance);

			// Repairing service
			var repairingService = GetComponent<PlayerRepairService>();
			repairingService.SetupSpeed(_repairingSpeed);

			// Visual controller
			_visualController = GetComponent<VisualCharacterController>();
			_visualController.Init(_weaponController);

			// Controller
			_controller = GetComponent<ThirdPersonController>();
			_controller.SetAimingAssistance(_aimingAssistance);
			_controller.SetupRepairingService(repairingService);

			// Missions service
			_missionsService = GetComponent<PlayerMissionsService>();
		}

		private void Start()
		{
			_health.Init(_initialHp, _maxHp);

			_health.SetGodMode(_godMode);

			_health.OnTakeDamage += TakeDamage;
			_health.OnDead += Dead;

			var playerCraftingService = GetComponent<PlayerCraftingService>();

			_missionsService.Init(playerCraftingService);

			UI_GameplayEvents.OnPlayerInitialization?.Invoke();
		}

		private void OnDestroy()
		{
			_aimingAssistance.Teardown();
		}

		#endregion

		#region Public methods

		public MissionConfigurationData GetMissionInProgress()
		{
			return _missionsService.GetCurrentMission();
		}

		#endregion

		#region Missions

		public MissionConfigurationData GetFirstMissionData()
		{
			var playerMissionsService = GetComponent<PlayerMissionsService>();

			return playerMissionsService.GetFirstMissionData();
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