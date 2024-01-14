/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.General;
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
		[SerializeField] private ConfigurationMeleeLevelData[] _meleeConfigurationLevels = default;

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
		private const int TOOL_EXTRA_DAMAGE_DURATION_SECS = 10;

		#endregion

		#region Public properties

		public PlayerProgressData ProgressData => _progressData;
		public PlayerHealth Health => _health;

		#endregion

		#region Unity events

		private void Awake()
		{
			// Initialize melee data at first level
			InitializeMeleeData();

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
			_controller.SetupRepairingService(this, repairingService);

			// Missions service
			_missionsService = GetComponent<PlayerMissionsService>();

			UI_GameplayEvents.OnStartToolUpgrade += StartMeleeUpgrade;
			UI_GameplayEvents.OnClaimToolUpgrade += ClaimMeleeUpgrade;
			UI_GameplayEvents.OnSpeedUpToolUpgrade += SpeedUpToolUpgrade;
			UI_GameplayEvents.OnIncreaseToolDamage += HandleIncreaseToolDamage;

			UI_GameplayEvents.OnFixSlot += FixSlot;
			UI_GameplayEvents.OnStartRepairing += StartRepairing;
			UI_GameplayEvents.OnSpeedUpRepairing += SpeedUpRepairing;

			// Cheats
			UI_GameplayEvents.OnGrantResourceByCheat += HandleGrantResourceByCheat;
			UI_GameplayEvents.OnConsumeResourceByCheat += HandleConsumeResourceByCheat;

			// Missions
			PlayerMissionsEvents.OnMoveToMission += HandleMoveToMission;
		}

		private void Start()
		{
			_health.Init(_initialHp, _maxHp);

			_health.SetGodMode(_godMode);

			_health.OnTakeDamage += TakeDamage;
			_health.OnDead += Dead;

			var playerCraftingService = InitCrafting();

			_missionsService.Init(this, playerCraftingService);

			UI_GameplayEvents.OnPlayerInitialization?.Invoke();
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnStartToolUpgrade -= StartMeleeUpgrade;
			UI_GameplayEvents.OnClaimToolUpgrade -= ClaimMeleeUpgrade;
			UI_GameplayEvents.OnSpeedUpToolUpgrade -= SpeedUpToolUpgrade;
			UI_GameplayEvents.OnIncreaseToolDamage -= HandleIncreaseToolDamage;

			// Tool
			UI_GameplayEvents.OnFixSlot -= FixSlot;
			UI_GameplayEvents.OnStartRepairing -= StartRepairing;
			UI_GameplayEvents.OnSpeedUpRepairing -= SpeedUpRepairing;

			// Cheats
			UI_GameplayEvents.OnGrantResourceByCheat -= HandleGrantResourceByCheat;
			UI_GameplayEvents.OnConsumeResourceByCheat -= HandleConsumeResourceByCheat;

			// Missions
			PlayerMissionsEvents.OnMoveToMission -= HandleMoveToMission;

			_aimingAssistance.Teardown();
		}

		#endregion

		#region Public methods

		public void SetupRepairObjects(RepairObject[] repairObjects)
		{
			_progressData.InitRepairProgress(repairObjects);
		}

		//public void AddPartsToRepairObject(int objectId, int partId, int partAmount)
		//{
		//	_progressData.AddPartsToRepairObject(objectId, partId, partAmount);
		//}

		public void GrantResourceByDestructionDamage(int resourceId, int amountReward)
		{
			_progressData.GrantResourceAmount(resourceId, amountReward);
		}

		public void GrantResourceByPickup(int resourceId, int amount)
		{
			_progressData.GrantResourceAmount(resourceId, amount);
		}

		public ConfigurationMeleeLevelData GetCurrentToolLevelData()
		{
			var currentLevel = _progressData.meleeData.level;
			var levelData = _meleeConfigurationLevels[0];

			for (int i = 0; i < _meleeConfigurationLevels.Length; i++)
			{
				levelData = _meleeConfigurationLevels[i];

				if (levelData.level != currentLevel) continue;

				break;
			}

			return levelData;
		}

		public ConfigurationMeleeLevelData GetMeleeNextLevelData()
		{
			var currentLevel = _progressData.meleeData.level;
			var nextLevelData = _meleeConfigurationLevels[0];

			for (int i = 0; i < _meleeConfigurationLevels.Length; i++)
			{
				nextLevelData = _meleeConfigurationLevels[i];

				if (nextLevelData.level != currentLevel + 1) continue;

				break;
			}

			return nextLevelData;
		}

		public void CompleteMeleeUpgrade()
		{
			_progressData.CompleteMeleeUpgrade();
		}

		public MissionConfigurationData GetMissionInProgress()
		{
			return _missionsService.GetCurrentMission();
		}

		public void CompleteRepairing(int objectId)
		{
			_progressData.CompleteRepairing(objectId);
		}

		public void CompleteToolExtraDamage()
		{
			var toolLevelData = GetCurrentToolLevelData();

			_progressData.CompleteIncreaseToolDamage(toolLevelData);
		}

		#endregion

		#region Crafting

		public void CraftingStartProcess(CraftingConfigurationData data, int amount)
		{
			_progressData.CraftingStartProcess(data, amount);
		}

		public void CraftingCompleteProcess(int areaId)
		{
			_progressData.CraftingCompleteProcess(areaId);
		}

		public int CraftingClaimProcess(int resourceId, int areaId)
		{
			var amountObtained = _progressData.CraftingClaimProcess(areaId, resourceId);

			return amountObtained;
		}

		public void CraftingSpeedUpProcess(int areaId, int secondsToSkip)
		{
			var now = Time.time;

			_progressData.CraftingSpeedUpProcess(areaId, secondsToSkip, now);
		}

		#endregion

		#region Missions

		public MissionConfigurationData GetFirstMissionData()
		{
			var playerMissionsService = GetComponent<PlayerMissionsService>();

			return playerMissionsService.GetFirstMissionData();
		}

		private void HandleMoveToMission(MissionConfigurationData missionData)
		{
			_progressData.UpdateCurrentMission(missionData.Id);
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

		private void InitializeMeleeData()
		{
			_progressData.SetMeleeData(_meleeConfigurationLevels[0]);
		}

		private void StartMeleeUpgrade()
		{
			var nextLevelData = GetMeleeNextLevelData();
			var now = Time.time;
			var duration = nextLevelData.upgradeTime;
			var expiration = now + duration;

			_progressData.StartMeleeUpgrade(duration, expiration, nextLevelData.requirements, nextLevelData.damage);
		}

		private void ClaimMeleeUpgrade()
		{
			var nextLevelData = GetMeleeNextLevelData();

			_progressData.ClaimMeleeUpgrade(nextLevelData.damage);
		}

		private void SpeedUpToolUpgrade()
		{
			_progressData.SpeedUpToolUpgrade();
		}

		private void HandleIncreaseToolDamage()
		{
			var now = Time.time;
			var duration = TOOL_EXTRA_DAMAGE_DURATION_SECS;

			_progressData.IncreaseToolDamage(now, duration);
		}

		private void FixSlot(int objectId, int resourceId)
		{
			_progressData.Fix(objectId, resourceId);
		}

		private void StartRepairing(int id)
		{
			var now = Time.time;

			_progressData.StartRepairing(id, now);
		}

		private void SpeedUpRepairing(int objectId)
		{
			_progressData.SpeedUpRepairing(objectId);
		}

		private void HandleGrantResourceByCheat(UI_GameplayEvents.UpdateResourceByCheatArgs args)
		{
			_progressData.GrantResourceAmount(args.resourceId, args.amount);
		}

		private void HandleConsumeResourceByCheat(UI_GameplayEvents.UpdateResourceByCheatArgs args)
		{
			_progressData.ConsumeResourceAmount(args.resourceId, args.amount);
		}

		private PlayerCraftingService InitCrafting()
		{
			_progressData.InitCrafting();

			// Crafting service
			var craftingService = GetComponent<PlayerCraftingService>();
			craftingService.Initialization(this);

			return craftingService;
		}

		#endregion
	}
}