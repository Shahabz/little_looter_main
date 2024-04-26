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
		[SerializeField] private LayerMask _obstaclesLayer = default;

		[Header("Config")]
		[SerializeField] private int _initialHp = default;
		[SerializeField] private int _maxHp = default;
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
		private PlayerAutofireAssistance _autofireAssistance = default;

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
			_aimingAssistance.Init(transform, _aimingAssistanceAngle, _weaponController.WeaponRadiusDetection, _levelEnemies, _obstaclesLayer);

			_weaponController.Init(_aimingAssistance);

			// Repairing service
			var repairingService = GetComponent<PlayerRepairService>();
			repairingService.SetupSpeed(_repairingSpeed);

			// Stamina service
			var staminaService = GetComponent<PlayerStaminaService>();

			// Visual controller
			_visualController = GetComponent<VisualCharacterController>();
			_visualController.Init(_weaponController);

			// Controller
			_controller = GetComponent<ThirdPersonController>();
			_controller.SetAimingAssistance(_aimingAssistance);
			_controller.SetupRepairingService(repairingService);
			_controller.SetupStaminaService(staminaService);

			// Autofire assistance
			_autofireAssistance = new PlayerAutofireAssistance();
			_autofireAssistance.Init(_controller, _weaponController, _aimingAssistance);
			
			_controller.SetupAutofireAssistance(_autofireAssistance);

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
			_autofireAssistance.Teardown();
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

		private void TakeDamage(int damage)
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