/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.Model;
using StarterAssets;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerEntryPoint : MonoBehaviour
    {
		#region Inspector

		[Header("Progress data")]
		[SerializeField] private PlayerProgressData _progressData = default;

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

		//private PlayerProgressData _progressData = default;
		private PlayerHealth _health = default;
		private WeaponController _weaponController = default;
		private VisualCharacterController _visualController = default;
		private ThirdPersonController _controller = default;

		#endregion

		#region Public properties

		public PlayerProgressData ProgressData => _progressData;

		#endregion

		#region Unity events

		private void Awake()
		{
			//_progressData = new PlayerProgressData();

			// Health
			_health = GetComponent<PlayerHealth>();

			// Aiming assistance
			var aimingAssistance = new PlayerAimingAssistance();
			aimingAssistance.Init(transform, _aimingAssistanceAngle, _aimingAssistanceRadius, _levelEnemies);

			// Repairing service
			var repairingService = GetComponent<PlayerRepairService>();
			repairingService.SetupSpeed(_repairingSpeed);

			// Weapon controller
			_weaponController = GetComponent<WeaponController>();
			_weaponController.Init(aimingAssistance);

			// Visual controller
			_visualController = GetComponent<VisualCharacterController>();
			_visualController.Init(_weaponController);

			// Controller
			_controller = GetComponent<ThirdPersonController>();
			_controller.SetAimingAssistance(aimingAssistance);
			_controller.SetupRepairingService(this, repairingService);
		}

		private void Start()
		{
			_health.Init(_initialHp, _maxHp);

			_health.SetGodMode(_godMode);

			_health.OnTakeDamage += TakeDamage;
			_health.OnDead += Dead;

			UI_GameplayEvents.OnPlayerInitialization?.Invoke();
		}

		#endregion

		#region Public methods

		public void SetupRepairObjects(RepairObject[] repairObjects)
		{
			_progressData.SetupRepairObjects(repairObjects);
		}

		public void AddPartsToRepairObject(int objectId, int partId, int partAmount)
		{
			_progressData.AddPartsToRepairObject(objectId, partId, partAmount);
		}

		public void GrantResourceByDestructionDamage(int resourceId, int amountReward)
		{
			_progressData.GrantResourceAmount(resourceId, amountReward);
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